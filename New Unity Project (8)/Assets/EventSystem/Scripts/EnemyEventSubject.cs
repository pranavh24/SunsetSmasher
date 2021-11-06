using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For any events related to enemies
public class EnemyEventSubject
{
    public static event Action<EnemyEventArgs, Transform> enemyDeathEvent;
    public static void RaiseEnemyDeathEvent(EnemyEventArgs args, Transform killer) {
        enemyDeathEvent?.Invoke(args, killer);
    }
    
    public class EnemyEventArgs {
        public Transform enemyTransform;
        public Enemy enemyBehavior;
    }
}
