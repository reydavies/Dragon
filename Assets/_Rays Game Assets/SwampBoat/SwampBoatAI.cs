using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{

    public class SwampBoatAI : MonoBehaviour
    {
        public Transform target;

        private GameObject player;
        private GameObject Insect_Man2;
        public GameObject StandOnDeck;
        
        public float speed = 0;

        bool onDeck = false;


        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            StandOnDeck = GameObject.Find("StandOnDeck");
            Insect_Man2 = GameObject.Find("Insect_Man2");
        }

        void Move()
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.CompareTag ( "Player") && transform.position != target.transform.position)
            {
                onDeck = true;
                Insect_Man2.GetComponent<InsectManAI>().animator.SetTrigger("isPaddling");
            }

            if (transform.position == target.transform.position)
            {
                player.transform.position = player.transform.position;
                Insect_Man2.GetComponent<InsectManAI>().animator.SetTrigger("isIdle");
                onDeck = false;
            }
        }


        void FixedUpdate()   
        {
            if (onDeck)
            {
                player.transform.position = StandOnDeck.transform.position;
                Move();
            }
        } 
    }
}
