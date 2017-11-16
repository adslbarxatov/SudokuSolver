# SudokuSolver v 1.5

A tool for solving sudoku / Инструмент для решения судоку
#
This console tool allows you to solve standart (9x9) sudoku tables.

Solution based on recursive function that builds series of 'assumptions'
and finds first one that doesn't conflict with sudoku game rules. Also
tool uses binary representation of known (7 -> 001000000b) and unknown
(1 or 2 or 6 -> 000100011b) numbers for simplifying solution process.
#
Этот консольный инструмент позволяет решать стандартные (9x9) судоку.

Решение основано на рекурсивной функции, строящей серии 'предположений'
и возвращающей первое из них, которое не конфликтует с правилами судоку.
Программа использует бинарное представление известных (7 -> 001000000b)
и неизвестных (1 или 2 или 6 -> 000100011b) значений для упрощения
поиска решения.
#

Only .c and .h must be compiled; other are included by .c. Interface language: en_us

Компилировать нужно только файлы .c и .h; остальные включены в файлы .c. Язык интерфейса: en_us
