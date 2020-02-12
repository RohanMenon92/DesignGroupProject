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
        StartGameUI,
        TurnIntro,
        TurnPlay,
        TurnPlayOut,
        EndGameUI
    }

    public static int NotePoolSize = 30;
    public static float SelectorDelay = 1f;
}
