# 🌲 Stand Tall

> **Protect the cypress. Stand tall.**

**Stand Tall** is a strategic Tower Defense game developed by **Jolly Panda Studio** for **PC and Android**.

In the game, players must defend a symbolic **cypress tree** against waves of enemies. Build and manage your defenses, stop enemies from reaching the tree, and survive increasingly challenging waves.

---

## 🎮 Game Overview

The cypress stands at the center of the battlefield.

Enemies approach in waves, becoming stronger and more challenging as the game progresses. Your goal is to build an effective defense and protect the cypress for as long as possible.

The game is built around the idea of **standing strong under pressure**.

> **Protect the cypress. Stand tall.**

---

## ✨ Features

* 🌲 Cypress-based defense objective
* 🏰 Tower Defense gameplay
* 🌊 Wave-based enemy system
* 👾 Multiple enemy types
* 📈 Progressive difficulty
* 🎯 Strategic defense placement
* 🗺️ Level-based progression
* 💾 Save & progression system
* 🌍 Persian & English localization
* 🔊 Centralized audio system
* 📱 Android support
* 🖥️ PC support

---

## 🕹️ Gameplay

The core gameplay loop consists of:

1. **Prepare your defenses**
2. **Start an enemy wave**
3. **Defend the cypress**
4. **Manage your towers and resources**
5. **Survive the wave**
6. **Upgrade and prepare for what comes next**

As the player progresses, enemies become stronger and new challenges are introduced.

---

## 🌲 The Cypress

The central cypress tree represents **freedom, resilience, and standing tall in the face of adversity**.

Rather than simply defending a traditional objective such as a flag or base, the game places the cypress at the heart of its identity and gameplay.

---

## 🧩 Technical Architecture

Stand Tall is developed using **Unity** and follows a modular and data-driven architecture.

### Wave System

Enemy waves are generated through the custom:

`WaveDataGenerator`

This system allows enemy compositions, quantities, and progression to be configured without hardcoding individual waves.

### Climbing System

The battlefield uses a custom:

`RopeGrower`

system to manage the dynamically growing climbing path used by enemies.

This allows the battlefield and enemy progression to scale with level design.

---

## 👾 Enemy System

The game is designed to support approximately **10 different enemy types**.

Each enemy type can introduce different:

* Movement characteristics
* Health values
* Speeds
* Behaviors
* Defensive challenges

The enemy system is designed to remain extensible so new enemy types can be added without heavily modifying existing gameplay systems.

---

## 🗺️ Level Progression

The current design targets approximately:

**50 Levels**

Levels progressively introduce:

* Stronger enemies
* More difficult waves
* New enemy behaviors
* Increased strategic pressure
* More challenging defensive situations

---

## 🌍 Localization

Stand Tall supports:

* 🇬🇧 English
* 🇮🇷 Persian

The localization architecture is designed to allow additional languages to be added in the future.

---

## 🔊 Audio

The project uses a centralized audio architecture including:

* `SoundManager`
* `SoundTag`
* Music management
* Sound effects
* Configurable audio settings

---

## 💾 Save System

Player progression and settings are handled through a centralized save system.

The system is designed to persist:

* Level progression
* Player settings
* Gameplay preferences
* Unlocks and progression data

---

## 🛠️ Technology

| Technology             | Usage                |
| ---------------------- | -------------------- |
| **Unity**              | Game Engine          |
| **C#**                 | Programming          |
| **JSON**               | Data & Configuration |
| **Git**                | Version Control      |
| **Unity Localization** | Localization         |
| **Easy Save 3**        | Save System          |

---

## 📱 Platforms

### Android

Primary mobile platform.

### PC

Windows desktop build.

---

## 🎨 Art Direction

Stand Tall uses a stylized visual direction focused on:

* Clear gameplay readability
* Strong silhouettes
* A distinctive central cypress
* Clean and recognizable gameplay elements
* A cohesive visual identity

The cypress is the primary visual symbol of the game and is reflected throughout the game's presentation.

---

## 🚧 Development Status

**Current Version: `v2.1.0`**

**Status: In Development**

The project is actively being developed and improved.

### Current Focus

* Gameplay refinement
* Visual improvements
* Enemy balancing
* Wave balancing
* Level progression
* UI/UX improvements
* Performance optimization
* Android optimization
* Additional content

---

## 🗂️ Project Structure

```text
StandTall/
├── Assets/
│   ├── Art/
│   ├── Audio/
│   ├── Data/
│   ├── Prefabs/
│   ├── Scenes/
│   ├── Scripts/
│   │   ├── Enemies/
│   │   ├── Towers/
│   │   ├── Waves/
│   │   ├── UI/
│   │   ├── Systems/
│   │   └── Utilities/
│   └── Localization/
│
├── Packages/
└── ProjectSettings/
```

---

## 🧠 Development Principles

Stand Tall is developed with a focus on:

* **Modular architecture**
* **Reusable systems**
* **Data-driven gameplay**
* **Maintainable code**
* **Scalable content**
* **Cross-platform development**

The goal is to make new enemies, waves, levels, and gameplay systems easy to implement and maintain.

---

## 🗺️ Roadmap

* [x] Core Unity project
* [x] Basic enemy system
* [x] Cypress defense concept
* [x] Cylindrical climbing system
* [x] Wave generation architecture
* [x] Rope growth system
* [x] Initial visual refresh
* [x] Expand tower system
* [x] Expand enemy variety
* [x] Level progression
* [x] Boss enemies
* [x] Advanced balancing
* [x] Android optimization
* [x] PC release build
* [x] Final polish
* [x] Public release

---

## 🏴 About Jolly Panda

**Stand Tall** is an independent game developed under the **Jolly Panda** brand.

Jolly Panda focuses on creating games, interactive experiences, and experimental projects with an emphasis on gameplay, technology, and creative design.

---

## 📄 License

This project is proprietary.

The source code, artwork, assets, game design, and other project materials are owned by **Jolly Panda** and may not be reproduced, redistributed, or used commercially without permission.

---

<p align="center">

**🌲 Protect the Cypress. Stand Tall.**

Made with ❤️ by **Jolly Panda**

</p>
