using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CheckAnimation : EditorWindow
{

    bool mIsCheckSingleFile = false;
    List<string> mList = new List<string>();
    int mIndex = 0;
    bool mIsFirst = false;
    static string PrefabPath = "Resources/Prefabs/";
    
    [@MenuItem("CheckTools/CheckAnimation")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(CheckAnimation));
        
    }
    void OnGUI()
    {
        this.Repaint();
        if (!mIsFirst)
        {
            mList.Add("Actor/Hero");
            mList.Add("Actor/NPC");
            mIsFirst = true;
        }
        mIsCheckSingleFile = EditorGUILayout.Toggle("Check Single File", mIsCheckSingleFile);
        mIndex = EditorGUILayout.Popup("Check Type:", mIndex, mList.ToArray());
        Rect rct = new Rect(30, 50, 100, 30);
        Rect rct1 = new Rect(30, 85, 100, 30);
        if (GUI.Button(rct, @"Check"))
        {
            if (mIsCheckSingleFile)
            {
                GameObject[] selection = Selection.gameObjects;
                if (selection.Length <= 0)
                {
                    return;
                }

                if (null == selection[0])
                {
                    return;
                }
                // 选择的物件
                GameObject obj = selection[0];
                // 获得动画列表
                CheckActorAnimation(obj);
            }
            else
            {
                string sPth = Application.streamingAssetsPath;
                sPth = sPth.Remove(sPth.LastIndexOf("/") + 1);
                string path = sPth + PrefabPath + mList[mIndex];
                if (!Directory.Exists(path))
                {
                    Debug.Log("not exist " + path);
                    return;
                }
                string[] fls = Directory.GetFiles(path, "*.prefab");
                foreach (string str in fls)
                {
                    string strkey = "/" + mList[mIndex];
                    int nIdx = str.LastIndexOf(strkey);
                    string prefabFile = str.Substring(nIdx + 1);
                    nIdx = prefabFile.LastIndexOf(".");
                    prefabFile = prefabFile.Remove(nIdx);
                    GameObject obj = GameData.LoadPrefab<GameObject>(prefabFile);
                    // 获得动画列表
                    CheckActorAnimation(obj);
                }
            }
            Debug.Log("!!!!!!!!check success!!!!!!!!");
        }
        //检查玩家骨骼资源
        if (GUI.Button(rct1, @"CheckBone"))
        {
            if (mIsCheckSingleFile) 
            {
                GameObject[] selection = Selection.gameObjects;
                if (selection.Length <= 0)
                {
                    return;
                }

                if (null == selection[0])
                {
                    return;
                }
                // 选择的物件
                GameObject obj = selection[0];
                mainPlayerCfg playerCfg = null;
                // 获得骨骼列表
                playerCfg = obj.GetComponent<mainPlayerCfg>();
                CheckBone(obj, playerCfg);
                Debug.Log("prefab = " + obj.name + " Check Complete");
            }
            else 
            {
                string sPth = Application.streamingAssetsPath;
                sPth = sPth.Remove(sPth.LastIndexOf("/") + 1);
                string path = sPth + PrefabPath + mList[mIndex];
                if (!Directory.Exists(path))
                {
                    Debug.Log("not exist " + path);
                    return;
                }
                string[] fls = Directory.GetFiles(path, "*.prefab");
                foreach (string str in fls)
                {
                    string strkey = "/" + mList[mIndex];
                    int nIdx = str.LastIndexOf(strkey);
                    string prefabFile = str.Substring(nIdx + 1);
                    nIdx = prefabFile.LastIndexOf(".");
                    prefabFile = prefabFile.Remove(nIdx);
                    GameObject obj = GameData.LoadPrefab<GameObject>(prefabFile);
                    mainPlayerCfg playerCfg = null;
                    // 获得骨骼列表
                    playerCfg = obj.GetComponent<mainPlayerCfg>();
                    CheckBone(obj, playerCfg);
                    Debug.Log("prefab = " + obj.name + " Check Complete");
                }
            }
        }
    }

    void CheckActorAnimation(GameObject obj) 
    {
        Animation AniList = null;
        AniList = obj.GetComponent<Animation>();
        if (null == AniList)
        {
            Debug.Log("prefab = " + obj.name + "    Animation    " + " = Null");
        }
        else
        {
            foreach (AnimationState state in AniList)
            {
                if (state.wrapMode == WrapMode.Default)
                {
                    Debug.Log("prefab=" + obj.name + " stateName=" + state.name + " state.wrapMode=" + state.wrapMode);
                }
            }
        }
        //判断是否带有碰撞盒子
        BoxCollider boxColl = null;
        boxColl = obj.GetComponent<BoxCollider>();
        if (null == boxColl)
        {
            Debug.Log("prefab = " + obj.name + "    BoxCollider    " + " = Null");
        }
        else 
        {
            if (!boxColl.isTrigger)
            {
                Debug.Log("prefab = " + obj.name + "    BoxCollider isTrigger    " + " = false");
            }
        }

        //判断是否有胶囊碰撞
        Transform collider = obj.transform.Find("Collider");
        if (null == collider)
        {
            Debug.Log("prefab = " + obj.name + "    Collider    " + " = Null");
        }
        else 
        {
            //是否有胶囊碰撞
            CapsuleCollider capsuleColl = null;
            capsuleColl = collider.GetComponent<CapsuleCollider>();
            if (null == capsuleColl)
            {
                Debug.Log("prefab = " + obj.name + "    CapsuleCollider    " + " = Null");
            }
            else 
            {
                if (capsuleColl.isTrigger)
                {
                    Debug.Log("prefab = " + obj.name + "    CapsuleCollider  isTrigger  " + " Wrong");
                }
            }
            //是否有脚本
            CfgFollowPosition cfgFollow = null;
            cfgFollow = collider.GetComponent<CfgFollowPosition>();
            if (null == cfgFollow)
            {
                Debug.Log("prefab = " + obj.name + "    CfgFollowPosition    " + " = Null");
            }
            else
            {
                Transform bip01 = null;
                bip01 = obj.transform.Find("Bip01");
                if (null == bip01)
                {
                    Debug.Log("prefab = " + obj.name + "    Transform   Bip01 " + " = Null");
                }
                else
                {
                    if (cfgFollow.m_follow != bip01)
                    {
                        Debug.Log("prefab = " + obj.name + "    CfgFollowPosition   Follow Not Bip01");
                    }
                }
            }
        }

        //检查GameObject 集合
        Transform adherentPoints = null;
        adherentPoints = obj.transform.Find("AdherentPoints");
        if (null == adherentPoints)
        {
            Debug.Log("prefab = " + obj.name + "    AdherentPoints " + " = Null");
        }
        else 
        {
            Transform headPoint = adherentPoints.Find("headPoint");
            if (null == headPoint)
            {
                Debug.Log("prefab = " + obj.name + "    AdherentPoints.headPoint" + " = Null");
            }
            Transform hpbarPoint = adherentPoints.Find("HPbarPoint");
            if (null == hpbarPoint)
            {
                Debug.Log("prefab = " + obj.name + "    AdherentPoints.HPbarPoint" + " = Null");
            }
            Transform shadowPoint = adherentPoints.Find("shadowPoint");
            if (null == shadowPoint)
            {
                Debug.Log("prefab = " + obj.name + "    AdherentPoints.shadowPoint" + " = Null");
            }
            Transform strikePoint = adherentPoints.Find("strikePoint");
            if (null == strikePoint)
            {
                Debug.Log("prefab = " + obj.name + "    AdherentPoints.strikePoint" + " = Null");
            }
            Transform textPoint = adherentPoints.Find("TextPoint");
            if (null == textPoint)
            {
                Debug.Log("prefab = " + obj.name + "    AdherentPoints.TextPoint" + " = Null");
            }
            Transform warningPoint = adherentPoints.Find("WarningPoint");
            if (null == warningPoint)
            {
                Debug.Log("prefab = " + obj.name + "    AdherentPoints.WarningPoint" + " = Null");
            }
            Transform zeroPoint = adherentPoints.Find("zeroPoint");
            if (null == zeroPoint)
            {
                Debug.Log("prefab = " + obj.name + "    AdherentPoints.zeroPoint" + " = Null");
            }
        }
        
    }

    void CheckBone(GameObject obj,mainPlayerCfg playerCfg) 
    {
        if (null == playerCfg)
        {
            Debug.Log("prefab = " + obj.name + "    playerCfg    " + " = Null");
        }
        else
        {
            if (null == playerCfg.m_headTransform)
            {
                Debug.Log("prefab = " + obj.name + " playerCfg.m_headTransform " + " = Null");
            }
            else
            {
                if ("Bip01 Head" != playerCfg.m_headTransform.name)
                {
                    Debug.Log("prefab = " + obj.name + " playerCfg.m_headTransform.name " + "  wrong");
                }
            }
            if (null == playerCfg.m_upperBodyTransform)
            {
                Debug.Log("prefab = " + obj.name + " playerCfg.m_upperBodyTransform " + " = Null");
            }
            else
            {
                if ("Bip01 Spine1" != playerCfg.m_upperBodyTransform.name)
                {
                    Debug.Log("prefab = " + obj.name + " playerCfg.m_upperBodyTransform.name " + "  wrong");
                }
            }
            if (null == playerCfg.m_rootTransform)
            {
                Debug.Log("prefab = " + obj.name + " playerCfg.m_rootTransform " + " = Null");
            }
            else
            {
                if ("Bip01" != playerCfg.m_rootTransform.name)
                {
                    Debug.Log("prefab = " + obj.name + " playerCfg.m_rootTransform.name " + "  wrong");
                }
            }
            if (null == playerCfg.m_ghostResource)
            {
                Debug.Log("prefab = " + obj.name + " playerCfg.m_ghostResource " + " = Null");
            }
        }
        //检查骨
        Transform rootBone = LookupBone(obj.transform, "Bip01");
        if (null == rootBone)
        {
            Debug.Log("prefab = " + obj.name + " Bip01 Bone " + " = Null");
        }
        Transform footsteps = LookupBone(obj.transform, "Bip01 Footsteps");
        if (null == footsteps)
        {
            Debug.Log("prefab = " + obj.name + " Bip01 Footsteps Bone " + " = Null");
        }
        Transform spine = LookupBone(obj.transform, "Bip01 Spine");
        if (null == spine)
        {
            Debug.Log("prefab = " + obj.name + " Bip01 Spine Bone " + " = Null");
        }
        Transform spine1 = LookupBone(obj.transform, "Bip01 Spine1");
        if (null == spine1)
        {
            Debug.Log("prefab = " + obj.name + " Bip01 Spine Bone1 " + " = Null");
        }
        Transform head = LookupBone(obj.transform, "Bip01 Head");
        if (null == head)
        {
            Debug.Log("prefab = " + obj.name + " Bip01 Head " + " = Null");
        }
    }
    
    Transform LookupBone(Transform parent, string name)
    {
        Transform c = parent.Find(name);
        if (c != null)
        {
            return c;
        }
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform cc = parent.GetChild(i);
            c = LookupBone(cc, name);
            if (c != null)
            {
                return c;
            }
        }
        return c;
    }
}
//检查特效
public class CheckEffect : EditorWindow
{
    bool mIsCheckSingleFile = false;
    List<string> mList = new List<string>();
    int mIndex = 0;
    bool mIsFirst = false;
    static string PrefabPath = "Resources/Prefabs/";
    int maxParticles = 10;
    int textureWidth = 128;
    int textureHeight = 128;
    int particlesWidth = 512;
    int particlesHeight = 512;
    protected static GUIContent LABEL_maxParticles = new GUIContent("maxParticles", "10");
    protected static GUIContent LABEL_textureWidth = new GUIContent("textureWidth", "128");
    protected static GUIContent LABEL_textureHeight = new GUIContent("textureHeight", "128");
    protected static GUIContent LABEL_particlesWidth = new GUIContent("particlesWidth", "512");
    protected static GUIContent LABEL_particlesHeight = new GUIContent("particlesHeight", "512");

