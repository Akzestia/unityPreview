using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerMovementMAIN : MonoBehaviour
{
    public float interactionDistance = 2f;
    public float speed = 5.0f;
    public float mouseSensitivity = 1500.0f; // Adjust this to your liking
    private float xRotation = 0.0f;
    private Animator animator;
    public Transform camera;
    public AnimatorController idle;
    public AnimatorController forward;
    public AnimatorController backward;
    public AnimatorController left;
    public AnimatorController leftBackward;
    public AnimatorController leftForward;
    public AnimatorController right;
    public AnimatorController rightBackward;
    public AnimatorController rightForward;
    public AnimatorController swordHit;
    public AnimatorController shieldHit;
    public AnimatorController tookDamage;
    public AnimatorController sprint;
    public bool isAttacking = false;
    bool isTakingDamage = false;
    private float lastTeleportTime = -10.0f;
    private float stamina = 100.0f;
    public float zoomSpeed = 30.0f;
    public float minZoom = 30.0f;
    public float maxZoom = 90.0f;
    public GameObject inventoryUI;
    private Inventory inventory;
    public GameObject inventoryItemPrefab;
    void ChangeController(AnimatorController controller)
    {
        animator.runtimeAnimatorController = controller as RuntimeAnimatorController;
    }
    int layerMask;
    void Start()
    {
        animator = GetComponent<Animator>();
        inventory = GetComponent<Inventory>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inventoryUI.SetActive(false);
        layerMask = ~LayerMask.GetMask("IgnoreRaycast");
        interactionDistance = 2f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemySword"))
        {
            animator.SetBool("TakeDamage", true);
            ChangeController(tookDamage);
            Debug.Log("Character took damage!");
        }
    }

    public void UpdateInventoryUI()
    {

        if (inventoryUI == null)
        {
            Debug.LogError("inventoryUI is not assigned in the Unity editor.");
            return;
        }

        // Get the Scroll View's content
        Transform content = inventoryUI.transform.Find("Viewport/Content");

        if (content == null)
        {
            Debug.LogError("inventoryUI does not have a child with the path 'Viewport/Content'.");
            return;
        }

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Get the ScrollRect component
        ScrollRect scrollRect = inventoryUI.GetComponent<ScrollRect>();

        // Enable scrolling
        scrollRect.horizontal = true; // Enable horizontal scrolling
        scrollRect.vertical = false; // Disable vertical scrolling

        // Disable other interactions
        scrollRect.movementType = ScrollRect.MovementType.Clamped; // Prevents bouncing effect
        scrollRect.inertia = false; // Disables inertia

        // Get the GridLayoutGroup component or add one if it doesn't exist
        GridLayoutGroup gridLayout = content.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            gridLayout = content.gameObject.AddComponent<GridLayoutGroup>();
            if (gridLayout == null)
            {
                Debug.LogError("Failed to add GridLayoutGroup component to content.");
                return;
            }
        }

        // Set the properties of the GridLayoutGroup
        gridLayout.cellSize = new Vector2(28, 35); // Smaller cell size
        gridLayout.spacing = new Vector2(10, 10); // Smaller spacing
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal; // Change this to Horizontal
        gridLayout.childAlignment = TextAnchor.UpperCenter;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 5;
        gridLayout.padding = new RectOffset(20, 10, 10, 10); // No padding
        // Determine the number of items you want to instantiate// Change this to the number of items in the inventory
        foreach (KeyValuePair<string, int> itemx in inventory.GetItems())
        {
            if (inventoryItemPrefab == null)
            {
                Debug.LogError("inventoryItemPrefab is not assigned in the Unity editor.");
                return;
            }

            GameObject item = Instantiate(inventoryItemPrefab, content);
            InventoryItemUI inventoryItemUI = item.GetComponent<InventoryItemUI>();
            Transform childTransform = item.transform.Find("Item Name");
            Transform btn = item.transform.Find("Button (Legacy)");
            if (childTransform != null)
            {
                TextMeshProUGUI uiText = childTransform.GetComponent<TextMeshProUGUI>();
                if (uiText != null)
                {
                    uiText.text = itemx.Key + " x" + itemx.Value;
                }
                else
                {
                    Debug.LogError("The 'Item Name' child does not have a TextMeshProUGUI component.");
                }
            }
            else
            {
                Debug.LogError("Could not find a child named 'Item Name'.");
            }


            if (btn)
            {
                Button button = btn.GetComponent<Button>();
                button.onClick.AddListener(() => OnInventoryItemClick(itemx));
            }
            else
            {
                Debug.LogError("Could not find a child namSed 'Button (Legacy)'.");
            }

            if (inventoryItemUI == null)
            {
                Debug.LogError("inventoryItemPrefab does not have an InventoryItemUI component.");
                return;
            }
            inventoryItemUI.SetData(itemx);

        }
    }

    void OnInventoryItemClick(KeyValuePair<string, int> item)
    {
        Debug.Log("Clicked on inventory item: " + item.Key);
    }
    Interactable lastHit;

    float distanceToMove = 2.7f;
    void Update()
    {
        Vector3 newOrigin = Camera.main.transform.position + Camera.main.transform.forward * distanceToMove;
        Ray ray = new Ray(newOrigin, Camera.main.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red, 1f);
        if (Physics.Raycast(ray, out hit, interactionDistance, layerMask))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                if (lastHit != null && lastHit != interactable)
                {
                    lastHit.Unhighlight();
                    lastHit.HideName();
                }

                interactable.Highlight();
                interactable.ShowName();
                lastHit = interactable;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    Debug.Log("Interacting with " + interactable.name);
                    interactable.Interact();
                }
            }
            else
            {
                if (lastHit != null)
                {
                    lastHit.Unhighlight();
                    lastHit.HideName();
                    lastHit = null;
                }
            }
        }
        else
        {
            if (lastHit != null)
            {
                lastHit.Unhighlight();
                lastHit.HideName();
                lastHit = null;
            }
        }

        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (inventoryUI.activeSelf == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = camera.forward * moveVertical + camera.right * moveHorizontal;
        movement.y = 0.0f;

        if (stamina < 100 && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            stamina += 0.1f;
        }

        if (!isAttacking)
        {
            transform.Translate(movement * speed * Time.deltaTime, Space.Self);
        }
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            Cursor.lockState = inventoryUI.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = inventoryUI.activeSelf;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            float teleportCooldown = 10.0f; // 10 second cooldown
            if (Time.time - lastTeleportTime < teleportCooldown)
            {
                Debug.Log("Teleport is on cooldown. Time remaining: " + (teleportCooldown - (Time.time - lastTeleportTime)));
                return;
            }

            Vector3 teleportDirection = transform.forward;
            teleportDirection.y = 0; // Only allow horizontal teleportation
            float teleportDistance = 10.0f;
            Vector3 newPosition = transform.position + teleportDirection * teleportDistance;

            // Debugging information
            Debug.Log("Teleport direction: " + teleportDirection);
            Debug.Log("Teleport distance: " + teleportDistance);
            Debug.Log("Current position: " + transform.position);
            Debug.Log("New position: " + newPosition);

            // Offset the raycast origin to be slightly above the bottom of the character's collider
            float raycastOriginOffset = 0.1f;
            Vector3 raycastOrigin = transform.position + Vector3.up * raycastOriginOffset;

            // Check if there are any obstacles in the way
            if (!Physics.Raycast(raycastOrigin, teleportDirection, teleportDistance))
            {
                // If there are no obstacles, disable the CharacterController, teleport, then re-enable it
                CharacterController characterController = GetComponent<CharacterController>();
                characterController.enabled = false;
                transform.position = newPosition;
                characterController.enabled = true;

                // Update the last teleport time
                lastTeleportTime = Time.time;
            }
            else
            {
                Debug.Log("Teleportation blocked by an obstacle.");
            }
        }
        // Get the mouse movement input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Apply the mouse movement to the rotation
        if (inventoryUI.activeSelf == false && Cursor.lockState == CursorLockMode.Locked)
        {
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Rotate the camera and the character
            camera.parent.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }


        // Rotate the player character around the Y-axis (horizontal rotation)
        if (inventoryUI.activeSelf == false && Cursor.lockState == CursorLockMode.Locked)
        {
            transform.Rotate(Vector3.up * mouseX);
        }


        if (inventoryUI.activeSelf == false)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            Camera cam = camera.GetComponent<Camera>();

            // Change the field of view based on the scroll input
            cam.fieldOfView -= scrollInput * zoomSpeed;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);
        }


        // Reset all animation parameters
        // animator.SetBool("MoveForward", false);
        // animator.SetBool("MoveBackward", false);
        // animator.SetBool("MoveLeft", false);
        // animator.SetBool("MoveRight", false);
        // animator.SetBool("MoveRightForward", false);
        // animator.SetBool("MoveRightBackward", false);
        // animator.SetBool("MoveLeftForward", false);
        // animator.SetBool("MoveLeftBackward", false);
        // animator.SetBool("SwordHit", false);
        // animator.SetBool("ShieldHit", false);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.0f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("EnemySword") && !isTakingDamage)
            {
                StartCoroutine(TakeDamageCoroutine());
            }
        }

        if (Input.GetMouseButtonDown(1) && !isAttacking && !isTakingDamage && !inventoryUI.activeSelf && Cursor.lockState == CursorLockMode.Locked)
        {
            StartCoroutine(PlayShieldHitAnimation());
        }

        if (Input.GetMouseButtonDown(0) && !isAttacking && !isTakingDamage && !inventoryUI.activeSelf && Cursor.lockState == CursorLockMode.Locked)
        {
            StartCoroutine(PlaySwordHitAnimation());
        }

        if (!isAttacking && !isTakingDamage)
        {
            if (moveVertical > 0)
            {
                if (moveHorizontal > 0)
                {
                    ChangeController(rightForward);
                    // animator.SetBool("MoveRightForward", true);
                }
                else if (moveHorizontal < 0)
                {
                    ChangeController(leftForward);
                    // animator.SetBool("MoveLeftForward", true);
                }
                else
                {
                    if (stamina > 0.0f && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        speed = 10.0f;
                        ChangeController(sprint);

                        stamina -= 0.1f;
                        if (stamina < 0.0f)
                        {
                            stamina = 0.0f;
                        }
                        Debug.Log("Stamina: " + stamina);
                        // animator.SetBool("Sprint", true);
                    }
                    else
                    {
                        speed = 5.0f;
                        ChangeController(forward);
                        // animator.SetBool("MoveForward", true);
                    }
                }
            }
            else if (moveVertical < 0)
            {
                if (moveHorizontal > 0)
                {
                    ChangeController(rightBackward);
                    // animator.SetBool("MoveRightBackward", true);
                }
                else if (moveHorizontal < 0)
                {
                    ChangeController(leftBackward);
                    // animator.SetBool("MoveLeftBackward", true);
                }
                else
                {
                    ChangeController(backward);
                    // animator.SetBool("MoveBackward", true);
                }
            }
            else if (moveHorizontal > 0)
            {
                ChangeController(right);
                // animator.SetBool("MoveRight", true);
            }
            else if (moveHorizontal < 0)
            {
                ChangeController(left);
                // animator.SetBool("MoveLeft", true);
            }

            // if (Input.GetMouseButtonDown(0))
            // {
            //     ChangeController(swordHit);
            //     // animator.SetBool("SwordHit", true);
            // }

            if (moveVertical == 0 && moveHorizontal == 0)
            {
                ChangeController(idle);
            }

            if (moveVertical == 0 && moveHorizontal == 0)
            {
                ChangeController(idle);
            }
        }

    }

    IEnumerator PlaySwordHitAnimation()
    {
        isAttacking = true;
        ChangeController(swordHit);
        // animator.SetBool("SwordHit", true);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.3f);
        // animator.SetBool("SwordHit", false);
        isAttacking = false;
        ChangeController(idle);
    }

    public bool IsPlayerAttacking()
    {
        return isAttacking;
    }

    IEnumerator PlayShieldHitAnimation()
    {
        isAttacking = true;  // Prevent other animations from starting
        ChangeController(shieldHit);
        // animator.SetBool("ShieldHit", true);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.2f);
        // animator.SetBool("ShieldHit", false);
        isAttacking = false;  //
        ChangeController(idle);
    }

    IEnumerator TakeDamageCoroutine()
    {
        isTakingDamage = true;
        // animator.SetBool("TakeDamage", true);
        ChangeController(tookDamage);
        Debug.Log("Character took damage!");

        // Wait until the "TakeDamage" animation ends
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isTakingDamage = false;
    }
}
