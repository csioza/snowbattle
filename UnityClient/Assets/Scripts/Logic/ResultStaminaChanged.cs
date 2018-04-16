using UnityEngine;
using System.Collections;
using NGE.Network;

public class ResultStaminaChanged : IResult
{
    public ResultStaminaChanged()
		:base((int)ENResult.StaminaChanged)
	{
	}
    public static IResult CreateNew()
    {
        return new ResultStaminaChanged();
    }
}
