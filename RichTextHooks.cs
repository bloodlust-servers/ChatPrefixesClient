using System;
using HarmonyLib;
using TMPro;

namespace ChatPrefixesClient
{
    /// <summary>
    /// Клиентская часть: включает rich-text у TMP-текста, если в строке есть теги цвета.
    /// Так ник с префиксом красится и в чате, и в табличке над головой.
    /// Сервер присылает ник уже с тегами (&lt;color=#RRGGBB&gt;), клиент их просто рисует.
    /// </summary>
    internal static class RichTextHooks
    {
        private static bool HasTags(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            return value.IndexOf("<color=", StringComparison.Ordinal) >= 0
                   || value.IndexOf("<#", StringComparison.Ordinal) >= 0;
        }

        private static void Enable(TMP_Text text, string value)
        {
            if (text == null || !HasTags(value)) return;

            try
            {
                if (!text.richText) text.richText = true;
            }
            catch { }
        }

        // --- установка текста через свойство text ---

        internal static void UiText(TMP_Text __instance, string value) => Enable(__instance, value);
        internal static void WorldText(TMP_Text __instance, string value) => Enable(__instance, value);

        // --- установка текста через SetText(string) ---

        internal static void UiSetText(TMP_Text __instance, string value) => Enable(__instance, value);
        internal static void WorldSetText(TMP_Text __instance, string value) => Enable(__instance, value);
    }
}
