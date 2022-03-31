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
            rb.velocity = new Vector3(direction.x, direction.y, direction.z);

            if (InAttackRadius(direction.normalized) && !attackCooldown)
            {
                float attackOffset = Vector3.Angle(direction, transform.forward) + attackAngleOffset;
                var axisOffset = Quaternion.AngleAxis(attackOffset, Vector3.up);

                GameObject attack = Instantiate(enemyAttackPrefab, transform.position + transform.forward + transform.up - transform.right, transform.rotation);
                Instantiate(enemyAttackContactPrefab, transform.position + transform.forward + transform.up - transform.right, transform.rotation);
                attack.GetComponent<EnemyAttackHandler>().direction = axisOffset * transform.forward;
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
        if (collision.gameObject.CompareTag("Player"))
        {
            meleeCooldown = true;
            DealDamage();
            StartCoroutine(MeleeCooldown());
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

        Instantiate(coinPrefab, new Vector3(transform.position.x, hit.transform.position.y + 1.5f, transform.position.z + 1.5f), Quaternion.identity * Quaternion.Euler(90, 0, 0));
        Instantiate(coinPrefab, new Vector3(transform.position.x + 1.0f, hit.transform.position.y + 1.5f, transform.position.z), Quaternion.identity * Quaternion.Euler(90, 45, 0));
        Instantiate(coinPrefab, new Vector3(transform.position.x - 1.0f, hit.transform.position.y + 1.5f, transform.position.z), Quaternion.identity * Quaternion.Euler(90, 135, 0));
    }
}
