using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

public class AttachToPlayer : MonoBehaviour {

    GameObject player;

    public float lift = 5.0f;
    float posX;
    float posZ;


	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        float posX = player.transform.position.x;
        float lift = player.transform.position.y + 5.0f;
        float posZ = player.transform.position.z;
        transform.position = new Vector3(posX, lift, posZ);
        transform.rotation = player.transform.rotation;
	}
}
