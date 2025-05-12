using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{
    //basic attack variables
    public int health;
    public int attackDamage;
    public int attackRange;

    public float attackSpeed;

    protected float attackTimer;

    protected PlayerRPG player;
    protected EnemyManager enemyManager;

    //All the nav nonsense
    protected NavMeshAgent navAgent;

    [SerializeField]
    protected float aggroRange = 30f;

    protected bool hasSeenPlayer = false;

    [SerializeField]
    protected List<Transform> patrolPoints = new List<Transform>();
    protected int patrolPointIndex = 0;

    //this is where my own damage nonsense begins
    public int damageModMin;
    public int damageModMax;

    public AudioSource attackSound;
    public AudioSource damageSound;
    public AudioSource deathSound;

    public AudioSource fail;

    //more stuff
    public int rngEnm = 3;
    public int chsEnm = 2;
    public int slmEnm = 9;

    public GameObject powerUp;



    protected virtual void Start()
    {
        player = FindAnyObjectByType<PlayerRPG>();
        enemyManager = FindAnyObjectByType<EnemyManager>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.SetDestination(patrolPoints[patrolPointIndex].position);
    }

    protected virtual void Update()
    {
        //If enemy has seen the player
        if (hasSeenPlayer == true)
        {
            //and it is close enough to the player
            if (navAgent.remainingDistance < 0.5f)
            {
                if (Vector3.Distance(this.transform.position, player.transform.position) > aggroRange)//if the distane between the enemy and player is not wihin the aggro range
                {
                    hasSeenPlayer = false;//it has not seen the player
                }
                else//if it is within aggro range
                {
                    if (IsPlayerInLOS() == true)//if the player is in lOS
                    {
                        navAgent.SetDestination(player.transform.position);//the enemy will go to the player position
                    }
                    else//if the player is within range but IS NOT in los
                    {
                        hasSeenPlayer = false;//it does not see the player
                        navAgent.isStopped = false;//it continues to move
                    }
                }
            }

            if (Vector3.Distance(this.transform.position, player.transform.position) > attackRange)//if the distance between the enemy and player is not within the attack range
            {
                if (IsPlayerInLOS() == true)//and the player is within LOS
                {
                    navAgent.SetDestination(player.transform.position);//the enemy will go toward the player
                    navAgent.isStopped = false; //it continues to move
                }
            }
            else//if the distance between player and enimy is within attack range
            {
                if (IsPlayerInLOS() == true)//and the player is within LOS
                {
                    navAgent.isStopped = true;//and the player is in LOS
                    this.transform.LookAt(player.transform.position);//keep looking at the player

                    Hit();
                }
                else//the player is in atack range, but not in LOS
                {
                    navAgent.isStopped = false;
                }
            }
        }
        else//if the player has not been seen
        {
            if (navAgent.remainingDistance < 0.5f)//if it reaches its destination
            {
                patrolPointIndex++;//increase the pp index

                if (patrolPointIndex >= patrolPoints.Count)//if the patrol point index is out of range
                {
                    patrolPointIndex = 0;//reset it to 0 so it will go back to the first point
                }
                navAgent.SetDestination(patrolPoints[patrolPointIndex].position);//set destination to current pp index point
                navAgent.isStopped = false;
            }
        }
    }

    //LINE OF SIGHT CHECK
    protected bool IsPlayerInLOS()
    {
        RaycastHit hit;

        Vector3 dir = player.transform.position - this.transform.position;
        dir.Normalize();

        if (Physics.Raycast(this.transform.position, dir, out hit))
        {
            if (hit.collider.tag == "Player")
            {
                return true;
            }
            if(hit.collider.tag == "Door")
            {
                navAgent.SetDestination(patrolPoints[patrolPointIndex].position);
                hasSeenPlayer = false;
                navAgent.isStopped = false;
                return false;
            }
        }
        return false;
    }
    
    //HIT CHECK
    protected virtual void Hit()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer > attackSpeed)
        {
            Attack();
            attackTimer = 0;
        }
    }
    
    //ATTACK
    protected virtual void Attack()
    {
        int damageModifier = Random.Range(damageModMin, damageModMax);
        int damageTotal = attackDamage + damageModifier;
        player.TakeDamage(attackDamage + damageModifier);
        Debug.Log(this.gameObject.name + " deals " + damageTotal + " damage");
        //attackSound.Play();
    }
    
    //TAKE DAMAGE (int is either from on colision enter void call, or player ATTACK (attackDamage + damageModifier)
    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        damageSound.Play();
        Debug.Log(this.gameObject.name + " takes " + damage);

        if (health <= 0)
        {
            if(this.gameObject.tag == "Chase")
            {
                enemyManager.Chase();
            }
            if (this.gameObject.tag == "Range")
            {
                enemyManager.Range();
            }
            if (this.gameObject.tag == "Shroom")
            {
                Debug.Log("Shroom Destroy Call1");
                enemyManager. Mushroom();
                Debug.Log("Shroom Destroy Call2");
            }

            GameObject go = Instantiate(powerUp, this.transform.position, this.transform.rotation);
            Destroy(this.gameObject);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            if(this.gameObject.CompareTag("Shroom"))
            {
                fail.Play();
            }
            else
            {
                hasSeenPlayer = true;
                TakeDamage(5);
            }
        }
    }


    

   

    public void SeePlayer()
    {
        if (IsPlayerInLOS() == true)
        {
            hasSeenPlayer = true;
        }
    }
}
