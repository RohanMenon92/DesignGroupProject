using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using static NoteGeneratorManager;

public class HypeMeterUI : MonoBehaviour
{
    int hypeValue;
    public bool canHype = false;
    public bool isHyped = false;

    float hypeValueMax = 100f;

    public Image hypeBar;

    Sequence hypeAnimateSequence;
    Sequence hypeChangeSequence;

    EncounterConstants encounterConstants;
    NoteGeneratorManager noteGeneratorManager;
    // Start is called before the first frame update
    AudioManager audioManager;
    void Start()
    {
        encounterConstants = FindObjectOfType<EncounterConstants>();
        noteGeneratorManager = FindObjectOfType<NoteGeneratorManager>();
        hypeValue = encounterConstants.HypeValueStart;
        hypeValueMax = encounterConstants.HypeValueMax;

        audioManager = FindObjectOfType<AudioManager>();
        hypeBar.transform.DOScaleY(hypeValue / hypeValueMax, 0);
        hypeBar.DOFade(0.5f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncrementHypeValue(int amount)
    {
        if(isHyped)
        {
            return;
        }

        if (hypeChangeSequence != null)
        {
            hypeChangeSequence.Complete(true);
        }


        if (hypeValue > hypeValueMax && !canHype)
        {
            AnimateCanHype();
        } else
        {
            hypeValue += amount;
            // Do Animation for HypeValue

            hypeChangeSequence = DOTween.Sequence();
            hypeChangeSequence.Insert(0, hypeBar.transform.DOScaleY(hypeValue / hypeValueMax, encounterConstants.HypeIncrementDuration).SetEase(Ease.InOutBack));
        }
    }

    public void DecrementHypeValue(int amount)
    {
        if (isHyped)
        {
            return;
        }

        if (hypeValue > amount)
        {
            hypeValue -= amount;
        }

        if (hypeChangeSequence != null)
        {
            hypeChangeSequence.Complete(true);
        }

        hypeChangeSequence = DOTween.Sequence();

        if (canHype)
        {
            canHype = false;
            hypeChangeSequence.Insert(0, hypeBar.DOColor(encounterConstants.CanHypeColor, encounterConstants.HypeIncrementDuration / 2));
        }

        hypeChangeSequence.Insert(0, hypeBar.transform.DOScaleY(hypeValue/hypeValueMax, encounterConstants.HypeIncrementDuration).SetEase(Ease.InOutBack));
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
        if(canHype)
        {
            return;
        }

        canHype = true;

        if (hypeAnimateSequence != null)
        {
            hypeAnimateSequence.Complete(true);
        }
        hypeAnimateSequence = DOTween.Sequence();
        hypeAnimateSequence.Insert(0, transform.DOScale(encounterConstants.CanHypeScale, encounterConstants.CanHypeDuration).SetEase(Ease.InOutBack));

        hypeAnimateSequence.Insert(encounterConstants.CanHypeDuration / 2, hypeBar.DOColor(encounterConstants.CanHypeColor, encounterConstants.CanHypeDuration / 2));
        hypeAnimateSequence.Insert(encounterConstants.CanHypeDuration, hypeBar.DOFade(0.9f, encounterConstants.CanHypeDuration).SetEase(Ease.InOutBack));

        // Play Audio Effect for can hype
        audioManager.PlaySoundEffect(SoundEffects.SetComplete);

        hypeAnimateSequence.Play();
    }


    void AnimateHasHyped()
    {
        isHyped = true;
        canHype = false;

        if (hypeAnimateSequence != null)
        {
            hypeAnimateSequence.Complete(true);
        }

        hypeAnimateSequence = DOTween.Sequence();

        hypeAnimateSequence.Insert(0, transform.DOScale(encounterConstants.IsHypeScale, encounterConstants.IsHypedDuration).SetEase(Ease.InOutBack));

        hypeAnimateSequence.Insert(encounterConstants.IsHypedDuration/2, hypeBar.DOColor(encounterConstants.IsHypeColor, encounterConstants.IsHypedDuration/2).SetEase(Ease.InOutQuad));
        hypeAnimateSequence.Insert(encounterConstants.IsHypedDuration, hypeBar.DOFade(1f, encounterConstants.IsHypedDuration).SetEase(Ease.InOutBack));

        hypeAnimateSequence.OnComplete(() => {
            noteGeneratorManager.HasHyped();
        });
        // Animate Hype Mask getting larger
        // Emit particles

        audioManager.PlaySoundEffect(SoundEffects.SetComplete);
    }

    public void AnimateHypeComplete(AnimationCallback animCallback)
    {
        hypeValue = 0;
        if (hypeAnimateSequence != null && hypeAnimateSequence.IsPlaying())
        {
            hypeAnimateSequence.Complete(true);
        }

        hypeAnimateSequence = DOTween.Sequence();

        hypeChangeSequence.Insert(0, hypeBar.transform.DOScaleY(hypeValue / hypeValueMax, encounterConstants.HypeIncrementDuration*3).SetEase(Ease.InOutBack));

        hypeAnimateSequence.Insert(0, transform.DOScale(encounterConstants.BaseHypeScale, encounterConstants.IsHypedDuration).SetEase(Ease.InOutBack));

        hypeAnimateSequence.Insert(encounterConstants.IsHypedDuration / 2, hypeBar.DOColor(encounterConstants.BaseHypeColor, encounterConstants.IsHypedDuration / 2).SetEase(Ease.InOutQuad));

        hypeAnimateSequence.Insert(encounterConstants.IsHypedDuration, hypeBar.DOFade(0.75f, encounterConstants.IsHypedDuration).SetEase(Ease.InOutBack));

        FindObjectOfType<AudioManager>().PlaySoundEffect(SoundEffects.Bing);

        hypeAnimateSequence.OnComplete(() => {
            animCallback?.Invoke();
        });
    }

    void AnimateCantHype()
    {
        if(hypeAnimateSequence != null)
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
    }


}
