using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AnimationShaderParamCallback : MonoBehaviour {
    public enum ENParamType{
        enFloat,
        enColor,
        enVector,
    }
    class ShaderParamNameAndValue
    {
        public string m_name;
        public ENParamType m_type;
        public Vector4 m_valueStartVec4;
        public Color m_valueStartColor;
        public float m_valueStartFloat;
        public Vector4 m_valueVec4;
        public Color m_valueColor;
        public float m_valueFloat;
        //!变化持续时间
        public float m_loopTime;
        //!返回到初始值需要多长时间
        public float m_backTime=0.0f;
        //!已经过去多少时间
        public float m_goTime;
        public bool m_isgoback = false;
    }
    class TimeScaleData
    {
        public float m_scale;
        public float m_loopTime;
		public bool m_isInited=false;
    }
    class CameraFOVData
    {
        public float m_fov;
        public float m_startFov;
        public float m_startTime=-1.0f;
        public float m_fovLoopTime = 0.0f;
        public float m_currentToFovTime;
        public float m_fovGoBackTime;
    }
    TimeScaleData m_timeScale;
    CameraFOVData m_fovLinear;

    Dictionary<Material, Material> m_backupSharedMaterial = new Dictionary<Material, Material>();
    void OnDestroy()
    {
        //foreach (Material mat in m_backupSharedMaterial.Values)
        //{
        //    DestroyImmediate(mat);
        //}
        m_backupSharedMaterial.Clear();
        m_shaderList.Clear();
    }
    void FixedUpdate()
    {
        //显示boxCollider
        if (m_isBoxColliderHide)
        {
            if (m_self != null && !m_self.IsDead)
            {
                if (Time.time - m_boxColliderHideTime >= m_boxColliderHideDuration)
                {//显示
                    m_isBoxColliderHide = false;
                    if (m_self.CenterCollider != null)
                    {
                        m_self.CenterCollider.gameObject.layer = LayerMask.NameToLayer("Actor");
                    }
                }
            }
        }
        {
            foreach (Shader item in m_shaderList.Values)
            {
                if (item.m_index >= item.m_data.AnimCurveInfoList.Count)
                {//shader改变完毕，等待重新开始
                    continue;
                }

                float now = Time.time;
                if (now - item.m_starTime > item.m_data.m_interval || item.m_index == -1)
                {
                    item.m_starTime = now - item.m_data.m_interval;
                    ++item.m_index;

                    if (item.m_index >= item.m_data.AnimCurveInfoList.Count)
                    {//shader改变完毕，等待重新开始
                        continue;
                    }
                    Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>();
                    foreach (Renderer r in renders)
                    {
                        KeyframeData info = item.m_data.AnimCurveInfoList[item.m_index % item.m_data.AnimCurveInfoList.Count];
                        foreach (var propItem in info.PropertyInfoList)
                        {
                            if (r.material.HasProperty(propItem.m_name))
                            {
                                if (propItem.m_color != Color.white)
                                {
                                    r.material.SetColor(propItem.m_name, propItem.m_color);
                                }
                                if (propItem.m_value != float.MinValue)
                                {
                                    r.material.SetFloat(propItem.m_name, propItem.m_value);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
	void Update()
	{
		if (m_timeScale != null)
		{
			UpdateTime();
		}
        if (m_fovLinear != null)
        {
            UpdateFov();
        }
	}
    void UpdateFov()
    {
        if(m_fovLinear.m_startTime<0.0f)
        {
            m_fovLinear.m_startTime = Time.time;
        }
        if (m_fovLinear.m_currentToFovTime > 0.0f)
        {
            float s = (Time.time - m_fovLinear.m_startTime) / m_fovLinear.m_currentToFovTime;
            if (s > 1.0f)
            {
                s = 1.0f;
                m_fovLinear.m_currentToFovTime = -1.0f;
                m_fovLinear.m_startTime = -1.0f;
            }
            float fov = m_fovLinear.m_startFov + s * (m_fovLinear.m_fov - m_fovLinear.m_startFov);
            MainGame.Singleton.MainCamera.MainCamera.GetComponent<Camera>().fieldOfView = fov;
        }
        else if (m_fovLinear.m_fovLoopTime > 0.0f)
        {
            if (Time.time - m_fovLinear.m_startTime > m_fovLinear.m_fovLoopTime)
            {
                m_fovLinear.m_fovLoopTime = -1.0f;
                m_fovLinear.m_startTime = Time.time;
            }
        }
        else
        {
            float s = (Time.time - m_fovLinear.m_startTime) / m_fovLinear.m_fovGoBackTime;
            if (s > 1.0f)
            {
                MainGame.Singleton.MainCamera.MainCamera.GetComponent<Camera>().fieldOfView = m_fovLinear.m_startFov;
                m_fovLinear = null;
            }
            else
            {
                float fov = m_fovLinear.m_fov + s * (m_fovLinear.m_startFov - m_fovLinear.m_fov);
                MainGame.Singleton.MainCamera.MainCamera.GetComponent<Camera>().fieldOfView = fov;
            }
        }
    }
	void UpdateTime()
	{
        Time.timeScale = m_timeScale.m_scale;
        if (m_timeScale.m_isInited)
        {
            m_timeScale.m_loopTime -= Time.deltaTime / m_timeScale.m_scale;
        }
        else
        {
           // Debug.Log("Start ScaleTime:" + Time.realtimeSinceStartup);
        }
		m_timeScale.m_isInited = true;
        if (m_timeScale.m_loopTime <= 0)
        {
            Time.timeScale = 1.0f;
            m_timeScale = null;
            //Debug.Log("End ScaleTime:" + Time.realtimeSinceStartup);
        }
    }
    
    IEnumerator Cor_ShaderParam(List<ShaderParamNameAndValue> shaderParams,int id)
    {
        List<Material> mats = new List<Material>();
        Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer r in renders)
		{
			mats.Add(r.material);
		}
        //float t = gotime;
        while (shaderParams.Count > 0)
        {
            for (int i = 0; i < shaderParams.Count; )
            {
                ShaderParamNameAndValue param = shaderParams[i];
                param.m_goTime += Time.deltaTime;
                float s = param.m_goTime / param.m_loopTime;
                if (s > 1.0f || param.m_loopTime <= 0.000001f)
                {
                    s = 1;
                    if (param.m_isgoback)
                    {
                        shaderParams.RemoveAt(i);
                    }
                    else
                    {
                        //go back value
                        param.m_isgoback = true;
                        param.m_goTime = 0;
                        param.m_loopTime = param.m_backTime;
                        switch (param.m_type)
                        {
                            case ENParamType.enFloat:
                                {
                                    float vbackup = param.m_valueStartFloat;
                                    param.m_valueStartFloat = param.m_valueFloat;
                                    param.m_valueFloat = vbackup;
                                }
                                break;
                            case ENParamType.enColor:
                                {
                                    Color vbackup = param.m_valueStartColor;
                                    param.m_valueStartColor = param.m_valueColor;
                                    param.m_valueColor = vbackup;
                                }
                                break;
                            case ENParamType.enVector:
                                {
                                    Vector4 vbackup = param.m_valueStartVec4;
                                    param.m_valueStartVec4 = param.m_valueVec4;
                                    param.m_valueVec4 = vbackup;
                                }
                                break;
                        }
                    }
                }
                else
                {
                    i++;
                }
                
                float vfloat = 0;
                switch (param.m_type)
                {
                    case ENParamType.enFloat:
                        vfloat = param.m_valueStartFloat + s * (param.m_valueFloat - param.m_valueStartFloat);
                        foreach (Material r in mats)
                        {
                            if (r.HasProperty(param.m_name))
                            {
                                r.SetFloat(param.m_name, vfloat);
                            }
                        }
                        break;
                    case ENParamType.enColor:
                        {
                            Color v = param.m_valueStartColor + s * (param.m_valueColor - param.m_valueStartColor);
                            foreach (Material r in mats)
                            {
                                if (r.HasProperty(param.m_name))
                                {
                                    r.SetColor(param.m_name, v);
                                }
                            }
                        }
                        break;
                    case ENParamType.enVector:
                        {
                            Vector4 v = param.m_valueStartVec4 + s * (param.m_valueVec4 - param.m_valueStartVec4);
                            foreach (Material r in mats)
                            {
                                if (r.HasProperty(param.m_name))
                                {
                                    r.SetVector(param.m_name, v);
                                }
                            }
                        }
                        break;
                }
            }
            yield return 1;
        }
//        foreach (Material r in mats)
//        {
            //DestroyImmediate(r);
            
//        }
//        foreach (Renderer r in renders)
//        {
            //r.material.CopyPropertiesFromMaterial(r.sharedMaterial);
//        }
    }
    int m_testID = 1;


    //shader float直接改变
    public void ChangeShaderFloat(string shaderName, float value)
    {
        Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renders)
        {
            if (r.material.HasProperty(shaderName))
            {
                r.material.SetFloat(shaderName, value);
            }
        }
    }
    //shader color直接改变
    public void ChangeShaderColor(string shaderColorName, string shaderColorParam)
    {
        string[] values = shaderColorParam.Split(new char[1] { ',' });
        if (values.Length >= 3)
        {
            Color c = Color.white;
            c.r = float.Parse(values[0]) / 255.0f;
            c.g = float.Parse(values[1]) / 255.0f;
            c.b = float.Parse(values[2]) / 255.0f;
            c.a = 1.0f;
            if (values.Length == 4)
            {
                c.a = float.Parse(values[3]) / 255.0f;
            }
            Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renders)
            {
                if (r.material.HasProperty(shaderColorName))
                {
                    r.material.SetColor(shaderColorName, c);
                }
            }
        }
    }
    //恢复所有的shader
    public void RestoreAllShader()
    {
        Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renders)
        {
            Material sharedMaterial = BackupSharedMaterial(r);
            r.material = sharedMaterial;
        }
    }
    //恢复shader
    public void RestoreShader(string name, ENParamType type)
    {
        Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renders)
        {
            Material sharedMaterial = BackupSharedMaterial(r);
            if (!sharedMaterial.HasProperty(name))
            {
                continue;
            }
            if (type == ENParamType.enColor)
            {
                r.material.SetColor(name, sharedMaterial.GetColor(name));
            }
            else if (type == ENParamType.enFloat)
            {
                r.material.SetFloat(name, sharedMaterial.GetFloat(name));
            }
        }
    }
    public void BackupAllSharedMaterial()
    {
        Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renders)
        {
            Material sharedMaterial = r.sharedMaterial;
            Material mat = r.material;
            if (!m_backupSharedMaterial.ContainsKey(mat))
            {
                m_backupSharedMaterial[mat] = sharedMaterial;
            }
        }
    }
    Material BackupSharedMaterial(Renderer r)
    {
        Material sharedMaterial = r.sharedMaterial;
        Material mat = r.material;
        if(m_backupSharedMaterial.ContainsKey(mat))
        {
            return m_backupSharedMaterial[mat];
        }
        m_backupSharedMaterial[mat] = sharedMaterial;
        return sharedMaterial;
    }
    //格式:name:值0 value1 value2,持续时间,恢复时间
    public void ShaderParam(AnimationEvent animEvent)
    {
        List<ShaderParamNameAndValue> shaderParamsCall=new List<ShaderParamNameAndValue>();
        try
        {
            float loopTime = 0;
            float gobackTime = 0;
            Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>();
            if(renders.Length==0)
            {
                return;
            }
            string[] shaderParams = animEvent.stringParameter.Split(new char[1] { ',' });
            int startParamIndex = shaderParamsCall.Count;
            int segmentIndex = 0;
            foreach (string p in shaderParams)
            {
                string[] nameAndValue = p.Split(new char[1] { ':' });
                if(nameAndValue.Length==1)
                {//这个字段是持续时间
                    if (segmentIndex == 0)
                    {
                        loopTime = float.Parse(p);
                    }
                    else
                    {
                        gobackTime = float.Parse(p);
                        segmentIndex = 0;
                    }
                    segmentIndex += 1;
                    continue;
                }
                ShaderParamNameAndValue param = new ShaderParamNameAndValue();
                param.m_name = nameAndValue[0];
                string[] values = nameAndValue[1].Split(new char[1] { ' ' });
                if(values.Length==1)
                {
                    param.m_valueFloat = float.Parse(values[0]);
                    param.m_type = ENParamType.enFloat;
                    foreach (Renderer r in renders)
                    {
                        if (r.sharedMaterial.HasProperty(param.m_name))
                        {
                            Material sharedMaterial = BackupSharedMaterial(r);
                            param.m_valueStartFloat = sharedMaterial.GetFloat(param.m_name);
                            break;
                        }
                    }
                }
                else if(values.Length==3){
                    Color c=Color.white;
                    c.r = int.Parse(values[0]) /255.0f;
                    c.g = int.Parse(values[1]) / 255.0f;
                    c.b = int.Parse(values[2]) / 255.0f;
                    c.a = 1.0f;
                    param.m_valueColor=c;
                    param.m_type = ENParamType.enColor;
                    foreach (Renderer r in renders)
                    {
                        if (r.sharedMaterial.HasProperty(param.m_name))
                        {
                            Material sharedMaterial = BackupSharedMaterial(r);
                            param.m_valueStartColor = sharedMaterial.GetColor(param.m_name);
                            break;
                        }
                    }
                }
                else if(values.Length==4){
                    Vector4 c;
                    c.x = float.Parse(values[0]);
                    c.y = float.Parse(values[1]);
                    c.z = float.Parse(values[2]);
                    c.w = float.Parse(values[3]);
                    param.m_valueVec4 = c;
                    param.m_type = ENParamType.enVector;
                    foreach (Renderer r in renders)
                    {
                        if (r.sharedMaterial.HasProperty(param.m_name))
                        {
                            Material sharedMaterial = BackupSharedMaterial(r);
                            param.m_valueStartVec4 = sharedMaterial.GetVector(param.m_name);
                            break;
                        }
                    }
                }
                shaderParamsCall.Add(param);
            }
            for (int i = startParamIndex; i < shaderParamsCall.Count; i++)
            {
                ShaderParamNameAndValue param = shaderParamsCall[i];
                param.m_loopTime = loopTime;
                param.m_goTime = 0.0f;
                param.m_backTime = gobackTime;
            }
            StartCoroutine(Cor_ShaderParam(shaderParamsCall, m_testID++));
        }
        catch (System.Exception)
        {
            Debug.LogError("ShaderParam error:" + animEvent.stringParameter);
        }
    }
    void ShaderParamInTarget(AnimationEvent animEvent)
    {
        GameObject actorObj = gameObject.transform.parent.gameObject;
        ActorProp actorProp = actorObj.GetComponent<ActorProp>();
        if (actorProp == null)
        {
            return;
        }
        Actor actor = actorProp.ActorLogicObj;
        AttackAction action = actor.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
        if (null == action)
        {
            return;
        }
        Actor targetActor = action.m_skillTarget;
        if (targetActor == null)
        {
            return;
        }
        if (targetActor.IsDead)
        {
            return;
        }
        AnimationShaderParamCallback[] callbacks = targetActor.MainObj.GetComponentsInChildren<AnimationShaderParamCallback>();
        foreach (AnimationShaderParamCallback c in callbacks)
        {
            c.ShaderParam(animEvent);
        }
    }
    public void TimeScale(AnimationEvent animEvent)
    {
        try
        {
            TimeScaleData timeScale = new TimeScaleData();
            string[] timeScaleParam = animEvent.stringParameter.Split(new char[1] { ',' });
            if (timeScaleParam.Length >= 2)
            {
                timeScale.m_scale = float.Parse(timeScaleParam[0]);
                timeScale.m_loopTime = float.Parse(timeScaleParam[1]);
                m_timeScale = timeScale;
                UpdateTime();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("TimeScale:" + animEvent.stringParameter + "======error:" + e.Message);
        }
    }
    public void CameraFieldOfView(AnimationEvent animEvent)
    {
        ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
        if (null == selfProp)
        {
            return;
        }
        Actor self = selfProp.ActorLogicObj;
        if (self.Type != ActorType.enMain)
        {//不是主控角色
            return;
        }
        string[] theparams = animEvent.stringParameter.Split(new char[1] { ',' });
        try
        {
            CameraFOVData fovChanged = new CameraFOVData();
            if (theparams.Length >= 4)
            {
                fovChanged.m_fov = float.Parse(theparams[0]);
                if (fovChanged.m_fov > 10)
                {
                    fovChanged.m_fov = (fovChanged.m_fov / 25.0f) * GameSettings.Singleton.CameraFieldOfView;
                }else{
                    fovChanged.m_fov = fovChanged.m_fov* GameSettings.Singleton.CameraFieldOfView;
                }
                fovChanged.m_currentToFovTime = float.Parse(theparams[1]);
                fovChanged.m_fovLoopTime = float.Parse(theparams[2]);
                fovChanged.m_fovGoBackTime = float.Parse(theparams[3]);
                fovChanged.m_startFov = GameSettings.Singleton.CameraFieldOfView;
                m_fovLinear = fovChanged;
                UpdateFov();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("CameraFieldOfView:" + animEvent.stringParameter + "======error:" + e.Message);
        }
    }
    void MoveToTarget(AnimationEvent animEvent)
    {
        GameObject actorObj = gameObject.transform.parent.gameObject;
        ActorProp actorProp = actorObj.GetComponent<ActorProp>();
        if (actorProp == null)
        {
            return;
        }
        Actor actor = actorProp.ActorLogicObj;
        //actor.ActionControl.LookupAction<AttackAction>
        Actor target = actor.TargetManager.CurrentTarget;
        if (target == null)
        {
            return;
        }
        
        string[] theparams = animEvent.stringParameter.Split(new char[1] { ',' });
        Vector3 pos = target.MainPos;
        ///Vector3 forward = actor.MainObj.transform.forward;
        //bool rot = false;
        try
        {
            if (theparams.Length >= 3)
            {
                pos.x = float.Parse(theparams[0]);
                pos.y = float.Parse(theparams[1]);
                pos.z = float.Parse(theparams[2]);
                pos = target.MainObj.transform.TransformPoint(pos);
                //rot = true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("MoveToTarget:" + animEvent.stringParameter + "======error:" + e.Message);
            pos = target.MainPos;
        }
        actor.MainObj.transform.localPosition = pos;

        Vector3 direction = target.MainPos - pos;
        direction.y = 0.0f;
        direction.Normalize();
        actor.MainObj.transform.forward = direction;
    }
    public void EffectInTarget(AnimationEvent animEvent)
    {
        GameObject actorObj = gameObject.transform.parent.gameObject;
        ActorProp actorProp = actorObj.GetComponent<ActorProp>();
        if (actorProp == null)
        {
            return;
        }
        Actor actor = actorProp.ActorLogicObj;
        AttackAction action = actor.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
        if (null == action)
        {
            return;
        }
        Actor target = action.m_skillTarget;
        if (target == null)
        {
            return;
        }
        if (target.IsDead)
        {
            return;
        }
        target.OnPlayEffect(null, animEvent);
    }
    //远程攻击-激发飞行特效
	void RemoteAttack(AnimationEvent animEvent)
	{
		GameObject actorObj = gameObject.transform.parent.gameObject;
		ActorProp actorProp = actorObj.GetComponent<ActorProp>();
		if (actorProp == null)
		{
			return;
		}
		Actor actor = actorProp.ActorLogicObj;
        AttackAction action = actor.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
        if (null == action)
        {
            Debug.LogWarning("Remote Attack is error, attack action is null");
            return;
        }
        ++action.m_skillResultIDIndex;
        if (null == action.m_skillInfo)
        {//技能为空
            Debug.LogWarning("remote attack,skill info is null,skillID:"+action.m_skillID);
            return;
        }
        int skillID = action.m_skillID;
        //飞行道具的id和SkillResultID是一样的
        FlyingItemInfo itemInfo = GameTable.FlyingItemTableAsset.LookUp(action.SkillResultID);
        if (itemInfo == null)
        {
            Debug.LogWarning("flying item info is null, id is " + action.SkillResultID.ToString());
            return;
        }
        Vector3 targetPos = Vector3.zero;
        Actor skillTarget = action.m_skillTarget;
        if (skillTarget != null)
        {
            if (skillTarget.Type == ActorType.enMain && skillTarget.IsActorExit)
            {
                skillTarget = ActorManager.Singleton.MainActor;
                action.m_skillTargetID = skillTarget.ID;
            }
            targetPos = skillTarget.RealPos;
        }
        if (skillTarget != null)
        {
            actor.Forward(skillTarget.RealPos);
        }
        RemoteAttackManager.Info info = new RemoteAttackManager.Info(itemInfo);
        info.m_srcActor = actor;
        info.m_dstActor = skillTarget;
        info.m_targetPos = targetPos;
        info.m_skillID = skillID;
        RemoteAttackManager.Singleton.Add(info);
        if (animEvent.intParameter != 0)
        {//隐藏武器
            actor.ShowWeaponModelWithTable(false);
        }
	}
    //远程攻击-多个目标
    void RemoteAttackMultiTarget(AnimationEvent animEvent)
    {
        GameObject actorObj = gameObject.transform.parent.gameObject;
        ActorProp actorProp = actorObj.GetComponent<ActorProp>();
        if (actorProp == null)
        {
            return;
        }
        m_self = actorProp.ActorLogicObj;
        AttackAction action = m_self.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
        if (null == action)
        {
            Debug.LogWarning("RemoteAttackMultiTarget is error, attack action is null");
            return;
        }
        ++action.m_skillResultIDIndex;
        int skillID = action.m_skillID;
        //animEvent.intParameter;//目标个数
        //animEvent.floatParameter;//范围
        m_multiTargetNumber = animEvent.intParameter;
        m_multiTargetRange = animEvent.floatParameter;
        if (m_multiTargetNumber == 0)
        {//不填写，只指定技能目标
            m_multiTargetNumber = 1;
        }
        //find target
        m_multiTargetList = new List<Actor>(m_multiTargetNumber);
        if (action.m_skillTarget != null)
        {
            m_self.Forward(action.m_skillTarget.RealPos);
            m_multiTargetList.Add(action.m_skillTarget);
        }
        if (m_multiTargetList.Count < m_multiTargetNumber)
        {
            ActorManager.Singleton.ForEach(CheckTarget);
            //while (m_multiTargetList.Count < m_multiTargetNumber)
            //{//目标数量不足，用技能目标填满
            //    if (action.m_skillTarget == null)
            //    {
            //        break;
            //    }
            //    m_multiTargetList.Add(action.m_skillTarget);
            //}
        }
        //飞行道具的id和SkillResultID是一样的
        FlyingItemInfo itemInfo = GameTable.FlyingItemTableAsset.LookUp(action.SkillResultID);
        if (itemInfo == null)
        {
            Debug.LogWarning("flying item info is null, id is " + action.SkillResultID.ToString());
            return;
        }
        for (int i = 0; i < m_multiTargetList.Count; ++i)
        {
            Vector3 targetPos = Vector3.zero;
            Actor skillTarget = m_multiTargetList[i];
            if (skillTarget.Type == ActorType.enMain && skillTarget.IsActorExit)
            {
                skillTarget = ActorManager.Singleton.MainActor;
                action.m_skillTargetID = skillTarget.ID;
            }
            targetPos = skillTarget.RealPos;

            RemoteAttackManager.Info info = new RemoteAttackManager.Info(itemInfo);
            info.m_srcActor = m_self;
            info.m_dstActor = skillTarget;
            info.m_targetPos = targetPos;
            info.m_skillID = skillID;
            RemoteAttackManager.Singleton.Add(info);
        }

        if (string.IsNullOrEmpty(animEvent.stringParameter))
        {//隐藏武器
            m_self.ShowWeaponModelWithTable(false);
        }
    }
    int m_multiTargetNumber = 0;
    float m_multiTargetRange = 0;
    List<Actor> m_multiTargetList = null;
    void CheckTarget(Actor target)
    {
        if (m_multiTargetList.Count == m_multiTargetNumber)
        {
            return;
        }
        if (target.IsDead)
        {
            return;
        }
        if (ActorTargetManager.IsEnemy(m_self, target))
        {
            float d = ActorTargetManager.GetTargetDistance(m_self.MainPos, target.MainPos);
            if (m_multiTargetRange > d)
            {
                if (!m_multiTargetList.Contains(target))
                {
                    m_multiTargetList.Add(target);
                }
            }
        }
    }
    private float m_boxColliderHideTime = 0;
    private float m_boxColliderHideDuration = 0;
    private bool m_isBoxColliderHide = false;
    private Actor m_self = null;
    ////隐藏boxCollider
    void HideBoxCollider(AnimationEvent animEvent)
    {
        ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
        if (null == selfProp)
        {
            return;
        }
        m_self = selfProp.ActorLogicObj;
        if (null != m_self && !m_self.IsDead)
        {
            m_isBoxColliderHide = true;
            m_boxColliderHideTime = Time.time;
            m_boxColliderHideDuration = animEvent.floatParameter;
            if (m_self.AnimationSpeed != 0)
            {
                m_boxColliderHideDuration /= m_self.AnimationSpeed;
            }
            if (m_self.CenterCollider != null)
            {
                m_self.CenterCollider.gameObject.layer = LayerMask.NameToLayer("DisableCollider");
            }
        }
    }
    //显示残影
    public void ShowGhost(AnimationEvent animEvent)
    {
        ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
        if (null == selfProp || selfProp.ActorLogicObj == null)
        {
            return;
        }
        Actor self = selfProp.ActorLogicObj;
        self.ShowGhost(animEvent.floatParameter);
    }
    //清除命令
    void ClearCMD(AnimationEvent animEvent)
    {
        ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
        if (null == selfProp)
        {
            return;
        }
        Actor actor = selfProp.ActorLogicObj;
        if (actor.Type == ActorType.enMain || actor.Type == ActorType.enPlayer)
        {
            (actor as Player).CurrentCmd = null;
        }
        else if (actor.Type == ActorType.enNPC)
        {
            (actor as NPC).CurrentCmd = null;
        }
    }
    class Shader
    {
        public AnimCurveData m_data = null;
        public float m_starTime = 0;
        public int m_index = 0;

        public void Reset()
        {
            m_starTime = Time.time;
            m_index = -1;
        }
    }

    Dictionary<string, Shader> m_shaderList = new Dictionary<string, Shader>();
    //改变shader
    public void ChangeShader(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }
        Shader s = null;
        if (!m_shaderList.TryGetValue(name, out s))
        {
            s = new Shader();
            m_shaderList.Add(name, s);

            s.m_data = AnimCurveDataManager.Singleton.GetAnimCurveData("Anim/" + name + ".bytes");
        }
        s.Reset();
    }
}
