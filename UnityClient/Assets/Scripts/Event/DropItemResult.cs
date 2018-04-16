using System;
using System.Collections.Generic;
using UnityEngine;

public class DropItemResult : EResultBase
{
    public enum ID
    {
        smallPill = 1,          //0.50%生命恢复血球
        largePill = 2,             //1.100%生命恢复血球
        key = 3,               //2.钥匙
        magicRunes = 4,        //3.魔法符文
    }

    public struct DropItemData
    {
        public int mDropItemId;
        public int mDropItemCount;
        public int mDropKeyId;
        public SM.TrapData mTrapData;
    }
    List<DropItemData> mDropItemDataList = new List<DropItemData>();
    public DropItemResult()
    {
        ResultTypeId = EResultManager.ENResultTypeID.enDropItem;
        IsTicked = true;
        IsEnabled = false;
    }
    public override void Tick()
    {
        base.Tick();

        if (IsEnabled)
        {

        }
    }
    public override bool Execute()
    {
        SM.SceneRoom room = SM.RandomRoomLevel.Singleton.LookupRoom(RoomGUID);
        Actor eventActor = BattleArena.Singleton.m_blackBoard.GetBlackBoardActor("DropItemActor");
        foreach (var dropItemData in mDropItemDataList)
        {
                    //掉落Func
            if ((ID)dropItemData.mDropItemId == ID.key)
            {
                DropItemPerformanceType itemType = new DropItemPerformanceType();
                itemType.m_intParam = (int)dropItemData.mDropItemId;
                itemType.m_deadPos = new Vector3(eventActor.MainObj.transform.position.x, eventActor.MainObj.transform.position.y, eventActor.MainObj.transform.position.z);
                itemType.m_eulerAngles = new Vector3(eventActor.MainObj.transform.localEulerAngles.x, eventActor.MainObj.transform.localEulerAngles.y, eventActor.MainObj.transform.localEulerAngles.z);
                MainGame.Singleton.StartCoroutine(DropItemPerformance.Singleton.LoadDropItem(itemType));
                BattleArena.Singleton.KeyCount += 1;
            }
            else
            {
//                 dropItemData.mTrapData.position = new Vector3(eventActor.MainObj.transform.position.x, eventActor.MainObj.transform.position.y, eventActor.MainObj.transform.position.z);
//                 SM.TrapRefresh.SpawnMe(room, dropItemData.mTrapData);
            }
        }
        return true;
    }

    public override void ParseJsonData(LitJson.JsonData data)
    {
        base.ParseJsonData(data);
        LitJson.JsonData dropDataList = data[EResultManager.sItemList];
        for (int i = 0; i < dropDataList.Count; i++)
        {
            DropItemData tmpData = new DropItemData();
            tmpData.mDropItemId = int.Parse(dropDataList[i][EResultManager.sItemType].ToString());
            tmpData.mDropKeyId = int.Parse(dropDataList[i][EResultManager.sKeyID].ToString());
            //tmpData.mTrapData = new SM.TrapData();
            //tmpData.mDropItemCount = int.Parse(dropDataList[i][EResultManager.sItemCount].ToString());
            mDropItemDataList.Add(tmpData);
        }
    }
}