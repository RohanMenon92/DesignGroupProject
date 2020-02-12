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

    public List<NoteGeneratorScript> noteGenerators;

    public List<SelectorScript> selectors;

    public GameObject notePrefab;

    public Transform reputationPointerTransform;

    public int correctNoteScore;

    public KnobControlScript knobControl;

    PlayableDirector selfPlayableDirector;
    // Start is called before the first frame update
    void Start()
    {
        selfPlayableDirector = GetComponent<PlayableDirector>();

        for (int i = 0; i <= EncounterConstants.NotePoolSize; i++)
        {
            GameObject newNote = Instantiate(notePrefab, unusedNotePool);
            newNote.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectors[0].SelectorPressed(EncounterConstants.FretColors[0]);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectors[1].SelectorPressed(EncounterConstants.FretColors[1]);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectors[2].SelectorPressed(EncounterConstants.FretColors[2]);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectors[3].SelectorPressed(EncounterConstants.FretColors[3]);
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            knobControl.NextSelection();
        } else if(Input.GetKeyDown(KeyCode.N))
        {
            knobControl.LastSelection();
        }

        if (Input.GetKeyDown(KeyCode.Space))
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
        Image noteImage = newNote.GetComponent<Image>();

        // select note generators
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

    public void RegisterNoteHit(NoteScript noteToStore)
    {
        turnScore += correctNoteScore;

        noteToStore.isCollected = true;
        noteToStore.PlaySuccessAnimation();

        noteToStore.transform.SetParent(reputationPointerTransform.parent);

        // Create animation to collect note
        noteToStore.GetComponent<Image>().color = Color.cyan;
        Sequence completeSequence = DOTween.Sequence();
        completeSequence.Insert(0f, noteToStore.GetComponent<RectTransform>()
            .DOMove(reputationPointerTransform.position, 0.75f).SetEase(Ease.InBack));
        completeSequence.Insert(0.5f, noteToStore.GetComponent<Image>().DOFade(0f, 0.2f));

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
}
