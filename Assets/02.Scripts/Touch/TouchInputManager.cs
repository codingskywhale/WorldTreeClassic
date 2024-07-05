using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputManager : MonoBehaviour
{
    public LifeManager lifeManager;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isPointerOverUIElement())
        {
            lifeManager.IncreaseWater(LifeManager.Instance.touchData.touchIncreaseAmount);
        }
    }

    private bool isPointerOverUIElement()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
