using System;
using UnityEngine;

public class OfflineProgressCalculator
{
    public TimeSpan CalculateOfflineDuration(string lastSaveTime)
    {
        if (!string.IsNullOrEmpty(lastSaveTime))
        {
            DateTime lastSave = DateTime.Parse(lastSaveTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
            DateTime currentTime = DateTime.UtcNow;
            TimeSpan offlineDuration = currentTime - lastSave;
            Debug.Log($"오프라인 기간: {offlineDuration.TotalSeconds}초");
            return offlineDuration;
        }
        return TimeSpan.Zero;
    }
}

