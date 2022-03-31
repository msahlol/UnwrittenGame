using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;

    [Range(0,360)]
    public float angle;

    public GameObject player;

    public LayerMask obstructionMask;

    public bool playerInView;

    public Vector3 directionToTarget;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        if ((player.transform.position - transform.position).sqrMagnitude < radius * radius)
        {
            directionToTarget = (player.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, player.transform.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    playerInView = true;
                else
                {
                    playerInView = false;
                    directionToTarget = transform.position.normalized;
                }
            }
            else
            {
                playerInView = false;
                directionToTarget = transform.position.normalized;
            }
        }
        else if (playerInView)
        {
            playerInView = false;
            directionToTarget = transform.position.normalized;
        }

    }
}
