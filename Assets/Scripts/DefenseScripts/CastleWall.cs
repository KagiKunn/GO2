using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CastleWall : MonoBehaviour
{
    //CastleWall 클래스는 성벽이 데미지를 받을 때 CastleWallManager에 처리를 위임하고, 성벽 오브젝트를 삭제하는 역할만 담당
    private Tilemap castleTile;
    private Color originColor;

    private void Awake()
    {
        castleTile = GetComponent<Tilemap>();
        originColor = castleTile.color;
    }

    public void TakeDamage(float damage)
    {
        string wallTag = gameObject.tag;
        CastleWallManager.Instance.ApplyDamage(damage);

        Debug.Log($"Damage applied to {wallTag}");
        if (CastleWallManager.Instance.GetHealth() <= 0)
        {
            Debug.Log($"{wallTag} destroyed.");
            Destroy(gameObject);  // 성벽 오브젝트 삭제
        }
    }

    public void EarnShield(float duration, float shieldAmount)
    {
        Debug.Log("EarnShield called with duration: " + duration + " and shieldAmount: " + shieldAmount);
        ChangeWallColor(true);  // 실드 활성화 시 색상 변경
        CastleWallManager.Instance.EarnShield(duration, shieldAmount);
    }

    private void ChangeWallColor(bool activateShield)
    {
        if (activateShield)
        {
            castleTile.color = Color.cyan;
        }
        else
        {
            castleTile.color = originColor;
        }
    }
}