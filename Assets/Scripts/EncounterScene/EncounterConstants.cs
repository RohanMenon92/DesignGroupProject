using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EncounterConstants
{
    public static Color[] FretColors =
    {
        Color.green,
        Color.red,
        Color.blue,
        Color.yellow
    };

    public static Color[] KnobColors =
    {
        Color.grey,
        Color.magenta,
        Color.green,
        Color.blue
    };

    public enum GameplayState {
        StartGame,
        StartGameUI,
        TurnIntro,
        TurnPlay,
        EnemyIntro,
        EnemyPlay,
        TurnPlayOut,
        EndGameUI,
        EndGame
    }

    public static int NotePoolSize = 30;
    public static float SelectorDelay = 1f;

    public static Vector3 cameraTurnPos = new Vector3(-7f, 6f, -15f);
    public static Vector3 cameraTurnRot = new Vector3(30f, -60f, 0f);

    public static Vector3 cameraEnemyPos = new Vector3(7f, 6f, -15f);
    public static Vector3 cameraEnemyRot = new Vector3(30f, 60f, 0f);

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

}
