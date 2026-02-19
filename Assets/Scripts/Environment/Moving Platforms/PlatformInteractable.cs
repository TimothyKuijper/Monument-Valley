using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider))]
public class PlatformInteractable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static PlatformInteractable currentInteractable;
    public UnityEvent startInteractEvent;
    public UnityEvent stopInteractEvent;

    public bool canDrag = true;

    const string StandardLayer = "Grab Interactable";


    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer(StandardLayer);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canDrag == false) return;

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

    public void SetWalkOn(bool walkOn)
    {
        canDrag = !walkOn;
        if (walkOn) OnPointerUp(null);
    }
}
