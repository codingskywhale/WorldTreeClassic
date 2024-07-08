using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableObject : MonoBehaviour, IClickableObject
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (CameraTargetHandler.Instance.IsFreeCamera())
        {
            CameraTargetHandler.Instance.SetTarget(transform);
        }
    }
}