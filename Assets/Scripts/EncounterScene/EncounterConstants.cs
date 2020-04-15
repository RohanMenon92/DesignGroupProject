using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EncounterConstants
{
    public static Color[] PlayerColors =
    {
        Color.red,
        Color.blue,
        Color.magenta,
        Color.yellow
    };

    public static Vector3 startStageCamPos = new Vector3(-3.0f, 6.0f, 0.0f);
    public static Vector3 startStageCamRot = new Vector3(30f, 90f, 0.0f);

    public static float startLightIntensity = 18f;
    //public static float focusLightIntensity = 0f;
    public static float endLightIntensity = 0f;

    public static Vector3 KnobPlayPos = new Vector3(700f, 35f, 0f);
    public static Vector3 KnobStartPos = new Vector3(600f, -250f, 0f);

    public static Color[] FretColors =
    {
        Color.yellow,
        Color.blue,
        Color.red,
        Color.green
    };

    public static Color[] KnobColors =
    {
        Color.cyan,
        Color.magenta,
        Color.red
    };

    public static string[][] MoveDescriptions = new string[][] {
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

    public static string[][] MoveNames = new string[][] {
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
    public enum GameplayState {
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

    public static int NotePoolSize = 30;
    public static float SelectorDelay = 0.15f;

    public static Vector3 cameraTurnPos = new Vector3(2.5f, 20f, 0f);
    public static Vector3 cameraTurnRot = new Vector3(45f, -90f, 0f);

    public static Vector3 cameraFocusPos = new Vector3(-9.5f, 6f, 0f);
    public static Vector3 cameraFocusOffset = new Vector3(0f, -2f, 0f);

    public static Vector3 cameraEnemyPos = new Vector3(6f, 8.2f, 0f);
    public static Vector3 cameraEnemyRot = new Vector3(30f, 90f, 0f);

    public static Vector3 cameraOverviewPos = new Vector3(0f, 25f, -30f);
    public static Vector3 cameraOverviewRot = new Vector3(45f, 0f, 0f);

    public enum AccuracyRating
    {
        Perfect,
        Good,
        OK
    }

    public static float accuracyPerfect = 10;
    public static float accuracyGood = 20;
    public static float accuracyOK = 40;

    public static Color colorPerfect = Color.white;
    public static Color colorGood = Color.cyan;
    public static Color colorOK = Color.blue;

    public static int scorePerfect = 15;
    public static int scoreGood = 10;
    public static int scoreOK = 5;

    public static float repBarWidth = 1000;
    public static float maxScore = 700;
    internal static Vector3 startLightRot = new Vector3(135f, 90f, 90f);

    public static float enemyDifficulty = 0.0f;
    public static int setLength = 2;

    public static float crowdYPosition = 1.9f;
    public static float crowdMoveProbability = 0.5f;
}
