using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//// Attack Status
//public class AttackStatus
//{
//    public bool isEnemy = false;
//    public bool isActive = false;

//    public void OnStart(MoveEffects effectI)
//    {
//        //Call Functions For Enabling Here
//        isActive = false;
//    }

//    public void OnStop()
//    {
//        isActive = true;
//    }
//}

public class EncounterGameManager : MonoBehaviour
{
    [SerializeField]
    NoteGeneratorManager noteManager;
    [SerializeField]
    GameplayPanelManager gamePanelManager;
    [SerializeField]
    ActiveMovesPanel activeMovesObject;
    [SerializeField]
    Camera mainCamera;

    [SerializeField]
    Light crowdLight;
    [SerializeField]
    CrowdEntitiesScript crowdEntities;
    [SerializeField]
    EnemyEntitiesScrpt enemyEntities;

    public int currentScore;

    public Sprite AmplifierSprite;
    public Sprite CrazyStandSprite;
    public Sprite RythmSprite;
    public Sprite StompSprite;

    public Dictionary<MoveEffects, bool> movesActive = new Dictionary<MoveEffects, bool>();

    int turnScore;
    int enemyScore;

    GameplayState currentState = GameplayState.StartGame;

    AudioManager audioManager;

    public delegate void AnimationCallback();
    // Start is called before the first frame update

    Image[] activeImages;
    Sequence animateMoveEffects;
    EncounterConstants encounterConstants;
    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();

        // Take values from encounter constants
        encounterConstants = FindObjectOfType<EncounterConstants>();

        movesActive.Clear();
        // Add Attack statuses
        foreach (PlayerMove move in encounterConstants.GuitarMoves)
        {
            if(move.effect != MoveEffects.None)
            {
                movesActive.Add(move.effect, false);
            }
        }
        foreach (PlayerMove move in encounterConstants.BassMoves)
        {
            if (move.effect != MoveEffects.None)
            {
                movesActive.Add(move.effect, false);
            }
        }
        foreach (PlayerMove move in encounterConstants.KeytarMoves)
        {
            if (move.effect != MoveEffects.None)
            {
                movesActive.Add(move.effect, false);
            }
        }
        foreach (PlayerMove move in encounterConstants.DrumMoves)
        {
            if (move.effect != MoveEffects.None)
            {
                movesActive.Add(move.effect, false);
            }
        }


        activeImages = activeMovesObject.GetComponentsInChildren<Image>(true);
        foreach (Image activeImage in activeImages)
        {
            activeImage.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        int index = 0;
        foreach (MoveEffects moveKey in movesActive.Keys)
        {
            switch (moveKey)
            {
                case MoveEffects.Amplifier:
                    activeImages[index].sprite = AmplifierSprite;
                    break;
                case MoveEffects.CrazyStand:
                    activeImages[index].sprite = CrazyStandSprite;
                    break;
                case MoveEffects.Rhythm:
                    activeImages[index].sprite = RythmSprite;
                    break;
                case MoveEffects.Stomp:
                    activeImages[index].sprite = StompSprite;
                    break;
            }
            activeImages[index].gameObject.name = moveKey.ToString();
            activeImages[index].gameObject.SetActive(true);
            index++;
        }

        FadeOutOverlayUI();
        crowdLight.DOIntensity(0.0f, 0.2f);
        audioManager.PlayCrowdEffect(CrowdEffects.CrowdIdle);
    }

    // Update is called once per frame
    void Update()
    {
        // This Update loop will be commented out when actual gameplay is written
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 8") || Input.GetKeyDown("joystick button 10"))
        {
            switch(currentState)
            {
                case GameplayState.StartGame:
                    audioManager.PlaySoundEffect(SoundEffects.MenuNext);
                    SwitchState(GameplayState.StartGameUI);
                    break;
                case GameplayState.StartGameUI:
                    audioManager.PlaySoundEffect(SoundEffects.MenuNext);
                    SwitchState(GameplayState.TurnIntro);
                    break;
                case GameplayState.TurnPlayOut:
                    audioManager.PlaySoundEffect(SoundEffects.MenuNext);
                    SwitchState(GameplayState.TurnIntro);
                    break;
            }
        }

        OnProcessState(currentState);
    }

