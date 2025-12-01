using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DayAndNightController : MonoBehaviour
{
    [Header("Ánh sáng toàn cảnh")]
    public Light2D globalLight;

    [Header("Đèn đường")]
    public Light2D[] streetLights;
    public float streetLightMaxIntensity = 1.5f;

    [Header("Thời gian")]
    [Range(0, 24)] public float timeOfDay = 12f;
    public float dayDuration = 300f;

    [Header("Thời tiết")]
    public GameObject rainPrefab;
    public ParticleSystem rainDrops;
    public ParticleSystem rainRipples;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindStreetLights();
    }

    // Tìm tất cả đèn có tên WayLight
    void FindStreetLights()
    {
        Light2D[] allLights = FindObjectsByType<Light2D>(FindObjectsSortMode.None);
        var list = new System.Collections.Generic.List<Light2D>();

        foreach (var light in allLights)
        {
            if (light.gameObject.name == "WayLight")
                list.Add(light);
        }

        streetLights = list.ToArray();

        Debug.Log("Found " + streetLights.Length + " WayLight objects.");
    }

    void Update()
    {
        UpdateTimeOfDay();
    }

    void UpdateTimeOfDay()
    {
        timeOfDay = (timeOfDay + (24f / dayDuration) * Time.deltaTime) % 24f;

        globalLight.intensity = CalculateGlobalLightIntensity(timeOfDay);

        float streetIntensity = CalculateStreetLightIntensity(timeOfDay);
        foreach (var light in streetLights)
        {
            if (light != null)
                light.intensity = streetIntensity;
        }

        HandleRain();
    }

    void HandleRain()
    {
        if (timeOfDay >= 18f && timeOfDay < 24f && rainPrefab != null)
        {
            rainPrefab.SetActive(true);

            if (!rainDrops.isPlaying) rainDrops.Play();
            if (!rainRipples.isPlaying) rainRipples.Play();
        }
        else
        {
            if (rainDrops.isPlaying) rainDrops.Stop();
            if (rainRipples.isPlaying) rainRipples.Stop();

            rainPrefab.SetActive(false);
        }
    }

    private float CalculateGlobalLightIntensity(float t)
    {
        if (t >= 8f && t <= 18f) return 1f;
        if (t > 18f && t <= 20f) return Mathf.Lerp(1f, 0.2f, (t - 18f) / 2f);
        if (t > 20f || t < 6f) return 0.2f;
        if (t >= 6f && t < 8f) return Mathf.Lerp(0.2f, 1f, (t - 6f) / 2f);

        return 1f;
    }

    private float CalculateStreetLightIntensity(float t)
    {
        if (t >= 17.5f && t < 18f)
            return Mathf.Lerp(0f, streetLightMaxIntensity, (t - 17.5f) / 0.5f);

        if (t >= 18f || t < 6f)
            return streetLightMaxIntensity;

        if (t >= 6f && t < 6.5f)
            return Mathf.Lerp(streetLightMaxIntensity, 0f, (t - 6f) / 0.5f);

        return 0f;
    }
}
