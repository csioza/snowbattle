using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

public class AnimationTools : EditorWindow
{


    int m_index                 = 0;            // 当前动画列表索引

    int[] m_methodIndex         = new int[32];  // 当前函数名称索引

    int m_page                  = 1; // 当前页码

    protected static GUIContent LABEL_Float         = new GUIContent("Float", "");
    protected static GUIContent LABEL_Int           = new GUIContent("Int", "");
    protected static GUIContent LABEL_SAMPLEFloat   = new GUIContent("Sample", "");
    protected static GUIContent LABEL_FEAME_Float   = new GUIContent("所在帧", "");

    [MenuItem("EditorTools/AnimationTools")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
        EditorWindow.GetWindow(typeof(AnimationTools));
	}

	void OnGUI()
	{
        this.Repaint();

        // 获得当前选中物件
        Transform[] selection = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab | SelectionMode.Editable);
	
        if ( selection.Length <= 0 )
        {
            return;
        }
        
        if ( null == selection[0] )
        {
            return;
        }

        // 选择的物件
        GameObject obj      = selection[0].gameObject;
        Animation AniList   = null;

        if ( null != obj.transform.parent )
        {
            return;
        }
        else
        {
            // 获得动画列表
            AniList = obj.GetComponent<Animation>();
        }

        if ( null == AniList )
        {
            return;
        }

        // 函数名称列表
        List<string> list = new List<string>();
        foreach (AnimationState state in AniList)
        {
            list.Add(state.name);
        }

 		EditorGUILayout.LabelField("动画关键帧信息:", GUILayout.Width(100f));

        m_index = EditorGUILayout.Popup("动画列表", m_index, list.ToArray());

        if (m_index >= list.Count)
        {
            return;
        }
       
        string name         = list[m_index];
        AnimationClip clip = AniList.GetClip(name);
        if ( null == clip )
        {
            return;
        }
        clip.frameRate = EditorGUILayout.FloatField(LABEL_SAMPLEFloat, clip.frameRate);

        EditorGUILayout.Space();
       

        // 动画列表
        AnimationEvent[] events = AnimationUtility.GetAnimationEvents(clip);

        // 获取一个物件上所有的 脚本组件
        Component[] children    = obj.GetComponents(typeof(MonoBehaviour));

        list.Clear();

        // 关键帧事件个数
        int index                   = 0;

        // 函数名称+参数 的字符列表
        List<string> fullNameList   = new List<string>();

        // 动画列表总数
        int totalPage               = 1;
        if ( events.GetLength(0) % 3 == 0 )
        {
            totalPage = events.GetLength(0)/3;
        }
        else
        {
            totalPage = events.GetLength(0)/3 +1;
        }

        if (totalPage >1)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            Rect rct = new Rect(155, 60, 40, 30);
            if (GUI.Button(rct, @"Next"))
            {
                m_page++;
                if (m_page > totalPage)
                {
                    m_page = 1;
                }
            }
        }

        foreach (AnimationEvent data in events)
        {
            // 总索引
            int totalMethodIndex    = 0;

            // 获得帧数
            float frame = data.time / (1 / clip.frameRate);

            MethodInfo[] methodList;

            // 遍历物件上的所有脚本
            foreach (Component child in children)
            {

                Type t = child.GetType();
                if (null == t)
                {
                    continue;
                }

                // 获得共有的 私有的 函数名称列表
                methodList = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                // 遍历函数列表 
                foreach (MethodInfo info in methodList)
                {
                    System.Reflection.ParameterInfo[] parmaInfo = info.GetParameters();
                    if (info.Name == data.functionName)
                    {
                        // 更新 当前函数 索引
                        m_methodIndex[index] = totalMethodIndex;
                    }
                    // 参数名称
                    string strParma = "( ";
                    foreach (System.Reflection.ParameterInfo paInfo in parmaInfo)
                    {
                        strParma += paInfo.ParameterType.Name;
                    }
                    strParma += " )";

                    list.Add(info.Name);
                    fullNameList.Add(info.Name + strParma);
                    totalMethodIndex++;

                }
            }

            // 显示当前页的 相关信息
            if (index >= (m_page-1) * 3 && index < m_page * 3)
            {
                m_methodIndex[index] = EditorGUILayout.Popup("Function" + index, m_methodIndex[index], fullNameList.ToArray());
                data.functionName = list[m_methodIndex[index]];
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Event Data" + index, GUILayout.Width(100f));

                EditorGUILayout.FloatField(LABEL_FEAME_Float, frame);
                // 修改数据
                data.floatParameter = EditorGUILayout.FloatField(LABEL_Float, data.floatParameter);
                data.intParameter = EditorGUILayout.IntField(LABEL_Int, data.intParameter);
                data.stringParameter = EditorGUILayout.TextField("String", data.stringParameter);
                data.objectReferenceParameter = EditorGUILayout.ObjectField("Object", data.objectReferenceParameter, typeof(UnityEngine.Object), true) as UnityEngine.Object;

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("——————————————————————" + index, GUILayout.Width(100f));
            }
            index++; 
        }
       
        // 设置后才可以生效
        AnimationUtility.SetAnimationEvents(clip, events);

	}

}


