using System;
using UnityEngine;
using UnityEngine.UI;

public class CastleWall : MonoBehaviour
{
   [SerializeField]
   public int health = 3000; // 성벽의 초기 체력

   [SerializeField] private Image gameOverImage; // 겜오버 이미지 참조변수


   private void Start()
   {
      if (gameOverImage != null)
      {
         gameOverImage.gameObject.SetActive(false);
      }
   }


   // 성벽이 공격당했을 때 체력을 감소시키는 함수
   public void TakeDamage(int damage)
   {
      health -= damage;
      // Debug.Log("성벽의 체력: " + health);
      if (health <= 0)
      {
         Destroy(gameObject);
         Debug.Log("성벽이 파괴되었습니다!");
         ShowGameOverImage();
      }
   }

   private void ShowGameOverImage()
   {
      if (gameOverImage != null)
      {
         gameOverImage.gameObject.SetActive(true);
      }
   }
}
