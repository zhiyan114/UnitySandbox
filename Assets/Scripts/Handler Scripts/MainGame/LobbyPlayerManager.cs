using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class LobbyPlayerManager : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField]
    private float WalkSpeed;
    [SerializeField]
    private float JumpHeight;
    private SaveData UserData = SaveManager.Data;
    [SerializeField]
    private GameObject PauseMenu;
    [SerializeField]
    private TextMeshProUGUI BalanceText;
    private PlayerInput PlrInput;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PlrInput = GetComponent<PlayerInput>();
        if (UserData.Position.TryGetValue("x",out float xPos) && UserData.Position.TryGetValue("y", out float yPos))
            rb.position = new Vector3(xPos, yPos);
        BalanceText.text = Economy.Manager.GetBalance.ToString();
        Economy.Manager.BalanceChanged += Manager_BalanceChanged;
    }

    private void Manager_BalanceChanged(object sender, Economy.BalanceChangedEventArgs e)
    {
        BalanceText.text = e.newBalance.ToString();
    }
    
    // Update is called once per frame
    void Update()
    {
        // Developer func
        if (Input.GetKey(KeyCode.LeftBracket)) Economy.Manager.SetBalance(-1);
        if (Input.GetKey(KeyCode.RightBracket)) Economy.Manager.SetBalance(1);
        // End developer func
        // Quick Menu Controls and UI
        if (PauseMenu.activeSelf) return;
        // General User Movement Control
        if (PlrInput.actions["MoveUp"].IsPressed() && PlrInput.actions["MoveDown"].IsPressed())
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        } else if(PlrInput.actions["MoveUp"].IsPressed())
        {
            // Jump Key
            rb.velocity = new Vector2(rb.velocity.x,JumpHeight);
        } else if(PlrInput.actions["MoveDown"].IsPressed())
        {
            rb.velocity = new Vector2(rb.velocity.x, -JumpHeight);
        }
        if (PlrInput.actions["MoveLeft"].IsPressed() && PlrInput.actions["MoveRight"].IsPressed())
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        } else if(PlrInput.actions["MoveRight"].IsPressed())
        {
            // Right Side Key
            rb.velocity = new Vector2(WalkSpeed,rb.velocity.y);
        } else if(PlrInput.actions["MoveLeft"].IsPressed()) {
            // Left side Key
            rb.velocity = new Vector2(-WalkSpeed, rb.velocity.y);
        }
        UserData.Position["x"] = rb.position.x;
        UserData.Position["y"] = rb.position.y;
    }
    // Ends Here
    public void PauseGame(InputAction.CallbackContext cb)
    {
        if(cb.phase == InputActionPhase.Started)
        {
            Time.timeScale = !PauseMenu.activeSelf ? 0 : 1;
            PauseMenu.SetActive(!PauseMenu.activeSelf);
        }
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
