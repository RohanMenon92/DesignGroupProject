using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KnobControlScript : MonoBehaviour
{
    public Image knobSprite;
    public Transform knobRotate;
    public CanvasGroup playerLabels;
    public CanvasGroup moveLabels;
    public Image pointerImage;

    public Sprite selectedImage;
    public Sprite unselectedImage;

    Image[] playerSprites;
    Image[] moveSprites;

    Image[] selectedSprites;

    int currentKnob = 0;

    bool showPlayers = true;

    Color[] PlayerColors;
    Color[] MoveColors;

    EncounterConstants encounterConstants;
    NoteGeneratorManager noteGeneratorManager;

    Sequence animationSequence;

    int knobCount;

    private void Awake()
    {
        encounterConstants = FindObjectOfType<EncounterConstants>();
        noteGeneratorManager = FindObjectOfType<NoteGeneratorManager>();

        PlayerColors = encounterConstants.PlayerColors;

        int index = 0;
        knobCount = PlayerColors.Length;

        playerSprites = playerLabels.GetComponentsInChildren<Image>(true);
        moveSprites = moveLabels.GetComponentsInChildren<Image>(true);

        foreach (Image labelSprite in playerSprites)
        {
            if (index < PlayerColors.Length)
            {
                labelSprite.gameObject.SetActive(true);
            } else
            {
                labelSprite.gameObject.SetActive(false);
            }
            index++;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        moveLabels.DOFade(0f, 0.5f);
        playerLabels.DOFade(1f, 0.5f);
        showPlayers = true;
        selectedSprites = playerSprites;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void AnimationToSelect(int knobPosition)
    {
        SelectImage(knobPosition, true);

        Sequence selectSequence = DOTween.Sequence();

        Color imageColor = showPlayers ? PlayerColors[knobPosition] : noteGeneratorManager.IsMoveUnlocked(knobPosition) ? MoveColors[knobPosition] : encounterConstants.moveLockTextColor;
        Color textColor = showPlayers ? Color.white : noteGeneratorManager.IsMoveUnlocked(knobPosition) ? Color.white : encounterConstants.moveLockColor;

        selectSequence.Insert(0f, knobSprite.DOColor(imageColor, 0.5f));

        selectSequence.Insert(0f, selectedSprites[knobPosition].GetComponent<RectTransform>().DOScale(encounterConstants.moveSelectedScale, 0.5f).SetEase(Ease.OutBack));
        selectSequence.Insert(0f,
            selectedSprites[knobPosition].
            DOColor(imageColor, 0.5f));
        selectSequence.Insert(0f,
            selectedSprites[knobPosition].GetComponentInChildren<TextMeshProUGUI>().
            DOColor(textColor, 0.5f));

        Vector3 diff = selectedSprites[knobPosition].transform.position - knobSprite.transform.position;
        diff.Normalize();
        Vector3 rotateTo = new Vector3(0f, 0f, (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) + 180);
        knobRotate.DORotate(rotateTo, 0.35f).SetEase(Ease.InOutBack);
    }

    private void SelectImage(int knobPosition, bool toggle)
    {
        if(toggle)
        {
            // Set Image and move canvas position
            selectedSprites[knobPosition].transform.SetAsLastSibling();
            selectedSprites[knobPosition].sprite = selectedImage;
        }
        else
        {
            selectedSprites[knobPosition].sprite = unselectedImage;
        }
    }

    void AnimationToDeselect(int knobPosition)
    {
        SelectImage(knobPosition, false);

        Sequence selectSequence = DOTween.Sequence();
        Color imageColor = showPlayers ? Color.white : noteGeneratorManager.IsMoveUnlocked(knobPosition) ? Color.white : encounterConstants.moveLockColor;
        Color textColor = showPlayers ? PlayerColors[knobPosition] : noteGeneratorManager.IsMoveUnlocked(knobPosition) ? MoveColors[knobPosition] : encounterConstants.moveLockTextColor;

        float scaleValue = showPlayers ? encounterConstants.knobDeselectScale : noteGeneratorManager.IsMoveUnlocked(knobPosition) ? encounterConstants.moveLockScale : encounterConstants.knobDeselectScale;

        selectSequence.Insert(0f, selectedSprites[knobPosition].GetComponent<RectTransform>().DOScale(scaleValue, 0.5f).SetEase(Ease.OutBack));

        selectSequence.Insert(0f, selectedSprites[knobPosition]. DOColor(imageColor, 0.5f));
        selectSequence.Insert(0f, selectedSprites[knobPosition].GetComponentInChildren<TextMeshProUGUI>().DOColor(textColor, 0.5f));
    }

    public void NextSelection()
    {
        AnimationToDeselect(currentKnob);
        currentKnob++;
        if (currentKnob >= knobCount)
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
            currentKnob = knobCount - 1;
        }
        AnimationToSelect(currentKnob);
    }

    public void OnPlayerSelector()
    {
        currentKnob = 0;
        playerLabels.transform.localScale = Vector3.one;

        animationSequence = DOTween.Sequence();

        animationSequence.Insert(0f, pointerImage.DOColor(Color.white, 0.5f));
        animationSequence.Insert(0f, moveLabels.DOFade(0f, 0.5f));
        animationSequence.Insert(0f, playerLabels.DOFade(1f, 0.5f));
        showPlayers = true;
        selectedSprites = playerSprites;
        knobCount = PlayerColors.Length;
        int index = 0;

        animationSequence.Insert(0f, knobRotate.DORotate(encounterConstants.firstKnobRotation, 0.35f).SetEase(Ease.InOutBack));

        foreach (Image sprite in selectedSprites)
        {
            TextMeshProUGUI textMesh = sprite.GetComponentInChildren<TextMeshProUGUI>();
            // Fade in Canvas
            animationSequence.Insert(index * 0.1f, sprite.GetComponent<CanvasGroup>().DOFade(1f, 0.5f));

            if(index != 0)
            {
                SelectImage(index, false);
                animationSequence.Insert(0f, sprite.GetComponent<RectTransform>().DOScale(encounterConstants.knobDeselectScale, 0.5f).SetEase(Ease.OutBack));
                sprite.color = Color.white;
                textMesh.color = PlayerColors[index];
            }
            else
            {
                SelectImage(index, true);
                sprite.color = PlayerColors[index];
                textMesh.color = Color.white;

                animationSequence.Insert(0f, sprite.GetComponent<RectTransform>().DOScale(encounterConstants.moveSelectedScale, 0.5f).SetEase(Ease.OutBack));

                animationSequence.Insert(0f, knobSprite.DOColor(PlayerColors[index], 0.5f));
            }
            index++;
        }
    }

    public void OnHypeSelector()
    {
        animationSequence.Insert(0f, playerLabels.transform.DOScale(0.1f, 1f).SetEase(Ease.InOutBack));
        animationSequence.Insert(0f, playerLabels.DOFade(0f, 1f));
        animationSequence.Insert(0f, moveLabels.DOFade(0f, 0.5f));

        animationSequence.Insert(0.75f, transform.DOLocalMove(encounterConstants.KnobPlayPos, 0.5f).SetEase(Ease.InOutBack));
        animationSequence.Insert(0.75f, transform.DOScale(2f, 0.5f).SetEase(Ease.InOutBack));
    }

    public void OnMoveSelector()
    {
        currentKnob = 0;

        animationSequence.Insert(0f, playerLabels.transform.DOScale(0.1f, 1f).SetEase(Ease.InOutBack));
        animationSequence.Insert(0f, playerLabels.DOFade(0f, 1f));

        animationSequence.Insert(0f, moveLabels.DOFade(1f, 0.5f));

        animationSequence.Insert(0, knobRotate.DORotate(encounterConstants.firstKnobRotation, 0.35f)).SetEase(Ease.InOutBack);

        int index = 0;
        foreach (Image labelSprite in playerSprites)
        {
            animationSequence.Insert(0.5f + (index * 0.25f), labelSprite.GetComponent<RectTransform>().DOScale(2f, 0.5f).SetEase(Ease.InOutBack));
            animationSequence.Insert(0.5f + (index * 0.25f), labelSprite.transform.GetComponent<CanvasGroup>().DOFade(0f, 0.5f));
            index++;
        }

        showPlayers = false;
        selectedSprites = moveSprites;

        index = 0;
        foreach (Image sprite in moveSprites)
        {
            TextMeshProUGUI textMesh = sprite.GetComponentInChildren<TextMeshProUGUI>();

            animationSequence.Insert(index * 0.1f, sprite.GetComponent<CanvasGroup>().DOFade(1f, 0.5f));

            if (index != 0)
            {
                SelectImage(index, false);
                if (noteGeneratorManager.IsMoveUnlocked(index))
                {
                    sprite.color = Color.white;
                    textMesh.color = MoveColors[index];

                    animationSequence.Insert(0f, sprite.GetComponent<RectTransform>().DOScale(encounterConstants.knobDeselectScale, 0.5f).SetEase(Ease.OutBack));

                } else
                {
                    sprite.color = encounterConstants.moveLockColor;
                    textMesh.color = encounterConstants.moveLockTextColor;
                    animationSequence.Insert(0f, sprite.GetComponent<RectTransform>().DOScale(encounterConstants.moveLockedScale, 0.5f).SetEase(Ease.OutBack));
                }
            }
            else
            {
                SelectImage(index, true);

                animationSequence.Insert(0f, sprite.GetComponent<RectTransform>().DOScale(encounterConstants.moveSelectedScale, 0.5f).SetEase(Ease.OutBack));

                sprite.color = MoveColors[index];
                textMesh.color = Color.white;
                animationSequence.Insert(0f, knobSprite.DOColor(MoveColors[index], 0.5f));
            }
            index++;
        }

        animationSequence.Play();
    }

    internal void OnPlayerSelected(PlayerMove[] moves, int currentPlayer)
    {
        // Create new colours
        MoveColors = new Color[moves.Length];

        int index = 0;
        foreach(PlayerMove move in moves)
        {
            MoveColors[index] = move.moveColor;
            index++;
        }

        // Iterate through and find moves
        knobCount = moves.Length;

        animationSequence.Insert(0f, pointerImage.DOColor(PlayerColors[currentPlayer], 0.5f));
        animationSequence.Insert(0f, knobRotate.DOScale(encounterConstants.knobSelectScale, 0.25f).SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo));

        index = 0;
        foreach (Image moveSprite in moveSprites)
        {
            if (index < moves.Length)
            {
                moveSprite.gameObject.SetActive(true);

                moveSprite.GetComponentInChildren<TextMeshProUGUI>().text = moves[index].name;
                moveSprite.GetComponentInChildren<TextMeshProUGUI>().ForceMeshUpdate();
                if (!moves[index].IsUnlocked())
                {
                    moveSprite.GetComponentInChildren<TextMeshProUGUI>().color = encounterConstants.moveLockTextColor;
                    moveSprite.color = encounterConstants.moveLockColor;
                    animationSequence.Insert(0f, moveSprite.GetComponent<RectTransform>().DOScale(encounterConstants.moveLockScale, encounterConstants.moveLockScale));
                }
                index++;
            } else
            {
                moveSprite.gameObject.SetActive(false);
            }
        };
    }

    internal void OnMoveSelected(int currentMove)
    {
        animationSequence.Insert(0f, pointerImage.DOColor(MoveColors[currentMove], 0.5f));
        animationSequence.Insert(0f, knobRotate.DOScale(encounterConstants.knobSelectScale, 0.25f).SetEase(Ease.OutBack).SetLoops(2,LoopType.Yoyo));

        int index = 0;
        foreach(Image moveSprite in moveSprites)
        {
            if (index != currentMove)
            {
                animationSequence.Insert(index * 0.25f, moveSprite.GetComponent<RectTransform>().DOScale(0f, 0.5f).SetEase(Ease.InOutBack));
                animationSequence.Insert(index * 0.25f, moveSprite.transform.GetComponent<CanvasGroup>().DOFade(0f, 0.5f));
            }
            index++;
        }
        animationSequence.Insert(0.75f, transform.DOLocalMove(encounterConstants.KnobPlayPos, 0.5f).SetEase(Ease.InOutBack));
        animationSequence.Insert(0.75f, transform.DOScale(encounterConstants.KnobPlayScale, 0.5f).SetEase(Ease.InOutBack));
    }

    internal void OnMoveLocked(int currentMove)
    {
        animationSequence.Insert(0f, selectedSprites[currentMove].DOColor(encounterConstants.moveLockedColour, 0.5f).SetLoops(2, LoopType.Yoyo));
        animationSequence.Insert(0f, selectedSprites[currentMove].GetComponent<RectTransform>().DOScale(encounterConstants.moveLockedScale, 0.5f).SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo));

        animationSequence.Play();
    }
}
