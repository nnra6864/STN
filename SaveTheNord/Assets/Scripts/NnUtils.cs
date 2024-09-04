using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;

public static class NnUtils
{
    public static float EaseIn(float t) => 1 - Mathf.Cos((t * Mathf.PI) / 2f);
    public static float EaseOut(float t) => Mathf.Sin((t * Mathf.PI) / 2f);
    public static float EaseInOut(float t) => -(Mathf.Cos(Mathf.PI * t) - 1f) / 2f;
    public static float EaseInOutQuad(float t) => t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
    public static float EaseInOutCubic(float t) => t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    public static float EaseInBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return c3 * t * t * t - c1 * t * t;
    }

    private static int _uiLayer;
    public static void UpdateLayers()
    {
        _uiLayer = LayerMask.NameToLayer("UI");
    }
    [Tooltip("Must call UpdateLayers() in the Awake of the function you are using it in order to update the UI Layer")]
    public static bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
    private static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaycastResults)
    {
        foreach (var curRaycastResult in eventSystemRaycastResults)
        {
            if (curRaycastResult.gameObject.layer == _uiLayer)
                return true;
        }
        return false;
    }
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults;
    }
    
    public static Color32 HexToRgba(string hex, Color32 currentColor)
    {
        if (hex.Length < 1) return currentColor;
        int i = hex[0] == '#' ? 1 : 0;
        int r = currentColor.r, g = currentColor.g, b = currentColor.b, a = currentColor.a;
        if (hex.Length >= 2 + i) if (!int.TryParse($"{hex[0 + i]}{hex[1 + i]}", NumberStyles.HexNumber, CultureInfo.InvariantCulture, out r)) {}
        if (hex.Length >= 4 + i) if (!int.TryParse($"{hex[2 + i]}{hex[3 + i]}", NumberStyles.HexNumber, CultureInfo.InvariantCulture, out g)) {}
        if (hex.Length >= 6 + i) if (!int.TryParse($"{hex[4 + i]}{hex[5 + i]}", NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b)) {}
        if (hex.Length >= 8 + i) if (!int.TryParse($"{hex[6 + i]}{hex[7 + i]}", NumberStyles.HexNumber, CultureInfo.InvariantCulture, out a)) {}

        return new Color32(byte.Parse(r.ToString()), byte.Parse(g.ToString()), byte.Parse(b.ToString()), byte.Parse(a.ToString()));
    }
}