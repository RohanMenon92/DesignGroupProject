using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorScript : MonoBehaviour
{
    public Sprite PressedIcon;
    public Sprite unpressedIcon;

    public bool isPressed;

    private NoteGeneratorManager noteGenManager;
    Image selectorImage;

    Tween playButtonAnim;

    // Start is called before the first frame update
    void Start()
    {
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
        selectorImage.color = color;
        isPressed = true;

        // Do Animation and unpress button
        playButtonAnim = selectorImage.DOColor(Color.white, EncounterConstants.SelectorDelay).OnComplete(SelectorUnPressed);
    }

    public void SelectorUnPressed()
    {
        if(playButtonAnim != null)
        {
            playButtonAnim.Kill(true);
        }

        selectorImage.sprite = unpressedIcon;
        selectorImage.color = Color.white;
        isPressed = false;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(!isPressed)
        {
            return;
        }

        NoteScript collNoteScript = collision.GetComponent<NoteScript>();

        if (collNoteScript == null || collNoteScript.isCollected)
        {
            return;
        }

        noteGenManager.RegisterNoteHit(collNoteScript);
        SelectorUnPressed();
    }
}
