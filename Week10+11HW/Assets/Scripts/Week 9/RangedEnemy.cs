using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : BaseEnemy
{
    public GameObject projectilePrefab;

    public Transform projectileSpawnPosition;

    public float projectileForce = 500f;

    Coroutine co;

    protected override void Attack()
    {
        co = StartCoroutine(Fire());
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    IEnumerator Fire()
    {
        GameObject go = Instantiate(projectilePrefab, projectileSpawnPosition.position, projectileSpawnPosition.rotation);

        go.GetComponent<Rigidbody>().AddForce(go.transform.forward * projectileForce);

        yield return new WaitForSeconds(2f);

        Destroy(go);

        yield return null;
    }
}
