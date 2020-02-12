using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterGameManager : MonoBehaviour
{
    private EncounterConstants.GameplayState currentState = EncounterConstants.GameplayState.StartGameUI;
    // To check Entry and Exit of state
    private EncounterConstants.GameplayState lastState = EncounterConstants.GameplayState.StartGameUI;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckState();
    }

    void SwitchState(EncounterConstants.GameplayState newState)
    {
        bool switchAllowed = false;

        // Do check for if switch is possible
        switch (currentState)
        {
            case EncounterConstants.GameplayState.StartGameUI:
                {

                }
                break;
            case EncounterConstants.GameplayState.TurnIntro:
                {

                }
                break;
            case EncounterConstants.GameplayState.TurnPlay:
                {
                }
                break;
            case EncounterConstants.GameplayState.TurnPlayOut:
                {
                }
                break;
            case EncounterConstants.GameplayState.EndGameUI:
                {

                }
                break;
        }

        if (switchAllowed)
        {
            currentState = newState;
        }
    }

    void CheckState()
    {
        if (currentState != lastState)
        {
            OnExitState(lastState);
            OnEnterState(currentState);
            lastState = currentState;
        }
        OnProcessState(currentState);
    }


    // Check entry to stateEnter
    void OnEnterState(EncounterConstants.GameplayState stateEnter)
    {
        switch (stateEnter)
        {
            case EncounterConstants.GameplayState.StartGameUI:
                {

                }
                break;
            case EncounterConstants.GameplayState.TurnIntro:
                {

                }
                break;
            case EncounterConstants.GameplayState.TurnPlay:
                {
                }
                break;
            case EncounterConstants.GameplayState.TurnPlayOut:
                {
                }
                break;
            case EncounterConstants.GameplayState.EndGameUI:
                {

                }
                break;
        }
    }
    void OnExitState(EncounterConstants.GameplayState stateExit)
    {
        switch (stateExit)
        {
            case EncounterConstants.GameplayState.StartGameUI:
                {

                }
                break;
            case EncounterConstants.GameplayState.TurnIntro:
                {

                }
                break;
            case EncounterConstants.GameplayState.TurnPlay:
                {
                }
                break;
            case EncounterConstants.GameplayState.TurnPlayOut:
                {
                }
                break;
            case EncounterConstants.GameplayState.EndGameUI:
                {

                }
                break;
        }
    }
    void OnProcessState(EncounterConstants.GameplayState stateProcess)
    {

        switch (stateProcess)
        {
            case EncounterConstants.GameplayState.StartGameUI:
                {

                }
                break;
            case EncounterConstants.GameplayState.TurnIntro:
                {

                }
                break;
            case EncounterConstants.GameplayState.TurnPlay:
                {
                }
                break;
            case EncounterConstants.GameplayState.TurnPlayOut:
                {
                }
                break;
            case EncounterConstants.GameplayState.EndGameUI:
                {

                }
                break;
        }
    }
}