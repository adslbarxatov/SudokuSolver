///////////////////////////////////////////////////////////////////////////
// Отключение предупреждений
#define _CRT_SECURE_NO_WARNINGS

// Ресурсы: данные о версии создаваемого приложения
#define	ASSEMBLYCOMPANY	"RD AAOW"				// EXE Company name
#define	ASSEMBLYNAME	"Sudoku solver"			// EXE Product name
#define	ASSEMBLYVERSION	1,5,0,0					// EXE File / Product version
#define	ASSEMBLYCOPYRIGHT	"(C) Barhatov N."	// EXE Copyright
#define	ASSEMBLYDESCRIPTION	ASSEMBLYNAME		// EXE Description
#define	ASSEMBLYUPDATE	"14.08.2017; 21:21"		// EXE Last update

///////////////////////////////////////////////////////////////////////////
// Заголовочные файлы
#include <stdio.h>
#include <conio.h>
#include <math.h>
#include <string.h>
#include <windows.h>

///////////////////////////////////////////////////////////////////////////
// Базовые параметры
#define SDS					9					// РАЗМЕР СУДОКУ
/*
	Данная программа использует двоичную систему счисления в качестве
	базового средства решения задачи. Т.е. если значения ячеек матрицы
	представить	в двоичном виде, то номера бит, установленных в 1, будут
	теми цифрами, которые в данный момент предполагается проставить в 
	соответствующую клетку судоку
	Соответственно, если бит, равный 1, в значении ячейки остался лишь
	один, клетка считается вычисленной. При	этом очевидно, что её
	значение будет степенью числа 2. Определение этого состояния
	является критерием решения задачи
*/
#define SDS_FULL			((1 << SDS) - 1)			// 123456789

///////////////////////////////////////////////////////////////////////////
// Макросы
#define MAX_ITER			50							// Остановить, если выполнено итераций больше этого значения

#define SQ					((int)sqrt ((float)SDS))	// Линейный размер квадрата матрицы
#define SQI					(j % SQ + (i % SQ) * SQ)	// Пересчитываемые параметры, позволяющие использовать
#define SQJ					(j / SQ + (i / SQ) * SQ)	// линейные коэффициенты для зигзагообразного движения
														// по квадрату

#define FOR_I				for (i = 0; i < SDS; i++)	// Циклы
#define FOR_J				for (j = 0; j < SDS; j++)
#define FOR_K				for (k = 0; k < SDS; k++)

#define IS_MIJ_POW2			IsPowOf2 (Mtx[i][j])		// Определяют, является ли текущий элемент матрицы
#define IS_MSIJ_POW2		IsPowOf2 (Mtx[SQI][SQJ])	// степенью числа 2

#define GETCH				SetConsoleTextAttribute (hStdout, 0x07); _getch ();