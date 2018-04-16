using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

class Info
{
    public enum ENTabelType
    {
        enNone,
        enHero,
        enNPC,
    }
    public ENTabelType m_tabelType = ENTabelType.enNone;
    public int m_tabelID = 0;
    public int m_modelType = 0;
    public int m_weaponType = 0;
    public class AnimInfo
    {
        public enum ENSkillStepType
        {
            enNone,
            enPrepare,//准备
            enSpell,//吟唱
            enSlash,//冲锋
            enSlashBlend,//冲锋融合
            enRelease,//释放
            enReleaseBlend,//释放融合
            enConduct,//引导
            enEndConduct,//引导结束
        }
        public string m_animName;
        public int m_skillID;
        public ENSkillStepType m_skillStepType = ENSkillStepType.enNone;

        public AnimInfo(string animName, int skillID, ENSkillStepType type)
        {
            m_animName = animName;
            m_skillID = skillID;
            m_skillStepType = type;
        }
    }
    //key:animName(加了技能id等前缀)
    public Dictionary<string, AnimInfo> m_animMap = new Dictionary<string, AnimInfo>();

    public void Clear()
    {
        m_tabelType = ENTabelType.enNone;
        m_tabelID = 0;
        m_modelType = 0;
        m_weaponType = 0;
        m_animMap.Clear();
    }
}

class AnimationEventPanel : EditorWindow
{
    protected static GUIContent LABEL_HERO_ID = new GUIContent(@"卡牌  ID：", "");
    public static GameObject ObjPanel
    {
        get
        {
            GameObject obj = GameObject.Find("AnimationEventPanel");
            if (obj == null)
            {
                obj = new GameObject("AnimationEventPanel");
            }
            return obj;
        }
    }
    public static GameObject ObjEffect
    {
        get
        {
            Transform objT = ObjPanel.transform.Find("AnimationEventPanel_Effect");
            if (objT == null)
            {
                GameObject obj = new GameObject("AnimationEventPanel_Effect");
                obj.transform.parent = ObjPanel.transform;
                obj.AddComponent<EDSkillEventTrigger>();
                return obj;
            }
            else
            {
                return objT.gameObject;
            }
        }
    }
    public List<string> m_loadedAnimationList=new List<string>();
    public List<string> m_skillAnimList = new List<string>();
    int m_playingAniIndex;


    //卡牌表
    HeroInfoTable m_heroTabel = null;
    //npc表
    NPCInfoTable m_npcTabel = null;
    //模型表
    ModelInfoTable m_modelTable = null;
    //武器表
    WeaponInfoTable m_weaponTable = null;
    //技能表
    SkillTable m_skillTable = null;
    //卡牌名称列表
    List<string> m_nameList = new List<string>();
    //卡片id列表
    List<int> m_idList = new List<int>();
    //表的索引，0是Hero表，1是NPC表
    int m_tableIndex = 0;
    //卡牌名称列表和卡片id列表的索引
    int m_index = 0;
    //当前卡牌的obj
    GameObject m_currentObj = null;
    //曲线类型列表
    List<Type> m_typeList = new List<Type>();
    //信息
    Info m_currentInfo = new Info();

