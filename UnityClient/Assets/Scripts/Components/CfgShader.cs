using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CfgShader : MonoBehaviour {

	[System.Serializable]
	public class ShaderPair
	{
		public string m_name;
		public Shader m_oldShader;
		public Shader m_shader;
        //public string m_newName;
    };
    static CfgShader _instance = null;
    public static CfgShader Instance
    {
        get
        {
            return _instance;
        }
    }

	public List<ShaderPair>	m_mainPlayerReplaceShader = new List<ShaderPair>();

	// Use this for initialization
	void Start () {
        _instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		if (!Application.isEditor) 
		{
			return;
		}
		for (int i=0; i<m_mainPlayerReplaceShader.Count; i++) 
		{
			ShaderPair p = m_mainPlayerReplaceShader[i];
			if(p.m_oldShader != null)
			{
				p.m_name = p.m_oldShader.name;
			}
			//if(p.m_shader != null)
			//{
				//p.m_newName = p.m_shader.name;
			//}
		}
	}
    Shader LookupShader(string oldName)
    {
        for (int i = 0; i < m_mainPlayerReplaceShader.Count; i++)
        {
            ShaderPair p = m_mainPlayerReplaceShader[i];
            if (p.m_name == oldName)
            {
                return p.m_shader;
            }
        }
        return null;
    }
    public void ReplaceActorShader(GameObject body,Actor actor)
    {
        if (actor.Type != ActorType.enMain)
        {
            return;
        }
        if (body.GetComponent<Renderer>() != null)
        {
            Material[] ms = body.GetComponent<Renderer>().materials;
            for (int i = 0; i < ms.Length;i++ )
            {
                Material m = ms[i];
                if (m.shader != null)
                {
                    Shader newShader = LookupShader(m.shader.name);
                    if (newShader != null)
                    {
                        m.shader = newShader;
                    }                    
                }
            }
        }
        for (int i = 0; i < body.transform.childCount;i++ )
        {
            Transform t = body.transform.GetChild(i);
            ReplaceActorShader(t.gameObject, actor);
        }
    }
}
