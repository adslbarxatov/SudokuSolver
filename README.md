# SudokuSolver v 1.5

A tool for solving sudoku

#

This console tool allows you to solve standart (9x9) sudoku tables.

Solution based on recursive function that builds series of 'assumptions'
and finds first one that doesn't conflict with sudoku game rules. Also
tool uses binary representation of known (7 -> 001000000b) and unknown
(1 or 2 or 6 -> 000100011b) numbers for simplifying solution process.

Final control is not realised yet. If you see, that your solution is
crazy (for example, three 6's in the same square), it only means that
the input table contains some mistakes. Be accurate when create input
file.

#

Needs Windows XP and newer, some C/C++ compiler. Only .c and .h must
be compiled; other are included by .c. Interface language: en_us
