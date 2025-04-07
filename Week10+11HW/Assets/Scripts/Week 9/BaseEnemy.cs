using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public int health;
    public int attackDamage;
    public int attackRange;

    public int attackSpeed;

    private float attackTimer;

    protected PlayerRPG player;
    
    //add all the nav mesh stuff here


    //this is where my own damage nonsense begins
    public int damageModMin;
    public int damageModMax;
    public int damageTotal;

    public AudioSource attackSound;
    public AudioSource damageSound;
    public AudioSource deathSound;



    

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = FindAnyObjectByType<PlayerRPG>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(Vector3.Distance(this.transform.position, player.transform.position) < attackRange)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer >= attackSpeed)
            {
                Attack();
                attackTimer = 0f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            health -= 2;
            //damageSound.Play();
            Debug.Log("2 Damage");
            Destroy(collision.gameObject);

            if (health <= 0f)
            {
                Destroy(this.gameObject);
                //deathSound.Play();
            }
        }
    }

    public virtual void Attack()
    {
        int damageModifier = Random.Range(damageModMin, damageModMax);
        damageTotal = attackDamage + damageModifier;
        player.TakeDamage(attackDamage + damageModifier);
        //attackSound.Play();
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        //damageSound.Play();

        if (health <= 0f)
        {
            Destroy(this.gameObject);
        }
    }
}
