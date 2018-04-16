using UnityEngine;
using System.Collections;

public class UICardSortSettings : MonoBehaviour
{
    public bool enDefault = true;
    public bool enGotTime = true;
    public bool enByRarity = true;
    public bool enByPhyAttack = true;
    public bool enByMagAttack = true;
    public bool enByHp = true;
    public bool enByOccupation = true;
    public bool enByType = true;
    public bool enByLevel = true;
    public bool enByFavorite = true;
    public bool enCanEvolve = true;
    public bool enGold = true;
    public bool enRing = true;

    void Start()
    {
        //UISort sort = UISort.GetInstance();
        //sort.Init(this);
    }

}
