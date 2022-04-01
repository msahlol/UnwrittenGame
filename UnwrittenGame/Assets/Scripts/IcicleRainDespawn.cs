using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleRainDespawn : MonoBehaviour
{
    // Update is called once per frame
    void Start()
    {
        StartCoroutine(Despawn());
    }

    private IEnumerator Despawn()
    {
        WaitForSeconds wait = new WaitForSeconds(5.0f);
        yield return wait;
        gameObject.SetActive(false);
    }
}
