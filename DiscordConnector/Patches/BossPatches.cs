using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace DiscordConnector.Patches;
internal class BossPathces
{
    [HarmonyPatch]
    public class BossPatches
    {
        // Патчим метод RandEventSystem.GetBossEvent
        [HarmonyPatch(typeof(RandEventSystem), nameof(RandEventSystem.GetBossEvent))]
        [HarmonyPrefix]
        private static bool Prefix(ref string __result)
        {
            if (EnemyHud.instance != null)
            {
                var character = new Character();
                // Получаем активного босса
                string activeBoss = character.m_bossEvent;

                // Если активный босс найден и у него есть событие
                if (activeBoss != null && !string.IsNullOrEmpty(activeBoss))
                {
                    string bossName = Localization.instance.Localize(activeBoss); // Имя босса
                    // Vector3 bossPosition = activeBoss.transform.position; Неверная позиция босса, надо исправить
                    string bossPosition = "типа его позиция";
                    Plugin.StaticLogger.LogDebug(
                        $"{bossName} был вызван на {bossPosition}."
                    );

                    // Если включено уведомление о начале события
                    if (Plugin.StaticConfig.BossStartMessageEnabled)
                    {
                        // Формируем сообщение
                        string message = MessageTransformer.FormatEventStartMessage(
                            Plugin.StaticConfig.BossStartMessage, // Шаблон сообщения
                            bossName, // Имя босса
                            bossPosition = "типа его позиция"// Позиция босса
                        );

                        // Проверяем, нужно ли включать позицию в сообщение
                        if (!Plugin.StaticConfig.BossStartPosEnabled)
                        {
                            DiscordApi.SendMessage(Webhook.Event.EventStart, message);
                        }
                        else
                        {
                            if (Plugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
                            {
                                DiscordApi.SendMessage(Webhook.Event.BossStart, message); // bossPosition
                            }
                            else
                            {
                                message = MessageTransformer.FormatEventStartMessage(
                                    Plugin.StaticConfig.BossStartMessage,
                                    bossName,
                                    bossPosition
                                );
                                DiscordApi.SendMessage(Webhook.Event.BossStart, message);
                            }
                        }
                    }
                    __result = activeBoss;
                    return false;
                }
            }
            __result = null;
            return false;
        }
    }
}
