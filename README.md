## Concept
A JRPG where bands play(fight) against each other using different types of music and moves such asAmplifier, Stomp and Crazy Stand. Your band consists of 4 members(max) trying their best to win battlebands and get a record deal. Inspiration for the mini game was taken from Scott Pilgrim vs the World.

Video link: ​https://youtu.be/I31cKd9qFGo

## Gameplay
- The player can select an instrument (From guitar, bass, keytar or drums) and then a corresponding move for each player.
- Some moves are purely to gain additional crowd interest while some moves help enable special effects like disabling the next band’s turn, getting bonus moves or adding to the “Hype!” meter. 

Try to get better crowd interest and beat the enemy band. The mini game is about precision and hitting the notes at the correct time to get OK, GOOD and PERFECT scores with corresponding crowd interest and scores. And also about using the correct moves in a strategic manner.

Not playing correctly (Pressing random buttons or missing notes) reduces crown interest.
- Missing notes will tune out the sound for the selected instrument temporarily.
- Pressing random/incorrect buttons will distort the sound of the selected instrument and also reduce hype meter slightly.

The “Hype!” meter being full allows you to use an attack during player selection that doesn’t count as an additional move and allows you to play 2 songs together. 

After the player is done, the enemy team will perform based on a difficulty parameter decided in the scene.

The crowd moves after every song set is completed, based on the gathered crowd intrest of each team.

## Design Tools
- EncounterConstants
- Move Lists
- Encounter Manager, Note Game Manager
- Song Note Generator Timeline
- Audio Manager
- UI Manager

## Controls
- E in the initial scene to enter a band battle
- Select instrument/move: Left joystick or “W/S” and “Up/Down”
- Button prompt A = “Space” on PC
- Button prompt B = “ESC”
- The note collectors are controlled by 1,2,3,4 on PC and A,B,X,Y on your controller.
- To play HYPE Move, press “Left Cntrl” on PC and L2+ R2 on controller

## Screenshots
![ExplorationScene](https://github.com/RohanMenon92/DesignGroupProject/blob/master/Screenshots/ExplorationScene.PNG)
![MoveSelect](https://github.com/RohanMenon92/DesignGroupProject/blob/master/Screenshots/MoveSelect.PNG)
![Player Gameplay](https://github.com/RohanMenon92/DesignGroupProject/blob/master/Screenshots/PlayerGameplay.PNG)
![Enemy Gameplay](https://github.com/RohanMenon92/DesignGroupProject/blob/master/Screenshots/EnemyPlay.PNG)

## Design Tools
![NoteGeneratorManager](https://github.com/RohanMenon92/DesignGroupProject/blob/master/Screenshots/NoteGeneratorManager.PNG)
![Encounter Constants](https://github.com/RohanMenon92/DesignGroupProject/blob/master/Screenshots/EncounterConstants.PNG)
![MoveList](https://github.com/RohanMenon92/DesignGroupProject/blob/master/Screenshots/MoveLists.PNG)
