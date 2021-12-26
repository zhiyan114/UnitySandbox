using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class LobbyPlayerManager : MonoBehaviour
{
    private Rigidbody2D rb;
    public float WalkSpeed;
    public float JumpHeight;
    private JObject UserData = (JObject)SaveManager.Data.GetValue("PlayerData");
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if(!SaveManager.SetDefaultValue("Position", new JObject(), "PlayerData"))
            rb.position = new Vector3((int)UserData["Position"]["x"], (int)UserData["Position"]["y"]);

    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow)))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        } else if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            // Jump Key
            rb.velocity = new Vector2(rb.velocity.x,JumpHeight);
        } else if((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && rb.velocity.y != 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, -JumpHeight);
        }
        if ((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow)))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        } else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            // Right Side Key
            rb.velocity = new Vector2(WalkSpeed,rb.velocity.y);
        } else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            // Left side Key
            rb.velocity = new Vector2(-WalkSpeed, rb.velocity.y);
        }
        UserData["Position"]["x"] = rb.position.x;
        UserData["Position"]["y"] = rb.position.y;
    }
}