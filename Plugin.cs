using System;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace ChatPrefixesClient
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BasePlugin
    {
        public const string PluginGuid = "agenthere.chatprefixes.client";
        public const string PluginName = "Chat Prefixes Client";
        public const string PluginVersion = "1.0.0";

        internal static ManualLogSource Logger;

        public override void Load()
        {
            Logger = base.Log;
            Logger.LogInfo($"[ChatPrefixesClient] {PluginName} v{PluginVersion} loading...");

            try
            {
                var harmony = new Harmony(PluginGuid);
                PatchInstaller.Install(harmony);
                Logger.LogInfo("[ChatPrefixesClient] rich-text hooks installed.");
            }
            catch (Exception e)
            {
                Logger.LogError($"[ChatPrefixesClient] patch failed: {e}");
            }

            Logger.LogInfo($"[ChatPrefixesClient] {PluginName} v{PluginVersion} loaded.");
        }
    }
}
