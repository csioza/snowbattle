using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PreResourceInfoTool
{
    enum Type
    {
        enPrefab = 0,
        enEffect,
    }
    static Dictionary<string, int> m_resourceList = new Dictionary<string, int>(); // 名称 类型Type
    static List<string> m_hadResourceList       = new List<string>();

    static ModelInfoTable m_modelInfoTable      = null;
    static SkillResultTable m_skillResultTable  = null;
    static BuffTable m_buffTable                = null;
    static BuffEffectTable m_buffEffectTable    = null;
    static FlyingItemTable m_flyingItemTable    = null;
    static NPCInfoTable m_npcInfoTable          = null;

    [@MenuItem("EditorTools/PreResourceInfoTool")]
    static void Scanning()
    {

        string fileName         = "Assets/Resources/Config/PreResourceLoad.bytes";

        TextAsset obj           = null;
        if ( null == m_modelInfoTable )
        {
            obj                 = GameData.LoadConfig<TextAsset>("ModelInfoTable");
            m_modelInfoTable    = new ModelInfoTable();
            m_modelInfoTable.Load(obj.bytes);
        }


        if (null == m_skillResultTable)
        {
            obj                 = GameData.LoadConfig<TextAsset>("SkillResultTable");
            m_skillResultTable  = new SkillResultTable();
            m_skillResultTable.Load(obj.bytes);
        }


        if (null == m_buffTable)
        {
            obj                 = GameData.LoadConfig<TextAsset>("Buff");
            m_buffTable         = new BuffTable();
            m_buffTable.Load(obj.bytes);
        }

        if (null == m_buffEffectTable)
        {
            obj                 = GameData.LoadConfig<TextAsset>("BuffEffect");
            m_buffEffectTable   = new BuffEffectTable();
            m_buffEffectTable.Load(obj.bytes);
        }

        if (null == m_flyingItemTable)
        {
            obj                 = GameData.LoadConfig<TextAsset>("FlyingObjBehaviorTable");
            m_flyingItemTable   = new FlyingItemTable();
            m_flyingItemTable.Load(obj.bytes);
        }

        if ( null == m_npcInfoTable )
        {
            obj                 = GameData.LoadConfig<TextAsset>("NPCInfoTable");
            m_npcInfoTable      = new NPCInfoTable();
            m_npcInfoTable.Load(obj.bytes);
        }

        using (FileStream targetFile = new FileStream(fileName, FileMode.Create))
        {
            byte[] buff = Save();

            targetFile.Write(buff, 0, buff.Length);
        }
    }


   static public byte[] Save()
    {
        m_hadResourceList.Clear();

        BinaryHelper helper             = new BinaryHelper();

        TextAsset asset                 = GameData.LoadConfig<TextAsset>("HeroInfoTable");
        HeroInfoTable heroInfoTable     = new HeroInfoTable();
        heroInfoTable.Load(asset.bytes);

        Debug.Log("扫描资源信息条为:" + (heroInfoTable.m_list.Count + m_npcInfoTable.m_list.Count));
        // 信息数量
        helper.Write(heroInfoTable.m_list.Count + m_npcInfoTable.m_list.Count);

       // HERO
        foreach (KeyValuePair<int, HeroInfo> item in heroInfoTable.m_list)
        {
            m_resourceList.Clear();

            ModelInfo modelInfo = m_modelInfoTable.Lookup(item.Value.ModelId);
            if (null == modelInfo)
            {
                helper.Write(0);
                continue;
            }

            GameObject preObj = GameData.LoadPrefab<GameObject>(modelInfo.ModelFile);

            if (null == preObj)
            {
                helper.Write(0);
                continue;
            }
            Animation AniList = preObj.GetComponent<Animation>();

            if (string.IsNullOrEmpty(modelInfo.ModelFile))
            {
                helper.Write(0);
                continue;
            }
                
            if (m_hadResourceList.Contains(modelInfo.ModelFile))
            {
                helper.Write(0);
                continue;
            }

            // 模型预设名称
            m_resourceList.Add(modelInfo.ModelFile,(int)Type.enPrefab);
            m_hadResourceList.Add(modelInfo.ModelFile);
            Debug.Log("modelInfo.ModelFile:" + modelInfo.ModelFile);
         
            foreach (AnimationState state in AniList)
            {
                AnimationClip clip = AniList.GetClip(state.name);
                if (null == clip)
                {
                    continue;
                }

                // 动画列表
                AnimationEvent[] events = AnimationUtility.GetAnimationEvents(clip);

                foreach (AnimationEvent data in events)
                {
                    if (data.functionName == "ChangeResultID" || data.functionName == "ChangeInstantResultID")
                    {
                        LoadEffect(data.intParameter);
                    }
                    else if (data.functionName == "RemoteAttack")
                    {
                        LoadRemoteObj(data.intParameter);
                    }

                }
            }

            Debug.Log("m_resourceList.Count:" + m_resourceList.Count );

            helper.Write(m_resourceList.Count);

            foreach (KeyValuePair<string,int> resourceListItem in m_resourceList)
            {
                helper.Write(resourceListItem.Value);
                helper.Write(resourceListItem.Key);
                //Debug.Log("resourceListItem:" + resourceListItem);
            }
        }

       // NPC
        foreach (KeyValuePair<int, NPCInfo> item in m_npcInfoTable.m_list)
        {
            m_resourceList.Clear();

            ModelInfo modelInfo = m_modelInfoTable.Lookup(item.Value.ModelId);
            if (null == modelInfo)
            {
                helper.Write(0);
                continue;
            }

            GameObject preObj = GameData.LoadPrefab<GameObject>(modelInfo.ModelFile);

            if (null == preObj)
            {
                helper.Write(0);
                continue;
            }
            Animation AniList = preObj.GetComponent<Animation>();

            if (string.IsNullOrEmpty(modelInfo.ModelFile))
            {
                helper.Write(0);
                continue;
            }

            if (m_hadResourceList.Contains(modelInfo.ModelFile))
            {
                helper.Write(0);
                continue;
            }

            // 模型预设名称
            m_resourceList.Add(modelInfo.ModelFile, (int)Type.enPrefab);
            m_hadResourceList.Add(modelInfo.ModelFile);
            Debug.Log("NPC modelInfo.ModelFile:" + modelInfo.ModelFile);

            foreach (float buffId in item.Value.GiftBuffIDList)
            {
                BuffInfo buffInfo = m_buffTable.Lookup((int)buffId);

                if (null == buffInfo)
                {
                    continue;
                }

                BuffEffectInfo buffEffectInfo = m_buffEffectTable.Lookup(buffInfo.BuffEffectID);
                if (null == buffEffectInfo)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(buffEffectInfo.EffectName))
                {
                    continue;
                }

                if (!m_hadResourceList.Contains(buffEffectInfo.EffectName))
                {
                    m_resourceList.Add(buffEffectInfo.EffectName, (int)Type.enEffect);
                    m_hadResourceList.Add(buffEffectInfo.EffectName);
                    Debug.Log("NPC m_resourceList 添加  " + buffEffectInfo.EffectName);
                }
            }

            foreach (AnimationState state in AniList)
            {
                AnimationClip clip = AniList.GetClip(state.name);
                if (null == clip)
                {
                    continue;
                }

                // 动画列表
                AnimationEvent[] events = AnimationUtility.GetAnimationEvents(clip);

                foreach (AnimationEvent data in events)
                {
                    if (data.functionName == "ChangeResultID" || data.functionName == "ChangeInstantResultID")
                    {
                        LoadEffect(data.intParameter);
                    }
                    else if (data.functionName == "RemoteAttack")
                    {
                        LoadRemoteObj(data.intParameter);
                    }

                }
            }

            Debug.Log(" NPC m_resourceList.Count:" + m_resourceList.Count);

            helper.Write(m_resourceList.Count);

            foreach (KeyValuePair<string, int> resourceListItem in m_resourceList)
            {
                helper.Write(resourceListItem.Value);
                helper.Write(resourceListItem.Key);
                //Debug.Log("resourceListItem:" + resourceListItem);
            }
        }
        

        return helper.GetBytes();
    }

   // 加载特效
   static void LoadEffect(int resultId)
   {
       if (resultId <= 0)
       {
           return;
       }

       SkillResultInfo skillResultInfo = m_skillResultTable.Lookup(resultId);
       if (null == skillResultInfo)
       {
           return;
       }

       // 特效物件1
      // GameObject obj = GameData.LoadEffect<GameObject>(skillResultInfo.EffectName);

       if (skillResultInfo.EffectName != null && !m_hadResourceList.Contains(skillResultInfo.EffectName))
       {
           m_resourceList.Add(skillResultInfo.EffectName,(int)Type.enEffect);
           m_hadResourceList.Add(skillResultInfo.EffectName);
           Debug.Log("m_resourceList 添加 LoadEffect " + skillResultInfo.EffectName + ",resultId:" + resultId);
       }


       // 特效物件2
       for (int i = 0; i < skillResultInfo.ParamList.Length; i++)
       {
           ResultParam resultParam = skillResultInfo.ParamList[i];

           if (resultParam.ID == 3)
           {
               int buffId = (int)resultParam.Param[0];

               BuffInfo buffInfo = m_buffTable.Lookup(buffId);

               if (null == buffInfo)
               {
                   continue;
               }

               BuffEffectInfo buffEffectInfo = m_buffEffectTable.Lookup(buffInfo.BuffEffectID);
               if (null == buffEffectInfo)
               {
                   continue;
               }

               if (string.IsNullOrEmpty(buffEffectInfo.EffectName))
               {
                   continue;
               }

               if (!m_hadResourceList.Contains(buffEffectInfo.EffectName))
               {
                   m_resourceList.Add(buffEffectInfo.EffectName,(int)Type.enEffect);
                   m_hadResourceList.Add(buffEffectInfo.EffectName);
                   Debug.Log("m_resourceList 添加  " + buffEffectInfo.EffectName);
               }

           }
       }
       // 特效物件3
       foreach (var item in skillResultInfo.ExtraParamList)
       {
           LoadEffect(item.SkillResultID);
       }
   }

   //  加载 远程相关资源
   static void LoadRemoteObj(int id)
   {
       if (id <= 0)
       {
           return;
       }

       FlyingItemInfo flyingItemInfo = m_flyingItemTable.LookUp(id);

       if (null == flyingItemInfo)
       {
           return;
       }


       // 飞行道具预设
       if (!string.IsNullOrEmpty(flyingItemInfo.Item_Name) && !m_hadResourceList.Contains(flyingItemInfo.Item_Name))
       {
           m_resourceList.Add(flyingItemInfo.Item_Name, (int)Type.enEffect);
           m_hadResourceList.Add(flyingItemInfo.Item_Name);
           Debug.Log("m_resourceList 添加 flyingItemInfo.Item_Name " + flyingItemInfo.Item_Name);
          
       }
      

       // 预警特效

       if (!string.IsNullOrEmpty(flyingItemInfo.WarningEffectName) && !m_hadResourceList.Contains(flyingItemInfo.WarningEffectName))
       {
           m_resourceList.Add(flyingItemInfo.WarningEffectName,(int)Type.enEffect);
           m_hadResourceList.Add(flyingItemInfo.WarningEffectName);
           Debug.Log("m_resourceList 添加 flyingItemInfo.WarningEffectName" + flyingItemInfo.WarningEffectName);
       }

       // 移除后特效

       if (!string.IsNullOrEmpty(flyingItemInfo.EffectNameForRemove) && !m_hadResourceList.Contains(flyingItemInfo.EffectNameForRemove))
       {
           m_resourceList.Add(flyingItemInfo.EffectNameForRemove,(int)Type.enEffect);
           m_hadResourceList.Add(flyingItemInfo.EffectNameForRemove);
           Debug.Log("m_resourceList 添加 flyingItemInfo.EffectNameForRemove" + flyingItemInfo.EffectNameForRemove);
       }


       //飞行道具技能Result
       LoadEffect(flyingItemInfo.Item_ResultID);


       //移除后效果的resultid
       LoadEffect(flyingItemInfo.ResultIDForRemove);

   }
}