    //检查特效
    [@MenuItem("CheckTools/CheckEffect")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(CheckEffect));

    }
    void OnGUI()
    {
        this.Repaint();
        if (!mIsFirst)
        {
            mList.Add("Effect");
            mIsFirst = true;
        }
        mIsCheckSingleFile = EditorGUILayout.Toggle("Check Single File", mIsCheckSingleFile);
        mIndex = EditorGUILayout.Popup("Check Type:", mIndex, mList.ToArray());
        maxParticles = EditorGUILayout.IntField(LABEL_maxParticles, maxParticles);
        textureWidth = EditorGUILayout.IntField(LABEL_textureWidth, textureWidth);
        textureHeight = EditorGUILayout.IntField(LABEL_textureHeight, textureHeight);
        particlesWidth = EditorGUILayout.IntField(LABEL_particlesWidth, particlesWidth);
        particlesHeight = EditorGUILayout.IntField(LABEL_particlesHeight, particlesHeight);
        Rect rct = new Rect(30, 150, 100, 30);
        if (GUI.Button(rct, @"CheckEffect"))
        {
            if (mIsCheckSingleFile)
            {
                GameObject[] selection = Selection.gameObjects;
                if (selection.Length <= 0)
                {
                    return;
                }

                if (null == selection[0])
                {
                    return;
                }
                // 选择的物件
                GameObject obj = selection[0];
                ParticleSystem particle = null;
                particle = obj.GetComponent<ParticleSystem>();
                //检查粒子个数
                CheckChooseEffect(obj, particle,maxParticles);
                //检查纹理大小
                CheckChooseTexture(obj, textureWidth, textureHeight, particlesWidth, particlesHeight);
            }
            else 
            {
                string sPth = Application.streamingAssetsPath;
                sPth = sPth.Remove(sPth.LastIndexOf("/") + 1);
                string path = sPth + PrefabPath + mList[mIndex];
                if (!Directory.Exists(path))
                {
                    Debug.Log("not exist " + path);
                    return;
                }
                string[] fls = Directory.GetFiles(path, "*.prefab");
                foreach (string str in fls)
                {
                    string strkey = "/" + mList[mIndex];
                    int nIdx = str.LastIndexOf(strkey);
                    string prefabFile = str.Substring(nIdx + 1);
                    nIdx = prefabFile.LastIndexOf(".");
                    prefabFile = prefabFile.Remove(nIdx);
                    GameObject obj = GameData.LoadPrefab<GameObject>(prefabFile);
                    ParticleSystem particle = null;
                    particle = obj.GetComponent<ParticleSystem>();
                    //检查粒子个数
                    CheckChooseEffect(obj, particle,maxParticles);
                    //检查纹理大小
                    CheckChooseTexture(obj, textureWidth,textureHeight,particlesWidth,particlesHeight);
                }
            }
        }
    }
    void CheckChooseTexture(GameObject obj, int textureWidth,int textureHeight,int particlesWidth,int particlesHeight) 
    {
        string objPath = AssetDatabase.GetAssetPath(obj);
        string[] nameList = AssetDatabase.GetDependencies(new string[] { objPath });
        int tempWidth = 0;
        int tempHeight = 0;
        foreach (string name in nameList) 
        {
//            int index = name.LastIndexOf('.');
            //string prefabFile = name.Substring(index+1);
            Texture objTemp = AssetDatabase.LoadMainAssetAtPath(name) as Texture;
            
            if (null != objTemp)
            {
                if (objTemp.width > textureWidth)
                {
                    Debug.Log("prefab = " + obj.name + "    width    " + " more");
                }
                if (objTemp.height > textureHeight)
                {
                    Debug.Log("prefab = " + obj.name + "    height    " + " more");
                }
                tempWidth += objTemp.width;
                tempHeight += objTemp.height;
            }
        }
        if (tempWidth > particlesWidth)
        {
            Debug.Log("prefab = " + obj.name + "    particlesWidth    " + " more");
        }
        if (tempHeight > particlesHeight)
        {
            Debug.Log("prefab = " + obj.name + "    particlesHeight    " + " more");
        }
    }
    void CheckChooseEffect(GameObject obj, ParticleSystem particle,int maxParticles)
    {
        if (null == particle)
        {
            Debug.Log("prefab = " + obj.name + "    ParticleSystem    " + " = Null");
        }
        else 
        {
            if (particle.maxParticles > maxParticles)
            {
                Debug.Log("prefab = " + obj.name + "    maxParticles    " + " more ");
            }
        } 
        //查找子空间的粒子数量
        int childCount = obj.transform.childCount;
        for (int i = 0; i < childCount; i++ )
        {
            Transform childTransform = obj.transform.GetChild(i);
            ParticleSystem[] childList = childTransform.gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem item in childList)
            {
                CheckChooseEffect(item.gameObject, item, maxParticles);
            }
        }
    }
}
//检查材质纹理
public class CheckMaterial : EditorWindow
{
    [@MenuItem("CheckTools/CheckMaterial")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(CheckMaterial));

    }
    void OnGUI()
    {
        this.Repaint();
    }
}

