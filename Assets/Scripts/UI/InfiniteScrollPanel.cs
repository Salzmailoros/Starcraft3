using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfiniteScrollPanel : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    //Main objects etc.
    [SerializeField] private ScrollRect mainScroller;
    [SerializeField] private float outOfBoundsOffset = 50f;  // Extra offset for out-of-bounds detection
    [SerializeField] private int itemsPerRow = 2;  // Number of items per row (can be adjusted in Inspector)
    //mouse stuff
    private Vector2 dragStartPos;
    private Vector2 lastFramePos;
    private bool dragDirectionUp;
    private float itemSpacing;

    //screen stuff
    private Vector3[] viewportCorners = new Vector3[4];
    private float upperThreshold;
    private float lowerThreshold;
    private void Start()
    {
        // Calculate item spacing dynamically based on the height of the first item
        itemSpacing = mainScroller.content.GetChild(0).GetComponent<RectTransform>().rect.height;
        mainScroller.viewport.GetWorldCorners(viewportCorners);
        upperThreshold = viewportCorners[1].y + outOfBoundsOffset;
        lowerThreshold = viewportCorners[3].y - outOfBoundsOffset;
        itemSpacing = itemSpacing * (Screen.height / 1080f);
        int offset = mainScroller.content.childCount % itemsPerRow;
        if (offset != 0)       //fill the row with empty gameobjects for the empty slots for symmetry purposes and simpler logic :P
        {
            var emptyObject = new GameObject();
            for (int i = 0; i < offset; i++)
            {
                Instantiate(emptyObject, mainScroller.content.GetChild(mainScroller.content.childCount-1).transform.position,Quaternion.identity,mainScroller.content);// place at correct Y position
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPos = eventData.position;
        lastFramePos = dragStartPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragDirectionUp = eventData.position.y >= lastFramePos.y;
        lastFramePos = eventData.position;
    }

    public void ScrollValueChange(Vector2 v2) // Called from event on the ScrollRect GameObject.
    {
        if (dragDirectionUp)
        {
            Transform firstItem = mainScroller.content.GetChild(0);
            if (IsOutOfBounds(firstItem, true))
            {
                MoveRowToBottom();
            }
        }
        else
        {
            Transform lastItem = mainScroller.content.GetChild(mainScroller.content.childCount - 1);
            if (IsOutOfBounds(lastItem, false))
            {
                MoveRowToTop();
            }
        }
    }

    private void MoveRowToBottom()
    {

        Transform item = mainScroller.content.GetChild(0);
        Transform lastItem = mainScroller.content.GetChild(mainScroller.content.childCount - (itemsPerRow)); // other end of the stack. just need the Y position.
        Vector3 newPosition = lastItem.position;
        newPosition.y -= itemSpacing;
        item.position = new Vector3(item.position.x, newPosition.y, item.position.z);
        item.SetSiblingIndex(mainScroller.content.childCount);    // become the new last piece

    }

    private void MoveRowToTop()
    {

        Transform item = mainScroller.content.GetChild(mainScroller.content.childCount - 1); // Get the last item
        Transform firstItem = mainScroller.content.GetChild(itemsPerRow - 1);// object i same X pos but other end of the stack.
        Vector3 newPosition = firstItem.position;
        newPosition.y += itemSpacing;
        item.position = new Vector3(item.position.x, newPosition.y, item.position.z);
        item.SetSiblingIndex(0);

    }

    private bool IsOutOfBounds(Transform item, bool checkUpperBounds)
    {
        
        float itemY = item.position.y;
        if (checkUpperBounds)
        {
            return itemY > upperThreshold;
        }
        else
        {
            //Debug.Log(itemY < lowerThreshold);      
            return itemY < lowerThreshold;          // minor bug. if yeeted too fast this stops reporting true even though the case is true. only happens when scrolling extremely fast and only when this needs to work.
        }
    }
}