using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetConfrim : MonoBehaviour
{
    public void resetGame()
    {
        PlayerLocalManager.Instance.CreateNewPlayer();
        SceneManager.LoadScene("RoguePoint");
    }
}
