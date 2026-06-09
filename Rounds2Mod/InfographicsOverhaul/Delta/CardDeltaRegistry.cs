using System;

using System.Collections.Generic;

using System.Linq;



namespace InfoOverhaul.Delta;



/// <summary>

/// Opt-in registry for card stat-delta previews.

/// Tier 1: <see cref="Register"/> stat providers (auto or manual).

/// Tier 2: <see cref="RegisterAddon"/> merges extra lines (e.g. custom effect notes).

/// </summary>

public static class CardDeltaRegistry

{

    public delegate IReadOnlyList<StatDeltaLine> DeltaProvider(Player player, CardInfo card);



    /// <summary>Simpler provider shape for cross-mod registration via reflection.</summary>

    public delegate IReadOnlyList<(string label, string before, string after)> SimpleDeltaProvider(

        Player player, CardInfo card);



    private static readonly Dictionary<string, DeltaProvider> Providers =

        new Dictionary<string, DeltaProvider>(StringComparer.Ordinal);



    private static readonly Dictionary<string, List<DeltaProvider>> Addons =

        new Dictionary<string, List<DeltaProvider>>(StringComparer.Ordinal);



    private static readonly Dictionary<string, DeltaProvider> RemovalProviders =

        new Dictionary<string, DeltaProvider>(StringComparer.Ordinal);



    public static void RegisterSimple(string cardName, SimpleDeltaProvider provider) =>

        Register(cardName, (player, card) => ToLines(provider(player, card)));



    public static void Register(string cardName, DeltaProvider provider)

    {

        if (string.IsNullOrEmpty(cardName) || provider == null)

            return;



        Providers[cardName] = provider;

        IOLog.Line($"CardDeltaRegistry — registered '{cardName}'.");

    }



    /// <summary>

    /// Adds extra preview lines merged after the primary provider (Tier 2 compat).

    /// Safe to call even when the card already has an auto-registered stat provider.

    /// </summary>

    public static void RegisterAddon(string cardName, DeltaProvider addon)

    {

        if (string.IsNullOrEmpty(cardName) || addon == null)

            return;



        if (!Addons.TryGetValue(cardName, out List<DeltaProvider> list))

        {

            list = new List<DeltaProvider>();

            Addons[cardName] = list;

        }



        list.Add(addon);

        IOLog.Line($"CardDeltaRegistry — addon registered for '{cardName}'.");

    }



    public static void RegisterAddonSimple(string cardName, SimpleDeltaProvider addon) =>

        RegisterAddon(cardName, (player, card) => ToLines(addon(player, card)));



    public static void RegisterRemovalSimple(string cardName, SimpleDeltaProvider provider) =>

        RegisterRemoval(cardName, (player, card) => ToLines(provider(player, card)));



    public static void RegisterRemoval(string cardName, DeltaProvider provider)

    {

        if (string.IsNullOrEmpty(cardName) || provider == null)

            return;



        RemovalProviders[cardName] = provider;

        IOLog.Line($"CardDeltaRegistry — removal registered '{cardName}'.");

    }



    public static bool IsRegistered(string cardName) =>

        !string.IsNullOrEmpty(cardName) &&

        (Providers.ContainsKey(cardName) || (Addons.TryGetValue(cardName, out var list) && list.Count > 0));



    public static bool TryGetDeltas(Player player, CardInfo card, out IReadOnlyList<StatDeltaLine> deltas)

    {

        deltas = null;

        if (player == null || card == null)

            return false;



        string key = ResolveCardName(card);

        if (string.IsNullOrEmpty(key))

            return false;



        bool hasPrimary = Providers.TryGetValue(key, out DeltaProvider primary);

        bool hasAddons = Addons.TryGetValue(key, out List<DeltaProvider> addonList) && addonList.Count > 0;



        if (!hasPrimary && !hasAddons)

            return false;



        var merged = new List<StatDeltaLine>();

        if (hasPrimary)

            merged.AddRange(primary(player, card) ?? Array.Empty<StatDeltaLine>());



        if (hasAddons)

        {

            foreach (DeltaProvider addon in addonList)

                merged.AddRange(addon(player, card) ?? Array.Empty<StatDeltaLine>());

        }



        deltas = merged;

        return true;

    }



    /// <summary>

    /// Stat changes if <paramref name="card"/> is removed from the player's hand.

    /// </summary>

    public static bool TryGetRemovalDeltas(Player player, CardInfo card, out IReadOnlyList<StatDeltaLine> deltas)

    {

        deltas = null;

        if (player == null || card == null)

            return false;



        string key = ResolveCardName(card);

        if (string.IsNullOrEmpty(key))

            return false;



        if (RemovalProviders.TryGetValue(key, out DeltaProvider removal))

        {

            deltas = removal(player, card) ?? Array.Empty<StatDeltaLine>();

            return true;

        }



        var templateRemoval = StatTemplateDeltaProvider.ComputeRemoval(player, card);

        if (templateRemoval.Count > 0)

        {

            deltas = templateRemoval;

            return true;

        }



        if (!TryGetDeltas(player, card, out IReadOnlyList<StatDeltaLine> addDeltas) || addDeltas.Count == 0)

            return false;



        deltas = addDeltas.Select(line => line.Inverted).ToList();

        return true;

    }



    internal static string ResolveCardName(CardInfo card)

    {

        if (!string.IsNullOrEmpty(card.cardName))

            return card.cardName;

        return card.sourceCard != null ? card.sourceCard.cardName : null;

    }



    private static IReadOnlyList<StatDeltaLine> ToLines(IReadOnlyList<(string label, string before, string after)> tuples)

    {

        if (tuples == null || tuples.Count == 0)

            return Array.Empty<StatDeltaLine>();



        return tuples.Select(t =>

        {

            if (string.IsNullOrEmpty(t.before) && string.IsNullOrEmpty(t.after))

                return StatDeltaLine.Note(t.label);

            return new StatDeltaLine(t.label, t.before, t.after);

        }).ToList();

    }

}


