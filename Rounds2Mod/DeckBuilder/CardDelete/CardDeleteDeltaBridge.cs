using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DeckBuilder.CardDelete;

/// <summary>Reads card-removal stat deltas from InfoOverhaul via reflection (soft dependency).</summary>
internal static class CardDeleteDeltaBridge
{
    private static bool _resolved;
    private static MethodInfo _tryGetRemovalDeltas;
    private static Type _statDeltaLineType;
    private static FieldInfo _labelField;
    private static FieldInfo _beforeField;
    private static FieldInfo _afterField;
    private static FieldInfo _isNoteField;
    private static MethodInfo _formatRichText;

    public static bool IsAvailable
    {
        get
        {
            Resolve();
            return _tryGetRemovalDeltas != null;
        }
    }

    public static string FormatRemovalPreview(Player player, CardInfo card)
    {
        if (player == null || card == null)
            return "N/A";

        if (!TryGetRemovalPreview(player, card, out string preview))
            return IsAvailable ? "N/A" : "Stat preview unavailable (InfoOverhaul not loaded)";

        return preview;
    }

    private static bool TryGetRemovalPreview(Player player, CardInfo card, out string preview)
    {
        preview = "N/A";
        Resolve();

        if (_tryGetRemovalDeltas == null)
            return false;

        try
        {
            object[] args = { player, card, null };
            bool ok = (bool)_tryGetRemovalDeltas.Invoke(null, args);
            if (!ok)
                return true;

            if (!(args[2] is IEnumerable deltas))
                return true;

            var formatted = new StringBuilder();
            int count = 0;
            foreach (object delta in deltas)
            {
                if (delta == null)
                    continue;

                if (count > 0)
                    formatted.AppendLine();

                formatted.Append(FormatLine(delta));
                count++;
            }

            preview = count == 0 ? "No stat changes" : formatted.ToString();
            return true;
        }
        catch (Exception ex)
        {
            CardDeleteLog.Warn($"CardDeleteDeltaBridge failed: {ex.Message}");
            return false;
        }
    }

    private static string FormatLine(object delta)
    {
        if (_formatRichText != null)
            return (string)_formatRichText.Invoke(delta, null);

        bool isNote = _isNoteField != null && (bool)_isNoteField.GetValue(delta);
        string label = (string)_labelField?.GetValue(delta);
        if (isNote)
            return label ?? string.Empty;

        string before = (string)_beforeField?.GetValue(delta);
        string after = (string)_afterField?.GetValue(delta);
        return $"{label}: {before} --> {after}";
    }

    private static void Resolve()
    {
        if (_resolved)
            return;

        _resolved = true;

        try
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "InfoOverhaul");
            if (asm == null)
            {
                CardDeleteLog.Line("InfoOverhaul not loaded — delete preview will show N/A.");
                return;
            }

            var registryType = asm.GetType("InfoOverhaul.Delta.CardDeltaRegistry");
            if (registryType == null)
            {
                CardDeleteLog.Warn("CardDeltaRegistry type not found.");
                return;
            }

            _tryGetRemovalDeltas = registryType.GetMethod(
                "TryGetRemovalDeltas",
                BindingFlags.Public | BindingFlags.Static);

            _statDeltaLineType = asm.GetType("InfoOverhaul.Delta.StatDeltaLine");
            if (_statDeltaLineType != null)
            {
                _labelField = _statDeltaLineType.GetField("Label", BindingFlags.Public | BindingFlags.Instance);
                _beforeField = _statDeltaLineType.GetField("Before", BindingFlags.Public | BindingFlags.Instance);
                _afterField = _statDeltaLineType.GetField("After", BindingFlags.Public | BindingFlags.Instance);
                _isNoteField = _statDeltaLineType.GetField("IsNote", BindingFlags.Public | BindingFlags.Instance);
                _formatRichText = _statDeltaLineType.GetMethod("FormatRichText", BindingFlags.Public | BindingFlags.Instance);
            }
        }
        catch (Exception ex)
        {
            CardDeleteLog.Warn($"CardDeleteDeltaBridge resolve failed: {ex.Message}");
        }
    }
}
