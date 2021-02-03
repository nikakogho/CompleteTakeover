using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attackable : MonoBehaviour
{
    public abstract Transform Center { get; }
    public abstract void HealBy(float value);
    public abstract void TakeDamage(float damage); //may do different kinds of damage
    public abstract bool IsAlive { get; }
    public abstract Collider BoundsCollider { get; }
}
