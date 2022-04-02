using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyHandler : MonoBehaviour
{
    public float enemyHealth = 10.0f;
    public float enemyDamage = 7.0f;
    public float knockback = 10.0f;
    public float knockbackSpeed = 0.5f;
    public GameObject player;
    public LayerMask groundMask;

    public ParticleSystem burnEffect;
    public ParticleSystem freezeEffect;

    public float enemySpeed = 1.0f;

    public bool flameThrowerCooldown = false;
    public bool iceCloudCooldown = false;

    public float burnTime = 0.0f;
    public float freezeTime = 0.0f;

    private PlayerController pc;
    private Rigidbody rb;
    private Rigidbody prb;
    private FieldOfView fieldOfView;
    private float attackCooldownDelay = 2.0f;
    private float meleeCooldownDelay = 3.0f;
    private float attackAngle = 10.0f;
    public float gravityOffset = -10.0f;
    private bool attackCooldown = false;
    private bool meleeCooldown = false;
    private bool foundPlayer = false;
    private Collider collider;

    public Vector3 spawnLocation;
    public float yOffset = 2.0f;

    public float yVal = 0.0f;
    public float attackAngleOffset = 15.0f;


    public GameObject enemyAttackPrefab;
    public GameObject enemyAttackContactPrefab;

    public GameObject coinPrefab;
    public GameObject healthPrefab;
    public GameObject manaPrefab;


    void Start()
    {
        fieldOfView = gameObject.GetComponent<FieldOfView>();
        rb = gameObject.GetComponent<Rigidbody>();
        pc = player.gameObject.GetComponent<PlayerController>();
        collider = gameObject.GetComponent<Collider>();
        spawnLocation = transform.position;
    }

    void Update()
    {
        if (fieldOfView.playerInView && !meleeCooldown)
        {
            StopCoroutine(LostSight());
            foundPlayer = true;
            Vector3 direction = new Vector3(player.transform.position.x, player.transform.position.y + yOffset, player.transform.position.z) - transform.position;
            Quaternion angle = Quaternion.LookRotation(direction, Vector3.up);

            rb.rotation = angle;
            rb.velocity = new Vector3(direction.x, direction.y, direction.z) * enemySpeed;

            /*
            if (InAttackRadius(direction.normalized) && !attackCooldown)
            {
                GameObject attack = Instantiate(enemyAttackPrefab, transform.position + transform.forward, transform.rotation);
                Instantiate(enemyAttackContactPrefab, transform.position + transform.forward, transform.rotation);
                attack.GetComponent<EnemyAttackHandler>().direction = transform.forward;
                attackCooldown = true;
                StartCoroutine(AttackCooldown());
            }
            */
        }
        else
        {
            if (foundPlayer)
            {
                foundPlayer = false;
                StartCoroutine(LostSight());
            }
        }
    }

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        if (enemyHealth <= 0)
        {
            gameObject.SetActive(false);
            SpawnCoins();
        }

    }

    public void DealDamage()
    {
        pc.health -= enemyDamage;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            meleeCooldown = true;
            DealDamage();
            StartCoroutine(MeleeCooldown());
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Aura"))
        {
            float effect = Random.Range(0.0f, 1.0f);
            AbilityHandler ability = collider.gameObject.GetComponent<AbilityHandler>();
            if ((ability.statusEffectChance * Time.deltaTime) >= effect)
            {
                if (ability.fire)
                {
                    Burn();
                }
                else
                {
                    Freeze();
                }
            }
        }
        else if (collider.gameObject.CompareTag("Attack"))
        {
            AbilityHandler ability = collider.gameObject.GetComponent<AbilityHandler>();
            if (ability.fire && !flameThrowerCooldown)
            {
                TakeDamage(ability.damage);
                StartCoroutine(FlameThrowerCooldown());
            }
            else if (!ability.fire)
            {
                TakeDamage(ability.damage);
            }

            float effect = Random.Range(0.0f, 1.0f);
            if ((ability.statusEffectChance * Time.deltaTime) >= effect)
            {
                if (ability.fire)
                {
                    Burn();
                }
                else
                {
                    Freeze();
                }
            }
        }
        else if (collider.gameObject.CompareTag("Ultimate"))
        {
            float effect = Random.Range(0.0f, 1.0f);
            AbilityHandler ability = collider.gameObject.GetComponent<AbilityHandler>();
            if ((ability.statusEffectChance * Time.deltaTime) >= effect)
            {
                if (!ability.fire)
                {
                    Freeze();
                }
            }
            if (!ability.fire && !iceCloudCooldown)
            {
                StartCoroutine(IceCloudCooldown());
                TakeDamage(ability.damage);
            }
        }
        else if (collider.gameObject.CompareTag("Projectile"))
        {
            float effect = Random.Range(0.0f, 1.0f);
            AbilityHandler ability = collider.gameObject.GetComponent<AbilityHandler>();
            if ((ability.statusEffectChance * Time.deltaTime) >= effect)
            {
                if (ability.fire)
                {
                    Burn();
                }
                else
                {
                    Freeze();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Ultimate"))
        {
            AbilityHandler ability = collider.gameObject.GetComponent<AbilityHandler>();
            if (ability.fire)
            {
                TakeDamage(ability.damage);
            }
        }
    }

    bool InAttackRadius(Vector3 directionToTarget)
    {
        return Vector3.Angle(transform.forward, directionToTarget) < attackAngle / 2;
    }

    private IEnumerator AttackCooldown()
    {
        if (attackCooldown)
        {
            WaitForSeconds wait = new WaitForSeconds(attackCooldownDelay);
            yield return wait;
            attackCooldown = false;
        }
    }

    private IEnumerator LostSight()
    {
        Vector3 playerDirection = player.transform.position - transform.position;
        LookAt2D(playerDirection);
        rb.velocity = Vector3.zero;
        WaitForSeconds wait = new WaitForSeconds(3.0f);
        yield return wait;
        Vector3 spawnDirection = spawnLocation - transform.position;
        LookAt2D(spawnDirection);
        //rb.velocity = spawnDirection;
        //GoToSpawn();
    }

    private void FollowPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        LookAt3D(direction);
        rb.velocity = new Vector3(direction.x, gravityOffset, direction.z);
    }

    private void LookAt3D(Vector3 direction)
    {
        Quaternion angle = Quaternion.LookRotation(direction, Vector3.up);
        rb.rotation = angle;
    }

    private void LookAt2D(Vector3 direction)
    {
        LookAt3D(direction);
        Vector3 rotation = Quaternion.LookRotation(direction).eulerAngles;
        rotation.x = 0.0f;
        rotation.z = 0.0f;
        transform.rotation = Quaternion.Euler(rotation);
    }

    private IEnumerator MeleeCooldown()
    {
        rb.AddForce(transform.forward * -1 * knockback, ForceMode.Impulse);
        WaitForSeconds wait = new WaitForSeconds(knockbackSpeed);
        yield return wait;
        rb.velocity = Vector3.zero;
        collider.isTrigger = true;
        WaitForSeconds waitAgain = new WaitForSeconds(3.0f);
        yield return waitAgain;
        collider.isTrigger = false;
        meleeCooldown = false;
    }

    private void SpawnCoins()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, -Vector3.up, out hit, groundMask);

        Instantiate(manaPrefab, new Vector3(transform.position.x - 1.0f, transform.position.y, transform.position.z + 3f), Quaternion.identity * Quaternion.Euler(90, 0, 0));
        Instantiate(healthPrefab, new Vector3(transform.position.x + 1.0f, transform.position.y, transform.position.z + 3f), Quaternion.identity * Quaternion.Euler(90, 45, 0));
        Instantiate(coinPrefab, new Vector3(transform.position.x, hit.transform.position.y, transform.position.z + 1.5f), Quaternion.identity * Quaternion.Euler(90, 0, 0));
        Instantiate(coinPrefab, new Vector3(transform.position.x + 1.0f, hit.transform.position.y, transform.position.z), Quaternion.identity * Quaternion.Euler(90, 45, 0));
        Instantiate(coinPrefab, new Vector3(transform.position.x - 1.0f, hit.transform.position.y, transform.position.z), Quaternion.identity * Quaternion.Euler(90, 135, 0));
    }

    private IEnumerator FlameThrowerCooldown()
    {
        flameThrowerCooldown = true;
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        yield return wait;
        flameThrowerCooldown = false;
    }

    private IEnumerator IceCloudCooldown()
    {
        iceCloudCooldown = true;
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        yield return wait;
        iceCloudCooldown = false;
    }

    private void Burn()
    {
        if (burnTime <= 0f)
        {
            burnTime = 5.0f;
            StartCoroutine(ApplyBurnDamage());
        }
        else
        {
            burnTime = 5.0f;
        }
    }

    private IEnumerator ApplyBurnDamage()
    {
        while (burnTime > 0)
        {
            burnEffect.Play();
            burnTime -= 0.5f;
            WaitForSeconds wait = new WaitForSeconds(0.5f);
            yield return wait;
            TakeDamage(2.0f);
        }
    }

    private void Freeze()
    {
        if (freezeTime <= 0f)
        {
            freezeTime = 5.0f;
            StartCoroutine(ApplyFreezeEffect());
        }
        else
        {
            freezeTime = 5.0f;
        }
    }

    private IEnumerator ApplyFreezeEffect()
    {
        enemySpeed = 0.5f;
        while (freezeTime > 0)
        {
            freezeEffect.Play();
            freezeTime -= 0.5f;
            WaitForSeconds wait = new WaitForSeconds(0.5f);
            yield return wait;
        }
        enemySpeed = 1.0f;
    }
}
