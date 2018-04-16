using UnityEngine;
using System;
using System.Collections.Generic;

public enum TileAttrib
{
	Stand=0,
	Block,
	Foundation
}


public class Point
{
	public int X=0;
	public int Y=0;
	
	public Point(int x,int y)
	{
		X=x;
		Y=y;
	}
}

public class TileBlock
{
	public  int x;
	public  int y;

	public  bool  IsPath;

	public  int   DistanceSteps = 20000;

	public  TileAttrib  Attrib = TileAttrib.Stand;

	public TileBlock(int Cx,int Cy,TileAttrib Ta)
	{
		x=Cx;
		y=Cy;
		
		IsPath = false;
		DistanceSteps = 20000;
		Attrib=Ta;
	}
}

namespace PF
{
    public class Pathfinder
    {
        public int MapX = 0;
        public int MapY = 0;

        public Vector3 Center;

        int EX = 0;
        int EY = 0;

        int DirCount = 8;
        int PointCount = 0;
        Point[] _movements;
        Point[] _movePoints;
        TileBlock[] MapTile;

        Vector3 MoveMent;
        Vector3 NextPos;
        Vector3 NextStandPos;

        int CurrentPathStep = -1;
        bool IsFindPath = false;

        MeshCollider CollisionGround;

        void Clean()
        {
            MapX = 0;
            MapY = 0;

            EX = 0;
            EY = 0;
            PointCount = 0;

            if (_movements != null)
            {
                for (int i = 0; i < _movements.Length; i++)
                {
                    _movements[i] = null;
                }
                _movements = null;
            }

            if (_movePoints != null)
            {
                for (int i = 0; i < _movePoints.Length; i++)
                {
                    _movePoints[i] = null;
                }
                _movePoints = null;
            }

            if (MapTile != null) { MapTile = null; }
        }

        int GetBlockXID(float x)
        {
            int xid = (int)(x - Center.x) + MapX / 2;
            if ((xid >= 0) && (xid < MapX))
            {
                return xid;
            }
            return -1;
        }

        int GetBlockYID(float z)
        {
            int yid = (int)(z - Center.z) + MapY / 2;
            if ((yid >= 0) && (yid < MapY))
            {
                return yid;
            }
            return -1;
        }

        int GetBlockID(Vector3 Pos)
        {
            int XID = GetBlockXID(Pos.x);
            int YID = GetBlockYID(Pos.z);

            if ((XID >= 0) && (YID >= 0) && (XID < MapX) && (YID < MapY))
            {
                return YID * MapX + XID;
            }

            return -1;
        }

        Vector3 GetPosFromID(int XID, int YID)
        {
            if (YID * MapX + XID >= MapTile.Length) { return new Vector3(0, 0, 0); }
            if (YID * MapX + XID < 0) { return new Vector3(0, 0, 0); }
            return new Vector3((float)XID - (float)MapX * 0.5f + Center.x, 0.0f, (float)YID - (float)MapY * 0.5f + Center.z);
        }

        Vector2 GetPosFromSID(int ID)
        {
            if (ID >= MapTile.Length) { return new Vector3(0, 0, 0); }
            if (ID < 0) { return new Vector3(0, 0, 0); }

            return new Vector3((float)MapTile[ID].x - (float)MapX * 0.5f + Center.x, (float)MapTile[ID].y - (float)MapY * 0.5f + Center.z);
        }

        void SetTileCollision(int XID0, int YID0)
        {
            RaycastHit hitinfo;
            Ray PosRay = new Ray();
            PosRay.origin = GetPosFromID(XID0, YID0);
            PosRay.origin = new Vector3(PosRay.origin.x, 100.0f, PosRay.origin.z);
            PosRay.direction = new Vector3(0, -1000.0f, 0);

            if (CollisionGround.Raycast(PosRay, out hitinfo, 200.0f) == true)
            {
                MapTile[YID0 * MapX + XID0].Attrib = TileAttrib.Stand;
            }
        }

