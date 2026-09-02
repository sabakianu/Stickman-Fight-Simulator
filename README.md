# Stickman Fight Simulator (Tactical Deck-Building)

> A tactical 2D fighting simulator that abandons the traditional "global health bar" in favor of a deeply interconnected biological and anatomical engine. 

Developed as a Bachelor's Thesis project in Computer Science, this game shifts the paradigm of fighting games from pure reflex-based button mashing to tactical planning, resource management, and dynamic physiological adaptation.

## Core Features

### 1. Advanced Anatomical & Biological Engine
Instead of generic health points, fighters have a complex internal structure divided into distinct layers that react realistically to trauma:
* **Damage Cascading (Overflow):** Attacks penetrate the body, distributing damage sequentially. When a muscle is destroyed, residual force (overflow) transfers to the bones, and eventually to the internal organs.
* **Joint Stability & Mobility:** Attacks targeting joints directly degrade mobility; if a joint's stability drops below 30%, specific abilities become locked and unusable.
* **Real-Time Physiological Penalties:** Accumulated pain slows down attack speed, while organ damage affects stamina regeneration (lungs/heart) and blood coagulation (liver).
* **Consciousness System:** Blood loss and extreme pain drain the fighter's consciousness; dropping below 5% results in an instant Knock-Out (K.O.).

### 2. Hybrid Deck-Building Combat Loop
The game operates on a strict 4-phase loop (Start, Strategy, Running, End):
* **Strategy Phase:** Action is paused. Players analyze their body's status and their opponent's condition to construct a deck of 5 abilities (Attacks, Blocks, or Dodges).
* **Execution Phase:** The selected abilities are played out automatically over a 15-second timer, with real-time biometric calculations adjusting damage, speed, and defense.

### 3. Adaptive AI & Decision Making
The enemy is controlled by two distinct AI algorithms utilizing Minimax concepts and utility scoring functions:
* **Deck Construction AI:** Predicts the player's strategy by analyzing their previous deck, dynamically adjusting its own ability scores to counter aggressive or defensive playstyles.
* **Real-Time Execution AI:** Evaluates combat variables second-by-second (stamina, hit chance, target health) to select the optimal move from its deck, incorporating stochastic noise to remain unpredictable.

## Technology Stack
* **Engine:** Unity 2D
* **Language:** C#
* **Architecture:** Component-Based Design, Event-Driven UI, Finite State Machine (FSM)
* **Data Management:** Object-Oriented Hierarchy utilizing Unity's `ScriptableObjects` for modular anatomical parts and abilities.

## Screenshots
<img width="1694" height="1055" alt="image2" src="https://github.com/user-attachments/assets/a1ffd36e-292e-49fb-9cf1-85b7bb98e81c" />
<img width="1708" height="1063" alt="image" src="https://github.com/user-attachments/assets/86681c81-6c93-4c0a-a170-0ff54e5a1c7d" />
<img width="1485" height="940" alt="image3" src="https://github.com/user-attachments/assets/d9eb3ca7-8a9e-4d4b-9463-ef0540bf2d15" />
<img width="1524" height="953" alt="image6" src="https://github.com/user-attachments/assets/8d14d33b-f55f-47b2-a5ee-fc404354d436" />
<img width="1521" height="957" alt="image5" src="https://github.com/user-attachments/assets/0f89caa2-c753-4088-8293-be475ca03d88" />
<img width="1523" height="958" alt="image4" src="https://github.com/user-attachments/assets/5d226109-adfe-47ab-99e4-b204e6031efc" />

