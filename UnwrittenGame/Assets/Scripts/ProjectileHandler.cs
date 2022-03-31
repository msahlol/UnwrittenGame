using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    public float moveSpeed = 25.0f;
    public float projectileDamage = 4.0f;
    public float projectileKnockback = 0.0f;
    public Vector3 direction;
    public Vector3 destroyOffset;
    public GameObject destroyPrefab;

    void Start()
    {
        Invoke("DestroyProjectile", 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            DestroyProjectile();
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyHandler>().TakeDamage(projectileDamage);
            DestroyProjectile();
        }

        if (collision.gameObject.CompareTag("FlyingEnemy"))
        {
            collision.gameObject.GetComponent<FlyingEnemyHandler>().TakeDamage(projectileDamage);
            DestroyProjectile();
        }

    }

    void DestroyProjectile()
    {
        GameObject explosion = Instantiate(destroyPrefab, transform.position + destroyOffset, transform.rotation);
        explosion.GetComponent<ParticleSystem>().Play();
        Destroy(gameObject);
    }
}
