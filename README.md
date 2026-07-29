### RunTrackerOverlay

`RunTrackerOverlay` is a lightweight, customizable WPF application designed to track run times and loot drops in real-time via a transparent overlay.

---

### Getting Started

1.  **Launch**: Run the executable to open the semi-transparent overlay.
2.  **Position**: Click and drag the overlay to your preferred location. The position is saved automatically.
3.  **Configure**: Click the **Gear Icon** (Settings) to customize hotkeys, appearance, and tracking modes.

---

### Window Behavior & Focusing

The overlay stays out of your way while gaming but remains easily accessible.

#### Unfocused State (Click-Through Mode)
When you are playing a game or using another application:
*   **Click-Through**: The overlay is "transparent" to mouse clicks. You can interact with the game UI behind it without accidental clicks on the tracker.
*   **Visuals**: The background opacity typically increases (becomes more transparent) to minimize obstruction.
*   **Global Hotkeys**: The app continues to listen for your hotkeys, allowing full control without leaving your game.

#### How to Focus the App
To move the window or change settings, you must bring it into focus:
*   **Focus Key**: Press the configured **Focus Key** (Default: `F8`). This makes the overlay interactive and brings it to the foreground.
*   **Alt-Tab**: Select the application via the standard Windows `Alt+Tab` menu or Taskbar.

#### Focused State
When the app is active:
*   **Interactive UI**: Buttons for Settings (Gear), Reset (X), Save (Disk), and Close become clickable.
*   **Draggable**: Click and drag anywhere on the overlay to reposition it.

---

### Core Features

#### 1. Tracking Runs
*   **Start/Stop**: Press the **Activation Key** (Default: `Pause`) to toggle the timer.
*   **Continuous Mode**: When enabled, stopping a run immediately starts the next one, incrementing the run count automatically.

#### 2. Logging Loot
*   **Add Loot**: Press the **Loot Key** (Default: `Page Up`) to open the quick input dialog.
*   **Quick Entry**: The dialog automatically steals focus so you can type immediately. Press `Enter` to save or `Esc` to cancel.
*   **Smart Formatting**: Item names are automatically converted to "Title Case."
*   **Contextual Logging**: Loot is associated with the active run. If no run is active, it is appended to the most recently completed run.

#### 3. Display Statistics
Choose to show or hide the following in settings:
*   **Current/Last Run Time**: Real-time timer or previous run duration.
*   **Session Stats**: Best, Worst, Average, and Total session time.
*   **Run Count**: Total runs in the current session.
*   **Session Name**: A custom label displayed at the top.

---

### Default Hotkeys

| Action | Default Key | Description |
| :--- | :--- | :--- |
| **Activation** | `Pause` | Start or Stop the current run. |
| **Loot Key** | `Page Up` | Open the loot entry dialog. |
| **Focus Key** | `F8` | Bring overlay to foreground / Enable mouse interaction. |
| **Pause Key** | `F9` | Hard stop (useful in Continuous Mode). |

---

### Configuration Options

*   **Opacity**: Independent sliders for Background and Text transparency.
*   **Stat Toggles**: Choose exactly which session statistics are visible.
*   **Snapping**: Enable window snapping to screen edges for pixel-perfect placement.
*   **Timer Format**: Option to hide milliseconds for a cleaner look.
*   **Session Management**:
    *   **Reset**: Clears session stats and optionally deletes the temporary log.
    *   **Save/Export**: Export data to `.txt`, with filters to include all runs or only those with loot.
    *   **Current Session Log**: The app maintains `currentSession.txt` in the root folder, updated after every run.
<img width="612" height="475" alt="photo_4_2026-07-30_00-33-59" src="https://github.com/user-attachments/assets/d8ba0366-094d-46db-8b69-01d091e5f48d" />
<img width="571" height="469" alt="photo_5_2026-07-30_00-33-59" src="https://github.com/user-attachments/assets/22d99ffa-4ce8-41d5-bc4f-a32a65604320" />
<img width="317" height="396" alt="photo_2_2026-07-30_00-33-59" src="https://github.com/user-attachments/assets/1a5f38fd-c49e-4bb7-926a-a79f641ee7b5" />
<img width="392" height="312" alt="photo_3_2026-07-30_00-33-59" src="https://github.com/user-attachments/assets/1a215498-2911-4711-93ae-243fc4cbd195" />
