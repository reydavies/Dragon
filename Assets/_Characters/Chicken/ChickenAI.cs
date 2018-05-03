using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChickenAI : MonoBehaviour {

    NavMeshAgent navMeshAgent;
    [SerializeField] ChickenWaypointContainer patrolPath;
    [SerializeField] float waypointTolerance = 2.0f;


    int nextWaypointIndex;
    public float count = 0;
    public float animationCount;
    public float wayPointChangeTime;
    private float animCount = 0;

    Animator animator;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        StartCoroutine(Patrol());
    }

    IEnumerator Patrol()
    {
        count++;
        while (patrolPath != null)
        {
            if (count >= wayPointChangeTime)
            {
            nextWaypointIndex = UnityEngine.Random.Range(0, 3);
            count = 0;
            }
            Vector3 nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
            navMeshAgent.SetDestination(nextWaypointPos);
            CycleWaypointWhenClose(nextWaypointPos);
            yield return nextWaypointPos;
        }
    }

    private void CycleWaypointWhenClose(Vector3 nextWaypointPos)
    {
        if (Vector3.Distance(transform.position, nextWaypointPos) <= waypointTolerance)
        {
            nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
        }
    }

    IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        navMeshAgent.isStopped = false;
        count = 0;
    }

    void AnimControl()
    {
        animCount++;
        count++;
        if (animCount >= animationCount && count >= wayPointChangeTime)
        {
            animator.SetTrigger("isTrigger");
            navMeshAgent.isStopped = true;
            StartCoroutine(Wait(1.1f)); 
            animCount = 0;
        } 
    }

    void Update()
    {
        AnimControl();       
    }
}
