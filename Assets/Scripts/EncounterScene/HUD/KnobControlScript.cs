using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KnobControlScript : MonoBehaviour
{
    public RectTransform knobSprite;
    public List<RectTransform> labelSprites;

    int currentKnob = 0;

    // Start is called before the first frame update
    void Start()
    {
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

        deselectSequence.Insert(0f, labelSprites[knobPosition].DOScale(1.2f, 0.5f).SetEase(Ease.OutBack));
        deselectSequence.Insert(0f,
            labelSprites[knobPosition].GetComponent<Image>().
            DOColor(EncounterConstants.KnobColors[knobPosition], 0.5f).SetEase(Ease.InOutBack));
        deselectSequence.Insert(0f,
            labelSprites[knobPosition].GetComponentInChildren<TextMeshProUGUI>().
            DOColor(Color.white, 0.5f).SetEase(Ease.InOutBack));

        Vector3 diff = labelSprites[knobPosition].transform.position - knobSprite.transform.position;
        diff.Normalize();
        Vector3 rotateTo = new Vector3(0f, 0f, (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) + 180);
        knobSprite.transform.DORotate(rotateTo, 0.35f).SetEase(Ease.InOutBack);


        deselectSequence.Play();
    }

    void AnimationToDeselect(int knobPosition)
    {
        Sequence selectSequence = DOTween.Sequence();

        selectSequence.Insert(0f, labelSprites[knobPosition].DOScale(0.8f, 0.5f).SetEase(Ease.OutBack));
        selectSequence.Insert(0f,
            labelSprites[knobPosition].GetComponent<Image>().
            DOColor(Color.white, 0.5f).SetEase(Ease.InOutBack));
        selectSequence.Insert(0f,
            labelSprites[knobPosition].GetComponentInChildren<TextMeshProUGUI>().
            DOColor(EncounterConstants.KnobColors[knobPosition], 0.5f).SetEase(Ease.InOutBack));
        selectSequence.Play();
    }

    public void NextSelection()
    {
        AnimationToDeselect(currentKnob);
        currentKnob++;
        if (currentKnob >= labelSprites.Count)
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
            currentKnob = labelSprites.Count - 1;
        }
        AnimationToSelect(currentKnob);
    }

}
