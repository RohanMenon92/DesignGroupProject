using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class NoteGeneratorManager : MonoBehaviour
{
    [SerializeField]
    PlayerEntitiesScript playerEntities;

    [SerializeField]
    List<TimelineAsset> playableSongs;

    [SerializeField]
    List<Sprite> buttonImages;

    [SerializeField]
    Transform unusedNotePool;

    [SerializeField]
    List<NoteGeneratorScript> noteGenerators;

    [SerializeField]
    List<SelectorScript> selectors;

    [SerializeField]
    GameObject notePrefab;
    
    public Transform reputationPointerTransform;

    [SerializeField]
    KnobControlScript knobControl;

    [SerializeField]
    CanvasGroup moveCanvas;

    [SerializeField]
    TextMeshProUGUI moveDescription;

    [SerializeField]
    TextMeshProUGUI lockCounter;

    [SerializeField]
    Image moveLock;

    [SerializeField]
    TextMeshProUGUI moveTitle;

    [SerializeField]
    bool stateMachineDebug = false;

    PlayableDirector selfPlayableDirector;

    // Start is called before the first frame update
    EncounterGameManager gameManager;
    AudioManager audioManager;

    private NotesGameStates currentState = NotesGameStates.Idle;
    //private NotesGameStates lastState = NotesGameStates.Idle;
    int currentPlayer = 0;
    int currentMove = 0;

    int currSet = 0;

    bool isTurnVisible;

    [SerializeField]
    Sprite notePerfect;
    [SerializeField]
    Sprite noteGood;
    [SerializeField]
    Sprite noteOK;

    public bool songFinishRecieved = false;

    public delegate void AnimationCallback();
    AnimationCallback playSongCallback = null;

    bool initialFadeComplete = false;

    EncounterConstants encounterConstants;

    HypeMeterUI hypeMeterUI;

    Sequence descriptionSequence;

    public PlayerMove[][] playerMoves;
    private void Awake()
    {
        selfPlayableDirector = GetComponent<PlayableDirector>();
        gameManager = FindObjectOfType<EncounterGameManager>();
        audioManager = FindObjectOfType<AudioManager>();
        hypeMeterUI = FindObjectOfType<HypeMeterUI>();

        // Take values from encounter constants
        encounterConstants = FindObjectOfType<EncounterConstants>();

        playerMoves = new PlayerMove[][] { encounterConstants.GuitarMoves, encounterConstants.BassMoves, encounterConstants.KeytarMoves, encounterConstants.DrumMoves };
    }

    void Start()
    {
        for (int i = 0; i < encounterConstants.notePoolSize; i++)
        {
            GameObject newNote = Instantiate(notePrefab, unusedNotePool);
            newNote.SetActive(false);
        }
        ClearUI(() => {
            initialFadeComplete = true;
        });
    }

    public void OnWrongPress()
    {
        hypeMeterUI.DecrementHypeValue(encounterConstants.hypeWrongPunishment);
        gameManager.TurnScoreDecrement(encounterConstants.scoreWrongPunishment);
        audioManager.PlayMusicEffect(MusicEffects.PlayerWrong, currentPlayer);
        audioManager.PlaySoundEffect(SoundEffects.WrongPress);
    }

    public void NoteMissed()
    {
        hypeMeterUI.DecrementHypeValue(encounterConstants.hypeMissPunishment);
        gameManager.TurnScoreDecrement(encounterConstants.scoreMissPunishment);
        audioManager.PlayMusicEffect(MusicEffects.PlayerMiss, currentPlayer);
        audioManager.PlaySoundEffect(SoundEffects.SetFailure);
    }

    public NotesGameStates GetCurrentState()
    {
        return currentState;
    }

    void NextInstrumentSelection()
    {
        audioManager.PlaySoundEffect(SoundEffects.PlayerSelect);
        currentPlayer++;
        if (currentPlayer >= playerEntities.stageEntities.Count)
        {
            currentPlayer = 0;
        }

        playerEntities.TransitionToPlayer(currentPlayer);
        FadeColorUI(encounterConstants.PlayerColors[currentPlayer], null, false);
        knobControl.NextSelection();
    }

    void LastInstrumentSelection()
    {
        audioManager.PlaySoundEffect(SoundEffects.PlayerSelect);
        currentPlayer--;
        if (currentPlayer < 0f)
        {
            currentPlayer = playerEntities.stageEntities.Count - 1;
        }

        playerEntities.TransitionToPlayer(currentPlayer);
        FadeColorUI(encounterConstants.PlayerColors[currentPlayer], null, false);
        knobControl.LastSelection();
    }

    void UpdateMoveDescription()
    {
        descriptionSequence.Complete(true);

        descriptionSequence = DOTween.Sequence();

        bool isUnlocked = playerMoves[currentPlayer][currentMove].IsUnlocked();

        descriptionSequence.Insert(0f, moveLock.DOFade(0f, encounterConstants.descriptionTransition/3));
        descriptionSequence.Insert(0f, lockCounter.DOFade(0f, encounterConstants.descriptionTransition / 3));
        descriptionSequence.Insert(0.1f, moveTitle.DOFade(0f, encounterConstants.descriptionTransition / 3));
        descriptionSequence.Insert(0.1f, moveDescription.DOFade(0f, encounterConstants.descriptionTransition / 3).OnComplete(() => {
            // Update Values
            moveDescription.text = playerMoves[currentPlayer][currentMove].description;
            moveTitle.text = playerMoves[currentPlayer][currentMove].name;
            if (isUnlocked)
            {
                moveDescription.color = encounterConstants.MoveColors[currentMove];
            } else
            {

                moveDescription.color = Color.white;
                lockCounter.text = playerMoves[currentPlayer][currentMove].currentLock + " turns";

                moveLock.color = encounterConstants.MoveColors[currentMove];
                lockCounter.color = encounterConstants.MoveColors[currentMove];
            }
        }));


        Color moveColor = moveCanvas.GetComponent<Image>().color;

        if (isUnlocked)
        {
            moveColor = encounterConstants.PlayerColors[currentPlayer];

        } else
        {
            moveColor = encounterConstants.moveLockColor;
            descriptionSequence.Insert(encounterConstants.descriptionTransition / 2, moveLock.DOFade(1f, 1f));
            descriptionSequence.Insert(encounterConstants.descriptionTransition / 2, lockCounter.DOFade(1f, 1f));
        }
        descriptionSequence.Insert(encounterConstants.descriptionTransition / 2, moveCanvas.GetComponent<Image>().DOColor(moveColor, encounterConstants.descriptionTransition / 2));
        descriptionSequence.Insert(encounterConstants.descriptionTransition / 2, moveTitle.DOFade(1f, encounterConstants.descriptionTransition / 2));
        descriptionSequence.Insert(encounterConstants.descriptionTransition / 2, moveDescription.DOFade(1f, encounterConstants.descriptionTransition / 2));
    }

    void NextMoveSelection()
    {
        audioManager.PlaySoundEffect(SoundEffects.MoveSelect);
        currentMove++;
        if (currentMove >= playerMoves[currentPlayer].Length)
        {
            currentMove = 0;
        }

        UpdateMoveDescription();
        knobControl.NextSelection();
    }

    void LastMoveSelection()
    {
        audioManager.PlaySoundEffect(SoundEffects.MoveSelect);
        currentMove--;
        if (currentMove < 0f)
        {
            currentMove = playerMoves[currentPlayer].Length - 1;
        }

        UpdateMoveDescription();
        knobControl.LastSelection();
    }

    void TakeInput()
    {
        switch (currentState)
        {
            case NotesGameStates.Start:
                break;

            case NotesGameStates.Idle:
                break;

            case NotesGameStates.Intro:
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown("joystick button 5"))
                {
                    LastInstrumentSelection();
                }
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown("joystick button 4"))
                {
                    NextInstrumentSelection();
                }
                
                if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 1"))
                {
                    OnInstrumentSelect();
                }
                break;
            case NotesGameStates.MoveSelect:
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown("joystick button 5"))
                {
                    LastMoveSelection();
                }
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown("joystick button 4"))
                {
                    NextMoveSelection();
                }
                
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 1"))
                {
                    OnMoveSelected();
                }
                break;
            case NotesGameStates.Playing:
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown("joystick button 3"))
                {
                    selectors[0].SelectorPressed(encounterConstants.FretColors[0]);
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown("joystick button 2"))
                {
                    selectors[1].SelectorPressed(encounterConstants.FretColors[1]);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown("joystick button 1"))
                {
                    selectors[2].SelectorPressed(encounterConstants.FretColors[2]);
                }
                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown("joystick button 0"))
                {
                    selectors[3].SelectorPressed(encounterConstants.FretColors[3]);
                }

                if (Input.GetKeyDown(KeyCode.Space) || (Input.GetKeyDown("joystick button 4") && Input.GetKeyDown("joystick button 5")))
                {
                    hypeMeterUI.TryHype();
                }

                break;
            case NotesGameStates.EnemyIntro:
                break;
            case NotesGameStates.Enemy:
                break;
            case NotesGameStates.EndSet:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTurnVisible)
        {
            TakeInput();
        }

        if(initialFadeComplete)
        {
            OnProcessState(currentState);
        }
    }

    void SwitchState(NotesGameStates newState)
    {
        bool switchAllowed = false;

        // Do check for if switch is possible
        switch (currentState)
        {
            case NotesGameStates.Idle:
                {
                    switchAllowed = newState == NotesGameStates.Start;
                }
                break;
            case NotesGameStates.Start:
                {
                    switchAllowed = newState == NotesGameStates.Intro;
                }
                break;
            case NotesGameStates.Intro:
                {
                    switchAllowed = newState == NotesGameStates.MoveSelect;
                }
                break;
            case NotesGameStates.MoveSelect:
                {
                    switchAllowed = newState == NotesGameStates.Playing;
                }
                break;
            case NotesGameStates.Playing:
                {
                    switchAllowed = newState == NotesGameStates.EnemyIntro;
                }
                break;
            case NotesGameStates.EnemyIntro:
                {
                    switchAllowed = newState == NotesGameStates.Enemy;
                }
                break;
            case NotesGameStates.Enemy:
                {
                    switchAllowed = newState == NotesGameStates.Intro || newState == NotesGameStates.EndSet;
                }
                break;
            case NotesGameStates.EndSet:
                {
                    switchAllowed = newState == NotesGameStates.Idle;
                }
                break;
        }

        if (stateMachineDebug)
        {
            Debug.Log("Trying to switch to:" + newState + " Allowed:" + switchAllowed);
        }

        if (switchAllowed)
        {
            OnExitState(currentState);
            currentState = newState;
            OnEnterState(newState);
        }
    }

    // Check entry to stateEnter
    void OnEnterState(NotesGameStates stateEnter)
    {
        if(stateMachineDebug)
        {
            Debug.Log("On Enter state " + stateEnter);
        }
        switch (stateEnter)
        {
            case NotesGameStates.Idle:
                {
                }
                break;
            case NotesGameStates.Start:
                {
                    Debug.Log("In Start ");
                    currSet = 0;

                    OnStartGame(() => {
                        SwitchState(NotesGameStates.Intro);
                    });
                    // Start Game Function Fade In UI Here, call from EncounterGameManager?
                }
                break;
            case NotesGameStates.Intro:
                {
                    // Call function to fade UI and call next state
                    Debug.Log("In Intro Enter " + currSet);

                    gameManager.MoveCameraToPlayer();

                    OnIntro(() => {
                        isTurnVisible = true;
                        // OnIntroComplete
                    });
                }
                break;
            case NotesGameStates.MoveSelect:
                {
                    // Call function to fade UI and call next state
                    Debug.Log("In MoveSelect " + currentPlayer);

                    gameManager.MoveCameraToFocus(playerEntities.stageEntities[currentPlayer]);

                    OnMoveSelect(() => {
                        gameManager.OnNotesStartComplete();
                        // On Move Select Complete
                    });
                }
                break;
            case NotesGameStates.Playing:
                {
                    Debug.Log("In Playing " + currentMove);
                    songFinishRecieved = false;

                    OnPlay(() =>
                    {
                        // Reset song finished
                        SelectSongAndPlay(() =>
                        {
                            OnPlayEnd(() =>
                            {
                                songFinishRecieved = true;
                            });
                        });
                        // TODO :: Play Song here
                    });
                }
                break;
            case NotesGameStates.EnemyIntro:
                {
                    Debug.Log("In Enemy Enter Intro");
                    gameManager.MoveCameraToEnemy();

                    OnEnemyIntro(() => {
                        SwitchState(NotesGameStates.Enemy);
                    });
                }
                break;
            case NotesGameStates.Enemy:
                {
                    songFinishRecieved = false;
                    Debug.Log("In Enemy Enter");
                    //isTurnVisible = true;
                    ////TransitionStageElements();
                    SelectEnemySongAndPlay(() =>
                    {
                        OnPlayEnd(() =>
                        {
                            songFinishRecieved = true;
                        });
                    });

                    // current player calls here

                }
                break;
            case NotesGameStates.EndSet:
                {
                    ReducePlayerMoves();
                    ClearUI(() =>
                    {
                        SwitchState(NotesGameStates.Idle);
                        gameManager.OnSetComplete();
                        //gameManager switch state
                    });
                }
                break;
        }
    }

    void ReducePlayerMoves()
    {
        // Reduce Player Moves
        foreach(PlayerMove[] playerMove in playerMoves)
        {
            foreach (PlayerMove move in playerMove)
            {
                move.DecrementLock();
            }
        }
    }

    void OnInstrumentSelect()
    {
        audioManager.PlaySoundEffect(SoundEffects.MenuNext);
        knobControl.OnPlayerSelected(playerMoves[currentPlayer]);
        SwitchState(NotesGameStates.MoveSelect);
    }

    void OnMoveSelected()
    {

        if(playerMoves[currentPlayer][currentMove].IsUnlocked())
        {
            SelectMove();
        } else
        {
            CannotSelectMove();
        }
    }

    void CannotSelectMove()
    {
        audioManager.PlaySoundEffect(SoundEffects.SetFailure);
        knobControl.OnMoveLocked(currentMove);
    }

    void SelectMove()
    {
        // Lock the move
        if(playerMoves[currentPlayer][currentMove].turnLock > 0)
        {
            playerMoves[currentPlayer][currentMove].LockMove();
        }

        // Enable Attack Effect
        if (playerMoves[currentPlayer][currentMove].effect != MoveEffects.None)
        {
            gameManager.EnableAttackEffect(playerMoves[currentPlayer][currentMove].effect);
        }

        knobControl.OnMoveSelected(currentMove);
        HideMoveDescription();
        audioManager.PlaySoundEffect(SoundEffects.MenuNext);
        SwitchState(NotesGameStates.Playing);
    }

    IEnumerator PlaySong()
    {
        selfPlayableDirector.playableAsset = playableSongs[currSet];

        selfPlayableDirector.Play();

        yield return new WaitForSeconds(encounterConstants.MusicPlayDelay);
        audioManager.StartMusic(currSet);
    }

    private void SelectSongAndPlay(AnimationCallback animCallback)
    {
        Debug.Log("Select Song and Play");
        
        StartCoroutine(PlaySong());
        playSongCallback = animCallback;
    }

    IEnumerator PlayEnemySong()
    {
        selfPlayableDirector.playableAsset = playableSongs[currSet];
        selfPlayableDirector.Play();

        yield return new WaitForSeconds(encounterConstants.MusicPlayDelay);
        audioManager.StartMusic(currSet);
    }

    private void SelectEnemySongAndPlay(AnimationCallback animCallback)
    {
        Debug.Log("Select Enemy Song and Play");
        StartCoroutine(PlayEnemySong());
        playSongCallback = animCallback;
    }

    public void OnPlaySongComplete()
    {
        Debug.Log("ON PLAY SONG COMPLETE");
        playSongCallback?.Invoke();

        playSongCallback = null;
    }

    void OnExitState(NotesGameStates stateExit)
    {
        if (stateMachineDebug)
        {
            Debug.Log("On Exit state " + stateExit);
        }

        switch (stateExit)
        {
            case NotesGameStates.Idle:
                {
                }
                break;
            case NotesGameStates.Start:
                {
                }
                break;
            case NotesGameStates.Intro:
                {
                }
                break;
            case NotesGameStates.MoveSelect:
                {
                }
                break;
            case NotesGameStates.Playing:
                {
                }
                break;
            case NotesGameStates.EnemyIntro:
                {
                }
                break;
            case NotesGameStates.Enemy:
                {
                }
                break;
            case NotesGameStates.EndSet:
                {
                }
                break;
        }
    }
    void OnProcessState(NotesGameStates stateProcess)
    {
        switch (stateProcess)
        {
            case NotesGameStates.Idle:
                {
                }
                break;
            case NotesGameStates.Start:
                {
                }
                break;
            case NotesGameStates.Intro:
                {
                }
                break;
            case NotesGameStates.MoveSelect:
                {
                }
                break;
            case NotesGameStates.Playing:
                {
                    // Switch state to enemy intro after song is finished and all notes are collected
                    if(songFinishRecieved && unusedNotePool.childCount == encounterConstants.notePoolSize)
                    {
                        // Should only be called once
                        songFinishRecieved = false;

                        SwitchState(NotesGameStates.EnemyIntro);
                    }
                }
                break;
            case NotesGameStates.EnemyIntro:
                {
                }
                break;
            case NotesGameStates.Enemy:
                {
                    // Switch state to enemy intro after song is finished and all notes are collected
                    if (songFinishRecieved && unusedNotePool.childCount == encounterConstants.notePoolSize)
                    {
                        EnemySongComplete();
                    }
                }
                break;
            case NotesGameStates.EndSet:
                {
                }
                break;
        }
    }

    private void EnemySongComplete()
    {    
        // Should only be called once
        songFinishRecieved = false;

        currSet++;
        if (currSet >= encounterConstants.setLength)
        {
            Debug.Log("Switching to end");
            SwitchState(NotesGameStates.EndSet);
        }
        else
        {
            ClearUI(null);
            Debug.Log("Switching to intro");
            SwitchState(NotesGameStates.Intro);
        }
    }

    public void StartGame()
    {
        SwitchState(NotesGameStates.Start);
    }

    void OnStartGame(AnimationCallback animCallback)
    {
        currentPlayer = -1;

        // Set initial sizes and scales
        playerEntities.StartTurn();

        animCallback();
    }

    void OnNoteGeneratorFade(AnimationCallback animCallback)
    {
        transform.localScale = Vector3.one * 0.8f;

        // Start animations and on complete, go to Intro
        transform.DOScale(1f, 0.25f);
        GetComponent<CanvasGroup>().DOFade(1f, 1f).OnComplete(() => {
            animCallback?.Invoke();
        });
    }

    public void CallNoteGenerator(int noteGeneratorID)
    {
        NoteScript newNote = GetUnusedNote(noteGeneratorID);
        Image noteImage = newNote.GetComponent<Image>();

        // select note generator as parent
        newNote.noteTypeID = noteGeneratorID;
        newNote.transform.SetParent(noteGenerators[noteGeneratorID].transform.parent);
        newNote.GetComponent<RectTransform>().localPosition = noteGenerators[noteGeneratorID].GetComponent<RectTransform>().localPosition;
        //noteImage.color = FretColors[noteGeneratorID];

        // Reset for animation
        newNote.isCollected = false;
        noteImage.sprite = buttonImages[noteGeneratorID];
        noteImage.color = new Color(noteImage.color.r, noteImage.color.g, noteImage.color.b, 0f);
        noteImage.transform.localScale = Vector3.zero;

        newNote.gameObject.SetActive(true);
        // Do Fade In Animation
        Sequence createSequence = DOTween.Sequence();

        createSequence.Insert(0f, noteImage.DOFade(1.0f, 0.3f));
        createSequence.Insert(0f, newNote.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutBack)).OnComplete(() =>
        {
        });

        createSequence.Play();
    }

    public void CollectEnemyNote(NoteScript noteToStore)
    {
        // Calculate random accuracy
        float accuracyRating = UnityEngine.Random.Range(10 * (1 - encounterConstants.enemyDifficulty), 60f - (20 * encounterConstants.enemyDifficulty));

        AccuracyRating currRating = AccuracyRating.OK;
        if (accuracyRating < encounterConstants.accuracyPerfect)
        {
            currRating = AccuracyRating.Perfect;
        }
        else if (accuracyRating < encounterConstants.accuracyGood)
        {
            currRating = AccuracyRating.Good;
        }

        Color gradeColor = encounterConstants.missedNoteColor;
        int scoreDecrement = 0;
        Sprite noteSprite = noteOK;

        switch (currRating)
        {
            case AccuracyRating.Perfect:
                gradeColor = encounterConstants.EnemyGradeColors[0];
                noteSprite = notePerfect;
                scoreDecrement = encounterConstants.scorePerfectMultiplier;

                audioManager.PlaySoundEffect(SoundEffects.Perfect);
                break;
            case AccuracyRating.Good:
                gradeColor = encounterConstants.EnemyGradeColors[1];
                noteSprite = noteGood;
                scoreDecrement = encounterConstants.scoreGoodMultiplier;

                audioManager.PlaySoundEffect(SoundEffects.Good);
                break;
            case AccuracyRating.OK:
                gradeColor = encounterConstants.EnemyGradeColors[2];
                noteSprite = noteOK;
                scoreDecrement = encounterConstants.scoreOKMultiplier;
                break;
        }

        // Add handicap based on difficulty
        float handicapScore = scoreDecrement - (scoreDecrement * (0.2f * (1f - encounterConstants.enemyDifficulty)));
        gameManager.EnemyScoreDecrement((int)handicapScore);

        noteToStore.PlaySuccessAnimation(gradeColor, noteSprite);
    }

    public void CollectPlayerNote(NoteScript noteToStore)
    {
        // Calculate Score
        float accuracyRating = Mathf.Abs(noteToStore.transform.position.x - selectors[noteToStore.noteTypeID].transform.position.x);
        AccuracyRating currRating = AccuracyRating.OK;
        if (accuracyRating < encounterConstants.accuracyPerfect)
        {
            currRating = AccuracyRating.Perfect;
        } else if (accuracyRating < encounterConstants.accuracyGood)
        {
            currRating = AccuracyRating.Good;
        }

        Color gradeColor = encounterConstants.missedNoteColor;
        Sprite noteSprite = noteOK;
        int scoreMultiplier = 0;

        switch (currRating)
        {
            case AccuracyRating.Perfect:
                gradeColor = encounterConstants.PlayerGradeColors[0];
                noteSprite = notePerfect;
                scoreMultiplier = encounterConstants.scorePerfectMultiplier;

                audioManager.PlaySoundEffect(SoundEffects.Perfect);
                break;
            case AccuracyRating.Good:
                gradeColor = encounterConstants.PlayerGradeColors[1];
                noteSprite = noteGood;
                scoreMultiplier = encounterConstants.scoreGoodMultiplier;

                audioManager.PlaySoundEffect(SoundEffects.Good);
                break;
            case AccuracyRating.OK:
                gradeColor = encounterConstants.PlayerGradeColors[2];
                noteSprite = noteOK;
                scoreMultiplier = encounterConstants.scoreOKMultiplier;
                break;
        }


        hypeMeterUI.IncrementHypeValue((int)playerMoves[currentPlayer][currentMove].hypeRate);
        gameManager.TurnScoreIncrement((int)(scoreMultiplier * playerMoves[currentPlayer][currentMove].score));

        audioManager.PlayMusicEffect(MusicEffects.PlayerCorrect, currentPlayer);
        noteToStore.PlaySuccessAnimation(gradeColor, noteSprite);
    }

    NoteScript GetUnusedNote(int noteGeneratorID)
    {
        NoteScript newNote = unusedNotePool.GetComponentInChildren<NoteScript>(true);
        return newNote;
    }

    public void StoreNoteScript(NoteScript noteToStore)
    {
        noteToStore.GetComponent<Image>().color = encounterConstants.initialNoteColor;
        noteToStore.transform.SetParent(unusedNotePool);
        noteToStore.gameObject.SetActive(false);
        noteToStore.transform.localScale = Vector3.one;
    }

    void OnPlayEnd(AnimationCallback animCallback)
    {
        animCallback();
    }

    void OnIntro(AnimationCallback animCallback)
    {

        // Fade in Knob Controller
        Sequence uiFadeInSequence = DOTween.Sequence();

        knobControl.transform.localPosition = encounterConstants.KnobStartPos;
        knobControl.transform.localScale = Vector3.zero;
        uiFadeInSequence.Insert(0f, knobControl.transform.DOScale(2.5f, 1f).SetEase(Ease.InOutBack));
        uiFadeInSequence.Insert(0f, knobControl.GetComponent<CanvasGroup>().DOFade(1f, 1f));

        uiFadeInSequence.OnComplete(() =>
        {
            // Set knob control to player selector
            currentPlayer = 0;
            knobControl.OnPlayerSelector();
            playerEntities.TransitionToPlayer(0);
            FadeColorUI(encounterConstants.PlayerColors[0], animCallback, false);
        });
    }

    void OnMoveSelect(AnimationCallback animCallback)
    {
        currentMove = 0;
        // Set knob control to player selector
        knobControl.OnMoveSelector();

        // Fade in Knob Controller
        Sequence uiFadeInSequence = DOTween.Sequence();

        uiFadeInSequence.Insert(0f, moveCanvas.DOFade(1f, 0.5f));
        uiFadeInSequence.Insert(0.25f, moveCanvas.GetComponent<Image>().DOColor(encounterConstants.PlayerColors[currentPlayer], 0.5f));

        // ROHAN DO FADE UI HERE
        UpdateMoveDescription();

        //uiFadeInSequence.Insert(0f, moveCanvas.DOFade(1f, 0.5f));
        //uiFadeInSequence.Insert(0.25f, moveDescription.DOColor(encounterConstants.MoveColors[0], 0.5f));

        uiFadeInSequence.OnComplete(() =>
        {
            animCallback();
            // ROHAN DO FADE MOVE DETAILS HERE
        });
    }

    void OnPlay(AnimationCallback animCallback)
    {
        gameManager.ShowGamePanelTurn();

        OnNoteGeneratorFade(() => {
            FadeInSelectorUI(animCallback);
        });
    }

    void OnEnemyIntro(AnimationCallback animCallback)
    {
        gameManager.ShowGamePanelEnemy();

        Sequence uiFadeInSequence = DOTween.Sequence();

        uiFadeInSequence.Insert(0f, knobControl.transform.DOScale(0f, 1f).SetEase(Ease.InOutBack));
        uiFadeInSequence.Insert(0f, knobControl.GetComponent<CanvasGroup>().DOFade(0f, 1f));

        float delay = 0f;
        foreach (SelectorScript selector in selectors)
        {
            // Fade In NoteGenerators
            uiFadeInSequence.Insert(delay, selector.GetComponent<Image>().DOColor(new Color(0f, 0f, 0f, 0f), 0.5f));
            delay += 0.2f;
        }

        uiFadeInSequence.OnComplete(() =>
        {
            // Fade color to grey
            FadeColorUI(Color.grey, animCallback, true);
        });
    }

    public void FadeColorUI(Color uiColor, AnimationCallback animationCallback, bool isEnemySelect)
    {
        Sequence uiColorSequence = DOTween.Sequence();
        
        uiColorSequence.Insert(0f, knobControl.knobSprite.GetComponent<Image>().DOColor(uiColor, 0.5f));
        uiColorSequence.Insert(0f, this.GetComponent<Image>().DOColor(uiColor, 0.5f));

        float delay = 0.0f;
        foreach (NoteGeneratorScript noteGen in noteGenerators)
        {
            // Fade In NoteGenerators
            uiColorSequence.Insert(delay, noteGen.GetComponent<Image>().DOColor(uiColor, 0.5f));
            delay += 0.1f;
        }
        if (!isEnemySelect)
        {
            foreach (SelectorScript selector in selectors)
            {
                // Fade In NoteGenerators
                uiColorSequence.Insert(delay, selector.GetComponent<Image>().DOColor(uiColor, 0.5f));
                delay += 0.2f;
            }
        }

        // set is turn visible to true on complete
        uiColorSequence.OnComplete(() =>
        {
            animationCallback?.Invoke();
        });
    }

    public void FadeInSelectorUI(AnimationCallback animationCallback)
    {
        Sequence uiFadeInSequence = DOTween.Sequence();

        float delay = 0.0f;
        foreach (NoteGeneratorScript noteGen in noteGenerators)
        {
            // Fade In NoteGenerators
            noteGen.transform.localScale = Vector3.one * 0.75f;
            uiFadeInSequence.Insert(delay, noteGen.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 1f), 0.5f));
            uiFadeInSequence.Insert(delay + 0.1f, noteGen.transform.DOScale(1, 0.25f).SetEase(Ease.InOutBack));
            delay+= 0.1f;
        }
        foreach (SelectorScript selector in selectors)
        {
            // Fade In NoteGenerators
            selector.transform.localScale = Vector3.one * 1.2f;
            uiFadeInSequence.Insert(delay, selector.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 1f), 0.5f));
            uiFadeInSequence.Insert(delay + 0.1f, selector.transform.DOScale(1, 0.25f).SetEase(Ease.InOutBack));
            delay += 0.2f;
        }

        // set is turn visible to true on complete
        uiFadeInSequence.OnComplete(() =>
        {
            animationCallback();
        });
    }
    void HideMoveDescription()
    {
        moveCanvas.DOFade(0f, 1f);
    }

    public bool IsMoveUnlocked(int moveID)
    {
        return playerMoves[currentPlayer][moveID].IsUnlocked();
    }

    public void ClearUI(AnimationCallback animCallback)
    {
        Debug.Log("Set Complete Called");
        Sequence uiFadeOutSequence = DOTween.Sequence();

        isTurnVisible = false;

        playerEntities.ResetBandEndTurn();

        HideMoveDescription();

        uiFadeOutSequence.Insert(0f, GetComponent<CanvasGroup>().DOFade(0f, 1f));
        uiFadeOutSequence.Insert(0f, transform.DOScale(0.75f, 0.25f));

        foreach (NoteGeneratorScript noteGen in noteGenerators)
        {
            uiFadeOutSequence.Insert(0f, noteGen.transform.DOScale(0.1f, 0.35f));
            uiFadeOutSequence.Insert(0f, noteGen.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 0f), 0.5f));
        }
        foreach (SelectorScript selector in selectors)
        {
            uiFadeOutSequence.Insert(0f, selector.transform.DOScale(0.0f, 0.35f));
            uiFadeOutSequence.Insert(0f, selector.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 0f), 0.5f));
        }
        uiFadeOutSequence.Insert(0f, knobControl.GetComponent<CanvasGroup>().DOFade(0f, 0.5f));

        uiFadeOutSequence.OnComplete(() =>
        {
            animCallback?.Invoke();
        });
    }
}
