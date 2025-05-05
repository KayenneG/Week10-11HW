using System.Collections;
using TMPro;
using UnityEditor.Rendering;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRPG : MonoBehaviour
{
    #region PLAYER STATS + TIMERS
    public float health = 100;
    public float maxHealth = 100;
    public int attackDamage = 3;
    int damageModifier;
    public int reloadNumb = 1;
    public int ammoNumb = 20;
    private int ammoCap = 20;
    bool isOnFloor;

    //mele
    public float meleAttackInterval = 2f;
    private float meleAttackTimer = 2;
    private bool isMeleAttackReady = true;

    //ranged
    public bool hasAmmo = true;
    private float rangedAttackInterval = 0.5f;
    private float rangedAttackTimer = 0.5f;
    private bool isRangedAttackReady = true;
    #endregion

    #region UI
    public TextMeshProUGUI healthCounter;
    public TextMeshProUGUI ammoCounter;
    public TextMeshProUGUI reloadCounter;
    public Image healthBar;
    public Image sprintBar;
    public Image knifeBar;
    public Image boltBar;

    public GameObject damVinParent;
    public GameObject damageVin;
    Coroutine ouch;

    public Image tabList;
    Coroutine goOut;
    
    public GameObject deathScreen;
    #endregion

    #region ptojrctile variables
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileForce = 1500f;
    Coroutine projectile;
    #endregion

    public FirstPersonController FirstPersonController;
    private float maxSprint = 5;
    private float sprintRemaining;
    Coroutine sprintRecharge;
    private bool canRecharge;
    private bool isRecharging;

    //you still have to add these in
    public AudioSource attack1Sound;
    public AudioSource attack2Sound;
    public AudioSource deathSound;

    

    void Start()
    {
        canRecharge = false;
        isRecharging = false;
        sprintRemaining = maxSprint;
        UiUpdate();
    }

    void Update()
    {
        #region ATTACK TIMERS + SLIDERS
        if (isMeleAttackReady == false)
        {
            meleAttackTimer += Time.deltaTime;
            UiUpdate();

            if (meleAttackTimer >= meleAttackInterval)
            {
                isMeleAttackReady = true;
                meleAttackTimer = 2f;
            }
        }
        if (isRangedAttackReady == false)
        {
            rangedAttackTimer += Time.deltaTime;
            UiUpdate();

            if (rangedAttackTimer >= rangedAttackInterval)
            {
                isRangedAttackReady = true;
                rangedAttackTimer = 0.5f;
            }
        }
        #endregion

        # region MELE ATTACK CHECK
        if (Input.GetMouseButtonDown(0))
        {
            if (isMeleAttackReady == true)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3f))
                {
                    attack1Sound.Play();
                    BaseEnemy enemy = hit.collider.GetComponent<BaseEnemy>();

                    if (enemy != null)
                    {
                        meleAttackTimer = 0;
                        Attack(enemy);
                    }
                }
            }
        }
        #endregion

        #region Range Attack
        if (Input.GetMouseButtonDown(1))
        {
            if (isRangedAttackReady == true)
            {
                if (ammoNumb >= 1)
                {
                    attack2Sound.Play();
                    ammoNumb--;
                    rangedAttackTimer = 0;
                    projectile = StartCoroutine(Fire());
                }
            }
        }
        #endregion

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

        if (FirstPersonController != null)
        {
            bool isSprinting = FirstPersonController.isSprinting;
            bool enableSprint = FirstPersonController.enableSprint;

            if(isRecharging)
            {
                canRecharge = false;
                UiUpdate();
            }
            if (isSprinting && enableSprint)
            {
                isRecharging = false;
                canRecharge = false;

                if(sprintRemaining > 0)
                {
                    if(sprintRecharge != null)
                    {
                        StopCoroutine(sprintRecharge);
                    }

                    sprintRemaining -= Time.deltaTime;
                    UiUpdate();
                }
                if(sprintRemaining <= 0)
                {
                    FirstPersonController.enableSprint = false;
                    sprintRecharge = StartCoroutine(SprintRecharge());
                    sprintBar.GetComponent<Image>().color = Color.grey;
                }
            }
            if(!isSprinting && !isRecharging)
            {
                if(sprintRemaining < 5)
                {
                    canRecharge = true;
                }
                else
                {
                    canRecharge = false;
                }

                if (canRecharge)
                {
                    isRecharging = true;
                    sprintRecharge = StartCoroutine(SprintRecharge());
                }
                
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
        isMeleAttackReady = false;
        knifeBar.fillAmount = 0;
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
        isRangedAttackReady = false;
        GameObject go = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

        go.GetComponent<Rigidbody>().AddForce(go.transform.forward * projectileForce);
        //play atk sound
        UiUpdate();

        yield return new WaitForSeconds(2f);

        Destroy(go);

        yield return null;
    }

    IEnumerator SprintRecharge()
    {
        canRecharge = false;
        while (sprintRemaining < maxSprint)
        {
            sprintRemaining += 0.5f * Time.deltaTime;
            UiUpdate();
            yield return null;
        }
        sprintBar.GetComponent<Image>().color = Color.white;
        FirstPersonController.enableSprint = true;
    }

    void UiUpdate()
    { 
        ammoCounter.text = ammoNumb.ToString() + "/" + ammoCap;
        healthCounter.text = health.ToString() + "/" + maxHealth;
        reloadCounter.text = reloadNumb.ToString();
        healthBar.fillAmount = health / maxHealth;
        sprintBar.fillAmount = sprintRemaining / maxSprint;
        knifeBar.fillAmount = meleAttackTimer / meleAttackInterval;
        boltBar.fillAmount = rangedAttackTimer / rangedAttackInterval;
    }

    public void HealthBoost()
    {
        Debug.Log("Health Boost");
        health += 50;
        if(health > maxHealth)
        {
            maxHealth += (health - maxHealth);
        }
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
