using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRPG : MonoBehaviour
{
    //PLAYER STATS
    public float health = 100;
    public int attackDamage = 3;
    int damageModifier;
    public int reloadNumb = 1;
    public int ammoNumb = 20;
    private int ammoCap = 20;
    public float attackInterval = 1f;
    public bool hasAmmo = true;
    private float attackTimer;
    private bool isAttackReady = true;
    bool isOnFloor;




    //UI
    public TextMeshProUGUI healthCounter;
    public TextMeshProUGUI ammoCounter;
    public TextMeshProUGUI reloadCounter;
    public Image attackReadyImage;
    
    public GameObject damVinParent;
    public GameObject damageVin;
    Coroutine ouch;

    public Image tabList;
    Coroutine goOut;
    
    public GameObject deathScreen;




    //ptojrctile variables
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileForce = 1500f;
    Coroutine projectile;

    


    //you still have to add these in
    public AudioSource attack1Sound;
    public AudioSource attack2Sound;
    public AudioSource deathSound;

    

    void Start()
    {
        UiUpdate();
    }

    void Update()
    {
        //ATTACK TIMER
        if (isAttackReady == false)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackInterval)
            {
                isAttackReady = true;
                attackReadyImage.gameObject.SetActive(isAttackReady);
                attackTimer = 0f;
            }
        }

        //MELE ATTACK CHECK
        if (Input.GetMouseButtonDown(0))
        {
            if (isAttackReady == true)
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

        if (Input.GetMouseButtonDown(1))
        {
            if (ammoNumb >= 1)
            {
                attack2Sound.Play();
                ammoNumb--;

                projectile = StartCoroutine(Fire());
            }
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            //StopCoroutine(goOut);

            //Debug.Log("TabPressed");
            if (tabList.rectTransform.anchoredPosition.x < 0f)
            {
                tabList.rectTransform.anchoredPosition += Vector2.right * 10000 * Time.deltaTime;
                //Debug.Log("movin'");
            }
            if (tabList.rectTransform.anchoredPosition.x > 0f)
            {
                tabList.rectTransform.anchoredPosition = new Vector2(0, 0);
            }
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            goOut = StartCoroutine(OutHeGoes());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (reloadNumb >= 1)
            {
                reloadNumb--;
                ammoNumb = ammoCap;

                UiUpdate();


            }
            if (reloadNumb <= 0)
            {
                reloadNumb = 0;
            }
        }
    }

    //ATTACK (uses raycast script reference from MELE ATTACK CHECK)
    public void Attack(BaseEnemy enemy)
    {
        damageModifier = Random.Range(0, 4);
        if(isOnFloor == false)
        {
            damageModifier += 3;
        }    
        enemy.TakeDamage(attackDamage + damageModifier);
        Debug.Log("Player does " + (attackDamage + damageModifier) + " damage to " + enemy.gameObject);
        isAttackReady = false;
        attackReadyImage.gameObject.SetActive(isAttackReady);
    }

    IEnumerator OutHeGoes()
    {
        while(tabList.rectTransform.anchoredPosition.x > -541f)
        {
            tabList.rectTransform.anchoredPosition += Vector2.left * 10000 * Time.deltaTime;
            yield return null;
        }
        /*if (tabList.rectTransform.anchoredPosition.x < -541f)
        {
            tabList.rectTransform.anchoredPosition = new Vector2(-541, 0);
        }*/
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


    

    public void TakeDamage(float damage)
    {
        health -= damage;
        ouch = StartCoroutine(Damage());

        if (health <= 0)
        {
            Die();
        }
    }

    IEnumerator Damage()
    {
        GameObject vin = Instantiate(damageVin, damVinParent.transform);
        Debug.Log("Player health: " + health);
        UiUpdate();

        yield return new WaitForSeconds(1f);

        Destroy(vin);
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
            TakeDamage(5f);

            if (health <= 0)
            {
                Die();
            }
        }
        if (other.gameObject.CompareTag("Floor"))
        {
            isOnFloor = true;
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            isOnFloor = false;
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
