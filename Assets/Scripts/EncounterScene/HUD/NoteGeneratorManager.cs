using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class NoteGeneratorManager : MonoBehaviour
{
    [HideInInspector]
    public int turnScore;

    public Transform unusedNotePool;

    public NoteGeneratorScript noteG1;
    public NoteGeneratorScript noteG2;
    public NoteGeneratorScript noteG3;
    public NoteGeneratorScript noteG4;

    public SelectorScript selector1;
    public SelectorScript selector2;
    public SelectorScript selector3;
    public SelectorScript selector4;

    public GameObject notePrefab;

    public Transform reputationPointerTransform;

    public int correctNoteScore;

    PlayableDirector selfPlayableDirector;
    // Start is called before the first frame update
    void Start()
    {
        selfPlayableDirector = GetComponent<PlayableDirector>();

        for (int i = 0; i <= EncounterSceneConstants.NotePoolSize; i++)
        {
            GameObject newNote = Instantiate(notePrefab, unusedNotePool);
            newNote.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            selector1.SelectorPressed(EncounterSceneConstants.Fret1Color);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            selector2.SelectorPressed(EncounterSceneConstants.Fret2Color);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            selector3.SelectorPressed(EncounterSceneConstants.Fret3Color);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            selector4.SelectorPressed(EncounterSceneConstants.Fret4Color);
        }


        if(Input.GetKeyDown(KeyCode.Space))
        {
            selfPlayableDirector.Play();
        }
    }

    public void PlaySongTurn()
    {

    }

    public void CallNoteGenerator(int noteGeneratorID)
    {
        NoteScript newNote = GetUnusedNote(noteGeneratorID);

        switch (noteGeneratorID)
        {
            case 0:
                newNote.transform.SetParent(noteG1.transform.parent);
                newNote.GetComponent<RectTransform>().localPosition = noteG1.GetComponent<RectTransform>().localPosition;
                newNote.GetComponent<Image>().color = EncounterSceneConstants.Fret1Color;
                break;
            case 1:
                newNote.transform.SetParent(noteG2.transform.parent);
                newNote.GetComponent<RectTransform>().localPosition = noteG2.GetComponent<RectTransform>().localPosition;
                newNote.GetComponent<Image>().color = EncounterSceneConstants.Fret2Color;
                break;
            case 2:
                newNote.transform.SetParent(noteG3.transform.parent);
                newNote.GetComponent<RectTransform>().localPosition = noteG3.GetComponent<RectTransform>().localPosition;
                newNote.GetComponent<Image>().color = EncounterSceneConstants.Fret3Color;
                break;
            case 3:
                newNote.transform.SetParent(noteG4.transform.parent);
                newNote.GetComponent<RectTransform>().localPosition = noteG4.GetComponent<RectTransform>().localPosition;
                newNote.GetComponent<Image>().color = EncounterSceneConstants.Fret4Color;
                break;
            default:
                break;
        }

        newNote.isCollected = false;
        newNote.gameObject.SetActive(true);
    }

    public void RegisterNoteHit(NoteScript noteToStore)
    {
        turnScore += correctNoteScore;

        noteToStore.isCollected = true;
        noteToStore.PlaySuccessAnimation();

        noteToStore.transform.SetParent(reputationPointerTransform.parent);
        Sequence completeSequence = DOTween.Sequence();
        completeSequence.Insert(0f, noteToStore.GetComponent<RectTransform>().DOMove(reputationPointerTransform.position, 0.5f).SetEase(Ease.InOutBack).OnComplete(() => {
            StoreNoteScript(noteToStore);
        }));
        completeSequence.Insert(0.1f, noteToStore.GetComponent<Image>().DOFade(0f, 0.3f));
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
}
