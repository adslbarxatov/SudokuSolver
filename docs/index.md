# Sudoku solver: user guide
> **ƒ** &nbsp;RD AAOW FDL; 10.07.2025; 2:26



### Page contents

- [General information](#general-information)
- [Game mode (Android)](#game-mode)
- [Controls (Android)](#controls-android)
- [Controls (Windows)](#controls-windows)
- [Download links](https://adslbarxatov.github.io/DPArray#sudoku-solver)
- [Версия на русском языке](https://adslbarxatov.github.io/SudokuSolver/ru)

---

### General information

This tool allows you to automatically or manually solve default (9 x 9)
sudoku tables.

Solution based on recursive function that builds series of “assumptions”
and finds first one that doesn’t conflict with sudoku game rules.

> Warning! This method ***always*** finishes with some result (it is finite).
> But in some cases it can take time. This behavior is correct
> for the application.

In addition, the app now has a game mode: it can check for the presence
of a solution, but not display it, only reporting its presence or absence.

&nbsp;



### Game mode

A new game can be launched from any state of the app. However, we recommend
switching the interface to the game mode to display buttons more suitable
for the gameplay on the main screen. In addition, in this mode, the app doesn’t
allow the device to go into sleep mode if the player is inactive.

The button for starting a new game switches the app to the active game state,
in which it begins to take into account the player’s actions for the purpose
of calculating the winnings. In this case, the player can select the difficulty
level of the generated table. In addition, the active game doesn’t allow changing
the values in the cells filled in when generating the table.

Exit from the active game state occurs if:
- reset all values in all cells
- switch the interface to the table-solving mode or exit the app in this mode
- load the table from file
- make the complete sudoku solution (with the result displayed) from the menu (green checkmark or `F5` key)
- finish solving (win) the table manually

In the active game state, the main function is to check for a solution (blue checkmark or `F3` key).
When you click this button, the current state of the table is checked for the possibility
of a successful solution. If there is no solution, the user is notified of an error.
Otherwise, it is not displayed in the interface, but the user can see a message about
its presence. At the same time, his winnings are calculated, and the entered numbers
become unchangeable.

The player’s winnings are the number of correct values specified between pressing the check
button, squared and multiplied by the difficulty level selected at the beginning of the game.
This formula allows us to simultaneously take into account the complexity of the task
and the length of the chain of values entered between checks.

The game is considered successfully completed when, upon pressing the check button,
there is one or less empty cell left in the table. The active game state is disabled,
and the user is provided with game mode statistics.

It should be noted that exiting the app doesn’t stop the game. The state of the app
is saved until the next launch, so the game can be continued at any time.

&nbsp;



### Controls (Android)

- Click on cells to select them
- Click them again to select their values (1 to 9 + empty)
- Use the numeric keypad to set the value of the selected cell
- Use the buttons to execute the solution, check for its presence, and start a new game

&nbsp;



### Controls (Android TV)

- Click on cells to select their values (1 to 9 + empty)
- Use the buttons to execute the solution, check for its presence, and start a new game

&nbsp;



### Controls (Windows)

- `←`, `→`, `↑`, `↓` – to move over the field
- `1` – `9` – press these numbers while on cells to enter them
- mouse buttons and wheel – to change values (1 to 9 + empty)
- any other character – to clear selected cell
- `F5` – to run the solution process:
    - you will get red cells if your table is unsolvable (has or leads to a duplication of values)
    - you will get green cells when a solution is found
- `F8` – to clear only found cells
- `F12` – to clear all cells
- `Ctrl` + `O` – to load the table from file
- `Ctrl` + `S` – to save the table to file
- `Ctrl` + `1` – to generate easy sudoku table
- `Ctrl` + `2` – to generate medium table
- `Ctrl` + `3` – to generate hard table
- `F3` – to check the availability of solution without showing it (game mode)
- `F1` – to get the quick help
- `Alt` + `F4`, `Ctrl` + `X` – to exit the app
