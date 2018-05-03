using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

namespace RPG.Characters
{

    public class HealthSystem : MonoBehaviour
    {

        public float maxHealthPoints = 100f;
        [SerializeField] Image healthBar;
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;  

        const string DEATH_TRIGGER = "Death";

        public float currentHealthPoints;
        Animator animator;
        AudioSource audioSource;
        Character characterMovement;
        EnemyAI enemyAI;
         
        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } } 

        void Start()
        {

            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            characterMovement = GetComponent<Character>();
            enemyAI = GetComponent<EnemyAI>();

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

        public void TakeDamage(float damage)
        {
            bool characterDies = (currentHealthPoints - damage <= 0);
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints); 
            var clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
            audioSource.PlayOneShot(clip);
            if (characterDies && characterMovement.CompareTag ("Player"))  // Player dies
            {
                StartCoroutine(KillCharacter());
            }
            if (characterDies && characterMovement.CompareTag ("Enemy"))  // Enemy dies
            {
                StartCoroutine(KillEnemy());
            }
        }

        public void Heal(float points)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + points, 0f, maxHealthPoints);
        }


        IEnumerator KillCharacter()
        {
            characterMovement.Kill();
            animator.SetTrigger(DEATH_TRIGGER);

            var playerComponent = GetComponent<PlayerControl>();
            if (playerComponent && playerComponent.isActiveAndEnabled)  // relying on lazy evaluation
            {
                audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
                audioSource.Play(); // override any existing sounds
                yield return new WaitForSecondsRealtime(audioSource.clip.length);
                SceneManager.LoadScene(0);
            }           
        }

        IEnumerator KillEnemy()
        {
            animator.Play("Death"); 
            audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
            audioSource.Play(); // override any existing sounds 
            yield return new WaitForSeconds(audioSource.clip.length);
            enemyAI.KillAndReset();
            animator.SetTrigger("Reset");
            currentHealthPoints = maxHealthPoints;
        }
    }
}
