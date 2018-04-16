using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ConvertAnimPanel : EditorWindow
{
    static GUIContent LABEL_NAME_FBX = new GUIContent("FBX:", "");
    static GUIContent LABEL_NAME_ANIM = new GUIContent("anim:", "");
    Vector2 scrollFBXPos = new Vector2(0, 0);
    Vector2 scrollAnimPos = new Vector2(0, 0);
    string mSavePath = "";
    [MenuItem("EditorTools/ConvertAnimPanel")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(ConvertAnimPanel));

    }
    void OnGUI()
    {
        this.Repaint();
        EditorGUILayout.LabelField("selected fbx file list:", GUILayout.Width(200f));
        GameObject[] objArr = Selection.gameObjects;
        if (objArr.Length <= 0)
        {
            return;
        }
        scrollFBXPos = EditorGUILayout.BeginScrollView(scrollFBXPos, false, true, GUILayout.Width(400), GUILayout.Height(200));
        for (int i = 0; i < objArr.Length; i++ )
        {
            GUIContent LABEL_NAME_OBJ = new GUIContent(objArr[i].name, "");
            EditorGUILayout.LabelField(LABEL_NAME_FBX, LABEL_NAME_OBJ, GUILayout.Width(500f));
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.LabelField("animation list:", GUILayout.Width(200f));
        scrollAnimPos = EditorGUILayout.BeginScrollView(scrollAnimPos, false, true, GUILayout.Width(400), GUILayout.Height(200));
        List<string> animNameList = new List<string>();
        Dictionary<string, List<AnimationState>> dictObjAnim = new Dictionary<string, List<AnimationState>>();
        for (int i = 0; i < objArr.Length; i++)
        {
            Animation anim = objArr[i].GetComponent<Animation>();
            if (null == anim)
            {
                continue;
            }
            List<AnimationState> animStateList = new List<AnimationState>();
            foreach (AnimationState state in anim)
            {
                string strFile = state.name + ".anim";
                animNameList.Add(strFile);
                animStateList.Add(state);
            }
            dictObjAnim.Add(objArr[i].name, animStateList);
        }
        for (int i = 0; i < animNameList.Count; i++ )
        {
            GUIContent LABEL_NAME_OBJ = new GUIContent(animNameList[i], "");
            EditorGUILayout.LabelField(LABEL_NAME_ANIM, LABEL_NAME_OBJ, GUILayout.Width(500f));
        }
        EditorGUILayout.EndScrollView();

        //#region PATH
        EditorGUILayout.TextField("save folder:", mSavePath);
        Rect rct = new Rect(100, 500, 100, 30);
        if (GUI.Button(rct, @"Path"))
        {
            mSavePath = EditorUtility.OpenFolderPanel(
                    "select save file path",
                    mSavePath,
                    "");

            if (mSavePath.Contains("Assets"))
            {
                int startIndex = mSavePath.IndexOf("Assets");
                mSavePath = mSavePath.Substring(startIndex);
            }
            if (mSavePath.Length != 0)
            {
                Debug.Log("path:" + mSavePath);
            }
        }
       // #region BUTTON_CONVERT_ALL
        rct = new Rect(300, 500, 100, 30);
        if (GUI.Button(rct, @"Convert All"))
        {
            foreach (KeyValuePair<string, List<AnimationState>> pair in dictObjAnim)
            {
                string savePath = mSavePath + "/";
                if (Directory.Exists(savePath))
                {
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        AnimationState state = pair.Value[i];
                        string wholeFile = savePath + state.name + ".anim";
                        Debug.Log("wholeFile:" + wholeFile);
                        if (File.Exists(wholeFile))
                        {
                            AnimationClip clip = GameObject.Instantiate(state.clip) as AnimationClip;
                            AnimationClip oldClip = AssetDatabase.LoadAssetAtPath(wholeFile, typeof(AnimationClip)) as AnimationClip;
                            AnimationEvent[] animArr = AnimationUtility.GetAnimationEvents(oldClip);
                            AnimationUtility.SetAnimationEvents(clip, animArr);

                            AnimationClipCurveData[] curvesArray = AnimationUtility.GetAllCurves(oldClip);
                            foreach (AnimationClipCurveData item in curvesArray)
                            {
                                if (item.type == typeof(Transform))
                                {
                                    continue;
                                }
                                clip.SetCurve(item.path, item.type, item.propertyName, item.curve);
                            }
                            clip.wrapMode = oldClip.wrapMode;
                            AssetDatabase.CreateAsset(clip, wholeFile);

                        }
                        else
                        {
                            AnimationClip clip = GameObject.Instantiate(state.clip) as AnimationClip;
                            AssetDatabase.CreateAsset(clip, wholeFile);
                        }
                    }
                    AssetDatabase.Refresh();
                }
                else
                {
                    Directory.CreateDirectory(savePath);
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        AnimationState state = pair.Value[i];
                        AnimationClip clip = GameObject.Instantiate(state.clip) as AnimationClip;
                        AssetDatabase.CreateAsset(clip, savePath + state.name + ".anim");
                    }
                    AssetDatabase.Refresh();
                }
            }

        }
        // #endregion
        //m_index = EditorGUILayout.Popup("FBX列表:", m_index, fbxList.ToArray());
    }
}
