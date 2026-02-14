# BlindfoldProtocol

A Unity3D project using ECS (Entity Component System / DOTS) with a fully programmatic structure.

## Project Overview

This is a basic Unity game featuring:
- **Top-down camera view** of the game world
- **Procedurally generated green meadow** (10x10 plane)
- **Single red cube** spawned at the center (0, 0.5, 0)
- **Mouse selection** - Left-click on the cube to select it (shows yellow bounding box)
- **Right-click movement** - Right-click on the ground to move the selected cube to that position
- **ECS/DOTS architecture** - All game logic uses Unity's Entity Component System

## Project Structure

```
BlindfoldProtocol/
├── Assets/
│   ├── MainScene.unity          # Main game scene with camera and lighting
│   └── Scripts/
│       ├── Components.cs        # ECS component definitions
│       ├── SpawnSystem.cs       # System that spawns meadow and cube
│       ├── SelectionSystem.cs   # System that handles mouse selection
│       ├── MovementSystem.cs    # System that handles movement commands
│       └── GameObjectBridge.cs  # Bridge between ECS and GameObject rendering
├── Packages/
│   └── manifest.json            # Package dependencies (ECS/DOTS packages)
└── ProjectSettings/
    ├── ProjectVersion.txt       # Unity version info
    └── ProjectSettings.asset    # Project configuration
```

## ECS Architecture

### Components
- **CubeTag**: Tag component for the player cube entity
- **MeadowTag**: Tag component for the meadow entity
- **Selected**: Tag component indicating an entity is selected
- **MoveTo**: Component with target position and speed for movement

### Systems
- **SpawnSystem**: Runs once at initialization to create the meadow and cube entities
- **SelectionSystem**: Handles left-click input for selecting the cube
- **MovementInputSystem**: Handles right-click input to command cube movement and updates positions

### GameObject Bridge
Since Unity ECS doesn't directly handle rendering in all cases, the `GameObjectBridge` MonoBehaviour:
- Creates visual GameObjects (cube and meadow meshes)
- Syncs positions from ECS entities to GameObjects
- Renders the selection box using a LineRenderer

## How to Use

1. **Open in Unity**: Open the project in Unity 2022.3.10f1 or compatible version
2. **Play the scene**: Open `Assets/MainScene.unity` and press Play
3. **Select the cube**: Left-click on the red cube to select it (yellow box appears)
4. **Move the cube**: Right-click on the meadow to move the cube to that location

## Key Features

- **Fully Programmatic**: All entities and visuals are created through code, no editor setup required
- **ECS/DOTS**: Uses Unity's modern Entity Component System for better performance
- **Dynamic Content**: Everything is generated at runtime
- **Clean Architecture**: Separation of concerns between components, systems, and rendering

## Technical Details

- **Unity Version**: 2022.3.10f1
- **Rendering**: Uses Universal Render Pipeline (URP) shaders
- **Input**: Classic Unity Input system for mouse controls
- **Camera**: Orthogonal top-down view at position (0, 15, 0) looking down
- **Movement Speed**: 5 units per second
- **Cube Height**: Maintained at 0.5 units above ground during movement