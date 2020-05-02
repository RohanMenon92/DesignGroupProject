using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UI CClass Definitions
[System.Serializable]
public class UIDescripton
{
    public string descriptionText;
    public string acceptText;
    public string cancelText;
    public string selectText;
    public string specialText;

    public UIDescripton(string descriptionTextI, string acceptTextI, string cancelTextI, string selectTextI, string specialTextI = "")
    {
        descriptionText = descriptionTextI;
        acceptText = acceptTextI;
        cancelText = cancelTextI;
        selectText = selectTextI;
        specialText = specialTextI;
    }

}
// Move Definitions
[System.Serializable]
public class PlayerMove
{
    public string name;
    public string description;
    public float score;
    public float hypeRate;
    public int turnLock;
    public MoveEffects effect;
    public int currentLock;
    public Color moveColor;

    public PlayerMove(string nameI, string descriptionI, float scoreI, float hypeRateI, int turnLockI, MoveEffects effectI, Color colorI)
    {
        name = nameI;
        description = descriptionI;
        score = scoreI;
        hypeRate = hypeRateI;
        turnLock = turnLockI;
        effect = effectI;
        currentLock = 0;
        moveColor = colorI;
    }

    public void LockMove()
    {
        currentLock = turnLock;
    }

    public void DecrementLock()
    {
        if(currentLock > 0)
        {
            currentLock--;
        }
    }

    public bool IsUnlocked()
    {
        return currentLock == 0;
    }

};

// Enums
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
    WrongPress,
    MenuSelect,
    Hyped,
    Amplify,
    CrazyStand,
    Rhythm,
    Stomp
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
    PlayerMiss,
    PlayerSelect
}

public enum MoveEffects
{
    None,
    Amplifier, // Boost Crowd Interest for 1 turn
    Rhythm, // Reduce opponents gain of crowd inerest by 50%
    Stomp, // Prevent next enemy from playing
    CrazyStand // Add Random Notes at certain parts
    //Solo, // Play Solo, allow large increment to crowd interest
    //Pickups, // Boost Accuracy, Ok notes become good notes etc
    //Morale, // Invincibility for 1 set to crowd interest
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
    EndSet,
    Idle,
    Hyped
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

public class EncounterConstants: MonoBehaviour
{
    [Header("Player Colours")]
    public Color[] PlayerColors =
    {
        new Color(255, 0, 2, 255),
        new Color(0, 65, 255, 255),
        new Color(255, 4, 217, 255),
        new Color(53, 229, 88, 255)
    };
    public Color[] FretColors =
    {
        Color.yellow,
        Color.blue,
        Color.red,
        Color.green
    };
    //public Color[] MoveColors =
    //{
    //    Color.cyan,
    //    Color.magenta,
    //    Color.red
    //};

    public Color missedNoteColor = Color.red;
    public Color initialNoteColor = Color.white;

    public Color[] PlayerGradeColors =
    {
        new Color(226, 182, 42),
        new Color(0, 110, 255),
        new Color(35, 238, 88)
    };

