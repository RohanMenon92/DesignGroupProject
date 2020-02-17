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

    int currentScore;
    int turnScore;
    int enemyScore;

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
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown("joystick button 8") || Input.GetKeyDown("joystick button 10"))
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
                    SwitchState(EncounterConstants.GameplayState.EnemyIntro);
                    break;
                case EncounterConstants.GameplayState.EnemyIntro:
                    SwitchState(EncounterConstants.GameplayState.EnemyPlay);
                    break;
                case EncounterConstants.GameplayState.EnemyPlay:
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

        //Debug.Log(currentState);
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
                    switchAllowed = newState == EncounterConstants.GameplayState.EnemyIntro;
                }
                break;
            case EncounterConstants.GameplayState.EnemyIntro:
                {
                    switchAllowed = newState == EncounterConstants.GameplayState.EnemyPlay;
                }
                break;
            case EncounterConstants.GameplayState.EnemyPlay:
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
                    turnScore = 0;
                    currentScore = 0;
                    FadeInOverlayUI();
                }
                break;
            case EncounterConstants.GameplayState.TurnIntro:
                {
                    turnScore = 0;
                    StartTurnUI();
                }
                break;
            case EncounterConstants.GameplayState.TurnPlay:
                {
                    SpawnSelectorUI();
                }
                break;
            case EncounterConstants.GameplayState.EnemyIntro:
                {
                    currentScore += turnScore;
                    enemyScore = 0;
                    gameplayUI.GetComponent<GameplayPanelManager>().OnTurnEnd(currentScore);
                    EndTurnUI();
                }
                break;
            case EncounterConstants.GameplayState.EnemyPlay:
                {
                    gameplayUI.GetComponent<GameplayPanelManager>().ShowEnemyUI();
                    // TODO: This should be removed and done through gameplay
                    EnemyScoreDecrement(100);
                }
                break;
            case EncounterConstants.GameplayState.TurnPlayOut:
                {
                    // TODO Delete this. Enemy score reduction
                    currentScore -= enemyScore;
                    gameplayUI.GetComponent<GameplayPanelManager>().OnEnemyEnd(currentScore);
                    EndEnemyUI();
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
            case EncounterConstants.GameplayState.EnemyIntro:
                {
                }
                break;
            case EncounterConstants.GameplayState.EnemyPlay:
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
            case EncounterConstants.GameplayState.EnemyIntro:
                {
                }
                break;
            case EncounterConstants.GameplayState.EnemyPlay:
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
        gameplayUI.GetComponent<GameplayPanelManager>().ShowTurnUI();
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
        mainCamera.transform.DOMove(EncounterConstants.cameraTurnPos, 0.75f).SetEase(Ease.OutBack);
        mainCamera.transform.DORotate(EncounterConstants.cameraTurnRot, 1f).SetEase(Ease.OutSine);
    }

    void MoveCameraToOverview()
    {
        mainCamera.transform.DOMove(EncounterConstants.cameraOverviewPos, 0.75f).SetEase(Ease.OutBack);
        mainCamera.transform.DORotate(EncounterConstants.cameraOverviewRot, 1f).SetEase(Ease.OutSine);
    }

    void MoveCameraToEnemy()
    {
        mainCamera.transform.DOMove(EncounterConstants.cameraEnemyPos, 0.75f).SetEase(Ease.OutBack);
        mainCamera.transform.DORotate(EncounterConstants.cameraEnemyRot, 1f).SetEase(Ease.OutSine);
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
        MoveCameraToEnemy();
        FadeOutTurnUI();
    }

    void EndEnemyUI()
    {
        MoveCameraToOverview();
    }

    public void TurnScoreIncrement(int scoreAmount)
    {
        turnScore += scoreAmount;
        gameplayUI.GetComponent<GameplayPanelManager>().UpdateTurnScore(currentScore + turnScore);
    }

    public void EnemyScoreDecrement(int scoreAmount)
    {
        enemyScore += scoreAmount;
        gameplayUI.GetComponent<GameplayPanelManager>().UpdateEnemyScore(currentScore - enemyScore);
    }
}