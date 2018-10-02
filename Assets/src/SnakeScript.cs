using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeScript : MonoBehaviour
{
    private float moveSpeed = 0.075f;
    private float moveMagnitude;
    private Vector2 moveDirection;

    private float turnSpeed = 0.15f;
    private Vector3 currentRot = Vector3.up;
    private Vector3 targetRot;

    public int segNum = 0;
    public Transform body;
    private Transform prefab;

    private Collider2D coll;
    private RaycastHit2D[] results;
    private ContactFilter2D contactFilter;

    // Use this for initialization
    void Start ()
    {
        contactFilter.useTriggers = false;

        coll = this.GetComponent<Collider2D>();

        if (segNum > 0) SpawnBody();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * moveSpeed;
        moveMagnitude = Mathf.Sqrt(Mathf.Pow(Input.GetAxis("Horizontal"), 2) + Mathf.Pow(Input.GetAxis("Vertical"), 2));

        targetRot = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        //if(targetRot.normalized == -currentRot.normalized) currentRot = Vector3.RotateTowards(currentRot, new Vector3(currentRot.y, -currentRot.x), turnSpeed * moveMagnitude, 0);
        /*else*/ currentRot = Vector3.RotateTowards(currentRot, targetRot, turnSpeed * moveMagnitude, 0);

        //moveDirection = new Vector2(currentRot.normalized.x * moveSpeed, currentRot.normalized.y * moveSpeed) * moveMagnitude;

        if (castUp() > 0 && moveDirection.y > 0) moveDirection = new Vector2(moveDirection.x, 0);
        if (castDown() > 0 && moveDirection.y < 0) moveDirection = new Vector2(moveDirection.x, 0);
        if (castLeft() > 0 && moveDirection.x < 0) moveDirection = new Vector2(0, moveDirection.y);
        if (castRight() > 0 && moveDirection.x > 0) moveDirection = new Vector2(0, moveDirection.y);
        
        transform.Translate(moveDirection, Space.World);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, currentRot);
        prefab.SendMessage("Move", new Vector4(currentRot.x, currentRot.y, currentRot.z, moveMagnitude));
    }

    void SpawnBody()
    {
        prefab = Instantiate(body, this.transform);
        if (segNum - 1 > 0) prefab.SendMessage("SpawnBody", segNum - 1);
    }

    int castUp()
    {
        return coll.Cast(Vector2.up, contactFilter, results, 0.01f);
    }

    int castDown()
    {
        return coll.Cast(Vector2.down, contactFilter, results, 0.01f);
    }

    int castLeft()
    {
        return coll.Cast(Vector2.left, contactFilter, results, 0.01f);
    }

    int castRight()
    {
        return coll.Cast(Vector2.right, contactFilter, results, 0.01f);
    }
}
