using UnityEngine;
using System.Collections;

public class UIQTEDoneorFailed : UIWindow
{
    public enum EnShowType
    {
        enAllHide,
        enShowSucc,
        enShowFail,
    }
    UISprite m_spriteSucc = null;
    UISprite m_spriteFail = null;
    Animation m_anim = null;
    static public UIQTEDoneorFailed Create()
    {
        return UIManager.Singleton.LoadUI<UIQTEDoneorFailed>("UI/UIQTEDoneorFailed", UIManager.Anchor.Center);
    }
    public void Init(GameObject parent)
    {
        SetParent(parent);
    }
    public void Reset(EnShowType type = EnShowType.enAllHide)
    {
        WindowRoot.transform.localPosition = Vector3.zero;
        WindowRoot.transform.localScale = Vector3.one;
        switch (type)
        {
            case EnShowType.enAllHide:
                {
                    m_spriteSucc.gameObject.SetActive(false);
                    m_spriteFail.gameObject.SetActive(false);
                }
                break;
            case EnShowType.enShowSucc:
                {
                    m_spriteSucc.gameObject.SetActive(true);
                    m_spriteFail.gameObject.SetActive(false);
                    m_anim.Play("qtedone", PlayMode.StopAll);
                }
                break;
            case EnShowType.enShowFail:
                {
                    m_spriteSucc.gameObject.SetActive(false);
                    m_spriteFail.gameObject.SetActive(true);
                    m_anim.Play("qtefailed", PlayMode.StopAll);
                }
                break;
            default:
                {
                    m_spriteSucc.gameObject.SetActive(false);
                    m_spriteFail.gameObject.SetActive(false);
                }
                break;
        }
    }
    public override void OnInit()
    {
        base.OnInit();

        m_spriteSucc = FindChildComponent<UISprite>("qtedone");
        m_spriteFail = FindChildComponent<UISprite>("qtefailed");
        m_anim = WindowRoot.GetComponent<Animation>();
        Reset();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
