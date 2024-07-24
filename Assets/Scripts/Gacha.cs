using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gacha : MonoBehaviour
{
    public List<ItemSO> items;
    private ItemRarity[] itemList;
    private Randomizer rand;
    public int normalProbability = 70;
    public int rareProbability = 25;
    public int uniqueProbability = 5;

    public Image resultImage;
    public Text resultText;
    public Text multiGachaResultText;


    private void Start()
    {
        InitializeRandomizer();
        InitializeItems();
        ItemSO GachaItem = GachaRandomItem();
        Debug.Log($"Gacha Item: {GachaItem.itemName}, Rarity: {GachaItem.rarity}");
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

        for (int i = 0; i < GachaCount; i++)
        {
            ItemSO gachaItem = GachaRandomItem();
            gachaResults[gachaItem.rarity]++;
        }

        DisplayMultiGachaResult(gachaResults, GachaCount);
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
        }

        if (resultText != null)
        {
            resultText.text = $"Item: {item.itemName}\nRarity: {item.rarity}";
        }
    }

    void DisplayMultiGachaResult(Dictionary<ItemRarity, int> gachaResults, int gachaCount)
    {
        if (multiGachaResultText != null)
        {
            float normalPercent = (float)gachaResults[ItemRarity.Normal] / gachaCount * 100;
            float rarePercent = (float)gachaResults[ItemRarity.Rare] / gachaCount * 100;
            float uniquePercent = (float)gachaResults[ItemRarity.Unique] / gachaCount * 100;

            multiGachaResultText.text = $"Result of {gachaCount} Gachas:\n" +
                                        $"Normal: {gachaResults[ItemRarity.Normal]} ({normalPercent:F2}%)\n" +
                                        $"Rare: {gachaResults[ItemRarity.Rare]} ({rarePercent:F2}%)\n" +
                                        $"Unique: {gachaResults[ItemRarity.Unique]} ({uniquePercent:F2}%)";
            Debug.Log($"Result of {gachaCount} Gachas:\n" +
                      $"Normal: {gachaResults[ItemRarity.Normal]} ({normalPercent:F2}%)\n" +
                      $"Rare: {gachaResults[ItemRarity.Rare]} ({rarePercent:F2}%)\n" +
                      $"Unique: {gachaResults[ItemRarity.Unique]} ({uniquePercent:F2}%)");
        }
    }
}