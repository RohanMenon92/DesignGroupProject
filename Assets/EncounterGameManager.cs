using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterGameManager : MonoBehaviour
{
    public CanvasGroup turnUI;
    public CanvasGroup gameplayUI;
    public Camera mainCamera;

    public CrowdEntitiesScript crowdEntities;
    public PlayerEntitiesScript playerEntities;
    public EnemyEntitiesScrpt enemyEntities;

    private EncounterConstants.GameplayState currentState = EncounterConstants.GameplayState.StartGame;
    private EncounterConstants.GameplayState lastState = EncounterConstants.GameplayState.StartGame;

    // Start is called before the first frame update
    void Start()
    {
        FadeOutTurnUI();
        FadeOutOverlayUI();
    }

    // Update is called once per frame
    void Update()
    {
        // This Update loop will be commented out when actual gameplay is written
        if(Input.GetKeyDown(KeyCode.T))
        {
            switch(currentState)
            {
                case EncounterConstants.GameplayState.StartGame:
                    SwitchState(EncounterConstants.GameplayState.StartGameUI);
                    break;
                case EncounterConstants.GameplayState.StartGameUI:
                    SwitchState(EncounterConstants.GameplayState.TurnIntro);
                    break;
                case EncounterConstants.GameplayState.TurnIntro:
                    SwitchState(EncounterConstants.GameplayState.TurnPlay);
                    break;
                case EncounterConstants.GameplayState.TurnPlay:
                    SwitchState(EncounterConstants.GameplayState.TurnPlayOut);
                    break;
                case EncounterConstants.GameplayState.TurnPlayOut:
                    SwitchState(EncounterConstants.GameplayState.TurnIntro);
                    break;
                //case EncounterConstants.GameplayState.EndGameUI:
                //    SwitchState(EncounterConstants.GameplayState.StartGameUI);
                //    break;
                //case EncounterConstants.GameplayState.EndGame:
                //    SwitchState(EncounterConstants.GameplayState.StartGameUI);
                //    break;
            }
        }

        Debug.Log(currentState);
        CheckState();
    }

    void SwitchState(EncounterConstants.GameplayState newState)
    {
        bool switchAllowed = false;

        // Do check for if switch is possible
        switch (currentState)
        {
            case EncounterConstants.GameplayState.StartGame:
                {
                    switchAllowed = newState == EncounterConstants.GameplayState.StartGameUI;
                }
                break;
            case EncounterConstants.GameplayState.StartGameUI:
                {
                    switchAllowed = newState == EncounterConstants.GameplayState.TurnIntro;
                }
                break;
            case EncounterConstants.GameplayState.TurnIntro:
                {
                    switchAllowed = newState == EncounterConstants.GameplayState.TurnPlay;
                }
                break;
            case EncounterConstants.GameplayState.TurnPlay:
                {
                    switchAllowed = newState == EncounterConstants.GameplayState.TurnPlayOut;
                }
                break;
            case EncounterConstants.GameplayState.TurnPlayOut:
                {
                    switchAllowed = newState == EncounterConstants.GameplayState.EndGameUI || newState == EncounterConstants.GameplayState.TurnIntro;
                }
                break;
            case EncounterConstants.GameplayState.EndGameUI:
                {
                    switchAllowed = newState == EncounterConstants.GameplayState.EndGame;
                }
                break;
            case EncounterConstants.GameplayState.EndGame:
                {
                    switchAllowed = false;
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

            case EncounterConstants.GameplayState.StartGame:
                {
                    FadeOutTurnUI();
                }
                break;
            case EncounterConstants.GameplayState.StartGameUI:
                {
                    FadeInOverlayUI();
                }
                break;
            case EncounterConstants.GameplayState.TurnIntro:
                {
                    StartTurnUI();
                }
                break;
            case EncounterConstants.GameplayState.TurnPlay:
                {
                    SpawnSelectorUI();
                }
                break;
            case EncounterConstants.GameplayState.TurnPlayOut:
                {
                    EndTurnUI();
                }
                break;
            case EncounterConstants.GameplayState.EndGameUI:
                {
                    FadeOutOverlayUI();
                }
                break;
            case EncounterConstants.GameplayState.EndGame:
                {

                }
                break;
        }
    }
    void OnExitState(EncounterConstants.GameplayState stateExit)
    {
        switch (stateExit)
        {

            case EncounterConstants.GameplayState.StartGame:
                {
                }
                break;
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
            case EncounterConstants.GameplayState.EndGame:
                {

                }
                break;
        }
    }
    void OnProcessState(EncounterConstants.GameplayState stateProcess)
    {

        switch (stateProcess)
        {
            case EncounterConstants.GameplayState.StartGame:
                {

                }
                break;
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
            case EncounterConstants.GameplayState.EndGame:
                {

                }
                break;
        }
    }

    // Fade in and fade out UI
    void FadeOutTurnUI()
    {
        turnUI.GetComponent<NoteGeneratorManager>().FadeOutSelectorUI();
        turnUI.DOFade(0f, 1f);
        turnUI.transform.DOScale(0.75f, 0.25f);
    }

    void FadeInTurnUI()
    {
        turnUI.transform.localScale = Vector3.one * 0.8f;
        turnUI.DOFade(1f, 1f);
        turnUI.transform.DOScale(1f, 0.25f);
    }

    void FadeInOverlayUI()
    {
        gameplayUI.transform.localScale = Vector3.one * 0.8f;
        gameplayUI.DOFade(1f, 1f);
        gameplayUI.transform.DOScale(1f, 0.25f);
    }

    void FadeOutOverlayUI()
    {
        gameplayUI.DOFade(0f, 1f);
        gameplayUI.transform.DOScale(0.75f, 0.25f);
    }

    void MoveCameraToTurnFocus()
    {
        mainCamera.transform.DOMove(EncounterConstants.cameraTurnPos, 0.5f).SetEase(Ease.OutBack);
        mainCamera.transform.DORotate(EncounterConstants.cameraTurnRot, 0.75f).SetEase(Ease.OutSine);
    }

    void MoveCameraToOverview()
    {
        mainCamera.transform.DOMove(EncounterConstants.cameraOverviewPos, 0.5f).SetEase(Ease.OutBack);
        mainCamera.transform.DORotate(EncounterConstants.cameraOverviewRot, 0.75f).SetEase(Ease.OutSine);
    }

    void SpawnSelectorUI()
    {
        turnUI.GetComponent<NoteGeneratorManager>().FadeInSelectorUI();
    }

    void StartTurnUI()
    {
        MoveCameraToTurnFocus();
        FadeInTurnUI();
    }

    void EndTurnUI()
    {
        MoveCameraToOverview();
        FadeOutTurnUI();
    }
}