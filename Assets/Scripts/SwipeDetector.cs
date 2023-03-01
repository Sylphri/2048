using UnityEngine;
using UnityEngine.EventSystems;

public enum SwipeDirection
{
    Left,
    Right,
    Up,
    Down
}

public class SwipeDetector : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public delegate void OnSwipe(SwipeDirection direction);
    public event OnSwipe Swiped;

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData data)
    {
        Vector2 swipeDir = data.position - data.pressPosition;
        if (Mathf.Abs(swipeDir.x) > Mathf.Abs(swipeDir.y))
        {
            Swiped(swipeDir.x > 0 ? SwipeDirection.Right : SwipeDirection.Left);
        }
        else
        {
            Swiped(swipeDir.y > 0 ? SwipeDirection.Up : SwipeDirection.Down);
        }
    }
}
