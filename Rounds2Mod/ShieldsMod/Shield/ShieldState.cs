using UnityEngine;

namespace ShieldsMod.Shield;

/// <summary>Per-player persistent shield pool (separate from block/parry).</summary>
public class ShieldState
{
    public const float DefaultMax = 100f;

    public float Max;
    public float Current;

    public bool HasShield => Max > 0f;

    public float Percent => Max > 0f ? Current / Max : 0f;

    public bool IsActive => Current > 0f;

    public void ResetToFull()
    {
        Current = Max;
    }

    public const float BubbleAlpha = 0.05f;

    /// <summary>Green / yellow / red thresholds from plan (65% and 35%).</summary>
    public Color GetBubbleColor(float alpha = BubbleAlpha)
    {
        float pct = Percent;
        Color rgb;
        if (pct > 0.65f)
            rgb = new Color(0.2f, 0.9f, 0.25f);
        else if (pct > 0.35f)
            rgb = new Color(0.95f, 0.85f, 0.15f);
        else
            rgb = new Color(0.95f, 0.2f, 0.15f);

        return new Color(rgb.r, rgb.g, rgb.b, alpha);
    }
}
