using UnityEngine;

public class AlphaChanger : MonoBehaviour
{
    public void SetAlpha(GameObject target, float alpha)
    {
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = alpha;
    }
}
