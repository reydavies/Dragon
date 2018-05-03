using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

namespace RPG.Characters
{

    public class CrocHealthSystem : MonoBehaviour 
    {
        public float maxHealthPoints = 200f;
        [SerializeField] Image healthBar;
        [SerializeField] AudioClip clip;

        public float currentHealthPoints;
        public Animator animator;
        AudioSource audioSource;
        CrocodileAI crocodileAI;

        public NavMeshAgent agent;

         
        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            crocodileAI = GetComponent<CrocodileAI>();
            currentHealthPoints = maxHealthPoints;
        }

        void Update()
        {
            UpdateHealthBar();
        }

        void UpdateHealthBar()
        {
            if (healthBar)  // Enemies may not have health bars to update
            {
                healthBar.fillAmount = healthAsPercentage;
            }
        }

        public void KillCroc()
        {
            agent.isStopped = true;           
            StartCoroutine(Wait());
        }

        public void TakeDamage(float damage)
        {
            bool crocDies = (currentHealthPoints - damage <= 0);
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);

            if (crocDies )  
            {
                KillCroc();
            }
        }

        IEnumerator Wait()
        {
            audioSource.clip = clip;
            audioSource.Play();
            animator.Play("Death");
            yield return new WaitForSeconds(audioSource.clip.length);
            //agent.isStopped = false;
            crocodileAI.KillAndReset();
            if (crocodileAI.patrolPath == null)
            {
                animator.SetTrigger("Idle");
            }
            currentHealthPoints = maxHealthPoints;
        }        
    }
}
