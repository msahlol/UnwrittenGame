using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHover : MonoBehaviour
{
    public float speed = 5.0f;
    public float height = 0.5f;

    private float startingHeight;

    // Start is called before the first frame update
    private void Start()
    {
        startingHeight = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 startingPos = transform.position;
        float newY = startingHeight + (Mathf.Sin(Time.time * speed) * height);
        transform.position = new Vector3(startingPos.x, newY, startingPos.z);
    }
}
