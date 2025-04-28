using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRPG : MonoBehaviour
{
    public float health = 100;

    public int attackDamage = 3;
    int damageModifier;
    public float attackInterval = 1f;
    private float timer;
    private bool isAttackReady = true;

    public bool isOnFloor;

    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileForce = 1500f;
    public bool hasAmmo = true;

    public TextMeshProUGUI healthCounter;
    public TextMeshProUGUI ammoCounter;
    public TextMeshProUGUI reloadCounter;
    public Image attackReadyImage;
    public Image damageVin;

    //you still have to add these in
    public AudioSource attack1Sound;
    public AudioSource attack2Sound;
    public AudioSource damageSound;
    public AudioSource deathSound;

    public Animation ouch;

    public int reloadNumb = 1;
    public int ammoNumb = 20;
    private int ammoCap = 20;

    Coroutine projectile;

    public GameObject deathScreen;


    void Start()
    {
        UiUpdate();
    }

    void Update()
    {
        if(isAttackReady == false)
        {
            timer += Time.deltaTime;

            if (timer >= attackInterval)
            {
                isAttackReady = true;
                attackReadyImage.gameObject.SetActive(isAttackReady);
                timer = 0f;
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            if(isAttackReady == true)
            {
                
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3f))
                {
                    attack1Sound.Play();
                    BaseEnemy enemy = hit.collider.GetComponent<BaseEnemy>();

                    if (enemy != null)
                    {
                        Attack(enemy);
                    }
                }
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            if (ammoNumb >= 1)
            {
                attack2Sound.Play();
                ammoNumb--;

                projectile = StartCoroutine(Fire());
            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            if(reloadNumb >= 1)
            {
                reloadNumb--;
                ammoNumb = ammoCap;

                UiUpdate();

                
            }
            if(reloadNumb <= 0)
            {
                reloadNumb = 0;
            }
        }
    }

    IEnumerator Fire()
    {
        GameObject go = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

        go.GetComponent<Rigidbody>().AddForce(go.transform.forward * projectileForce);
        //play atk sound
        UiUpdate();

        yield return new WaitForSeconds(2f);

        Destroy(go);

        yield return null;
    }

    void UiUpdate()
    { 
        ammoCounter.text = ammoNumb.ToString() + "/" + ammoCap;
        healthCounter.text = health.ToString();
        reloadCounter.text = reloadNumb.ToString();
    }

    public void HealthBoost()
    {
        Debug.Log("Health Boost");
        health += 50;
        UiUpdate();
    }

    public void AmmoBoost()
    {
        Debug.Log("ammo boost");
        reloadNumb += 1;
        UiUpdate();
    }

    public void DamageBoost()
    {
        Debug.Log("Damage Boost");
        ammoCap += 5;
        UiUpdate();
    }


    public void Attack(BaseEnemy enemy)
    {
        damageModifier = Random.Range(0, 4);
        enemy.TakeDamage(attackDamage + damageModifier);
        Debug.Log("Player does " + (attackDamage + damageModifier) + " damage to " + enemy.gameObject);
        isAttackReady = false;
        attackReadyImage.gameObject.SetActive(isAttackReady);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        ouch.Play("DamageVinFade");
        damageSound.Play();
        Debug.Log("Player health: " + health);
        UiUpdate();

        if (health <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "VisionCone")
        {
            other.GetComponentInParent<BaseEnemy>().SeePlayer();
        }

        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("PainBall"))
        {
            health -= 5;
            ouch.Play("DamageVinFade");
            damageSound.Play();
            Debug.Log("player takes 5 Damage");
            Destroy(other.gameObject);
            UiUpdate();

            if (health <= 0)
            {
                Die();
            }
        }

        if (other.gameObject.CompareTag("Floor"))
        {
            isOnFloor = true;
            attackDamage = 3;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            isOnFloor = false;
            attackDamage = 5;
        }
    }

    public void Die()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        deathScreen.SetActive(true);
        deathSound.Play();
    }
}
