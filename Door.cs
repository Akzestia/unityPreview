using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    
    public GameObject door;
    public bool isOpen = false;
    private Animator animator;
    void Start()
    {
        animator = door.GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    public override void Interact()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);
        Debug.Log("Interacting with door");
        Debug.Log("Door is open: " + isOpen);
    }
}
