using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragElement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Transform element;
    Vector2 mouseInitial = Vector2.zero;
    Vector2 elementInitial = Vector2.zero;
    bool dragging = false;

    private void Update()
    {
        if(dragging)
        {
            Vector2 difference = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - mouseInitial;
            element.position = elementInitial + difference;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        elementInitial = element.position;
        mouseInitial = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        dragging = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }
}
