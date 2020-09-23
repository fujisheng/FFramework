using System;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Framework.Component.UI
{
    public class EventHandler : MonoBehaviour, 
        IPointerClickHandler, 
        IPointerDownHandler, 
        IPointerEnterHandler, 
        IPointerExitHandler, 
        IPointerUpHandler, 
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {

        Action<PointerEventData> onBeginDrag;
        Action<PointerEventData> onDrag;
        Action<PointerEventData> onEndDrag;
        Action<PointerEventData> onPointerClick;
        Action<PointerEventData> onPointerDown;
        Action<PointerEventData> onPointerEnter;
        Action<PointerEventData> onPointerExit;
        Action<PointerEventData> onPointerUp;

        public void AddOnBeginDragListener(Action<PointerEventData> action)
        {
            onBeginDrag += action;
        }

        public void RemoveOnBeginDragListener(Action<PointerEventData> action)
        {
            onBeginDrag -= action;
        }

        public void RemoveAllOnBeginDragListener()
        {
            onBeginDrag = null;
        }

        public void AddOnDragListener(Action<PointerEventData> action)
        {
            onDrag += action;
        }

        public void RemoveOnDragListener(Action<PointerEventData> action)
        {
            onDrag -= action;
        }

        public void RemoveAllOnDragListener()
        {
            onDrag = null;
        }

        public void AddOnEndDragListener(Action<PointerEventData> action)
        {
            onEndDrag += action;
        }

        public void RemoveOnEndDragListener(Action<PointerEventData> action)
        {
            onEndDrag -= action;
        }

        public void RemoveAllOnEndDragListener()
        {
            onEndDrag = null;
        }

        public void AddOnPointerClickListener(Action<PointerEventData> action)
        {
            onPointerClick += action;
        }

        public void RemoveOnPointerClickListener(Action<PointerEventData> action)
        {
            onPointerClick -= action;
        }

        public void RemoveAllOnPointerClickListener()
        {
            onPointerClick = null;
        }

        public void AddOnPointerDownListener(Action<PointerEventData> action)
        {
            onPointerDown += action;
        }

        public void RemoveOnPointerDownListener(Action<PointerEventData> action)
        {
            onPointerDown -= action;
        }

        public void RemoveAllOnPointerDownListener()
        {
            onPointerDown = null;
        }

        public void AddOnPointerEnterListener(Action<PointerEventData> action)
        {
            onPointerEnter += action;
        }

        public void RemoveOnPointerEnterListener(Action<PointerEventData> action)
        {
            onPointerEnter -= action;
        }

        public void RemoveAllOnPointerEnterListener()
        {
            onPointerEnter = null;
        }

        public void AddOnPointerExitListener(Action<PointerEventData> action)
        {
            onPointerExit += action;
        }

        public void RemoveOnPointerExitListener(Action<PointerEventData> action)
        {
            onPointerExit -= action;
        }

        public void RemoveAllOnPointerExitListener()
        {
            onPointerExit = null;
        }

        public void AddOnPointerUpListener(Action<PointerEventData> action)
        {
            onPointerUp += action;
        }

        public void RemoveOnPointerUpListener(Action<PointerEventData> action)
        {
            onPointerUp -= action;
        }

        public void RemoveAllOnPointerUpListener()
        {
            onPointerUp = null;
        }

        public void RemoveAllListener()
        {
            onBeginDrag = null;
            onDrag = null;
            onEndDrag = null;
            onPointerClick = null;
            onPointerDown = null;
            onPointerEnter = null;
            onPointerExit = null;
            onPointerUp = null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDrag?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDrag?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onEndDrag?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onPointerClick?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown?.Invoke(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUp?.Invoke(eventData);
        }
    }
}