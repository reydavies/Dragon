using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RPG.CameraUI; // for mouse events

namespace RPG.Characters
{
    public class PlayerControl : MonoBehaviour
    {
        List<GameObject> newTarget = new List<GameObject>();

        public float moveSpeed = 0;
        public float crocDamage = 20;
        public float attackDistanceToCroc;

        const string ATTACK_TRIGGER = "Attack";

        GameObject enemy; 
        Character character;
        SpecialAbilities abilities;
        WeaponSystem weaponSystem;
        Animator animator;

        bool isInDirectMode = false;

        GameObject closestEnemy;

        public BloodSplatter splatter;

        void Start()
        {
            character = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();
            weaponSystem = GetComponent<WeaponSystem>();

            RegisterForMouseEvents();
        }

        GameObject GetClosestEnemy()
        {
            GameObject[] gos;
            GameObject[] gos1;
            gos = GameObject.FindGameObjectsWithTag("Enemy");

            gos1 = GameObject.FindGameObjectsWithTag("Crocodile2");

            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = transform.position;
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

        private void RegisterForMouseEvents()
        {
            var cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
        }

        void CrocAttack()
        {
            closestEnemy = GetClosestEnemy();
            if (closestEnemy != null && closestEnemy.CompareTag ("Crocodile2"))
            {
                var distanceToCroc = Vector3.Distance(transform.position, closestEnemy.transform.position);
                if (distanceToCroc <= attackDistanceToCroc)
                {
                    closestEnemy.GetComponent<CrocHealthSystem>().currentHealthPoints
                    = closestEnemy.GetComponent<CrocHealthSystem>().currentHealthPoints - crocDamage;
                    if (closestEnemy.CompareTag ("Crocodile2"))
                    {
                        weaponSystem.animator.SetTrigger(ATTACK_TRIGGER);
                        transform.LookAt(closestEnemy.transform);
                        splatter.transform.position = new Vector3(closestEnemy.transform.position.x, closestEnemy.transform.position.y + 0.7f, closestEnemy.transform.position.z );
                        splatter.partSys.Play();
                    }
                    if (closestEnemy.GetComponent<CrocHealthSystem>().currentHealthPoints <= 0)
                    {
                        closestEnemy.GetComponent<CrocHealthSystem>().KillCroc();
                    }
                }
            }
        }

        void Update()
        {
            ScanForAbilityKeyDown();
        }

        private void ProcessDirectMovement()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 m_CamForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(moveSpeed, 0, moveSpeed / Time.deltaTime));
            Vector3 m_Move = v * m_CamForward + h * Camera.main.transform.right * moveSpeed / Time.deltaTime;
            
            character.Move(m_Move); 
        }

        void ScanForAbilityKeyDown()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                isInDirectMode = !isInDirectMode;  // Toggle mode
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                CrocAttack();
            }

            if (isInDirectMode)
            {
                ProcessDirectMovement();
            }

            if (Input.GetButtonDown("Fire3"))
            {
                enemy = GetClosestEnemy();
                if (enemy != null && IsTargetInRange(enemy.gameObject))
                {
                    weaponSystem.AttackTarget(enemy.gameObject);
                }
            }


            for (int keyIndex = 1; keyIndex < abilities.GetNumberOfAbilities(); keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()) )
                {
                    abilities.AttemptSpecialAbility(keyIndex);
                }

                if(Input.GetAxis("Fire2") > 0)
                {
                    abilities.AttemptSpecialAbility(1);
                }             
            }
        }

        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                weaponSystem.StopAttacking();
                character.SetDestination(destination);
            }
        }

        bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
        }

        void OnMouseOverEnemy(EnemyAI enemy)
        {
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                weaponSystem.AttackTarget(enemy.gameObject);
            }
            else if (Input.GetMouseButton(0) && !IsTargetInRange(enemy.gameObject))
            {
                StartCoroutine(MoveAndAttack(enemy));
            }
            else if (Input.GetMouseButtonDown(1) && IsTargetInRange(enemy.gameObject))
            {
                abilities.AttemptSpecialAbility(0, enemy.gameObject);
            }
            else if (Input.GetMouseButtonDown(1) && !IsTargetInRange(enemy.gameObject))
            {
                StartCoroutine(MoveAndPowerAttack(enemy));
            }
        }

        IEnumerator MoveToTarget(GameObject target)
        {
            character.SetDestination(target.transform.position);
            while (!IsTargetInRange(target))
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }

        IEnumerator MoveAndAttack(EnemyAI enemy)
        {
            yield return StartCoroutine(MoveToTarget(enemy.gameObject));
            weaponSystem.AttackTarget(enemy.gameObject);
        }

        IEnumerator MoveAndPowerAttack(EnemyAI enemy)
        {
            yield return StartCoroutine(MoveToTarget(enemy.gameObject));
            abilities.AttemptSpecialAbility(0, enemy.gameObject);
        }
    }
}
