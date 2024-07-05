using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableObject : MonoBehaviour, IClickableObject
{
    public void OnPointerClick(PointerEventData eventData)
    {
        CameraTargetHandler.Instance.SetTarget(transform);
    }
}