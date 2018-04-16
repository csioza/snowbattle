using UnityEngine;
using System.Collections;

public class StateLoading : State
{
	AsyncOperation m_loadingScene;
	float m_progress;
	//UILoading m_UILoading;

	private State PreState { get; set; }
	public StateLoading(State preState)
	{
		PreState = preState;
	}
	public override void OnEnter()
	{
	}
	public override void OnExit()
	{
		//UIManager.Singleton.RemoveUI<UILoading>();
        base.OnExit();
	}
	public override void OnUpdate()
	{

	}
	public override void OnLateUpdate()
	{

	}
	public override void OnFixedUpdate()
	{
		if (m_loadingScene != null)
		{
			m_progress = m_loadingScene.progress;
			if (m_progress > 0 && m_progress <= 1)
			{
				//m_UILoading.SetProcess(m_progress);
			}

			//Debug.Log("progress : " + m_progress);
			if (m_loadingScene.isDone)
			{
				//m_UILoading.SetProcess(1);
				MainGame.Singleton.TranslateTo(PreState);
			}
		}
		else
		{
			MainGame.Singleton.TranslateTo(PreState);
		}
	}


}
