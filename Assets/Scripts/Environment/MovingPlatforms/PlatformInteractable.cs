using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider))]
public class PlatformInteractable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static PlatformInteractable currentInteractable;
    public UnityEvent startInteractEvent;
    public UnityEvent stopInteractEvent;

    protected Camera _camera;

    public bool canDrag = true;

    private const string StandardLayer = "Grab Interactable";
    private const float DebugGizmoSize = .5f;


    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer(StandardLayer);
        _camera = FindAnyObjectByType<Camera>();
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



    protected void OnDrawGizmos()
    {
        Gizmos.color = canDrag ? Color.yellow : Color.red;
        Gizmos.DrawSphere(transform.position, DebugGizmoSize);
    }
}
