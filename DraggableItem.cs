using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 originalPosition;
    private Transform originalParent;
    private RectTransform myRectTransform;
    private RectTransform canvasRectTransform;
    private Vector2 dragOffset;

    private void Awake()
    {
        myRectTransform = GetComponent<RectTransform>();
        canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = myRectTransform.anchoredPosition;
        originalParent = transform.parent;

        // Calculate the offset from the object's position to the mouse cursor
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out mousePosition);
        Vector3 mousePosition3D = canvasRectTransform.InverseTransformPoint(mousePosition);
        dragOffset = myRectTransform.anchoredPosition - new Vector2(mousePosition3D.x, mousePosition3D.y);

        // Make the item a child of the canvas while it's being dragged
        transform.SetParent(canvasRectTransform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            // Apply the offset and clamp the position so the item stays within the ScrollView
            localPoint += dragOffset;
            float clampedX = Mathf.Clamp(localPoint.x, canvasRectTransform.rect.min.x + myRectTransform.rect.width / 2, canvasRectTransform.rect.max.x - myRectTransform.rect.width / 2);
            float clampedY = Mathf.Clamp(localPoint.y, canvasRectTransform.rect.min.y + myRectTransform.rect.height / 2, canvasRectTransform.rect.max.y - myRectTransform.rect.height / 2);
            myRectTransform.anchoredPosition = new Vector2(clampedX, clampedY);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Return the item to its original parent when the drag operation ends
        transform.SetParent(originalParent);
        myRectTransform.anchoredPosition = originalPosition;
    }
}