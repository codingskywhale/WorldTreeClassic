using System.Collections;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    public static CameraSettings Instance { get; private set; }

    public Vector3 basePosition = new Vector3(-1.7f, 2.5f, -2.3f);
    public Quaternion baseRotation = Quaternion.Euler(-90, -116, 0);
    public Quaternion finalRotation = Quaternion.Euler(20, -116, 0);
    public Quaternion zoomInRotation = Quaternion.Euler(25, -116, 0);

    public WorldTree worldTree;

    public Vector3 currentCameraPosition;
    public Quaternion currentCameraRotation;
    public float currentCameraFOV;

    public float duration = 2.5f;
    public float zoomDuration = 1.0f;

    public float minVerticalAngle = 0f; // 최소 각도 제한
    public float maxVerticalAngle = 30f; // 최대 각도 제한
    public float minHeight = 1f; // 카메라의 최소 높이 제한
    public float maxHeight = 15f;

    public bool animationCompleted = false;
    public bool isZooming = false;

    private void Awake()
    {       
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }

        if (worldTree == null)
        {
            worldTree = FindObjectOfType<WorldTree>();
        }

        int treeLevel = DataManager.Instance.touchData.touchIncreaseLevel;
        currentCameraPosition = GetInitialPosition(treeLevel);
        currentCameraRotation = GetInitialRotation();
        currentCameraFOV = Camera.main.fieldOfView;
    }

    private void Start()
    {
        
    }
    //private void AdjustOffsetBasedOnTreeLevel(int treeLevel)
    //{
    //    int levelFactor = treeLevel / 10;

    //    if (levelFactor > 0)
    //    {
    //        basePosition -= Camera.main.transform.forward * (worldTree.positionIncrement * levelFactor);
    //    }
    //}

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

    public void UpdateCameraState(float newFOV, Vector3 newPosition, Quaternion newRotation)
    {
        currentCameraFOV = newFOV;
        currentCameraPosition = newPosition;
        currentCameraRotation = newRotation;

        Camera.main.fieldOfView = currentCameraFOV;
        Camera.main.transform.position = currentCameraPosition;
        Camera.main.transform.rotation = currentCameraRotation;
    }        
}