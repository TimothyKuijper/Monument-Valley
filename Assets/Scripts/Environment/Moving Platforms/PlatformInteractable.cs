using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider))]
public class PlatformInteractable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static PlatformInteractable currentInteractable;
    public UnityEvent startInteractEvent;
    public UnityEvent stopInteractEvent;

    const string StandardLayer = "Grab Interactable";


    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer(StandardLayer);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var clickObject = eventData.pointerCurrentRaycast.gameObject;
        if (clickObject != gameObject) return;

        currentInteractable = this;
        startInteractEvent.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (currentInteractable != this) return;

        currentInteractable = null;
        stopInteractEvent.Invoke();
    }
}