        TileBlock GetBlock(Vector3 PickPos)
        {
            if ((MapX > 0) && (MapY > 0))
            {
                if ((PickPos.x < Center.x + MapX * 0.5f)
                 && (PickPos.z < Center.z + MapY * 0.5f)
                 && (PickPos.x > Center.x - MapX * 0.5f)
                 && (PickPos.z > Center.z - MapY * 0.5f))
                {
                    int XID = GetBlockXID(PickPos.x);
                    int YID = GetBlockYID(PickPos.z);

                    if ((XID >= 0) && (YID >= 0) && (XID < MapX) && (YID < MapY))
                    {
                        return MapTile[YID * MapX + XID];
                    }
                }
            }
            return null;
        }

        TileAttrib GetBlockAttrib(Vector3 PickPos)
        {
            TileBlock TB = GetBlock(PickPos);
            if (TB != null)
            {
                return TB.Attrib;
            }

            return TileAttrib.Block;
        }

        public void init(MeshCollider CollisionGroundObj)
        {
            Clean();
            CollisionGround = CollisionGroundObj;
            Vector3 Min = CollisionGround.sharedMesh.bounds.min;
            Vector3 Max = CollisionGround.sharedMesh.bounds.max;
            MapX = ((int)((Max.x - Min.x) * 0.5f) + 2) * 2;
            MapY = ((int)((Max.z - Min.z) * 0.5f) + 2) * 2;

            Center = CollisionGround.sharedMesh.bounds.center;

            MapTile = new TileBlock[MapY * MapX];

            for (int y = 0; y < MapY; y++)
            {
                for (int x = 0; x < MapX; x++)
                {
                    MapTile[y * MapX + x] = new TileBlock(x, y, TileAttrib.Block);
                    SetTileCollision(x, y);
                }
            }
            InitMovements(DirCount);
        }

        public void StartFindPath(GameObject Player, Vector2 ScenePoint)
        {
            RaycastHit hitinfo;
            if (CollisionGround.Raycast(Camera.main.ScreenPointToRay(ScenePoint), out hitinfo, 200.0f) == true)
            {
                int XID = GetBlockXID(hitinfo.point.x);
                int YID = GetBlockYID(hitinfo.point.z);

                if ((XID >= 0) && (YID >= 0) && (XID < MapX) && (YID < MapY))
                {
                    if (MapTile[YID * MapX + XID].Attrib != TileAttrib.Block)
                    {
                        NextStandPos = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z);//cause charactor point under the foot

                        int CXID = GetBlockXID(NextStandPos.x);
                        int CYID = GetBlockYID(NextStandPos.z);

                        if ((CXID >= 0) && (CYID >= 0) && (CXID < MapX) && (CYID < MapY))
                        {
                            FindPath(CXID, CYID, XID, YID);

                            CurrentPathStep = GetNextStep(CYID * MapX + CXID);
                            if (CurrentPathStep >= 0)
                            {
                                IsFindPath = true;
                            }
                        }
                    }
                }
            }
        }

