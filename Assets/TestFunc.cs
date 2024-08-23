using UnityEngine;

public class TestFunc : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnGoldButton()
    {
        PlayerLocalManager.Instance.lMoney = 10000;
    }
}
