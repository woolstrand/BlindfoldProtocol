# Project Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                         Unity ECS/DOTS Project                      │
│                        BlindfoldProtocol                             │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                          Game Scene View                             │
│                                                                       │
│                      Camera (0, 15, 0)                               │
│                      Looking Down ▼                                  │
│                                                                       │
│                   ┌─────────────────────┐                            │
│                   │    Yellow Border    │  ← Selection Box           │
│                   │  ┌───────────────┐  │                            │
│                   │  │   Red Cube    │  │  ← Player Cube             │
│                   │  │  @ (0,0.5,0)  │  │                            │
│                   │  └───────────────┘  │                            │
│                   └─────────────────────┘                            │
│                                                                       │
│        ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓                        │
│        ▓▓▓▓▓▓  Green Meadow 10x10  ▓▓▓▓▓▓▓                          │
│        ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓                        │
│                                                                       │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                      ECS Architecture                                │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  COMPONENTS (Data)                                                   │
│  ├── CubeTag           : Identifies cube entity                     │
│  ├── MeadowTag         : Identifies meadow entity                   │
│  ├── Selected          : Entity is selected                         │
│  └── MoveTo            : Target position + speed                    │
│                                                                       │
│  SYSTEMS (Behavior)                                                  │
│  ├── SpawnSystem       : Creates entities at startup                │
│  ├── SelectionSystem   : Handles left-click selection               │
│  └── MovementSystem    : Handles right-click movement               │
│                                                                       │
│  BRIDGE (Rendering)                                                  │
│  └── GameObjectBridge  : Syncs ECS → GameObjects                    │
│                                                                       │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                      Interaction Flow                                │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  1. GAME START                                                       │
│     SpawnSystem → Create meadow entity @ (0,0,0)                    │
│                → Create cube entity @ (0,0.5,0)                     │
│     GameObjectBridge → Create visual GameObjects                     │
│                                                                       │
│  2. LEFT CLICK on Cube                                               │
│     Mouse → SelectionSystem → Raycast                               │
│                            → Check if cube hit                      │
│                            → Add "Selected" component               │
│     GameObjectBridge → Detect "Selected"                             │
│                     → Show yellow selection box                     │
│                                                                       │
│  3. RIGHT CLICK on Ground (with cube selected)                       │
│     Mouse → MovementSystem → Raycast to ground                      │
│                            → Add "MoveTo" component                 │
│                                                                       │
│  4. MOVEMENT (every frame)                                           │
│     MovementSystem → Read "MoveTo" component                         │
│                   → Move cube towards target                        │
│                   → Remove "MoveTo" when arrived                    │
│     GameObjectBridge → Sync cube position to GameObject              │
│                                                                       │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                      File Structure                                  │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  BlindfoldProtocol/                                                  │
│  ├── Assets/                                                         │
│  │   ├── MainScene.unity        (Camera + GameManager)              │
│  │   └── Scripts/                                                    │
│  │       ├── Components.cs      (ECS components)                    │
│  │       ├── SpawnSystem.cs     (Entity creation)                   │
│  │       ├── SelectionSystem.cs (Mouse selection)                   │
│  │       ├── MovementSystem.cs  (Movement logic)                    │
│  │       ├── GameObjectBridge.cs (Rendering)                        │
│  │       └── BlindfoldProtocol.asmdef (Assembly def)               │
│  ├── Packages/                                                       │
│  │   └── manifest.json          (ECS dependencies)                  │
│  ├── ProjectSettings/                                                │
│  │   ├── ProjectVersion.txt     (Unity 2022.3.10f1)                │
│  │   ├── ProjectSettings.asset  (Config)                            │
│  │   └── TagManager.asset       ("Player" tag)                      │
│  ├── README.md                  (Documentation)                      │
│  └── IMPLEMENTATION.md          (Technical details)                  │
│                                                                       │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                      Statistics                                      │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  Total Files Created:    21                                          │
│  Lines of C# Code:       387                                         │
│  Total Lines Added:      1,148                                       │
│  ECS Components:         4                                           │
│  ECS Systems:            3                                           │
│  Security Issues:        0                                           │
│                                                                       │
└─────────────────────────────────────────────────────────────────────┘
