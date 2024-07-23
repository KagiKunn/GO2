using UnityEngine;

public class GameTurn : MonoBehaviour
{
    private int gameTurn;
    private int maxTurn;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void IncreaseGameTurn()
    {
        if (gameTurn == maxTurn)
        {
            CustomLogger.Log("GameOver","green");
        }
        gameTurn++;
    }
}
