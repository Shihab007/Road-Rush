using UnityEngine;
using UnityEngine.EventSystems;

// Attach this to both the LeftZone and RightZone UI panels
// Set Direction to -1 for Left, +1 for Right in the Inspector
public class TapZone : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Direction: -1 = Left, +1 = Right")]
    public float direction = -1f;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.isPlaying)
            PlayerCar.Instance.SetInput(direction);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PlayerCar.Instance.SetInput(0f);
    }
}