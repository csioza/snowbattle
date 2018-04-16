using UnityEngine;
using System.Collections;
using System;

public class AnimationCameraCallback : MonoBehaviour
{
    enum ENCameraFollowType
    {
        enNone,
        enSourceActor,//������
        enSkillTarget,//����Ŀ��
        enBackToActor,//���ص�ԭʼ
        enCurrentTarget,//��ǰĿ��
    }
    public Action<GameObject, AnimationEvent> Callback { get; set; }
    void CameraAction(AnimationEvent animEvent)
    {
        ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
        if (null == selfProp)
        {
            return;
        }
        Actor self = selfProp.ActorLogicObj;
        if (self.Type != ActorType.enMain)
        {//�������ؽ�ɫ
            return;
        }
        MainGame.Singleton.MainCamera.Shake(animEvent.stringParameter);
        //string[] theparams = animEvent.stringParameter.Split(new char[1]{','});
        //float shakeRange = float.Parse(theparams[0]);
        //float shakeTime = float.Parse(theparams[1]);
        //float fieldOfView = float.Parse(theparams[2]);
        //if (fieldOfView <= 0.0001f)
        //{
        //    fieldOfView = GameSettings.Singleton.CameraFieldOfView;
        //}
        //int shakeType = int.Parse(theparams[3]);
        //if (MainGame.Singleton != null && MainGame.Singleton.MainCamera!=null) 
        //{
        //    MainGame.Singleton.MainCamera.Shake(shakeRange, shakeTime, fieldOfView, shakeType);
        //}
        
        //ShakeParamInfo shakeInfo = GameTable.ShakeTableAsset.Lookup(animEvent.intParameter);
        //if (null != shakeInfo)
        //{
        //    MainGame.Singleton.MainCamera.Shack(shakeInfo.CameraParam1, shakeInfo.CameraParam2, shakeInfo.CameraParam3, shakeInfo.ShakeNum);
        //}  
    }
    //������䰵����
    void CameraCoverLightChange(AnimationEvent animEvent)
    {
        ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
        if (null == selfProp)
        {
            return;
        }
        Actor self = selfProp.ActorLogicObj;
        if (self.Type != ActorType.enMain)
        {//�������ؽ�ɫ
            return;
        }
        string[] param = animEvent.stringParameter.Split(new char[1] { ',' });
        if (param.Length != 2)
        {
            return;
        }
        MainGame.Singleton.MainCamera.CameraCoverLightChange(param[0], param[1], animEvent.floatParameter);
    }
    //�ı�camera�����Ŀ��
    void CameraFollow(AnimationEvent animEvent)
    {
        ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
        if (null == selfProp)
        {
            return;
        }
        Actor self = selfProp.ActorLogicObj;
        if (self.Type != ActorType.enMain)
        {//�������ؽ�ɫ
            return;
        }
        Transform t = null;
        switch ((ENCameraFollowType)animEvent.intParameter)
        {
            case ENCameraFollowType.enSourceActor:
                {
                    t = self.MainObj.transform;
                }
                break;
            case ENCameraFollowType.enSkillTarget:
                {
                    AttackAction action = self.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
                    if (action == null || null == action.m_skillTarget)
                    {//���ǹ�����û�й���Ŀ��
                        return;
                    }
                    t = action.m_skillTarget.MainObj.transform;
                }
                break;
            case ENCameraFollowType.enBackToActor:
                {
                    MainGame.Singleton.MainCamera.BackToActor(animEvent.floatParameter);
                    return;
                }
//                break;
            case ENCameraFollowType.enCurrentTarget:
                {
                    if (self.CurrentTargetIsDead)
                    {//û��Ŀ��
                        return;
                    }
                    t = self.CurrentTarget.MainObj.transform;
                }
                break;
            default:
                return;
        }
        string[] array = animEvent.stringParameter.Split(new char[] { ',' });
        if (array.Length != 5)
        {
            return;
        }
        Transform bone = self.LookupBone(t, array[0]);
        if (bone == null || bone.gameObject == null)
        {
            return;
        }
        float duration = float.Parse(array[1]) + float.Parse(array[2]);
        bool isChangeY = int.Parse(array[4]) != 0;
        MainGame.Singleton.MainCamera.ChangeFollowTarget(bone.gameObject, float.Parse(array[1]), float.Parse(array[3]), isChangeY, duration);
    }
}
