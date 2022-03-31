using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHandler : MonoBehaviour
{
    public float moveSpeed = 25.0f;
    public float projectileDamage = 4.0f;
    public float projectileKnockback = 0.0f;
    public float range = 15.0f;
    public Vector3 direction;
    public GameObject destroyPrefab;

    void Start()
    {
        Invoke("DestroyProjectile", 1.0f);
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
        else if (collision.gameObject.CompareTag("Player"))
        {
            //collision.gameObject.GetComponent<PlayerController>().TakeDamage(projectileDamage);
            DestroyProjectile();
        }

    }

    void DestroyProjectile()
    {
        GameObject explosion = Instantiate(destroyPrefab, transform.position, transform.rotation);
        explosion.GetComponent<ParticleSystem>().Play();
        Destroy(gameObject);
    }
}
