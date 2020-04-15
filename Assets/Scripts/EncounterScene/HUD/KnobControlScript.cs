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

    bool showPlayers = false;


    // Start is called before the first frame update
    void Start()
    {
        moveLabels.DOFade(0f, 0.5f);
        playerLabels.DOFade(1f, 0.5f);
        showPlayers = true;
        selectedSprites = labelSprites;

        AnimationToSelect(0);
        AnimationToDeselect(1);
        AnimationToDeselect(2);
        AnimationToDeselect(3);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void AnimationToSelect(int knobPosition)
    {
        Sequence selectSequence = DOTween.Sequence();

        selectSequence.Insert(0f, selectedSprites[knobPosition].DOScale(1.2f, 0.5f).SetEase(Ease.OutBack));
        selectSequence.Insert(0f,
            selectedSprites[knobPosition].GetComponent<Image>().
            DOColor(showPlayers ? EncounterConstants.PlayerColors[knobPosition] : EncounterConstants.KnobColors[knobPosition], 0.5f));
        selectSequence.Insert(0f,
            selectedSprites[knobPosition].GetComponentInChildren<TextMeshProUGUI>().
            DOColor(Color.white, 0.5f));

        Vector3 diff = selectedSprites[knobPosition].transform.position - knobSprite.transform.position;
        diff.Normalize();
        Vector3 rotateTo = new Vector3(0f, 0f, (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) + 180);
        knobSprite.transform.DORotate(rotateTo, 0.35f).SetEase(Ease.InOutBack);
    }

    void AnimationToDeselect(int knobPosition)
    {
        Sequence selectSequence = DOTween.Sequence();

        selectSequence.Insert(0f, selectedSprites[knobPosition].DOScale(0.8f, 0.5f).SetEase(Ease.OutBack));
        selectSequence.Insert(0f,
            selectedSprites[knobPosition].GetComponent<Image>().
            DOColor(Color.white, 0.5f));

        selectSequence.Insert(0f,
            selectedSprites[knobPosition].GetComponentInChildren<TextMeshProUGUI>().
            DOColor(showPlayers ? EncounterConstants.PlayerColors[knobPosition] : EncounterConstants.KnobColors[knobPosition], 0.5f));
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

        Sequence selectSequence = DOTween.Sequence();

        selectSequence.Insert(0f, moveLabels.DOFade(0f, 0.5f));
        selectSequence.Insert(0f, playerLabels.DOFade(1f, 0.5f));
        showPlayers = true;
        selectedSprites = labelSprites;

        int index = 0;
        foreach(RectTransform sprite in labelSprites)
        {
            if(index != 0)
            {
                selectSequence.Insert(0f, sprite.DOScale(0.8f, 0.5f).SetEase(Ease.OutBack));
                selectSequence.Insert(0f,
                    sprite.GetComponent<Image>().
                    DOColor(Color.white, 0.5f));

                selectSequence.Insert(0f,
                    sprite.GetComponentInChildren<TextMeshProUGUI>().
                    DOColor(EncounterConstants.PlayerColors[index], 0.5f));
                selectSequence.Insert(0f,
                    sprite.GetComponentInChildren<TextMeshProUGUI>().
                    DOFade(1f, 0.5f));
            } else
            {
                selectSequence.Insert(0f, sprite.DOScale(1.2f, 0.5f).SetEase(Ease.OutBack));
                selectSequence.Insert(0f,
                    sprite.GetComponent<Image>().
                    DOColor(EncounterConstants.PlayerColors[index], 0.5f));
                selectSequence.Insert(0f,
                    sprite.GetComponentInChildren<TextMeshProUGUI>().
                    DOColor(Color.white, 0.5f));
                selectSequence.Insert(0f,
                    sprite.GetComponentInChildren<TextMeshProUGUI>().
                    DOFade(1f, 0.5f));
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

        Sequence selectSequence = DOTween.Sequence();

        selectSequence.Insert(0f, playerLabels.transform.DOScale(0.1f, 1f).SetEase(Ease.InOutBack));
        selectSequence.Insert(0f, playerLabels.DOFade(0f, 1f));

        selectSequence.Insert(0f, moveLabels.DOFade(1f, 0.5f));

        int index = 0;
        foreach (RectTransform moveSprite in moveSprites)
        {
            moveSprite.localScale = Vector3.zero;
            selectSequence.Insert(0.5f + (index * 0.25f), moveSprite.DOScale(1f, 0.5f).SetEase(Ease.InOutBack));
            selectSequence.Insert(0.5f + (index * 0.25f), moveSprite.transform.GetComponent<CanvasGroup>().DOFade(1f, 0.5f));
            index++;
        }

        showPlayers = false;
        selectedSprites = moveSprites;

        index = 0;
        foreach (RectTransform sprite in moveSprites)
        {
            if (index != 0)
            {
                selectSequence.Insert(0f, sprite.DOScale(0.8f, 0.5f).SetEase(Ease.OutBack));
                selectSequence.Insert(0f,
                    sprite.GetComponent<Image>().
                    DOColor(Color.white, 0.5f));

                selectSequence.Insert(0f,
                    sprite.GetComponentInChildren<TextMeshProUGUI>().
                    DOColor(EncounterConstants.KnobColors[index], 0.5f));
            }
            else
            {
                selectSequence.Insert(0f, sprite.DOScale(1.2f, 0.5f).SetEase(Ease.OutBack));
                selectSequence.Insert(0f,
                    sprite.GetComponent<Image>().
                    DOColor(EncounterConstants.KnobColors[index], 0.5f));
                selectSequence.Insert(0f,
                    sprite.GetComponentInChildren<TextMeshProUGUI>().
                    DOColor(Color.white, 0.5f));

                Vector3 diff = sprite.transform.position - knobSprite.transform.position;
                diff.Normalize();
                Vector3 rotateTo = new Vector3(0f, 0f, (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) + 180);
                knobSprite.transform.DORotate(rotateTo, 0.35f).SetEase(Ease.InOutBack);
            }
            index++;
        }

        selectSequence.Play();
    }

    internal void OnPlayerSelected(int currentPlayer)
    {
        string[] moveNames = EncounterConstants.MoveNames[currentPlayer];

        int index = 0;
        foreach(string moveName in moveNames)
        {
            moveSprites[index].GetComponentInChildren<TextMeshProUGUI>().text = moveName;
            index++;
        }
    }

    internal void OnMoveSelected(int currentMove)
    {
        Sequence selectSequence = DOTween.Sequence();

        int index = 0;
        foreach(RectTransform moveSprite in moveSprites)
        {
            if (index != currentMove)
            {
                selectSequence.Insert(index * 0.25f, moveSprite.DOScale(0f, 0.5f).SetEase(Ease.InOutBack));
                selectSequence.Insert(index * 0.25f, moveSprite.transform.GetComponent<CanvasGroup>().DOFade(0f, 0.5f));
            }
            index++;
        }
        selectSequence.Insert(0.75f, transform.DOLocalMove(EncounterConstants.KnobPlayPos, 0.5f).SetEase(Ease.InOutBack));
        selectSequence.Insert(0.75f, transform.DOScale(2f, 0.5f).SetEase(Ease.InOutBack));

        selectSequence.Play();
    }
}
