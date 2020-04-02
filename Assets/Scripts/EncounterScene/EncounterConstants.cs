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


    public static Color[] FretColors =
    {
        Color.yellow,
        Color.blue,
        Color.red,
        Color.green
    };

    public static Color[] KnobColors =
    {
        Color.grey,
        Color.magenta,
        Color.green,
        Color.blue
    };

    // NoteManagerStates
    public enum NotesGameStates
    {
        Start,
        Intro,
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
    public static float crowdMoveThreshold = 0.5f;
    public static float crowdMoveProbability = 0.8f;
}
