using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class SaveAnimationCurveDataPanel : EditorWindow
{
    static GUIContent LABEL_NAME_ANIM = new GUIContent("animation:", "");
    static GUIContent LABEL_PATH_SAVE = new GUIContent("source path:", "");
    static GUIContent LABEL_NAME_SAVE = new GUIContent("target path:", "");
    static GUIContent LABEL_CURVE_DUR = new GUIContent("curve duration:", "");    

    [MenuItem("EditorTools/保存Animation曲线数据")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(SaveAnimationCurveDataPanel));
    }
    float m_duration = 0.033f;
    void OnGUI()
    {
        this.Repaint();

        AnimationClip clip = Selection.activeObject as AnimationClip;
        if (clip == null)
        {
            return;
        }
        //animation: xxx.anim
        string animName = clip.name + ".anim";
        GUIContent labelAnimName = new GUIContent(animName, "");
        EditorGUILayout.LabelField(LABEL_NAME_ANIM, labelAnimName, GUILayout.Width(500f));

        //save path: xx/xx/
        string path = AssetDatabase.GetAssetPath(clip);
        if (path == "")
        {

            return;
        }
        path = Path.GetDirectoryName(path);
        path = path + "/";
        GUIContent labelSavePath = new GUIContent(path, "");
        EditorGUILayout.LabelField(LABEL_PATH_SAVE, labelSavePath, GUILayout.Width(500f));

        //save name: Assets/Resources/Anim/xxx.bytes
        string saveName = "Assets/Resources/Anim/" + clip.name + "_bytes.bytes";
        GUIContent labelSaveName = new GUIContent(saveName, "");
        EditorGUILayout.LabelField(LABEL_NAME_SAVE, labelSaveName, GUILayout.Width(500f));

        m_duration = EditorGUILayout.FloatField(LABEL_CURVE_DUR, m_duration);
        Rect rct = new Rect(80, 80, 80, 30);
        if (GUI.Button(rct, @"Save"))
        {
            AnimCurveData csData = new AnimCurveData();
            AnimationClipCurveData[] array = AnimationUtility.GetAllCurves(clip);
            float time = 0;
            while (time < clip.length)
            {
                GetData(time, array, csData);
                time += m_duration;
            }
            GetData(clip.length, array, csData);
            string targetPath = saveName;
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }
            using (FileStream targetFile = new FileStream(targetPath, FileMode.Create))
            {
                BinaryHelper helper = new BinaryHelper();
                csData.m_interval = m_duration;
                csData.Save(helper);
                byte[] buff = helper.GetBytes();
                targetFile.Write(buff, 0, buff.Length);
            }
            Debug.Log("save finished");
        }
    }
    private void GetData(float time, AnimationClipCurveData[] array, AnimCurveData data)
    {
        KeyframeData info = new KeyframeData();
        foreach (AnimationClipCurveData item in array)
        {
            float value = item.curve.Evaluate(time);
            string name = item.propertyName;
            name = name.Replace("material.", "");

            if (name.Contains(".r") || name.Contains(".g") ||
                name.Contains(".b") || name.Contains(".a"))
            {
                string key = name.Substring(0, name.Length - 2);

                KeyframeData.PropertyInfo propInfo = info.PropertyInfoList.Find(i => i.m_name == key);
                if (propInfo == null)
                {
                    Color c = Color.white;
                    if (name.Contains(".r")) { c.r = value; }
                    else if (name.Contains(".g")) { c.g = value; }
                    else if (name.Contains(".b")) { c.b = value; }
                    else if (name.Contains(".a")) { c.a = value; }

                    propInfo = new KeyframeData.PropertyInfo(key);
                    propInfo.m_color = c;

                    info.PropertyInfoList.Add(propInfo);
                }
                else
                {
                    if (name.Contains(".r")) { propInfo.m_color.r = value; }
                    else if (name.Contains(".g")) { propInfo.m_color.g = value; }
                    else if (name.Contains(".b")) { propInfo.m_color.b = value; }
                    else if (name.Contains(".a")) { propInfo.m_color.a = value; }
                }
            }
            else
            {
                KeyframeData.PropertyInfo propInfo = info.PropertyInfoList.Find(i => i.m_name == name);
                if (propInfo == null)
                {
                    propInfo = new KeyframeData.PropertyInfo(name);
                    propInfo.m_value = value;

                    info.PropertyInfoList.Add(propInfo);
                }
                else
                {
                    propInfo.m_value = value;
                }
            }
        }
        data.AnimCurveInfoList.Add(info);
    }
}