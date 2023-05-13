# AirRace

This project is a collaborative work finished together with Lupei ([GitHub](https://github.com/pei-lu), [bilibili](https://space.bilibili.com/60606254?spm_id_from=333.337.0.0)). It has also been published on itch.io: https://ranmeraklis.itch.io/vr-air-racing. Hope you will like and enjoy it :D

Thank you so much!!!

## 1. Race Track

- When loading the campus model, apply a scale factor of 0.0254 and select “Generate Colliders”

- The `TrackGenerator.cs` is used to generate the race track. It is attached to a GameObject named Track and includes the following steps: 

  - ParseFile(); read the positions of waypoints from the provided file
  - GenerateWaypoints(); initialize the waypoint markers
  - GeneratePath(); generate the bezier path using waypoints as its control points
  - GenerateRoadSigns(); generate the arrows that show the way
  - InitPlayer(); initiate the player at the first waypoint, heading towards the second waypoint

- The waypoints are designed with a unique style:

  ![waypoint](waypoint.gif)

  It mainly has three parts. The top is a dragon resting on a spinning ancient ring, roaring from time to time. The bottom is a spinning sci-fi station. The middle is a glowing transparent sphere with a 3D waypoint ID in it. Also, it has the sound effect of dragon roar.

## 2. Flight Controls

### 2.1 Left Hand Moving 

1. Holding fist while keeping the palm pointing forward to move forward.

2. Turning your palm to face yourself will enable you to move backward.

3. The speed is controlled by the distance of thumb and index finger (the smaller the faster, so if you clench your fist really hard, your speed will be very large)

4. Sound effect is added, the engine sound will become louder as the player moves faster

### 2.2 Right Hand Turning

1. Using right hand pinch to activate the turning function.

2. While maintaining the pinch pose, moving right hand would map player's hand movement to their rotation.

### 2.3 View Switching

Clap to change view!

## 3. Wayfinding 

Visual ways of helping the player to know where to go next include the following:

1. The waypoint. It has a unique artistic graphic design, animations, neon light effect, and a number in the center telling the current progress. [world coordinates]

2. There are road signs along the path (floating arrows with neon effect). [world coordinates]

3. There is an arrow following the player and pointing to the next waypoint. [head coordinates]

   `ArrowGuide.cs` attached to the camera is used for the guiding arrow which is a child of the camera, follows the gaze and points to the next waypoint.

## 4. Motion sickness mitigation

The aircraft has three cameras that allow the player to toggle between. Cameras can be switched by clapping hands.

1. Nose view: the camera is placed at the nose of the plane, nearly with no visuals.
2. Cockpit view: the camera is placed in the cockpit (as in the picture below).
3. Tail view: the camera is placed at the tail of the airplane so that the player can see the whole of it.

![](https://i.imgur.com/mis5Jwz.png)

## 5. Gameplay
- Start game:
  
  The player's aircraft starts at the first waypoint, heading towards the second waypoint. Use your left-hand pinch gesture to start the game.
  
  - When the game hasn’t started yet, use left hand to pinch would start a 3-second countdown.
  - After the countdown turns to 0, the timer will start and the player will be able to move.
  
- Timer system:
  
  1. main timer
  
     1. countdown timer
  
        Before the countdown (3s) ends the aircraft can't move
  
     2. regular timer
  
        Once the countdown is done, the regular timer starts running. When the last waypoint has been reached, the race is over and the timer is halted
  
  2. freeze timer
  
     This is a countdown timer (3s) after the aircraft collides with the ground, a building, or the wrong waypoint
  
- Collision Manager (should be a component of the cameras)
  
  1. collide with a road sign: deactivate the road sign
  2. Reach (collide with) the right waypoint in order: visual and sound effect shows up; save the waypoint’s position in case the aircraft needs to be teleported back (see below); delete the waypoint in the Waypoints Container and deactivate the glowing sphere in the scene; delete the road signs (arrows glowing and floating up and down) from the start to the last reached waypoint
  3. Collide with the wrong waypoint: do nothing
  4. Collide with the ground or a building: the aircraft will be teleported back to the last successfully cleared waypoint (the GameObject has been deactivated but its position is saved), heading towards the next waypoint; upon collision, the player won’t be able to move within 3 seconds which will be visualized with the freeze timer. In the meanwhile, the main timer is still running.
  5. Collision Detection: a waypoint is reached once the aircraft's origin gets within a fixed radius of 30 feet (9.144m) around the waypoint.
  
  ![](https://i.imgur.com/MeqBja5.png)
  

