using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CastleWall : MonoBehaviour
{
   [SerializeField]
   public float health = 3000; // 성벽의 초기 체력

   [SerializeField] private Image gameOverImage; // 겜오버 이미지 참조변수

   public bool hasShield;
   public float shield = 0;
   private Coroutine earnShieldCoroutine;

   private Tilemap castleTile;
   private Color originColor;
   private void Start()
   {
      castleTile = GetComponent<Tilemap>();
      originColor = castleTile.color;
      if (gameOverImage != null)
      {
         gameOverImage.gameObject.SetActive(false);
      }
   }


   // 성벽이 공격당했을 때 체력을 감소시키는 함수
   public void TakeDamage(int damage)
   {
      if (hasShield)
      {
         shield -= damage;
      }
      else
      {
         health -= damage;
      }

      if (shield <= 0 && hasShield)
      {
         hasShield = false;
         CustomLogger.Log("보호막이 파괴되었습니다!");
      }
      if (health <= 0)
      {
         Destroy(gameObject);
         CustomLogger.Log("성벽이 파괴되었습니다!");
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

   public void EarnShield(float duration, float shieldAmount)
   {
      Debug.Log("EarnShield called with duration: " + duration + " and shieldAmount: " + shieldAmount);

      // 기존의 코루틴이 실행 중이면 중지합니다.
      if (earnShieldCoroutine != null)
      {
         shield = 0;
         StopCoroutine(earnShieldCoroutine);
      }

      ChangeWallColor(true);
      hasShield = true;
      shield += shieldAmount;
      earnShieldCoroutine = StartCoroutine(ResetEarnShieldAfterDelay(duration));
   }
   private IEnumerator ResetEarnShieldAfterDelay(float delay)
   {
      yield return new WaitForSeconds(delay);
      ChangeWallColor(false);
      hasShield = false;
      shield = 0;
     
      Debug.Log("Shield reset to original values for: " + gameObject.name);
   }

   void ChangeWallColor(bool boo)
   {
      if (boo)
      {
         castleTile.color = Color.cyan;
      }
      else
      {
         castleTile.color = originColor;
      }
   }
}
