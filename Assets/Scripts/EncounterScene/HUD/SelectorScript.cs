using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorScript : MonoBehaviour
{
    public Sprite PressedIcon;
    public Sprite unpressedIcon;

    bool isPressed;
    bool noteCollected = false;

    private NoteGeneratorManager noteGenManager;
    Image selectorImage;

    Tween playButtonAnim;

    EncounterConstants encounterConstants;


    // Start is called before the first frame update
    void Start()
    {
        encounterConstants = FindObjectOfType<EncounterConstants>();
        selectorImage = this.GetComponent<Image>();
        noteGenManager = FindObjectOfType<NoteGeneratorManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectorPressed(Color color)
    {
        if(isPressed)
        {
            return;
        }

        selectorImage.sprite = PressedIcon;
        transform.localScale = transform.localScale * 0.5f;
        isPressed = true;

        noteCollected = false;
        // Do Animation and unpress button
        playButtonAnim = transform.DOScale(1f, encounterConstants.SelectorDelay).OnComplete(SelectorUnPressed);
    }

    public void SelectorUnPressed()
    {
        if(playButtonAnim != null)
        {
            playButtonAnim.Kill(true);
        }

        if(!noteCollected && noteGenManager.GetCurrentState() == EncounterConstants.NotesGameStates.Playing)
        {
            noteGenManager.OnWrongPress();
        }

        selectorImage.sprite = unpressedIcon;
        isPressed = false;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        NoteScript collNoteScript = collision.GetComponent<NoteScript>();

        if (collNoteScript == null || collNoteScript.isCollected)
        {
            return;
        }

        if (noteGenManager.GetCurrentState() == EncounterConstants.NotesGameStates.Playing)
        {
            if (!isPressed)
            {
                return;
            }
            if(!noteCollected)
            {
                noteCollected = true;
            }
            noteGenManager.CollectPlayerNote(collNoteScript);
        } else if(noteGenManager.GetCurrentState() == EncounterConstants.NotesGameStates.Enemy)
        {
            noteGenManager.CollectEnemyNote(collNoteScript);
        }

        SelectorUnPressed();
    }
}
