using UnityEngine;

public class Boss : MonoBehaviour
{
    public delegate void BossDefeatedHandler();
    public event BossDefeatedHandler OnBossDefeated;

    // 보스가 쓰러질 때 호출되는 함수
    public void DefeatBoss()
    {
        if (OnBossDefeated != null)
        {
            OnBossDefeated();
        }
        gameObject.SetActive(false);
    }
}