using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyResult : EResultBase
{
    ///////////////////////////////////////////////
    public enum ENKeyOptType
    {
        enNone = -1,
        endAdd,
        enExpend,
    }
    public struct KeyData
    {
        public int mKeyId;
        public ENKeyOptType mKeyOptType;
    }
    List<KeyData> mKeyDataList = new List<KeyData>();
    public KeyResult()
    {
        ResultTypeId = EResultManager.ENResultTypeID.enTrapResult;
        IsTicked = true;
        IsEnabled = false;
    }
    public override void Tick()
    {
        base.Tick();

        if (IsEnabled)
        {

        }
    }
    public override bool Execute()
    {
        if (IsEnabled)
        {
            foreach (var item in mKeyDataList)
            {
                switch (item.mKeyOptType)
                {
                    case ENKeyOptType.endAdd:
                        BattleArena.Singleton.KeyCount += 1;
                        //BattleArena.Singleton.SetKeyCount(item.mKeyId, 1);
                        break;
                    case ENKeyOptType.enExpend:
                        BattleArena.Singleton.KeyCount += 1;
                        //BattleArena.Singleton.SetKeyCount(item.mKeyId, -1);
                        break;
                    default:
                        break;
                }
            }

        }
        return true;
    }
    public override void ParseJsonData(LitJson.JsonData data)
    {
        base.ParseJsonData(data);
        LitJson.JsonData keyDataList = data[EResultManager.sSchemeKeyList];
        for (int i = 0; i < keyDataList.Count; i++ )
        {
            KeyData tmpData = new KeyData();
            tmpData.mKeyId = int.Parse(keyDataList[i][EResultManager.sKeyId].ToString());
            tmpData.mKeyOptType = (ENKeyOptType)int.Parse(keyDataList[i][EResultManager.sKeyOptType].ToString());
            mKeyDataList.Add(tmpData);
        }       
    }
}