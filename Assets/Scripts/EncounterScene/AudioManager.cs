using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
using System;

public class AudioManager : MonoBehaviour
{
    public AudioClip moveSelect;
    public AudioClip playerSelect;
    public AudioClip menuNext;
    public AudioClip perfect;
    public AudioClip setComplete;
    public AudioClip setFailure;
    public AudioClip setDing;
    public AudioClip setBing;
    public AudioClip wrongPress;

    public AudioSource audioMenu;
    public AudioSource audioNotes;
    public AudioSource audioCrowdStart;
    public AudioSource audioCrowdSet;
    public AudioSource audioCrowdBG;

    public List<AudioSource> playerAudios;

    // Music maps definition
    [System.Serializable]
    public class AudioSet
    {
        public List<AudioClip> list;
    }
    [System.Serializable]
    public class SongList
    {
        public List<AudioSet> list;
    }
    public SongList musicClips;
    
    EncounterConstants encounterConstants;

    // Start is called before the first frame update
    void Awake()
    {
        // Take values from encounter constants
        encounterConstants = FindObjectOfType<EncounterConstants>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySoundEffect(SoundEffects effectID)
    {
        switch(effectID)
        {
            case SoundEffects.Bing:
                audioMenu.PlayOneShot(setBing);
                break;
            case SoundEffects.Good:
                audioMenu.PlayOneShot(setDing);
                break;
            case SoundEffects.MenuNext:
                audioMenu.PlayOneShot(menuNext);
                break;
            case SoundEffects.MoveSelect:
                audioMenu.PlayOneShot(moveSelect);
                break;
            case SoundEffects.PlayerSelect:
                audioMenu.PlayOneShot(playerSelect);
                break;
            case SoundEffects.Perfect:
                audioMenu.PlayOneShot(perfect);
                break;
            case SoundEffects.SetComplete:
                audioNotes.PlayOneShot(setComplete);
                break;
            case SoundEffects.SetFailure:
                audioNotes.PlayOneShot(setFailure);
                break;
            case SoundEffects.WrongPress:
                audioNotes.PlayOneShot(wrongPress);
                break;
        }
    }

    public void PlayCrowdEffect(CrowdEffects crowdEffectID)
    {
        switch(crowdEffectID)
        {
            case CrowdEffects.CrowdStart:
                audioCrowdStart.volume = 1;
                audioCrowdStart.Play();
                audioCrowdStart.DOFade(0f, encounterConstants.CrowdStartDuration).SetDelay(encounterConstants.CrowdStartDelay);
                break;
            case CrowdEffects.CrowdIdle:
                audioCrowdBG.volume = 0;
                audioCrowdBG.Play();
                audioCrowdBG.DOFade(1f, encounterConstants.CrowdSoundTransition);
                audioCrowdSet.DOFade(0f, encounterConstants.CrowdSoundTransition).OnComplete(() => {
                    audioCrowdSet.Stop();
                });
                break;
            case CrowdEffects.CrowdSet:
                audioCrowdSet.volume = 0;
                audioCrowdSet.Play();
                audioCrowdSet.DOFade(1f, encounterConstants.CrowdSoundTransition);
                audioCrowdBG.DOFade(0f, encounterConstants.CrowdSoundTransition).OnComplete(() => {
                    audioCrowdBG.Stop();
                });
                break;
        }
    }

    public void PlayMusicEffect(MusicEffects musicEffectID, int trackID)
    {
        switch (musicEffectID)
        {
            case MusicEffects.PlayerCorrect:
                // Expose parameter and call mixer DOSetFloat
                break;
            case MusicEffects.PlayerMiss:
                break;
            case MusicEffects.PlayerWrong:
                break;
        }
    }

    public void StartMusic(int songCount)
    {
        int index = 0;
        foreach (AudioSource playerSource in playerAudios)
        {
            playerSource.clip = musicClips.list[songCount].list[index];
            index++;
        }

        foreach (AudioSource playerSource in playerAudios)
        {
            playerSource.Play();
        }
    }
}
