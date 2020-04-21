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

    // EXAMLE CODE
    //public people[] person;

    [System.Serializable]
    public struct AttackMove
    {
        public string name;
        public string description;
        public float score;
        public float hypeRate;
        public int turnLock;

        int currentLock;

        public AttackMove(string nameI, string descriptionI, float scoreI, float hypeRateI, int turnLockI)
        {
            name = nameI;
            description = descriptionI;
            score = scoreI;
            hypeRate = hypeRateI;
            turnLock = turnLockI;
            currentLock = 0;
        }
    };

    [Header("Score Values")]
    public float accuracyPerfect = 10;
    public float accuracyGood = 20;
    public float accuracyOK = 40;

    public int scorePerfectMultiplier = 15;
    public int scoreGoodMultiplier = 10;
    public int scoreOKMultiplier = 5;

    public int scoreWrongPunishment = 5;
    public int scoreMissPunishment = 30;

    public int hypeWrongPunishment = 1;
    public int hypeMissPunishment = 5;

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
    public int HypeValueMax = 100;
    public int HypeValueStart = 25;

    [Header("Move Lists")]
    public AttackMove[] GuitarMoves = new AttackMove[]
    {
        new AttackMove("Pitch", "This is the description for pitch", 1.5f,2,0),
        new AttackMove("Amplifier", "This is the description for amplifier", 0.8f,3,2),
        new AttackMove("Solo", "This is the description for solo", 0.7f,5,3)
    };

    public AttackMove[] BassMoves = new AttackMove[]
    {
        new AttackMove("Root", "This is the description for root", 1.5f,2,0),
        new AttackMove("Pickups", "This is the description for pickups", 0.7f,3,4),
        new AttackMove("Rythm", "This is the description for rythm", 0.6f,5,4)
    };

    public AttackMove[] KeytarMoves = new AttackMove[]
    {
        new AttackMove("Jazzy", "This is the description for jazzy", 1.5f,3,0),
        new AttackMove("Rocky", "This is the description for rocky", 2,5,2),
        new AttackMove("Stance", "This is the description for stance", 2,12,4)
    };

    public AttackMove[] DrumMoves = new AttackMove[]
    {
        new AttackMove("MetalHead", "This is the description for metalhead", 1.5f,2,0),
        new AttackMove("Stomp", "This is the description for stomp", 0.8f,3,3),
        new AttackMove("Morale", "This is the description for morale", 1f,3,1)
    };


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

    public float CannotHypeScale = 0.8f;
    public float BaseHypeScale = 1.0f;
    public float CanHypeScale = 1.1f;
    public float IsHypeScale = 1.3f;

    public float CannotHypeDuration = 0.3f;
    public float HypeIncrementDuration = 0.3f;
    public float CanHypeDuration = 0.5f;
    public float IsHypedDuration = 1.0f;

    public Color CannotHypeColor = Color.red;
    public Color BaseHypeColor = Color.blue;
    public Color CanHypeColor = Color.cyan;
    public Color IsHypeColor = Color.green;

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
