Overview

Code-first Unity app with no scene-authoring dependency at runtime: app bootstraps itself, shows Menu first, then transitions to Game.
Game uses DOTS/Entities Graphics to spawn and render a green ground + cube from code, plus top-down orthographic camera.
Structure

App entry and screen orchestration: AppEntryPoint.cs
Menu renderer (single Start button, Input System UI): MenuScreenRenderer.cs
Replaceable game renderer implementation (DOTS): DotsGameScreenRenderer.cs
Camera pan/zoom controls (WASD/arrows + wheel): GameCameraController.cs
Old bootstrap is intentionally inert: RuntimeDotsBootstrap.cs
Approach

Interface-based screen rendering: game/menu are swappable implementations.
Entry point owns transitions (Menu -> Start -> Game).
Game renderer creates/destroys tagged ECS entities on Show/Hide and configures camera in code.
Running

Script diagnostics: VS Code Problems + get compile/runtime traces from Unity Editor log if Console is unclear.
Most common setup pitfalls already handled: Input System UI module mismatch and nullable EntityManager misuse.