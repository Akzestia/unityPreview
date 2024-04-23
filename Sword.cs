using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Interactable
{
    public Transform rightHand;  
    public Weapon weaponScript;
    public override void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);

        Vector3 offset = new Vector3(0, -0.1f, 0f); // Replace xOffset, yOffset, zOffset with the desired offset values
        Vector3 backpackPosition = rightHand.position + offset;
        gameObject.transform.position = backpackPosition;
        gameObject.transform.rotation = Quaternion.Euler(rightHand.rotation.eulerAngles.x, rightHand.rotation.eulerAngles.y, rightHand.rotation.eulerAngles.z + 170f);
        gameObject.transform.SetParent(rightHand);

        if (weaponScript != null)
        {
             Weapon addedWeaponScript = gameObject.AddComponent(weaponScript.GetType()) as Weapon;

            // Find the player GameObject by name
            GameObject playerGameObject = GameObject.Find("SwordWarrior"); // Replace "Player" with the actual name of your player GameObject

            // Initialize the player instance in the Weapon script
            addedWeaponScript.player = playerGameObject;
        }
        gameObject.layer = 2;
        isEquipped = true;

        var rigidbody = gameObject.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = true;
        }

        var boxCollider = gameObject.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
        }
    }
}
