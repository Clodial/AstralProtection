using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Transform player;
    private int direction = -1;
    private Vector2 moveDirection;
    public Vector3 destination;
    private float moveSpeed = 0.05f;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (direction == -1)
        {
            if (player.position.y > transform.position.y + 6)
            {
                direction = 0;
                moveDirection = new Vector2(0, moveSpeed * 3);
                destination = new Vector3(transform.position.x, transform.position.y + 12, transform.position.z);
                player.SendMessage("MoveCamera", direction);
            }
            else if (player.position.x > transform.position.x + 8)
            {
                direction = 1;
                moveDirection = new Vector2(moveSpeed * 4, 0);
                destination = new Vector3(transform.position.x + 16, transform.position.y, transform.position.z);
                player.SendMessage("MoveCamera", direction);
            }
            else if (player.position.y < transform.position.y - 6)
            {
                direction = 2;
                moveDirection = new Vector2(0, -moveSpeed * 3);
                destination = new Vector3(transform.position.x, transform.position.y - 12, transform.position.z);
                player.SendMessage("MoveCamera", direction);
            }
            else if (player.position.x < transform.position.x - 8)
            {
                direction = 3;
                moveDirection = new Vector2(-moveSpeed * 4, 0);
                destination = new Vector3(transform.position.x - 16, transform.position.y, transform.position.z);
                player.SendMessage("MoveCamera", direction);
            }
        }
        else
        {
            switch (direction)
            {
                case 0:
                    if (transform.position.y + (moveSpeed * 3) > destination.y) stopCamera();
                    break;
                case 1:
                    if (transform.position.x + (moveSpeed * 4) > destination.x) stopCamera();
                    break;
                case 2:
                    if (transform.position.y - (moveSpeed * 3) < destination.y) stopCamera();
                    break;
                case 3:
                    if (transform.position.x - (moveSpeed * 4) < destination.x) stopCamera();
                    break;
            }

            transform.Translate(moveDirection);
        }
	}

    void stopCamera()
    {
        transform.position = destination;
        moveDirection = Vector2.zero;
        direction = -1;
        player.SendMessage("MoveCamera", direction);
    }
}
