using System;
using UnityEngine;

public class CastleWall : MonoBehaviour
{
   [SerializeField]
   public int health = 3000; // 성벽의 초기 체력

   // 성벽이 공격당했을 때 체력을 감소시키는 함수
   public void TakeDamage(int damage)
   {
      health -= damage;
      Debug.Log("성벽의 체력: " + health);
      if (health <= 0)
      {
         Destroy(gameObject);
         Debug.Log("성벽이 파괴되었습니다!");
      }
   }
}
