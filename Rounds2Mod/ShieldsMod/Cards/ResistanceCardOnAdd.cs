using System;
using System.Collections.Generic;
using System.Linq;
using ShieldsMod.Resistance;

namespace ShieldsMod.Cards;

internal static class ResistanceCardOnAdd
{
    internal static void Apply(
        Player player,
        CharacterData data,
        int level,
        CardInfo addingCard,
        Func<IEnumerable<CardInfo>, float> computeFromHand,
        Action<int> rebuild)
    {
        if (player == null || computeFromHand == null)
            return;

        IEnumerable<CardInfo> survivors = player.data?.currentCards?
            .Where(c => c != null && !ReferenceEquals(c, addingCard)) ?? Enumerable.Empty<CardInfo>();

        float before = computeFromHand(survivors);
        ResistanceTierLogic.ApplyCard(before, level, out float? healthMult);

        if (healthMult.HasValue)
            ResistanceTierLogic.ApplyHealthBonus(data, healthMult.Value);

        rebuild?.Invoke(player.playerID);
    }

    internal static void ApplyForType(
        Player player,
        CharacterData data,
        ResistanceType type,
        CardInfo addingCard)
    {
        if (player == null || addingCard == null)
            return;

        var survivors = player.data?.currentCards?
            .Where(c => c != null && !ReferenceEquals(c, addingCard)) ?? Enumerable.Empty<CardInfo>();

        int tier = ResistanceHandLogic.GetNextUpgradeTier(survivors, type);
        Apply(
            player,
            data,
            tier,
            addingCard,
            ResistanceCardCatalog.GetComputeFromHand(type),
            ResistanceCardCatalog.GetRebuildAction(type));
    }
}
