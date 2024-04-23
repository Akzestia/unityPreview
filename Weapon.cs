using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject player;
    private PlayerMovementMAIN playerMovement;
    private bool hasTriggered = false;
    public Inventory inventory;
    void Start()
    {
        playerMovement = player.GetComponent<PlayerMovementMAIN>();
        inventory = player.GetComponent<Inventory>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (playerMovement.IsPlayerAttacking() && !hasTriggered)
        {
            switch(other.gameObject.tag)
            {
                case "TrreReesource":
                    Debug.Log("Gained resource: wood");
                    hasTriggered = true;
                    inventory.AddItem("wood", 1);
                    break;
                 case "StoneResource":
                    Debug.Log("Gained resource: stone");
                    hasTriggered = true;
                    inventory.AddItem("stone", 1);
                    break;
                default:
                    break;
            }
        }
    }

    void Update()
    {
        if(!playerMovement.IsPlayerAttacking() && hasTriggered)
        {
            hasTriggered = false;
        }
    }
}
