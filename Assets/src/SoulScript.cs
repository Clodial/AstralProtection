using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulScript : MonoBehaviour
{
    private float moveSpeed = 0.1f;
    private Vector2 moveDirection;
    private int direction = 2;          /*0 = Up    1 = Right   2 = Down     3 = Left*/

    private float spawnTime;
    private GameObject player;

    private Collider2D coll;
    private RaycastHit2D[] results;
    private Animator anim;
    private ContactFilter2D filter;
    private ContactFilter2D contactFilter;
    private Collider2D[] overlapResults;
    private int overlapCt = 0;

    // Use this for initialization
    void Start()
    {
        spawnTime = Time.time;
        player = GameObject.FindGameObjectWithTag("Player");
        results = new RaycastHit2D[10];
        overlapResults = new Collider2D[10];

        coll = this.GetComponent<Collider2D>();
        anim = this.GetComponent<Animator>();
        filter.NoFilter();
        contactFilter.useTriggers = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time - spawnTime >= 0.5f)
        {
            moveDirection = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, Input.GetAxis("Vertical") * moveSpeed);
            
            if (castUp() > 0 && moveDirection.y > 0) moveDirection = new Vector2(moveDirection.x, 0);
            if (castDown() > 0 && moveDirection.y < 0) moveDirection = new Vector2(moveDirection.x, 0);
            if (castLeft() > 0 && moveDirection.x < 0) moveDirection = new Vector2(0, moveDirection.y);
            if (castRight() > 0 && moveDirection.x > 0) moveDirection = new Vector2(0, moveDirection.y);

            if (Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Abs(Input.GetAxis("Vertical")))
            {
                if (Input.GetAxis("Horizontal") > 0) direction = 1;
                else direction = 3;
            }
            else
            {
                if (Input.GetAxis("Vertical") > 0) direction = 0;
                else if (Input.GetAxis("Vertical") < 0) direction = 2;
            }            
        }
        else
        {
            moveDirection *= 0.9f;
        }

        if (anim.GetInteger("Direction") != direction) anim.SetInteger("Direction", direction);

        transform.Translate(moveDirection);
    }

    void Update()
    {
        if (Time.time - spawnTime >= 1)
        {
            overlapCt = coll.OverlapCollider(filter, overlapResults);
            if (Input.GetButtonDown("Fire1") && overlapCt > 0)
            {
                for (int i = 0; i < overlapCt; i++)
                {
                    if (overlapResults[i].gameObject.layer == LayerMask.NameToLayer("Possess"))
                    {
                        overlapResults[i].SendMessage("Possess");
                        player.SendMessage("AddToPool", overlapResults[i].gameObject.transform);
                        Possess(gameObject);
                        i = 11;
                    }
                }
            }
            //if(Input.GetButtonDown("Fire2"))
        }
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

    void SetDirection(int i)
    {
        direction = i;

        switch(i)
        {
            case 0:
                moveDirection = new Vector2(0, moveSpeed);
                break;
            case 1:
                moveDirection = new Vector2(moveSpeed, 0);
                break;
            case 2:
                moveDirection = new Vector2(0, -moveSpeed);
                break;
            case 3:
            default:
                moveDirection = new Vector2(-moveSpeed, 0);
                break;
        }
    }

    void Possess(GameObject n)
    {

    }
}
