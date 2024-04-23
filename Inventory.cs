using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Dictionary<string, int> inventory = new Dictionary<string, int>();
    public TextMeshProUGUI uiText;
    public GameObject player;
    PlayerMovementMAIN playerController;
    public Button myButton;
    void Start()
    {
        Debug.Log("Inventory script started!");
        // myButton.interactable = false; // Disable the button and change its color to gray
        Debug.Log("Button disabled!");
        myButton.onClick.AddListener(() => StartCoroutine(HandleButtonClick()));
        Debug.Log("Button added!");

        inventory["wood"] = 0;
        inventory["stone"] = 0;
        inventory["iron"] = 0;
        inventory["silver"] = 0;
        inventory["gold"] = 0;
        inventory["copper"] = 0;
        inventory["coal"] = 0;
        inventory["diamond"] = 0;
        inventory["titanium"] = 0;
        inventory["aluminium"] = 0;
        inventory["food"] = 0;
        inventory["water"] = 0;

        // GameObject uiTextObject = GameObject.Find("NameOfYourTextObject");
        // if (uiTextObject != null)
        // {
        //     uiText = uiTextObject.GetComponent<TextMeshProUGUI>();
        // }
        // Update the UI text
        playerController = player.GetComponent<PlayerMovementMAIN>();
        UpdateUIText();
    }
    IEnumerator HandleButtonClick()
    {
        Debug.Log("Button clicked!");

        AddItem("gold", 200);

        Debug.Log("Button click handled!");
        myButton.gameObject.SetActive(false); 

        yield break;
    }
    public void AddItem(string item, int quantity = 1)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item] += quantity;
        }
        else
        {
            inventory[item] = quantity;
        }
        Debug.Log("Added " + quantity + " of item: " + item);

        UpdateUIText();
        playerController.UpdateInventoryUI();
    }

    public void RemoveItem(string item, int quantity = 1)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item] -= quantity;
            if (inventory[item] <= 0)
            {
                inventory.Remove(item);
            }
            Debug.Log("Removed " + quantity + " of item: " + item);
        }
        else
        {
            Debug.Log("Could not remove item, item not found: " + item);
        }

        UpdateUIText();
        playerController.UpdateInventoryUI();
    }

    public bool HasItem(string item)
    {
        return inventory.ContainsKey(item);
    }

    public void PrintInventory()
    {
        foreach (KeyValuePair<string, int> item in inventory)
        {
            Debug.Log("Item: " + item.Key + ", Quantity: " + item.Value);
        }
    }

    public Dictionary<string, int> GetItems()
    {
        return inventory;
    }

    void UpdateUIText()
    {
        string woodText = inventory["wood"] >= 10 ? "Collected" : inventory["wood"].ToString() + "/10";
        string stoneText = inventory["stone"] >= 5 ? "Collected" : inventory["stone"].ToString() + "/5";

        if (inventory["wood"] >= 10 && inventory["stone"] >= 5)
        {
            myButton.interactable = true; // Enable the button and change its color to lime
        }
        else
        {
            myButton.interactable = false; // Disable the button and change its color to gray
        }
        //62 255 0
        uiText.text = "Resources to be collected:\n\n\tWood: " + woodText + "\n\n\tStone: " + stoneText + "";
    }
}
