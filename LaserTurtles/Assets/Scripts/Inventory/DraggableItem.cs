using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform _originalParent;
    private Transform _canvasParent;
    private Image _img;

    public Transform OriginalParent { get => _originalParent; set => _originalParent = value; }
    public Transform CanvasParent { get => _canvasParent; set => _canvasParent = value; }

    private void Awake()
    {
        _img = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");
        _originalParent = transform.parent;
        //transform.SetParent(transform.parent.parent.parent.parent.parent);
        transform.SetParent(_canvasParent);
        _img.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");
        transform.SetParent(_originalParent);
        _img.raycastTarget = true;
    }
}
