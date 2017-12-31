# MazeFighters

-Description

A multithreaded, self-sufficient application. Fully coded in C#.
The text file, named shapeOfYou.txt, generates a maze where the application operates (you can modify).
In the text file, 0 stands for empty spot, 1 for wall, 2 for exit, 3 for loot, and 4 for a player.
The app file can also be launched from console, and it accepts up to 6 arguments (refer to Program class).
The user can set the game speed, inner voice (see below), fighter count aka AI count.

-Functionalities

There are several loot boxes with objects in the maze (around 10% of the total free space in the maze), these objects can be equipped to boost DPS stats by a random number [1, 10]. Upon picking up a loot box, an AI has a chance on going offensive.
Equipped objects expire and are redistributed throughout the maze’s empty spots every time the innerVoice ticks. When AIs lose their objects they have a chance on going defensive.
Several fighters are 'parachuted' inside the maze. These fighter AIs have 100hp, 10 base dmg and are either offensive or defensive (more chance on offensive for more entertainment).
These fighters try to exit the maze but may also be willing to fight (if offensive).
The game ends when a fighter escapes the maze.
The fighters move on intelligence (personal algorithm).
They remember where they've been and update their paths so that when they back-track they rearrange their paths (see GameManager class' WatchStep method, 2nd part).

-Bugs

Sometimes passing through enemies without damaging them.
When starting to backtrack, disappearing for 1 frame worth of time.
Ants, bees and beetles.
