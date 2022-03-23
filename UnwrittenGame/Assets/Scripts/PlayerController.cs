using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    public float defaultSpeed = 10.0f;
    public float sprintSpeed = 20.0f;
    public float jumpForce = 10.0f;
    public float cameraSensitivity = 2.0f;
    public int coins = 0;
    public GameObject focalPoint;
    public GameObject gameCamera;
    public GameObject projectilePrefab;
    public GameObject projectileBurstPrefab;

    private Rigidbody rb;
    private MenuHandler menuHandler;
    private ParticleSystem sprintTrail;
    private ParticleSystem jumpBurstPrefab;
    private float maxCameraAngle = 60.0f;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    public bool isTouchingGround = true;
    public bool canDoubleJump = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        sprintTrail = transform.Find("Trail").GetComponent<ParticleSystem>();
        jumpBurstPrefab = transform.Find("Jump Burst").GetComponent<ParticleSystem>();
        menuHandler = gameObject.GetComponent<MenuHandler>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menuHandler.isPaused)
            {
                menuHandler.PauseGame();
            }
            else
            {
                menuHandler.UnPauseGame();
            }
        }

        if (!menuHandler.isPaused)
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
                FireProjectile();
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
    }
}
