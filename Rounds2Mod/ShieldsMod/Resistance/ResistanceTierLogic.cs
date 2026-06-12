using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShieldsMod.Resistance;

/// <summary>Shared first-pick and stacking rules for poison / stun / lifesteal resistance cards.</summary>
internal static class ResistanceTierLogic
{
    public const float StackBonus = 0.05f;
    public const float Lv2UpgradeThreshold = 0.50f;
    public const float Lv3UpgradeThreshold = 0.75f;

    private static readonly float[] FirstReductionByLevel = { 0f, 0.25f, 0.50f, 0.75f };

    internal static float GetFirstReduction(int level) =>
        level >= 1 && level <= 3 ? FirstReductionByLevel[level] : 0f;

    internal static float ComputeFromHand(
        IEnumerable<CardInfo> cards,
        IReadOnlyDictionary<string, int> tierByCardName,
        CardInfo exclude = null)
    {
        if (tierByCardName == null || cards == null)
            return 0f;

        float reduction = 0f;
        bool established = false;

        foreach (CardInfo card in cards)
        {
            if (card == null || (exclude != null && card.name == exclude.name))
                continue;

            if (!TryGetLevel(card, tierByCardName, out int level))
                continue;

            if (!established)
            {
                reduction = GetFirstReduction(level);
                established = true;
            }
            else
            {
                reduction = ApplyStacking(reduction, level, out _);
            }
        }

        return established ? ClampReduction(reduction) : 0f;
    }

    /// <summary>Preview or apply one card onto an existing reduction value.</summary>
    internal static float ApplyCard(float currentReduction, int level, out float? healthMultiplier)
    {
        healthMultiplier = null;
        bool hadResistance = currentReduction > 0.0001f;

        if (!hadResistance)
            return ClampReduction(GetFirstReduction(level));

        float next = ApplyStacking(currentReduction, level, out healthMultiplier);
        return ClampReduction(next);
    }

    internal static void ApplyHealthBonus(CharacterData data, float multiplier)
    {
        if (data == null || multiplier <= 1f + 0.0001f)
            return;

        float ratio = data.maxHealth > 0f ? data.health / data.maxHealth : 1f;
        data.maxHealth *= multiplier;
        data.health = Mathf.Clamp(data.maxHealth * ratio, 1f, data.maxHealth);
    }

    internal static float ApplyStacking(float current, int level, out float? healthMultiplier)
    {
        healthMultiplier = null;

        switch (level)
        {
            case 1:
                return current + StackBonus;
            case 2:
                if (current < Lv2UpgradeThreshold)
                    return Lv2UpgradeThreshold;
                healthMultiplier = 1.10f;
                return current + StackBonus;
            case 3:
                if (current < Lv3UpgradeThreshold)
                    return Lv3UpgradeThreshold;
                healthMultiplier = 1.25f;
                return current + StackBonus;
            default:
                return current;
        }
    }

    private static bool TryGetLevel(
        CardInfo card,
        IReadOnlyDictionary<string, int> tierByCardName,
        out int level)
    {
        level = 0;
        if (card == null)
            return false;

        string name = card.cardName;
        if (string.IsNullOrEmpty(name) && card.sourceCard != null)
            name = card.sourceCard.cardName;

        return !string.IsNullOrEmpty(name) && tierByCardName.TryGetValue(name, out level);
    }

    internal static float ClampReduction(float value) => Mathf.Clamp01(value);
}
