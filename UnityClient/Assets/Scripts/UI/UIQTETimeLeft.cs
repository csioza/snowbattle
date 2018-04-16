using UnityEngine;
using System.Collections;

//qte技能释放计时
public class UIQTETimeLeft : UIWindow
{
    UIHPBar m_hpBar = null;
    public float m_length = 0;

    static public UIQTETimeLeft Create()
    {
        return UIManager.Singleton.LoadUI<UIQTETimeLeft>("UI/UIQTETimeLeft", UIManager.Anchor.Bottom);
    }
    public void Init(GameObject parent)
    {
        SetParent(parent);
    }
    public void Reset(float value = 1)
    {
        if (m_hpBar == null) return;
        WindowRoot.transform.localPosition = Vector3.zero;
        m_hpBar.SetHPWithoutFlash(value);
    }
    public void SetValue(float value)
    {
        m_hpBar.HP = value;
    }
    public override void OnInit()
    {
        base.OnInit();
        m_hpBar = FindChildComponent<UIHPBar>("BarControl");
    }
    public override void OnShowWindow()
    {
        Reset();
        base.OnShowWindow();
    }
}
