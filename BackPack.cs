using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackPack : Interactable
{
    public Transform upperChest; // The B-upperChest component on your character

    public override void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);

        Vector3 backpackPosition = upperChest.position - upperChest.forward * 0.25f; // Adjust the multiplier as needed
        gameObject.transform.position = backpackPosition;
        gameObject.transform.rotation = Quaternion.Euler(upperChest.rotation.eulerAngles.x, upperChest.rotation.eulerAngles.y - 90f, upperChest.rotation.eulerAngles.z - 16f);

        gameObject.transform.SetParent(upperChest);
        gameObject.layer = 2;
        isEquipped = true;

        var rigidbody = gameObject.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = true;
        }
    }
}
