using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace AtomicApps.Infrastructure.Loader
{
    public class LoadingJumpWave_Stable : MonoBehaviour
{
    [SerializeField] private List<LetterCellForLoadingAnimation> letters = new();

    [Header("Motion")]
    [SerializeField] private float jumpHeight = 18f;
    [SerializeField] private float upDuration = 0.22f;
    [SerializeField] private float downDuration = 0.26f;
    [SerializeField] private float holdAfterLanding = 0.06f;
    [SerializeField] private float fadeBackDuration = 0.14f;
    [SerializeField] private float perLetterDelay = 0.10f;
    [SerializeField] private float pauseBetweenCycles = 0.30f;

    [Header("Colors")]
    [SerializeField] private Color activeColor = new(0.10f, 0.85f, 0.35f);
    [SerializeField] private Color inactiveColor = Color.black;

    [Header("Timing")]
    [SerializeField] private bool useUnscaledTime = true;
    [SerializeField] private bool autoSortLeftToRight = true;

    private readonly List<float> _baseY = new();
    private float _t; // global time (seconds)
    private float _perLetterCycle;
    private float _globalCycle;
    private bool _primed; // prevents any animation on the very first frame
    private List<Vector2> _positions = new();

    private void Awake()
    {
        foreach (var letter in letters)
        {
            _positions.Add(letter.Container.anchoredPosition);
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < _positions.Count; i++)
        {
            letters[i].Container.anchoredPosition = _positions[i];
        }

        if (autoSortLeftToRight)
        {
            letters.Sort((a, b) =>
                a && a.Container && b && b.Container
                    ? a.Container.anchoredPosition.x.CompareTo(b.Container.anchoredPosition.x)
                    : 0);
        }

        CacheBase();
        PrimeAtRest();        // put everything at ground/black
        PrecomputeDurations();
        _t = 0f;
        _primed = false;      // next Update will prime once more and exit
    }

    private void Update()
    {
        // One-frame prime to avoid any initial evaluation/flicker
        if (!_primed)
        {
            PrimeAtRest();
            _t = 0f;         // ensure we truly start at the first letter
            _primed = true;
            return;          // nothing animates on this very first Update
        }

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        _t += dt;
        if (letters == null || letters.Count == 0) return;

        // Cycle-local time in [0, _globalCycle)
        float cycleTime = Mathf.Repeat(_t, Mathf.Max(_globalCycle, 0.0001f));

        for (int i = 0; i < letters.Count; i++)
        {
            var cell = letters[i];
            if (!cell || !cell.Container) continue;

            float y0 = _baseY[i];

            // This letter’s local time within the cycle
            float local = cycleTime - i * perLetterDelay;

            // Not started yet this cycle → idle
            if (local < 0f)
            {
                SetY(cell, y0);
                SetColor(cell, inactiveColor);
                continue;
            }

            // After its active window → idle
            if (local >= _perLetterCycle)
            {
                SetY(cell, y0);
                SetColor(cell, inactiveColor);
                continue;
            }

            // Phases
            float t;

            // 1) Ascent: black -> green
            if (local < upDuration)
            {
                t = local / upDuration; // 0..1
                SetY(cell, y0 + EaseOutQuad(t) * jumpHeight);
                SetColor(cell, Color.Lerp(inactiveColor, activeColor, t));
                continue;
            }
            local -= upDuration;

            // 2) Descent: stay green
            if (local < downDuration)
            {
                t = local / downDuration;
                SetY(cell, y0 + (1f - EaseInQuad(t)) * jumpHeight);
                SetColor(cell, activeColor);
                continue;
            }
            local -= downDuration;

            // 3) Hold on ground: still green
            if (local < holdAfterLanding)
            {
                SetY(cell, y0);
                SetColor(cell, activeColor);
                continue;
            }
            local -= holdAfterLanding;

            // 4) Fade back to black on ground
            t = Mathf.Clamp01(local / fadeBackDuration);
            SetY(cell, y0);
            SetColor(cell, Color.Lerp(activeColor, inactiveColor, t));
        }
    }

    // --- helpers ---

    private void CacheBase()
    {
        _baseY.Clear();
        if (letters == null) return;
        foreach (var c in letters)
            _baseY.Add(c && c.Container ? c.Container.anchoredPosition.y : 0f);
    }

    private void PrimeAtRest()
    {
        if (letters == null) return;
        for (int i = 0; i < letters.Count; i++)
        {
            var c = letters[i];
            if (!c) continue;

            if (c.Container)
            {
                var p = c.Container.anchoredPosition;
                p.y = _baseY.Count > i ? _baseY[i] : 0f;
                c.Container.anchoredPosition = p;
            }
            if (c.Label) c.Label.color = inactiveColor;
        }
    }

    private void PrecomputeDurations()
    {
        _perLetterCycle = upDuration + downDuration + holdAfterLanding + fadeBackDuration;
        float span = Mathf.Max(letters.Count - 1, 0) * perLetterDelay;
        _globalCycle = span + _perLetterCycle + pauseBetweenCycles;
    }

    private static float EaseOutQuad(float x) => 1f - (1f - x) * (1f - x);
    private static float EaseInQuad(float x)  => x * x;

    private static void SetY(LetterCellForLoadingAnimation cell, float y)
    {
        var pos = cell.Container.anchoredPosition;
        pos.y = y;
        cell.Container.anchoredPosition = pos;
    }

    private static void SetColor(LetterCellForLoadingAnimation cell, Color c)
    {
        if (cell.Label) cell.Label.color = c;
    }
}
}