    public Color[] EnemyGradeColors =
    {
        new Color(212, 0, 255),
        new Color(255, 26, 0),
        new Color(253, 255, 4)
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
    public float enemyMissThreshold = 90f;

    [Header("Game Values")]
    public int setLength = 4;
    public int totalSets = 3;
    public float crowdYPosition = 1.9f;
    public float crowdMoveProbability = 0.5f;

    public float NoteSpeed = 6f;
    public float MusicPlayDelay = 2f;

    public float CrowdStartDuration = 5f;
    public float CrowdStartDelay = 3f;

    public float CrowdSoundTransition = 2f;
    public int HypeValueMax = 100;
    public int HypeValueStart = 25;

    [Header("UI Lists")]
    public List<UIDescripton> UITextSets = new List<UIDescripton> {
        new UIDescripton("Lets get ready to RUMBLE!!", "Accept!", "", ""),
        new UIDescripton("Lets go!", "Select Instrument", "", ""),
        new UIDescripton("Select an Instrument", "Ready", "", "Select Band Member"),
        new UIDescripton("Select a Move", "Ready", "Select Instrument", "Select Move"),
        new UIDescripton("Lets go!", "", "", ""),
        new UIDescripton("It's their turn now", "", "", ""),
        new UIDescripton("The crowds really into our music.", "", "", ""),
        new UIDescripton("The crowds likes their music better.", "", "", ""),
        new UIDescripton("LETS GET HYPED!", "Skip HYPE", "", "Select Band Member", "Start HYPE Move"),
        new UIDescripton("YEAH! We've won!", "Yeah!", "Lets Do better", "", ""),
        new UIDescripton("Oh no! They've won!", "Lets try again!", "", "", ""),
    };


    [Header("Move Lists")]
    public PlayerMove[] GuitarMoves = new PlayerMove[]
    {
        new PlayerMove("Pitch", "Play an attack based verse.", 1.5f,2,0, MoveEffects.None, new Color(255, 95, 0)),
        new PlayerMove("Amplifier", "Play a verse that boosts Crowd Interest gain for the team for the turn.", 0.8f,3,2, MoveEffects.Amplifier, new Color(217, 0, 91)),
        //new PlayerMove("Solo", "Play a stronger attack verse. Plays faster than usual.", 0.7f,5,3, MoveEffects.Solo)
    };

    public PlayerMove[] BassMoves = new PlayerMove[]
    {
        new PlayerMove("Root", "Play an attack based verse.", 1.5f,2,0, MoveEffects.None, new Color(87, 0, 255)),
        //new PlayerMove("Pickups", "Play a verse that boosts the team's accuracy for 1 turn.", 0.7f,3,4, MoveEffects.Pickups),
        new PlayerMove("Rhythm", "Play a verse that reduces opponent Crowd Interest gain for 2 turns.", 0.6f,5,4, MoveEffects.Rhythm, new Color(0, 255, 236))
    };

    public PlayerMove[] KeytarMoves = new PlayerMove[]
    {
        new PlayerMove("Jazzy", "Play an attack based verse.", 1.5f,3,0, MoveEffects.None, new Color(255, 4, 84)),
        //new PlayerMove("Rocky", "Play a stronger attack based verse.", 2,5,2, MoveEffects.None),
        new PlayerMove("Crazy Stand", "Play a devastating randomized verse of fast notes.", 1,12,4, MoveEffects.CrazyStand, new Color(150, 21, 255))
    };

    public PlayerMove[] DrumMoves = new PlayerMove[]
    {
        new PlayerMove("MetalHead", "Play an attack based verse.", 1.5f,2,0, MoveEffects.None, new Color(145, 244, 56)),
        new PlayerMove("Stomp", "Play a verse that skips the next opponent's turn.", 0.8f,3,3, MoveEffects.Stomp, new Color(24, 153, 18)),
        //new PlayerMove("Morale", "Play a verse that negates all crowd interest gain for both teams for 1 turn.", 1f,3,1, MoveEffects.Morale)
    };
    public float amplifierMultiplier = 1.5f;
    public float rhythmMultiplier = 0.8f;
    public float CrazyStandNoteDelay = 2.0f;

    [Header("Camera Position And Rotation Variables")]
    public Vector3 startcamera = new Vector3(2, -0.5f, 6);

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

    [Header("UI Values")]
    public Color HypeLight = Color.white;
    public float KnobPlayScale = 1.5f;
    public Vector3 KnobPlayPos = new Vector3(650f, 75f, 0f);
    public Vector3 KnobStartPos = new Vector3(600f, -250f, 0f);
    public float SelectorDelay = 0.15f;

    public float knobSelectScale = 1.25f;
    public float knobDeselectScale = 0.8f;

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

    public float moveLockedScale = 0.6f;
    public float moveLockScale = 0.7f;
    public float moveSelectedScale = 1.2f;
    public Color moveLockedColour = Color.red;
    public Color moveLockColor = Color.gray;
    public Color moveLockTextColor = Color.black;
    public Color moveUnlockedTextColour = Color.white;
    public float descriptionTransition = 1.5f;

    public float moveEffectDuration = 0.5f;
    public Color effectAnimateColor = Color.blue;
    public float moveAnimateDuration = 0.2f;
    public Vector2 descriptionOffset = new Vector2(-150, 60);

    [Header("Audio Control Values")]
    public float MusicCompressorMute = -60f;
    public float MusicCompressorNormal = 0.0f;
    public float MusicMissDelay = 0.4f;

    public float MusicDistortWrong = 0.8f;
    public float MusicDistrotNormal = 0.0f;
    public float MusicWrongDelay = 0.1f;

    public float SelectedMusicVol = 3f;
    public float UnselectedMusicVol = -5f;

    public float MusicSelectDelay = 0.3f;

    [Header("Other Values and Enums")]
    public float startLightIntensity = 30f;
    //public float focusLightIntensity = 0f;
    public float endLightIntensity = 0f;
    public Vector3 startLightRot = new Vector3(35f, 90f, 90f);
    public float spotLightFocus = 45f;
    public float spotLightGeneral = 90f;

    public int notePoolSize = 30;
    public Vector3 firstKnobRotation = new Vector3(0f, 0f, -27.575f);

    [Header("UI")]
    public float showCountDuration = 2f;
    public float showCountBounce = 1.3f;

    public string SetCountText = "Set Count: {0}/{1}";
    public string SongCountText = "Song Count: {0}/{1}";

    public float textFadeDuration = 1.0f;

    public float startUIFadeDuration = 5.0f;
    public float startGameUIFadeDuration = 3.0f;
    public float introUIFadeDuration = 2.0f;

    //public float IntroUIFadeDuration = 3.0f;


}
