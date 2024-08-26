using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetConfrim : MonoBehaviour
{
    public void resetGame()
    {
        PlayerLocalManager.Instance.CreateNewPlayer();
        SceneManager.LoadScene("RoguePoint");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		    Application.Quit();
#endif
    }
}
