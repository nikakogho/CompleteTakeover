using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class CampMoveAgent : MonoBehaviour {
    int index;
    Animator anim;
    Rigidbody rb;
    Transform[] points;
    bool moving = false;
    Transform target;
    float approachSqr = 0.2f;
    float speed;
    
    public void Init(float speed, Transform[] points, int index)
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        this.speed = speed;
        this.points = points;
        this.index = index;
        StartMoving(points[index]);
    }

    void StopMoving()
    {
        moving = false;
        anim.SetBool("moving", false);
    }

    void StartMoving(Transform target)
    {
        moving = true;
        this.target = target;
        transform.LookAt(target);
        anim.SetBool("moving", true);
    }

    IEnumerator NextPointRoutine()
    {
        StopMoving();
        float waitTime = Random.Range(1f, 7f);
        yield return new WaitForSeconds(waitTime);
        index++;
        if (points.Length == index) index = 0;
        StartMoving(points[index]);
    }

    void FixedUpdate()
    {
        if (!moving) return;
        Vector3 dir = target.position - transform.position;
        if (dir.sqrMagnitude < approachSqr) StartCoroutine(NextPointRoutine());
        else rb.MovePosition(rb.position + dir.normalized * speed * Time.fixedDeltaTime);
    }
}
