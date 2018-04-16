using UnityEngine;
using System.Collections;

public class DelegateCoroutine : MonoBehaviour {

    public static Coroutine CallStartCoroutine(GameObject obj, IEnumerator routine)
    {
        DelegateCoroutine mono = obj.GetComponent<DelegateCoroutine>();
        if (mono == null)
        {
            mono = obj.AddComponent<DelegateCoroutine>();
        }
        if (mono == null)
        {
            return MainGame.Singleton.StartCoroutine(routine);
        }
        return mono.StartCoroutine(routine);
    }
}
