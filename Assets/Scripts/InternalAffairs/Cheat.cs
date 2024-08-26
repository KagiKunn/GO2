using UnityEngine;

public class Cheat : MonoBehaviour
{
    public void ShowMeTheMoney()
    {
        PlayerLocalManager.Instance.lMoney = 10000;
    }
}
