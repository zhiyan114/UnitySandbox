using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Rigidbody2D rb;
    public float WalkSpeed;
    public float JumpHeight;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            // Jump Key
            rb.velocity = new Vector2(rb.velocity.x,JumpHeight);
        } else if((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && rb.velocity.y != 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, -JumpHeight);
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            // Right Side Key
            rb.velocity = new Vector2(WalkSpeed,rb.velocity.y);
        } else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            // Left side Key
            rb.velocity = new Vector2(-WalkSpeed, rb.velocity.y);
        }
    }
}
