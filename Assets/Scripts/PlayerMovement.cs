using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb2d;
    public float speedVariable; 

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        double horizontalInput = Input.GetAxisRaw("Horizontal");
        // Move the player based on the input (X-axis)

        double verticalInput = Input.GetAxisRaw("Vertical");
        // Move the player based on the input (Y-axis)

        Debug.Log("Horizontal Input: " + horizontalInput + " |" + " Vertical Input: " + verticalInput);

        Vector2 movementDirection = new Vector2((float)horizontalInput, (float)verticalInput).normalized;

        rb2d.velocity = movementDirection * speedVariable;

    }
}
