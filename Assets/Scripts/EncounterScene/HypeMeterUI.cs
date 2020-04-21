using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HypeMeterUI : MonoBehaviour
{
    int hypeValue;
    public bool canHype = false;
    public bool isHyped = false;

    float hypeValueMax = 100f;

    public Image hypeBar;

    Sequence hypeAnimateSequence;
    Sequence hypeIncrementSequence;

    EncounterConstants encounterConstants;

    // Start is called before the first frame update
    void Start()
    {
        encounterConstants = FindObjectOfType<EncounterConstants>();
        hypeValue = encounterConstants.HypeValueStart;
        hypeValueMax = encounterConstants.HypeValueMax;

        hypeBar.transform.DOScaleY(hypeValue / hypeValueMax, 0);
        hypeBar.DOFade(0.5f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncrementHypeValue(int amount)
    {
        
        if (hypeIncrementSequence != null && hypeIncrementSequence.IsPlaying())
        {
            hypeIncrementSequence.Complete(true);
        }


        if (hypeValue > hypeValueMax)
        {
            AnimateCanHype();
        } else
        {
            hypeValue += amount;
            // Do Animation for HypeValue

            hypeIncrementSequence = DOTween.Sequence();
            hypeIncrementSequence.Insert(0, hypeBar.transform.DOScaleY(hypeValue / hypeValueMax, encounterConstants.HypeIncrementDuration).SetEase(Ease.InOutBack));
        }
    }

    public void DecrementHypeValue(int amount)
    {
        if (hypeValue > amount)
        {
            hypeValue -= amount;
        }

        if (hypeIncrementSequence != null && hypeIncrementSequence.IsPlaying())
        {
            hypeIncrementSequence.Complete(true);
        }

        hypeIncrementSequence = DOTween.Sequence();
        hypeIncrementSequence.Insert(0, hypeBar.transform.DOScaleY(hypeValue/hypeValueMax, encounterConstants.HypeIncrementDuration).SetEase(Ease.InOutBack));
    }

    public void TryHype()
    {
        if(canHype && !isHyped)
        {
            AnimateHasHyped();
        } else
        {
            AnimateCantHype();
        }
    }

    void AnimateCanHype()
    {
        canHype = true;

        if (hypeAnimateSequence != null && hypeAnimateSequence.IsPlaying())
        {
            hypeAnimateSequence.Complete(true);
        }
        hypeAnimateSequence = DOTween.Sequence();
        hypeAnimateSequence.Insert(0, transform.DOScale(encounterConstants.CanHypeScale, encounterConstants.CanHypeDuration).SetEase(Ease.InOutBack));

        hypeAnimateSequence.Insert(encounterConstants.CanHypeDuration / 2, hypeBar.DOColor(encounterConstants.CanHypeColor, encounterConstants.CanHypeDuration / 2).SetEase(Ease.InOutQuad));
        hypeAnimateSequence.Insert(encounterConstants.CanHypeDuration, hypeBar.DOFade(0.9f, encounterConstants.CanHypeDuration).SetEase(Ease.InOutBack));

        hypeAnimateSequence.Play();
    }


    void AnimateHasHyped()
    {
        isHyped = true;
        canHype = false;

        if (hypeAnimateSequence != null && hypeAnimateSequence.IsPlaying())
        {
            hypeAnimateSequence.Complete(true);
        }

        hypeAnimateSequence = DOTween.Sequence();

        hypeAnimateSequence.Insert(0, transform.DOScale(encounterConstants.IsHypeScale, encounterConstants.IsHypedDuration).SetEase(Ease.InOutBack));

        hypeAnimateSequence.Insert(encounterConstants.IsHypedDuration/2, hypeBar.DOColor(encounterConstants.IsHypeColor, encounterConstants.IsHypedDuration/2).SetEase(Ease.InOutQuad));
        hypeAnimateSequence.Insert(encounterConstants.IsHypedDuration, hypeBar.DOFade(1f, encounterConstants.IsHypedDuration).SetEase(Ease.InOutBack));

        hypeAnimateSequence.Play();
        // Animate Hype Mask getting larger
        // Emit particles
    }

    void AnimateHypeComplete()
    {
        if (hypeAnimateSequence != null && hypeAnimateSequence.IsPlaying())
        {
            hypeAnimateSequence.Complete(true);
        }

        hypeAnimateSequence = DOTween.Sequence();
        hypeAnimateSequence.Insert(0, transform.DOScale(encounterConstants.BaseHypeScale, encounterConstants.IsHypedDuration).SetEase(Ease.InOutBack));

        hypeAnimateSequence.Insert(encounterConstants.IsHypedDuration / 2, hypeBar.DOColor(encounterConstants.BaseHypeColor, encounterConstants.IsHypedDuration / 2).SetEase(Ease.InOutQuad));

        hypeAnimateSequence.Insert(encounterConstants.IsHypedDuration, hypeBar.DOFade(0.75f, encounterConstants.IsHypedDuration).SetEase(Ease.InOutBack));

        hypeAnimateSequence.Play();
    }

    void AnimateCantHype()
    {
        if(hypeAnimateSequence != null && hypeAnimateSequence.IsPlaying())
        {
            hypeAnimateSequence.Complete(true);
        }

        hypeAnimateSequence = DOTween.Sequence();
        hypeAnimateSequence.Insert(0, transform.DOScale(encounterConstants.CannotHypeScale, 
            encounterConstants.CannotHypeDuration/2).SetEase(Ease.InOutBack));

        hypeAnimateSequence.Insert(0, hypeBar.DOColor(encounterConstants.CannotHypeColor, 
            encounterConstants.CannotHypeDuration/2).SetEase(Ease.InOutQuad));


        hypeAnimateSequence.Insert(encounterConstants.CannotHypeDuration / 2, transform.DOScale(encounterConstants.BaseHypeScale,
            encounterConstants.CannotHypeDuration / 2).SetEase(Ease.InOutBack));

        hypeAnimateSequence.Insert(encounterConstants.CannotHypeDuration, hypeBar.DOColor(encounterConstants.BaseHypeColor,
            encounterConstants.CannotHypeDuration / 2).SetEase(Ease.InOutQuad));

        hypeAnimateSequence.Play();
        //hypeBar.transform.DOScale(encounterConstants.);
    }


}
