﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class NoteGeneratorManager : MonoBehaviour
{
    public Transform unusedNotePool;

    public List<NoteGeneratorScript> noteGenerators;

    public List<SelectorScript> selectors;

    public GameObject notePrefab;

    public Transform reputationPointerTransform;

    public KnobControlScript knobControl;

    PlayableDirector selfPlayableDirector;
    // Start is called before the first frame update
    EncounterGameManager gameManager;

    bool isTurnVisible;

    public Sprite notePerfect;
    public Sprite noteGood;
    public Sprite noteOK;

    void Start()
    {
        selfPlayableDirector = GetComponent<PlayableDirector>();
        gameManager = FindObjectOfType<EncounterGameManager>();
        for (int i = 0; i <= EncounterConstants.NotePoolSize; i++)
        {
            GameObject newNote = Instantiate(notePrefab, unusedNotePool);
            newNote.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isTurnVisible)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown("joystick button 0"))
            {
                selectors[0].SelectorPressed(EncounterConstants.FretColors[0]);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown("joystick button 1"))
            {
                selectors[1].SelectorPressed(EncounterConstants.FretColors[1]);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown("joystick button 2"))
            {
                selectors[2].SelectorPressed(EncounterConstants.FretColors[2]);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown("joystick button 3"))
            {
                selectors[3].SelectorPressed(EncounterConstants.FretColors[3]);
            }

            if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown("joystick button 5"))
            {
                knobControl.NextSelection();
            }
            else if (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown("joystick button 4"))
            {
                knobControl.LastSelection();
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 9") || Input.GetKeyDown("joystick button 11"))
            {
                selfPlayableDirector.Play();
            }
        }
    }

    public void PlaySongTurn()
    {

    }

    public void CallNoteGenerator(int noteGeneratorID)
    {
        NoteScript newNote = GetUnusedNote(noteGeneratorID);
        Image noteImage = newNote.GetComponent<Image>();

        // select note generator as parent
        newNote.noteTypeID = noteGeneratorID;
        newNote.transform.SetParent(noteGenerators[noteGeneratorID].transform.parent);
        newNote.GetComponent<RectTransform>().localPosition = noteGenerators[noteGeneratorID].GetComponent<RectTransform>().localPosition;
        noteImage.color = EncounterConstants.FretColors[noteGeneratorID];

        // Reset for animation
        newNote.isCollected = false;
        noteImage.color = new Color(noteImage.color.r, noteImage.color.g, noteImage.color.b, 0f) ;
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

    public void CollectNote(NoteScript noteToStore)
    {
        // Calculate Score
        float accuracyRating = Mathf.Abs(noteToStore.transform.position.x - selectors[noteToStore.noteTypeID].transform.position.x);
        EncounterConstants.AccuracyRating currRating = EncounterConstants.AccuracyRating.OK;
        if(accuracyRating < EncounterConstants.accuracyPerfect)
        {
            currRating = EncounterConstants.AccuracyRating.Perfect;
        } else if (accuracyRating < EncounterConstants.accuracyGood)
        {
            currRating = EncounterConstants.AccuracyRating.Good;
        }

        Color gradeColor = Color.red;
        Image noteImage = noteToStore.GetComponent<Image>();
        int scoreIncrement = 0;

        switch(currRating)
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

    public void FadeInSelectorUI()
    {
        Sequence selectorFadeInSequence = DOTween.Sequence();

        knobControl.transform.localScale = Vector3.zero;
        selectorFadeInSequence.Insert(0f, knobControl.transform.DOScale(1.6f, 0.5f).SetEase(Ease.InOutBack));
        selectorFadeInSequence.Insert(0f, knobControl.GetComponent<CanvasGroup>().DOFade(1f, 0.5f));

        float delay = 0.0f;
        foreach (NoteGeneratorScript noteGen in noteGenerators)
        {
            // Fade In NoteGenerators
            noteGen.transform.localScale = Vector3.one * 0.75f;
            selectorFadeInSequence.Insert(delay, noteGen.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 1f), 0.5f));
            selectorFadeInSequence.Insert(delay + 0.1f, noteGen.transform.DOScale(1, 0.25f).SetEase(Ease.InOutBack));
            delay+= 0.1f;
        }
        foreach (SelectorScript selector in selectors)
        {
            // Fade In NoteGenerators
            selector.transform.localScale = Vector3.one * 1.2f;
            selectorFadeInSequence.Insert(delay, selector.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 1f), 0.5f));
            selectorFadeInSequence.Insert(delay + 0.1f, selector.transform.DOScale(1, 0.25f).SetEase(Ease.InOutBack));
            delay += 0.2f;
        }

        // set is turn visible to true on complete
        selectorFadeInSequence.OnComplete(() =>
        {
            isTurnVisible = true;
        });
    }

    public void FadeOutSelectorUI()
    {
        isTurnVisible = false;

        foreach (NoteGeneratorScript noteGen in noteGenerators)
        {
            noteGen.transform.DOScale(0.1f, 0.35f);
            noteGen.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
        }
        foreach (SelectorScript selector in selectors)
        {
            selector.transform.DOScale(0.1f, 0.35f);
            selector.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
        }
        knobControl.GetComponent<CanvasGroup>().DOFade(0f, 0.5f);
    }
}
