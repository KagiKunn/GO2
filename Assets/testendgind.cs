using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class testendgind : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonClick()
    {
        SceneManager.LoadScene("EndingCredit");
    }
}
