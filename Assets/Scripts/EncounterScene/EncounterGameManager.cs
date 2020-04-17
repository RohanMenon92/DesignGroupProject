using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterGameManager : MonoBehaviour
{
    [SerializeField]
    NoteGeneratorManager noteManager;
    [SerializeField]
    GameplayPanelManager gamePanelManager;
    [SerializeField]
    Camera mainCamera;

    [SerializeField]
    Light crowdLight;
    [SerializeField]
    CrowdEntitiesScript crowdEntities;
    [SerializeField]
    EnemyEntitiesScrpt enemyEntities;

    public int currentScore;
    
    int turnScore;
    int enemyScore;

    EncounterConstants.GameplayState currentState = EncounterConstants.GameplayState.StartGame;
    EncounterConstants.GameplayState lastState = EncounterConstants.GameplayState.StartGame;

    AudioManager audioManager;

    float crowdMoveProbability;
    float crowdYPosition;
    Vector3 cameraEnemyRot;
    Vector3 cameraEnemyPos;
    Vector3 cameraOverviewRot;
    Vector3 cameraOverviewPos;
    Vector3 cameraFocusOffset;
    Vector3 cameraFocusPos;
    Vector3 cameraTurnRot;
    Vector3 cameraTurnPos;

    public delegate void AnimationCallback();
    // Start is called before the first frame update

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();

        // Take values from encounter constants
        EncounterConstants encounterConstants = FindObjectOfType<EncounterConstants>();
        crowdMoveProbability = encounterConstants.crowdMoveProbability;
        cameraEnemyRot = encounterConstants.cameraEnemyRot;
        cameraEnemyPos = encounterConstants.cameraEnemyPos;
        cameraOverviewRot = encounterConstants.cameraOverviewRot;
        cameraOverviewPos = encounterConstants.cameraOverviewPos;
        cameraFocusOffset = encounterConstants.cameraFocusOffset;
        cameraFocusPos = encounterConstants.cameraFocusPos;
        cameraTurnRot = encounterConstants.cameraTurnRot;
        cameraTurnPos = encounterConstants.cameraTurnPos;
        crowdYPosition = encounterConstants.crowdYPosition;
    }

    void Start()
    {
        FadeOutOverlayUI();
        crowdLight.DOIntensity(0.0f, 0.2f);
        audioManager.PlayCrowdEffect(EncounterConstants.CrowdEffects.CrowdIdle);
    }

    // Update is called once per frame
    void Update()
    {
        // This Update loop will be commented out when actual gameplay is written
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 8") || Input.GetKeyDown("joystick button 10"))
        {
            switch(currentState)
            {
                case EncounterConstants.GameplayState.StartGame:
                    audioManager.PlaySoundEffect(EncounterConstants.SoundEffects.MenuNext);                    
                    SwitchState(EncounterConstants.GameplayState.StartGameUI);
                    break;
                case EncounterConstants.GameplayState.StartGameUI:
                    audioManager.PlaySoundEffect(EncounterConstants.SoundEffects.MenuNext);
                    SwitchState(EncounterConstants.GameplayState.TurnIntro);
                    break;
                case EncounterConstants.GameplayState.TurnPlayOut:
                    audioManager.PlaySoundEffect(EncounterConstants.SoundEffects.MenuNext);
                    SwitchState(EncounterConstants.GameplayState.TurnIntro);
                    break;
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
                    audioManager.PlayCrowdEffect(EncounterConstants.CrowdEffects.CrowdStart);
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
                    audioManager.PlayCrowdEffect(EncounterConstants.CrowdEffects.CrowdSet);
                }
                break;
            case EncounterConstants.GameplayState.TurnPlayOut:
                {
                    audioManager.PlayCrowdEffect(EncounterConstants.CrowdEffects.CrowdIdle);
                    ChangeScore();

                    crowdLight.DOIntensity(3.0f, 1.0f);
                    
                    MoveCameraToOverview();
                    MoveCrowds();
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

    void ChangeScore()
    {
        currentScore += turnScore - enemyScore;
        turnScore = 0;
        enemyScore = 0;

        gamePanelManager.OnTurnEnd(currentScore);
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

    void MoveCrowds()
    {
        Sequence moveSequence = DOTween.Sequence();
        float delay = 0.0f;

        foreach (Transform person in crowdEntities.transform)
        {
            float xValue = 10f * (float)-currentScore / 600f;

            if(Random.Range(0f, 1f) < crowdMoveProbability) // 
            {
                Vector3 newPos = new Vector3(xValue + ((xValue > 0 ? 1 : -1) * Random.Range(1, 3f)), crowdYPosition, Random.Range(10f, -10f));

                moveSequence.Insert(delay, person.DOLocalMove(newPos, 1.5f).SetEase(Ease.InOutQuad));
                delay += 0.5f;
            }
        }

        moveSequence.OnComplete(() => {
            crowdLight.DOIntensity(0.0f, 1f);
            SwitchState(EncounterConstants.GameplayState.TurnIntro);
        });
    }

    public void MoveCameraToPlayer()
    {
        mainCamera.transform.DOMove(cameraTurnPos, 0.75f).SetEase(Ease.OutBack);
        mainCamera.transform.DORotate(cameraTurnRot, 1f).SetEase(Ease.OutSine);
    }

    public void MoveCameraToFocus(Transform bandMemberTransform)
    {
        mainCamera.transform.DOMove(cameraFocusPos, 1f).SetEase(Ease.OutBack).OnComplete(() => {
            mainCamera.transform.DOLookAt(bandMemberTransform.position + cameraFocusOffset, 1f).SetEase(Ease.OutBack);
        });
    }

    public void MoveCameraToOverview()
    {
        mainCamera.transform.DOMove(cameraOverviewPos, 0.75f).SetEase(Ease.OutBack);
        mainCamera.transform.DORotate(cameraOverviewRot, 1f).SetEase(Ease.OutSine);
    }

    public void MoveCameraToEnemy()
    {
        mainCamera.transform.DOMove(cameraEnemyPos, 0.75f).SetEase(Ease.OutBack);
        mainCamera.transform.DORotate(cameraEnemyRot, 1f).SetEase(Ease.OutSine);
    }

    void StartNoteGame()
    {
        MoveCameraToPlayer();
        noteManager.StartGame();
    }

    public void TurnScoreIncrement(int scoreAmount)
    {
        turnScore += scoreAmount;
        gamePanelManager.UpdateTurnScore(currentScore + turnScore);
    }

    public void TurnScoreDecrement(int scoreAmount)
    {
        if(turnScore - scoreAmount > 0)
        {
            turnScore -= scoreAmount;
        }
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
        audioManager.PlaySoundEffect(EncounterConstants.SoundEffects.SetComplete);
        SwitchState(EncounterConstants.GameplayState.TurnPlayOut);
    }

    public void ShowGamePanelTurn()
    {
        gamePanelManager.ShowTurnUI();
    }

    public void ShowGamePanelEnemy()
    {
        gamePanelManager.ShowEnemyUI();
    }
}