using UnityEngine;
using System.Collections;
using System;

public class AnimationSummaryCallback : MonoBehaviour {

    public Action<GameObject, AnimationEvent> Callback { get; set; }

    // 经验值 开始变化
    void ExpChange(AnimationEvent animEvent)
    {
        BattleSummary.Singleton.ExpChange(animEvent.intParameter);
    }

    // 金钱值 开始变化
    void MoneyChange(AnimationEvent animEvent)
    {
        BattleSummary.Singleton.MoneyChange(animEvent.intParameter);
    }

    // 经验进度条 开始变化
    void ExpBarChange(AnimationEvent animEvent)
    {
        BattleSummary.Singleton.ExpBarChange();
    }
    
}
