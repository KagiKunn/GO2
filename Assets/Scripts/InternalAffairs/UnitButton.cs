using UnityEngine;

public class UnitButton : MonoBehaviour
{
    public void NextButton()
    {
        GameObject.Find("Panel").GetComponent<UnitShopInit>().NextUnitButton();
    }
    
    public void PrevButton()
    {
        GameObject.Find("Panel").GetComponent<UnitShopInit>().PrevUnitButton();
    }
}
