using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//卡牌 段位升级 卡牌进阶 基类
public class UIBaseCardOperate : UIWindow
{

    public override void OnInit() 
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enBaseCardOperateProp, OnPropertyChanged);
    }

    public override void AttachEvent() 
    {
        base.AttachEvent();
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj) 
    {

    }
}
