using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubAttackArea : MonoBehaviour
{
    public float damageRatio = 1;
    public Target target = Target.EnemyOnly;
    public enum Target
    {
        EnemyOnly,
        AllyOnly,
        All
    }
}
