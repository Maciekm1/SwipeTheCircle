using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayButtonState : UIState
{
    [SerializeField] private RectTransform colour;
    public static event Action OnTapAction;
    private readonly int mainMenuIdleState = Animator.StringToHash("MainMenuIdle");
    private readonly int ToGame = Animator.StringToHash("ToGameTransition");
    protected override void executeState()
    {
        switch (_state)
        {
            case State.MainMenuIdle:
                LeanTween.alpha(colour, 1f, 0.1f);
                GetComponent<Button>().onClick.AddListener(changeGameStateToInGame);
                GetComponent<Button>().onClick.RemoveListener(AddOnClickAction);
                GetComponent<Animator>().Play(mainMenuIdleState);
                break;
            case State.InGameIdle:
                GetComponent<Button>().onClick.RemoveListener(changeGameStateToInGame);
                GetComponent<Button>().onClick.AddListener(AddOnClickAction);
                _animator.Play(ToGame);
                break;
            case State.LoseIdle: // Max 2 sec
                LeanTween.alpha(colour, 0f, 1f).setDelay(0.5f);
                break;
        }
    }

    private void changeGameStateToInGame()
    {
        _gameManager.ChangeGameState(GameState.InGame);
    }

    private void AddOnClickAction()
    {
        OnTapAction?.Invoke();
    }

    private void OnEnable() {
        if(_state == State.MainMenuIdle)
        {
            _animator.Play(mainMenuIdleState);
        }
    }
}

/*
    private readonly int mainMenuIdleState = Animator.StringToHash("MainMenuIdle");
    protected override void executeState()
    {
        switch (_state)
        {
            case State.TransitionToMain:
                changeState(State.MainMenuIdle);
                break;
            case State.MainMenuIdle:
                break;
            case State.InGameIdle:
                break;
            case State.TransitionToInGame:
                changeState(State.InGameIdle);
                break;
            case State.TransitionToLose:
                changeState(State.LoseIdle);
                break;
            case State.LoseIdle:
                break;
        }
    }
    */
