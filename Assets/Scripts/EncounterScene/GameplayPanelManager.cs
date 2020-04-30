using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EncounterGameManager;

public class GameplayPanelManager : MonoBehaviour
{
    public Image repBarBG;
    public Image repBarScorePointer;
    public Image repBarTurnPointer;
    public Image repBarEnemyPointer;

    public int reputation;

    public CanvasGroup countBG;
    public TextMeshProUGUI setCount;
    public TextMeshProUGUI songCount;


    Sequence currScoreTween;
    Sequence turnScoreTween;
    Sequence enemyScoreTween;

    EncounterConstants encounterConstants;
    private Sequence songSetSequence;
    private Sequence hideSetSequence;

    // Start is called before the first frame update
    void Start()
    {
        encounterConstants = FindObjectOfType<EncounterConstants>();
        // SetReputationBarWidth
        repBarBG.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, encounterConstants.repBarWidth + 400f);

        repBarTurnPointer.DOColor(Color.cyan, 0.5f).OnComplete(() => {
            repBarTurnPointer.DOFade(0.0f, 0.25f);
        });
        repBarEnemyPointer.DOColor(Color.red, 0.5f).OnComplete(() => {
            repBarEnemyPointer.DOFade(0.0f, 0.25f);
        });

        countBG.GetComponent<RectTransform>().localScale = new Vector3(1, 0, 1);
    }

    void UpdateSongCount(int currentCount, int totalCount)
    {
        songSetSequence.Insert(0f, songCount.GetComponent<RectTransform>()
            .DOScale(encounterConstants.showCountBounce, encounterConstants.showCountDuration/2)
            .SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutBack)
            .OnStepComplete(() => {
                songCount.text = string.Format(encounterConstants.SongCountText, currentCount, totalCount);
                songCount.ForceMeshUpdate();
            }))
            .OnComplete(() => {
                HideSongSet();
            });
    }

    void UpdateSetCount(int currentCount, int totalCount)
    {
        songSetSequence.Insert(0f, setCount.GetComponent<RectTransform>()
            .DOScale(encounterConstants.showCountBounce, encounterConstants.showCountDuration/2)
            .SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutBack)
            .OnStepComplete(() => {
                setCount.text = string.Format(encounterConstants.SetCountText, currentCount, totalCount);
                setCount.ForceMeshUpdate();
            }))
            .OnComplete(() => {
                HideSongSet();
            });
    }


    public void ShowSongSet(int currSet, int currSong, bool updateSet, bool updateSong)
    {
        if(!updateSet && !updateSong)
        {
            songCount.text = string.Format(encounterConstants.SongCountText, currSong, encounterConstants.setLength);
            setCount.text = string.Format(encounterConstants.SetCountText, currSet, encounterConstants.totalSets);
            songCount.ForceMeshUpdate();
            setCount.ForceMeshUpdate();
        }

        songSetSequence.Insert(0f, countBG.GetComponent<RectTransform>().DOScale(new Vector3(1f, 1f, 1f), encounterConstants.showCountDuration/2).SetEase(Ease.OutBack)
            .OnComplete(() => {
            if (updateSong)
            {
                UpdateSongCount(currSong, encounterConstants.setLength);
            }
            else if (updateSet)
            {
                UpdateSetCount(currSet, encounterConstants.totalSets);
            }
            else
            {
                HideSongSet();
            }
        }));

        songSetSequence.Insert(encounterConstants.showCountDuration/4, setCount.DOFade(1.0f, encounterConstants.showCountDuration));

        songSetSequence.Insert(encounterConstants.showCountDuration/4, songCount.DOFade(1.0f, encounterConstants.showCountDuration));
    }

    void HideSongSet()
    {
        hideSetSequence = DOTween.Sequence();
        hideSetSequence.Insert(encounterConstants.showCountDuration, setCount.DOFade(0.0f, encounterConstants.showCountDuration/2));
        hideSetSequence.Insert(encounterConstants.showCountDuration, songCount.DOFade(0.0f, encounterConstants.showCountDuration/2));
        hideSetSequence.Insert(encounterConstants.showCountDuration * 1.5f, countBG.GetComponent<RectTransform>().DOScale(new Vector3(1f, 0f, 1f), encounterConstants.showCountDuration / 2).SetEase(Ease.InBack));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTurnScore(float newTurnScore)
    {
        turnScoreTween.Complete();
        turnScoreTween = DOTween.Sequence();
        turnScoreTween.Insert(0f,
            repBarTurnPointer.rectTransform
            .DOAnchorPosX(-(newTurnScore / encounterConstants.maxScore) * (encounterConstants.repBarWidth / 2), 0.2f)
            .SetEase(Ease.OutBack));

        turnScoreTween.Play();
    }

    public void UpdateEnemyScore(float newEnemyScore)
    {
        enemyScoreTween.Complete();
        enemyScoreTween = DOTween.Sequence();
        enemyScoreTween.Insert(0f,
            repBarEnemyPointer.rectTransform
            .DOAnchorPosX(-(newEnemyScore / encounterConstants.maxScore) * (encounterConstants.repBarWidth / 2), 0.2f)
            .SetEase(Ease.OutBack));

        enemyScoreTween.Play();
    }

    public void ShowTurnUI(float turnScore)
    {
        repBarTurnPointer.rectTransform.DOAnchorPosX((-(turnScore / encounterConstants.maxScore) * (encounterConstants.repBarWidth / 2)), 0.5f).SetEase(Ease.InOutBack);
        repBarTurnPointer.DOColor(Color.cyan, 0.5f).OnComplete(() => {
            repBarTurnPointer.DOFade(0.5f, 0.25f);
        });

    }

    public void ShowEnemyUI(float enemyScore)
    {
        repBarEnemyPointer.rectTransform.DOAnchorPosX(-(enemyScore / encounterConstants.maxScore) * (encounterConstants.repBarWidth / 2), 0.5f).SetEase(Ease.InOutBack);
        repBarEnemyPointer.DOColor(Color.red, 0.5f).OnComplete(() => {
            repBarEnemyPointer.DOFade(0.5f, 0.25f);
        });
    }

    public void OnTurnEnd(float newCurrentScore)
    {
        // Fade enemy and turn bar
        repBarEnemyPointer.DOFade(0.0f, 0.25f);
        repBarTurnPointer.DOFade(0.0f, 0.25f);

        if (currScoreTween != null && currScoreTween.IsPlaying())
        {
            currScoreTween.Kill();
        }

        // do tween to new score
        currScoreTween = DOTween.Sequence();
        currScoreTween.Insert(0f,
            repBarScorePointer.rectTransform
            .DOAnchorPosX(-(newCurrentScore / encounterConstants.maxScore) * (encounterConstants.repBarWidth / 2), 0.5f)
            .SetEase(Ease.InOutBack));

        currScoreTween.Play();
    }
}
