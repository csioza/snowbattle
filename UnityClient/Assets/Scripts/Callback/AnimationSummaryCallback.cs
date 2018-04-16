using UnityEngine;
using System.Collections;
using System;

public class AnimationSummaryCallback : MonoBehaviour {

    public Action<GameObject, AnimationEvent> Callback { get; set; }

    // ����ֵ ��ʼ�仯
    void ExpChange(AnimationEvent animEvent)
    {
        BattleSummary.Singleton.ExpChange(animEvent.intParameter);
    }

    // ��Ǯֵ ��ʼ�仯
    void MoneyChange(AnimationEvent animEvent)
    {
        BattleSummary.Singleton.MoneyChange(animEvent.intParameter);
    }

    // ��������� ��ʼ�仯
    void ExpBarChange(AnimationEvent animEvent)
    {
        BattleSummary.Singleton.ExpBarChange();
    }
    
}
