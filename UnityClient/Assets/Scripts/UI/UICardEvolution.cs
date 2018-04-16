using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UICardEvolution : UIBaseCardOperate
{
//共同属性
    UILabel m_costMoney;//升段所需金钱
    GameObject m_cardAfter;//显示卡牌晋级后卡牌头像
    GameObject m_cardBefor;//显示卡牌晋级前卡牌头像
    UICardHead m_cardBeforHead;//升级前头像
    UICardHead m_cardAfterHead;//升级后头像
    //卡牌晋级界面需要隐藏的相关界面
    GameObject m_evolutionBefore;//晋级前属性面板
    GameObject m_evolutionAfter;//晋级后属性面板
    //卡牌晋级界面需要显示的相关界面
    GameObject m_danBefor;//升段前属性面板
    GameObject m_danAfter;//升段后属性面板
    //面板 文字
    UISprite m_panelHead;
    UISprite m_promotionBefore;
    UISprite m_promotionAfter;
    UIPanel m_formulaPanel;
    List<UICardHead> m_materialList;
    List<UISprite> m_materialSpritList;
    List<GameResPackage.AsyncLoadObjectData> m_modelDataList = new List<GameResPackage.AsyncLoadObjectData>();
    //3D模型
    GameObject m_beforModel;
    GameObject m_afterModel;
    UIButton m_buttonOk;//确定升段按钮


    

    static public UICardEvolution GetInstance()
	{
        UICardEvolution self = UIManager.Singleton.GetUIWithoutLoad<UICardEvolution>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UICardEvolution>("UI/UICardEvolution", UIManager.Anchor.Center);
		return self;
	}

	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enCardEvolution, OnPropertyChanged);
        m_materialList = new List<UICardHead>();
        m_materialSpritList = new List<UISprite>();
        m_evolutionBefore = FindChild("Evolution_Before_Panel");
        m_evolutionAfter = FindChild("Evolution_After_Panel");
        m_danBefor = FindChild("Dan_Befor_Panel");
        m_danAfter = FindChild("Dan_After_Panel");
        m_cardAfter = FindChild("Card_After");
        m_cardBefor = FindChild("Card_Before");
        m_panelHead = FindChildComponent<UISprite>("CardEvolutionl_head");
        m_promotionBefore = FindChildComponent<UISprite>("Promotion_Before");
        m_promotionAfter = FindChildComponent<UISprite>("Promotion_After");
        m_formulaPanel = FindChildComponent<UIPanel>("FormulaPanel");
        for (int i = 1; i <= 5; i++)
        {
            UISprite material = FindChildComponent<UISprite>("Materia" + i);
            m_materialSpritList.Add(material);
        }
        m_beforModel = FindChild("Befor_Model");
        m_afterModel = FindChild("After_Model");
        m_buttonOk = FindChildComponent<UIButton>("ButtonOK");
        m_costMoney = FindChildComponent<UILabel>("CostMoney");
	}

    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("ButtonOK", OnButtonOK);
        AddChildMouseClickEvent("ButtonExit", OnButtonExit);
    }

     void OnButtonExit(GameObject obj)
    {
        HideWindow();
        //CardDivisionUpdateProp.Singleton.ClearChooseCardList();
        //CardDivisionUpdateProp.Singleton.m_curtFormula = (int)CardDivisionUpdateProp.CardDivisionFormula.FormulaOne;
    }
     void OnButtonOK(GameObject obj) 
     {
        bool isShowCommonMsgBox = false;

        CSItem card = CardBag.Singleton.GetCardByGuid(CardEvolution.Singleton.m_curEvolutionGuid);
        if (null == card)
        {
            Debug.Log("UICardEvolution card == null card:" + CardEvolution.Singleton.m_curEvolutionGuid);
            return;
        }
        HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
        if (null == info)
        {
            Debug.Log("UICardEvolution HeroInfo == null card.m_id:" + card.IDInTable);
            return;
        }
         Dictionary<int, int> evolveCardList = new Dictionary<int, int>();
        foreach (var cardID in info.EvolveCardList)
        {
            if (evolveCardList.ContainsKey(cardID))
            {
                ++evolveCardList[cardID];
            }
            else
            {
                evolveCardList.Add(cardID, 1);
            }
        }
         //判断素材卡里是否有稀有素材 如果有 则弹出二次确认框
        foreach (KeyValuePair<int, int> item in evolveCardList)
        {
             WorldParamInfo worldInfo = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enCardDivisionMaterialStarLevel);
             HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(item.Value);
             if (null != heroInfo && worldInfo.IntTypeValue <= heroInfo.Rarity)
             {
                isShowCommonMsgBox = true;
             }
        }
        if (isShowCommonMsgBox)
        {
            UICommonMsgBoxCfg boxCfg = m_buttonOk.transform.GetComponent<UICommonMsgBoxCfg>();
            UICommonMsgBox.GetInstance().ShowMsgBox(OnCardEvolutionOK, OnCardEvolutionNO, boxCfg);
            string formulaText = Localization.Get("WarningOfTheMaterial");
            UICommonMsgBox.GetInstance().GetMainText().SetHintText(formulaText);
            UICommonMsgBox.GetInstance().GetMainText().SetHintTextColor(Color.red);
        }
        else
        {
            //进入转菊花loading界面
            Loading.Singleton.SetLoadingTips((int)LOADINGTIPSENUM.enJumpToBag);
            IMiniServer.Singleton.req_herocardEvolve(CardEvolution.Singleton.m_curEvolutionGuid);
            HideWindow();
        }
     }

     void OnCardEvolutionOK(object sender, EventArgs e)
     {
         //进入转菊花loading界面
         Loading.Singleton.SetLoadingTips((int)LOADINGTIPSENUM.enJumpToBag);
         IMiniServer.Singleton.req_herocardEvolve(CardEvolution.Singleton.m_curEvolutionGuid);
         HideWindow();
     }

     void OnCardEvolutionNO(object sender, EventArgs e)
     {

     }
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (null == WindowRoot)
        {
            return;
        }
    }

    public override void OnShowWindow()
    {
        base.OnShowWindow();
        ShowCardEvolutionPanel();
        HideCardDivisionPanel();
        UpdateInfo();
    }
    public override void OnHideWindow()
    {
        base.OnHideWindow();
        //清空素材表
        ResetMaterialList();
    }
    //清空素材列表
    void ResetMaterialList() 
    {
        foreach (UICardHead item in m_materialList)
        {
            item.Destroy();
        }
        m_materialList.Clear();
        if (null != m_cardBeforHead)
        {
            m_cardBeforHead.Destroy();
            m_cardBeforHead = null;
        }
        if (null != m_cardAfterHead)
        {
            m_cardAfterHead.Destroy();
            m_cardAfterHead = null;
        }
        foreach (var item in m_modelDataList)
        {
            UIManager.Singleton.HideModel(item.m_obj as GameObject);
        }
        m_modelDataList.Clear();
        foreach (UISprite item in m_materialSpritList)
        {
            Transform labelTransform = item.transform.Find("MateriaLabel");
            labelTransform.gameObject.SetActive(false);
            Transform texure = item.transform.Find("Sprite");
            WorldParamInfo worldInfo = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enCardDivisionNullMaterialIcon);
            IconInfomation imageInfo = GameTable.IconInfoTableAsset.Lookup(worldInfo.IntTypeValue);
            texure.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(imageInfo.dirName);
            Transform Startexure = item.transform.Find("Texture");
            Startexure.gameObject.SetActive(false);
            item.GetComponent<Parma>().m_type = 0;
        }
    }

    void UpdateInfo()
    {

        //设置卡牌晋级前卡牌头像
        if (null == m_cardBeforHead)
        {
            m_cardBeforHead = UICardHead.Create();
            m_cardBeforHead.SetParent(m_cardBefor);
            m_cardBeforHead.SetLocalPosition(Vector3.zero);
        }
        CSItem card = CardBag.Singleton.GetCardByGuid(CardEvolution.Singleton.m_curEvolutionGuid);
        if (null == card)
        {
            Debug.Log("UICardEvolution card == null card:" + CardEvolution.Singleton.m_curEvolutionGuid);
            return;
        }
        HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
        if (null == info)
        {
            Debug.Log("UICardEvolution HeroInfo == null card.m_id:" + card.IDInTable);
            return;
        }
        m_cardBeforHead.SetCardInfo(card);
        m_cardBeforHead.RegisterCallBack(null, PressCardHead);
        m_cardBeforHead.ShowCard();
        m_cardBeforHead.SetCardInfoShow(false);
        m_cardBeforHead.SetCardLoveShow(false);

        //设置卡牌晋级后卡牌头像
        if (null == m_cardAfterHead)
        {
            m_cardAfterHead = UICardHead.Create();
            m_cardAfterHead.SetParent(m_cardAfter);
            m_cardAfterHead.SetLocalPosition(Vector3.zero);
        }
        CSItem tempCard = new CSItem();
        tempCard.m_id = (short)info.EvolveChangeID;
        m_cardAfterHead.SetCardInfo(tempCard);
        m_cardAfterHead.RegisterCallBack(null, PressCardHead);
        m_cardAfterHead.ShowCard();
        m_cardAfterHead.SetCardInfoShow(false);
        m_cardAfterHead.SetCardLoveShow(false);
        int costMoneny = info.EvolveNeedLevel * info.Rarity;
        //设置素材卡牌
        SetMaterial(card);
        //设置金钱数量
        SetCostMoney(costMoneny);
        //设置属性面板显示
        SetCardPropPanel(card);
        //设置卡牌的3D模型
        SetCard3DHead(card);
        //检查素材是否齐全
        CheckMaterialEnough(card);
  
    }
    //检查素材是否齐全
    void CheckMaterialEnough(CSItem card) 
    {
        bool isCanEvolution = true;
        HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
        if (null == info)
        {
            Debug.Log("UICardEvolution HeroInfo == null card.m_id:" + card.IDInTable);
            return;
        }
        //检查金钱是否足够
        int needMoney = info.EvolveNeedLevel * info.Rarity;
        int money = User.Singleton.UserProps.GetProperty_Int32(UserProperty.money);
        if (money < needMoney)
        {
            isCanEvolution = false;
        }
        //检查素材是否足够
        Dictionary<int, int> evolveCardList = new Dictionary<int, int>();
        foreach (var cardID in info.EvolveCardList)
        {
            if (evolveCardList.ContainsKey(cardID))
            {
                ++evolveCardList[cardID];
            }
            else
            {
                evolveCardList.Add(cardID, 1);
            }
        }
        foreach (KeyValuePair<int, int> item in evolveCardList) 
        {
            int cardCount = User.Singleton.ItemBag.GetCardNumById(item.Key);
            if (cardCount < item.Value)
            {
                isCanEvolution = false;
            }
        }


        m_buttonOk.isEnabled = isCanEvolution;
    }

    //设置卡牌晋级的素材卡牌
    void SetMaterial(CSItem card) 
    {
        HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
        if (null == info)
        {
            Debug.Log("UICardEvolution HeroInfo == null card.m_id:" + card.IDInTable);
            return;
        }

        Dictionary<int, int> evolveCardList = new Dictionary<int, int>();
        foreach (var cardID in info.EvolveCardList)
        {
            if (evolveCardList.ContainsKey(cardID))
            {
                ++evolveCardList[cardID];
            }
            else
            {
                evolveCardList.Add(cardID, 1);
            }
        }
        int spritIndex = 0;
        foreach (KeyValuePair<int, int> item in evolveCardList)
        {
            //设置添加素材卡牌头像
            UICardHead tempHead = UICardHead.Create();
            m_materialList.Add(tempHead);//加入到素材列表
            tempHead.SetParent(m_materialSpritList[spritIndex].transform);
            tempHead.SetLocalPosition(Vector3.zero);
            tempHead.RegisterCallBack(null, PressCardHead);
            CSItem tempCard = new CSItem();
            tempCard.m_id = (short)item.Key;
            tempHead.SetCardInfo(tempCard);
            tempHead.ShowCard();
            tempHead.SetCardLoveShow(false);
            //根据需要素材个数显示文字颜色
            int cardCount = User.Singleton.ItemBag.GetCardNumById(item.Key);
            tempHead.m_cardInfo.text = string.Format(Localization.Get("WithTheMaterial"), cardCount);
            if (cardCount >= item.Value)
            {
                tempHead.m_cardInfo.color = Color.green;//显示 绿色文字
            }
            else 
            {
                tempHead.m_cardInfo.color = Color.red;//显示红色文字 显示 透明遮罩
                tempHead.SetCardMask(true);
            }
            spritIndex++;
        }
    }
    //设置属性面板显示
    void SetCardPropPanel(CSItem card) 
    {
        //进阶前属性
        int beaforLevel = card.Level;//当前等级
        int beaforMaxLevel = card.GetMaxLevel();//最大等级
        m_evolutionBefore.transform.Find("LevelTips").GetComponent<UILabel>().text = beaforLevel.ToString() + '/' + beaforMaxLevel.ToString();
        int beaforHp = card.GetHp();//当前生命值
        m_evolutionBefore.transform.Find("HP").GetComponent<UILabel>().text = beaforHp.ToString();

        int beaforAttack = card.GetPhyAttack();//物理攻击力
        m_evolutionBefore.transform.Find("Attack").GetComponent<UILabel>().text = beaforAttack.ToString();

        int beaforMagAtt = card.GetMagAttack();//魔法攻击力
        m_evolutionBefore.transform.Find("MagAttack").GetComponent<UILabel>().text = beaforMagAtt.ToString();

        HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
        if (null == info)
        {
            Debug.Log("UICardEvolution HeroInfo == null card.m_id:" + card.IDInTable);
            return;
        }

        HeroInfo afterInfo = GameTable.HeroInfoTableAsset.Lookup(info.EvolveChangeID);
        if (null == afterInfo)
        {
            Debug.Log("UICardEvolution afterInfo == null card.m_id:" + info.EvolveChangeID);
            return;
        }
        //进阶后属性

        int afterLevel = 1;//升级后等级
        int afterMaxLevel = card.GetMaxLevel() + afterInfo.MaxLevel;//最大等级
        m_evolutionAfter.transform.Find("LevelTips").GetComponent<UILabel>().text = afterLevel.ToString() + '/' + afterMaxLevel.ToString();
        m_evolutionAfter.transform.Find("LevelTips").GetComponent<UILabel>().color = Color.red;

        int afterHp = (int)afterInfo.FHPMax;//升级后生命值
        m_evolutionAfter.transform.Find("HP").GetComponent<UILabel>().text = afterHp.ToString();
        if (afterHp > beaforHp )
        {
            m_evolutionAfter.transform.Find("HP").GetComponent<UILabel>().color = Color.green;
        }
        else
        {
            m_evolutionAfter.transform.Find("HP").GetComponent<UILabel>().color = Color.red;
        }

        int afterAttack = (int)afterInfo.FPhyAttack;//升级后攻击力
        m_evolutionAfter.transform.Find("Attack").GetComponent<UILabel>().text = afterAttack.ToString();
        if (afterAttack > beaforAttack)
        {
            m_evolutionAfter.transform.Find("Attack").GetComponent<UILabel>().color = Color.green;
        }
        else
        {
            m_evolutionAfter.transform.Find("Attack").GetComponent<UILabel>().color = Color.red;
        }

        int afterMagAtt = (int)afterInfo.FMagAttack;//升级后攻击力
        m_evolutionAfter.transform.Find("MagAttack").GetComponent<UILabel>().text = afterMagAtt.ToString();
        if (afterMagAtt > beaforMagAtt)
        {
            m_evolutionAfter.transform.Find("MagAttack").GetComponent<UILabel>().color = Color.green;
        }
        else
        {
            m_evolutionAfter.transform.Find("MagAttack").GetComponent<UILabel>().color = Color.red;
        }
    }

    //设置金钱数量
    void SetCostMoney(int costMoney) 
    {
        int money = User.Singleton.UserProps.GetProperty_Int32(UserProperty.money);
        m_costMoney.text = costMoney.ToString();

        if (money < costMoney)
        {
            m_costMoney.color = Color.red;
            //以后需要挂UI特效
        }
        else
        {
            m_costMoney.color = Color.green;
        }
    }

    //设置3D头像
    void SetCard3DHead(CSItem card)
    {
        int childCount = m_beforModel.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = m_beforModel.transform.GetChild(i);
            GameObject.Destroy(child.gameObject);

        }
        AddModel(card, m_beforModel);
        int childCount1 = m_afterModel.transform.childCount;
        for (int i = 0; i < childCount1; i++)
        {
            Transform child = m_afterModel.transform.GetChild(i);
            GameObject.Destroy(child.gameObject);

        }
        HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
        if (null == info)
        {
            Debug.Log("UICardEvolution HeroInfo == null card.m_id:" + card.IDInTable);
            return;
        }
        CSItem tempCard = new CSItem();
        tempCard.m_id = (short)info.EvolveChangeID;
        AddModel(tempCard, m_afterModel);
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

    //长按显示卡牌详情界面 只带返回按钮
    void PressCardHead(CSItem card)
    {
        CardBag.Singleton.OnShowCardDetail(card, true);
    }

    //隐藏段位升级相关面板
    void HideCardDivisionPanel()
    {
        m_danAfter.SetActive(false);
        m_danBefor.SetActive(false);
        m_formulaPanel.gameObject.SetActive(false);
    }

    //显示卡牌晋级相关面板
    void ShowCardEvolutionPanel()
    {
        m_evolutionAfter.SetActive(true);
        m_evolutionBefore.SetActive(true);
        //设置面板文字
        Transform headTransform = m_panelHead.transform.Find("Label");
        headTransform.GetComponent<UILabel>().text = Localization.Get("TheMercenaryPromotion");
        Transform promotionBefore = m_promotionBefore.transform.Find("Label");
        promotionBefore.GetComponent<UILabel>().text = Localization.Get("BeforeStrengthening");
        Transform promotionAfter = m_promotionAfter.transform.Find("Label");
        promotionAfter.GetComponent<UILabel>().text = Localization.Get("AfterReinforcement");
    }

}
