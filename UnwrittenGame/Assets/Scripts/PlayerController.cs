using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    public float defaultSpeed = 10.0f;
    public float sprintSpeed = 20.0f;
    public float jumpForce = 10.0f;
    public float cameraSensitivity = 2.0f;
    public float maxHealth = 100.0f;
    public float health = 100.0f;
    public float maxMana = 100.0f;
    public float mana = 100.0f;
    public float manaRegen = 1.0f;
    public int coins = 0;
    public int selectedAbility = 0;
    public GameObject focalPoint;
    public GameObject gameCamera;
    public GameObject projectilePrefab;
    public GameObject projectileBurstPrefab;
    public Healthbar healthBar;
    public Manabar manaBar;
    public TextMeshProUGUI healthAmount;
    public TextMeshProUGUI manaAmount;
    public TextMeshProUGUI coinCounter;
    
    public bool isTouchingGround = true;
    public bool canDoubleJump = false;
    public bool isInDecision = false;

    public ParticleSystem fireJump;
    public ParticleSystem fireTrail;

    public ParticleSystem iceJump;
    public ParticleSystem iceTrail;


    private Rigidbody rb;
    private MenuHandler menuHandler;
    private PlayerAbilities abilityHandler;
    private ParticleSystem sprintTrail;
    private ParticleSystem jumpBurstPrefab;
    private GameObject currentDecision;
    private float maxCameraAngle = 60.0f;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        fireTrail = transform.Find("Fire Trail").GetComponent<ParticleSystem>();
        iceTrail = transform.Find("Ice Trail").GetComponent<ParticleSystem>();
        fireJump = transform.Find("Fire Jump Burst").GetComponent<ParticleSystem>();
        iceJump = transform.Find("Ice Jump Burst").GetComponent<ParticleSystem>();
        menuHandler = gameObject.GetComponent<MenuHandler>();
        abilityHandler = gameObject.GetComponent<PlayerAbilities>();
        Cursor.lockState = CursorLockMode.Locked;

        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(health);

        manaBar.SetMaxMana(maxMana);
        manaBar.SetMana(mana);

        sprintTrail = fireTrail;
        jumpBurstPrefab = fireJump;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isInDecision && currentDecision != null)
            {
                isInDecision = false;
                currentDecision.transform.Find("Decision Menu").gameObject.SetActive(false);
                currentDecision = null;
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;

                
            }
            else if (!menuHandler.isPaused)
            {
                menuHandler.PauseGame();
            }
            else
            {
                menuHandler.UnPauseGame();
            }
        }

        if (!menuHandler.isPaused && !isInDecision)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                speed = sprintSpeed;
                sprintTrail.Play();
                sprintTrail.Clear();
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                speed = defaultSpeed;
                sprintTrail.Stop();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isTouchingGround)
                {
                    Jump();
                }
                else if (canDoubleJump)
                {
                    Jump();
                    jumpBurstPrefab.Play();
                    canDoubleJump = false;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                //FireProjectile();
                abilityHandler.UseAbility(selectedAbility);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //if (abilityHandler.abilityList[0].isUnlocked)
                selectedAbility = 0;
                menuHandler.UpdateSelectedAbility(selectedAbility);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                //if (abilityHandler.abilityList[1].isUnlocked)
                selectedAbility = 1;
                menuHandler.UpdateSelectedAbility(selectedAbility);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                //if (abilityHandler.abilityList[2].isUnlocked)
                selectedAbility = 2;
                menuHandler.UpdateSelectedAbility(selectedAbility);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                //if (abilityHandler.abilityList[3].isUnlocked)
                selectedAbility = 3;
                menuHandler.UpdateSelectedAbility(selectedAbility);
            }

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // Getting camera inputs and applying them
            xRotation += Input.GetAxis("Mouse X") * cameraSensitivity;
            yRotation -= Input.GetAxis("Mouse Y") * cameraSensitivity;
            xRotation = Mathf.Repeat(xRotation, 360);
            yRotation = Mathf.Clamp(yRotation, -maxCameraAngle, maxCameraAngle);

            transform.Rotate(0, Input.GetAxis("Mouse X") * cameraSensitivity, 0);
            focalPoint.transform.rotation = Quaternion.Euler(yRotation, xRotation, 0);

            Vector2 direction = new Vector2(horizontalInput * speed, verticalInput * speed);
            rb.velocity = Quaternion.Euler(0, focalPoint.transform.rotation.eulerAngles.y, 0) * new Vector3(direction.normalized.x * speed, rb.velocity.y, direction.normalized.y * speed);
            focalPoint.transform.position = transform.position;
        }

        mana = Mathf.Min(maxMana, mana + manaRegen * Time.deltaTime); 
        manaBar.SetMana(mana);

        coinCounter.text = "X " + coins;
        healthAmount.text = "" + (int)health;
        manaAmount.text = "" + (int)mana;
    }

    void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward + transform.up - transform.right, transform.rotation);
        Instantiate(projectileBurstPrefab, transform.position + transform.forward + transform.up - transform.right, transform.rotation);
        projectile.GetComponent<ProjectileHandler>().direction = gameCamera.transform.forward;
    }

    void Jump()
    {
        Vector3 playerVelocity = rb.velocity;
        rb.velocity = new Vector3(playerVelocity.x, 0, playerVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isTouchingGround = true;
            canDoubleJump = false;
        }

        if (collision.gameObject.CompareTag("EnemyAttack"))
        {
            health -= collision.gameObject.GetComponent<EnemyAttackHandler>().projectileDamage;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            health -= collision.gameObject.GetComponent<EnemyHandler>().enemyDamage;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isTouchingGround = false;
            canDoubleJump = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            coins += other.gameObject.GetComponent<CoinValue>().coinValue;
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("Decision"))
        {
            isInDecision = true;
            currentDecision = other.gameObject;
            other.gameObject.transform.Find("Decision Menu").gameObject.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
        }

        if (other.gameObject.CompareTag("Health"))
        {
            if((health + other.gameObject.GetComponent<HealthValue>().healthValue) > maxHealth) {
                health = maxHealth;
            }
            else
            {
                health += other.gameObject.GetComponent<HealthValue>().healthValue;
            }
            
            other.gameObject.SetActive(false);
            healthBar.SetHealth(health);
        }

        if (other.gameObject.CompareTag("Mana"))
        {
            if ((mana + other.gameObject.GetComponent<ManaValue>().manaValue) > maxMana)
            {
                mana = maxMana;
            }
            else
            {
                mana += other.gameObject.GetComponent<ManaValue>().manaValue;
            }
            
            other.gameObject.SetActive(false);
            manaBar.SetMana(mana);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Decision"))
        {
            other.gameObject.transform.Find("Decision Menu").gameObject.SetActive(false);
        }
    }

    public void SetMovementPrefabsFire()
    {
        sprintTrail.Stop();
        sprintTrail = fireTrail;
        jumpBurstPrefab = fireJump;
    }

    public void SetMovementPrefabsIce()
    {
        sprintTrail.Stop();
        sprintTrail = iceTrail;
        jumpBurstPrefab = iceJump;
    }
}
