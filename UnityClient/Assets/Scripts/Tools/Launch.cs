//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Tools
//	created:	2013-6-14
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;
using System.Collections;


public class Launch : MonoBehaviour
{
	public string Path = "";
	void Start()
	{
		//bool isPlay = Handheld.PlayFullScreenMovie("logo.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFill);
		//if (!isPlay)
		//{
		//	Debug.LogWarning("Can not play movie");
		//}

		GameObject main = Resources.Load("Prefabs/MainGame") as GameObject;
		if (null != main)
		{
			Load(main, "");
			Debug.Log("Load MainGame from Resources");
			return;
		}
		main = GameResMng.LoadResource(Path + "Prefabs/MainGame.prefab") as GameObject;
		if (null != main)
		{
			Load(main, Path);
			Debug.Log("Load MainGame from " + Path);
			return;
        }

        StartCoroutine(RemoveEffect(0.5f));
	}

	private void Load(GameObject obj, string path)
	{
		obj = GameObject.Instantiate(obj) as GameObject;
		obj.name = "MainGame";
		obj.GetComponent<Main>().ResourcePath = path;
	}

    private IEnumerator RemoveEffect(float duration)
    {
        yield return new WaitForSeconds(duration);
        GameObject.Destroy(gameObject);
    }
};