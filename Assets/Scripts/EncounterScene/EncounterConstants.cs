using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EncounterConstants
{
    public static Color[] FretColors =
    {
        Color.yellow,
        Color.red,
        Color.green,
        Color.blue
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
        TurnPlayOut,
        EndGameUI,
        EndGame
    }

    public static int NotePoolSize = 30;
    public static float SelectorDelay = 1f;

    public static Vector3 cameraTurnPos = new Vector3(-7f, 6f, -15f);
    public static Vector3 cameraTurnRot = new Vector3(30f, -60f, 0f);

    public static Vector3 cameraOverviewPos = new Vector3(0f, 25f, -30f);
    public static Vector3 cameraOverviewRot = new Vector3(45f, 0f, 0f);
}
