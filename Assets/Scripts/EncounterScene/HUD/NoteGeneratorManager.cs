using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class NoteGeneratorManager : MonoBehaviour
{
    public PlayerEntitiesScript playerEntities;

    public List<TimelineAsset> playableSongs;

    public List<Sprite> buttonImages;

    public Transform unusedNotePool;

    public List<NoteGeneratorScript> noteGenerators;

    public List<SelectorScript> selectors;

    public GameObject notePrefab;

    public Transform reputationPointerTransform;

    public KnobControlScript knobControl;

    PlayableDirector selfPlayableDirector;

    // Start is called before the first frame update
    EncounterGameManager gameManager;

    private EncounterConstants.NotesGameStates currentState = EncounterConstants.NotesGameStates.Idle;
    private EncounterConstants.NotesGameStates lastState = EncounterConstants.NotesGameStates.Idle;
    int currentPlayer = 0;
    int currSong = 0;
    int currSet = 0;

    bool isTurnVisible;

    public Sprite notePerfect;
    public Sprite noteGood;
    public Sprite noteOK;

    public bool songFinishRecieved = false;

    public delegate void AnimationCallback();
    AnimationCallback playSongCallback = null;

    bool initialFadeComplete = false;

    void Start()
    {
        selfPlayableDirector = GetComponent<PlayableDirector>();
        gameManager = FindObjectOfType<EncounterGameManager>();
        for (int i = 0; i < EncounterConstants.NotePoolSize; i++)
        {
            GameObject newNote = Instantiate(notePrefab, unusedNotePool);
            newNote.SetActive(false);
        }
        FadeOutUI(() => {
            initialFadeComplete = true;
        });
    }

    public EncounterConstants.NotesGameStates GetCurrentState()
    {
        return currentState;
    }

    void NextPlayerSelection()
    {
        currentPlayer++;
        if (currentPlayer >= playerEntities.stageEntities.Count)
        {
            currentPlayer = 0;
        }

        playerEntities.TransitionToPlayer(currentPlayer);
        FadeColorUI(EncounterConstants.PlayerColors[currentPlayer], null);
        knobControl.NextSelection();
    }

    void LastPlayerSelection()
    {
        currentPlayer--;
        if (currentPlayer < 0f)
        {
            currentPlayer = playerEntities.stageEntities.Count - 1;
        }

        playerEntities.TransitionToPlayer(currentPlayer);
        FadeColorUI(EncounterConstants.PlayerColors[currentPlayer], null);
        knobControl.LastSelection();
    }
    void NextMoveSelection()
    {
        //currentMove++;
        //if (currentMove >= currentMoves.Count)
        //{
        //    currentMove = 0;
        //}

        knobControl.NextSelection();
    }

    void LastMoveSelection()
    {
        //currentMove--;
        //if (currentMove < 0f)
        //{
        //    currentMove = playerEntities.stageEntities.Count - 1;
        //}

        knobControl.LastSelection();
    }

    void TakeInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 9") || Input.GetKeyDown("joystick button 11"))
        {
            //PlaySong(songCount);
        }

        switch (currentState)
        {
            case EncounterConstants.NotesGameStates.Start:
                break;

            case EncounterConstants.NotesGameStates.Idle:
                break;

            case EncounterConstants.NotesGameStates.Intro:
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown("joystick button 5"))
                {
                    LastPlayerSelection();
                }
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown("joystick button 4"))
                {
                    NextPlayerSelection();
                } else if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 1"))
                {
                    OnIntroSelect();
                }
                break;
            case EncounterConstants.NotesGameStates.Playing:
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown("joystick button 5"))
                {
                    LastMoveSelection();
                }
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown("joystick button 4"))
                {
                    NextMoveSelection();
                } else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown("joystick button 3"))
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
            CheckState();
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
    void OnEnterState(EncounterConstants.NotesGameStates stateEnter)
    {
        switch (stateEnter)
        {
            case EncounterConstants.NotesGameStates.Idle:
                {
                }
                break;
            case EncounterConstants.NotesGameStates.Start:
                {
                    currSet = 0;

                    OnStartGame(() => {
                        gameManager.OnNotesStartComplete();
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
            case EncounterConstants.NotesGameStates.Playing:
                {
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

    void OnIntroSelect()
    {
        Debug.Log("Intro Switch to Playing");
        SwitchState(EncounterConstants.NotesGameStates.Playing);
    }

    void PlaySong(int songCount)
    {
        selfPlayableDirector.playableAsset = playableSongs[songCount];
        selfPlayableDirector.Play();
    }

    private void SelectSongAndPlay(AnimationCallback animCallback)
    {
        Debug.Log("Select Song and Play");
        PlaySong(currSong);
        playSongCallback = animCallback;
    }

    void PlayEnemySong(int songCount)
    {
        selfPlayableDirector.playableAsset = playableSongs[songCount];
        selfPlayableDirector.Play();
    }

    private void SelectEnemySongAndPlay(AnimationCallback animCallback)
    {
        Debug.Log("Select Song and Play");
        PlayEnemySong(currSong);
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
                        // Should only be called once
                        songFinishRecieved = false;

                        currSong++;
                        currSet++;
                        if(currSet >= EncounterConstants.setLength)
                        {
                            Debug.Log("Switching to end");
                            SwitchState(EncounterConstants.NotesGameStates.End);
                        } else
                        {
                            Debug.Log("Switching to intro");
                            SwitchState(EncounterConstants.NotesGameStates.Intro);
                        }
                    }
                }
                break;
            case EncounterConstants.NotesGameStates.End:
                {
                }
                break;
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
        transform.localScale = Vector3.one * 0.8f;

        // Start animations and on complete, go to Intro
        transform.DOScale(1f, 0.25f);
        GetComponent<CanvasGroup>().DOFade(1f, 1f).OnComplete(() => {
            animCallback();
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
        Image noteImage = noteToStore.GetComponent<Image>();
        int scoreDecrement = 0;

        switch (currRating)
        {
            case EncounterConstants.AccuracyRating.Perfect:
                gradeColor = Color.magenta;
                noteImage.sprite = notePerfect;
                scoreDecrement = EncounterConstants.scorePerfect;
                break;
            case EncounterConstants.AccuracyRating.Good:
                gradeColor = Color.red;
                noteImage.sprite = noteGood;
                scoreDecrement = EncounterConstants.scoreGood;
                break;
            case EncounterConstants.AccuracyRating.OK:
                gradeColor = Color.yellow;
                noteImage.sprite = noteOK;
                scoreDecrement = EncounterConstants.scorePerfect;
                break;
        }

        // Add handicap based on difficulty
        gameManager.EnemyScoreDecrement(scoreDecrement - (int)(scoreDecrement * 0.8f * (1 - EncounterConstants.enemyDifficulty)));

        noteToStore.isCollected = true;
        noteToStore.PlaySuccessAnimation();

        noteToStore.transform.SetParent(reputationPointerTransform.parent);

        // Create animation to collect note
        noteImage.color = gradeColor;
        Sequence completeSequence = DOTween.Sequence();

        completeSequence.Insert(0f, noteToStore.GetComponent<RectTransform>()
            .DOScale(1.5f, 0.25f).SetEase(Ease.InBack));
        completeSequence.Insert(0.25f, noteToStore.GetComponent<RectTransform>()
            .DOMove(reputationPointerTransform.position, 0.75f).SetEase(Ease.InSine));
        completeSequence.Insert(0.5f, noteImage.DOFade(0f, 0.5f));

        completeSequence.OnComplete(() =>
        {
            StoreNoteScript(noteToStore);
        });
        completeSequence.Play();
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
        Image noteImage = noteToStore.GetComponent<Image>();
        int scoreIncrement = 0;

        switch (currRating)
        {
            case EncounterConstants.AccuracyRating.Perfect:
                gradeColor = Color.white;
                noteImage.sprite = notePerfect;
                scoreIncrement = EncounterConstants.scorePerfect;
                break;
            case EncounterConstants.AccuracyRating.Good:
                gradeColor = Color.cyan;
                noteImage.sprite = noteGood;
                scoreIncrement = EncounterConstants.scoreGood;
                break;
            case EncounterConstants.AccuracyRating.OK:
                gradeColor = Color.blue;
                noteImage.sprite = noteOK;
                scoreIncrement = EncounterConstants.scorePerfect;
                break;
        }

        gameManager.TurnScoreIncrement(scoreIncrement);

        noteToStore.isCollected = true;
        noteToStore.PlaySuccessAnimation();

        noteToStore.transform.SetParent(reputationPointerTransform.parent);

        // Create animation to collect note
        noteImage.color = gradeColor;
        Sequence completeSequence = DOTween.Sequence();

        completeSequence.Insert(0f, noteToStore.GetComponent<RectTransform>()
            .DOScale(1.5f, 0.25f).SetEase(Ease.InBack));
        completeSequence.Insert(0.25f, noteToStore.GetComponent<RectTransform>()
            .DOMove(reputationPointerTransform.position, 0.75f).SetEase(Ease.InSine));
        completeSequence.Insert(0.5f, noteImage.DOFade(0f, 0.5f));

        completeSequence.OnComplete(() =>
        {
            StoreNoteScript(noteToStore);
        });
        completeSequence.Play();
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
        // Set knob control to player selector
        knobControl.OnPlayerSelect();

        // Fade in Knob Controller
        Sequence uiFadeInSequence = DOTween.Sequence();

        knobControl.transform.localScale = Vector3.zero;
        uiFadeInSequence.Insert(0f, knobControl.transform.DOScale(1.6f, 1f).SetEase(Ease.InOutBack));
        uiFadeInSequence.Insert(0f, knobControl.GetComponent<CanvasGroup>().DOFade(1f, 1f));

        uiFadeInSequence.OnComplete(() =>
        {
            FadeInSelectorUI(() => {
                currentPlayer = 0;
                playerEntities.TransitionToPlayer(0);
                FadeColorUI(EncounterConstants.PlayerColors[0], animCallback);
            });
        });
    }

    void OnPlay(AnimationCallback animCallback)
    {
        knobControl.OnMoveSelect();
        animCallback?.Invoke();
    }

    void OnEnemyIntro(AnimationCallback animCallback)
    {
        Sequence uiFadeInSequence = DOTween.Sequence();

        knobControl.transform.localScale = Vector3.zero;
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
            animCallback();
        });
    }

    public void FadeColorUI(Color uiColor, AnimationCallback animationCallback)
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
        foreach (SelectorScript selector in selectors)
        {
            // Fade In NoteGenerators
            uiColorSequence.Insert(delay, selector.GetComponent<Image>().DOColor(uiColor, 0.5f));
            delay += 0.2f;
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

    public void FadeOutUI(AnimationCallback animCallback)
    {
        Debug.Log("FadeOutUI UI Called");
        Sequence uiFadeOutSequence = DOTween.Sequence();

        isTurnVisible = false;

        playerEntities.ResetBandEndTurn();

        uiFadeOutSequence.Insert(0f, GetComponent<CanvasGroup>().DOFade(0f, 1f));
        uiFadeOutSequence.Insert(0f, transform.DOScale(0.75f, 0.25f));

        foreach (NoteGeneratorScript noteGen in noteGenerators)
        {
            uiFadeOutSequence.Insert(0f, noteGen.transform.DOScale(0.1f, 0.35f));
            uiFadeOutSequence.Insert(0f, noteGen.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 0f), 0.5f));
        }
        foreach (SelectorScript selector in selectors)
        {
            uiFadeOutSequence.Insert(0f, selector.transform.DOScale(0.1f, 0.35f));
            uiFadeOutSequence.Insert(0f, selector.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 0f), 0.5f));
        }
        uiFadeOutSequence.Insert(0f, knobControl.GetComponent<CanvasGroup>().DOFade(0f, 0.5f));

        uiFadeOutSequence.OnComplete(() =>
        {
            animCallback?.Invoke();
        });
    }
}
