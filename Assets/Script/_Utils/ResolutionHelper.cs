using UnityEngine;

public static class ResolutionHelper
{
    // Convert Resolution to string
    public static string ResolutionToString(Resolution resolution)
    {
        return $"{resolution.width}x{resolution.height}@{resolution.refreshRateRatio.value:0}Hz";
    }

    // Convert string to Resolution
    public static bool TryParseResolution(string resolutionString, out int resolutionWidth, out int resolutionHeight)
    {
        resolutionWidth = 0;
        resolutionHeight = 0;
        try
        {
            // Split by 'x' and '@' to extract width, height
            var dimensions = resolutionString.Split('x', '@');
            if (dimensions.Length == 3 && int.TryParse(dimensions[0], out int width) &&
                int.TryParse(dimensions[1], out int height) &&
                int.TryParse(dimensions[2].Replace("Hz", ""), out int refreshRate))
            {
                resolutionWidth = width;
                resolutionHeight = height;
                return true;
            }
        }
        catch
        {
            Debug.LogError($"Failed to parse resolution from string: {resolutionString}");
        }
        return false;
    }
}
