using UnityEngine;
using System.Collections;

public class DropHolderTrigger : IWidget
{
	short mDropID;  // ��Ҫ������������
	short mItemID;
    int m_type = 0;
    float m_posX = 0;
    float m_posZ = 0;
	private GameObject m_obj;
	private float m_startTime = 0.0f;

	public void CreateDropObj(int nType, short nDropID, short itmID, float fx, float fz)
	{
		mDropID = nDropID;
		mItemID = itmID;
        m_type = nType;
        m_posX = fx;
        m_posZ = fz;

        MainGame.Singleton.StartCoroutine(Coroutine_Load());
    }
    IEnumerator Coroutine_Load()
    {
        if (m_obj == null)
        {
            GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
            IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetUIPath("DropItem"), data, true);
            while (true)
            {
                e.MoveNext();
                if (data.m_isFinish)
                {
                    break;
                }
                yield return e.Current;
            }
            m_obj = data.m_obj as GameObject;
        }
        IsEnable = true;
        m_obj.transform.localScale = new UnityEngine.Vector3(0.2f, 0.2f, 0.2f);
        m_obj.transform.localPosition = new Vector3(m_posX, 0.0f, m_posZ);

        GameObject item = m_obj.transform.Find("DropItmSupply").gameObject;

        GameObject name = m_obj.transform.Find("ItemName").gameObject;

        int nIcnID = 0;
        string NAME = "";
        int color = 1;
        if (1 == m_type) // ����: װ��(��)
        {
            EquipInfo einf = GameTable.EquipTableAsset.LookUpEquipInfoById(mItemID);
            if (einf != null)
            {
                int nMdlInfID = einf.ModelRes;  // ���Ҷ�Ӧ��ʾͼ��
                ModelInfo MdlInf = GameTable.ModelInfoTableAsset.Lookup(nMdlInfID);
                if (MdlInf != null) nIcnID = MdlInf.DropIcon;
            }
            NAME = einf.EquipName;
            color = einf.EqRank;
        }
        else if (2 == m_type) // ����: Buf(��)
        {
            BuffInfo bfInfo = GameTable.BuffTableAsset.Lookup(mItemID);
            if (null != bfInfo) nIcnID = bfInfo.BuffIcon;
            else Debug.LogWarning("CreateDropObj: LOST Buf ItemID = " + mItemID.ToString());
            NAME = bfInfo.BuffName;
        }
        else if (3 == m_type) // ��Ǯ
        {
            WorldParamInfo WorldParamList;
            WorldParamList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enMoneyDropIcon);
            nIcnID = WorldParamList.IntTypeValue;
            NAME = GameTable.StringTableAsset.GetString(ENStringIndex.UIEquipmentBindMoney);
        }
        else if (4 == m_type) // ����
        {
            WorldParamInfo WorldParamList;
            WorldParamList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enRMBMoneyDropIcon);
            nIcnID = WorldParamList.IntTypeValue;
            NAME = GameTable.StringTableAsset.GetString(ENStringIndex.UIEquipmentRMBMoney);
        }
        else if (5 == m_type) //��Ӿ�
        {
            WorldParamInfo WorldParamList;
            WorldParamList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enTeamVolumeDropIcon);
            nIcnID = WorldParamList.IntTypeValue;
            //todo ����ַ�����
            NAME = GameTable.StringTableAsset.GetString(ENStringIndex.TeamVolume);
        }
        else Debug.LogWarning("CreateDropObj: Unknown Type = " + m_type.ToString());

        IconInfo icInf = GameTable.IconTableAsset.Lookup(nIcnID);
        if (icInf == null)
        {
            Debug.LogWarning("CreateDropObj: LOST = " + nIcnID.ToString());
        }
        else
        {
            // �����ɼ�����
            GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
            IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetUIAtlasPath(icInf.AtlasName), data, false, false);
            while (true)
            {
                e.MoveNext();
                if (data.m_isFinish)
                {
                    break;
                }
                yield return e.Current;
            }
            if (data.m_obj != null)
            {
                UIAtlas ua = data.m_obj as UIAtlas;
                if (ua != null)
                {
                    UISprite spt = item.GetComponent<UISprite>();
                    spt.atlas = ua;
                    spt.name = icInf.AtlasName;
                    spt.spriteName = icInf.SpriteName;
                }
            }
        }

        if (NAME != "")
        {
            UILabel label = name.GetComponent<UILabel>();
            label.text = NAME;
            switch (color)
            {
                case 1:
                    label.color = Color.white;
                    break;
                case 2:
                    label.color = Color.green;
                    break;
                case 3:
                    label.color = Color.blue;
                    break;
                case 4:
                    label.color = Color.magenta;
                    break;
                case 5:
                    label.color = Color.yellow;
                    break;
                default:
                    break;
            }
        }
        PickupDropCallback pdCall = item.GetComponent<PickupDropCallback>();
        if (pdCall != null)
        {
            pdCall.AttachDropValue(this, mDropID, mItemID);    // ��Щ������Ҫͬ����������
        }
    }

	public void DisableDropHolder()
	{
		IsEnable = false;
	}

	public override void Update()
	{
		MainGame.FaceToCamera(m_obj);
	}

	public override void FixedUpdate()
	{
		if (Time.realtimeSinceStartup - m_startTime > 60.0f)
		{
			IsEnable = false;
		}
	}

	public override void Init()
	{
		IsEnable = true;
		m_startTime = Time.realtimeSinceStartup;
	}

	public override void Release()
	{
		m_obj.SetActive(false);
		IPoolWidget<DropHolderTrigger>.ReleaseObj(this);
	}

	public override void Destroy()
	{
		if (null != m_obj)
		{
            PoolManager.Singleton.ReleaseObj(m_obj);
			//GameObject.Destroy(m_obj);
			m_obj = null;
		}
	}
}
