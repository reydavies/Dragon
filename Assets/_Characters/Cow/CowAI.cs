using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CowAI : MonoBehaviour {

    [SerializeField] CowWaypointContainer patrolPath;
    [SerializeField] float waypointTolerance = 2f;

    Animator animator;
    NavMeshAgent navMeshAgent;

    int nextWaypointIndex;
    public float count = 0;
    public float animationCount;
    public float waypointChangeTime = 0;

    private float animCount = 0;

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
            if (count >= waypointChangeTime)
            {
                nextWaypointIndex = Random.Range(0, 3);
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
        if (animCount >= animationCount && count >= waypointChangeTime)
        {
            animator.SetTrigger("isTrigger");
            navMeshAgent.isStopped = true;
            StartCoroutine(Wait(7.5f));
            animCount = 0;
        }
    }

    void Update()
    {
        AnimControl();
    }
}
