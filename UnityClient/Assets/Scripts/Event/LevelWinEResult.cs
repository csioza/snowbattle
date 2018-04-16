using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelWinEResult : EResultBase
{
    ///////////////////////////////////////////////
    public LevelWinEResult()
    {
        ResultTypeId    = EResultManager.ENResultTypeID.enVictorToLevel;
        IsTicked        = true;
        IsEnabled       = false;
    }
    public override void Tick()
    {
        base.Tick();

        if (IsEnabled)
        {
            Execute();
        }
    }
    public override bool Execute()
    {
        if (base.Execute())
        {
            BattleArena.Singleton.StageStop();
            return true;
        }
        return false;
    }
    public override void ParseJsonData(LitJson.JsonData data)
    {
        base.ParseJsonData(data);
    }
}