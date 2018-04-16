using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// 凋落物的信息
public class DropItem
{
    public GameObject m_obj;
    public long m_deadTime;
}

// 掉落类型
public class DropItemPerformanceType
{
    public enum Type
    {
        enGold = 0,
        enCard,
        enKey,
        enHp,
    }

    public Type m_type;
    public int m_intParam = 0 ;

    public Vector3 m_deadPos        = Vector3.zero;
    public Vector3 m_eulerAngles    = Vector3.zero;

}

// 掉落卡牌类型
public class DropItemPerformanceCard : DropItemPerformanceType
{
    public List<int> m_cardList         = new List<int>(); // 掉落的卡牌列表 
}

// 掉落表现
public class DropItemPerformance 
{
    public int m_dropIndex = 0; // 掉落索引
    public Dictionary<int, DropItem> m_dropObjList = new Dictionary<int, DropItem>(); // 掉落物件列表 GameObject 掉落的物件 long 消失的时间点

    #region Singleton
    static DropItemPerformance m_singleton;
    static public DropItemPerformance Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new DropItemPerformance();
            }
            return m_singleton;
        }
    }
    #endregion


    // 掉落前 的 记录整理
    public void Start(NPC npc)
    {

        // 掉落卡牌
        if (npc.IsHaveDropItem())
        {
            // 掉落卡牌
            DropItemPerformanceCard itemType    = new DropItemPerformanceCard();
            itemType.m_type                     = DropItemPerformanceType.Type.enCard;
            itemType.m_cardList.Clear();
            itemType.m_deadPos                  = new Vector3(npc.MainObj.transform.position.x, npc.MainObj.transform.position.y, npc.MainObj.transform.position.z);
            itemType.m_eulerAngles              = new Vector3(npc.MainObj.transform.localEulerAngles.x, npc.MainObj.transform.localEulerAngles.y, npc.MainObj.transform.localEulerAngles.z);
           
            // 更新掉落列表
            if (null != npc.roomElement)
            {
                foreach (int cardId in npc.roomElement.MonsterData.dropList)
                {
                    itemType.m_cardList.Add(cardId);
                }
            }

           
            MainGame.Singleton.StartCoroutine(LoadDropItem(itemType));
        }

        // 掉落金钱
        if ((npc.GetNpcType() == ENNpcType.enBoxNPC && npc.GetNpcInterSubType() == ENNpcInterSubType.enTreasureBox))
        {
            // 掉落金钱
            DropItemPerformanceType itemType    = new DropItemPerformanceType();
            itemType.m_type                     = DropItemPerformanceType.Type.enGold;
            itemType.m_intParam                      = 100;
            itemType.m_deadPos                  = new Vector3(npc.MainObj.transform.position.x, npc.MainObj.transform.position.y, npc.MainObj.transform.position.z);
            itemType.m_eulerAngles              = new Vector3(npc.MainObj.transform.localEulerAngles.x, npc.MainObj.transform.localEulerAngles.y, npc.MainObj.transform.localEulerAngles.z);
            MainGame.Singleton.StartCoroutine(LoadDropItem(itemType));
        }
    }

    public IEnumerator LoadDropItem(DropItemPerformanceType item)
    {
        yield return new WaitForSeconds(0.2f);

       
        GameResPackage.AsyncLoadPackageData loadData = new GameResPackage.AsyncLoadPackageData();
        IEnumerator e = null;

        EventItemInfo eventInfo = GameTable.eventItemAsset.LookUp(item.m_intParam);

        DropItemPerformanceType.Type type = item.m_type;
        if (eventInfo != null)
        {
            type = (DropItemPerformanceType.Type)eventInfo.dropType;
        }

        switch (type)
        {

                // 卡牌
            case DropItemPerformanceType.Type.enCard:
                {
                    DropItemPerformanceCard cardItem = (DropItemPerformanceCard)item;

                    foreach (int cardId in cardItem.m_cardList)
                    {

                        e = NewDropItem(loadData, item, cardId);
                        while (true)
                        {
                            e.MoveNext();
                            if (loadData.m_isFinish)
                            {
                                break;
                            }
                            yield return e.Current;
                        }

                        Debug.Log("....cardId:" + cardId);

                        yield return new WaitForSeconds(0.1f);
                    }

                    // 卡牌的增长数目
                    Reward.Singleton.m_cardDesNum = Reward.Singleton.m_cardNum + cardItem.m_cardList.Count;

                    break;
                }

                // 金钱
            case DropItemPerformanceType.Type.enGold:
                {
                    // 如果有钱则掉落金币 效果
                    if (item.m_intParam > 0)
                    {
                        int min = 15;
                        int max = 30;
                        WorldParamInfo worldInfo = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enDropGoldMinMax);
                        if (null != worldInfo)
                        {

                            List<int> cardList = ConvertorTool.StringToList_Int(worldInfo.StringTypeValue);
                            if (null != cardList && cardList.Count >= 2)
                            {
                                min = cardList[0];
                                max = cardList[1];
                            }
                        }

                        // 金币增长的数目
                        int randomGold = UnityEngine.Random.Range(100, 1000);
                        Reward.Singleton.m_goldDesNum = Reward.Singleton.m_goldNum + randomGold;

                        int dropNum = UnityEngine.Random.Range(min, max);

                        for (int i = 0; i < dropNum; i++)
                        {
                            
                            e = NewDropItem(loadData, item);
                            while (true)
                            {
                                e.MoveNext();
                                if (loadData.m_isFinish)
                                {
                                    break;
                                }
                                yield return e.Current;
                            }
                            yield return new WaitForSeconds(0.01f);
                        }
                    }
                    break;
                }
                     // 钥匙
            case DropItemPerformanceType.Type.enKey:
                {
                    e = NewDropItem(loadData, item, item.m_intParam);
                    while (true)
                    {
                        e.MoveNext();
                        if (loadData.m_isFinish)
                        {
                            break;
                        }
                        yield return e.Current;
                    }
                    break;
                }
        }
        
    }

    //
    IEnumerator NewDropItem(GameResPackage.AsyncLoadPackageData loadData, DropItemPerformanceType item,int intParam = 0)
    {

        EventItemInfo eventInfo = GameTable.eventItemAsset.LookUp(item.m_intParam);

        DropItemPerformanceType.Type type = item.m_type;
        if (eventInfo != null)
        {
            type = (DropItemPerformanceType.Type)eventInfo.dropType;
        }

         // 
        string objName      = "";

        switch (type)
        {
            case DropItemPerformanceType.Type.enGold:
                {
                    objName = "Balk/p-b-gold";
                    break;
                }
            case DropItemPerformanceType.Type.enCard:
                {
                    objName = "Balk/p-b-contract";
                    //DropItemPerformanceCard cardItem    = (DropItemPerformanceCard)item; 
                    // 根据不同星级掉落不同 模型
                    HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(intParam);
                    if (null != heroInfo)
                    {
                        RarityRelativeInfo rarityInfo   = GameTable.RarityRelativeAsset.LookUp(heroInfo.Rarity);
                        if (null != rarityInfo)
                        {
                            ModelInfo modelInfo         = GameTable.ModelInfoTableAsset.Lookup(rarityInfo.m_dropItemModelID);
                            if (null != modelInfo)
                            {
                                objName = modelInfo.ModelFile;
                            }
                        }
                    }
                    break;
                }
            case DropItemPerformanceType.Type.enKey:
                {
                    // 钥匙模型
                    objName     = "Balk/p-b-gold";

                    EventItemInfo eventItemInfo = GameTable.eventItemAsset.LookUp(intParam);
                    if ( null != eventItemInfo )
                    {
                        ModelInfo modelInfo = GameTable.ModelInfoTableAsset.Lookup(eventItemInfo.modelId);
                        if (null != modelInfo)
                        {
                            objName = modelInfo.ModelFile;
                        }
                    }
                    break;
                }
            
        }

        float rotation  = 30f;
        rotation        = UnityEngine.Random.Range(0f, 360f);
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        IEnumerator e   = PoolManager.Singleton.Coroutine_Load(GameData.GetPrefabPath(objName), data);
        while (true)
        {
            e.MoveNext();
            if (data.m_isFinish)
            {
                break;
            }
            yield return e.Current;
        }
        GameObject goldObj                  = data.m_obj as GameObject;
        goldObj.transform.parent            = MainGame.Singleton.MainObject.transform;
        goldObj.transform.position          = item.m_deadPos;
        goldObj.SetActive(true);
        goldObj.transform.localEulerAngles  = new Vector3(item.m_eulerAngles.x, item.m_eulerAngles.y + rotation, item.m_eulerAngles.z);


        int dropIndex                       = AddDropItem(goldObj);

        Animation anim                      = goldObj.transform.Find("gold_body").GetComponent<Animation>();

        PickUpCallback pickUpCallback       = goldObj.transform.Find("gold_body").GetComponent<PickUpCallback>();
        if (null != pickUpCallback)
        {
            pickUpCallback.Clear();
            pickUpCallback.m_index = dropIndex;
        }

        // 随机动画
        int animationCount = anim.GetComponent<Animation>().GetClipCount();
        int animationIndex = UnityEngine.Random.Range(1, animationCount);
        int index = 1;
        foreach (AnimationState state in anim.GetComponent<Animation>())
        {
            if (index == animationIndex)
            {
                anim.CrossFade(state.name);
                break;
            }
            index++;
        }

        loadData.m_isFinish = true;

    }

    // 添加掉落物
    public int AddDropItem(GameObject obj)
    {
        long deadTime   = TimeUtil.GetServerTimeStampNow() + 5;
        DropItem item   = new DropItem();
        item.m_deadTime = deadTime;
        item.m_obj      = obj;

        m_dropObjList.Add(++m_dropIndex, item);

        return m_dropIndex;
    }

    // 删除掉落物
    public void RemoveDropItem(int index)
    {

        if (!m_dropObjList.ContainsKey(index))
        {
            return;
        }

        //Debug.Log(" DropItemPerformance RemoveDropItem m_dropObjList[index].m_obj:" + m_dropObjList[index].m_obj.name);
        PoolManager.Singleton.ReleaseObj(m_dropObjList[index].m_obj);
        m_dropObjList.Remove(index);
    }

    public void ClearDropItem()
    {
        foreach (KeyValuePair<int, DropItem> item in m_dropObjList)
        {
            if (item.Value != null)
            {
                PoolManager.Singleton.ReleaseObj(item.Value.m_obj);
            }
        }

        m_dropObjList.Clear();

        m_dropIndex = 0;
    }

}