        public void FindPathUpdate(GameObject Player, Vector3 VJRnormals, float MoveSpeed)
        {
            if (IsFindPath == true)
            {
                if (Mathf.Abs(VJRnormals.x) > 0.1f || Mathf.Abs(VJRnormals.y) > 0.1f)
                {
                    IsFindPath = false;
                }
            }

            if (IsFindPath == true)
            {
                if (Mathf.Abs(VJRnormals.x) > 0.1f || Mathf.Abs(VJRnormals.y) > 0.1f)
                {
                    IsFindPath = false;
                }

                Vector2 BPos = GetPosFromSID(CurrentPathStep);
                float fdist = Vector2.Distance(BPos, new Vector2(Player.transform.position.x, Player.transform.position.z));

                if (fdist < MoveSpeed * Time.deltaTime)
                {
                    Player.transform.position = new Vector3(BPos.x, Player.transform.position.y, BPos.y);

                    int CID = GetBlockID(Player.transform.position);
                    CurrentPathStep = GetNextStep(CID);

                    if (CurrentPathStep == -1)
                    {
                        IsFindPath = false;
                    }
                }
                else
                {
                    Vector2 Director = new Vector2(BPos.x - Player.transform.position.x, BPos.y - Player.transform.position.z);
                    Director.Normalize();
                    MoveMent = new Vector3(MoveSpeed * Director.x * Time.deltaTime, 0, MoveSpeed * Director.y * Time.deltaTime);
                    NextPos = new Vector3(Player.transform.position.x + MoveMent.x, Player.transform.position.y, Player.transform.position.z + MoveMent.z);
                    Player.transform.position = NextPos;
                }
            }
            else
            {
                MoveMent = new Vector3(MoveSpeed * VJRnormals.x * Time.deltaTime, 0, MoveSpeed * VJRnormals.y * Time.deltaTime);
                MoveMent = new Vector3(MoveMent.x * Mathf.Cos(Mathf.Deg2Rad * -90.0f) + MoveMent.z * Mathf.Sin(Mathf.Deg2Rad * -90.0f), 0, MoveMent.z * Mathf.Cos(Mathf.Deg2Rad * -90.0f) - MoveMent.x * Mathf.Sin(Mathf.Deg2Rad * -90.0f));
                NextPos = new Vector3(Player.transform.position.x + MoveMent.x, Player.transform.position.y, Player.transform.position.z + MoveMent.z);
                NextStandPos = new Vector3(Player.transform.position.x + MoveMent.x * MoveSpeed, Player.transform.position.y, Player.transform.position.z + MoveMent.z * MoveSpeed);

                if (GetBlockAttrib(NextStandPos) == TileAttrib.Stand)
                {
                    Player.transform.position = NextPos;
                }
                else
                {
                    NextPos = new Vector3(Player.transform.position.x + MoveMent.x, Player.transform.position.y, Player.transform.position.z);
                    NextStandPos = new Vector3(Player.transform.position.x + MoveMent.x * MoveSpeed, Player.transform.position.y, Player.transform.position.z);

                    if (GetBlockAttrib(NextStandPos) == TileAttrib.Stand)
                    {
                        Player.transform.position = NextPos;
                    }
                    else
                    {
                        NextPos = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z + MoveMent.z);
                        NextStandPos = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z + MoveMent.z * MoveSpeed);
                        if (GetBlockAttrib(NextStandPos) == TileAttrib.Stand)
                        {
                            Player.transform.position = NextPos;
                        }
                    }
                }
            }
        }

        void InitMovements(int movementCount)
        {
            if (movementCount == 4)
            {
                _movements = new Point[]
                {
                    new Point(0, -1),
                    new Point(1, 0),
                    new Point(0, 1),
                    new Point(-1, 0)
                };

                _movePoints = new Point[]
                {
                    new Point(0, 0),
                    new Point(0, 0),
                    new Point(0, 0),
                    new Point(0, 0)
                };
            }
            else
            {
                _movements = new Point[]
                {
                    new Point(-1, -1),
				    new Point(1,  -1),
				    new Point(1,   1),
				    new Point(-1,  1),
                    new Point(0,  -1),
                    new Point(1,   0),
                    new Point(0,   1),
                    new Point(-1,  0)
                };

                _movePoints = new Point[]
                {
                    new Point(0, 0),
                    new Point(0, 0),
                    new Point(0, 0),
                    new Point(0, 0),
                    new Point(0, 0),
                    new Point(0, 0),
                    new Point(0, 0),
                    new Point(0, 0)
                };
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void FindPath(int StartX, int StartY, int EndX, int EndY)
        {
            if (StartX <= -1 || StartY <= -1 || StartX >= MapX || StartY >= MapY || EndX <= -1 || EndY <= -1 || EndX >= MapX || EndY >= MapY) { return; }
            //SX=StartX;
            //SY=StartY;
            EX = EndX;
            EY = EndY;

            for (int x = 0; x < MapX; x++)
            {
                for (int y = 0; y < MapY; y++)
                {
                    MapTile[y * MapX + x].IsPath = false;
                    MapTile[y * MapX + x].DistanceSteps = 20000;
                }
            }

            MapTile[StartY * MapX + StartX].DistanceSteps = 0;
            MapTile[StartY * MapX + StartX].IsPath = true;
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            int ccount = 0;
            while (true)
            {
                bool madeProgress = false;
                for (int i = 0; i < MapTile.Length; i++)
                {
                    if (MapTile[i].DistanceSteps != 20000)
                    {
                        if (SquareOpen(MapTile[i].x, MapTile[i].y))
                        {
                            ccount += 1;
                            int passHere = MapTile[i].DistanceSteps;
                            ValidMoves(MapTile[i].x, MapTile[i].y);

                            for (int k = 0; k < PointCount; k++)
                            {
                                int newX = _movePoints[k].X;
                                int newY = _movePoints[k].Y;

                                int newPass = passHere + 1;

                                if (MapTile[newY * MapX + newX].DistanceSteps > newPass)
                                {
                                    MapTile[newY * MapX + newX].DistanceSteps = newPass;
                                    madeProgress = true;
                                }
                            }
                        }
                    }
                }

                if ((madeProgress == false) || (ccount >= MapX * MapTile.Length))
                {
                    break;
                }
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            int pointX = EndX;
            int pointY = EndY;

            if (MapTile[pointY * MapX + pointX].Attrib != TileAttrib.Foundation)
            {
                MapTile[pointY * MapX + pointX].IsPath = true;
            }

            while (true)
            {
                Point lowestPoint = new Point(0, 0);
                int lowest = 20000;

                ValidMoves(pointX, pointY);

                for (int k = 0; k < PointCount; k++)
                {
                    int count = MapTile[_movePoints[k].Y * MapX + _movePoints[k].X].DistanceSteps;

                    if (count < lowest)
                    {
                        lowest = count;
                        lowestPoint.X = _movePoints[k].X;
                        lowestPoint.Y = _movePoints[k].Y;
                    }
                }

                if (lowest != 20000)
                {
                    MapTile[lowestPoint.Y * MapX + lowestPoint.X].IsPath = true;
                    pointX = lowestPoint.X;
                    pointY = lowestPoint.Y;
                }
                else
                {
                    break;
                }

                if ((pointX == StartX) && (pointY == StartY))
                {
                    break;
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        int GetNextStep(int CID)
        {
            int newID = -1;

            int Distance = 20000;

            MapTile[CID].IsPath = false;

            if ((MapTile[CID].x == EX) && (MapTile[CID].y == EY))
            {
                return -1;
            }


            foreach (Point movePoint in _movements)
            {
                if (MapTile[(MapTile[CID].y + movePoint.Y) * MapX + (MapTile[CID].x + movePoint.X)].IsPath == true)
                {
                    if (MapTile[(MapTile[CID].y + movePoint.Y) * MapX + (MapTile[CID].x + movePoint.X)].DistanceSteps < Distance)
                    {
                        Distance = MapTile[(MapTile[CID].y + movePoint.Y) * MapX + (MapTile[CID].x + movePoint.X)].DistanceSteps;
                        newID = (MapTile[CID].y + movePoint.Y) * MapX + (MapTile[CID].x + movePoint.X);
                    }
                }
            }

            return newID;
        }

        bool ValidCoordinates(int x, int y)
        {
            if (x < 0)
            {
                return false;
            }
            if (y < 0)
            {
                return false;
            }
            if (x > MapX - 1)
            {
                return false;
            }
            if (y > MapY - 1)
            {
                return false;
            }
            return true;
        }

        bool SquareOpen(int x, int y)
        {
            switch (MapTile[y * MapX + x].Attrib)
            {
                case TileAttrib.Stand:
                    return true;

                case TileAttrib.Block:
                    return false;

                case TileAttrib.Foundation:
                    return false;

            }
            return false;
        }

        Point FindCode(TileAttrib contentIn)
        {
            for (int i = 0; i < MapTile.Length; i++)
            {
                if (MapTile[i].Attrib == contentIn)
                {
                    return new Point(MapTile[i].x, MapTile[i].y);
                }
            }
            return new Point(-1, -1);
        }

        void ValidMoves(int x, int y)
        {
            PointCount = 0;

            foreach (Point movePoint in _movements)
            {
                int newX = x + movePoint.X;
                int newY = y + movePoint.Y;

                if (ValidCoordinates(newX, newY) && SquareOpen(newX, newY))
                {
                    _movePoints[PointCount] = new Point(newX, newY);
                    PointCount += 1;
                }
            }
        }
    }
}