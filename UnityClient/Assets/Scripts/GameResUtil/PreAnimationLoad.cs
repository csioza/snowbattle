using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;


//加载 模型 动画 文件
public class PreAnimationLoad
{
    public struct TAnimationTypeKey 
    {
        public int m_modelID;
        public int m_weaphonType;
        public override bool Equals(object obj)
        {
            TAnimationTypeKey rhs = (TAnimationTypeKey)obj;
            return m_modelID == rhs.m_modelID && m_weaphonType == rhs.m_weaphonType;
        }
    }
    Dictionary<TAnimationTypeKey, List<AnimationClip>> m_allTypeAnimations = new Dictionary<TAnimationTypeKey, List<AnimationClip>>();
    System.Text.StringBuilder m_stringBuilder = new System.Text.StringBuilder();
    public List<string> m_animationList = null;

    public List<string> AnimationList
    {
        get
        {
            if (m_animationList == null)
            {
                m_animationList = new List<string>();
                TextAsset text = PoolManager.Singleton.LoadWithoutInstantiate<TextAsset>("AnimationTransition.txt");
                ByteReader reader = new ByteReader(text);
                Dictionary<string, string> dictionary = reader.ReadDictionary();
                foreach (var item in dictionary)
                {
                    m_animationList.Add(item.Value);
                }
            }
            if (m_animationList.Count == 0)
            {
                Debug.LogError("PreAnimationLoad, m_animationList.Count == 0");
            }
            return m_animationList;
        }
    }
    public void LoadAnimation(GameObject body, int modelID, int weaponType)
    {
        TAnimationTypeKey key;
        key.m_modelID = modelID;
        key.m_weaphonType = weaponType;
        List<AnimationClip> animations;
        m_allTypeAnimations.TryGetValue(key, out animations);
        if (animations == null)
        {
            animations = new List<AnimationClip>();
            m_allTypeAnimations[key]=animations;
            //为Body添加动画
            ModelInfo modelInfo = GameTable.ModelInfoTableAsset.Lookup(modelID);
            if (null == modelInfo || 0 == modelInfo.modelType)
            {
                Debug.Log("add animation modelInfo null" + modelID);
                return;
            }
            WeaponInfo weaponInfo = GameTable.WeaponInfoTableAsset.Lookup(weaponType);
            if (null == weaponInfo)
            {
                Debug.Log("add animation weaponInfo  null" + weaponType);
                return;
            }
            string prefabName = modelInfo.modelType.ToString() + "-w" + weaponInfo.WeaponType.ToString();
            GameObject animprefab = PoolManager.Singleton.Load(GameData.GetPrefabPath("Anim/" + prefabName), true);
            if (null == animprefab)
            {
                Debug.LogError("add animation animprefab  null");
                return;
            }
            Animation prefabAnimation = animprefab.GetComponent<Animation>();
            if (null == prefabAnimation)
            {
                Debug.LogError("add animation animation  null");
                return;
            }
            string name = "";
            string item;
            List<string> names = AnimationList;
            for (int i = 0; i < names.Count; i++)
            {
                item = names[i];
                m_stringBuilder.Length = 0;
                m_stringBuilder.AppendFormat(item, modelInfo.modelType.ToString(), weaponInfo.WeaponType.ToString());
                name = m_stringBuilder.ToString();
                AnimationClip clip = prefabAnimation.GetClip(name);
                if (null != clip)
                {//添加
                    animations.Add(clip);
                }
            }
            PoolManager.Singleton.ReleaseObj(animprefab);
        }

        Animation bodyAnimation = body.GetComponent<Animation>();
        if (null == bodyAnimation)
        {
            bodyAnimation = body.AddComponent<Animation>();
        }
        for (int i=0;i<animations.Count;i++)
        {
            AnimationClip clip = animations[i];
            bodyAnimation.AddClip(clip,clip.name);
        }
    }

    #region Singleton
    static PreAnimationLoad m_singleton;
    static public PreAnimationLoad Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new PreAnimationLoad();
            }
            return m_singleton;
        }
    }
    #endregion
}