using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeckBuilder.Data
{
    [Serializable]
    public class CardEntry
    {
        public string cardObjectName;
        public int count;
    }

    [Serializable]
    public class DeckData
    {
        public string name;
        public int maxSize = 50;
        public List<CardEntry> cards = new List<CardEntry>();

        public int TotalCount => cards.Sum(e => e.count);
    }

    [Serializable]
    public class DeckCollection
    {
        public List<DeckData> decks = new List<DeckData>();
        public string activeDeckName;
    }

    // JsonUtility cannot serialize List<T> — use arrays for disk I/O only.
    // These must be public for JsonUtility to serialize properly.
    [Serializable]
    public class CardEntrySave
    {
        public string cardObjectName;
        public int count;
    }

    [Serializable]
    public class DeckDataSave
    {
        public string name;
        public int maxSize;
        public CardEntrySave[] cards;
    }

    [Serializable]
    public class DeckCollectionSave
    {
        public DeckDataSave[] decks;
        public string activeDeckName;
    }

    /// <summary>
    /// Manual JSON serialization because Unity's JsonUtility fails on nested arrays of custom objects.
    /// </summary>
    public static class DeckSaveMapper
    {
        public static string ToJson(DeckCollection collection)
        {
            if (collection == null)
                return "{}";

            var sb = new System.Text.StringBuilder();
            sb.Append("{\n");

            // activeDeckName
            sb.Append($"  \"activeDeckName\": \"{EscapeJson(collection.activeDeckName ?? "")}\",\n");

            // decks array
            sb.Append("  \"decks\": [\n");
            for (int i = 0; i < collection.decks.Count; i++)
            {
                var deck = collection.decks[i];
                sb.Append("    {\n");
                sb.Append($"      \"name\": \"{EscapeJson(deck.name)}\",\n");
                sb.Append($"      \"maxSize\": {deck.maxSize},\n");
                sb.Append("      \"cards\": [\n");

                var validCards = deck.cards.Where(c => c != null && c.count > 0).ToList();
                for (int j = 0; j < validCards.Count; j++)
                {
                    var card = validCards[j];
                    sb.Append("        {\n");
                    sb.Append($"          \"cardObjectName\": \"{EscapeJson(card.cardObjectName)}\",\n");
                    sb.Append($"          \"count\": {card.count}\n");
                    sb.Append("        }");
                    if (j < validCards.Count - 1) sb.Append(",");
                    sb.Append("\n");
                }

                sb.Append("      ]\n");
                sb.Append("    }");
                if (i < collection.decks.Count - 1) sb.Append(",");
                sb.Append("\n");
            }
            sb.Append("  ]\n");
            sb.Append("}");

            return sb.ToString();
        }

        public static DeckCollection FromJson(string json)
        {
            var collection = new DeckCollection();
            if (string.IsNullOrEmpty(json))
                return collection;

            try
            {
                // Simple manual JSON parsing
                // Find activeDeckName
                collection.activeDeckName = ExtractStringValue(json, "activeDeckName");

                // Find decks array
                int decksStart = json.IndexOf("\"decks\"");
                if (decksStart < 0)
                {
                    RTGLog.Warn("FromJson: no 'decks' key found.");
                    return collection;
                }

                int arrayStart = json.IndexOf('[', decksStart);
                if (arrayStart < 0)
                {
                    RTGLog.Warn("FromJson: no '[' after 'decks'.");
                    return collection;
                }

                int arrayEnd = FindMatchingBracket(json, arrayStart, '[', ']');
                if (arrayEnd < 0)
                {
                    RTGLog.Warn("FromJson: no matching ']' for decks array.");
                    return collection;
                }

                string decksArrayContent = json.Substring(arrayStart + 1, arrayEnd - arrayStart - 1);

                // Parse each deck object
                int pos = 0;
                while (pos < decksArrayContent.Length)
                {
                    int objStart = decksArrayContent.IndexOf('{', pos);
                    if (objStart < 0) break;

                    int objEnd = FindMatchingBracket(decksArrayContent, objStart, '{', '}');
                    if (objEnd < 0) break;

                    string deckJson = decksArrayContent.Substring(objStart, objEnd - objStart + 1);
                    var deck = ParseDeck(deckJson);
                    if (deck != null && !string.IsNullOrEmpty(deck.name))
                    {
                        collection.decks.Add(deck);
                    }

                    pos = objEnd + 1;
                }

                RTGLog.Line($"FromJson: parsed {collection.decks.Count} deck(s).");
            }
            catch (Exception ex)
            {
                RTGLog.Error($"FromJson exception: {ex.Message}");
            }

            return collection;
        }

        private static DeckData ParseDeck(string deckJson)
        {
            var deck = new DeckData
            {
                name = ExtractStringValue(deckJson, "name"),
                maxSize = ExtractIntValue(deckJson, "maxSize", 50),
                cards = new List<CardEntry>()
            };

            // Find cards array
            int cardsStart = deckJson.IndexOf("\"cards\"");
            if (cardsStart < 0) return deck;

            int arrayStart = deckJson.IndexOf('[', cardsStart);
            if (arrayStart < 0) return deck;

            int arrayEnd = FindMatchingBracket(deckJson, arrayStart, '[', ']');
            if (arrayEnd < 0) return deck;

            string cardsArrayContent = deckJson.Substring(arrayStart + 1, arrayEnd - arrayStart - 1);

            // Parse each card entry
            int pos = 0;
            while (pos < cardsArrayContent.Length)
            {
                int objStart = cardsArrayContent.IndexOf('{', pos);
                if (objStart < 0) break;

                int objEnd = FindMatchingBracket(cardsArrayContent, objStart, '{', '}');
                if (objEnd < 0) break;

                string cardJson = cardsArrayContent.Substring(objStart, objEnd - objStart + 1);
                string cardName = ExtractStringValue(cardJson, "cardObjectName");
                int count = ExtractIntValue(cardJson, "count", 0);

                if (!string.IsNullOrEmpty(cardName) && count > 0)
                {
                    deck.cards.Add(new CardEntry { cardObjectName = cardName, count = count });
                }

                pos = objEnd + 1;
            }

            return deck;
        }

        private static string ExtractStringValue(string json, string key)
        {
            string pattern = $"\"{key}\"";
            int keyIdx = json.IndexOf(pattern);
            if (keyIdx < 0) return null;

            int colonIdx = json.IndexOf(':', keyIdx + pattern.Length);
            if (colonIdx < 0) return null;

            // Find the opening quote
            int quoteStart = json.IndexOf('"', colonIdx + 1);
            if (quoteStart < 0) return null;

            // Find the closing quote (handle escaped quotes)
            int quoteEnd = quoteStart + 1;
            while (quoteEnd < json.Length)
            {
                if (json[quoteEnd] == '"' && json[quoteEnd - 1] != '\\')
                    break;
                quoteEnd++;
            }

            if (quoteEnd >= json.Length) return null;

            return UnescapeJson(json.Substring(quoteStart + 1, quoteEnd - quoteStart - 1));
        }

        private static int ExtractIntValue(string json, string key, int defaultValue)
        {
            string pattern = $"\"{key}\"";
            int keyIdx = json.IndexOf(pattern);
            if (keyIdx < 0) return defaultValue;

            int colonIdx = json.IndexOf(':', keyIdx + pattern.Length);
            if (colonIdx < 0) return defaultValue;

            // Find the number
            int start = colonIdx + 1;
            while (start < json.Length && (json[start] == ' ' || json[start] == '\t' || json[start] == '\n' || json[start] == '\r'))
                start++;

            int end = start;
            while (end < json.Length && (char.IsDigit(json[end]) || json[end] == '-'))
                end++;

            if (end <= start) return defaultValue;

            if (int.TryParse(json.Substring(start, end - start), out int result))
                return result;

            return defaultValue;
        }

        private static int FindMatchingBracket(string s, int start, char open, char close)
        {
            int depth = 0;
            for (int i = start; i < s.Length; i++)
            {
                if (s[i] == open) depth++;
                else if (s[i] == close)
                {
                    depth--;
                    if (depth == 0) return i;
                }
            }
            return -1;
        }

        private static string EscapeJson(string s)
        {
            if (s == null) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
        }

        private static string UnescapeJson(string s)
        {
            if (s == null) return "";
            return s.Replace("\\\"", "\"").Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t").Replace("\\\\", "\\");
        }
    }
}
