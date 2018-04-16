using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AnimationStatePanel : EditorWindow
{
    static GUIContent LABEL_NAME_ANIM = new GUIContent("anim:", "");
    Animation AniList = null;
    Vector2 scrollFBXPos = new Vector2(0, 0);
    protected static GUIContent LABEL_Float = new GUIContent("Float", "");
    [MenuItem("EditorTools/AnimationStatePanel")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(AnimationStatePanel));
    }

    List<String> m_strs = new List<String>();

    void RefreshWithAnimationState(Animation obj)
    {
        // 获得动画列表
        AniList = obj;
        if (null == AniList)
        {
            return;
        }
        if (Application.isPlaying)
        {
            //scrollFBXPos = EditorGUILayout.BeginScrollView(scrollFBXPos, false, true, GUILayout.Width(1000), GUILayout.Height(200));
            foreach (AnimationState state in AniList)
            {
                if (state.enabled)
                {
                    //Debug.Log("state.name=" + state.name);
                    string str = state.name + "  weight=" + state.weight + "  wrap mode=" + state.wrapMode + "  animTime=" + state.time+" speed="+state.speed;
                    m_strs.Add(str);
                }
            }
            //EditorGUILayout.EndScrollView();
        }
    }
    void RefreshWithActions(ActorProp obj)
    {
		if (obj == null) {
			return;
		}
        ActorActionControl accontrol = obj.ActorLogicObj.ActionControl;
        if (accontrol != null)
        {
            for (int i = 0; i < accontrol.ActionList.Count; i++)
            {
                ActorAction ac = accontrol.ActionList[i];
                string str = ac.GetActionType().ToString();
                m_strs.Add(str);
            }
            return;
        }
        else
        {
            Trap tmpTrap = obj.ActorLogicObj as Trap;
            TrapActionControl trapActionControl = tmpTrap.mActionControl;
            for (int i = 0; i < trapActionControl.ActionList.Count; i++)
            {
                TrapAction ac = trapActionControl.ActionList[i];
                string str = ac.GetActionType().ToString();
                m_strs.Add(str);
            }
        }
    }
    public ActorProp SearchParentComponentActorProp(Transform obj)
    {
        ActorProp c = obj.GetComponent<ActorProp>();
        if (c != null)
        {
            return c;
        }
        if (obj.transform.parent != null)
        {
            return SearchParentComponentActorProp(obj.transform.parent);
        }
        return null;
    }
    public Animation SearchParentComponentAnimation(Transform obj) 
    {
        Animation c = obj.GetComponent<Animation>();
        if (c != null)
        {
            return c;
        }
        if (obj.transform.parent != null)
        {
            return SearchParentComponentAnimation(obj.transform.parent);
        }
        return null;
    }


    void OnGUI()
    {
		m_strs.Clear ();
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
        RefreshWithAnimationState(SearchParentComponentAnimation(obj.transform));
        RefreshWithActions(SearchParentComponentActorProp(obj.transform));
        scrollFBXPos = EditorGUILayout.BeginScrollView(scrollFBXPos, false, true, GUILayout.Width(1000), GUILayout.Height(200));
        foreach(string s in m_strs)
        {
            GUIContent LABEL_NAME_OBJ = new GUIContent(s, "");
            EditorGUILayout.LabelField(LABEL_NAME_ANIM, LABEL_NAME_OBJ, GUILayout.Width(1000f));
        }
        EditorGUILayout.EndScrollView();
    }
}