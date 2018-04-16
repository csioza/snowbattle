using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//段位升级UI界面
public class UICardDivisionUpdate : UIBaseCardOperate
{
    
    enum MaterialType//素材类型
    {
        enAppoint = 0,//指定卡牌
        enUnAppoint = 1,//非指定卡牌
        enRingOfHonor = 2,//荣誉戒指

    }
//段位升级界面需要隐藏的相关界面
    GameObject m_evolutionBefore;//强化前属性面板
    GameObject m_evolutionAfter;//强化后属性面板
//段位升级界面需要显示的相关界面
    GameObject m_danBefor;//升段前属性面板
    GameObject m_danAfter;//升段后属性面板

    GameObject m_cardAfter;//显示段位升级后卡牌头像
    GameObject m_cardBefor;//显示段位升级前卡牌头像

    UICardHead m_cardBeforHead;//升级前头像
    UICardHead m_cardAfterHead;//升级后头像

    UILabel m_costMoney;//升段所需金钱
    UIButton m_buttonOk;//确定升段按钮
    //UIButton m_buttonExit;//取消升段按钮
    List<UICardHead> m_materialList;
    List<UISprite> m_materialSpritList;

    UIButton m_formulaButton2;
    UIButton m_formulaButton3;
    UIButton m_formulaButton4;

    List<GameResPackage.AsyncLoadObjectData> m_modelDataList = new List<GameResPackage.AsyncLoadObjectData>();

    //3D模型
    GameObject m_beforModel;
    GameObject m_afterModel;

    //面板 文字
    UISprite m_panelHead;
    UISprite m_promotionBefore;
    UISprite m_promotionAfter;
    UIPanel m_formulaPanel;

    static public UICardDivisionUpdate GetInstance()
    {
        UICardDivisionUpdate self = UIManager.Singleton.GetUIWithoutLoad<UICardDivisionUpdate>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UICardDivisionUpdate>("UI/UICardEvolution", UIManager.Anchor.Center);
        return self;
    }


    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enCardDivisionUpdateProp, OnPropertyChanged);
        m_materialList = new List<UICardHead>();
        m_materialSpritList = new List<UISprite>();
        m_evolutionBefore = FindChild("Evolution_Before_Panel");
        m_evolutionAfter = FindChild("Evolution_After_Panel");
        m_danBefor = FindChild("Dan_Befor_Panel");
        m_danAfter = FindChild("Dan_After_Panel");
        m_cardAfter = FindChild("Card_After");
        m_cardBefor = FindChild("Card_Before");
        for (int i = 1; i <= 5; i++ )
        {
            UISprite material = FindChildComponent<UISprite>("Materia"+i);
            m_materialSpritList.Add(material);
        }

        m_costMoney = FindChildComponent<UILabel>("CostMoney");
        m_buttonOk = FindChildComponent<UIButton>("ButtonOK");
