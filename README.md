# CST320 Game Project

A Unity game project. The repository contains a complete Unity project including art assets, shaders, experimental scripts, and project configuration.

---

## Table of Contents

* [Overview](#overview)
* [Project Structure](#project-structure)
* [Getting Started](#getting-started)

  * [Prerequisites](#prerequisites)
  * [Cloning the Repository](#cloning-the-repository)
  * [Opening in Unity](#opening-in-unity)
* [Playing the Game](#playing-the-game)

  * [Basic Controls](#basic-controls)
  * [Game Objective](#game-objective)
* [Development Notes](#development-notes)
* [Assets & Credits](#assets--credits)
* [Contributing](#contributing)
* [License](#license)

---

## Overview

This repository is structured as a standard Unity project with:

* Core scenes, prefabs, scripts, and materials under the `Assets/` folder
* Experimental or prototype scripts under `Experiments/Scripts/`
* Third‑party or external 3D models under `KenneyModels/`
* Unity configuration and package metadata in `ProjectSettings/` and `Packages/`

The project uses a combination of **C#**, **ShaderLab**, and **HLSL** scripts, indicating custom gameplay logic, shaders, and visual effects.

---

## Project Structure

At the root of the repository you will find:

```text
GameProject/
├─ .vscode/             # Optional VS Code configuration
├─ Assets/              # Main Unity assets (scenes, scripts, prefabs, materials, etc.)
├─ Experiments/
│  └─ Scripts/          # Experimental / prototype scripts and tests
├─ KenneyModels/        # 3D models and resources from Kenney asset packs
├─ Packages/            # Unity package manifest and package cache
├─ ProjectSettings/     # Unity project and editor settings
├─ UIElementsSchema/    # UI schema / definitions for Unity UIElements
├─ .gitignore           # Git ignore rules
├─ .vsconfig            # Visual Studio configuration (optional)
└─ README.md            # Project documentation (this file)
```

> ℹ️ For more details about Unity project layout, see Unity's official documentation. The exact contents of each folder may evolve as the project grows.

---

## Getting Started

### Prerequisites

To open and run this project you will need:

* **Unity** (6.2 or newer is recommended)
* **Unity Hub** (for managing Unity editor versions)
* **Git** (optional, but recommended for cloning and version control)
* A C#‑friendly code editor such as **Visual Studio**, **Rider**, or **VS Code**

> ✅ You can find the exact Unity version used by checking the `ProjectSettings/ProjectVersion.txt` file inside the repository once cloned.

### Cloning the Repository

```bash
# Using HTTPS
git clone https://github.com/AQPublic/GameProject.git

# Or using SSH
# git clone git@github.com:AQPublic/GameProject.git

cd GameProject
```

### Opening in Unity

1. Open **Unity Hub**.
2. Click **Add** (or **Open**) and browse to the cloned `GameProject` folder.
3. Select the folder and let Unity Hub detect the project.
4. Choose an appropriate Unity editor version (ideally matching `ProjectVersion.txt`).
5. Open the project.

Once Unity finishes importing assets, you can open the main scene, `HomeSceneV5` (in `Assets/Scenes/`) and press **Play** in the editor.

---

## Playing the Game

### Basic Controls

This game uses the standard controls for a Meta Quest 3. All interaction is done through the controller.

While testing in the editor, the `R` key can be pressed to reset the game early at any time.

### Game Objective

The player is presented with a sandbox-style environment consisting of puzzles and various enemies. The
default spawn point is a small town, and the primary objective is to reach the boat without being attacked.

In the town are a few grabable objects, including a spear, which is the only available weapon at this time.
There is a small area to practice aiming and throwing the spear.

---

## Development Notes

Some tips and notes for developers working on this project:

* **Scripts**: Gameplay and experiment scripts are likely under `Assets/` and `Experiments/Scripts/`. Group related scripts into folders (e.g., `Player`, `Enemies`, `UI`) for maintainability.
* **Scenes**: Keep production scenes separate from experimental scenes. Consider having a `Scenes/Production` and `Scenes/Experiments` structure.
* **Prefabs**: Use prefabs for reusable objects (enemies, pickups, UI panels) and store them under `Assets/Prefabs/`.
* **Version Control**:

  * Commit frequently with meaningful commit messages.
  * Ignore build artifacts and library folders (Unity‑standard `.gitignore` is already included).
* **Experiments**: Use `Experiments/` for prototypes and testing ideas without breaking the main game. Once stable, promote them into the main `Assets/` hierarchy.

---

## Assets & Credits

This project includes 3D models and other assets from the `KenneyModels/` folder, which likely come from **Kenney** ([https://kenney.nl](https://kenney.nl)):

* Be sure to follow Kenney's licensing terms when distributing builds.
* If you add more third‑party assets, list them here with links and license notes.

---

## License

Anyone is free to use, modify, and redistribute this software for any purpose.

This software is provided "AS IS" without any warranty, and in no event will the authors be liable for any damages arising out of its use.
