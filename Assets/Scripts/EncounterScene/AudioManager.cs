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

    public AudioMixer audioMixer;

    public Sequence[] musicSequence = new Sequence[4];

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

    public void PlayMusicEffect(MusicEffects musicEffectID, int playerID)
    {
        string channelID = "Music" + (playerID + 1);

        //string distortParam = "Music" + (playerID + 1) + "Pitch";
        // Create MAke up gain parameter
        switch (musicEffectID)
        {
            case MusicEffects.PlayerCorrect:
                // Expose parameter and call mixer DOSetFloat
                break;
            case MusicEffects.PlayerMiss:
                string missParam = channelID + "Com";

                musicSequence[playerID].Complete();
                musicSequence[playerID] = DOTween.Sequence();

                musicSequence[playerID].Insert(0f,
                    audioMixer.DOSetFloat(missParam, encounterConstants.MusicCompressorMute, encounterConstants.MusicMissDelay/2));
                musicSequence[playerID].Insert(encounterConstants.MusicMissDelay,
                    audioMixer.DOSetFloat(missParam, encounterConstants.MusicCompressorNormal, encounterConstants.MusicMissDelay));
                break;
            case MusicEffects.PlayerWrong:
                string wrongParam = channelID + "Distort";

                musicSequence[playerID].Complete();
                musicSequence[playerID] = DOTween.Sequence();

                musicSequence[playerID].Insert(0f,
                    audioMixer.DOSetFloat(wrongParam, encounterConstants.MusicDistortWrong, encounterConstants.MusicWrongDelay));
                musicSequence[playerID].Insert(encounterConstants.MusicWrongDelay,
                    audioMixer.DOSetFloat(wrongParam, encounterConstants.MusicDistrotNormal, encounterConstants.MusicWrongDelay));
                break;
            case MusicEffects.PlayerSelect:
                for (int index = 0; index < 4; index++)
                {
                    string channelParam = "Music" + (index+1) + "Vol";

                    if (musicSequence[index] != null)
                    {
                        musicSequence[index].Complete();
                    }
                    musicSequence[index] = DOTween.Sequence();


                    if (index == playerID)
                    {
                        //audioMixer.SetFloat(volumeParam, encounterConstants.SelectedMusicVol);
                        // Increase Selected to selected Vol
                        musicSequence[index].Insert(0f, audioMixer.DOSetFloat(channelParam, encounterConstants.SelectedMusicVol, encounterConstants.MusicSelectDelay));
                    }
                    else
                    {
                        //audioMixer.SetFloat(volumeParam, encounterConstants.UnselectedMusicVol);
                        // Reduce Selected to selected Vol
                        musicSequence[index].Insert(0f, audioMixer.DOSetFloat(channelParam, encounterConstants.UnselectedMusicVol, encounterConstants.MusicSelectDelay));
                    }
                }
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
