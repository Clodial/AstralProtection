using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public string type;
    public int maxHp;
    private int curHp;

    private float moveSpeed = 0.04f;
    public Vector2 moveDirection;
    public Vector2 desiredDirection;
    private float turnSpeed = 0.1f;
    private int direction = 2;          /*0 = Up    1 = Right   2 = Down     3 = Left*/

    private Transform player;
    private Vector2[] distances;

    private bool haveSoul = false;
    public Transform soul;
    public Transform target;
    public Transform plTarget;
    public Transform attackEn;
    public Transform attackPl;
    private Transform prefab;
    private float jumpTime;

    private Collider2D coll;
    private RaycastHit2D[] results;
    private Animator anim;
    private ContactFilter2D contactFilter;
    public string scene;

    // Use this for initialization
    void Start()
    {
        curHp = maxHp;

        distances = new Vector2[4];

        results = new RaycastHit2D[10];
        contactFilter.useTriggers = false;
        jumpTime = Time.time;

        coll = this.GetComponent<Collider2D>();
        anim = this.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (haveSoul)
        {
            if (jumpTime <= Time.time)
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

                if (anim.GetInteger("Direction") != direction) anim.SetInteger("Direction", direction);
            }
            else moveDirection *= 0.9f;
        }
        else
        {
            if (jumpTime <= Time.time)
            {
                player = GameObject.Find("Player").transform;
                distances[0].Set(player.position.x - transform.position.x, (player.position.y - 2.25f) - transform.position.y);
                distances[1].Set((player.position.x - 2.25f) - transform.position.x, player.position.y - transform.position.y);
                distances[2].Set(player.position.x - transform.position.x, (player.position.y + 2.25f) - transform.position.y);
                distances[3].Set((player.position.x + 2.25f) - transform.position.x, player.position.y - transform.position.y);
                
                for (int i = 0; i < 4; i++)
                {
                    if (i == 0 || distances[i].magnitude < desiredDirection.magnitude)
                    {
                        desiredDirection.Set(distances[i].x, distances[i].y);
                        if (distances[i].magnitude <= 0.1f)
                        {
                            if (anim.GetInteger("Direction") != i) anim.SetInteger("Direction", i);
                            jumpAttack(i);
                            i = 5;
                        }
                    }
                }

                if (!anim.GetBool("Jump"))
                {
                    desiredDirection.Normalize();
                    desiredDirection *= moveSpeed;
                    moveDirection = Vector3.RotateTowards(moveDirection, desiredDirection, turnSpeed, moveSpeed);

                    if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
                    {
                        if (moveDirection.x > 0) direction = 1;
                        else direction = 3;
                    }
                    else
                    {
                        if (moveDirection.y > 0) direction = 0;
                        else if (moveDirection.y < 0) direction = 2;
                    }

                    if (anim.GetInteger("Direction") != direction) anim.SetInteger("Direction", direction);
                }
            }
            else moveDirection *= 0.9f;
        }

        transform.Translate(moveDirection);
    }

    void Update()
    {
        if (haveSoul && jumpTime <= Time.time)
        {
            if(Input.GetButtonDown("Fire1"))    /*Jump Attack*/
            {
                jumpAttack(direction);
            }

            if (Input.GetButtonDown("Fire2"))
            {
                prefab = Instantiate(soul, transform.position, transform.rotation);
                prefab.SendMessage("SetDirection", direction);
                haveSoul = false;
                this.gameObject.tag = "Enemy";
                moveDirection = Vector2.zero;
            }
        }
    }

    void jumpAttack(int i)
    {
        anim.SetBool("Jump", true);
        coll.enabled = false;

        if (haveSoul) jumpTime = Time.time + 1;
        else jumpTime = Time.time + 3;

        switch (i)
        {
            case 0:
                moveDirection = Vector2.up * 0.25f;
                break;
            case 1:
                moveDirection = Vector2.right * 0.25f;
                break;
            case 2:
                moveDirection = Vector2.down * 0.25f;
                break;
            case 3:
                moveDirection = Vector2.left * 0.25f;
                break;
        }
        if(this.gameObject.tag == "possessed"){
            prefab = Instantiate(plTarget, transform.position + (new Vector3(moveDirection.x, moveDirection.y, 0) * 8), transform.rotation);
        }else{
            prefab = Instantiate(target, transform.position + (new Vector3(moveDirection.x, moveDirection.y, 0) * 8), transform.rotation);
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

    void Possess()
    {
        haveSoul = true;
        this.transform.gameObject.tag = "possessed";
    }

    void Land()
    {
        if(this.gameObject.tag == "Enemy"){
            Instantiate(attackEn, transform.position + Vector3.up, transform.rotation);
            Instantiate(attackEn, transform.position + Vector3.down, transform.rotation);
            Instantiate(attackEn, transform.position + Vector3.left, transform.rotation);
            Instantiate(attackEn, transform.position + Vector3.right, transform.rotation);
        }else if(this.gameObject.tag == "possessed"){
            Instantiate(attackPl, transform.position + Vector3.up, transform.rotation);
            Instantiate(attackPl, transform.position + Vector3.down, transform.rotation);
            Instantiate(attackPl, transform.position + Vector3.left, transform.rotation);
            Instantiate(attackPl, transform.position + Vector3.right, transform.rotation);
        }

        moveDirection = Vector2.zero;
        coll.enabled = true;
    }
}