//检查场景
public class CheckScene : EditorWindow 
{
    bool mIsCheckSingleFile = false;
    List<string> mList = new List<string>();
    int mIndex = 0;
    bool mIsFirst = false;
    static string PrefabPath = "Resources/Prefabs/";
    [@MenuItem("CheckTools/CheckScene")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(CheckScene));

    }
    void OnGUI()
    {
        this.Repaint();
        if (!mIsFirst)
        {
            mList.Add("Bridge");
            mList.Add("Room");
            mIsFirst = true;
        }
        mIsCheckSingleFile = EditorGUILayout.Toggle("Check Single File", mIsCheckSingleFile);
        mIndex = EditorGUILayout.Popup("Check File:", mIndex, mList.ToArray());
        Rect rct = new Rect(30, 50, 100, 30);
        Rect rct1 = new Rect(30, 85, 100, 30);
        Rect rct2 = new Rect(30, 115, 100, 30);
        if (GUI.Button(rct, @"CheckSingleBridge")) 
        {
            if (mIsCheckSingleFile)
            {
                GameObject[] selection = Selection.gameObjects;
                if (selection.Length <= 0)
                {
                    return;
                }

                if (null == selection[0])
                {
                    return;
                }
                // 选择的物件
                GameObject obj = selection[0];
                CheckBridge(obj);
                Debug.Log("prefab = " + obj.name + " Check Complete");
            }
            else 
            {
                Debug.Log("Error Check Single Bridge");
            }
        }
        if (GUI.Button(rct1, @"CheckSingleRoom")) 
        {
            if (mIsCheckSingleFile)
            {
                GameObject[] selection = Selection.gameObjects;
                if (selection.Length <= 0)
                {
                    return;
                }

                if (null == selection[0])
                {
                    return;
                }
                // 选择的物件
                GameObject obj = selection[0];
                CheckRoom(obj);
                Debug.Log("prefab = " + obj.name + " Check Complete");
            }
            else 
            {
                Debug.Log("Error Check Single Room");
            }
        }
        if (GUI.Button(rct2, @"CheckAll"))
        {
            if (mIsCheckSingleFile)
            {
                Debug.Log("Error Check All Scene");
            }
            else 
            {
                string sPth = Application.streamingAssetsPath;
                sPth = sPth.Remove(sPth.LastIndexOf("/") + 1);
                string path = sPth + PrefabPath + "/Scene/Dungeons/"+ mList[mIndex] + "/Forest/";
                if (!Directory.Exists(path))
                {
                    Debug.Log("not exist " + path);
                    return;
                }
                string[] fls = Directory.GetFiles(path, "*.prefab");
                foreach (string str in fls)
                {
                    string strkey = "/Scene/Dungeons/" + mList[mIndex] + "/Forest/";
                    int nIdx = str.LastIndexOf(strkey);
                    string prefabFile = str.Substring(nIdx + 1);
                    nIdx = prefabFile.LastIndexOf(".");
                    prefabFile = prefabFile.Remove(nIdx);
                    GameObject obj = GameData.LoadPrefab<GameObject>(prefabFile);
                    if (0 == mIndex)
                    {
                        if (obj == null)
                        {
                            Debug.Log(prefabFile);
                        }
                        CheckBridge(obj);
                        Debug.Log("prefab = " + obj.name + " Check Complete");
                    }
                    if (1 == mIndex)
                    {
                        if (obj == null)
                        {
                            Debug.Log(prefabFile);
                        }
                        CheckRoom(obj);
                        Debug.Log("prefab = " + obj.name + " Check Complete");
                    }
                }
            }
        }
    }
    void CheckRoom(GameObject obj) 
    {
        Transform collider = obj.transform.Find("Collider");
        if (null == collider)
        {
            Debug.Log("prefab = " + obj.name + " Collider " + " = Null");
        }
        else 
        {
            if (collider.gameObject.layer != LayerMask.NameToLayer("EnableCollider"))
            {
                Debug.Log("prefab = " + obj.name + " Collider  layer type Not EnableCollider");
            }
            for (int i = 0; i < collider.childCount; i++)
            {
                Transform childTrans = collider.GetChild(i);
                if (childTrans.gameObject.layer != LayerMask.NameToLayer("EnableCollider"))
                {
                    Debug.Log("prefab = " + obj.name + " Collider  Child = " + childTrans.gameObject.name + "layer type Not EnableCollider");
                }
            }

            Transform gateLocate_E = obj.transform.Find("GateLocate_E");
            if (null == gateLocate_E)
            {
                Debug.Log("prefab = " + obj.name + " GateLocate_E " + " = Null");
            }
            Transform gateLocate_N = obj.transform.Find("GateLocate_N");
            if (null == gateLocate_N)
            {
                Debug.Log("prefab = " + obj.name + " GateLocate_N " + " = Null");
            }
            Transform gateLocate_S = obj.transform.Find("GateLocate_S");
            if (null == gateLocate_S)
            {
                Debug.Log("prefab = " + obj.name + " GateLocate_S " + " = Null");
            }
            Transform gateLocate_W = obj.transform.Find("GateLocate_W");
            if (null == gateLocate_W)
            {
                Debug.Log("prefab = " + obj.name + " GateLocate_W " + " = Null");
            }
            Transform locator = obj.transform.Find("Locator");
            if (null == locator)
            {
                Debug.Log("prefab = " + obj.name + " Locator " + " = Null");
            }
            Transform locatorRT = obj.transform.Find("LocatorRT");
            if (null == locatorRT)
            {
                Debug.Log("prefab = " + obj.name + " LocatorRT " + " = Null");
            }
            //RemovableGate_E检查
            Transform removableGate_E = obj.transform.Find("RemovableGate_E");
            if (null == removableGate_E)
            {
                Debug.Log("prefab = " + obj.name + " RemovableGate_E " + " = Null");
            }
            else 
            {
                Transform removableGate_ECollider = removableGate_E.Find("Collider");
                if (null == removableGate_ECollider)
                {
                    Debug.Log("prefab = " + obj.name + " RemovableGate_E Child Collider = NULL");
                }
                else 
                {
                    if (removableGate_ECollider.gameObject.layer != LayerMask.NameToLayer("EnableCollider"))
                    {
                        Debug.Log("prefab = " + obj.name + " RemovableGate_E->Collider layer type Not EnableCollider");
                    }
                    for (int i = 0; i < removableGate_ECollider.childCount; i++)
                    {
                        Transform childTrans = removableGate_ECollider.GetChild(i);
                        if (childTrans.gameObject.layer != LayerMask.NameToLayer("EnableCollider"))
                        {
                            Debug.Log("prefab = " + obj.name + "RemovableGate_E->Collider->" + childTrans.gameObject.name + "layer type Not EnableCollider");
                        }
                    }
                }
            }
            //RemovableGate_N 检查
            Transform removableGate_N = obj.transform.Find("RemovableGate_N");
            if (null == removableGate_N)
            {
                Debug.Log("prefab = " + obj.name + " RemovableGate_N " + " = Null");
            }
            else 
            {
                Transform removableGate_NCollider = removableGate_N.Find("Collider");
                if (null == removableGate_NCollider)
                {
                    Debug.Log("prefab = " + obj.name + " RemovableGate_N Child Collider = NULL");
                }
                else 
                {
                    if (removableGate_NCollider.gameObject.layer != LayerMask.NameToLayer("EnableCollider"))
                    {
                        Debug.Log("prefab = " + obj.name + " RemovableGate_N->Collider layer type Not EnableCollider");
                    }
                    for (int i = 0; i < removableGate_NCollider.childCount; i++)
                    {
                        Transform childTrans = removableGate_NCollider.GetChild(i);
                        if (childTrans.gameObject.layer != LayerMask.NameToLayer("EnableCollider"))
                        {
                            Debug.Log("prefab = " + obj.name + "RemovableGate_N->Collider->" + childTrans.gameObject.name + "layer type Not EnableCollider");
                        }
                    }
                }
            }
            //RemovableGate_S检查
            Transform removableGate_S = obj.transform.Find("RemovableGate_S");
            if (null == removableGate_S)
            {
                Debug.Log("prefab = " + obj.name + " RemovableGate_S " + " = Null");
            }
            else 
            {
                Transform removableGate_SCollider = removableGate_S.Find("Collider");
                if (null == removableGate_SCollider)
                {
                    Debug.Log("prefab = " + obj.name + " RemovableGate_S Child Collider = NULL");
                }
                else 
                {
                    if (removableGate_SCollider.gameObject.layer != LayerMask.NameToLayer("EnableCollider"))
                    {
                        Debug.Log("prefab = " + obj.name + " RemovableGate_S->Collider layer type Not EnableCollider");
                    }
                    for (int i = 0; i < removableGate_SCollider.childCount; i++)
                    {
                        Transform childTrans = removableGate_SCollider.GetChild(i);
                        if (childTrans.gameObject.layer != LayerMask.NameToLayer("EnableCollider"))
                        {
                            Debug.Log("prefab = " + obj.name + "RemovableGate_S->Collider->" + childTrans.gameObject.name + "layer type Not EnableCollider");
                        }
                    }
                }
            }
            //RemovableGate_W检查
            Transform removableGate_W = obj.transform.Find("RemovableGate_W");
            if (null == removableGate_W)
            {
                Debug.Log("prefab = " + obj.name + " RemovableGate_W " + " = Null");
            }
            else 
            {
                Transform removableGate_WCollider = removableGate_W.Find("Collider");
                if (null == removableGate_WCollider)
                {
                    Debug.Log("prefab = " + obj.name + " RemovableGate_W Child Collider = NULL");
                }
                else 
                {
                    if (removableGate_WCollider.gameObject.layer != LayerMask.NameToLayer("EnableCollider"))
                    {
                        Debug.Log("prefab = " + obj.name + " RemovableGate_W->Collider layer type Not EnableCollider");
                    }
                    for (int i = 0; i < removableGate_WCollider.childCount; i++)
                    {
                        Transform childTrans = removableGate_WCollider.GetChild(i);
                        if (childTrans.gameObject.layer != LayerMask.NameToLayer("EnableCollider"))
                        {
                            Debug.Log("prefab = " + obj.name + "RemovableGate_W->Collider->" + childTrans.gameObject.name + "layer type Not EnableCollider");
                        }
                    }
                }
            }
        }
    }

    void CheckBridge(GameObject obj) 
    {
        Transform locator = obj.transform.Find("locator");
        if (null == locator)
        {
            Debug.Log("prefab = " + obj.name + " locator " + " = Null");
        }
        Transform locator_A = obj.transform.Find("locator_A");
        if (null == locator_A)
        {
            Debug.Log("prefab = " + obj.name + " locator_A " + " = Null");
        }
        Transform locator_B = obj.transform.Find("locator_B");
        if (null == locator_B)
        {
            Debug.Log("prefab = " + obj.name + " locator_B " + " = Null");
        }
        Transform locatorRT = obj.transform.Find("locatorRT");
        if (null == locatorRT)
        {
            Debug.Log("prefab = " + obj.name + " locatorRT " + " = Null");
        }
    }
}