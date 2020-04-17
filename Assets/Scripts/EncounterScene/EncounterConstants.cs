using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterConstants: MonoBehaviour
{
    [Header("Player Colours")]
    public Color[] PlayerColors =
    {
        Color.red,
        Color.blue,
        Color.magenta,
        Color.yellow
    };
    public Color[] FretColors =
    {
        Color.yellow,
        Color.blue,
        Color.red,
        Color.green
    };
    public Color[] MoveColors =
    {
        Color.cyan,
        Color.magenta,
        Color.red
    };

    public Color missedNoteColor = Color.red;
    public Color initialNoteColor = Color.white;

    public Color[] PlayerGradeColors =
    {
        Color.white,
        Color.cyan,
        Color.blue
    };

    public Color[] EnemyGradeColors =
    {
        Color.magenta,
        Color.red,
        Color.yellow
    };

    [Header("Move Names")]
    public string[][] MoveNames = new string[][] {
        new string[]{
            "Pitch",
            "Amplifier",
            "Solo"
        },
        new string[]{
            "Root",
            "Pickups",
            "Rythm"
        },
        new string[]{
            "Jazzy",
            "Rocky",
            "Stance"
        },
        new string[]{
            "MetalHead",
            "Stomp",
            "Morale"
        }
    };

    [Header("Move Descriptions")]
    public string[][] MoveDescriptions = new string[][] {
        new string[]{
            "This is the description for pitch",
            "This is the description for amplifier",
            "This is the description for solo"
        },
        new string[]{
            "This is the description for root",
            "This is the description for pickups",
            "This is the description for rythm"
        },
        new string[]{
            "This is the description for Jazzy",
            "This is the description for Rocky",
            "This is the description for Stance"
        },
        new string[]{
            "This is the description for MetalHead",
            "This is the description for Stomp",
            "This is the description for Morale"
        }
    };

    [Header("Score Values")]
    public float accuracyPerfect = 10;
    public float accuracyGood = 20;
    public float accuracyOK = 40;

    public int scorePerfect = 15;
    public int scoreGood = 10;
    public int scoreOK = 5;

    public int scoreWrongPunishment = 5;
    public int scoreMissPunishment = 30;

    public float maxScore = 700;

    public float enemyDifficulty = 0.0f;

    [Header("Game Values")]
    public int setLength = 2;
    public float crowdYPosition = 1.9f;
    public float crowdMoveProbability = 0.5f;

    public float NoteSpeed = 10f;
    public float MusicPlayDelay = 2f;

    public float CrowdStartDuration = 5f;
    public float CrowdStartDelay = 3f;

    public float CrowdSoundTransition = 2f;

    //[Header("Attack Values")]



    [Header("Camera Position And Rotation Variables")]
    public Vector3 startStageCamPos = new Vector3(-3.0f, 6.0f, 0.0f);
    public Vector3 startStageCamRot = new Vector3(30f, 90f, 0.0f);

    public Vector3 cameraTurnPos = new Vector3(2.5f, 20f, 0f);
    public Vector3 cameraTurnRot = new Vector3(45f, -90f, 0f);

    public Vector3 cameraFocusPos = new Vector3(-9.5f, 6f, 0f);
    public Vector3 cameraFocusOffset = new Vector3(0f, -3f, 0f);

    public Vector3 cameraEnemyPos = new Vector3(6f, 8.2f, 0f);
    public Vector3 cameraEnemyRot = new Vector3(30f, 90f, 0f);

    public Vector3 cameraOverviewPos = new Vector3(0f, 25f, -30f);
    public Vector3 cameraOverviewRot = new Vector3(45f, 0f, 0f);

    [Header("Knob movement positions")]
    public Vector3 KnobPlayPos = new Vector3(700f, 35f, 0f);
    public Vector3 KnobStartPos = new Vector3(600f, -250f, 0f);

    [Header("Other Values and Enums")]
    public float startLightIntensity = 18f;
    //public float focusLightIntensity = 0f;
    public float endLightIntensity = 0f;
    public Vector3 startLightRot = new Vector3(135f, 90f, 90f);

    public int notePoolSize = 30;
    public float SelectorDelay = 0.15f;

    public float repBarWidth = 1000;

    public enum SoundEffects
    {
        MoveSelect,
        PlayerSelect,
        MenuNext,
        Perfect,
        SetComplete,
        SetFailure,
        Good,
        Bing,
        WrongPress
    }
    public enum CrowdEffects
    {
        CrowdStart,
        CrowdSet,
        CrowdIdle
    }
    public enum MusicEffects
    {
        PlayerWrong,
        PlayerCorrect,
        PlayerMiss
    }

    // NoteManagerStates
    public enum NotesGameStates
    {
        Start,
        Intro,
        MoveSelect,
        Playing,
        EnemyIntro,
        Enemy,
        End,
        Idle
    }

    // GamePlayManagerStates
    public enum GameplayState
    {
        StartGame,
        StartGameUI,
        TurnIntro,
        TurnPlay,
        //EnemyIntro,
        //EnemyPlay,
        TurnPlayOut,
        EndGameUI,
        EndGame
    }

    public enum AccuracyRating
    {
        Perfect,
        Good,
        OK
    }
}
