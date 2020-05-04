using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EncounterGameManager;

public class MainUIScript : MonoBehaviour
{
    public CanvasGroup textCanvas;
    // Start is called before the first frame update
    public Image overlayImage;
    public Image bandBattleImage;
    public Image band1Poster;
    public Image band2Poster;

    public Image winnerStamp;

    public TextMeshProUGUI textDescription;
    public TextMeshProUGUI textEvent;

    public RectTransform buttonIconParent;
    public EncounterConstants encounterConstants;

    List<TextMeshProUGUI> buttonIcons = new List<TextMeshProUGUI>();
    private Sequence uiSequence;
    private Sequence textSequence;

    void Awake()
    {
        overlayImage.color = Color.white;
        textCanvas.alpha = 0;
        textEvent.alpha = 0;
        textDescription.alpha = 0;
        bandBattleImage.color = new Color(1,1,1,0);
        band1Poster.color = new Color(1, 1, 1, 0);
        band2Poster.color = new Color(1, 1, 1, 0);
        winnerStamp.color = new Color(1, 1, 1, 0);

        encounterConstants = FindObjectOfType<EncounterConstants>();

        foreach (TextMeshProUGUI buttonIcon in buttonIconParent.GetComponentsInChildren<TextMeshProUGUI>())
        {
            buttonIcons.Add(buttonIcon);
        }
    }

    void Start()
    {
    }

    public void FadeUI()
    {

    }

    private void ShowTextDescription(UIDescripton uIDescripton)
    {
        textSequence.Complete();

        textSequence = DOTween.Sequence();
        // Fade out text and change values
        textSequence.Insert(0f, textCanvas.DOFade(0f, encounterConstants.textFadeDuration / 5).OnComplete(() =>
        {
            textDescription.text = uIDescripton.descriptionText;
            textDescription.ForceMeshUpdate();

            if (!String.IsNullOrEmpty(uIDescripton.acceptText))
            {
                buttonIcons[0].gameObject.SetActive(true);
                buttonIcons[0].text = uIDescripton.acceptText;
                buttonIcons[0].ForceMeshUpdate();
                buttonIcons[0].alpha = 0;
            }
            else
            {
                buttonIcons[0].gameObject.SetActive(false);
            }
            if (!String.IsNullOrEmpty(uIDescripton.cancelText))
            {
                buttonIcons[1].gameObject.SetActive(true);
                buttonIcons[1].text = uIDescripton.cancelText;
                buttonIcons[1].ForceMeshUpdate();
                buttonIcons[1].alpha = 0;
            }
            else
            {
                buttonIcons[1].gameObject.SetActive(false);
            }
            if (!String.IsNullOrEmpty(uIDescripton.selectText))
            {
                buttonIcons[2].gameObject.SetActive(true);
                buttonIcons[2].text = uIDescripton.selectText;
                buttonIcons[2].ForceMeshUpdate();
                buttonIcons[2].alpha = 0;
            }
            else
            {
                buttonIcons[2].gameObject.SetActive(false);
            }
            if (!String.IsNullOrEmpty(uIDescripton.specialText))
            {
                buttonIcons[3].gameObject.SetActive(true);
                buttonIcons[3].text = uIDescripton.specialText;
                buttonIcons[3].ForceMeshUpdate();
                buttonIcons[3].alpha = 0;
            }
            else
            {
                buttonIcons[3].gameObject.SetActive(false);
            }

            textCanvas.DOFade(1f, encounterConstants.textFadeDuration * 4 / 5).OnComplete(() => {
                textDescription.DOFade(1f, encounterConstants.textFadeDuration /2);
                foreach(TextMeshProUGUI buttonText in buttonIcons)
                {
                    if(buttonText.gameObject.activeSelf)
                    {
                        buttonText.DOFade(1f, encounterConstants.textFadeDuration / 2);
                    }   
                }
            });
        }));
    }


    public void StartUI(AnimationCallback animCallback)
    {
        uiSequence.Complete();
        Transform band1Transform = band1Poster.GetComponent<Transform>();
        Transform band2Transform = band2Poster.GetComponent<Transform>();

        band1Transform.DOScale(0f, 0f);
        band2Transform.DOScale(0f, 0f);

        ShowTextDescription(encounterConstants.UITextSets[0]);

        uiSequence = DOTween.Sequence();
        uiSequence.Insert(0f, bandBattleImage.DOFade(1f, encounterConstants.startUIFadeDuration / 2));
        uiSequence.Insert(encounterConstants.startUIFadeDuration / 4, band1Poster.DOFade(1f, encounterConstants.startUIFadeDuration / 4));
        uiSequence.Insert(encounterConstants.startUIFadeDuration / 4, band1Transform.DOScale(1f, encounterConstants.startUIFadeDuration / 3).SetEase(Ease.InOutBack));

        uiSequence.Insert(encounterConstants.startUIFadeDuration / 3, band2Poster.DOFade(1f, encounterConstants.startUIFadeDuration / 4));
        uiSequence.Insert(encounterConstants.startUIFadeDuration / 3, band2Transform.DOScale(1f, encounterConstants.startUIFadeDuration / 3).SetEase(Ease.InOutBack));

        uiSequence.Insert(encounterConstants.startUIFadeDuration / 2, overlayImage.DOFade(0f, encounterConstants.startUIFadeDuration / 2).OnComplete(() => {
            animCallback?.Invoke();
        }));
    }

