using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

public class LobbyPlayerManager : MonoBehaviour
{
    private Rigidbody2D rb;
    public float WalkSpeed;
    public float JumpHeight;
    private SaveData UserData = SaveManager.Data;
    public GameObject PauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if(UserData.Position.TryGetValue("x",out float xPos) && UserData.Position.TryGetValue("y", out float yPos))
            rb.position = new Vector3(xPos, yPos);

    }

    // Update is called once per frame
    void Update()
    {
        // Developer func
        if (Input.GetKey(KeyCode.LeftBracket)) Economy.Manager.SetBalance(-1);
        if (Input.GetKey(KeyCode.RightBracket)) Economy.Manager.SetBalance(1);
        // End developer func
        // Quick Menu Controls and UI
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = !PauseMenu.activeSelf ? 0 : 1;
            PauseMenu.SetActive(!PauseMenu.activeSelf);
        }
        if (PauseMenu.activeSelf) return;
        // General User Movement Control
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
        UserData.Position["x"] = rb.position.x;
        UserData.Position["y"] = rb.position.y;
    }
    public void UnpauseGame()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void ReturnToMainMenu()
    {
        PauseMenu.SetActive(false);
        SaveManager.SaveToDisk();
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
