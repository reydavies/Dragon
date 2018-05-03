using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Characters
{
    public class CrocodileAI : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 6f;
        public WaypointContainer patrolPath;
        [SerializeField] float waypointTolerance = 2f;
        [SerializeField] float waypointDwellTime = 2f; 

        PlayerControl player = null;
        int nextWaypointIndex;
        float currentStalkingRange = 2f;
        float distanceToPlayer;
        float playerDamage = 20;
        float playerHealth; 

        public float damageCount;

        public NavMeshAgent navMeshAgent;

        private Vector3 startPosition;
        private Quaternion startRotation;

        public Animator animator;

        public BloodSplatter splatt;

        void Start()  
        {
            player = FindObjectOfType<PlayerControl>();
            startPosition = transform.position;
            startRotation = transform.rotation;
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator.SetFloat("ChangeSpeed", 1.4f);
        }

        void Update()
        {
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

            bool inAttackRing = distanceToPlayer <= currentStalkingRange;
            bool inChaseRing = distanceToPlayer > currentStalkingRange
                                 &&
                                 distanceToPlayer <= chaseRadius;
            bool outsideChaseRing = distanceToPlayer > chaseRadius;
          
            if (outsideChaseRing) 
            {
                StopAllCoroutines();
                if (patrolPath != null)
                {
                    navMeshAgent.isStopped = false;
                    animator.SetTrigger("Walking");
                    StartCoroutine(Patrol());
                    navMeshAgent.speed = 1f;
                    navMeshAgent.angularSpeed = 50f;
                }
                else
                {
                    navMeshAgent.isStopped = true;
                    animator.SetTrigger("Idle");
                }

            }           
            if (inChaseRing)
            {
                StopAllCoroutines();
                StartCoroutine(ChasePlayer());
                animator.SetTrigger("Walking");
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = 2f;
                navMeshAgent.angularSpeed = 300f;
            }
            if (inAttackRing && player.GetComponent<HealthSystem>().currentHealthPoints > 0)
            {
                damageCount++;

                transform.LookAt(player.transform);  
                StopAllCoroutines();
                navMeshAgent.isStopped = true;
                animator.SetTrigger("Attacking");
                navMeshAgent.angularSpeed = 300f;
                if (damageCount >= 150 && GetComponent<CrocHealthSystem>().currentHealthPoints > 0) 
                {
                    splatt.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z);
                    splatt.partSys.Play();
                    player.GetComponent<HealthSystem>().TakeDamage(playerDamage);
                    damageCount = 0;
                }
                if (player.GetComponent<HealthSystem>().currentHealthPoints <= 0)
                {
                    damageCount = 0;
                    KillAndReset();
                }
            }

            if (!navMeshAgent.isOnNavMesh)
            {
                Debug.LogError(gameObject.name + " uh oh this guy is not on the navmesh");
            }
            
        }

        public void SetDestination(Vector3 worldPos)
        {
            navMeshAgent.destination = worldPos;
        }

        IEnumerator Patrol()
        {
            animator.SetTrigger("Walking");
            while (patrolPath != null)
            {
                Vector3 nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
                navMeshAgent.SetDestination(nextWaypointPos);
                CycleWaypointWhenClose(nextWaypointPos);
                yield return new WaitForSeconds(waypointDwellTime);
            }
        }

        private void CycleWaypointWhenClose(Vector3 nextWaypointPos)
        {
            if (Vector3.Distance(transform.position, nextWaypointPos) <= waypointTolerance)
            { 
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
            }
        }

        IEnumerator ChasePlayer()
        {
            while (distanceToPlayer >= currentStalkingRange)
            {
                navMeshAgent.SetDestination(player.transform.position);
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator Wait(float seconds)
        {
            yield return new WaitForSeconds(2);
        }

        public void KillAndReset()
        {
            transform.position = startPosition;
            transform.rotation = startRotation;
            navMeshAgent.isStopped = false;
            damageCount = 0;           
        }

        void OnDrawGizmos()
        {
            // Draw attack sphere 
            Gizmos.color = new Color(255f, 0, 0, .5f);
            Gizmos.DrawWireSphere(transform.position, currentStalkingRange); 

            // Draw chase sphere 
            Gizmos.color = new Color(0, 0, 255, .5f);
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }
    }
}

