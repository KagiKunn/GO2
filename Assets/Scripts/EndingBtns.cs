using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingBtns : MonoBehaviour
{
    public void OnLoadTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