//        m_buttonExit = FindChildComponent<UIButton>("ButtonExit");
        m_formulaButton2 = FindChildComponent<UIButton>("Formula_two");
        m_formulaButton3 = FindChildComponent<UIButton>("Formula_three");
        m_formulaButton4 = FindChildComponent<UIButton>("Formula_four");
        m_beforModel = FindChild("Befor_Model");
        m_afterModel = FindChild("After_Model");
        m_panelHead = FindChildComponent<UISprite>("CardEvolutionl_head");
        m_promotionBefore = FindChildComponent<UISprite>("Promotion_Before");
        m_promotionAfter = FindChildComponent<UISprite>("Promotion_After");
        m_formulaPanel = FindChildComponent<UIPanel>("FormulaPanel");
    }

    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("ButtonOK", OnButtonOK);
        AddChildMouseClickEvent("ButtonExit", OnButtonExit);
        AddChildMouseClickEvent("Formula_one", OnButtonFormulaOne);
        AddChildMouseClickEvent("Formula_two", OnButtonFormulaTwo);
        AddChildMouseClickEvent("Formula_three", OnButtonFormulaThree);
        AddChildMouseClickEvent("Formula_four", OnButtonFormulaFour);
        AddChildMouseClickEvent("Materia1", OnButtonChooseMateria);
        AddChildMouseClickEvent("Materia2", OnButtonChooseMateria);
        AddChildMouseClickEvent("Materia3", OnButtonChooseMateria);
        AddChildMouseClickEvent("Materia4", OnButtonChooseMateria);
        AddChildMouseClickEvent("Materia5", OnButtonChooseMateria);
        
    }
    void OnButtonChooseMateria(GameObject obj) 
    {
        Transform texture = obj.transform.Find("Texture");
        if (!texture.gameObject.activeSelf)
        {
            return;
        }
        int materiaId = obj.GetComponent<Parma>().m_id;
        OnClickCardHeadChooseMaterial(null, materiaId);
        
    }
    void OnButtonExit(GameObject obj)
    {
        HideWindow();
        CardDivisionUpdateProp.Singleton.ClearChooseCardList();
        CardDivisionUpdateProp.Singleton.m_curtFormula = (int)CardDivisionUpdateProp.CardDivisionFormula.FormulaOne;
    }
    void OnButtonOK(GameObject obj) 
    {
        //HideWindow();
        UICommonMsgBoxCfg boxCfg = m_buttonOk.transform.GetComponent<UICommonMsgBoxCfg>();
        UICommonMsgBox.GetInstance().ShowMsgBox(OnCardDivisionUpdateOK, OnCardDivisionUpdateNO, boxCfg);
        string formulaText = Localization.Get("EnsureChooseUpdateFormula");
        UICommonMsgBox.GetInstance().GetMainText().SetText(formulaText);
        //如果素材里面有 规定的稀有星级卡牌
        foreach (KeyValuePair<int, CSItem> item in CardDivisionUpdateProp.Singleton.m_chooseCardList)
        {
            WorldParamInfo worldInfo = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enCardDivisionMaterialStarLevel);  
            HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(item.Value.IDInTable);
            if (null != heroInfo && worldInfo.IntTypeValue <= heroInfo.Rarity)
            {
                formulaText = Localization.Get("WarningOfTheMaterial");
                UICommonMsgBox.GetInstance().GetMainText().SetHintText(formulaText);
                UICommonMsgBox.GetInstance().GetMainText().SetHintTextColor(Color.red);
            }
        }
    }

    void OnCardDivisionUpdateOK(object sender, EventArgs e)
    {
        //进入转菊花loading界面
        Loading.Singleton.SetLoadingTips((int)(LOADINGTIPSENUM.enJumpToStore));
        CardDivisionUpdateProp.Singleton.SendDivisionUpdate();
        HideWindow();
    }

    void OnCardDivisionUpdateNO(object sender, EventArgs e)
    {

    }


    void OnButtonFormulaOne(GameObject obj)
    {
        if (CardDivisionUpdateProp.Singleton.m_curtFormula == (int)CardDivisionUpdateProp.CardDivisionFormula.FormulaOne)
        {
            return;
        }
        CardDivisionUpdateProp.Singleton.SetCardDivisionFormula((int)CardDivisionUpdateProp.CardDivisionFormula.FormulaOne);
        ResetMaterialList();
        UpdateInfo();
        CardDivisionUpdateProp.Singleton.ClearChooseCardList();
    }
    void OnButtonFormulaTwo(GameObject obj)
    {
        if (CardDivisionUpdateProp.Singleton.m_curtFormula == (int)CardDivisionUpdateProp.CardDivisionFormula.FormulaTwo)
        {
            return;
        }
        CardDivisionUpdateProp.Singleton.SetCardDivisionFormula((int)CardDivisionUpdateProp.CardDivisionFormula.FormulaTwo);
        ResetMaterialList();
        UpdateInfo();
        CardDivisionUpdateProp.Singleton.ClearChooseCardList();
    }
    void OnButtonFormulaThree(GameObject obj)
    {
        if (CardDivisionUpdateProp.Singleton.m_curtFormula == (int)CardDivisionUpdateProp.CardDivisionFormula.FormulaThree)
        {
            return;
        }
        CardDivisionUpdateProp.Singleton.SetCardDivisionFormula((int)CardDivisionUpdateProp.CardDivisionFormula.FormulaThree);
        ResetMaterialList();
        UpdateInfo();
        CardDivisionUpdateProp.Singleton.ClearChooseCardList();
    }
    void OnButtonFormulaFour(GameObject obj)
    {
        if (CardDivisionUpdateProp.Singleton.m_curtFormula == (int)CardDivisionUpdateProp.CardDivisionFormula.FormulaFour)
        {
            return;
        }
        CardDivisionUpdateProp.Singleton.SetCardDivisionFormula((int)CardDivisionUpdateProp.CardDivisionFormula.FormulaFour);
        ResetMaterialList();
        UpdateInfo();
        CardDivisionUpdateProp.Singleton.ClearChooseCardList();
    }

   
    public override void OnShowWindow()
    {
        base.OnShowWindow();
        ShowCardDivisionPanel();
        HideCardEvolutionPanel();
        UpdateInfo();
    }
    public override void OnHideWindow() 
    {
        base.OnHideWindow();
        //清空素材列表
        ResetMaterialList();
    }
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
        foreach(UISprite item in m_materialSpritList)
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

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (null == WindowRoot)
        {
            return;
        }
        if (eventType == (int)CardDivisionUpdateProp.CardDivisionUpdate.enChooseCard)
        {
            UpdateMaterial();
            UpdateInfo();
        }

    }
    //更新选择的素材卡牌的List
    void UpdateMaterial() 
    {
        foreach (KeyValuePair<int, CSItem> item in CardDivisionUpdateProp.Singleton.m_chooseCardList)
        {
            UISprite tempSprite = m_materialSpritList[item.Key];
            if (null != tempSprite)
            {
                //移除以前的素材卡牌头像
                Transform cardHead = tempSprite.transform.Find("UICardHead");
                if (null != cardHead)
                {
                    GameObject.Destroy(cardHead.gameObject);
                }
                //添加新素材卡牌头像
                UICardHead tempCardHead = UICardHead.Create();
                tempCardHead.m_materialId = item.Key;
                m_materialList.Add(tempCardHead);//加入到素材列表
                tempCardHead.SetParent(tempSprite.transform);
                tempCardHead.SetLocalPosition(Vector3.zero);
                tempCardHead.SetCardInfo(item.Value);
                tempCardHead.RegisterCallBack(OnClickCardHeadChooseMaterial, PressCardHead);
                tempCardHead.ShowCard();
                tempCardHead.SetCardInfoShow(false);
                tempCardHead.SetCardLoveShow(false);
                tempSprite.GetComponent<Parma>().m_type = 1;
            }
        }
    }
    //点击已经选择的素材卡牌头像 显示素材卡牌界面
    void OnClickCardHeadChooseMaterial(CSItem temp, int materialId) 
    {
        CSItem card = CardBag.Singleton.GetCardByGuid(CardDivisionUpdateProp.Singleton.m_curDivisionCardGuid);
        //设置素材卡牌
        GradeUpRequireInfo gradeInfo = GameTable.gradeUpRequireAsset.Lookup(card.m_id);
        if (null == gradeInfo)
        {
            return;
        }
        int curtFormula = CardDivisionUpdateProp.Singleton.m_curtFormula;
        FormulaInfo tempFormula = gradeInfo.FormulaList[curtFormula - 2];
        if (null == tempFormula)
        {
            return;
        }
        FormulaParam param = tempFormula.ParamList[materialId];
        OperateCardList.Singleton.OnShowDivisionCard(param.paramOccu, param.paramLevel, param.paramRarity, materialId);
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enOperaterCardList);
    }

    //更新段位升级界面上所有信息
    void UpdateInfo() 
    {
        //设置段位升级前卡牌头像
        if (null == m_cardBeforHead)
        {
            m_cardBeforHead = UICardHead.Create();
            m_cardBeforHead.SetParent(m_cardBefor);
            m_cardBeforHead.SetLocalPosition(Vector3.zero);
        }
        CSItem card = CardBag.Singleton.GetCardByGuid(CardDivisionUpdateProp.Singleton.m_curDivisionCardGuid);
        if (null == card)
        {
            return;
        }
        m_cardBeforHead.SetCardInfo(card);
        m_cardBeforHead.RegisterCallBack(null, PressCardHead);
        m_cardBeforHead.ShowCard();
        m_cardBeforHead.SetCardInfoShow(false);
        m_cardBeforHead.SetCardLoveShow(false);
        
        //设置段位升级后卡牌头像
        if (null == m_cardAfterHead)
        {
            m_cardAfterHead = UICardHead.Create();
            m_cardAfterHead.SetParent(m_cardAfter);
            m_cardAfterHead.SetLocalPosition(Vector3.zero);
        }
        m_cardAfterHead.SetCardInfo(card);
        m_cardAfterHead.RegisterCallBack(null, PressCardHead);
        m_cardAfterHead.ShowCard();
        m_cardAfterHead.SetCardInfoShow(false);
        m_cardAfterHead.SetCardLoveShow(false);
        //设置素材卡牌
        GradeUpRequireInfo gradeInfo = GameTable.gradeUpRequireAsset.Lookup(card.m_id);
        if (null == gradeInfo)
        {
            return;
        }
        int curtFormula = CardDivisionUpdateProp.Singleton.m_curtFormula;
        //根据当前选择公式 显示素材卡牌
        switch (curtFormula)
        {
            case (int)CardDivisionUpdateProp.CardDivisionFormula.FormulaOne:
                SetFormulaOneMaterial(gradeInfo,card);
                break;
            case (int)CardDivisionUpdateProp.CardDivisionFormula.FormulaTwo:
                SetFormulaMaterial(gradeInfo, card, curtFormula);
                break;
            case (int)CardDivisionUpdateProp.CardDivisionFormula.FormulaThree:
                SetFormulaMaterial(gradeInfo, card, curtFormula);
                break;
            case (int)CardDivisionUpdateProp.CardDivisionFormula.FormulaFour:
                SetFormulaMaterial(gradeInfo, card, curtFormula);
                break;
            default:
                break;
        }
        //设置属性面板显示
        SetCardPropPanel(card);
        //设置公式按钮
        SetFormulaButton(gradeInfo);
        //设置3D动态头像
        SetCard3DHead(card);
        //检查素材是否齐全
        CheckMaterialEnough(gradeInfo);
    }
    //检查素材是否齐全
    void CheckMaterialEnough(GradeUpRequireInfo gradeInfo) 
    {
        if (null == gradeInfo)
        {
            return;
        }
        int money = User.Singleton.UserProps.GetProperty_Int32(UserProperty.money);
        bool isCanDivision = true;
        int curtFormula = CardDivisionUpdateProp.Singleton.m_curtFormula;
        int costMoney = 0;
        if (curtFormula == (int)CardDivisionUpdateProp.CardDivisionFormula.FormulaOne)
        {
            costMoney = gradeInfo.Formula1_Cost;
            if (m_materialSpritList[0].GetComponent<Parma>().m_type == 0)
            {
                isCanDivision = false;
            }
        }
        else
        {
            FormulaInfo tempFormula = gradeInfo.FormulaList[curtFormula - 2];
            if (null == tempFormula)
            {
                return;
            }
            costMoney = tempFormula.Formula_Cost;
            //检查素材
            for (int i = 0; i < tempFormula.Param_Num; i++)
            {
                if (m_materialSpritList[i].GetComponent<Parma>().m_type == 0)
                {
                    isCanDivision = false;
                }
            }
        }
        if (money < costMoney)
        {
            isCanDivision = false;
        }
        m_buttonOk.isEnabled = isCanDivision;
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

    //设置属性面板显示
    void SetCardPropPanel(CSItem card) 
    {
        //升段前属性
        int beaforLevel = card.Level;//当前等级
        int beaforMaxLevel = card.GetMaxLevel();//最大等级
        m_danBefor.transform.Find("LevelTips").GetComponent<UILabel>().text = beaforLevel.ToString() + '/' + beaforMaxLevel.ToString();

        GradeUpRequireInfo gradeInfo = GameTable.gradeUpRequireAsset.Lookup(card.m_id);
        if (null == gradeInfo)
        {
            return;
        }
        int beaforBreak = card.BreakCounts;//当前突破次数
        int maxBreak = gradeInfo.GradeUpTime;//最大突破次数
        m_danBefor.transform.Find("Dan").GetComponent<UILabel>().text = beaforBreak.ToString() + '/' + maxBreak.ToString();

        int beaforHp = card.GetHp();//当前生命值
        m_danBefor.transform.Find("HP").GetComponent<UILabel>().text = beaforHp.ToString();

        int beaforAttack = card.GetPhyAttack();//物理攻击力
        m_danBefor.transform.Find("Attack").GetComponent<UILabel>().text = beaforAttack.ToString();

        int beaforMagAtt = card.GetMagAttack();//魔法攻击力
        m_danBefor.transform.Find("MagAttack").GetComponent<UILabel>().text = beaforMagAtt.ToString();

        //升段后属性
        int afterLevel = card.Level;//升级后等级
        int afterMaxLevel = card.GetMaxLevel() + gradeInfo.LevelLimitUp;//最大等级
        m_danAfter.transform.Find("LevelTips").GetComponent<UILabel>().text = afterLevel.ToString() + '/' + afterMaxLevel.ToString();

        int afterBreak = card.BreakCounts + 1;//当前突破次数
        m_danAfter.transform.Find("Dan").GetComponent<UILabel>().text = afterBreak.ToString() + '/' + maxBreak.ToString();

        int afterHp = card.GetHp() + gradeInfo.HpUp;//升级后生命值
        m_danAfter.transform.Find("HP").GetComponent<UILabel>().text = afterHp.ToString() + "(+"+gradeInfo.HpUp.ToString()+')';

        int afterAttack = card.GetPhyAttack() + gradeInfo.AttackUp;//升级后攻击力
        m_danAfter.transform.Find("Attack").GetComponent<UILabel>().text = afterAttack.ToString() + "(+"+gradeInfo.AttackUp.ToString()+')';

        int afterMagAtt = card.GetMagAttack() + gradeInfo.MagicAttackUp;//升级后攻击力
        m_danAfter.transform.Find("MagAttack").GetComponent<UILabel>().text = afterMagAtt.ToString() + "(+" + gradeInfo.MagicAttackUp.ToString() + ')';

    }

    //设置公式按钮
    void SetFormulaButton(GradeUpRequireInfo gradeInfo) 
    {
        m_formulaButton4.isEnabled =  (gradeInfo.FormulaNum < (int)CardDivisionUpdateProp.CardDivisionFormula.FormulaFour ? false : true);
        m_formulaButton3.isEnabled = (gradeInfo.FormulaNum < (int)CardDivisionUpdateProp.CardDivisionFormula.FormulaThree ? false : true);
        m_formulaButton2.isEnabled = (gradeInfo.FormulaNum < (int)CardDivisionUpdateProp.CardDivisionFormula.FormulaTwo ? false : true);
    }

    //设置3D头像
    void SetCard3DHead(CSItem card) 
    {
        int childCount = m_beforModel.transform.childCount;
        for (int i = 0; i < childCount; i++ )
        {
            Transform child = m_beforModel.transform.GetChild(i);
            GameObject.Destroy(child.gameObject);
            
        }
        AddModel(card,m_beforModel);
        int childCount1 = m_afterModel.transform.childCount;
        for (int i = 0; i < childCount1; i++)
        {
            Transform child = m_afterModel.transform.GetChild(i);
            GameObject.Destroy(child.gameObject);

        }
        AddModel(card,m_afterModel);
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

    //设置公式一升段素材
    void SetFormulaOneMaterial(GradeUpRequireInfo info, CSItem card) 
    {
        SetCostMoney(info.Formula1_Cost);//设置公式一升段所需金钱
        //设置素材卡牌头像 与 段位升级卡头像相同
        UICardHead tempCardHead = UICardHead.Create();
        m_materialList.Add(tempCardHead);//加入到素材列表
        tempCardHead.SetParent(m_materialSpritList[0].transform);
        tempCardHead.SetLocalPosition(Vector3.zero);
        //获得公式一卡牌
        CSItem materialCard = User.Singleton.ItemBag.GetCardBuyIdCardDivisionFormula(card);
        CardDivisionUpdateProp.Singleton.AddMaterialCard(materialCard);
        if (null == materialCard)//如果背包里只有一张升段卡牌或者没有所需卡牌时
        {
            tempCardHead.SetCardMask(true);//素材显示黑色遮罩
            //升段确定按钮不可点击
            m_buttonOk.isEnabled = false;
            //如果背包里面没有这张卡则创建一张只带有ID的卡牌从表里读取信息
            CSItem tempCard = new CSItem();
            tempCard.m_id = card.m_id;
            tempCardHead.SetCardInfo(tempCard);
        }
        else
        {
            m_materialSpritList[0].GetComponent<Parma>().m_type = 1;
            //如果表里有这张卡 则显示素材卡信息
            tempCardHead.SetCardInfo(materialCard);
        }
        tempCardHead.RegisterCallBack(null, PressCardHead);
        tempCardHead.ShowCard();  
        tempCardHead.SetCardInfoShow(false);
        tempCardHead.SetCardLoveShow(false);
    }

    //设置其他公式升段素材
    void SetFormulaMaterial(GradeUpRequireInfo info, CSItem card,int formulaIndex)
    {
        FormulaInfo tempFormula = info.FormulaList[formulaIndex - 2];
        if (null == tempFormula)
        {
            return;
        }
        //设置金币数量
        int cost = tempFormula.Formula_Cost;
        SetCostMoney(cost);
        //设置素材卡牌
        for (int i = 0; i < tempFormula.Param_Num; i++)
        {
            switch (tempFormula.ParamList[i].paramType) 
            {
                case(int)MaterialType.enAppoint:
                    AddAppointCard(m_materialSpritList[i], tempFormula.ParamList[i],i);
                    break;
                case(int)MaterialType.enUnAppoint:
                    AddUnAppoint(m_materialSpritList[i], tempFormula.ParamList[i]);
                    break;
                case(int)MaterialType.enRingOfHonor:
                    AddRingOfHonor(m_materialSpritList[i], tempFormula.ParamList[i]);
                    break;
                default:
                    break;
            }
        }
    }
    
    //添加指定卡牌素材
    void AddAppointCard(UISprite sprite,FormulaParam param,int index) 
    {
        UICardHead tempHead = UICardHead.Create();
        m_materialList.Add(tempHead);//加入到素材列表
        tempHead.SetParent(sprite.transform);
        tempHead.SetLocalPosition(Vector3.zero);
        tempHead.RegisterCallBack(null, PressCardHead);
        CSItem tempCard = new CSItem();
        tempCard.m_id = (short)param.paramId;
        tempHead.SetCardInfo(tempCard);

        tempHead.ShowCard();
        tempHead.SetCardLoveShow(false);

        //获得这张ID的卡牌在背包中有多少张
        int cardCount = User.Singleton.ItemBag.GetCardNumById(param.paramId);
        tempHead.m_cardInfo.text = string.Format(Localization.Get("WithTheMaterial"), cardCount);
        int tempCardCount = cardCount;
        foreach (UICardHead item in m_materialList)
        {
            if (item != tempHead && tempHead.m_showCard.m_id == param.paramId)
            {
                tempCardCount--;
            }
            if (item == tempHead)
            {
                if (tempCardCount > 0)
                {
                    tempHead.m_cardInfo.color = Color.green;
                }
                else 
                {
                    tempHead.m_cardInfo.color = Color.red;
                    tempHead.SetCardMask(true);
                }
            }
        }
    }

    //添加非指定素材
    void AddUnAppoint(UISprite sprite, FormulaParam param) 
    {
        Transform texure = sprite.transform.Find("Sprite");
        Transform starTexure = sprite.transform.Find("Texture");
        WorldParamInfo worldInfo = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enCardDivisionUnAppointIcon);
        IconInfomation imageInfo = GameTable.IconInfoTableAsset.Lookup(worldInfo.IntTypeValue);
        texure.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(imageInfo.dirName);
        Transform label = sprite.transform.Find("MateriaLabel");
        int level = param.paramLevel;
        int occ = param.paramOccu;
        int star = param.paramRarity;
        OccupationInfo occTable = GameTable.OccupationInfoAsset.LookUp(occ);
        if (null == occTable)
        {
            return;
        }
        label.GetComponent<UILabel>().text = string.Format(Localization.Get("Occupation"), occTable.m_name, level);
        label.GetComponent<UILabel>().color = Color.red;
        label.gameObject.SetActive(true);
        //设置星级图标
        RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(star);
        if (null == rarityInfo)
        {
            Debug.Log("RarityRelativeInfo rarityInfo == null info.RarityId:" + star);
            return;
        }
        IconInfomation rarityIcon = GameTable.IconInfoTableAsset.Lookup(rarityInfo.m_iconId);
        if (null == rarityIcon)
        {
            Debug.Log("IconInfomation rarityIcon == null rarityInfo.m_iconId:" + rarityInfo.m_iconId);
            return;
        }
        starTexure.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(rarityIcon.dirName);
        starTexure.gameObject.SetActive(true);
    }
    //添加荣誉戒指素材
    void AddRingOfHonor(UISprite sprite, FormulaParam param) 
    {
        Transform texure = sprite.transform.Find("Sprite");
        WorldParamInfo worldInfo = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enCardDivisionRingHonorIcon);
        IconInfomation imageInfo = GameTable.IconInfoTableAsset.Lookup(worldInfo.IntTypeValue);
        texure.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(imageInfo.dirName);

        Transform label = sprite.transform.Find("MateriaLabel");
        int ringNum = User.Singleton.UserProps.GetProperty_Int32(UserProperty.ring);
        label.GetComponent<UILabel>().text = string.Format(Localization.Get("WithTheMaterial"), ringNum);
        if (ringNum < param.paramRingNum)
        {
            label.GetComponent<UILabel>().color = Color.red;
        }
        else
        {
            sprite.GetComponent<Parma>().m_type = 1;
            label.GetComponent<UILabel>().color = Color.green;
        }
        label.gameObject.SetActive(true);
    }

    //长按显示卡牌详情界面 只带返回按钮
    void PressCardHead(CSItem card)
    {
        CardBag.Singleton.OnShowCardDetail(card,true);
    }

    //隐藏卡牌进阶相关面板
    void HideCardEvolutionPanel() 
    {
        m_evolutionAfter.SetActive(false);
        m_evolutionBefore.SetActive(false);
    }

    //显示段位升级相关面板
    void ShowCardDivisionPanel() 
    {
        m_danAfter.SetActive(true);
        m_danBefor.SetActive(true);
        m_formulaPanel.gameObject.SetActive(true);
        //设置面板文字
        Transform headTransform = m_panelHead.transform.Find("Label");
        headTransform.GetComponent<UILabel>().text = Localization.Get("DanUpgrading");
        Transform promotionBefore = m_promotionBefore.transform.Find("Label");
        promotionBefore.GetComponent<UILabel>().text = Localization.Get("DanBeforeAscension");
        Transform promotionAfter = m_promotionAfter.transform.Find("Label");
        promotionAfter.GetComponent<UILabel>().text = Localization.Get("DanAfterPromotion");
    }
}
