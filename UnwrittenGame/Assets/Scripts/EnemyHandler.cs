using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    public float enemyHealth = 10.0f;
    public float enemyDamage = 7.0f;
    public float enemySpeed = 1.0f;

    public bool flameThrowerCooldown = false;
    public bool iceCloudCooldown = false;

    public float burnTime = 0.0f;
    public float freezeTime = 0.0f;

    public ParticleSystem burnEffect;
    public ParticleSystem freezeEffect;

    public GameObject player;
    public LayerMask groundMask;

    private PlayerController pc;
    private Rigidbody rb;
    private Rigidbody prb;
    private FieldOfView fieldOfView;
    private float attackCooldownDelay = 2.0f;
    private float attackAngle = 10.0f;
    public float gravityOffset = -10.0f;
    private bool attackCooldown = false;
    private bool foundPlayer = false;
    public Vector3 spawnLocation;

    public float attackAngleOffset = 15.0f;
    public float followBuffer = 3f;


    public GameObject enemyAttackPrefab;
    public GameObject enemyAttackContactPrefab;

    public GameObject coinPrefab;
    public GameObject healthPrefab;
    public GameObject manaPrefab;


    void Start()
    {
        burnEffect = transform.Find("EnemyBurnEffect").GetComponent<ParticleSystem>();
        fieldOfView = gameObject.GetComponent<FieldOfView>();
        rb = gameObject.GetComponent<Rigidbody>();
        pc = player.gameObject.GetComponent<PlayerController>();
        spawnLocation = transform.position;
    }

    void Update()
    {
        if (fieldOfView.playerInView)
        {
            StopCoroutine(LostSight());
            foundPlayer = true;
            Vector3 direction = player.transform.position - transform.position;
            Quaternion angle = Quaternion.LookRotation(direction, Vector3.up);

            rb.rotation = angle;
            if(direction.sqrMagnitude > followBuffer * followBuffer)
                rb.velocity = new Vector3(direction.x, 0, direction.z) * enemySpeed;

            if (InAttackRadius(direction.normalized) && !attackCooldown)
            {

                GameObject attack = Instantiate(enemyAttackPrefab, transform.position + transform.forward, transform.rotation * Quaternion.Euler(new Vector3(0, -90, 0)));
                Instantiate(enemyAttackContactPrefab, transform.position + transform.forward, transform.rotation);
                attack.GetComponent<EnemyAttackHandler>().direction = transform.forward;
                attackCooldown = true;
                StartCoroutine(AttackCooldown());
            }
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
        DealDamage();
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
        WaitForSeconds wait = new WaitForSeconds(3.0f);
        yield return wait;
        Vector3 spawnDirection = spawnLocation - transform.position;
        LookAt2D(spawnDirection);
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

    private void SpawnCoins()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, -Vector3.up, out hit, groundMask);

        Instantiate(manaPrefab, new Vector3(transform.position.x - 1.0f, transform.position.y, transform.position.z + 3f), Quaternion.identity * Quaternion.Euler(90, 0, 0));
        Instantiate(healthPrefab, new Vector3(transform.position.x + 1.0f, transform.position.y, transform.position.z + 3f), Quaternion.identity * Quaternion.Euler(90, 45, 0));
        Instantiate(coinPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z + 1.5f), Quaternion.identity * Quaternion.Euler(90, 0, 0));
        Instantiate(coinPrefab, new Vector3(transform.position.x + 1.0f, transform.position.y, transform.position.z), Quaternion.identity * Quaternion.Euler(90, 45, 0));
        Instantiate(coinPrefab, new Vector3(transform.position.x - 1.0f, transform.position.y, transform.position.z), Quaternion.identity * Quaternion.Euler(90, 135, 0));
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
        if(burnTime <= 0f)
        {
            burnEffect.Play();
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
