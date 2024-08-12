using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class UniqueID : MonoBehaviour, IClickableObject
{
    public string uniqueID;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CameraTargetHandler.Instance.IsFreeCamera())
        {
            return;
        }
        CameraTargetHandler.Instance.SetTarget(transform);
    }

    private void Awake()
    {
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = Guid.NewGuid().ToString();
        }
    }
}
