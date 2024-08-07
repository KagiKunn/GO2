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
        Invoke("ShowItemInfo", 0.65f);
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
        if (isMultiGacha)
        {
            if (Gacha.Instance.resultText != null)
            {
                Gacha.Instance.resultText.enabled = false;
            }

            if (Gacha.Instance.resultImage != null)
            {
                Gacha.Instance.resultImage.enabled = false;
            }
        }
        else
        {
            Transform multiGachaParent;
            
            if (Gacha.Instance.inventoryUI != null && !Gacha.Instance.inventoryUI.CanAdditems(1))
            {
                if (Gacha.Instance.resultImage != null)
                {
                    Gacha.Instance.resultImage.enabled = false;
                }

                if (Gacha.Instance.resultText != null)
                {
                    Gacha.Instance.resultText.enabled = false;
                }

                if (Gacha.Instance.multiGachaResultText != null)
                {
                    Gacha.Instance.multiGachaResultText.enabled = false;
                }

                multiGachaParent = Gacha.Instance.multiGachaResultText.transform.parent;
                foreach (Transform child in multiGachaParent)
                {
                    if (child.name.StartsWith("GachaItem"))
                    {
                        child.gameObject.SetActive(false);
                    }
                }
                return;
            }
            
            if (Gacha.Instance.resultImage != null)
            {
                Gacha.Instance.resultImage.enabled = true;
            }

            if (Gacha.Instance.resultText != null)
            {
                Gacha.Instance.resultText.enabled = true;
            }

            if (Gacha.Instance.multiGachaResultText != null)
            {
                Gacha.Instance.multiGachaResultText.enabled = false;
            }

            multiGachaParent = Gacha.Instance.multiGachaResultText.transform.parent;
            foreach (Transform child in multiGachaParent)
            {
                if (child.name.StartsWith("GachaItem"))
                {
                    child.gameObject.SetActive(false);
                }
            }
            
        }
    }

    public void SetMultiGacha(bool isMulti, int count)
    {
        isMultiGacha = isMulti;
        gachaCount = count;
    }

    public void SingleGacha()
    {
        if (Gacha.Instance.inventoryUI != null && Gacha.Instance.inventoryUI.CanAdditems(1))
        {
            SetMultiGacha(false, 1);
            OpenBox();
        }
        else
        {
            CustomLogger.LogWarning("인벤토리에 충분한 공간이 없습니다.");
        }
        
    }

    public void MultiGacha(int count)
    {
        if (Gacha.Instance.inventoryUI != null && Gacha.Instance.inventoryUI.CanAdditems(count))
        {
            SetMultiGacha(true, count);
            OpenBox();
        }
        else
        {
            CustomLogger.LogWarning("인벤토리에 충분한 공간이 없습니다.");
        }
        
    }
}