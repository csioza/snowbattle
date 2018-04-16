using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JoystickWrap : MonoBehaviour {
	[System.Serializable]
	public class JoystickKeyMapping
    {
        public delegate void OnJoystickKeyDown(JoystickKeyMapping obj);

		public string m_name;
        public GameObject m_clickObject;
        public OnJoystickKeyDown m_keyEvent;
		public void Check()
		{
			if (string.IsNullOrEmpty (m_name) || (m_clickObject == null && m_keyEvent == null) )
			{
				return;
			}
			if (Input.GetButtonDown (m_name) ) 
			{
                if (m_clickObject != null)
                {
                    UIEventListener e = m_clickObject.GetComponent<UIEventListener>();
                    if (e != null && e.onClick != null)
                    {
                        e.onClick(e.gameObject);
                    }
                    else
                    {
                        UIMouseClick e1 = m_clickObject.GetComponent<UIMouseClick>();
                        if (e1 != null)
                        {
                            e1.OnClick();
                        }
                    }
                }
                if (m_keyEvent != null)
                {
                    m_keyEvent(this);
                }
                
			}
		}
	}
    [System.Serializable]
    public class JoystickAxisMapping
    {
        public delegate void OnJoystickMoved(JoystickAxisMapping obj);

        public string m_verticalInputName;
        public string m_horizontalInputName;
        public OnJoystickMoved m_axisEvent;
        public Vector2 m_vec = Vector2.zero;
        public Vector2 m_preVec = Vector2.zero;
        public float m_notifyOffset = 0.1f;
        public float m_notifyInterval = 0.1f;

        public Vector2 m_axisUp3D = new Vector2(-1,1);
        public Vector2 m_axisDown3D = new Vector2(1,-1);
        public Vector2 m_axisLeft3D = new Vector2(-1,-1);
        public Vector2 m_axisRight3D = new Vector2(1,1);

        Vector3 m_vec3D = Vector3.zero;
        float m_lastTime = Time.realtimeSinceStartup;
        bool m_isMovingAxis = false;
        public Vector3 GetVector3D()
        {
            return m_vec3D;
        }
        public void Check()
        {
            if (string.IsNullOrEmpty(m_verticalInputName) || string.IsNullOrEmpty(m_horizontalInputName))
            {
                return;
            }
            m_vec.y = Input.GetAxis(m_verticalInputName);
            m_vec.x = Input.GetAxis(m_horizontalInputName);
            Vector2 v3d = m_vec.x * (m_axisRight3D - m_axisLeft3D) + m_vec.y * (m_axisUp3D - m_axisDown3D);
            m_vec3D.x = v3d.x;
            m_vec3D.z = v3d.y;
            m_vec3D.Normalize();
            bool isNotifyByTime = (Time.realtimeSinceStartup - m_lastTime)>m_notifyInterval;
            bool isNotifyByOffset = System.Math.Abs(m_vec.x - m_preVec.x) > m_notifyOffset || System.Math.Abs(m_vec.y - m_preVec.y) > m_notifyOffset;
            if (isNotifyByTime || isNotifyByOffset)
            {
                m_lastTime = Time.realtimeSinceStartup;
                if (m_axisEvent != null)
                {
                    if (m_vec.sqrMagnitude <0.01f)
                    {
                        if (m_isMovingAxis)
                        {
                            m_isMovingAxis = false;
                            m_axisEvent(this);
                        }
                    }
                    else
                    {
                        m_isMovingAxis = true;
                        m_axisEvent(this);
                    }
                }
            }
            m_preVec.x = m_vec.x;
            m_preVec.y = m_vec.y;
            if (System.Math.Abs(m_vec.x) < 0.001 && System.Math.Abs(m_vec.y)<0.001)
            {
                m_lastTime = Time.realtimeSinceStartup;
            }
        }
    }
    public List<JoystickKeyMapping> m_mapping = new List<JoystickKeyMapping>();
    public List<JoystickAxisMapping> m_axisMapping = new List<JoystickAxisMapping>();

	// Update is called once per frame
	void Update () {
        for (int i = 0; i < m_axisMapping.Count; i++)
        {
            m_axisMapping[i].Check();
        }
		for(int i=0;i<m_mapping.Count;i++)
		{
			m_mapping[i].Check();
		}
	}
    void OnGUI_()
    {
        string[] names = Input.GetJoystickNames();
        float fx = (Screen.width - 400) * 0.5f;
        float fy = (Screen.height - 95);
        GUIStyle st = new GUIStyle();
        st.fontSize = 24;
        st.normal.textColor = UnityEngine.Color.green;
        string msg = "Joystick :";
        if (names != null && names.Length>0)
        {
            string btn;
            for (int i=0;i<20;i++)
            {
                btn="joystick button "+i;
                if (Input.GetKey(btn))
                {
                    msg+=","+btn;
                }
            }
            GUI.Label(new Rect(fx, fy, 400, 95), msg, st);
        }

    }
}
