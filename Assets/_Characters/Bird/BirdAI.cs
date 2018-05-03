using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class BirdAI : MonoBehaviour
    {
        List<GameObject> newTarget = new List<GameObject>();

        public float attackSpeed = 0f;
        public GameObject target;
        public GameObject player;
        public float enemyDamage = 20f;
        public float crocDamage;
        public float attackDelayTime = 0;
        private float time = 20;

        GameObject closestEnemy;

        private int i = 0;

        bool isCircling = true;

        public BloodSplatter splatt;
        MoveOnPath moveOnPath;
        Animator animator;

        void Start()
        {
            time = Time.time;
            player = GameObject.FindGameObjectWithTag("Player");
            moveOnPath = GetComponent<MoveOnPath>();
            animator = GetComponent<Animator>();
        }

        GameObject GetClosestEnemy()
        {
            GameObject[] gos;
            GameObject[] gos1;
            gos = GameObject.FindGameObjectsWithTag("Enemy");
            gos1 = GameObject.FindGameObjectsWithTag("Crocodile2");
            
            GameObject closest = null;
            float distance = Mathf.Infinity; 
            Vector3 position = player.transform.position;
            foreach (GameObject go in gos)
            {                
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;

                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                    newTarget.Add(closest);                  
                }                
            }
            foreach (GameObject go in gos1)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;

                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                    newTarget.Add(closest);
                }
            }
            return closest;
        }

        public void BirdAttack()
        {           
            if (target)
            {
                isCircling = false;
                float step = attackSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
                transform.LookAt(target.transform.position);
                if (transform.position == target.transform.position)
                {
                    StartCoroutine(Wait());
                }               
            }
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(3);
            isCircling = true;
        }        

        void ScanForAbilityKeyDown()
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (Time.time >= time + attackDelayTime)
                {
                    if (target)
                    {
                        OnTriggerEnter(GetComponent<Collider>());
                        BirdAttack();
                    }
                    time = Time.time;
                }               
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            closestEnemy = GetClosestEnemy();
            if (collider.gameObject.CompareTag ("Enemy Attack Area")) 
            {
                if (i == 0)
                {
                    if (collider != null)
                    {
                        i++;
                        animator.Play("Bird_Attack");
                        splatt.transform.position = target.transform.position;
                        splatt.partSys.Play();

                        if (closestEnemy != null && closestEnemy.CompareTag ("Enemy"))
                        {
                            GetClosestEnemy().gameObject.GetComponent<HealthSystem>().TakeDamage(enemyDamage);
                        }
                        if (closestEnemy != null && closestEnemy.CompareTag ("Crocodile2"))
                        {
                            GetClosestEnemy().gameObject.GetComponent<CrocHealthSystem>().TakeDamage(crocDamage);
                        }
                    }
                }
                i = 0;
            }
        }
      
        void Update()
        {
            ScanForAbilityKeyDown();
                              
            if (isCircling)
            {
                moveOnPath.Circling();
            }
            else
            {
                if (target)
                {
                    BirdAttack();
                }
            }
        }
    }
}