    [MenuItem(@"EditorTools/Animation关键帧编辑器")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(AnimationEventPanel));
    }
    void GuardInitMyData()
    {
        if (m_typeList.Count == 0)
        {
            m_typeList.Add(typeof(UnityEngine.Collider));
        }
        if (m_heroTabel == null)
        {
            TextAsset asset = GameData.LoadConfig<TextAsset>("HeroInfoTable");
            m_heroTabel = new HeroInfoTable();
            m_heroTabel.Load(asset.bytes);
            m_nameList.Clear();
            m_idList.Clear();
            foreach (var item in m_heroTabel.m_list.Values)
            {
                m_nameList.Add(item.ID + "-" + item.StrName);
                m_idList.Add(item.ID);
            }
        }
        if (m_npcTabel == null)
        {
            TextAsset asset = GameData.LoadConfig<TextAsset>("NPCInfoTable");
            m_npcTabel = new NPCInfoTable();
            m_npcTabel.Load(asset.bytes);
        }
        if (m_modelTable == null)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("ModelInfoTable");
            m_modelTable = new ModelInfoTable();
            m_modelTable.Load(obj.bytes);
        }
        if (m_weaponTable == null)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("WeaponInfoTable");
            m_weaponTable = new WeaponInfoTable();
            m_weaponTable.Load(obj.bytes);
        }
        if (m_skillTable == null)
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("SkillTable");
            m_skillTable = new SkillTable();
            m_skillTable.Load(obj.bytes);
        }
    }
    void OnGUI()
    {
        Repaint();
        GuardInitMyData();
        //EditorGUILayout.LabelField("", GUILayout.Width(0));
        int tableIndex = EditorGUILayout.Popup(@"卡牌表分类：", m_tableIndex, new string[] { "Hero", "NPC" });
        if (tableIndex != m_tableIndex)
        {
            AssetDatabase.SaveAssets();
            Delete();

            m_index = 0;

            m_tableIndex = tableIndex;
            m_nameList.Clear();
            m_idList.Clear();
            if (m_tableIndex == 0)
            {
                foreach (var item in m_heroTabel.m_list.Values)
                {
                    m_nameList.Add(item.ID + "-" + item.StrName);
                    m_idList.Add(item.ID);
                }
            }
            else
            {
                foreach (var item in m_npcTabel.m_list.Values)
                {
                    m_nameList.Add(item.ID + "-" + item.StrName);
                    m_idList.Add(item.ID);
                }
            }
        }
        //EditorGUILayout.LabelField("", GUILayout.Width(0));
        m_index = EditorGUILayout.Popup(@"卡牌名称：", m_index, m_nameList.ToArray());
        //EditorGUILayout.LabelField("", GUILayout.Width(0));
        int id = EditorGUILayout.IntField(LABEL_HERO_ID, m_idList[m_index]);
        if (id != m_idList[m_index])
        {
            if (m_idList.Contains(id))
            {
                m_index = m_idList.FindIndex(item => item == id);
            }
        }
        //EditorGUILayout.LabelField("", GUILayout.Width(0));

        //////////////////////////////////////////////////////////////////////////
        #region//播放相关
        int oldAniIndex = m_playingAniIndex;
        m_playingAniIndex = EditorGUILayout.Popup(@"动画：", oldAniIndex, m_loadedAnimationList.ToArray());
        //m_copyIndex = EditorGUILayout.Popup(@"copy来源的卡牌名称：", m_copyIndex, m_nameList.ToArray());
        if (oldAniIndex != m_playingAniIndex)
        {
            PlaySkill(m_playingAniIndex);
        }
        Rect rct = new Rect(0, 100, 100, 100);
        GUI.Label(rct, @"动画播放相关：");
        rct = new Rect(150, 100, 100, 30);
        if (GUI.Button(rct, @"Play"))
        {
            PlaySkill(m_playingAniIndex);
        }
        rct = new Rect(260, 100, 100, 30);
        if (GUI.Button(rct, @"FixCamera"))
        {
            FixSceneCamera();
        }
        rct = new Rect(370, 100, 100, 30);
        if (GUI.Button(rct, @"Next"))
        {
            NextDeltaTime();
        }
        rct = new Rect(480, 100, 100, 30);
        if (GUI.Button(rct, @"PlaySelections"))
        {
            PlaySelectionsEffect();
        }
        rct = new Rect(590, 100, 100, 30);
        if (GUI.Button(rct, @"Pause/Resume"))
        {
            if (Time.timeScale < 0.01)
            {
                Time.timeScale = 1.0f;
            }
            else
            {
                Time.timeScale = 0.0f;
            }
        }
        #endregion
        //////////////////////////////////////////////////////////////////////////
        #region//卡牌编辑相关
        rct = new Rect(0, 140, 100, 100);
        GUI.Label(rct, @"卡牌编辑相关：");
        rct = new Rect(150, 140, 100, 30);
        if (GUI.Button(rct, @"Open"))
        {
            AssetDatabase.SaveAssets();
            Open();
            AssetDatabase.SaveAssets();
        }
        rct = new Rect(260, 140, 100, 30);
        if (GUI.Button(rct, @"Save"))
        {
            AssetDatabase.SaveAssets();
            Save();
            AssetDatabase.SaveAssets();
        }
        rct = new Rect(370, 140, 100, 30);
        if (GUI.Button(rct, @"Delete"))
        {
            AssetDatabase.SaveAssets();
            Delete();
            AssetDatabase.SaveAssets();
        }
        //rct = new Rect(480, 140, 100, 30);
        //if (GUI.Button(rct, @"Convert"))
        //{//转换所有animation
        //    AssetDatabase.SaveAssets();
        //    Convert();
        //    AssetDatabase.SaveAssets();
        //}
        //rct = new Rect(590, 140, 100, 30);
        //if (GUI.Button(rct, @"Save All"))
        //{
        //    AssetDatabase.SaveAssets();
        //    SaveAll();
        //    AssetDatabase.SaveAssets();
        //}
        #endregion
        //////////////////////////////////////////////////////////////////////////
        #region//读表相关
        rct = new Rect(0, 180, 100, 100);
        GUI.Label(rct, @"重新读表相关：");
        rct = new Rect(150, 180, 100, 30);
        if (GUI.Button(rct, @"Hero Table"))
        {
            TextAsset asset = GameData.LoadConfig<TextAsset>("HeroInfoTable");
            m_heroTabel.Load(asset.bytes);
            if (m_tableIndex == 0)
            {
                m_nameList.Clear();
                m_idList.Clear();
                foreach (var item in m_heroTabel.m_list.Values)
                {
                    m_nameList.Add(item.ID + "-" + item.StrName);
                    m_idList.Add(item.ID);
                }
            }
        }
        rct = new Rect(260, 180, 100, 30);
        if (GUI.Button(rct, @"NPC Table"))
        {
            TextAsset asset = GameData.LoadConfig<TextAsset>("NPCInfoTable");
            m_npcTabel.Load(asset.bytes);
            if (m_tableIndex == 1)
            {
                m_nameList.Clear();
                m_idList.Clear();
                foreach (var item in m_npcTabel.m_list.Values)
                {
                    m_nameList.Add(item.ID + "-" + item.StrName);
                    m_idList.Add(item.ID);
                }
            }
        }
        rct = new Rect(370, 180, 100, 30);
        if (GUI.Button(rct, @"Model Table"))
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("ModelInfoTable");
            m_modelTable.Load(obj.bytes);
        }
        rct = new Rect(480, 180, 100, 30);
        if (GUI.Button(rct, @"Weapon Table"))
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("WeaponInfoTable");
            m_weaponTable.Load(obj.bytes);
        }
        rct = new Rect(590, 180, 100, 30);
        if (GUI.Button(rct, @"Skill Table"))
        {
            TextAsset obj = GameData.LoadConfig<TextAsset>("SkillTable");
            m_skillTable.Load(obj.bytes);
        }
        #endregion
    }
    //打开
    void Open(bool isTest = false, int testID = 0, Info.ENTabelType testType = Info.ENTabelType.enNone)
    {
        Delete();
        Info.ENTabelType type = Info.ENTabelType.enNone;
        int modelID = 0, weaponID = 0;
        List<int> allSkillIDList = new List<int>();
        int id = 0;
        if (isTest)
        {//测试用
            id = testID;
            if (testType == Info.ENTabelType.enHero)
            {
                type = Info.ENTabelType.enHero;
                HeroInfo heroInfo = m_heroTabel.Lookup(id);
                modelID = heroInfo.ModelId;
                weaponID = heroInfo.WeaponId;
                allSkillIDList = heroInfo.GetAllSkillIDList();
            }
            else
            {
                type = Info.ENTabelType.enNPC;
                NPCInfo npcInfo = m_npcTabel.Lookup(id);
                modelID = npcInfo.ModelId;
                weaponID = npcInfo.WeaponID;
                allSkillIDList.AddRange(npcInfo.SkillList);
                allSkillIDList.Add(npcInfo.StaminaSkillID);
            }
        }
        else
        {
            id = m_idList[m_index];
            if (m_tableIndex == 0)
            {
                type = Info.ENTabelType.enHero;
                HeroInfo heroInfo = m_heroTabel.Lookup(id);
                modelID = heroInfo.ModelId;
                weaponID = heroInfo.WeaponId;
                allSkillIDList = heroInfo.GetAllSkillIDList();
            }
            else
            {
                type = Info.ENTabelType.enNPC;
                NPCInfo npcInfo = m_npcTabel.Lookup(id);
                modelID = npcInfo.ModelId;
                weaponID = npcInfo.WeaponID;
                allSkillIDList.AddRange(npcInfo.SkillList);
                allSkillIDList.Add(npcInfo.StaminaSkillID);
            }
        }

        ModelInfo modelInfo = m_modelTable.Lookup(modelID);
        if (modelInfo == null)
        {
            Debug.LogError("model error,modelID:"+modelID+",card id:"+id+",type:"+type.ToString());
            return;
        }
        WeaponInfo weaponInfo = m_weaponTable.Lookup(weaponID);
        if (weaponInfo == null)
        {
            Debug.LogError("weapon error,weaponID:" + weaponID + ",card id:" + id + ",type:" + type.ToString());
            return;
        }

        m_currentInfo.m_tabelType = type;
        m_currentInfo.m_tabelID = id;
        m_currentInfo.m_modelType = modelInfo.modelType;
        m_currentInfo.m_weaponType = (int)weaponInfo.WeaponType;
        List<int> skillIDList = new List<int>();
        foreach (var item in allSkillIDList)
        {//删除相同的技能id
            if (!skillIDList.Contains(item))
            {
                skillIDList.Add(item);
            }
        }
        GetAnimationList(skillIDList);

        //加载模型，并给模型加一个父
        GameObject parentObj = new GameObject();
        m_currentObj = parentObj;
        parentObj.transform.parent = ObjPanel.transform;
        GameObject modelObj = GameObject.Instantiate(GameData.LoadPrefab<GameObject>(modelInfo.ModelFile)) as GameObject;
        modelObj.name = "body";
        ObjEffect.GetComponent<EDSkillEventTrigger>().MainObj = modelObj;
        parentObj.name = modelObj.name + "_parent";
        modelObj.transform.parent = parentObj.transform;

        parentObj.AddComponent<AnimationCameraCallback>();
        parentObj.AddComponent<AnimationEffectCallback>();
        parentObj.AddComponent<AnimationEndCallback>();
        parentObj.AddComponent<AnimationShaderParamCallback>();
        parentObj.AddComponent<AnimationSoundCallback>();
        parentObj.AddComponent<BreakPointCallback>();
        parentObj.AddComponent<AttackActionCallback>();
        #region//加载模型的父的animation
        Animation parentAnim = parentObj.AddComponent<Animation>();
        string path = "Assets/Resources/SkillAnim/";
        foreach (var item in m_skillAnimList)
        {
            Info.AnimInfo info = null;
            m_currentInfo.m_animMap.TryGetValue(item, out info);

            string filename = m_currentInfo.m_modelType.ToString() + "-" +
                            m_currentInfo.m_weaponType.ToString() + "-" + 
                            info.m_skillID.ToString() + "-" + ((int)info.m_skillStepType).ToString() + "-" + info.m_animName + ".anim";
            string assetPath = path + filename;
            AnimationClip clip = AssetDatabase.LoadMainAssetAtPath(assetPath) as AnimationClip;
            if (clip != null)
            {
                parentAnim.AddClip(clip, item);
            }
            else
            {
                Debug.Log("load clip failed, path:" + assetPath);
            }
        }
        #endregion
        #region//加载模型的脚本和animation
        if (modelObj.GetComponent<AnimationCameraCallback>() == null)
        {
            modelObj.AddComponent<AnimationCameraCallback>();
        }
        if (modelObj.GetComponent<AnimationEffectCallback>() == null)
        {
            modelObj.AddComponent<AnimationEffectCallback>();
        }
        if (modelObj.GetComponent<AnimationEndCallback>() == null)
        {
            modelObj.AddComponent<AnimationEndCallback>();
        }
        if (modelObj.GetComponent<AnimationShaderParamCallback>() == null)
        {
            modelObj.AddComponent<AnimationShaderParamCallback>();
        }
        if (modelObj.GetComponent<AnimationSoundCallback>() == null)
        {
            modelObj.AddComponent<AnimationSoundCallback>();
        }
        if (modelObj.GetComponent<BreakPointCallback>() == null)
        {
            modelObj.AddComponent<BreakPointCallback>();
        }
        if (modelObj.GetComponent<AttackActionCallback>() == null)
        {
            modelObj.AddComponent<AttackActionCallback>();
        }
        if (LoadAnimationClip(modelObj, modelInfo.modelType, (int)weaponInfo.WeaponType))
        {
            Animation modelAnim = modelObj.GetComponent<Animation>();
            //加载模型的事件
            foreach (var item in m_skillAnimList)
            {
                Info.AnimInfo info = null;
                m_currentInfo.m_animMap.TryGetValue(item, out info);
                AnimationClip copyClip = modelAnim.GetClip(GetFullAnimName(info.m_animName, m_currentInfo.m_modelType, m_currentInfo.m_weaponType));
                if (copyClip == null)
                {
                    Debug.LogError("body get animation failed, anim:" + info.m_animName);
                }
                modelAnim.AddClip(copyClip, item);
                AnimationClip modelClip = modelAnim.GetClip(item);

                AnimationClip parentClip = parentAnim.GetClip(item);
                if (parentClip != null)
                {//从parentObj中获取事件信息
                    AnimationClipCurveData[] array = AnimationUtility.GetAllCurves(parentClip);
                    foreach (var data in array)
                    {
                        if (!m_typeList.Contains(data.type.BaseType))
                        {
                            continue;
                        }
                        string dataPath = "";
                        if (data.path.Contains("body"))
                        {
                            dataPath = data.path.Substring(4);
                        }
                        else if (data.path.Contains("body/"))
                        {
                            dataPath = data.path.Substring(5);
                        }
                        modelClip.SetCurve(dataPath, data.type, data.propertyName, data.curve);
                    }
                    AnimationUtility.SetAnimationEvents(modelClip, AnimationUtility.GetAnimationEvents(parentClip));
                }
                else
                {//获取原来的事件信息，存储在resources/skillanim目录下
                    string dir = "assets/resources/skillanim/";
                    string animName = "a-" + m_currentInfo.m_modelType.ToString() + "-w" +
                            m_currentInfo.m_weaponType.ToString() + "-" + info.m_animName + ".anim";
                    UnityEngine.Object tempObj = AssetDatabase.LoadMainAssetAtPath(dir + animName);
                    if (tempObj == null)
                    {
                        //Debug.LogWarning("animation name:" + animName);
                        //resources/skillanim目录下不存在
                        //从原动作中获取
                        AnimationClip bodyClip = modelAnim.GetClip(info.m_animName);
                        if (bodyClip == null)
                        {
                            Debug.LogError("animation is null, name:" + info.m_animName);
                        }
                        else
                        {
                            AnimationClipCurveData[] array = AnimationUtility.GetAllCurves(bodyClip);
                            foreach (var data in array)
                            {
                                if (!m_typeList.Contains(data.type.BaseType))
                                {
                                    continue;
                                }
                                //if (!string.IsNullOrEmpty(data.path))
                                //{//不是body下的
                                //    continue;
                                //}
                                modelClip.SetCurve(data.path, data.type, data.propertyName, data.curve);
                            }
                            AnimationUtility.SetAnimationEvents(modelClip, AnimationUtility.GetAnimationEvents(bodyClip));
                        }
                    }
                    else
                    {
                        AnimationClip clip = GameObject.Instantiate(tempObj) as AnimationClip;
                        AnimationClipCurveData[] array = AnimationUtility.GetAllCurves(clip);
                        foreach (var data in array)
                        {
                            if (!m_typeList.Contains(data.type.BaseType))
                            {
                                continue;
                            }
                            //if (!string.IsNullOrEmpty(data.path))
                            //{//不是body下的
                            //    continue;
                            //}
                            modelClip.SetCurve(data.path, data.type, data.propertyName, data.curve);
                        }
                        AnimationUtility.SetAnimationEvents(modelClip, AnimationUtility.GetAnimationEvents(clip));

                        GameObject.DestroyImmediate(clip);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("LoadAnimationClip failed");
        }
        #endregion
        #region//加载武器
        if (weaponInfo.LeftModelID != 0)
        {
            Transform leftArmParentTrans = Actor.S_LookupBone2(modelObj.transform, weaponInfo.LeftPoint);
            ArmLoad(leftArmParentTrans, weaponInfo.LeftModelID, weaponInfo.IsHideLeftModel);
        }
        if (weaponInfo.RightModelID != 0)
        {
            Transform rightArmParentTrans = Actor.S_LookupBone2(modelObj.transform, weaponInfo.RightPoint);
            ArmLoad(rightArmParentTrans, weaponInfo.RightModelID, weaponInfo.IsHideRightModel);
        }
        #endregion
        #region//刷新动画列表
        Animation AniList = modelObj.GetComponent<Animation>();
        m_loadedAnimationList.Clear();
        foreach (AnimationState state in AniList)
        {
            m_loadedAnimationList.Add(state.name);
        }
        m_loadedAnimationList.AddRange(m_skillAnimList);
        m_playingAniIndex = m_loadedAnimationList.Count - 1;
        ED_SimActorManager.Instance.Clear();
        ED_SimActorManager.Instance.AddActor(m_currentObj);
        #endregion

        Selection.activeObject = modelObj;
    }
    //保存
    void Save()
    {
        if (m_currentObj == null)
        {
            return;
        }
        try
        {
            Transform t = m_currentObj.transform.GetChild(0);
            if (t != null)
            {
                GameObject modelObj = t.gameObject;
                Animation parentAnim = m_currentObj.GetComponent<Animation>();
                Animation modelAnim = modelObj.GetComponent<Animation>();
                if (modelAnim != null)
                {
                    foreach (var item in m_skillAnimList)
                    {
                        Info.AnimInfo info = null;
                        m_currentInfo.m_animMap.TryGetValue(item, out info);

                        AnimationClip modelClip = modelAnim.GetClip(item);
                        AnimationClip parentClip = parentAnim.GetClip(item);
                        if (parentClip != null)
                        {
                            parentAnim.RemoveClip(parentClip);
                        }
                        //添加新的
                        parentClip = new AnimationClip();
                        parentClip.wrapMode = modelClip.wrapMode;
                        parentClip.frameRate = modelClip.frameRate;
                        AnimationClipCurveData[] array = AnimationUtility.GetAllCurves(modelClip);
                        foreach (var data in array)
                        {
                            if (!m_typeList.Contains(data.type.BaseType))
                            {
                                continue;
                            }
                            string path = "";
                            if (string.IsNullOrEmpty(data.path))
                            {
                                path = "body";
                            }
                            else
                            {
                                path = "body/" + data.path;
                            }
                            parentClip.SetCurve(path, data.type, data.propertyName, data.curve);
                        }
                        AnimationUtility.SetAnimationEvents(parentClip, AnimationUtility.GetAnimationEvents(modelClip));
                        string dir = "/resources/skillanim/";
                        string filename = m_currentInfo.m_modelType.ToString() + "-" +
                            m_currentInfo.m_weaponType.ToString() + "-" + info.m_skillID.ToString() + "-" + 
                            ((int)info.m_skillStepType).ToString() + "-" + info.m_animName + ".anim";
                        if (!System.IO.Directory.Exists(Application.dataPath + dir))
                        {
                            System.IO.Directory.CreateDirectory(Application.dataPath + dir);
                        }
                        AssetDatabase.CreateAsset(parentClip, "assets" + dir + filename);
                        parentAnim.AddClip(parentClip, item);
                    }
                }
            }
        }
        catch (MissingReferenceException e)
        {
            Debug.Log("SaveSkillAnimation exception, msg:" + e.Message);
        }
    }
    //删除
    void Delete()
	{
        m_currentObj = null;
        m_skillAnimList.Clear();
        m_loadedAnimationList.Clear();
        m_currentInfo.Clear();
        ED_SimActorManager.Instance.Clear();
        GameObject.DestroyImmediate(ObjPanel);
    }
    //转换所有animation
    void Convert()
    {
        GuardInitMyData();
        string dir = "assets/resources/prefabs/anim/";
        //攻击动作列表（动作中包含就是攻击动作）
        List<string> attackAnimList = new List<string>();
        attackAnimList.Add("attack-");
        attackAnimList.Add("skill");
        attackAnimList.Add("opentreasure");
        //忽略文件列表
        List<string> skipList = new List<string>();
        skipList.Add(".meta");
        List<string> fileList = ArchiveUtil.NtfGetFiles(dir, false, skipList);
        foreach (var item in fileList)
        {
            UnityEngine.Object loadedObj = AssetDatabase.LoadMainAssetAtPath(item);
            if (loadedObj == null)
            {
                Debug.LogError("LoadMainAssetAtPath obj is null, path:"+item);
                continue;
            }
            GameObject obj = GameObject.Instantiate(loadedObj) as GameObject;
            foreach (AnimationState state in obj.GetComponent<Animation>())
            {
                AnimationClip clip = state.clip;
                if (clip != null)
                {
                    //是否是攻击动作
                    bool isAttackAnimation = false;
                    foreach (var str in attackAnimList)
                    {
                        if (clip.name.Contains(str))
                        {
                            isAttackAnimation = true;
                            break;
                        }
                    }
                    if (!isAttackAnimation)
                    {
                        continue;
                    }
                    //保存曲线到skillanim下
                    //添加新的
                    AnimationClip newClip = new AnimationClip();
                    newClip.wrapMode = clip.wrapMode;
                    newClip.frameRate = clip.frameRate;
                    AnimationClipCurveData[] array = AnimationUtility.GetAllCurves(clip);
                    foreach (var data in array)
                    {
                        if (!m_typeList.Contains(data.type.BaseType))
                        {
                            continue;
                        }
                        newClip.SetCurve(data.path, data.type, data.propertyName, data.curve);
                    }
                    AnimationUtility.SetAnimationEvents(newClip, AnimationUtility.GetAnimationEvents(clip));
                    string saveDir = "/resources/skillanim/";
                    if (!System.IO.Directory.Exists(Application.dataPath + saveDir))
                    {
                        System.IO.Directory.CreateDirectory(Application.dataPath + saveDir);
                    }
                    if (System.IO.File.Exists(Application.dataPath + saveDir + clip.name + ".anim"))
                    {
                        System.IO.File.Delete(Application.dataPath + saveDir + clip.name + ".anim");
                    }
                    AssetDatabase.CreateAsset(newClip, "assets" + saveDir + clip.name + ".anim");
                }
            }
            foreach (AnimationState state in obj.GetComponent<Animation>())
            {
                AnimationClip clip = state.clip;
                if (clip != null)
                {
                    //是否是攻击动作
                    bool isAttackAnimation = false;
                    foreach (var str in attackAnimList)
                    {
                        if (clip.name.Contains(str))
                        {
                            isAttackAnimation = true;
                            break;
                        }
                    }
                    if (!isAttackAnimation)
                    {
                        continue;
                    }
                    //删除曲线
                    AnimationClipCurveData[] array = AnimationUtility.GetAllCurves(clip);
                    clip.ClearCurves();
                    foreach (var data in array)
                    {
                        if (m_typeList.Contains(data.type.BaseType))
                        {
                            continue;
                        }
                        clip.SetCurve(data.path, data.type, data.propertyName, data.curve);
                    }
                    AnimationUtility.SetAnimationEvents(clip, null);
                }
            }
            GameObject.DestroyImmediate(obj);
        }
    }
    //保存所有
    void SaveAll()
    {
        foreach (var item in m_npcTabel.m_list.Values)
        {
            Open(true, item.ID, Info.ENTabelType.enNPC);
            Save();
            Delete();
        }
        foreach (var item in m_heroTabel.m_list.Values)
        {
            Debug.LogWarning(item.ID);
            if (item.ID > 28)
            {
                continue;
            }
            Open(true, item.ID, Info.ENTabelType.enHero);
            Save();
            Delete();
        }
    }
    //获得所有技能的动作
    void GetAnimationList(List<int> skillIDList)
    {
        foreach (var item in skillIDList)
        {
            SkillInfo info = m_skillTable.Lookup(item);
            if (info == null)
            {
                if (item != 0)
                    Debug.LogError("skill is null,id:"+item);
                return;
            }
            string prefix = info.ID.ToString() + "-" + info.Name + "-";
            if (info.IsPrepareExist)
            {
                string name = info.PrepareMotion;
                if (!string.IsNullOrEmpty(name))
                {
                    string animName = prefix + "准备-" + name;
                    m_skillAnimList.Add(animName);
                    m_currentInfo.m_animMap.Add(animName, new Info.AnimInfo(name, item, Info.AnimInfo.ENSkillStepType.enPrepare));
                }
                else
                {
                    Debug.LogError("animation is null, skill id:" + info.ID + ",准备");
                }
            }
            if (info.IsSpellExist)
            {
                string name = info.SpellMotion;
                if (!string.IsNullOrEmpty(name))
                {
                    string animName = prefix + "吟唱-" + name;
                    m_skillAnimList.Add(animName);
                    m_currentInfo.m_animMap.Add(animName, new Info.AnimInfo(name, item, Info.AnimInfo.ENSkillStepType.enSpell));
                }
                else
                {
                    Debug.LogError("animation is null, skill id:" + info.ID + ",吟唱");
                }
            }
            if (info.IsSlashExist)
            {
                string name = info.SlashMotion;
                if (!string.IsNullOrEmpty(name))
                {
                    string animName = prefix + "冲锋-" + name;
                    m_skillAnimList.Add(animName);
                    m_currentInfo.m_animMap.Add(animName, new Info.AnimInfo(name, item, Info.AnimInfo.ENSkillStepType.enSlash));
                }
                else
                {
                    Debug.LogError("animation is null, skill id:" + info.ID + ",冲锋");
                }
                name = info.SlashBlendMotionName;
                if (!string.IsNullOrEmpty(name))
                {
                    string animName = prefix + "冲锋融合-" + name;
                    m_skillAnimList.Add(animName);
                    m_currentInfo.m_animMap.Add(animName, new Info.AnimInfo(name, item, Info.AnimInfo.ENSkillStepType.enSlashBlend));
                }
                else
                {
                    Debug.LogError("animation is null, skill id:" + info.ID + ",冲锋融合");
                }
            }
            if (info.IsReleaseExist)
            {
                string name = info.ReleaseMotion;
                if (!string.IsNullOrEmpty(name))
                {
                    string animName = prefix + "释放-" + name;
                    m_skillAnimList.Add(animName);
                    m_currentInfo.m_animMap.Add(animName, new Info.AnimInfo(name, item, Info.AnimInfo.ENSkillStepType.enRelease));
                }
                else
                {
                    Debug.LogError("animation is null, skill id:" + info.ID + ",释放");
                }
                name = info.ReleaseBlendMotionName;
                if (!string.IsNullOrEmpty(name))
                {
                    string animName = prefix + "释放融合-" + name;
                    m_skillAnimList.Add(animName);
                    m_currentInfo.m_animMap.Add(animName, new Info.AnimInfo(name, item, Info.AnimInfo.ENSkillStepType.enReleaseBlend));
                }
            }
            if (info.IsConductExist)
            {
                string name = info.ConductMotion;
                if (!string.IsNullOrEmpty(name))
                {
                    string animName = prefix + "引导-" + name;
                    m_skillAnimList.Add(animName);
                    m_currentInfo.m_animMap.Add(animName, new Info.AnimInfo(name, item, Info.AnimInfo.ENSkillStepType.enConduct));
                }
                else
                {
                    Debug.LogError("animation is null, skill id:" + info.ID + ",引导");
                }
            }
            if (info.IsEndConductExist)
            {
                string name = info.EndConductMotion;
                if (!string.IsNullOrEmpty(name))
                {
                    string animName = prefix + "引导结束-" + name;
                    m_skillAnimList.Add(animName);
                    m_currentInfo.m_animMap.Add(animName, new Info.AnimInfo(name, item, Info.AnimInfo.ENSkillStepType.enEndConduct));
                }
                else
                {
                    Debug.LogError("animation is null, skill id:" + info.ID + ",引导结束");
                }
            }
        }
    }
    //加载武器
    void ArmLoad(Transform parentT, int modelID, bool isHideModel)
    {
        ModelInfo info = m_modelTable.Lookup(modelID);
        GameObject armObj = GameObject.Instantiate(GameData.LoadPrefab<GameObject>(info.ModelFile)) as GameObject;
        armObj.transform.parent = parentT;
        armObj.transform.LocalPositionY(0);
        armObj.transform.LocalPositionX(0);
        armObj.transform.LocalPositionZ(0);
        armObj.transform.LocalScaleX(1);
        armObj.transform.LocalScaleY(1);
        armObj.transform.LocalScaleZ(1);
        armObj.transform.localRotation = Quaternion.Euler(0, 0, 0);

        armObj.SetActive(!isHideModel);
    }
    //加载body的animation
    public bool LoadAnimationClip(GameObject body, int modelType, int weaponType)
    {
        List<string> animationList = PreAnimationLoad.Singleton.AnimationList;
        Animation bodyAnimation = body.GetComponent<Animation>();
        if (null == bodyAnimation)
        {
            bodyAnimation = body.AddComponent<Animation>();
        }
        string dir = "Assets/Extends/actor/hero/" + modelType.ToString() + "/" + "w" + weaponType.ToString() + "/";
        string name = "";
        foreach (var item in animationList)
        {
            name = string.Format(item, modelType.ToString(), weaponType.ToString());
            AnimationClip clip = bodyAnimation.GetClip(name);
            if (null == clip)
            {
                clip = AssetDatabase.LoadMainAssetAtPath(dir + name + ".anim") as AnimationClip;
                if (null != clip)
                {
                    bodyAnimation.AddClip(clip, clip.name);
                }
            }
            else
            {
                Debug.Log("Model add animation error" + "    modelType:" + modelType + "   Value:" + name);
            }
        }
        return true;
    }
    //int m_skillAniIndex = -1;
    void PlaySkill(int index)
    {
        if (index<0||index>=m_loadedAnimationList.Count)
        {
            return;
        }
        string n = m_loadedAnimationList[index];
        if (ED_SimActorManager.Instance.MainActor != null)
        {
            ED_SimActorManager.Instance.MainActor.PlaySkill(n);
        }
        //m_skillAniIndex = index;
        //EditorCamera.SetFieldOfView(25.0f);
        //SceneView.lastActiveSceneView.camera.fieldOfView = 25;
    }
    void PlaySelectionsEffect()
    {
        EDSkillEventTrigger ef = ObjEffect.GetComponent<EDSkillEventTrigger>();
        foreach (GameObject obj in Selection.gameObjects)
        {
            ef.PlayEffect(obj, 2.0f, false);
        }
    }
    void NextDeltaTime()
    {
        ED_SimActorManager.Instance.Next(0.033f);
    }
    void FixSceneCamera()
    {
        GameObject c = GameObject.Find("/MainGame/Main Camera");
        EditorCamera.SetPosition(c.transform.localPosition * 0.25f);
        EditorCamera.SetRotation(c.transform.rotation);
    }


    string GetFullAnimName(string name, int modelType, int weaponType)
    {
        string temp = "";
        temp = "a-" + modelType.ToString() + "-" + "w" + weaponType.ToString() + "-" + name;
        return temp;
    }
}