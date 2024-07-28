using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gacha : MonoBehaviour
{
    public static Gacha Instance { get; private set; }
    public List<ItemSO> items;
    private ItemRarity[] itemList;
    private Randomizer rand;
    public int normalProbability = 70;
    public int rareProbability = 25;
    public int uniqueProbability = 5;

    public Image resultImage;
    public Text resultText;
    public Text multiGachaResultText;


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
        InitializeRandomizer();
        InitializeItems();
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

    private void InitializeItems()
    {
        itemList = new ItemRarity[normalProbability+rareProbability+uniqueProbability];
        var index = 0;

        // 확률에 맞는 배열 생성
        for (var i = 0; i < normalProbability; i++, index++)
        {
            itemList[index] = ItemRarity.Normal;
        }

        for (var i = 0; i < rareProbability; i++, index++)
        {
            itemList[index] = ItemRarity.Rare;
        }

        for (var i = 0; i < uniqueProbability; i++, index++)
        {
            itemList[index] = ItemRarity.Unique;
        }

        // 배열을 랜덤하게 섞기
        for (var i = itemList.Length - 1; i > 0; i--)
        {
            var j = rand.NextInt(i + 1);
            (itemList[i], itemList[j]) = (itemList[j], itemList[i]);
        }
    }

    private void InitializeRandomizer()
    {
        if (PlayerDataControl.Instance != null)
        {
            Guid uuid = Guid.Parse(PlayerDataControl.Instance.UUID);
            byte[] bytes = uuid.ToByteArray();
            ulong seed1 = BitConverter.ToUInt64(bytes, 0);
            ulong seed2 = BitConverter.ToUInt64(bytes, 8);
            rand = new Randomizer(seed1, seed2);
        }
        else
        {
            Guid uuid = Guid.Parse("123e4567-e89b-12d3-a456-426614174000");
            byte[] bytes = uuid.ToByteArray();
            ulong seed1 = BitConverter.ToUInt64(bytes, 0);
            ulong seed2 = BitConverter.ToUInt64(bytes, 8);
            rand = new Randomizer(seed1, seed2);
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

    private ItemSO GachaRandomItem()
    {

        int randomValue = rand.NextInt(0, 99);

        ItemRarity selectedRarity = itemList[randomValue];

        List<ItemSO> filteredItems = items.FindAll(item => item.rarity == selectedRarity);
        int randomIndex = rand.NextInt(0, filteredItems.Count - 1);

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