using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsectManAI : MonoBehaviour
{

    public Animator animator;

    public GameObject InsectStandOnDeck;

    void Start ()
    {
        animator = GetComponent<Animator>();
        InsectStandOnDeck = GameObject.Find("InsectStandOnDeck");

        transform.position = InsectStandOnDeck.transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = InsectStandOnDeck.transform.position;
    }
}
