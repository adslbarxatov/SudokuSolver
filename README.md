# SudokuSolver v 1.4

A tool for solving sudoku

#

This console tool allows you to solve standart (9x9) sudoku tables.
Solution based on recursive function that builds series of 'assumptions'
and finds first one that doesn't conflict with sudoku game rules. Also
tool uses binary representation of known (7 -> 001000000b) and unknown
(1 or 2 or 6 -> 000100011b) numbers for simplifying solution process

#

Needs Windows XP and newer, some C/C++ compiler. Only .c and .h must
be compiled; other are included by .c. Interface language: en_us
