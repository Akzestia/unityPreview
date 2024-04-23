using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Interactable : MonoBehaviour
{
    private TextMeshPro nameText;
    private Material material;
    private Color originalColor;
    protected bool isEquipped = false;
    void Start()
    {
        // Get the material of the object
        GameObject textObject = new GameObject("NameText");
        nameText = textObject.AddComponent<TextMeshPro>();

        // Set the parent of the TextMeshPro object to this GameObject
        textObject.transform.SetParent(transform);

        // Position the TextMeshPro object above this GameObject
        // Get the bounds of the object
        Bounds objectBounds = GetComponent<Renderer>().bounds;

        // Position the TextMeshPro object above and at the center of this GameObject
        textObject.transform.position = objectBounds.center + new Vector3(0, objectBounds.extents.y + 0.2f, 0);

        // Set the properties of the TextMeshPro object
        nameText.text = name;
        nameText.enabled = false;
        nameText.fontSize = 1.2f;
        nameText.alignment = TextAlignmentOptions.Center;

        // Get the material of the object
        material = GetComponent<Renderer>().material;

        // Store the original color of the object
        originalColor = material.color;
    }

    void Update()
    {
        nameText.transform.LookAt(Camera.main.transform.position);
        nameText.transform.Rotate(0, 180, 0);
    }

    public void Highlight()
    {
        if (!isEquipped && material)
        {
            material.color = Color.yellow;
        }

    }

    public void Unhighlight()
    {
        if(material){
            material.color = originalColor;
        }
    }

    public virtual void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
        ShowName();
    }

    public void ShowName()
    {
        if(!isEquipped && nameText){
            nameText.enabled = true;
        }
    }

    public void HideName()
    {
        if(nameText){
            nameText.enabled = false;
        }
    }
}
