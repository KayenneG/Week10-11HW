using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public float health = 100f;
    public float speed = 3f;
    public int attackDamage;
    public int damageModMin;
    public int damageModMax;
    public int damageTotal;
    public float attackRange;

    public AudioSource attackSound;
    public AudioSource damageSound;
    public AudioSource deathSound;

    private float timer = 0f;

    [SerializeField] protected float attackInterval = 1f;

    private PlayerRPG player;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRPG>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(Vector3.Distance(this.transform.position, player.transform.position) < attackRange)
        {
            timer += Time.deltaTime;
            if(timer >= attackInterval)
            {
                Attack();
                timer = 0f;
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

    public virtual void Move()
    {
        
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        //damageSound.Play();

        if (health <= 0f)
        {
            Destroy(this.gameObject);
        }
    }
}
