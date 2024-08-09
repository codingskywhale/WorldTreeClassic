using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    public Vector3 basePosition = new Vector3(-1.7f, 2.5f, -2.3f);
    public Quaternion baseRotation = Quaternion.Euler(-90, -116, 0);
    public Quaternion finalRotation = Quaternion.Euler(20, -116, 0);
    public Quaternion zoomInRotation = Quaternion.Euler(25, -116, 0);

    public WorldTree worldTree;

    private void Awake()
    {
        if (worldTree == null)
        {
            worldTree = FindObjectOfType<WorldTree>();
        }
    }

    public Vector3 GetInitialPosition(int treeLevel)
    {
        int levelFactor = treeLevel / 10;
        Vector3 adjustedPosition = basePosition;

        if (levelFactor > 0)
        {
            adjustedPosition -= Camera.main.transform.forward * (worldTree.positionIncrement * levelFactor);
        }

        return adjustedPosition;
    }

    public Quaternion GetInitialRotation()
    {
        return baseRotation;
    }

    public Quaternion GetFinalRotation()
    {
        return finalRotation;
    }

    public Quaternion GetZoomInRotation()
    {
        return zoomInRotation;
    }
}