    public void StartGameUI(AnimationCallback animCallback)
    {
        uiSequence.Complete();
        Transform band1Transform = band1Poster.GetComponent<Transform>();
        Transform band2Transform = band2Poster.GetComponent<Transform>();

        ShowTextDescription(encounterConstants.UITextSets[1]);

        uiSequence = DOTween.Sequence();
        uiSequence.Insert(encounterConstants.startGameUIFadeDuration / 2, bandBattleImage.DOFade(0f, encounterConstants.startGameUIFadeDuration / 2));
        uiSequence.Insert(encounterConstants.startGameUIFadeDuration / 2, bandBattleImage.GetComponent<Transform>().DOScale(1.5f, encounterConstants.startGameUIFadeDuration / 2).SetEase(Ease.InOutBack));

        uiSequence.Insert(encounterConstants.startGameUIFadeDuration / 4, band1Poster.DOFade(0f, encounterConstants.startGameUIFadeDuration / 4));
        uiSequence.Insert(encounterConstants.startGameUIFadeDuration / 4, band1Transform.DOScale(0.8f, encounterConstants.startGameUIFadeDuration / 3).SetEase(Ease.InOutBack));

        uiSequence.Insert(encounterConstants.startUIFadeDuration / 4, band2Poster.DOFade(0f, encounterConstants.startUIFadeDuration / 4));
        uiSequence.Insert(encounterConstants.startUIFadeDuration / 4, band2Transform.DOScale(0.8f, encounterConstants.startUIFadeDuration / 3).SetEase(Ease.InOutBack));

        uiSequence.OnComplete(() =>
        {
            band1Poster.gameObject.SetActive(false);
            band2Poster.gameObject.SetActive(false);
            bandBattleImage.gameObject.SetActive(false);
            animCallback?.Invoke();
        });
    }

    public void IntroUI(bool canHype, AnimationCallback animCallback)
    {
        uiSequence.Complete();

        uiSequence = DOTween.Sequence();
        // Fade out text and change values

        // If canHype, Show different text 
        ShowTextDescription(canHype ? encounterConstants.UITextSets[8] : encounterConstants.UITextSets[2]);

        uiSequence.OnComplete(() =>
        {
            animCallback?.Invoke();
        });
    }

    public void MoveSelectUI(AnimationCallback animCallback)
    {
        uiSequence.Complete();

        uiSequence = DOTween.Sequence();

        ShowTextDescription(encounterConstants.UITextSets[3]);

        uiSequence.OnComplete(() =>
        {
            animCallback?.Invoke();
        });
    }

    public void EnemyIntroUI(AnimationCallback animCallback)
    {
        uiSequence.Complete();

        uiSequence = DOTween.Sequence();

        ShowTextDescription(encounterConstants.UITextSets[5]);

        uiSequence.OnComplete(() =>
        {
            animCallback?.Invoke();
        });
    }

    public void TurnPlayoutUI(bool isBetter, AnimationCallback animCallback)
    {
        uiSequence.Complete();

        uiSequence = DOTween.Sequence();

        ShowTextDescription(isBetter ? encounterConstants.UITextSets[6]: encounterConstants.UITextSets[7]);

        uiSequence.OnComplete(() =>
        {
            animCallback?.Invoke();
        });
    }
    public void EndGameUI(bool hasWon, AnimationCallback animCallback)
    {
        uiSequence.Complete();

        RectTransform band1Transform = band1Poster.GetComponent<RectTransform>();
        RectTransform band2Transform = band2Poster.GetComponent<RectTransform>();

        bandBattleImage.transform.localScale = Vector3.zero;
        band1Transform.localScale = Vector3.zero;
        band2Transform.localScale = Vector3.zero;

        band1Poster.gameObject.SetActive(true);
        band2Poster.gameObject.SetActive(true);
        bandBattleImage.gameObject.SetActive(true);

        uiSequence = DOTween.Sequence();

        uiSequence.Insert(0f, bandBattleImage.DOFade(1f, encounterConstants.startUIFadeDuration / 2));
        uiSequence.Insert(encounterConstants.startUIFadeDuration / 4, band1Poster.DOFade(1f, encounterConstants.startUIFadeDuration / 4));
        uiSequence.Insert(encounterConstants.startUIFadeDuration / 4, band1Transform.DOScale(1f, encounterConstants.startUIFadeDuration / 3).SetEase(Ease.InOutBack));

        uiSequence.Insert(encounterConstants.startUIFadeDuration / 3, band2Poster.DOFade(1f, encounterConstants.startUIFadeDuration / 4));
        uiSequence.Insert(encounterConstants.startUIFadeDuration / 3, band2Transform.DOScale(1f, encounterConstants.startUIFadeDuration / 3).SetEase(Ease.InOutBack));

        ShowTextDescription(hasWon ? encounterConstants.UITextSets[9] : encounterConstants.UITextSets[10]);

        winnerStamp.GetComponent<RectTransform>().position = hasWon ? band1Transform.position: band2Transform.position;

        uiSequence.Insert(encounterConstants.startUIFadeDuration / 2, winnerStamp.DOFade(1f, encounterConstants.startUIFadeDuration / 3));

        uiSequence.OnComplete(() =>
        {
            animCallback?.Invoke();
        });
    }


    public void FadeTextDescription(AnimationCallback animCallback)
    {
        uiSequence.Complete();

        uiSequence = DOTween.Sequence();

        uiSequence.Insert(0f, textCanvas.DOFade(0f, encounterConstants.textFadeDuration));

        uiSequence.OnComplete(() =>
        {
            animCallback?.Invoke();
        });

        //textDescription;
        // Fade out text
    }

    public void ShowEventText(TextMeshProUGUI textAsset, string text)
    {
        uiSequence.Complete();

        // Change Description Text
        // Fade In text
        // Fade Out text after set delay
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
