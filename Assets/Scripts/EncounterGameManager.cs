using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterGameManager : MonoBehaviour
{
    public NoteGeneratorManager noteManager;
    public GameplayPanelManager gamePanelManager;
    public Camera mainCamera;

    public CrowdEntitiesScript crowdEntities;
    public EnemyEntitiesScrpt enemyEntities;

    int currentScore;
    int turnScore;
    int enemyScore;

    private EncounterConstants.GameplayState currentState = EncounterConstants.GameplayState.StartGame;
    private EncounterConstants.GameplayState lastState = EncounterConstants.GameplayState.StartGame;

    public delegate void AnimationCallback();
    // Start is called before the first frame update
    void Start()
    {
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
                //case EncounterConstants.GameplayState.TurnIntro:
                //    SwitchState(EncounterConstants.GameplayState.TurnPlay);
                //    break;
                //case EncounterConstants.GameplayState.TurnPlay:
                //    SwitchState(EncounterConstants.GameplayState.EnemyIntro);
                //    break;
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
                    //noteManager.FadeOutUI();
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
                    StartNoteGame();
                }
                break;
            case EncounterConstants.GameplayState.TurnPlay:
                {
                    gamePanelManager.ShowTurnUI();
                }
                break;
            case EncounterConstants.GameplayState.EnemyIntro:
                {
                    currentScore += turnScore;
                    enemyScore = 0;

                    gamePanelManager.OnTurnEnd(currentScore);
                    EndTurnUI();
                }
                break;
            case EncounterConstants.GameplayState.EnemyPlay:
                {
                    gamePanelManager.ShowEnemyUI();
                    // TODO: This should be removed and done through gameplay
                    EnemyScoreDecrement(100);
                }
                break;
            case EncounterConstants.GameplayState.TurnPlayOut:
                {
                    // TODO Delete this. Enemy score reduction
                    currentScore -= enemyScore;
                    gamePanelManager.OnEnemyEnd(currentScore);
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

    void FadeInOverlayUI()
    {
        gamePanelManager.transform.localScale = Vector3.one * 0.8f;
        gamePanelManager.GetComponent<CanvasGroup>().DOFade(1f, 1f);
        gamePanelManager.transform.DOScale(1f, 0.25f);
    }

    void FadeOutOverlayUI()
    {
        gamePanelManager.GetComponent<CanvasGroup>().DOFade(0f, 1f);
        gamePanelManager.transform.DOScale(0.75f, 0.25f);
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

    void StartNoteGame()
    {
        MoveCameraToTurnFocus();
        noteManager.StartGame();
    }

    void EndTurnUI()
    {
        MoveCameraToEnemy();
    }

    void EndEnemyUI()
    {
        MoveCameraToOverview();
    }

    public void TurnScoreIncrement(int scoreAmount)
    {
        turnScore += scoreAmount;
        gamePanelManager.UpdateTurnScore(currentScore + turnScore);
    }

    public void EnemyScoreDecrement(int scoreAmount)
    {
        enemyScore += scoreAmount;
        gamePanelManager.UpdateEnemyScore(currentScore - enemyScore);
    }

    public void OnNotesStartComplete()
    {
        SwitchState(EncounterConstants.GameplayState.TurnPlay);
    }

    public void OnNotesEndComplete()
    {
        SwitchState(EncounterConstants.GameplayState.EnemyIntro);
    }
}