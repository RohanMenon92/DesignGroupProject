using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KnobControlScript : MonoBehaviour
{
    public RectTransform knobSprite;
    public CanvasGroup playerLabels;
    public CanvasGroup moveLabels;

    public List<RectTransform> labelSprites;
    public List<RectTransform> moveSprites;

    List<RectTransform> selectedSprites;

    int currentKnob = 0;

    bool showPlayers = true;

    Color[] PlayerColors;
    Color[] MoveColors;

    EncounterConstants encounterConstants;
    NoteGeneratorManager noteGeneratorManager;

    Sequence animationSequence;

    private void Awake()
    {
        encounterConstants = FindObjectOfType<EncounterConstants>();
        noteGeneratorManager = FindObjectOfType<NoteGeneratorManager>();
        PlayerColors = encounterConstants.PlayerColors;
        MoveColors = encounterConstants.MoveColors;
    }

    // Start is called before the first frame update
    void Start()
    {
        moveLabels.DOFade(0f, 0.5f);
        playerLabels.DOFade(1f, 0.5f);
        showPlayers = true;
        selectedSprites = labelSprites;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void AnimationToSelect(int knobPosition)
    {
        Sequence selectSequence = DOTween.Sequence();

        Color imageColor = showPlayers ? PlayerColors[knobPosition] : noteGeneratorManager.IsMoveUnlocked(knobPosition) ? MoveColors[knobPosition] : encounterConstants.moveLockTextColor;
        Color textColor = showPlayers ? Color.white : noteGeneratorManager.IsMoveUnlocked(knobPosition) ? Color.white : encounterConstants.moveLockColor;

        selectSequence.Insert(0f, selectedSprites[knobPosition].DOScale(encounterConstants.moveSelectedScale, 0.5f).SetEase(Ease.OutBack));
        selectSequence.Insert(0f,
            selectedSprites[knobPosition].GetComponent<Image>().
            DOColor(imageColor, 0.5f));
        selectSequence.Insert(0f,
            selectedSprites[knobPosition].GetComponentInChildren<TextMeshProUGUI>().
            DOColor(textColor, 0.5f));

        Vector3 diff = selectedSprites[knobPosition].transform.position - knobSprite.transform.position;
        diff.Normalize();
        Vector3 rotateTo = new Vector3(0f, 0f, (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) + 180);
        knobSprite.transform.DORotate(rotateTo, 0.35f).SetEase(Ease.InOutBack);
    }

    void AnimationToDeselect(int knobPosition)
    {
        Sequence selectSequence = DOTween.Sequence();
        Color imageColor = showPlayers ? Color.white : noteGeneratorManager.IsMoveUnlocked(knobPosition) ? Color.white : encounterConstants.moveLockColor;
        Color textColor = showPlayers ? PlayerColors[knobPosition] : noteGeneratorManager.IsMoveUnlocked(knobPosition) ? MoveColors[knobPosition] : encounterConstants.moveLockTextColor;

        float scaleValue = showPlayers ? encounterConstants.knobDeselectScale : noteGeneratorManager.IsMoveUnlocked(knobPosition) ? encounterConstants.moveLockScale : encounterConstants.knobDeselectScale;

        selectSequence.Insert(0f, selectedSprites[knobPosition].DOScale(scaleValue, 0.5f).SetEase(Ease.OutBack));

        selectSequence.Insert(0f,
            selectedSprites[knobPosition].GetComponent<Image>().
            DOColor(imageColor, 0.5f));
        selectSequence.Insert(0f,
            selectedSprites[knobPosition].GetComponentInChildren<TextMeshProUGUI>().
            DOColor(textColor, 0.5f));
    }

    public void NextSelection()
    {
        AnimationToDeselect(currentKnob);
        currentKnob++;
        if (currentKnob >= selectedSprites.Count)
        {
            currentKnob = 0;
        }
        AnimationToSelect(currentKnob);
    }

    public void LastSelection()
    {
        AnimationToDeselect(currentKnob);
        currentKnob--;
        if (currentKnob < 0f)
        {
            currentKnob = selectedSprites.Count - 1;
        }
        AnimationToSelect(currentKnob);
    }

    public void OnPlayerSelector()
    {
        currentKnob = 0;
        playerLabels.transform.localScale = Vector3.one;

        animationSequence.Complete(true);
        animationSequence = DOTween.Sequence();

        animationSequence.Insert(0f, moveLabels.DOFade(0f, 0.5f));
        animationSequence.Insert(0f, playerLabels.DOFade(1f, 0.5f));
        showPlayers = true;
        selectedSprites = labelSprites;

        int index = 0;
        foreach(RectTransform sprite in labelSprites)
        {
            TextMeshProUGUI textMesh = sprite.GetComponentInChildren<TextMeshProUGUI>();
            // Fade in Canvas
            animationSequence.Insert(index * 0.1f, sprite.GetComponent<CanvasGroup>().DOFade(1f, 0.5f));

            if(index != 0)
            {
                animationSequence.Insert(0f, sprite.DOScale(encounterConstants.knobDeselectScale, 0.5f).SetEase(Ease.OutBack));
                sprite.GetComponent<Image>().color = Color.white;
                textMesh.color = PlayerColors[index];
            }
            else
            {
                animationSequence.Insert(0f, sprite.DOScale(encounterConstants.moveSelectedScale, 0.5f).SetEase(Ease.OutBack));

                sprite.GetComponent<Image>().color = PlayerColors[index];
                textMesh.color = Color.white;

                Vector3 diff = sprite.transform.position - knobSprite.transform.position;
                diff.Normalize();
                Vector3 rotateTo = new Vector3(0f, 0f, (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) + 180);
                knobSprite.transform.DORotate(rotateTo, 0.35f).SetEase(Ease.InOutBack);
            }
            index++;
        }
    }

    public void OnMoveSelector()
    {
        currentKnob = 0;

        animationSequence.Complete(true);

        animationSequence = DOTween.Sequence();

        animationSequence.Insert(0f, playerLabels.transform.DOScale(0.1f, 1f).SetEase(Ease.InOutBack));
        animationSequence.Insert(0f, playerLabels.DOFade(0f, 1f));

        animationSequence.Insert(0f, moveLabels.DOFade(1f, 0.5f));

        int index = 0;
        foreach (RectTransform labelSprite in labelSprites)
        {
            animationSequence.Insert(0.5f + (index * 0.25f), labelSprite.DOScale(2f, 0.5f).SetEase(Ease.InOutBack));
            animationSequence.Insert(0.5f + (index * 0.25f), labelSprite.transform.GetComponent<CanvasGroup>().DOFade(0f, 0.5f));
            index++;
        }

        showPlayers = false;
        selectedSprites = moveSprites;

        index = 0;
        foreach (RectTransform sprite in moveSprites)
        {
            TextMeshProUGUI textMesh = sprite.GetComponentInChildren<TextMeshProUGUI>();

            animationSequence.Insert(index * 0.1f, sprite.GetComponent<CanvasGroup>().DOFade(1f, 0.5f));

            if (index != 0)
            {
                if(noteGeneratorManager.IsMoveUnlocked(index))
                {
                    sprite.GetComponent<Image>().color = Color.white;
                    textMesh.color = MoveColors[index];

                    animationSequence.Insert(0f, sprite.DOScale(encounterConstants.knobDeselectScale, 0.5f).SetEase(Ease.OutBack));

                } else
                {
                    sprite.GetComponent<Image>().color = encounterConstants.moveLockColor;
                    textMesh.color = encounterConstants.moveLockTextColor;

                    animationSequence.Insert(0f, sprite.DOScale(encounterConstants.moveLockedScale, 0.5f).SetEase(Ease.OutBack));
                }
            }
            else
            {
                animationSequence.Insert(0f, sprite.DOScale(encounterConstants.moveSelectedScale, 0.5f).SetEase(Ease.OutBack));

                sprite.GetComponent<Image>().color = MoveColors[index];
                textMesh.color = Color.white;

                Vector3 diff = sprite.transform.position - knobSprite.transform.position;
                diff.Normalize();
                Vector3 rotateTo = new Vector3(0f, 0f, (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) + 180);
                knobSprite.transform.DORotate(rotateTo, 0.35f).SetEase(Ease.InOutBack);
            }
            index++;
        }

        animationSequence.Play();
    }

    internal void OnPlayerSelected(PlayerMove[] moves)
    {
        int index = 0;
        // Iterate through and find moves

        foreach (PlayerMove move in moves)
        {
            moveSprites[index].GetComponentInChildren<TextMeshProUGUI>().text = move.name;
            if(!move.IsUnlocked())
            {
                moveSprites[index].GetComponentInChildren<TextMeshProUGUI>().color = encounterConstants.moveLockTextColor;
                moveSprites[index].GetComponent<Image>().color = encounterConstants.moveLockColor;
                moveSprites[index].DOScale(encounterConstants.moveLockScale, encounterConstants.moveLockScale);
            }
            index++;
        };
    }

    internal void OnMoveSelected(int currentMove)
    {
        animationSequence.Complete(true);
        animationSequence = DOTween.Sequence();

        int index = 0;
        foreach(RectTransform moveSprite in moveSprites)
        {
            if (index != currentMove)
            {
                animationSequence.Insert(index * 0.25f, moveSprite.DOScale(0f, 0.5f).SetEase(Ease.InOutBack));
                animationSequence.Insert(index * 0.25f, moveSprite.transform.GetComponent<CanvasGroup>().DOFade(0f, 0.5f));
            }
            index++;
        }
        animationSequence.Insert(0.75f, transform.DOLocalMove(encounterConstants.KnobPlayPos, 0.5f).SetEase(Ease.InOutBack));
        animationSequence.Insert(0.75f, transform.DOScale(2f, 0.5f).SetEase(Ease.InOutBack));

        animationSequence.Play();
    }

    internal void OnMoveLocked(int currentMove)
    {
        animationSequence.Complete(true);

        animationSequence = DOTween.Sequence();

        animationSequence.Insert(0f, moveSprites[currentMove].GetComponent<Image>().DOColor(encounterConstants.moveLockedColour, 0.5f).SetLoops(2, LoopType.Yoyo));
        animationSequence.Insert(0f, moveSprites[currentMove].DOScale(encounterConstants.moveLockedScale, 0.5f).SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo));

        animationSequence.Play();
    }
}