    void SwitchState(GameplayState newState)
    {
        bool switchAllowed = false;

        // Do check for if switch is possible
        switch (currentState)
        {
            case GameplayState.StartGame:
                {
                    switchAllowed = newState == GameplayState.StartGameUI;
                }
                break;
            case GameplayState.StartGameUI:
                {
                    switchAllowed = newState == GameplayState.TurnIntro;
                }
                break;
            case GameplayState.TurnIntro:
                {
                    switchAllowed = newState == GameplayState.TurnPlay;
                }
                break;
            case GameplayState.TurnPlay:
                {
                    switchAllowed = newState == GameplayState.TurnPlayOut;
                }
                break;
            case GameplayState.TurnPlayOut:
                {
                    StopMoveEffect(MoveEffects.Amplifier);
                    //gameManager.StopMoveEffect(MoveEffects.PickUps);

                    switchAllowed = newState == GameplayState.EndGameUI || newState == GameplayState.TurnIntro;
                }
                break;
            case GameplayState.EndGameUI:
                {
                    switchAllowed = newState == GameplayState.EndGame;
                }
                break;
            case GameplayState.EndGame:
                {
                    switchAllowed = false;
                }
                break;
        }

        if (switchAllowed)
        {
            OnExitState(currentState);
            currentState = newState;
            OnEnterState(currentState);
        }
    }

    // Check entry to stateEnter
    void OnEnterState(GameplayState stateEnter)
    {
        switch (stateEnter)
        {

            case GameplayState.StartGame:
                {
                    //noteManager.FadeOutUI();
                }
                break;
            case GameplayState.StartGameUI:
                {
                    audioManager.PlayCrowdEffect(CrowdEffects.CrowdStart);
                    turnScore = 0;
                    currentScore = 0;
                    FadeInOverlayUI();
                }
                break;
            case GameplayState.TurnIntro:
                {
                    turnScore = 0;
                    StartNoteGame();
                }
                break;
            case GameplayState.TurnPlay:
                {
                    audioManager.PlayCrowdEffect(CrowdEffects.CrowdSet);
                }
                break;
            case GameplayState.TurnPlayOut:
                {
                    audioManager.PlayCrowdEffect(CrowdEffects.CrowdIdle);
                    ChangeScore();

                    crowdLight.DOIntensity(3.0f, 1.0f);
                    
                    MoveCameraToOverview();
                    MoveCrowds();
                }
                break;
            case GameplayState.EndGameUI:
                {
                    FadeOutOverlayUI();
                }
                break;
            case GameplayState.EndGame:
                {

                }
                break;
        }
    }
    void OnExitState(GameplayState stateExit)
    {
        switch (stateExit)
        {

            case GameplayState.StartGame:
                {
                }
                break;
            case GameplayState.StartGameUI:
                {
                }
                break;
            case GameplayState.TurnIntro:
                {

                }
                break;
            case GameplayState.TurnPlay:
                {
                }
                break;
            case GameplayState.TurnPlayOut:
                {
                }
                break;
            case GameplayState.EndGameUI:
                {

                }
                break;
            case GameplayState.EndGame:
                {

                }
                break;
        }
    }
    void OnProcessState(GameplayState stateProcess)
    {

        switch (stateProcess)
        {
            case GameplayState.StartGame:
                {

                }
                break;
            case GameplayState.StartGameUI:
                {

                }
                break;
            case GameplayState.TurnIntro:
                {

                }
                break;
            case GameplayState.TurnPlay:
                {
                }
                break;
            case GameplayState.TurnPlayOut:
                {
                }
                break;
            case GameplayState.EndGameUI:
                {

                }
                break;
            case GameplayState.EndGame:
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

            if(Random.Range(0f, 1f) < encounterConstants.crowdMoveProbability) // 
            {
                Vector3 newPos = new Vector3(xValue + ((xValue > 0 ? 1 : -1) * Random.Range(1, 3f)), encounterConstants.crowdYPosition, Random.Range(10f, -10f));

                moveSequence.Insert(delay, person.DOLocalMove(newPos, 1.5f).SetEase(Ease.InOutQuad));
                delay += 0.5f;
            }
        }

        moveSequence.OnComplete(() => {
            crowdLight.DOIntensity(0.0f, 1f);
            SwitchState(GameplayState.TurnIntro);
        });
    }

