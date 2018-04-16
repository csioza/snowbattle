using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class EDSkillEventTrigger : MonoBehaviour {

    public GameObject MainObj;
    public bool AutoDeleteEffect = true;
    public float ParticleTimeScale = 1.0f;
    public bool BindToActor = false;
    Vector3 RealPos;
    Vector3 MainPos;
    //public static float DeltaTime;
    public List<EffectData> Effects = new List<EffectData>();
    List<Animation> m_anis=new List<Animation>();
    List<Animator> m_anitors=new List<Animator>();
    List<ParticleSystem> m_pars = new List<ParticleSystem>();
    
	// Use this for initialization
    [System.Serializable]
    public class EffectData
    {
        public GameObject m_eff;
        public GameObject m_bindObject;
        public GameObject m_followObject;
        //public float m_startTime;
        public float m_duration;
        public float m_curTime;
        public bool m_destory;
        public float m_particleTime;
        public float m_particleTime2;
        public float m_particleDelta;
        public bool m_haveParticle;
    }
	void Start () {
	
	}
	
    void SearchAnimatorParticle(GameObject eff,List<Animation> anis,List<Animator> anitors,List<ParticleSystem> pars,bool searchParticle)
    {
        if (eff.GetComponent<Animation>() != null)
        {
            anis.Add(eff.GetComponent<Animation>());
        }
        Animator animator = eff.GetComponent<Animator>();
        if (animator != null)
        {
            anitors.Add(animator);
        }
        ParticleSystem ps = eff.GetComponent<ParticleSystem>();
        if (searchParticle)
        {
            if (ps != null)
            {
                pars.Add(ps);
            }
        }
        for (int i = 0; i < eff.transform.childCount; i++)
        {
            SearchAnimatorParticle(eff.transform.GetChild(i).gameObject, anis, anitors, pars, ps == null && searchParticle);
        }
    }

	// Update is called once per frame
    public void Tick(float deltaTime)
    {
        if (gameObject.activeSelf == false)
        {
            return;
        }
        m_anis.Clear();
        m_anitors.Clear();
        m_pars.Clear();
        
        for (int i = 0; i < Effects.Count;i++ )
        {
            EffectData d = Effects[i];
            if (d.m_curTime > d.m_duration && AutoDeleteEffect)
            {
                GameObject.DestroyImmediate(d.m_eff);
                d.m_eff = null;
                d.m_destory = true;
                continue;
            }
            SearchAnimatorParticle(d.m_eff, m_anis, m_anitors, m_pars,true);
            d.m_haveParticle = m_pars.Count>0;
            for (int k = 0; k < m_pars.Count; k++)
            {
                ParticleSystem par = m_pars[k];
                d.m_particleTime = par.time;
                //par.Simulate(d.m_curTime, true, true);
                par.Simulate(deltaTime * ParticleTimeScale, true, false);
                par.Play(true);
                d.m_particleDelta = deltaTime * ParticleTimeScale;
                d.m_particleTime2 = par.time;
            }
            d.m_curTime += deltaTime;
            m_pars.Clear();
            if (d.m_followObject != null)
            {
                d.m_bindObject.transform.localPosition = d.m_followObject.transform.position;
                if (!d.m_haveParticle)
                {
                    d.m_bindObject.transform.localRotation = d.m_followObject.transform.rotation;
                }
            }
        }
        Effects.RemoveAll(d => d.m_destory==true);
        for (int i = 0; i < m_anis.Count;i++ )
        {
            Animation ani = m_anis[i];
            if (ani.clip != null)
            {
                AnimationState st = ani[ani.clip.name];
                if (st.enabled == false)
                {
                    st.enabled = true;
                    st.time = 0.0f;
                }
                ani.Sample();
                st.time += deltaTime;
            }
        }

        for (int i = 0; i < m_anitors.Count; i++)
        {
            Animator ani = m_anitors[i];
            ani.Update(deltaTime);
        }
	}
    //查找actor的骨骼
    public Transform LookupBone(Transform parent, string name)
    {
        Transform c = LookupBone2(parent, name);
        if (c == null)
        {
            name = name.Replace(" ", "_");
            c = LookupBone2(parent, name);
        }
        return c;
    }
    Transform LookupBone2(Transform parent, string name)
    {
        Transform c = parent.Find(name);
        if (c != null)
        {
            return c;
        }
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform cc = parent.GetChild(i);
            c = LookupBone2(cc, name);
            if (c != null)
            {
                return c;
            }
        }
        return c;
    }
    public GameObject PlayEffect(GameObject obj, float effectTime, bool isParentBone, string posByBone = "")
    {
        if (null == obj || MainObj == null)
        {
            return null;
        }

        GameObject sceneEffect = GameObject.Instantiate(obj) as GameObject;
            //PoolManager.Singleton.CreateObj(obj, false);
        //sceneEffect.transform.parent = null;
            //GameObject.Instantiate(obj) as GameObject;
        sceneEffect.transform.localPosition = new Vector3(RealPos.x, 0.0f, RealPos.z);
        Vector3 currentForward = sceneEffect.transform.eulerAngles;
        currentForward.y = MainObj.transform.eulerAngles.y;
        sceneEffect.transform.eulerAngles = currentForward;
        Transform boneT;
        if (!string.IsNullOrEmpty(posByBone))
        {
            boneT = LookupBone(MainObj.transform, posByBone);
            if (boneT)
            {
                if (isParentBone)
                {
                    sceneEffect.transform.parent = boneT;
                }
                Vector3 pos = boneT.position;
                sceneEffect.transform.position = pos;
            }
            else
            {
                //Debug.LogWarning("PlayEffect miss bone=" + posByBone + ",actor=" + ID.ToString());
                sceneEffect.transform.position = MainPos;
            }
        }

        EffectData recordData = new EffectData();
        recordData.m_eff = sceneEffect;
        recordData.m_curTime = 0.0f;
        
        if (sceneEffect.transform.parent == null)
        {
            sceneEffect.transform.parent = gameObject.transform;
        }
        else
        {
            if (BindToActor == false)
            {
                recordData.m_followObject = sceneEffect.transform.parent.gameObject;
                Transform bindT = gameObject.transform.Find(recordData.m_followObject.name);
                if (bindT == null)
                {
                    recordData.m_bindObject = new GameObject(recordData.m_followObject.name);
                    recordData.m_bindObject.transform.parent = gameObject.transform;
                }
                else
                {
                    recordData.m_bindObject = bindT.gameObject;
                }
                sceneEffect.transform.parent = recordData.m_bindObject.transform;
            }
        }
        
        ParticleSystem partSys = sceneEffect.GetComponent<ParticleSystem>();
        if (null != partSys && !partSys.loop)
        {
            ParticleConfig config = sceneEffect.GetComponent<ParticleConfig>();
            if (null != config && config.RotateParticle)
            {
                partSys.startRotation = MainObj.transform.eulerAngles.y * Mathf.PI / 180.0f;
            }
            recordData.m_duration = partSys.duration;
            //MainGame.Singleton.StartCoroutine(RemoveEffect(++m_effectIndex, sceneEffect, partSys.duration));
        }
        else
        {
            //MainGame.Singleton.StartCoroutine(RemoveEffect(++m_effectIndex, sceneEffect, effectTime));
            recordData.m_duration = effectTime;
        }
        Effects.Add(recordData);
        sceneEffect.SetActive(true);
        return sceneEffect;
    }
    void Effect(AnimationEvent animEvent)
    {
        PlayEffect(animEvent.objectReferenceParameter as GameObject,
            animEvent.floatParameter, animEvent.intParameter != 0, animEvent.stringParameter);
    }

    //连接技特效
    void ConnectEffect(AnimationEvent animEvent)
    {
        
    }
    //终结技特效
    void ComboEffect(AnimationEvent animEvent)
    {
        
    }
}
