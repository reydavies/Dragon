﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnPath : MonoBehaviour {

    public PathScript PathToFollow;

    public int CurrentWayPointID = 0;
    public float speed;
    private float reachDistance = 1.0f;
    public float rotationSpeed = 5.0f;
    public string pathName;

    Vector3 current_position;

    GameObject player;

	void Start ()
    {
        //PathToFollow = GameObject.Find(pathName).GetComponent<PathScript>();
	}

    public void Circling()
    {
        float distance = Vector3.Distance(PathToFollow.path_objs[CurrentWayPointID].position, transform.position);
        transform.position = Vector3.MoveTowards(transform.position, PathToFollow.path_objs[CurrentWayPointID].position, Time.deltaTime * speed);

        var rotation = Quaternion.LookRotation(PathToFollow.path_objs[CurrentWayPointID].position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

        if (distance <= reachDistance)
        {
            CurrentWayPointID++;
        }

        if (CurrentWayPointID >= PathToFollow.path_objs.Count)
        {
            CurrentWayPointID = 0;
        }
    }
}