    public void MoveCameraToPlayer()
    {
        mainCamera.transform.DOMove(encounterConstants.cameraTurnPos, 0.75f).SetEase(Ease.OutBack);
        mainCamera.transform.DORotate(encounterConstants.cameraTurnRot, 1f).SetEase(Ease.OutSine);
    }

    public void MoveCameraToFocus(Transform bandMemberTransform)
    {
        mainCamera.transform.DOMove(encounterConstants.cameraFocusPos, 1f).SetEase(Ease.OutBack).OnComplete(() => {
            mainCamera.transform.DOLookAt(bandMemberTransform.position + encounterConstants.cameraFocusOffset, 1f).SetEase(Ease.OutBack);
        });
    }

    public void MoveCameraToOverview()
    {
        mainCamera.transform.DOMove(encounterConstants.cameraOverviewPos, 0.75f).SetEase(Ease.OutBack);
        mainCamera.transform.DORotate(encounterConstants.cameraOverviewRot, 1f).SetEase(Ease.OutSine);
    }

    public void MoveCameraToEnemy()
    {
        mainCamera.transform.DOMove(encounterConstants.cameraEnemyPos, 0.75f).SetEase(Ease.OutBack);
        mainCamera.transform.DORotate(encounterConstants.cameraEnemyRot, 1f).SetEase(Ease.OutSine);
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
        SwitchState(GameplayState.TurnPlay);
    }

    public void OnSetComplete()
    {
        audioManager.PlaySoundEffect(SoundEffects.SetComplete);
        SwitchState(GameplayState.TurnPlayOut);
    }

    public void ShowGamePanelTurn()
    {
        gamePanelManager.ShowTurnUI(turnScore);
    }

    public void ShowGamePanelEnemy()
    {
        gamePanelManager.ShowEnemyUI(enemyScore);
    }

    public void StartMoveEffect(MoveEffects attack)
    {
        if (!movesActive[attack])
        {
            movesActive[attack] = true;

            Image moveImage = activeMovesObject.transform.Find(attack.ToString()).GetComponent<Image>();

            moveImage.transform.DOScale(0f, 0f);
            moveImage.DOFade(1.0f, encounterConstants.moveEffectDuration);
            moveImage.transform.DOScale(1.0f, encounterConstants.moveEffectDuration).SetEase(Ease.InOutBack);
        }
    }

    public void AnimateEffectAction(MoveEffects attack)
    {
        Debug.Log("Animate Effect Action " + attack);
        if (movesActive[attack])
        {
            Image moveImage = activeMovesObject.transform.Find(attack.ToString()).GetComponent<Image>();
            animateMoveEffects.Complete();

            animateMoveEffects = DOTween.Sequence();
            moveImage.transform.localScale = Vector3.one;
            moveImage.color = Color.white;

            animateMoveEffects.Insert(0f, moveImage.transform.DOScale(1.2f, encounterConstants.moveAnimateDuration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutBack));
            animateMoveEffects.Insert(0f, moveImage.DOColor(encounterConstants.effectAnimateColor, encounterConstants.moveAnimateDuration).SetLoops(2, LoopType.Yoyo));
        }
    }

    public void StopMoveEffect(MoveEffects attack)
    {
        if (movesActive[attack])
        {
            movesActive[attack] = false;

            Image moveImage = activeMovesObject.transform.Find(attack.ToString()).GetComponent<Image>();

            moveImage.DOFade(0.0f, encounterConstants.moveEffectDuration);
            moveImage.transform.DOScale(0.0f, encounterConstants.moveEffectDuration).SetEase(Ease.InOutBack);
        }
    }
}