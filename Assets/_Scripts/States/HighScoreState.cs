using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreState : UIState
{
    private readonly int ToGame = Animator.StringToHash("HSToGame");
    private readonly int ToGameR = Animator.StringToHash("HSToGameR");
    protected override void executeState()
    {
        switch (_state)
        {
            case State.MainMenuIdle:
                GetComponent<Animator>().Play(ToGameR);
                break;
            case State.InGameIdle:
                _animator.Play(ToGame);
                break;
            case State.LoseIdle: // Max 2 sec
                break;
        }
    }
}
