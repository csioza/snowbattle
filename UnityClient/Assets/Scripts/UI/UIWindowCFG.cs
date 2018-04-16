using UnityEngine;
using System.Collections;

public enum ENUIWindowLevel
{
    enRoot,
    enBranches,
    enTrough,
}
public class UIWindowCFG : MonoBehaviour
{
    public ENUIWindowLevel m_uiWindowType = ENUIWindowLevel.enBranches;
    public int m_uiWndLevel = 0;
    public bool m_isHideOther = true;
}
