using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
    public LayerMask targetMask; //may do some bullets capable of friendly fire

    public float initialVelocity = 20;
    public float damage;
    public bool HasSplashAttack;
    [HideInInspector]public float splashRadius;
    [HideInInspector]public float splashDamage;
    Rigidbody rb;

    bool used = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Invoke("VelSet", 0.1f);
    }

    void VelSet()
    {
        rb.velocity = transform.forward * initialVelocity;
    }

    void OnTriggerEnter(Collider other)
    {
        if (used) return;
        used = true;

        var attackable = other.GetComponent<Attackable>();
        if (attackable != null) attackable.TakeDamage(damage);

        if (HasSplashAttack)
        {
            foreach(var col in Physics.OverlapSphere(transform.position, splashRadius, targetMask))
            {
                attackable = col.GetComponent<Attackable>();
                if (attackable != null) attackable.TakeDamage(damage);
            }
        }
        
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}
