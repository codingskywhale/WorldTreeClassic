using System;

public class OfflineProgressCalculator
{
    public TimeSpan CalculateOfflineDuration(string lastSaveTime)
    {
        if (!string.IsNullOrEmpty(lastSaveTime))
        {
            DateTime lastSave = DateTime.Parse(lastSaveTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
            DateTime currentTime = DateTime.UtcNow;
            return currentTime - lastSave;
        }
        return TimeSpan.Zero;
    }
}

