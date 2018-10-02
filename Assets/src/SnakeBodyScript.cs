using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBodyScript : MonoBehaviour
{
    private float turnSpeed = 0.05f;
    private float angle;
    private Vector3 currentRot = Vector3.up;
    private Vector3 targetRot;
    private Vector3 realRot;
    
    private float moveMagnitude;
    private Vector2 moveDirection;
    private Vector2 targetPos;

    private int position;
    public Transform body;
    private Transform prefab;
    private Transform leader;

    // Use this for initialization
    void Start ()
    {
        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y - 1);

        leader = transform.parent;
        transform.parent = null;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        angle = Vector3.Angle(currentRot, targetRot);
        if (angle > 90) currentRot = Vector3.RotateTowards(currentRot, targetRot, turnSpeed * moveMagnitude * (angle / 60), 0);
        if(moveMagnitude > 0) currentRot = Vector3.RotateTowards(currentRot, targetRot, turnSpeed * moveMagnitude * (angle / 45), 0);
        

        targetPos = new Vector2(leader.position.x - (currentRot.normalized.x * ((180 - angle) / 180)), 
                                    leader.position.y - (currentRot.normalized.y * ((180 - angle) / 180)));
        moveDirection = new Vector2(targetPos.x - transform.position.x, targetPos.y - transform.position.y);

        realRot = (leader.position - (targetRot.normalized * 0.4f)) - transform.position;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, realRot);
        transform.Translate(moveDirection, Space.World);
        prefab.SendMessage("Move", new Vector4(currentRot.x, currentRot.y, currentRot.z, moveMagnitude));
    }

    void Move(Vector4 i)
    {
        targetRot = new Vector3(i.x, i.y, i.z);
        moveMagnitude = i.w;
        //currentRot = Vector3.RotateTowards(currentRot, i, turnSpeed, 0);
        //transform.position = new Vector3(leader.position.x - currentRot.normalized.x, leader.position.y - currentRot.normalized.y);
        //transform.rotation = Quaternion.LookRotation(Vector3.forward, currentRot);
    }

    void SpawnBody(int i)
    {
        prefab = Instantiate(body, this.transform);
        if (i - 1 > 0) prefab.SendMessage("SpawnBody", i - 1);
        position = i;
    }
}
