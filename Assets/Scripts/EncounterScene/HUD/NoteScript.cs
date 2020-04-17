using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteScript : MonoBehaviour
{
    public bool isCollected = false;
    public int noteTypeID = 0;

    private NoteGeneratorManager noteGenManager;
    EncounterConstants encounterConstants;

    void Awake()
    {
        encounterConstants = FindObjectOfType<EncounterConstants>();
        noteGenManager = FindObjectOfType<NoteGeneratorManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(this.isActiveAndEnabled && !isCollected)
        {
            MoveNote();
        }
    }

    void MoveNote()
    {
        transform.Translate(-encounterConstants.NoteSpeed, 0f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<NoteEndLimit>() != null && !isCollected)
        {
            OnNoteEndLimit();
        }
    }

    public void OnNoteEndLimit()
    {
        PlayFailureAnimation();
    }

    public void PlaySuccessAnimation(Color gradeColor, Sprite noteSprite)
    {
        isCollected = true;

        transform.SetParent(noteGenManager.reputationPointerTransform.parent);

        // Create animation to collect note
        Image noteImage = GetComponent<Image>();
        noteImage.sprite = noteSprite;
        noteImage.color = gradeColor;
        Sequence completeSequence = DOTween.Sequence();

        completeSequence.Insert(0f, GetComponent<RectTransform>()
            .DOScale(1.5f, 0.25f).SetEase(Ease.InBack));
        completeSequence.Insert(0.25f, GetComponent<RectTransform>()
            .DOMove(noteGenManager.reputationPointerTransform.position, 0.75f).SetEase(Ease.InSine));
        completeSequence.Insert(0.5f, noteImage.DOFade(0f, 0.5f));

        completeSequence.OnComplete(() =>
        {
            noteGenManager.StoreNoteScript(this);
        });
        completeSequence.Play();
    }
    public void PlayFailureAnimation()
    {
        noteGenManager.NoteMissed();
        transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() => {
            noteGenManager.StoreNoteScript(this);
        });
    }
}
