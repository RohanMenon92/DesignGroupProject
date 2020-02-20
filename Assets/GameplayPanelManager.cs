﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayPanelManager : MonoBehaviour
{
    public Image repBarBG;
    public Image repBarScorePointer;
    public Image repBarTurnPointer;
    public Image repBarEnemyPointer;

    public int reputation;

    Sequence currScoreTween;
    Sequence turnScoreTween;
    Sequence enemyScoreTween;

    // Start is called before the first frame update
    void Start()
    {
        // SetReputationBarWidth
        repBarBG.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, EncounterConstants.repBarWidth + 400f);

        repBarTurnPointer.DOColor(Color.cyan, 0.5f).OnComplete(() => {
            repBarTurnPointer.DOFade(0.0f, 0.25f);
        });
        repBarEnemyPointer.DOColor(Color.red, 0.5f).OnComplete(() => {
            repBarEnemyPointer.DOFade(0.0f, 0.25f);
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTurnScore(float newTurnScore)
    {
        turnScoreTween = DOTween.Sequence();
        turnScoreTween.Insert(0f,
            repBarTurnPointer.rectTransform
            .DOAnchorPosX(-(newTurnScore / EncounterConstants.maxScore) * (EncounterConstants.repBarWidth / 2), 0.2f)
            .SetEase(Ease.InOutSine));

        turnScoreTween.Play();
    }

    public void UpdateEnemyScore(float newEnemyScore)
    {
        if (enemyScoreTween != null && enemyScoreTween.IsPlaying())
        {
            enemyScoreTween.Kill();
        }

        enemyScoreTween = DOTween.Sequence();
        enemyScoreTween.Insert(0f,
            repBarEnemyPointer.rectTransform
            .DOAnchorPosX(-(newEnemyScore / EncounterConstants.maxScore) * (EncounterConstants.repBarWidth / 2), 0.2f)
            .SetEase(Ease.InOutSine));

        enemyScoreTween.Play();
    }

    public void ShowTurnUI()
    {
        repBarTurnPointer.rectTransform.position = repBarScorePointer.rectTransform.position;
        repBarTurnPointer.DOColor(Color.cyan, 0.5f).OnComplete(() => {
            repBarTurnPointer.DOFade(0.5f, 0.25f);
        });

    }

    public void ShowEnemyUI()
    {
        repBarEnemyPointer.rectTransform.position = repBarScorePointer.rectTransform.position;
        repBarEnemyPointer.DOColor(Color.red, 0.5f).OnComplete(() => {
            repBarEnemyPointer.DOFade(0.5f, 0.25f);
        });
    }

    public void OnTurnEnd(float newCurrentScore)
    {
        repBarTurnPointer.DOFade(0.0f, 0.25f);

        if (currScoreTween != null && currScoreTween.IsPlaying())
        {
            currScoreTween.Kill();
        }

        currScoreTween = DOTween.Sequence();
        currScoreTween.Insert(0f,
            repBarScorePointer.rectTransform
            .DOAnchorPosX(-(newCurrentScore / EncounterConstants.maxScore) * (EncounterConstants.repBarWidth / 2), 0.5f)
            .SetEase(Ease.InOutBack));

        currScoreTween.Play();


        //currScoreTween.Insert(0f, repBarScorePointer.transform.DOMove());
    }

    public void OnEnemyEnd(float newCurrentScore)
    {
        repBarEnemyPointer.DOFade(0.0f, 0.25f);

        currScoreTween = DOTween.Sequence();
        currScoreTween.Insert(0f,
            repBarScorePointer.rectTransform
            .DOAnchorPosX(-(newCurrentScore / EncounterConstants.maxScore) * (EncounterConstants.repBarWidth / 2), 0.5f)
            .SetEase(Ease.InOutBack));

        currScoreTween.Play();


        //currScoreTween.Insert(0f, repBarScorePointer.transform.DOMove());
    }
}
