using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRPG : MonoBehaviour
{
    public int attackDamage = 5;
    int damageModifier;
    public float attackInterval = 1f;
    private float timer;
    private bool isAttackReady = true;

    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileForce = 1500f;
    public bool hasAmmo = true;

    public TextMeshProUGUI healthCounter;
    public TextMeshProUGUI ammoCounter;
    public Image attackReadyImage;

    public int ammoNumb = 20;
    public int ammoCap = 20;
    public float playerHealth = 100;

    void Start()
    {
        
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
            ammoNumb--;

            if (ammoNumb <= 0)
            {
                ammoNumb = 0;
                hasAmmo = false;
            }

            UiUpdate();

            if(hasAmmo)
            {
                GameObject go = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                go.GetComponent<Rigidbody>().AddForce(go.transform.forward * projectileForce);
            }
        }

        if(Input.GetKey(KeyCode.R))
        {
            ammoNumb = ammoCap;
            hasAmmo = true;
            UiUpdate();
        }
    }

    void UiUpdate()
    { 
        ammoCounter.text = ammoNumb.ToString() + "/" + ammoCap;
        healthCounter.text = playerHealth.ToString();
    }

    //all of this has to be moved to other parent/child power up scripts
    public void HealthBoost()
    {
        Debug.Log("Health Boost");
        playerHealth += 50;
        UiUpdate();
    }

    public void AmmoBoost()
    {
        Debug.Log("ammo boost");
        ammoCap += 5;
        UiUpdate();
    }

    public void DamageBoost()
    {
        Debug.Log("Damage Boost");
        attackDamage += 5;
    }


    public void Attack(BaseEnemy enemy)
    {
        damageModifier = Random.Range(0, 4);
        Debug.Log("Player Damage mod: " + damageModifier);
        enemy.TakeDamage(attackDamage + damageModifier);
        Debug.Log("Player Damage Total: " + attackDamage + damageModifier);
        isAttackReady = false;
        attackReadyImage.gameObject.SetActive(isAttackReady);
    }

    public void TakeDamage(float damage)
    {
        playerHealth -= damage;
        Debug.Log("Player health: " + playerHealth);
        UiUpdate();

        if (playerHealth <= 0)
        {
            //player death screen + restart button
            Debug.Log("YOU DIED");
        }
    }
}
