using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    PlayerInput playerInput;
    // Start is called before the first frame update
    InputAction pauseAction;
    [SerializeField] Canvas canvas;
    [SerializeField] bool paused = false;

    private void OnEnable()
    {
        pauseAction.performed += _ => DeterminePause();
    }

    private void OnDisable()
    {
        pauseAction.canceled -= _ => DeterminePause();
    }
    void Awake()
    {
        canvas.enabled = false;
        playerInput = GetComponent<PlayerInput>();
        // access the controls
        pauseAction = playerInput.actions["Pause"];
    }


    void PauseGame()
    {
        if (pauseAction.triggered)
        {
            GameState currentGameState = GameStateManager.Instance.CurrentGameState;
            GameState newGameState = currentGameState == GameState.Gameplay ? GameState.Paused : GameState.Gameplay;

            GameStateManager.Instance.SetState(newGameState);
        }
    }

    public void DeterminePause()
    {
        if (paused)
        {
            ResumeGame();
        }
        else
        {

            PauseGameState();
        }
    }


    public void PauseGameState()
    {
        Time.timeScale = 0;
        canvas.enabled = true;
        paused = true;
        AudioListener.volume = 0f;
        Cursor.lockState = CursorLockMode.None;

    }

    public void ResumeGame()
    {
        AudioListener.volume = 1f;
        Time.timeScale = 1;
        canvas.enabled = false;
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;

    }
}
