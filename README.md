# SudokuSolver v 3.3.5
> PCC: 0002F9B1D3DBECA2


A simple console and Windows tools for solving sudoku

---

Простой консольный и оконный инструменты для решения судоку

&nbsp;



## About application / Общие сведения

These tools allow you to solve standart (9x9) sudoku tables.

Solution based on recursive function that builds series of “assumptions”
and finds first one that doesn’t conflict with sudoku game rules. Also
tool uses binary representation of known (`7 → 001000000b`) and unknown
(`1 or 2 or 6 → 000100011b`) numbers for simplifying solution process.

---

Эти инструменты позволяют решать стандартные (9x9) судоку.

Решение основано на рекурсивной функции, строящей серии «предположений»
и возвращающей первое из них, которое не конфликтует с правилами судоку.
Программа использует бинарное представление известных (`7 → 001000000b`)
и неизвестных (`1 или 2 или 6 → 000100011b`) значений для упрощения
поиска решения.

---

Windows tool accepts next keys:
- `1 – 9` – press these numbers while on cells to enter them;
- any other symbol – to clear selected cell;
- `Enter` – to run solution process:
    - you will get red cells if your table is unsolvable (has or leads to a duplication of values);
    - you will get green cells when a solution is found;
- `Backspace` – to clear only found cells;
- `Esc` – to clear all cells;
- `F1` – to get application’s about;
- Arrow keys – to move over the field;
- Press buttons – to change values;
- `L` – to set the interface language;
- `Alt` + `F4` – to exit application

---

Оконный вариант утилиты управляется следующими клавишами:
- `1 - 9` – нажимайте эти цифры, находясь в ячейках, чтобы ввести их;
- любой другой символ – чтобы очистить выделенную ячейку;
- `Enter` – чтобы запустить процесс решения:
    - вы получите красные ячейки, если ваша таблица неразрешима (имеет или приводит к дублированию значений);
    - вы получите зелёные клетки, когда решение будет найдено;
- `Backspace` – чтобы очистить только найденные клетки;
- `Esc` – чтобы очистить все ячейки;
- `F1` – узнать о приложении;
- клавиши со стрелками – для перемещения по полю;
- нажатие кнопок – для изменения значений;
- `L` – для задания языка интерфейса;
- `Alt` + `F4` – для выхода из приложения

&nbsp;



## Requirements / Требования

- Windows 7 or newer / или новее;
- [Microsoft .NET Framework 4.8](https://go.microsoft.com/fwlink/?linkid=2088631).

Interface languages / языки интерфейса: ru_ru, en_us.

&nbsp;



## [Development policy and EULA](https://adslbarxatov.github.io/ADP) / [Политика разработки и EULA](https://adslbarxatov.github.io/ADP/ru)

This Policy (ADP), its positions, conclusion, EULA and application methods
describes general rules that we follow in all of our development processes, released applications and implemented ideas.
***It must be acquainted by participants and users before using any of laboratory’s products.
By downloading them, you agree and accept this Policy!***

Данная Политика (ADP), её положения, заключение, EULA и способы применения
описывают общие правила, которым мы следуем во всех наших процессах разработки, вышедших в релиз приложениях
и реализованных идеях.
***Обязательна к ознакомлению для всех участников и пользователей перед использованием любого из продуктов лаборатории.
Загружая их, вы соглашаетесь и принимаете эту Политику!***
