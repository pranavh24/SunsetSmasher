using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEventSubject
{
    public static event Action pauseEvent;
    public static event Action playerDeathEvent;
    public static void RaisePauseEvent() {
        pauseEvent?.Invoke();
    }

    public static void RaisePlayerDeathEvent()
    {
        playerDeathEvent?.Invoke();
    }
    
    public static event Action<float> enemyTimeUpdateEvent;
    public static void RaiseEnemyTimeUpdateEvent(float normalizedTime) {
        enemyTimeUpdateEvent?.Invoke(normalizedTime);
    }
    public static event Action<string> statusMsgEvent;
    public static void RaiseStatusMsgEvent(string text) { statusMsgEvent?.Invoke(text); }
    public static event Action<PlayerData> playerDataUpdatedEvent;
    public static void RaisePlayerDataUpdatedEvent(PlayerData playerData) { playerDataUpdatedEvent?.Invoke(playerData); }
}
