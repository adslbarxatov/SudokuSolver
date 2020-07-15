# SudokuSolver v 2.1

A simple console and Windows tools for solving sudoku

Простой консольный и оконный инструменты для решения судоку



## About application / Общие сведения

These tools allow you to solve standart (9x9) sudoku tables.

Solution based on recursive function that builds series of “assumptions”
and finds first one that doesn't conflict with sudoku game rules. Also
tool uses binary representation of known (```7 -> 001000000b```) and unknown
(```1 or 2 or 6 -> 000100011b```) numbers for simplifying solution process.

Эти инструменты позволяют решать стандартные (9x9) судоку.

Решение основано на рекурсивной функции, строящей серии «предположений»
и возвращающей первое из них, которое не конфликтует с правилами судоку.
Программа использует бинарное представление известных (```7 -> 001000000b```)
и неизвестных (```1 или 2 или 6 -> 000100011b```) значений для упрощения
поиска решения.



## Console tool / Версия для командной строки

Console tool requires text files (like presented below) as input

Консольный инструмент требует текстовые файлы (наподобие нижеприведённого) в качестве входных

```
030000800
070080400
000019300
600800007
200000009
400002005
009430000
005070060
001000020
```

Result can be received on the monitor or in file if its name is specified as the second parameter

Результат можно получить на экране или в виде файла, если указать его имя в качестве второго параметра



## Version for Windows / Версия для Windows

Windows tool accepts next keys:
- 1 – 9 – press these numbers while on cells to enter them;
- any other symbol – to clear selected cell;
- Enter – to run solution process:
    - you will get red cells if your table is unsolvable (has or leads to a duplication of values);
    - you will get green cells when a solution is found;
- Backspace – to clear only found cells;
- Esc – to clear all cells;
- F1 – to get application's about;
- Arrow keys – to move over the field;
- Alt + F4 – to exit application

Needs Windows XP and newer, Framework 4.0 and newer


Оконный вариант утилиты управляется следующими клавишами:
- 1 - 9 – нажимайте эти цифры, находясь в ячейках, чтобы ввести их;
- любой другой символ – чтобы очистить выделенную ячейку;
- Enter – чтобы запустить процесс решения:
    - вы получите красные ячейки, если ваша таблица неразрешима (имеет или приводит к дублированию значений);
    - вы получите зелёные клетки, когда решение будет найдено;
- Backspace – чтобы очистить только найденные клетки;
- Esc – чтобы очистить все ячейки;
- F1 – узнать о приложении;
- клавиши со стрелками – для перемещения по полю;
- Alt + F4 – для выхода из приложения

Требуется ОС Windows XP и новее, Framework 4.0 и новее



## Development policy and EULA / Политика разработки и EULA

This [Policy (ADP)](https://vk.com/@rdaaow_fupl-adp), its positions, conclusion, EULA and application methods
describes general rules that we follow in all of our development processes, released applications and implemented
ideas.
**It must be acquainted by participants and users before using any of laboratory's products.
By downloading them, you agree to this Policy**

Данная [Политика (ADP)](https://vk.com/@rdaaow_fupl-adp), её положения, заключение, EULA и способы применения
описывают общие правила, которым мы следуем во всех наших процессах разработки, вышедших в релиз приложениях
и реализованных идеях.
**Обязательна к ознакомлению всем участникам и пользователям перед использованием любого из продуктов лаборатории.
Загружая их, вы соглашаетесь с этой Политикой**
