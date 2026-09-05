# 🏴 Last Flag

> **Defend the flag. Survive the climb. Hold the line.**

**Last Flag** is a Tower Defense game developed by **Jolly Panda Studio** for **PC and Android**.

Players must defend their flag against waves of enemies climbing a vertical cylindrical battlefield. Build and manage your defenses, survive increasingly difficult waves, and keep the flag standing until the very end.

---

## 🎮 Game Overview

In **Last Flag**, the enemy doesn't simply walk along a traditional path.

Instead, enemies **climb around a cylindrical battlefield**, creating a unique defensive challenge where positioning, timing, and wave management are essential.

Your objective is simple:

**Protect the flag at all costs.**

As the game progresses, enemy waves become stronger and more complex, requiring better defensive strategies and careful resource management.

---

## ✨ Features

* 🏰 Tower Defense gameplay
* 🧗 Enemies climbing a cylindrical battlefield
* 🏴 Flag-based objective
* 🌊 Data-driven enemy wave system
* 👾 Multiple enemy types
* 📈 Progressive difficulty
* 🎯 Strategic tower placement
* 💾 Save & progression system
* 🌍 Persian & English localization
* 🔊 Centralized audio management
* 📱 Android support
* 🖥️ PC support

---

## 🕹️ Gameplay

The core gameplay loop is built around:

1. **Prepare your defenses**
2. **Start an enemy wave**
3. **Defend the flag**
4. **Manage your towers and resources**
5. **Survive the wave**
6. **Prepare for the next challenge**

Every wave introduces new challenges and gradually increases the pressure on the player.

---

## 🧩 Technical Architecture

Last Flag is built with **Unity** and follows a modular, data-driven approach.

### Wave System

Enemy waves are generated using a custom:

`WaveDataGenerator`

This system allows enemy compositions and wave progression to be configured without hardcoding every wave directly into gameplay logic.

### Rope / Climbing System

The cylindrical battlefield uses a custom:

`RopeGrower`

system to handle the dynamically growing climbing path used by enemies.

This allows the battlefield and enemy progression to scale with the level design.

---

## 👾 Planned Enemy Types

The game is designed around approximately **10 enemy types**, each introducing different gameplay characteristics and requiring different defensive strategies.

Enemy variety is intended to be expanded throughout development.

---

## 🗺️ Level Progression

The current design targets approximately:

**50 Levels**

Levels gradually introduce:

* Stronger enemies
* More difficult waves
* New enemy behaviors
* Increasing strategic pressure
* More demanding defensive layouts

---

## 🌍 Localization

Last Flag is designed with bilingual support:

* 🇬🇧 English
* 🇮🇷 Persian

The localization architecture is intended to make adding additional languages possible in the future.

---

## 🔊 Audio System

The project uses centralized audio management, including systems such as:

* `SoundManager`
* `SoundTag`
* Configurable music
* Sound effects
* Language-aware audio handling

---

## 💾 Save System

Player progression is handled through a centralized save architecture.

The project is designed to support persistent:

* Level progression
* Player settings
* Gameplay preferences
* Unlocks and progression data

---

## 🛠️ Technology

| Technology         | Usage                |
| ------------------ | -------------------- |
| Unity              | Game Engine          |
| C#                 | Programming          |
| JSON               | Data & Configuration |
| Git                | Version Control      |
| Unity Localization | Localization         |
| Easy Save 3        | Save System          |

---

## 📱 Target Platforms

### Android

Primary mobile target.

### PC

Desktop build for Windows.

---

## 🎨 Art Direction

Last Flag combines a stylized game presentation with a gameplay-focused visual language.

The visual direction is designed to keep the battlefield readable while maintaining a distinctive identity around the central **flag and cylindrical battlefield**.

---

## 🚧 Development Status

**Status: In Development**

The project is actively being developed and its systems are continuously being improved.

### Current Focus

* Core gameplay refinement
* Enemy systems
* Wave balancing
* Level progression
* Tower systems
* UI/UX improvements
* Performance optimization
* Android optimization
* Additional content

---

## 🗂️ Project Structure

The project follows a modular structure designed to keep gameplay systems, data, UI, and assets separated.

```text
LastFlag/
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
├── ProjectSettings/
└── Packages/
```

---

## 🧠 Development Philosophy

Last Flag is built with a focus on:

* **Modular systems**
* **Reusable components**
* **Data-driven design**
* **Maintainable architecture**
* **Scalable content**
* **Cross-platform development**

The goal is to make adding new enemies, waves, levels, and gameplay systems as straightforward as possible.

---

## 🏴 About Jolly Panda

**Last Flag** is an independent game project developed under the **Jolly Panda** brand.

Jolly Panda focuses on creating experimental games, interactive experiences, and game-development projects.

---

## 📌 Roadmap

* [x] Core Unity project
* [x] Basic enemy system
* [x] Cylindrical climbing concept
* [x] Wave generation architecture
* [x] Rope growth system
* [x] Tower system expansion
* [x] Enemy variety
* [x] Level progression
* [x] Boss enemies
* [x] Advanced balancing
* [x] Android optimization
* [x] PC release build
* [x] Final polish
* [x] Public release

---

## 📄 License

This project is proprietary.

The source code, artwork, assets, game design, and other project materials are owned by **Jolly Panda** and may not be reproduced, redistributed, or used commercially without permission.

---

<p align="center">

**🏴 Defend the Last Flag.**

Made with ❤️ by **Jolly Panda**

</p>
