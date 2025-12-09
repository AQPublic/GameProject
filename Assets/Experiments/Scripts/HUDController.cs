// Assets/Scripts/HUDController.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance { get; private set; }

    private Text _powerText;
    private Text _vanishText;
    private Text _debugBanner;

    private readonly Dictionary<string, int> _vanishCounts = new();
    private PlayerPower _power;

    // ---------- Entry ----------
    public static HUDController Create(PlayerPower power)
    {
        var go = new GameObject("HUDController");
        var hud = go.AddComponent<HUDController>();
        hud.Initialize(power);
        return hud;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (_power != null) _power.OnPowerChanged -= OnPowerChanged;
        VanishEvents.OnElementVanished -= OnElementVanished;
        if (Instance == this) Instance = null;
    }

    // ---------- Init ----------
    private void Initialize(PlayerPower power)
    {
        _power = power;

        EnsureEventSystem();

        // Canvas (overlay on top)
        var canvasGO = new GameObject("HUD_Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // Top-left panel
        var panel = new GameObject("HUD_Panel");
        panel.transform.SetParent(canvasGO.transform, false);
        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.30f);
        var prt = panel.GetComponent<RectTransform>();
        prt.anchorMin = new Vector2(0f, 1f);
        prt.anchorMax = new Vector2(0f, 1f);
        prt.pivot = new Vector2(0f, 1f);
        prt.anchoredPosition = new Vector2(12f, -12f);
        prt.sizeDelta = new Vector2(600f, 120f);

        // Built-in font for Unity 6+
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Power text
        _powerText = CreateText(panel.transform, "PowerText", new Vector2(10, -10),
                                "Power: 0 / 3", 28, FontStyle.Bold, font);

        // Vanish text
        _vanishText = CreateText(panel.transform, "VanishText", new Vector2(10, -50),
                                 "Vanished — Blue Cubes: 0 | Red Spheres: 0 | Total: 0",
                                 22, FontStyle.Normal, font);

        // Big center debug banner so you can *see* UI immediately (remove later)
        _debugBanner = CreateText(canvasGO.transform, "HUD_DEBUG_BANNER", Vector2.zero,
                                  "HUD ONLINE", 48, FontStyle.Bold, font);
        var drt = _debugBanner.rectTransform;
        drt.anchorMin = new Vector2(0.5f, 0.5f);
        drt.anchorMax = new Vector2(0.5f, 0.5f);
        drt.pivot = new Vector2(0.5f, 0.5f);
        drt.anchoredPosition = Vector2.zero;
        _debugBanner.alignment = TextAnchor.MiddleCenter;
        _debugBanner.color = new Color(1f, 1f, 1f, 0.95f);

        // Events
        if (_power != null) _power.OnPowerChanged += OnPowerChanged;
        VanishEvents.OnElementVanished += OnElementVanished;

        // Initial fill
        OnPowerChanged(_power != null ? _power.Power : 0);
        UpdateVanishText();
    }

    private void EnsureEventSystem()
    {
#if UNITY_600_OR_NEWER
        var existing = Object.FindFirstObjectByType<EventSystem>();
#else
        var existing = Object.FindObjectOfType<EventSystem>();
#endif
        if (existing != null) return;

        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    private Text CreateText(Transform parent, string name, Vector2 offset,
                            string initial, int size, FontStyle style, Font font)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var txt = go.AddComponent<Text>();
        txt.text = initial;
        txt.font = font;                    // <- critical in Unity 6
        txt.fontSize = size;
        txt.fontStyle = style;
        txt.color = new Color(1f, 1f, 1f, 1f);
        txt.alignment = TextAnchor.UpperLeft;
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;
        txt.verticalOverflow = VerticalWrapMode.Overflow;
        txt.raycastTarget = false;

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = offset;
        rt.sizeDelta = new Vector2(560f, 42f);
        return txt;
    }

    // ---------- Events ----------
    private void OnPowerChanged(int newValue)
    {
        if (_powerText == null || _power == null) return;
        _powerText.text = $"Power: {newValue} / {_power.powerNeeded}";
    }

    private void OnElementVanished(string type)
    {
        if (!_vanishCounts.ContainsKey(type)) _vanishCounts[type] = 0;
        _vanishCounts[type]++;
        UpdateVanishText();
    }

    private void UpdateVanishText()
    {
        if (_vanishText == null) return;

        int blue = _vanishCounts.TryGetValue("Blue Cube", out var b) ? b : 0;
        int red  = _vanishCounts.TryGetValue("Red Sphere", out var r) ? r : 0;
        int total = 0;
        foreach (var kv in _vanishCounts) total += kv.Value;

        _vanishText.text = $"Vanished — Blue Cubes: {blue} | Red Spheres: {red} | Total: {total}";
    }
}
