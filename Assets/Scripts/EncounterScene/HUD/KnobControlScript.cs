using DG.Tweening;
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
        Sequence deselectSequence = DOTween.Sequence();

        deselectSequence.Insert(0f, selectedSprites[knobPosition].DOScale(1.2f, 0.5f).SetEase(Ease.OutBack));
        deselectSequence.Insert(0f,
            selectedSprites[knobPosition].GetComponent<Image>().
            DOColor(showPlayers ? EncounterConstants.PlayerColors[knobPosition] : EncounterConstants.KnobColors[knobPosition], 0.5f).SetEase(Ease.InOutBack));
        deselectSequence.Insert(0f,
            selectedSprites[knobPosition].GetComponentInChildren<TextMeshProUGUI>().
            DOColor(Color.white, 0.5f).SetEase(Ease.InOutBack));

        Vector3 diff = selectedSprites[knobPosition].transform.position - knobSprite.transform.position;
        diff.Normalize();
        Vector3 rotateTo = new Vector3(0f, 0f, (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) + 180);
        knobSprite.transform.DORotate(rotateTo, 0.35f).SetEase(Ease.InOutBack);


        deselectSequence.Play();
    }

    void AnimationToDeselect(int knobPosition)
    {
        Sequence selectSequence = DOTween.Sequence();

        selectSequence.Insert(0f, selectedSprites[knobPosition].DOScale(0.8f, 0.5f).SetEase(Ease.OutBack));
        selectSequence.Insert(0f,
            selectedSprites[knobPosition].GetComponent<Image>().
            DOColor(Color.white, 0.5f).SetEase(Ease.InOutBack));

        selectSequence.Insert(0f,
            selectedSprites[knobPosition].GetComponentInChildren<TextMeshProUGUI>().
            DOColor(showPlayers ? EncounterConstants.PlayerColors[knobPosition] : EncounterConstants.KnobColors[knobPosition], 0.5f).SetEase(Ease.InOutBack));
        selectSequence.Play();
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

    public void OnPlayerSelect()
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

    public void OnMoveSelect()
    {
        moveLabels.DOFade(1f, 0.5f);
        playerLabels.DOFade(0f, 0.5f);
        showPlayers = false;
        selectedSprites = moveSprites;
        AnimationToSelect(0);
        AnimationToDeselect(1);
        AnimationToDeselect(2);
        AnimationToDeselect(3);
    }
}
