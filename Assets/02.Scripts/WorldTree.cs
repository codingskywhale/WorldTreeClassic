using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldTree : MonoBehaviour
{
    public MeshFilter outsideTreeMeshFilter; // 3D 오브젝트
    public TextMeshProUGUI upgradeRequirementText;
    public MeshFilter groundMeshFilter; // 3D 오브젝트
    public Mesh[] treeMeshes; // 3D 모델 배열

    public void UpdateTreeMeshes(int currentLevel)
    {
        int currentIndex = (currentLevel / 5) % treeMeshes.Length;
        outsideTreeMeshFilter.sharedMesh = treeMeshes[currentIndex];
    }
}
