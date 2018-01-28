using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxScript : MonoBehaviour
{
    private float spawnTime;

	// Use this for initialization
	void Start ()
    {
        spawnTime = Time.time;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Time.time - spawnTime > 1) DestroyObject(gameObject);
	}
}
