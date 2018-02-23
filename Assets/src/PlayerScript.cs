using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public int maxHP;
    public int curHP;
    public Slider healthBar;
    private float hitstun;
    private float invuln;  

    private float moveSpeed = 0.1f;
    private Vector2 moveDirection;
    private int direction = 2;          /*0 = Up    1 = Right   2 = Down     3 = Left*/

    private bool haveSoul = true;
    public Transform soul; 
    private Transform prefab;

    private Collider2D coll;
    private RaycastHit2D[] results;
    private Animator anim;
    private ContactFilter2D contactFilter;
    private ContactFilter2D overlapFilter;
    private Collider2D[] overlapResults;
    private int overlapCt = 0;

    // Use this for initialization
    void Start ()
    {
        curHP = maxHP;
        hitstun = Time.time;
        invuln = Time.time;

        results = new RaycastHit2D[10];
        contactFilter.useTriggers = false;
        overlapFilter.NoFilter();
        overlapResults = new Collider2D[10];

        coll = this.GetComponent<Collider2D>();
        anim = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (haveSoul && hitstun <= Time.time)
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
        else
        {
            if (hitstun > Time.time) moveDirection *= 0.9f;
        }

        overlapCt = coll.OverlapCollider(overlapFilter, overlapResults);
        if (overlapCt > 0)
        {
            for (int i = 0; i < overlapCt; i++)
            {
                if (overlapResults[i].gameObject.tag == "Enemy" || overlapResults[i].gameObject.tag == "enHit" || overlapResults[i].gameObject.tag == "Boss")
                {
                    playerHit(overlapResults[i].gameObject);
                }
                else if (overlapResults[i].gameObject.tag == "Health")
                {
                    heal();
                }
            }
        }

        if (healthBar.value != curHP) healthBar.value = curHP;

        transform.Translate(moveDirection);
    }

    void Update()
    {
        if (haveSoul)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                prefab = Instantiate(soul, transform.position, transform.rotation);
                prefab.SendMessage("SetDirection", direction);
                haveSoul = false;
                moveDirection = Vector2.zero;
            }
        }
    }

    void playerHit(GameObject other)
    {
        int curHp = getCurHp();

        if (invuln <= Time.time)
        {
            if (other.tag.Equals("Enemy")) curHp -= 15;
            if (other.tag.Equals("enHit")) curHp -= 10;
            if (other.tag.Equals("Boss")) curHp -= 20; 

            if (getCurHp() <= 0)
            {
                gameOver();
            }
            else
            {
                setCurHp(curHp);
                hitstun = Time.time + 1;
                invuln = Time.time + 2;
                moveDirection.Set(transform.position.x - other.transform.position.x, transform.position.y - other.transform.position.y);
                moveDirection.Normalize();
                moveDirection *= moveSpeed;
            }
        }
    }

    void setCurHp(int hp)
    {
        this.curHP = hp;
    }

    int getCurHp()
    {
        return this.curHP;
    }

    void gameOver()
    {
        print("Game Over");
        SceneManager.LoadScene(0);
    }

    void heal()
    {
        int curHp = getCurHp();
        if (this.maxHP - curHp <= 25)
        {
            setCurHp(this.maxHP);
        }
        else
        {
            curHp = curHp + 25;
            setCurHp(curHp);
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
    }

    void MoveCamera(int i)
    {
        if (i >= 0)
        {
            haveSoul = false;
            switch (i)
            {
                case 0:
                    moveDirection = Vector2.up * 0.01f;
                    break;
                case 1:
                    moveDirection = Vector2.right * 0.01f;
                    break;
                case 2:
                    moveDirection = Vector2.down * 0.01f;
                    break;
                case 3:
                    moveDirection = Vector2.left * 0.01f;
                    break;
            }
        }
        else
        {
            haveSoul = true;
            moveDirection = Vector2.zero;
        }
    }
}
