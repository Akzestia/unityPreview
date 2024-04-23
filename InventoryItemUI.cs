using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemUI : MonoBehaviour
{
    public string ItemName { get; set; }
    public string ItemDescription { get; set; }
    public int Quantity { get; set; }
    public Sprite ItemImage { get; set; }

    public void UpdateUI()
    {

    }

    public void SetData(KeyValuePair<string, int> item)
    {
        this.ItemName = item.Key;
        this.Quantity = item.Value;
    }
}
