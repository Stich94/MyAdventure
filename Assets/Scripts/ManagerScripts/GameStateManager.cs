using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager
{
    static GameStateManager instance;
    public GameState CurrentGameState { get; private set; }

    public static GameStateManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameStateManager();
            }
            return instance;
        }
    }


    public delegate void GameStateChangeHandler(GameState _newGameState);
    public event GameStateChangeHandler OnGameStateChanged;

    public void SetState(GameState _newState)
    {
        if (_newState == CurrentGameState)
        {
            return;
        }

        CurrentGameState = _newState;
        OnGameStateChanged?.Invoke(_newState);
    }

    private GameStateManager()
    {

    }



}
