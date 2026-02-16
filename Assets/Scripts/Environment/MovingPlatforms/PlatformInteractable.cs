using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlatformInteractable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static PlatformInteractable currentInteractable;
    public UnityEvent stopInteractEvent;


    public void OnPointerDown(PointerEventData eventData)
    {
        var clickObject = eventData.pointerCurrentRaycast.gameObject;
        if (clickObject == gameObject) currentInteractable = this;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var clickObject = eventData.pointerCurrentRaycast.gameObject.name;
        if (currentInteractable != this) return;

        currentInteractable = null;
        stopInteractEvent.Invoke();
    }
}
