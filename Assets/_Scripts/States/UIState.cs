using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIState : MonoBehaviour
{
    [SerializeField] protected State _state;
    protected Animator _animator;
    protected GameManager _gameManager;

    protected void Start() 
    {
        _animator = GetComponent<Animator>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    public void changeState(State newState)
    {
        if(_state == newState){
            return;
        }
        _state = newState;
        executeState();
    }

    protected virtual void Update()
    {

    }

    protected abstract void executeState();
}

    public enum State{
        StartState,
        MainMenuIdle,
        InGameIdle,
        LoseIdle,
    }
