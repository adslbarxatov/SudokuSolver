# Sudoku solver: user guide
> **ƒ** &nbsp;RD AAOW FDL; 9.04.2025; 23:32



### Page contents

- [General information](#general-information)
- [Download links](https://adslbarxatov.github.io/DPArray#sudoku-solver)
- [Версия на русском языке](https://adslbarxatov.github.io/SudokuSolver/ru)

---

### General information

This tool allows you to solve default (9 x 9) sudoku tables.

Solution based on recursive function that builds series of “assumptions”
and finds first one that doesn’t conflict with sudoku game rules.

> Warning! This method ***always*** finishes with some result (it is finite).
> But in some cases it can take some time. This behavior is correct
> for the application.

In addition, the app now has a game mode: it can check for the presence
of a solution, but not display it, only reporting its presence or absence.


***Controls (Android):***

- Click cells to select them;
- Click them again to change their values (from 1 to 9 + empty value);
- Use numeric keyboard to set the value of selected cell;
- Use menu to set the current mode and alignment of numeric keyboard;
- Use other buttons to solve the table or check it for solution


***Controls (Windows):***

- `←`, `→`, `↑`, `↓` – to move over the field;
- `1` – `9` – press these numbers while on cells to enter them;
- mouse buttons – to change values (from 1 to 9 + empty value);
- any other character – to clear selected cell;
- `F5` – to run the solution process:
    - you will get red cells if your table is unsolvable (has or leads to a duplication of values);
    - you will get green cells when a solution is found;
- `F8` – to clear only found cells;
- `F12` – to clear all cells;
- `Ctrl` + `O` – to load the table from file;
- `Ctrl` + `S` – to save the table to file;
- `Ctrl` + `1` – to generate easy sudoku table;
- `Ctrl` + `2` – to generate medium table;
- `Ctrl` + `3` – to generate hard table;
- `F3` – to check the availability of solution without showing it (game mode);
- `F1` – to get the quick help;
- `Alt` + `F4` – to exit the application
