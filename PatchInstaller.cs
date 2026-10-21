using System;
using System.Reflection;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace ChatPrefixesClient
{
    /// <summary>Точечная установка патчей: если какой-то метод не найден, остальные всё равно встанут.</summary>
    internal static class PatchInstaller
    {
        internal static void Install(Harmony harmony)
        {
            // UI-текст (чат, интерфейс)
            Patch(harmony, typeof(TextMeshProUGUI), "set_text", nameof(RichTextHooks.UiText));
            Patch(harmony, typeof(TextMeshProUGUI), "SetText", nameof(RichTextHooks.UiSetText));

            // Базовый класс TMP (если конкретные классы не переопределяют свойство)
            Patch(harmony, typeof(TMP_Text), "set_text", nameof(RichTextHooks.UiText));
            Patch(harmony, typeof(TMP_Text), "SetText", nameof(RichTextHooks.UiSetText));

            // Мировой текст (таблички над головой и т.п.)
            Patch(harmony, typeof(TextMeshPro), "set_text", nameof(RichTextHooks.WorldText));
            Patch(harmony, typeof(TextMeshPro), "SetText", nameof(RichTextHooks.WorldSetText));
        }

        private static void Patch(Harmony harmony, Type type, string methodName, string prefixName)
        {
            try
            {
                MethodInfo target = AccessTools.Method(type, methodName, new[] { typeof(string) });
                if (target == null)
                {
                    Plugin.Logger.LogWarning($"[ChatPrefixesClient] not found: {type.Name}.{methodName}");
                    return;
                }

                MethodInfo prefix = typeof(RichTextHooks).GetMethod(prefixName, BindingFlags.Static | BindingFlags.NonPublic);
                harmony.Patch(target, prefix: new HarmonyMethod(prefix));
                Plugin.Logger.LogInfo($"[ChatPrefixesClient] patched {type.Name}.{methodName}");
            }
            catch (Exception e)
            {
                Plugin.Logger.LogWarning($"[ChatPrefixesClient] patch {type.Name}.{methodName} failed: {e.Message}");
            }
        }
    }
}
