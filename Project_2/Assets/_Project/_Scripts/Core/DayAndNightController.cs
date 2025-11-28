using UnityEngine;
using UnityEngine.Rendering.Universal; // can cho Light2D

public class DayAndNightController : MonoBehaviour
{
    [Header("Anh sang toan canh (Global Light 2D)")]
    public Light2D globalLight; // anh sang chinh cua scene

    [Header("Den duong (Point Light 2D)")]
    public Light2D[] streetLights;
    public float streetLightMaxIntensity = 1.5f;

    [Header("Thoi gian")]
    [Range(0, 24)] public float timeOfDay = 12f;
    public float dayDuration = 300f; // 1 ngay = 200 giay

    [Header("Thoi tiet")]
    public GameObject rainPrefab = null; // prefab mưa

    void Update()
    {
        // cap nhat thoi gian
        UpdateTimeOfDay();
    }

    void UpdateTimeOfDay()
    {
        timeOfDay = (timeOfDay + (24f / dayDuration) * Time.deltaTime) % 24f;

        // dieu khien anh sang toan canh (ngay sang, dem toi)
        globalLight.intensity = CalculateGlobalLightIntensity(timeOfDay);

        // dieu khien den duong (bat/tat dan)
        float streetIntensity = CalculateStreetLightIntensity(timeOfDay);
        foreach (var light in streetLights)
        {
            light.intensity = streetIntensity;
        }

        // dieu khien mua (bat tu 18h -> 24h)
        if (timeOfDay >= 18f && timeOfDay < 24f && rainPrefab != null)
        {
            rainPrefab.SetActive(true);
        }
        else
        {
            rainPrefab.SetActive(false);
        }
    }

    private float CalculateGlobalLightIntensity(float t)
    {
        // ban ngay (8h -> 18h): sang nhat
        if (t >= 8f && t <= 18f) return 1f;
        // hoang hon (18h -> 20h): giam sang
        if (t > 18f && t <= 20f) return Mathf.Lerp(1f, 0.2f, (t - 18f) / 2f);
        // nua dem (20h -> 6h): toi nhat
        if (t > 20f || t < 6f) return 0.2f;
        // binh minh (6h -> 8h): tang sang
        if (t >= 6f && t < 8f) return Mathf.Lerp(0.2f, 1f, (t - 6f) / 2f);

        return 1f;
    }

    private float CalculateStreetLightIntensity(float t)
    {
        // fade in từ 17h30 → 18h
        if (t >= 17.5f && t < 18f)
            return Mathf.Lerp(0f, streetLightMaxIntensity, (t - 17.5f) / 0.5f);

        // sáng tối đa từ 18h → 6h
        if (t >= 18f || t < 6f)
            return streetLightMaxIntensity;

        // fade out từ 6h → 6h30
        if (t >= 6f && t < 6.5f)
            return Mathf.Lerp(streetLightMaxIntensity, 0f, (t - 6f) / 0.5f);

        // còn lại ban ngày: tắt
        return 0f;
    }

}