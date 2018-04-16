using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostFollow : MonoBehaviour {

    struct TransformData 
    {
        public Vector3 localScale;
        public Vector3 localPosition;
        public Quaternion localRotation;
    }

    class BoneData
    {
        public Transform m_self;
        public Transform m_follow;
        public List<TransformData> m_datas = new List<TransformData>();
    }
    public Transform m_selfRootBone;
	GhostFollow m_nextGhost;
    //public List<Material> m_materials;
    Transform m_followRootBone;
    Transform m_mainObj;
    List<BoneData> m_boneList = new List<BoneData>();
    List<TransformData> m_mainObjTransforms = new List<TransformData>();
    
    int m_delayKeyCount = 30;
    //ghost loop time
	float m_loopTime;
	//残影消失的时间
	float m_ghostDispearTime;
    //残影保留的是一个transform的列表，这里表示当前显示在列表的第几祯
    int m_curShowGhostKeyIndex = 0;
    //残影保留的是一个transform的列表，这里表示当前捕捉在列表的第几祯
    int m_curCaptureGhostKeyIndex = 0;

    public bool IsDispeard { get { return Time.time > m_ghostDispearTime; } }

    public void Init(Transform followRootBone,Transform mainObj,int index,GhostFollow nextGhost)
    {
		m_nextGhost = nextGhost;
        AutoChangeMaterial(index);

        int delay = 3;
        float timescale = 1.0f;
        if (Application.targetFrameRate >= 60)
        {
            timescale = 2.0f;
        }
        else if (Application.targetFrameRate >= 45)
        {
            timescale = 1.5f;
        }
        else if (Application.targetFrameRate >= 30)
        {
            timescale = 1.0f;
        }
        m_followRootBone = followRootBone;
        m_mainObj = mainObj;
//        m_nextCaptureTime = Time.time + 3 + index * m_autoCaptureTime;
		m_delayKeyCount = (int)(delay * (index + 1)*timescale);//dex * delay + index * index*2) * timescale);

        for (int i = 0; i < m_delayKeyCount; i++)
        {
            TransformData td;
            td.localRotation = Quaternion.identity;
            td.localPosition = Vector3.zero;
            td.localScale = Vector3.one;
            m_mainObjTransforms.Add(td);
        }
        Init_(m_selfRootBone, m_followRootBone);
    }
    void AutoChangeMaterial(int index)
    {
        Renderer[] rs = gameObject.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < rs.Length;i++ )
        {
            Renderer r = rs[i];
            MaterialHolder mh = r.gameObject.GetComponent<MaterialHolder>();
            if (mh != null)
            {
                r.material = mh.m_materials[index % mh.m_materials.Count];
            }
        }
    }
    void Init_(Transform self, Transform followObj)
    {
        BoneData d=new BoneData();
        d.m_self = self;
        d.m_follow = followObj;

        for (int i = 0; i < m_delayKeyCount; i++)
        {
            TransformData td;
            td.localRotation = Quaternion.identity;
            td.localPosition = Vector3.zero;
            td.localScale = Vector3.one;
            d.m_datas.Add(td);
        }

        m_boneList.Add(d);

        //find child
        for (int i = 0; i < followObj.childCount;i++ )
        {
            Transform folChild = followObj.GetChild(i);
            Transform selfChild = self.Find(folChild.name);
            if (selfChild != null)
            {
                Init_(selfChild, folChild);
            }
        }
    }

    void CaptureGhostData(int indexInput)
    {
        int index = indexInput % m_mainObjTransforms.Count;
        TransformData td;

        td.localPosition = m_mainObj.localPosition;
        td.localRotation = m_mainObj.localRotation;
        td.localScale = m_mainObj.localScale;
        m_mainObjTransforms[index] = td;
        for (int i = 0; i < m_boneList.Count; i++)
        {
            BoneData d = m_boneList[i];
            td.localPosition = d.m_follow.localPosition;
            td.localRotation = d.m_follow.localRotation;
            td.localScale = d.m_follow.localScale;
            d.m_datas[index] = td;
        }
    }

    public void CaptureGhost(float loopSecond)
    {
		if (m_mainObj == null)
		{
			return;
		}
		m_loopTime = loopSecond;
		m_ghostDispearTime = Time.time + m_loopTime;
		
		if(m_nextGhost != null)
		{
			m_nextGhost.CaptureGhost(m_loopTime);
		}
		if (gameObject.activeSelf) 
		{//self is active
		}
		else
		{
			m_curCaptureGhostKeyIndex=0;
			m_curShowGhostKeyIndex=0;

			CaptureGhostData(0);
			m_curCaptureGhostKeyIndex++;
			MoveGhost(0);
			gameObject.SetActive(true);
		}
    }
    void MoveGhost(int indexInput)
    {
        int index = indexInput % m_mainObjTransforms.Count;
        TransformData td=m_mainObjTransforms[index];
        gameObject.transform.localPosition = td.localPosition;
        gameObject.transform.localRotation = td.localRotation;
        gameObject.transform.localScale = td.localScale;
        for (int i = 0; i < m_boneList.Count; i++)
        {
            BoneData d = m_boneList[i];
            td = d.m_datas[index];
            d.m_self.localPosition = td.localPosition;
            d.m_self.localRotation = td.localRotation;
            d.m_self.localScale = td.localScale;
        }
    }
	// Use this for initialization
	void Start () {
        gameObject.SetActive(false);
	}
//    float m_nextCaptureTime=0;
    //float m_autoCaptureTime = 1.0f;
	// Update is called once per frame
	void LateUpdate () {
		if (IsDispeard && m_curShowGhostKeyIndex==m_curCaptureGhostKeyIndex)
        {
            gameObject.SetActive(false);
        }
        if (m_mainObj == null)
        {
            return;
        }
		if(!IsDispeard)
		{
        	CaptureGhostData(m_curCaptureGhostKeyIndex);
        	m_curCaptureGhostKeyIndex++;
		}
        if (m_curCaptureGhostKeyIndex>m_mainObjTransforms.Count/2 || IsDispeard)
        {
            MoveGhost(m_curShowGhostKeyIndex);
            m_curShowGhostKeyIndex++;
			if(m_nextGhost && m_nextGhost.gameObject.activeSelf == false)
			{
				m_nextGhost.CaptureGhost(m_loopTime);
			}
        }
        return;
        //if (Time.time > m_nextCaptureTime )
        //{
        //    CaptureGhost(1.0f);
        //    float t = UnityEngine.Random.RandomRange(0.5f, 1.5f);
        //    //t = m_autoCaptureTime;
        //    m_nextCaptureTime = Time.time + t;
        //    //Debug.Log("GhostFollow:obj=" + gameObject.GetInstanceID()+" Time:"+m_nextCaptureTime);
        //}
	}
}
