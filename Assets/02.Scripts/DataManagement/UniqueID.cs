using UnityEngine;
using System;

public class UniqueID : MonoBehaviour
{
    public string uniqueID;

    private void Awake()
    {
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = Guid.NewGuid().ToString();
        }
    }
}