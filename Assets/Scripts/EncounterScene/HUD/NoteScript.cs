using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteScript : MonoBehaviour
{
    public float noteSpeed = 5f;
    public bool isCollected = false;
    public int noteTypeID = 0;

    private NoteGeneratorManager noteGenManager;

    // Start is called before the first frame update
    void Awake()
    {
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
        transform.Translate(-noteSpeed, 0f, 0f);
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

        transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() => {
            noteGenManager.StoreNoteScript(this);
        });
    }

    public void PlaySuccessAnimation()
    {
        
    }
    public void PlayFailureAnimation()
    {

    }
}
