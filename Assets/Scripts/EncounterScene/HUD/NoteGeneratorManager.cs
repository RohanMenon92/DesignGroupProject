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
    TextMeshProUGUI moveDescription;

    [SerializeField]
    bool stateMachineDebug = false;

    PlayableDirector selfPlayableDirector;

    // Start is called before the first frame update
    EncounterGameManager gameManager;
    AudioManager audioManager;


    private EncounterConstants.NotesGameStates currentState = EncounterConstants.NotesGameStates.Idle;
    private EncounterConstants.NotesGameStates lastState = EncounterConstants.NotesGameStates.Idle;
    int currentPlayer = 0;
    int currentMove = 0;
    int currSong = 0;

    public void OnWrongPress()
    {
        gameManager.TurnScoreDecrement(EncounterConstants.scoreWrongPunishment);
        audioManager.PlayMusicEffect(EncounterConstants.MusicEffects.PlayerWrong, currentPlayer);
        audioManager.PlaySoundEffect(EncounterConstants.SoundEffects.WrongPress);
    }

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

    void Start()
    {
        selfPlayableDirector = GetComponent<PlayableDirector>();
        gameManager = FindObjectOfType<EncounterGameManager>();
        audioManager = FindObjectOfType<AudioManager>();
        for (int i = 0; i < EncounterConstants.NotePoolSize; i++)
        {
            GameObject newNote = Instantiate(notePrefab, unusedNotePool);
            newNote.SetActive(false);
        }
        FadeOutUI(() => {
            initialFadeComplete = true;
        });
    }

    public void NoteMissed()
    {
        gameManager.TurnScoreDecrement(EncounterConstants.scoreMissPunishment);
        audioManager.PlayMusicEffect(EncounterConstants.MusicEffects.PlayerMiss, currentPlayer);
        audioManager.PlaySoundEffect(EncounterConstants.SoundEffects.SetFailure);
    }

    public EncounterConstants.NotesGameStates GetCurrentState()
    {
        return currentState;
    }

    void NextInstrumentSelection()
    {
        audioManager.PlaySoundEffect(EncounterConstants.SoundEffects.PlayerSelect);
        currentPlayer++;
        if (currentPlayer >= playerEntities.stageEntities.Count)
        {
            currentPlayer = 0;
        }

        playerEntities.TransitionToPlayer(currentPlayer);
        FadeColorUI(EncounterConstants.PlayerColors[currentPlayer], null, false);
        knobControl.NextSelection();
    }

    void LastInstrumentSelection()
    {
        audioManager.PlaySoundEffect(EncounterConstants.SoundEffects.PlayerSelect);
        currentPlayer--;
        if (currentPlayer < 0f)
        {
            currentPlayer = playerEntities.stageEntities.Count - 1;
        }

        playerEntities.TransitionToPlayer(currentPlayer);
        FadeColorUI(EncounterConstants.PlayerColors[currentPlayer], null, false);
        knobControl.LastSelection();
    }

    void ChangeMoveDescription(string moveText, int moveID)
    {
        Image moveDesHeader = moveDescription.GetComponentInParent<Image>();

        moveDescription.DOFade(0f, 0.5f).OnComplete(() => {
            moveDescription.text = moveText;
            moveDescription.color = EncounterConstants.KnobColors[moveID];

            Vector3 originalPosition = moveDescription.transform.localPosition;
            moveDescription.DOFade(1f, 0.5f);
        });
    }

    void NextMoveSelection()
    {
        audioManager.PlaySoundEffect(EncounterConstants.SoundEffects.MoveSelect);
        currentMove++;
        if (currentMove >= playerEntities.moveCount)
        {
            currentMove = 0;
        }

        ChangeMoveDescription(EncounterConstants.MoveDescriptions[currentPlayer][currentMove], currentMove);
        knobControl.NextSelection();
    }

    void LastMoveSelection()
    {
        audioManager.PlaySoundEffect(EncounterConstants.SoundEffects.MoveSelect);
        currentMove--;
        if (currentMove < 0f)
        {
            currentMove = playerEntities.moveCount - 1;
        }

        ChangeMoveDescription(EncounterConstants.MoveDescriptions[currentPlayer][currentMove], currentMove);
        knobControl.LastSelection();
    }

    void TakeInput()
    {
        switch (currentState)
        {
            case EncounterConstants.NotesGameStates.Start:
                break;

            case EncounterConstants.NotesGameStates.Idle:
                break;

            case EncounterConstants.NotesGameStates.Intro:
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
            case EncounterConstants.NotesGameStates.MoveSelect:
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
            case EncounterConstants.NotesGameStates.Playing:
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown("joystick button 3"))
                {
                    selectors[0].SelectorPressed(EncounterConstants.FretColors[0]);
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown("joystick button 2"))
                {
                    selectors[1].SelectorPressed(EncounterConstants.FretColors[1]);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown("joystick button 1"))
                {
                    selectors[2].SelectorPressed(EncounterConstants.FretColors[2]);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown("joystick button 0"))
                {
                    selectors[3].SelectorPressed(EncounterConstants.FretColors[3]);
                }
                break;
            case EncounterConstants.NotesGameStates.EnemyIntro:
                break;
            case EncounterConstants.NotesGameStates.Enemy:
                break;
            case EncounterConstants.NotesGameStates.End:
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

    void SwitchState(EncounterConstants.NotesGameStates newState)
    {
        bool switchAllowed = false;

        // Do check for if switch is possible
        switch (currentState)
        {
            case EncounterConstants.NotesGameStates.Idle:
                {
                    switchAllowed = newState == EncounterConstants.NotesGameStates.Start;
                }
                break;
            case EncounterConstants.NotesGameStates.Start:
                {
                    switchAllowed = newState == EncounterConstants.NotesGameStates.Intro;
                }
                break;
            case EncounterConstants.NotesGameStates.Intro:
                {
                    switchAllowed = newState == EncounterConstants.NotesGameStates.MoveSelect;
                }
                break;
            case EncounterConstants.NotesGameStates.MoveSelect:
                {
                    switchAllowed = newState == EncounterConstants.NotesGameStates.Playing;
                }
                break;
            case EncounterConstants.NotesGameStates.Playing:
                {
                    switchAllowed = newState == EncounterConstants.NotesGameStates.EnemyIntro;
                }
                break;
            case EncounterConstants.NotesGameStates.EnemyIntro:
                {
                    switchAllowed = newState == EncounterConstants.NotesGameStates.Enemy;
                }
                break;
            case EncounterConstants.NotesGameStates.Enemy:
                {
                    switchAllowed = newState == EncounterConstants.NotesGameStates.Intro || newState == EncounterConstants.NotesGameStates.End;
                }
                break;
            case EncounterConstants.NotesGameStates.End:
                {
                    switchAllowed = newState == EncounterConstants.NotesGameStates.Idle;
                }
                break;
        }

        if (stateMachineDebug)
        {
            Debug.Log("Trying to switch to:" + newState + " Allowed:" + switchAllowed);
        }

        if (switchAllowed)
        {
            currentState = newState;

            OnExitState(lastState);
            OnEnterState(currentState);
            lastState = currentState;
        }
    }

    // Check entry to stateEnter
    void OnEnterState(EncounterConstants.NotesGameStates stateEnter)
    {
        if(stateMachineDebug)
        {
            Debug.Log("On Enter state " + stateEnter);
        }
        switch (stateEnter)
        {
            case EncounterConstants.NotesGameStates.Idle:
                {
                }
                break;
            case EncounterConstants.NotesGameStates.Start:
                {
                    Debug.Log("In Start ");
                    currSet = 0;

                    OnStartGame(() => {
                        SwitchState(EncounterConstants.NotesGameStates.Intro);
                    });
                    // Start Game Function Fade In UI Here, call from EncounterGameManager?
                }
                break;
            case EncounterConstants.NotesGameStates.Intro:
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
            case EncounterConstants.NotesGameStates.MoveSelect:
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
            case EncounterConstants.NotesGameStates.Playing:
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
            case EncounterConstants.NotesGameStates.EnemyIntro:
                {
                    Debug.Log("In Enemy Enter Intro");
                    gameManager.MoveCameraToEnemy();
                    OnEnemyIntro(() => {
                        SwitchState(EncounterConstants.NotesGameStates.Enemy);
                    });
                }
                break;
            case EncounterConstants.NotesGameStates.Enemy:
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
            case EncounterConstants.NotesGameStates.End:
                {
                    FadeOutUI(() =>
                    {
                        SwitchState(EncounterConstants.NotesGameStates.Idle);
                        gameManager.OnNotesEndComplete();
                        //gameManager switch state
                    });
                }
                break;
        }
    }

    void OnInstrumentSelect()
    {
        audioManager.PlaySoundEffect(EncounterConstants.SoundEffects.MenuNext);
        knobControl.OnPlayerSelected(currentPlayer);
        SwitchState(EncounterConstants.NotesGameStates.MoveSelect);
    }

    void OnMoveSelected()
    {
        audioManager.PlaySoundEffect(EncounterConstants.SoundEffects.MenuNext);
        knobControl.OnMoveSelected(currentMove);
        HideMoveDescription();
        SwitchState(EncounterConstants.NotesGameStates.Playing);
    }

    IEnumerator PlaySong()
    {
        selfPlayableDirector.playableAsset = playableSongs[currSong];

        selfPlayableDirector.Play();

        yield return new WaitForSeconds(EncounterConstants.MusicPlayDelay);
        audioManager.StartMusic(currSong);
    }

    private void SelectSongAndPlay(AnimationCallback animCallback)
    {
        Debug.Log("Select Song and Play");
        
        StartCoroutine(PlaySong());
        playSongCallback = animCallback;
    }

    IEnumerator PlayEnemySong()
    {
        selfPlayableDirector.playableAsset = playableSongs[currSong];
        selfPlayableDirector.Play();

        yield return new WaitForSeconds(EncounterConstants.MusicPlayDelay);
        audioManager.StartMusic(currSong);
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

    void OnExitState(EncounterConstants.NotesGameStates stateExit)
    {
        if (stateMachineDebug)
        {
            Debug.Log("On Exit state " + stateExit);
        }

        switch (stateExit)
        {
            case EncounterConstants.NotesGameStates.Idle:
                {
                }
                break;
            case EncounterConstants.NotesGameStates.Start:
                {
                }
                break;
            case EncounterConstants.NotesGameStates.Intro:
                {
                }
                break;
            case EncounterConstants.NotesGameStates.MoveSelect:
                {
                }
                break;
            case EncounterConstants.NotesGameStates.Playing:
                {
                }
                break;
            case EncounterConstants.NotesGameStates.EnemyIntro:
                {
                }
                break;
            case EncounterConstants.NotesGameStates.Enemy:
                {
                }
                break;
            case EncounterConstants.NotesGameStates.End:
                {
                }
                break;
        }
    }
    void OnProcessState(EncounterConstants.NotesGameStates stateProcess)
    {
        switch (stateProcess)
        {
            case EncounterConstants.NotesGameStates.Idle:
                {
                }
                break;
            case EncounterConstants.NotesGameStates.Start:
                {
                }
                break;
            case EncounterConstants.NotesGameStates.Intro:
                {
                }
                break;
            case EncounterConstants.NotesGameStates.MoveSelect:
                {
                }
                break;
            case EncounterConstants.NotesGameStates.Playing:
                {
                    // Switch state to enemy intro after song is finished and all notes are collected
                    if(songFinishRecieved && unusedNotePool.childCount == EncounterConstants.NotePoolSize)
                    {
                        // Should only be called once
                        songFinishRecieved = false;

                        SwitchState(EncounterConstants.NotesGameStates.EnemyIntro);
                    }
                }
                break;
            case EncounterConstants.NotesGameStates.EnemyIntro:
                {
                }
                break;
            case EncounterConstants.NotesGameStates.Enemy:
                {
                    // Switch state to enemy intro after song is finished and all notes are collected
                    if (songFinishRecieved && unusedNotePool.childCount == EncounterConstants.NotePoolSize)
                    {
                        EnemySongComplete();
                    }
                }
                break;
            case EncounterConstants.NotesGameStates.End:
                {
                }
                break;
        }
    }

    private void EnemySongComplete()
    {    
        // Should only be called once
        songFinishRecieved = false;

        currSong++;
        currSet++;
        if (currSet >= EncounterConstants.setLength)
        {
            Debug.Log("Switching to end");
            SwitchState(EncounterConstants.NotesGameStates.End);
        }
        else
        {
            FadeOutUI(null);
            Debug.Log("Switching to intro");
            SwitchState(EncounterConstants.NotesGameStates.Intro);
        }
    }

    public void StartGame()
    {
        SwitchState(EncounterConstants.NotesGameStates.Start);
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
        //noteImage.color = EncounterConstants.FretColors[noteGeneratorID];

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
        float accuracyRating = UnityEngine.Random.Range(10 * (1 - EncounterConstants.enemyDifficulty), 60f - (20 * EncounterConstants.enemyDifficulty));

        EncounterConstants.AccuracyRating currRating = EncounterConstants.AccuracyRating.OK;
        if (accuracyRating < EncounterConstants.accuracyPerfect)
        {
            currRating = EncounterConstants.AccuracyRating.Perfect;
        }
        else if (accuracyRating < EncounterConstants.accuracyGood)
        {
            currRating = EncounterConstants.AccuracyRating.Good;
        }

        Color gradeColor = Color.red;
        int scoreDecrement = 0;
        Sprite noteSprite = noteOK;

        switch (currRating)
        {
            case EncounterConstants.AccuracyRating.Perfect:
                gradeColor = Color.magenta;
                noteSprite = notePerfect;
                scoreDecrement = EncounterConstants.scorePerfect;

                audioManager.PlaySoundEffect(EncounterConstants.SoundEffects.Perfect);
                break;
            case EncounterConstants.AccuracyRating.Good:
                gradeColor = Color.red;
                noteSprite = noteGood;
                scoreDecrement = EncounterConstants.scoreGood;

                audioManager.PlaySoundEffect(EncounterConstants.SoundEffects.Good);
                break;
            case EncounterConstants.AccuracyRating.OK:
                gradeColor = Color.yellow;
                noteSprite = noteOK;
                scoreDecrement = EncounterConstants.scorePerfect;
                break;
        }

        // Add handicap based on difficulty
        float handicapScore = scoreDecrement - (scoreDecrement * (0.2f * (1f - EncounterConstants.enemyDifficulty)));
        gameManager.EnemyScoreDecrement((int)handicapScore);

        noteToStore.PlaySuccessAnimation(gradeColor, noteSprite);
    }

    public void CollectPlayerNote(NoteScript noteToStore)
    {
        // Calculate Score
        float accuracyRating = Mathf.Abs(noteToStore.transform.position.x - selectors[noteToStore.noteTypeID].transform.position.x);
        EncounterConstants.AccuracyRating currRating = EncounterConstants.AccuracyRating.OK;
        if (accuracyRating < EncounterConstants.accuracyPerfect)
        {
            currRating = EncounterConstants.AccuracyRating.Perfect;
        } else if (accuracyRating < EncounterConstants.accuracyGood)
        {
            currRating = EncounterConstants.AccuracyRating.Good;
        }

        Color gradeColor = Color.red;
        Sprite noteSprite = noteOK;
        int scoreIncrement = 0;

        switch (currRating)
        {
            case EncounterConstants.AccuracyRating.Perfect:
                gradeColor = Color.white;
                noteSprite = notePerfect;
                scoreIncrement = EncounterConstants.scorePerfect;

                audioManager.PlaySoundEffect(EncounterConstants.SoundEffects.Perfect);
                break;
            case EncounterConstants.AccuracyRating.Good:
                gradeColor = Color.cyan;
                noteSprite = noteGood;
                scoreIncrement = EncounterConstants.scoreGood;

                audioManager.PlaySoundEffect(EncounterConstants.SoundEffects.Good);
                break;
            case EncounterConstants.AccuracyRating.OK:
                gradeColor = Color.blue;
                noteSprite = noteOK;
                scoreIncrement = EncounterConstants.scorePerfect;
                break;
        }

        gameManager.TurnScoreIncrement(scoreIncrement);

        audioManager.PlayMusicEffect(EncounterConstants.MusicEffects.PlayerCorrect, currentPlayer);
        noteToStore.PlaySuccessAnimation(gradeColor, noteSprite);
    }

    NoteScript GetUnusedNote(int noteGeneratorID)
    {
        NoteScript newNote = unusedNotePool.GetComponentInChildren<NoteScript>(true);
        return newNote;
    }

    public void StoreNoteScript(NoteScript noteToStore)
    {
        noteToStore.GetComponent<Image>().color = Color.white;
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

        knobControl.transform.localPosition = EncounterConstants.KnobStartPos;
        knobControl.transform.localScale = Vector3.zero;
        uiFadeInSequence.Insert(0f, knobControl.transform.DOScale(2.5f, 1f).SetEase(Ease.InOutBack));
        uiFadeInSequence.Insert(0f, knobControl.GetComponent<CanvasGroup>().DOFade(1f, 1f));

        uiFadeInSequence.OnComplete(() =>
        {
            // Set knob control to player selector
            currentPlayer = 0;
            knobControl.OnPlayerSelector();
            playerEntities.TransitionToPlayer(0);
            FadeColorUI(EncounterConstants.PlayerColors[0], animCallback, false);
        });
    }

    void OnMoveSelect(AnimationCallback animCallback)
    {
        currentMove = 0;
        // Set knob control to player selector
        knobControl.OnMoveSelector();

        // Fade in Knob Controller
        Sequence uiFadeInSequence = DOTween.Sequence();

        // ROHAN DO FADE UI HERE
        moveDescription.text = EncounterConstants.MoveDescriptions[currentPlayer][currentMove];

        uiFadeInSequence.Insert(0f, moveDescription.GetComponentInParent<Image>().DOFade(1f, 0.5f));
        uiFadeInSequence.Insert(0.25f, moveDescription.GetComponentInParent<Image>().DOColor(EncounterConstants.PlayerColors[currentPlayer], 0.5f));
        uiFadeInSequence.Insert(0f, moveDescription.DOFade(1f, 0.5f));
        uiFadeInSequence.Insert(0.25f, moveDescription.DOColor(EncounterConstants.KnobColors[0], 0.5f));

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
        moveDescription.GetComponentInParent<Image>().DOFade(0f, 1f);
        moveDescription.DOFade(0f, 1f);
    }

    public void FadeOutUI(AnimationCallback animCallback)
    {
        Debug.Log("FadeOutUI UI Called");
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
