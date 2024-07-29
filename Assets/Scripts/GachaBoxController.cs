using UnityEngine;

public class GachaBoxController : MonoBehaviour
{
    private Animator animator;
    public Gacha gacha;
    private bool isMultiGacha = false;
    private int gachaCount = 1;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }


    public void OpenBox()
    {
        animator.SetTrigger("Open");
        Invoke("ShowItemInfo",0.65f);
    }

    public void ShowItemInfo()
    {
        if (isMultiGacha)
        {
            gacha.OnMultiGachaButtonClicked(gachaCount);
        }
        else
        {
            gacha.OnGachaButtonClicked();
        }
        EnableUIElements();
    }

    void EnableUIElements()
    {
        if (Gacha.Instance.resultImage != null)
        {
            Gacha.Instance.resultImage.enabled = true;
        }

        if (isMultiGacha)
        {
            if (Gacha.Instance.multiGachaResultText != null)
            {
                Gacha.Instance.multiGachaResultText.enabled = true;
            }

            if (Gacha.Instance.resultText != null)
            {
                Gacha.Instance.resultText.enabled = false;
            }
        }
        else
        {
            if (Gacha.Instance.resultText != null)
            {
                Gacha.Instance.resultText.enabled = true;
            }

            if (Gacha.Instance.multiGachaResultText != null)
            {
                Gacha.Instance.multiGachaResultText.enabled = false;
            }
        }
    }
    
    public void SetMultiGacha(bool isMulti, int count)
    {
        isMultiGacha = isMulti;
        gachaCount = count;
    }

    public void SingGacha()
    {
        SetMultiGacha(false,1);
        OpenBox();
    }

    public void MultiGacha(int count)
    {
        SetMultiGacha(true, count);
        OpenBox();
    }
}
