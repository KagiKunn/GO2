using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroBtnScene : MonoBehaviour
{
   public void SceneChange()
   {
       SceneManager.LoadScene("HeroManagement");
       CustomLogger.Log("Change HeroManagementScene successfuly!");
   }
}
