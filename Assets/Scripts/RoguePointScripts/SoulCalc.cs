using System;
using TMPro;
using UnityEngine;

public class SoulCalc : MonoBehaviour
{
    private int totSoul;
    private int crntSoul;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        totSoul = int.Parse(gameObject.GetComponent<TextMeshProUGUI>().text ?? "0");
        crntSoul = totSoul;

    }

    // Update is called once per frame
    public bool SoulIncDec(bool boo)
    {
        int resSoul = int.Parse(gameObject.GetComponent<TextMeshProUGUI>().text);
        if (boo)
        {
            if (resSoul > 0)
            {
                resSoul -= 1;
            }
            else
            {
                return false;
            }

        }
        else
        {
            resSoul += 1;
        }

        gameObject.GetComponent<TextMeshProUGUI>().text = resSoul.ToString();
        return true;
    }
}
