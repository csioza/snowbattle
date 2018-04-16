using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;


public class PreResourceInfo
{
    public string m_name;
    public Dictionary<string, int> m_list = new Dictionary<string,int>();
}
// 预加载
public class PreResourceLoad
{
    enum Type
    {
        enPrefab = 0,
        enEffect,
    }
    public delegate void OnFinished();

    public float m_process{ get {return (float)m_cur/(float)m_total;} }  // 进度

    public Dictionary<string, PreResourceInfo> m_list { get; protected set; }

    List<string> m_tempList = new List<string>();
    List<string> m_nameList = new List<string>();

    int m_total = 1;
    int m_cur   = 1;

    OnFinished m_finished = null;

    #region Singleton
    static PreResourceLoad m_singleton;
    static public PreResourceLoad Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new PreResourceLoad();
            }
            return m_singleton;
        }
    }
    #endregion

    public PreResourceLoad()
    {
        m_list = new Dictionary<string, PreResourceInfo>();
    }



    public void Read()
    {
        TextAsset asset = PoolManager.Singleton.LoadWithoutInstantiate<TextAsset>(GameData.GetConfigPath("PreResourceLoad"));

        BinaryHelper helper = new BinaryHelper(asset.bytes);

        int count = helper.ReadInt();

        Debug.Log("预加载信息 总共" + count + "条");

        for (int i = 0; i < count; i++)
        {
            int effectNum = helper.ReadInt();

            if (effectNum <= 0)
            {
                continue;
            }

            PreResourceInfo info = new PreResourceInfo();

            for (int j = 0; j < effectNum; j++)
            {

                int type = helper.ReadInt();
                string name = helper.ReadString();
                if (j == 0)
                {
                    info.m_name = name;
                }
                else
                {
                    info.m_list.Add(name, type);
                }
            }
            Debug.Log("名称" + info.m_name + ",info.m_list:" + info.m_list.Count);
            if (m_list.ContainsKey(info.m_name))
            {
                continue;
            }

            m_list.Add(info.m_name, info);
        }
   }

   // 加载
   public void Load(List<string> nameList,OnFinished finished)
   {
       if ( nameList == null )
       {
           return;
       }

       m_nameList.Clear();

       m_total  = 1;
       m_cur    = 1;

       m_finished = finished;

       PreResourceInfo info = null;
       foreach (string name in nameList)
       {

           if (m_nameList.Contains(name))
           {
               continue;
           }

           m_list.TryGetValue(name, out info);

           m_nameList.Add(name);

           // 总共加载资源数
           if (null != info)
           {
               m_total = m_total + info.m_list.Count + 1;     
           }
           else
           {
               m_total = m_total  + 1;  
           }

           Debug.Log("Load----------" + name);
       }

       Debug.Log("m_total:" + m_total);
       if ( m_nameList.Count <=0 )
       {
           return;
       }
       GameResPackage.AsyncLoadPackageData data = new GameResPackage.AsyncLoadPackageData();
       MainGame.Singleton.StartCoroutine(Load(m_nameList[0], (int)Type.enPrefab, data));
   }

   IEnumerator Load( string name ,int type, GameResPackage.AsyncLoadPackageData loadData )
   {

       if ( type == (int)Type.enEffect)
       {
           GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
           IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetEffectPath(name), data);
           while (true)
           {
               e.MoveNext();
               if (data.m_isFinish)
               {
                   break;
               }
               yield return e.Current;
           }
           //yield return PoolManager.Singleton.LoadEffectWithPool(name);
       }
       else
       {
           GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
           IEnumerator e = PoolManager.Singleton.Coroutine_Load(name, data);
           while (true)
           {
               e.MoveNext();
               if (data.m_isFinish)
               {
                   break;
               }
               yield return e.Current;
           }
           //yield return PoolManager.Singleton.LoadObjWithoutDir(name, false);
       }

       m_cur++;
       Debug.Log("Load 更新 m_cur" + m_cur + ",name:" + name);
       PreResourceInfo info = null;

       m_list.TryGetValue(name, out info);

       m_tempList.Clear();

       // 如果没有 子列表资源
       if ( null == info )
       {
           // 移除
           m_nameList.RemoveAt(0);

           // 所有资源加载完成
           if (m_nameList.Count == 0)
           {
               Debug.Log("所有资源加载完成");
               if (null != m_finished)
               {
                   m_finished();
               }
           }
           else
           {
               //MainGame.Singleton.StartCoroutine(Load(m_nameList[0], (int)Type.enPrefab));
               GameResPackage.AsyncLoadPackageData loadData2 = new GameResPackage.AsyncLoadPackageData();
               IEnumerator e = Load(m_nameList[0], (int)Type.enPrefab, loadData2);
               while (true)
               {
                   e.MoveNext();
                   if (loadData2.m_isFinish)
                   {
                       break;
                   }
                   yield return e.Current;
               }
           }
       }
       else
       {
           foreach (KeyValuePair<string, int> item in info.m_list)
           {
               m_tempList.Add(item.Key);
           }

           if ( m_tempList.Count == 0 )
           {
               // 移除
               m_nameList.RemoveAt(0);

               // 所有资源加载完成
               if (m_nameList.Count == 0)
               {
                   Debug.Log("所有资源加载完成");
                   if (null != m_finished)
                   {
                       m_finished();
                   }
               }
               else
               {
                   //MainGame.Singleton.StartCoroutine(Load(m_nameList[0], (int)Type.enPrefab));
                   GameResPackage.AsyncLoadPackageData loadData2 = new GameResPackage.AsyncLoadPackageData();
                   IEnumerator e = Load(m_nameList[0], (int)Type.enPrefab, loadData2);
                   while (true)
                   {
                       e.MoveNext();
                       if (loadData2.m_isFinish)
                       {
                           break;
                       }
                       yield return e.Current;
                   }
               }
           }
           else
           {
               //MainGame.Singleton.StartCoroutine(LoadInfo(m_tempList[0], (int)Type.enEffect));
               GameResPackage.AsyncLoadPackageData loadData2 = new GameResPackage.AsyncLoadPackageData();
               IEnumerator e = LoadInfo(m_tempList[0], (int)Type.enEffect, loadData2);
               while (true)
               {
                   e.MoveNext();
                   if (loadData2.m_isFinish)
                   {
                       break;
                   }
                   yield return e.Current;
               }
           }
           loadData.m_isFinish = true;
       }
      

      
   }

   IEnumerator LoadInfo(string name, int type, GameResPackage.AsyncLoadPackageData loadData)
   {
       if (type == (int)Type.enEffect)
       {
           GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
           IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetEffectPath(name), data);
           while (true)
           {
               e.MoveNext();
               if (data.m_isFinish)
               {
                   break;
               }
               yield return e.Current;
           }
           //yield return PoolManager.Singleton.LoadEffectWithPool(name);
       }
       else
       {
           GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
           IEnumerator e = PoolManager.Singleton.Coroutine_Load(name, data);
           while (true)
           {
               e.MoveNext();
               if (data.m_isFinish)
               {
                   break;
               }
               yield return e.Current;
           }
           //yield return PoolManager.Singleton.LoadObjWithoutDir(name, false);
       }

       m_cur++;
       Debug.Log("LoadInfo 更新m_cur " + m_cur + ",name:" + name);

       m_tempList.Remove(name);

       if (m_tempList.Count == 0 )
       {
            // 移除第一个
            m_nameList.RemoveAt(0);

            // 所有资源加载完成
            if (m_nameList.Count == 0)
            {
                Debug.Log("所有资源加载完成");
                if (null != m_finished)
                {
                    m_finished();
                }
            }
            else
            {
                GameResPackage.AsyncLoadPackageData loadData2 = new GameResPackage.AsyncLoadPackageData();
                IEnumerator e = Load(m_nameList[0], 0, loadData2);
                while (true)
                {
                    e.MoveNext();
                    if (loadData2.m_isFinish)
                    {
                        break;
                    }
                    yield return e.Current;
                }
                //MainGame.Singleton.StartCoroutine(Load(m_nameList[0], 0));
            }
       }
       else
       {
           GameResPackage.AsyncLoadPackageData loadData2 = new GameResPackage.AsyncLoadPackageData();
           IEnumerator e = LoadInfo(m_tempList[0], 1, loadData2);
           while (true)
           {
               e.MoveNext();
               if (loadData2.m_isFinish)
               {
                   break;
               }
               yield return e.Current;
           }
           //MainGame.Singleton.StartCoroutine(LoadInfo(m_tempList[0],1));
       }
       loadData.m_isFinish = true;
   }

}