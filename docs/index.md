# Sudoku solver: user guide
> **ƒ** &nbsp;RD AAOW FDL; 4.11.2023; 1:18



### Page contents

- [General information](#general-information)
- [Download links](https://adslbarxatov.github.io/DPArray#sudoku-solver)
- [Русская версия](https://adslbarxatov.github.io/SudokuSolver/ru)

---

### General information

This tool allows you to solve standart (9x9) sudoku tables.

Solution based on recursive function that builds series of “assumptions”
and finds first one that doesn’t conflict with sudoku game rules. Also
tool uses binary representation of known (`7 → 001000000b`) and unknown
(`1 or 2 or 6 → 000100011b`) numbers for simplifying solution process.

Tool accepts next keys:
- `1 – 9` – press these numbers while on cells to enter them;
- any other symbol – to clear selected cell;
- `F5` – to run solution process:
    - you will get red cells if your table is unsolvable (has or leads to a duplication of values);
    - you will get green cells when a solution is found;
- `F8` – to clear only found cells;
- `F12` – to clear all cells;
- `F3` – to load the table from file;
- `F4` – to save the table to file;
- `F1` – to get the quick help;
- Arrow keys – to move over the field;
- Press mouse buttons – to change values;
- `Alt` + `F4` – to exit the application