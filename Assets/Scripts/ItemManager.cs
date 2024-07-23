using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
   public List<ItemSO> items;

   private void Start()
   {
      foreach (ItemSO item in items)
      {
         // Debug.Log("Item: " + item.itemName+ ", Rarity" + item.rarity);
      }
   }
}
