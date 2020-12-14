using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SekiInfiniteScrollDragCallBack : MonoBehaviour, IDragHandler,IBeginDragHandler,IEndDragHandler
{
    public ScrollRect scroll = null;

    public void OnBeginDrag(PointerEventData eventData)
    {
        scroll.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        scroll.OnDrag(eventData);
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        scroll.OnEndDrag(eventData);
    }
}
