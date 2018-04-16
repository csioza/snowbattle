using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UITeam : UIWindow
{

    UILabel m_team              = null;
    UIGrid m_teamIndexGrid      = null;

    GameObject m_teamIndexItem    = null;
    UISprite m_teamTips         = null;
    //UILabel m_teamTipsText      = null;
    GameObject m_teamAdd        = null;
    UILabel m_costText          = null;

    GameObject m_main           = null;
    GameObject m_deputy         = null;
    GameObject m_support        = null;

    GameObject m_mainLv         = null;
    GameObject m_mainName       = null;
    GameObject m_mainRank       = null;
    GameObject m_mainModel      = null;
    GameObject m_mainOcc        = null;

    GameObject m_deputyLv       = null;
    GameObject m_deputyName     = null;
    GameObject m_deputyRank     = null;
    GameObject m_deputyModel    = null;
    GameObject m_deputyOcc      = null;

    GameObject m_supportLv      = null;
    GameObject m_supportName    = null;
    GameObject m_supportRank    = null;
    GameObject m_supportModel   = null;
    GameObject m_supportOcc     = null;

    GameObject m_teamNum        = null;
    GameObject m_curTeamIndex   = null;

    UIButton m_left             = null;
    UIButton m_right            = null;
  
    //Dictionary<CSItemGuid, UnityEngine.GameObject> m_modelList = new Dictionary<CSItemGuid, UnityEngine.GameObject>();
    List<GameResPackage.AsyncLoadObjectData> m_modelDataList = new List<GameResPackage.AsyncLoadObjectData>();
    Dictionary<int, GameObject> m_teamIndexList = new Dictionary<int, GameObject>();
    static public UITeam GetInstance()
    {
        UITeam self = UIManager.Singleton.GetUIWithoutLoad<UITeam>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UITeam>("UI/UITeam", UIManager.Anchor.Center);
        return self;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        foreach (KeyValuePair<int, GameObject> item in m_teamIndexList)
        {
          GameObject.Destroy(item.Value);
        }
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enTeam, OnPropertyChanged);

        m_team              = FindChildComponent<UILabel>("Team");
  
        m_teamIndexGrid     = FindChildComponent<UIGrid>("TeamIndexGrid");
        m_teamIndexItem     = FindChild("IndexItem");
        m_teamTips          = FindChildComponent<UISprite>("TeamTips");
//        m_teamTipsText      = FindChildComponent<UILabel>("TeamTipsText");
        m_costText          = FindChildComponent<UILabel>("CostText");
        m_teamAdd           = FindChild("ZAdd");

        m_main              = FindChild("Main");
        m_deputy            = FindChild("Deputy");
        m_support           = FindChild("Support");

        m_mainLv            = FindChild("MainLevel");
        m_mainName          = FindChild("MainName");
        m_mainModel         = FindChild("MainModel");
        m_mainRank          = FindChild("MainRankSprite");
        m_mainOcc           = FindChild("MainOcc");

        m_deputyLv          = FindChild("DeputyLevel");
        m_deputyName        = FindChild("DeputyName");
        m_deputyModel       = FindChild("DeputyModel");
        m_deputyRank        = FindChild("DeputyRankSprite");
        m_deputyOcc         = FindChild("DeputyOcc");  

        m_supportLv         = FindChild("SupportLevel");
        m_supportName       = FindChild("SupportName");
        m_supportRank       = FindChild("SupportRankSprite");
        m_supportModel      = FindChild("SupportModel");
        m_supportOcc        = FindChild("SupportOcc");

        m_teamNum           = FindChild("Total_number");
        m_curTeamIndex      = FindChild("Existing_number");

        m_left              = FindChildComponent<UIButton>("Select_left");
        m_right             = FindChildComponent<UIButton>("Select_right");
    }
   
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)Team.ENPropertyChanged.enShowTeam)
        {
//             if (MainUIManager.Singleton.ChangeUI(this))
//             {
//                 MainButtonList.Singleton.m_curShowType = MainButtonList.SHOWWNDTYPE.ENTeam;
//                 ShowWindow();
//                 UpdateInfo();
//             }
            
            //Debug.Log("CardBag.Singleton.GetCardList().Count:" + CardBag.Singleton.GetCardList().Count);
        }
        else if (eventType == (int)Team.ENPropertyChanged.enHideTeam)
        {
            HideWindow();
        }
        else if (eventType == (int)Team.ENPropertyChanged.enUpdateTeam)
        {
            //if (MainUIManager.Singleton.ChangeUI(this))
            {
                UpdateInfo();
            }
        }
        else if (eventType == (int)Team.ENPropertyChanged.enShowEditTeam)
        {
            Team.Singleton.UpdateTeamBagAllSlotId();
            Team.Singleton.m_curEditType = Team.EDITTYPE.enALL;
            HideAllModel();
            OperateCardList.Singleton.SetOperateTeamType();
            MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enOperaterCardList);
        }
        else if (eventType == (int)Team.ENPropertyChanged.enRemoveTeam)
        {
            OnRemoveTeamItem();
            // 显示
            UpdateInfo();
        }
        else if (eventType == (int)Team.ENPropertyChanged.enTeamEmpty)
        {
            UICommonMsgBoxCfg boxCfg = m_team.transform.Find("TeamEmpty").GetComponent<UICommonMsgBoxCfg>();
            UICommonMsgBox.GetInstance().ShowMsgBox(OnButtonYes, OnButtonNo, boxCfg);
            Debug.Log("全部队伍为空 则二次提示");
        }
        else if (eventType == (int)Team.ENPropertyChanged.enTeamMainEmpty)
        {
            UICommonMsgBoxCfg boxCfg = m_team.transform.Find("TeamMainEmpty").GetComponent<UICommonMsgBoxCfg>();
            UICommonMsgBox.GetInstance().ShowMsgBox(OnButtonYes, OnButtonNo, boxCfg);
            Debug.Log("全部队伍的主角色为空 则二次提示");
        }
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        
        AddChildMouseClickEvent("MainNameLable", OnClickTeamMain);
        AddChildMouseClickEvent("SupportNameLable", OnClickTeamSupport);
        AddChildMouseClickEvent("DeputyNameLable", OnClickTeamDeputy);

        AddChildMouseClickEvent("Deputybg", OnClickTeamDeputy);
        AddChildMouseClickEvent("Mainbg", OnClickTeamMain);
        AddChildMouseClickEvent("Supportbg", OnClickTeamSupport);

        AddChildMouseClickEvent("DeputyBox", OnClickTeamDeputy);
        AddChildMouseClickEvent("MainBox", OnClickTeamMain);
        AddChildMouseClickEvent("SupportBox", OnClickTeamSupport);

        AddChildMouseClickEvent("EditAll", OnClickEditAll);

        AddMouseClickEvent(m_teamAdd, OnClickAddTeam);
        AddMouseClickEvent(m_left.gameObject, OnLeft);
        AddMouseClickEvent(m_right.gameObject, OnRight);
        
    }

    
    public void OnClickEditAll(GameObject obj)
    {

        Team.Singleton.UpdateTeamBagAllSlotId();
        Team.Singleton.m_curEditType = Team.EDITTYPE.enALL;
        HideAllModel();
        OperateCardList.Singleton.SetOperateTeamType();
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enOperaterCardList);
    }


    // 点击主角色选择
    public void OnClickTeamMain(GameObject obj)
    {
        Team.Singleton.m_curEditType = Team.EDITTYPE.enMain;

        HideAllModel();

        OperateCardList.Singleton.SetOperateTeamType();
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enOperaterCardList);
    }

    // 点击副角色选择
    public void OnClickTeamDeputy(GameObject obj)
    {
        Team.Singleton.m_curEditType = Team.EDITTYPE.enDeputy;

        HideAllModel();

        OperateCardList.Singleton.SetOperateTeamType();
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enOperaterCardList);
    }

    // 点击支持角色选择
    public void OnClickTeamSupport(GameObject obj)
    {
        Team.Singleton.m_curEditType = Team.EDITTYPE.enSupport;

        HideAllModel();

        OperateCardList.Singleton.SetOperateTeamType();
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enOperaterCardList);
    }

    public override void OnShowWindow()
    {
        base.OnShowWindow();
        MainButtonList.Singleton.m_curShowType = MainButtonList.SHOWWNDTYPE.ENTeam;
        UpdateInfo();
    }

    // 更新队伍界面相关信息
    public void UpdateInfo()
    {
        // 队伍索引
        int index = Team.Singleton.m_curTeamIndex;

        UpdateTeamIndexItem();

        Debug.Log("当前队伍索引index：" + index);

        HideAllModel();

        // 允许 最大队伍数量
         PlayerAttrInfo playerInfo = GameTable.playerAttrTableAsset.LookUp(User.Singleton.GetLevel());
//         if (null != playerInfo && index+1 > playerInfo.m_teamNum)
//         {
//             // 显示 要达到的等级 并且重置界面
//             m_main.SetActive(false);
//             m_deputy.SetActive(false);
//             m_support.SetActive(false);
// 
//             m_teamTips.gameObject.SetActive(true);
// 
//            m_teamTipsText.text = string.Format(Localization.Get("TeamUnLockLevel"), playerInfo.m_unlockTeamLevel);
//            return;
 //      }

        m_left.gameObject.SetActive(true);
        m_right.gameObject.SetActive(true);

        if ( index == 0 )
        {
            m_left.gameObject.SetActive(false);
        }
        else if ( index == Team.Singleton.m_teamList.Count -1)
        {
            m_right.gameObject.SetActive(false);
        }
        m_teamNum.GetComponent<UILabel>().text      = playerInfo.m_teamNum.ToString();
        m_curTeamIndex.GetComponent<UILabel>().text = (index + 1) + "";

        m_teamTips.gameObject.SetActive(false);

        // 实际领导力消耗
        int realCost = 0;

        // 主角色
        CSItem card = Team.Singleton.GetCard(index, Team.EDITTYPE.enMain);
        if (null != card)
        {
            // 姓名
            HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);

            if (null == info)
            {
                Debug.LogWarning("null == hero card.IDInTable:" + card.IDInTable);
            }
            else
            {
                m_main.SetActive(true);
                m_mainLv.GetComponent<UILabel>().text               = Localization.Get("CardLevel") + card.Level ;

                AddModel(card, m_mainModel);

                // 星级
                RarityRelativeInfo rarityInfo                       = GameTable.RarityRelativeAsset.LookUp(info.Rarity);
                IconInfomation icon                                 = GameTable.IconInfoTableAsset.Lookup(rarityInfo.m_iconId);
                m_mainRank.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);
                realCost                                            = realCost + info.Cost;

                OccupationInfo occupationInfo                       = GameTable.OccupationInfoAsset.LookUp(info.Occupation);
                icon                                                = GameTable.IconInfoTableAsset.Lookup(occupationInfo.m_iconId);
                m_mainOcc.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);

                m_mainName.GetComponent<UILabel>().text             = info.StrName;
            }

            
           
        }
        else
        {
            m_main.SetActive(false);
        }

        // 副角色
        card = Team.Singleton.GetCard(index, Team.EDITTYPE.enDeputy);
        if (null != card)
        {
            // 姓名
            HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
            if (null == info)
            {
                Debug.LogWarning("null == hero card.IDInTable:" + card.IDInTable);
            }
            else
            {
                m_deputy.SetActive(true);
                m_deputyLv.GetComponent<UILabel>().text = Localization.Get("CardLevel") + card.Level ;

                AddModel(card, m_deputyModel);

                // 星级

                RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(info.Rarity);
                IconInfomation icon = GameTable.IconInfoTableAsset.Lookup(rarityInfo.m_iconId);
                m_deputyRank.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);

                realCost = realCost + info.Cost;


                OccupationInfo occupationInfo   = GameTable.OccupationInfoAsset.LookUp(info.Occupation);
                icon                            = GameTable.IconInfoTableAsset.Lookup(occupationInfo.m_iconId);
                m_deputyOcc.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);

                m_deputyName.GetComponent<UILabel>().text = info.StrName;
            }

           
        }
        else
        {
                m_deputy.SetActive(false);
        }

        // 支持角色
        card = Team.Singleton.GetCard(index, Team.EDITTYPE.enSupport);
        if (null != card)
        {
            // 姓名
            HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
            if (info != null)
            {
                m_support.SetActive(true);
                m_supportLv.GetComponent<UILabel>().text    = Localization.Get("CardLevel") + card.Level;
                // 星级
                RarityRelativeInfo rarityInfo               = GameTable.RarityRelativeAsset.LookUp(info.Rarity);
                IconInfomation icon = GameTable.IconInfoTableAsset.Lookup(rarityInfo.m_iconId);
                m_supportRank.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);

                realCost = realCost + info.Cost;

                AddModel(card, m_supportModel);


                OccupationInfo occupationInfo           = GameTable.OccupationInfoAsset.LookUp(info.Occupation);
                icon                                    = GameTable.IconInfoTableAsset.Lookup(occupationInfo.m_iconId);
                m_supportOcc.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);

                m_supportName.GetComponent<UILabel>().text = info.StrName;

            }
            else
            {
                Debug.Log("The info is null: " + card.IDInTable);
            }
        }
        else
        {
            m_support.SetActive(false);
        }

        // 玩家的领导力
        int playerLeadship = User.Singleton.GetLeadership();

        // 超过玩家的领导力显示红色
        if (realCost > playerLeadship)
        {
            m_costText.color = Color.red;
        }
        else
        {
            m_costText.color = Color.white;
        }
        // 领导力
        m_costText.text = realCost + "/" + playerLeadship;
    }

    // 更新 队伍索引按钮
    void UpdateTeamIndexItem()
    {
        AddTeamIndexItem();

        m_teamAdd.GetComponent<Parma>().m_id        = Team.Singleton.TeamNum + 1;
        m_teamAdd.GetComponent<UIToggle>().value    = Team.Singleton.m_curTeamIndex == Team.Singleton.TeamNum;

        m_teamIndexGrid.Reposition();
    }

    // 添加 队伍索引按钮
    void AddTeamIndexItem()
    {

        for (int i = 0; i < m_teamIndexList.Count;i++ )
        {
            m_teamIndexList[i].SetActive(false);
        }

        GameObject obj  = null;
        for (int i = 0; i < Team.Singleton.TeamNum; i++)
        {
            // 如果不存在 则创建
            if (false == m_teamIndexList.ContainsKey(i))
            {
                obj = GameObject.Instantiate(m_teamIndexItem.gameObject) as GameObject;

                obj.transform.parent        = m_teamIndexItem.transform.parent;
                obj.transform.localScale    = m_teamIndexItem.transform.localScale;
                obj.transform.localRotation = m_teamIndexItem.transform.localRotation;
                obj.transform.localPosition = m_teamIndexItem.transform.localPosition;

                obj.name = obj.name + i;
                AddChildMouseClickEvent(obj.name, OnChangTeam);
                m_teamIndexList.Add(i, obj);


            }

            obj             = m_teamIndexList[i].gameObject;
            Parma parma     = obj.GetComponent<Parma>();
            parma.m_id      = i;
            obj.SetActive(true);
            obj.GetComponent<UIToggle>().value = Team.Singleton.m_curTeamIndex == i;

            // 设置职业图标
            CSItem card = Team.Singleton.GetCard(i, Team.EDITTYPE.enMain);
            if (null != card)
            {
                HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
                if (null != info)
                {
                    OccupationInfo occupationInfo = GameTable.OccupationInfoAsset.LookUp(info.Occupation);
                    IconInfomation iconInfo = GameTable.IconInfoTableAsset.Lookup(occupationInfo.m_iconId);
                    obj.transform.Find("OccPic").GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);
                }
            }
            else
            {
                obj.transform.Find("OccPic").GetComponent<UITexture>().mainTexture =null;
            }

        }
    }

    // 删除一个队伍索引按钮
    void OnRemoveTeamItem()
    {
        if (m_teamIndexList.Count == 1)
        {
            m_teamIndexGrid.Reposition();
            return;
        }

        // 因为 在删除 队伍索引按钮时 队伍人数已经 减少 所以 删除最大的索引时 要加1
        int index = Team.Singleton.TeamNum;


        if (m_teamIndexList.ContainsKey(index))
        {
            m_teamIndexList[index].gameObject.SetActive(false);

            Debug.Log("RemoveTeamIndexItem index:" + index);
        }


        m_teamIndexGrid.Reposition();
    }

    // 添加显示模型
    void AddModel(CSItem card, GameObject parent)
    {
        if (null == card || parent == null)
        {
            return;
        }
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        m_modelDataList.Add(data);
        UIManager.Singleton.AddModel(card.IDInTable, parent, data);
    }

    void OnChangTeam(GameObject obj)
    {
        Parma parma = obj.GetComponent<Parma>();
        if (Team.Singleton.m_curTeamIndex == parma.m_id)
        {
            return;
        }
        Team.Singleton.m_curTeamIndex = parma.m_id;

       // Debug.Log("Team.Singleton.m_curTeamIndex:" + Team.Singleton.m_curTeamIndex);

       
        // 激活 删除队伍按钮

        UpdateInfo();

    }

    public void OnLeft(object sender, EventArgs e)
    {
        if (Team.Singleton.m_curTeamIndex == 0)
        {
            return;
        }

        Team.Singleton.m_curTeamIndex = Team.Singleton.m_curTeamIndex - 1;

        UpdateInfo();

    }

    public void OnRight(object sender, EventArgs e)
    {
        if (Team.Singleton.m_curTeamIndex == Team.Singleton.m_teamList.Count-1)
        {
            return;
        }

        Team.Singleton.m_curTeamIndex = Team.Singleton.m_curTeamIndex + 1;

        UpdateInfo();

    } 

    void HideAllModel()
    {
        foreach (var item in m_modelDataList)
        {
            UIManager.Singleton.HideModel(item.m_obj as GameObject);
        }
        m_modelDataList.Clear();
    }


    public void OnClickAddTeam(object sender, EventArgs e)
    {
        Team.Singleton.ShowEmptyTeam();

    }

    public void OnClickSubTeam(GameObject obj)
    {
        Team.Singleton.RemoveTeam();
    }

    // 退出队伍时的 删除空队伍 和 弹二次提示框的相关处理
    public override bool OnLeave()
    {
//        int index           = Team.Singleton.m_curTeamIndex;
        // 是否所有队伍为空
        bool isEmpty        = true;
        // 是否所有队伍的主角色为空
        bool isMainEmpty     = true;

        for (int i = 0; i < Team.Singleton.TeamNum;i++ )
        {
            if (!Team.Singleton.IsTeamEmpty(i))
            {
                isEmpty = false;
            }

            if (!Team.Singleton.IsDutyEmpty(i, Team.EDITTYPE.enMain))
            {
                isMainEmpty = false;
            }
        }
        // 如果全部队伍为空 则二次提示
        if (isEmpty)
        {
            UICommonMsgBoxCfg boxCfg = FindChild("TeamEmpty").GetComponent<UICommonMsgBoxCfg>();
            UICommonMsgBox.GetInstance().ShowMsgBox(OnButtonYes, OnButtonNo, boxCfg);
            //Debug.Log("全部队伍为空 则二次提示");
            return false;
        }

        // 如果全部队伍的主角色为空 则二次提示
        if (isMainEmpty)
        {
            UICommonMsgBoxCfg boxCfg = FindChild("TeamMainEmpty").GetComponent<UICommonMsgBoxCfg>();
            UICommonMsgBox.GetInstance().ShowMsgBox(OnButtonYes, OnButtonNo, boxCfg);
            //Debug.Log("全部队伍的主角色为空 则二次提示");
            return false;
        }

        // 然后删除 空白队伍 这时 会保证 必然有一个队伍不是空 所以可以放心的删除所有的空队伍
        Team.Singleton.RemoveEmpty();

        // 更新队伍当前索引 如果 当前索引的队伍为空 则取第一个队伍为 当前队伍索引
        if (Team.Singleton.IsTeamEmpty(Team.Singleton.m_curTeamIndex))
        {
            Team.Singleton.m_curTeamIndex = 0;
        }

        // 发送服务器保存 当前队伍索引
        IMiniServer.Singleton.SendSelectTeamIndex(Team.Singleton.m_curTeamIndex);

        Team.Singleton.UpdateBattleTeam();

        return true;
    }
    public void OnButtonYes(object sender, EventArgs e)
    {

    }
    public void OnButtonNo(object sender, EventArgs e)
    {

    }
}
