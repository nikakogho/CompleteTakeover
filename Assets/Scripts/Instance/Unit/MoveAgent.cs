using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(Unit))]
public class MoveAgent : MonoBehaviour {
    Unit unit;
    Rigidbody rb;
    Transform target;
    float approachDist;
    float approachSqr;

    const float rotationSpeed = 120;

    bool togo;
    Vector3 destination;
    Vector3 destDir;

    Vector3 lastPos;

    Animator anim;

    bool shouldMove = true;

    void Awake()
    {
        lastPos = transform.position;
        unit = GetComponent<Unit>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        destination = destDir = Vector3.zero;

        InvokeRepeating("MoveAnimUpdate", 0.2f, 0.2f);
        InvokeRepeating("ShouldMoveUpdate", 0.2f, 0.3f);
    }

    void ShouldMoveUpdate()
    {
        if (target == null) shouldMove = true;
        else shouldMove = !unit.IsInAttackRange;
    }

    void MoveAnimUpdate()
    {
        bool moving = lastPos != transform.position;
        lastPos = transform.position;
        anim.SetBool("moving", moving);
    }

    public void FollowAtActRange(Transform target)
    {
        this.target = target;
        approachDist = unit.data.attackRange;
        approachSqr = approachDist * approachDist;
        togo = false;
    }

    public void FollowTargetAtDist(Transform target, float dist)
    {
        this.target = target;
        approachDist = dist;
        approachSqr = approachDist * approachDist;
        togo = false;
    }

    public void SetDestination(Vector3 destination)
    {
        target = null;
        this.destination = destination;
        destDir = (destination - transform.position).normalized;
        approachDist = 0.2f;
    }

    public void Stop()
    {
        togo = false;
        target = null;
    }

    void FixedUpdate()
    {
        if (togo)
        {
            destDir.y = 0;
            if (Vector3.SqrMagnitude(transform.position - destination) > approachDist)
                rb.MovePosition(rb.position + destDir * unit.data.moveSpeed * Time.fixedDeltaTime);
            else togo = false;
            return;
        }

        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        float rotateBy = rotationSpeed * Time.fixedDeltaTime;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotateBy);

        if (!shouldMove) return;
        if (dir.sqrMagnitude < approachSqr) return;

        dir.Normalize();

        Vector3 move = dir * unit.data.moveSpeed * Time.fixedDeltaTime;
        move.y = 0;

        rb.MovePosition(rb.position + move);
    }
}
