using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Gacha : MonoBehaviour
{
    public static Gacha Instance { get; private set; }
    public List<ItemSO> items;
    public int normalProbability = 70;
    public int rareProbability = 25;
    public int uniqueProbability = 5;

    public Image resultImage;
    public Text resultText;
    public Text multiGachaResultText;

    private Dictionary<ItemRarity, int> probabilityMap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        probabilityMap = new Dictionary<ItemRarity, int>
        {
            { ItemRarity.Normal , normalProbability},
            { ItemRarity.Rare , rareProbability},
            { ItemRarity.Unique , uniqueProbability}
        };

        ItemSO GachaItem = GachaRandomItem();
        CustomLogger.Log($"Gacha Item: {GachaItem.itemName}, Rarity: {GachaItem.rarity}");

        if (resultImage != null)
        {
            resultImage.enabled = false;
        }
        if (resultText != null)
        {
            resultText.enabled = false;
        }
        
        
    }

    public void OnGachaButtonClicked()
    {
        ItemSO GachaItem = GachaRandomItem();
        DisplayResult(GachaItem);
    }

    public void OnMultiGachaButtonClicked(int GachaCount)
    {
        Dictionary<ItemRarity, int> gachaResults = new Dictionary<ItemRarity, int>
        {
            { ItemRarity.Normal, 0 },
            { ItemRarity.Rare, 0 },
            { ItemRarity.Unique, 0 }
        };

        List<ItemSO> gachaItems = new List<ItemSO>();

        for (int i = 0; i < GachaCount; i++)
        {
            ItemSO gachaItem = GachaRandomItem();
            gachaItems.Add(gachaItem);
            gachaResults[gachaItem.rarity]++;
        }

        DisplayMultiGachaResult(gachaResults, GachaCount, gachaItems);
    }

    public ItemSO GachaRandomItem()
    {
        int totalProbability = normalProbability + rareProbability + uniqueProbability;
        int randomValue = Random.Range(0, totalProbability);
        ItemRarity selectedRarity = ItemRarity.Normal;

        if (randomValue < normalProbability)
        {
            selectedRarity = ItemRarity.Normal;
        } 
        else if (randomValue < normalProbability + rareProbability)
        {
            selectedRarity = ItemRarity.Rare;
        }
        else
        {
            selectedRarity = ItemRarity.Unique;
        }

        List<ItemSO> filteredItems = items.FindAll(item => item.rarity == selectedRarity);
        int randomIndex = Random.Range(0, filteredItems.Count);

        return filteredItems[randomIndex];
    }

    private void DisplayResult(ItemSO item)
    {
        if (resultImage != null)
        {
            resultImage.sprite = item.icon;
            resultImage.enabled = true;
        }

        if (resultText != null)
        {
            resultText.text = $"Item: {item.itemName}\nRarity: {item.rarity}";
            resultText.enabled = true;
        }
    }

    void DisplayMultiGachaResult(Dictionary<ItemRarity, int> gachaResults, int gachaCount, List<ItemSO> gachaItems)
    {
        if (multiGachaResultText != null)
        {
            float normalPercent = (float)gachaResults[ItemRarity.Normal] / gachaCount * 100;
            float rarePercent = (float)gachaResults[ItemRarity.Rare] / gachaCount * 100;
            float uniquePercent = (float)gachaResults[ItemRarity.Unique] / gachaCount * 100;
            
            String resultText = $"Result of {gachaCount} Gachas:\n" +
                                $"Normal: {gachaResults[ItemRarity.Normal]} ({normalPercent:F2}%)\n" +
                                $"Rare: {gachaResults[ItemRarity.Rare]} ({rarePercent:F2}%)\n" +
                                $"Unique: {gachaResults[ItemRarity.Unique]} ({uniquePercent:F2}%)\n\n" +
                                "Items:\n";

            foreach (var item in gachaItems)
            {
                resultText += $"Item: {item.itemName}, Rarity: {item.rarity}\n";
            }

            multiGachaResultText.text = resultText;
            CustomLogger.Log(this.resultText);





            // multiGachaResultText.text = $"Result of {gachaCount} Gachas:\n" +
            //                             $"Normal: {gachaResults[ItemRarity.Normal]} ({normalPercent:F2}%)\n" +
            //                             $"Rare: {gachaResults[ItemRarity.Rare]} ({rarePercent:F2}%)\n" +
            //                             $"Unique: {gachaResults[ItemRarity.Unique]} ({uniquePercent:F2}%)";
            // Debug.Log($"Result of {gachaCount} Gachas:\n" +
            //           $"Normal: {gachaResults[ItemRarity.Normal]} ({normalPercent:F2}%)\n" +
            //           $"Rare: {gachaResults[ItemRarity.Rare]} ({rarePercent:F2}%)\n" +
            //           $"Unique: {gachaResults[ItemRarity.Unique]} ({uniquePercent:F2}%)");
        }
    }
}
