# BlinkMod for *Jump Ship*

A debug/utility mod for *Jump Ship* using [MelonLoader](https://melonwiki.xyz/) and Il2Cpp.  
BlinkMod enables teleportation, respawn, and intelligent blink-through-wall functionality using keyboard and mouse inputs.

## 🔧 Features

- **Blink (F1 or Mouse Forward Button)**  
  Instantly teleport to the object you're looking at.

- **Smart Blink (F2)**  
  Attempt to blink through a wall to the first safe open space behind it.

- **Respawn (F3)**  
  Instantly respawn your character.

## 🎮 Controls

| Key/Button            | Action                          |
|-----------------------|---------------------------------|
| `F1` or Mouse Forward | Teleport to where you're aiming |
| `F2`                  | Blink past wall to open space   |
| `F3`                  | Respawn player                  |

## 💡 Installation

1. Install [MelonLoader](https://melonwiki.xyz/#/mdk/setup) for *Jump Ship*.
2. Place `BlinkMod.dll` into your `Mods/` folder inside the game directory.
3. Launch the game. If successful, the console will display:  
   `BlinkMod: Initialized.`

## 🛠 Requirements

- **Game:** Jump Ship by KeepsakeGames  
- **Mod Loader:** [MelonLoader](https://melonwiki.xyz/)  
- **Dependencies:** None beyond base game libraries

## 🧠 Notes

- The mod uses Unity's physics system to locate teleport targets.
- "Smart Blink" avoids teleporting into obstructed spaces by checking for a clear capsule-sized area beyond the wall.
