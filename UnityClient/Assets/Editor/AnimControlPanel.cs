using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AnimControlPanel : EditorWindow
{
    int m_srcIndex = 0;            // 当前动画列表索引
    int m_srcModeIndex = 0;
    int m_destIndex = 0;
    int m_destModeIndex = 0;
    int m_otherIndex = 0;
    int m_otherModeIndex = 0;
    /// <summary>
    /// ////////////////////
    /// </summary>
    float m_srcPercent = 0.5f;
    float m_destPercent = 0.5f;
    float m_otherPercent = 0.5f;
    List<string> mList = new List<string>();
    List<string> mModeList = new List<string>();
    List<WrapMode> mEnModeList = new List<WrapMode>();
    GameObject m_curObj = null;
    protected static GUIContent LABEL_Float = new GUIContent("Float", "");
    [MenuItem("EditorTools/AnimControlPanel")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(AnimControlPanel));
    }

    void OnGUI()
    {
        this.Repaint();
        // 获得当前选中物件
        Transform[] selection = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab | SelectionMode.Editable);

        if (selection.Length <= 0)
        {
            return;
        }

        if (null == selection[0])
        {
            return;
        }

        // 选择的物件
        GameObject obj = selection[0].gameObject;
        if (m_curObj != obj)
        {
            m_curObj = obj;
            m_srcIndex = 0;          
            m_destIndex = 0;
            mList.Clear();
            mModeList.Clear();
            mEnModeList.Clear();
            Animation AniList = null;
            // 获得动画列表
            AniList = obj.GetComponent<Animation>();
            if (null == AniList)
            {
                return;
            }
            foreach (AnimationState state in AniList)
            {
                mList.Add(state.name);
            }
            mModeList.Add(WrapMode.Clamp.ToString());
            mEnModeList.Add(WrapMode.Clamp);
            mModeList.Add(WrapMode.Default.ToString());
            mEnModeList.Add(WrapMode.Default);
            mModeList.Add(WrapMode.ClampForever.ToString());
            mEnModeList.Add(WrapMode.ClampForever);
            mModeList.Add(WrapMode.Loop.ToString());
            mEnModeList.Add(WrapMode.Loop);
            mModeList.Add(WrapMode.Once.ToString());
            mEnModeList.Add(WrapMode.Once);
            mModeList.Add(WrapMode.PingPong.ToString());
            mEnModeList.Add(WrapMode.PingPong);
        }
        m_srcIndex = EditorGUILayout.Popup("源动画:", m_srcIndex, mList.ToArray());
        m_srcPercent = EditorGUILayout.FloatField(LABEL_Float, m_srcPercent);
        m_srcModeIndex = EditorGUILayout.Popup("wrapMode:", m_srcModeIndex, mModeList.ToArray());
        m_destIndex = EditorGUILayout.Popup("目标动画:", m_destIndex, mList.ToArray());
        m_destPercent = EditorGUILayout.FloatField(LABEL_Float, m_destPercent);
        m_destModeIndex = EditorGUILayout.Popup("wrapMode:", m_destModeIndex, mModeList.ToArray());
        m_otherIndex = EditorGUILayout.Popup("击飞动画:", m_otherIndex, mList.ToArray());
        m_otherPercent = EditorGUILayout.FloatField(LABEL_Float, m_otherPercent);
        m_otherModeIndex = EditorGUILayout.Popup("wrapMode:", m_otherModeIndex, mModeList.ToArray());
        Rect rct = new Rect(155, 200, 40, 30);
        if (GUI.Button(rct, @"Run"))
        {
            //if (Application.isPlaying)
            {
                obj.GetComponent<Animation>().Stop();

                obj.GetComponent<Animation>()[mList[m_srcIndex]].wrapMode = (mEnModeList[m_srcModeIndex]);
                obj.GetComponent<Animation>().Play(mList[m_srcIndex]);
                //obj.animation.Blend(mList[m_srcIndex], m_srcPercent);
                if (m_srcPercent <= 0f)
                {
                    obj.GetComponent<Animation>()[mList[m_srcIndex]].enabled = false;
                }
                //obj.animation.Play(mList[m_srcIndex]);
                //obj.animation[mList[m_destIndex]].blendMode = AnimationBlendMode.Blend;
                obj.GetComponent<Animation>()[mList[m_destIndex]].wrapMode = mEnModeList[m_destModeIndex];
                //obj.animation[mList[m_destIndex]].weight = m_destPercent;
                obj.GetComponent<Animation>().Blend(mList[m_destIndex], m_destPercent);
                if (m_destPercent <= 0f)
                {
                    obj.GetComponent<Animation>()[mList[m_destIndex]].enabled = false;
                }
//                 AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 1, 3, 120);
//                 AnimationClip animClip = new AnimationClip();
//                 animClip.SetCurve("Bip01", typeof(Transform), "localPosition.z", animCurve);
//                 obj.animation.AddClip(animClip, "offsetMove");
                //animClip.ClearCurves();
                obj.GetComponent<Animation>()[mList[m_otherIndex]].wrapMode = mEnModeList[m_otherModeIndex];
                //obj.animation[mList[m_otherIndex]].blendMode = AnimationBlendMode.Blend;
                //obj.animation[mList[m_otherIndex]].enabled = true;
                obj.GetComponent<Animation>().Blend(mList[m_otherIndex], m_otherPercent);
                if (m_otherPercent <= 0f)
                {
                    obj.GetComponent<Animation>()[mList[m_otherIndex]].enabled = false;
                }
//                 AnimationClip animClip;
//                 animClip.SetCurve()
//                 obj.animation.AddClip(animClip, "offsetMove");
                //obj.animation.Blend(mList[m_otherIndex], m_otherPercent);
                //obj.animation[mList[m_destIndex]].blendMode = AnimationBlendMode.Blend;
                //obj.animation[mList[m_destIndex]].weight = m_destPercent;
                //obj.animation[mList[m_destIndex]].wrapMode = WrapMode.Once;
                //obj.animation.Play(mList[m_destIndex]);
                //obj.animation[mList[m_destIndex]].wrapMode = WrapMode.Once;
                //obj.animation[mList[m_destIndex]].enabled = true;
                
                
                
                
            }
        }
        rct = new Rect(200, 200, 40, 30);
        if (GUI.Button(rct, @"Run1"))
        {
            obj.GetComponent<Animation>().Stop();
            //if (Application.isPlaying)
            {
                //obj.animation.Stop();

//                 obj.animation[mList[m_srcIndex]].wrapMode = (mEnModeList[m_srcModeIndex]);
//                 //obj.animation.Play(mList[m_srcIndex]);
//                 obj.animation.Blend(mList[m_srcIndex], m_srcPercent);
//                 if (m_srcPercent <= 0f)
//                 {
//                     obj.animation[mList[m_srcIndex]].enabled = false;
//                 }
                //obj.animation.Play(mList[m_srcIndex]);
                //obj.animation[mList[m_destIndex]].blendMode = AnimationBlendMode.Blend;
                obj.GetComponent<Animation>()[mList[m_destIndex]].wrapMode = mEnModeList[m_destModeIndex];
                //obj.animation[mList[m_destIndex]].weight = m_destPercent;
                obj.GetComponent<Animation>().Blend(mList[m_destIndex], m_destPercent);
                if (m_destPercent <= 0f)
                {
                    obj.GetComponent<Animation>()[mList[m_destIndex]].enabled = false;
                }
                //                 AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 1, 3, 120);
                //                 AnimationClip animClip = new AnimationClip();
                //                 animClip.SetCurve("Bip01", typeof(Transform), "localPosition.z", animCurve);
                //                 obj.animation.AddClip(animClip, "offsetMove");
                //animClip.ClearCurves();
                obj.GetComponent<Animation>()[mList[m_otherIndex]].wrapMode = mEnModeList[m_otherModeIndex];
                //obj.animation[mList[m_otherIndex]].blendMode = AnimationBlendMode.Blend;
                //obj.animation[mList[m_otherIndex]].enabled = true;
                obj.GetComponent<Animation>().Blend(mList[m_otherIndex], m_otherPercent);
                if (m_otherPercent <= 0f)
                {
                    obj.GetComponent<Animation>()[mList[m_otherIndex]].enabled = false;
                }
            }
        }
    }
}