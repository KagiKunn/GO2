using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BtnScene : MonoBehaviour
{
   public void heroSceneChange()
   {
       SceneManager.LoadScene("HeroManagement");
       // 편성한 영웅 정보 불러오기
       
       CustomLogger.Log("Change HeroManagementScene successfuly!");
   }

   public void internalSceneChange()
   {
       SceneManager.LoadScene("InternalAffairs");
       CustomLogger.Log("Change InternalAffairsScene successfuly!");
   }

   public void DefenseSceneChange()
   {
       SceneManager.LoadScene("Defense");
       CustomLogger.Log("Change Defense successfuly!");
   }
   
}
