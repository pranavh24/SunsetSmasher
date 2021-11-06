using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData
{
    public EnemyState enemyState;
}

public enum EnemyState {
    Wandering, InCombat, Chasing, Dead, Stunned
}