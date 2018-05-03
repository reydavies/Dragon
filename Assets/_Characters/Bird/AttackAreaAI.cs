using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAreaAI : MonoBehaviour {

    GameObject player;
    GameObject enemy;

    public GameObject target;

    List<GameObject> targets = new List<GameObject>();

    float lift = 0;

    float posX;
    float posY;
    float posZ;

    // Use this for initialization
    void Start ()
    {
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        player = GameObject.FindGameObjectWithTag("Player");
        target = GameObject.FindGameObjectWithTag("Enemy Attack Area");


        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject go in gos)
        {
            targets.Add(go);
        }
        gos = GameObject.FindGameObjectsWithTag("Crocodile2");
        foreach (GameObject go in gos)
        {
            targets.Add(go);
        }
    }

    public GameObject FindClosestEnemy()
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = player.transform.position;
        foreach (GameObject go in targets)
        {
            if (go != null)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    float Xpos = closest.transform.position.x;
                    lift = closest.transform.position.y + 1.15f;
                    float Zpos = closest.transform.position.z;
                    if (go.CompareTag ("Golem"))
                    {
                        lift = closest.transform.position.y + 2f;
                    }
                    if (go.CompareTag ("Crocodile2"))
                    {
                        Xpos = Xpos + -1.3f;
                        Zpos = Zpos + -.5f;
                    }
                    
                    distance = curDistance;
                    target.transform.position = new Vector3(Xpos, lift, Zpos);
                }
            }
        }       
        return closest;        
    }

    public void Target()
    {
        if (enemy != null)
        {
            posX = target.transform.position.x;
            posY = target.transform.position.y + 1.15f;
            posZ = target.transform.position.z;
            transform.position = new Vector3(posX, posY, posZ);
            transform.rotation = target.transform.rotation;
        }        
    }

    void Update ()
    {
        Target();
        FindClosestEnemy(); 
    }
}
