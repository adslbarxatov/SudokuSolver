///////////////////////////////////////////////////////////////////////////
// Главный заголовок
#include "SudokuSolver.h"

// Главная матрица и её копия
unsigned int			MtxS[SDS][SDS], Mtx[SDS][SDS];

// Размер матрицы
#define MTX_SIZE		(SDS * SDS * sizeof (Mtx[0][0]))

// Общие функции
#include "Log2.cas"
#include "IsPowOf2.cc"
#include "CheckResult.cc"
#include "CheckErrors.cc"
#include "UpdateMatrix.cc"
#include "Search.cc"

///////////////////////////////////////////////////////////////////////////
// ГЛАВНАЯ ФУНКЦИЯ
int main (int argc, char *argv[])
	{
	// Переменные
	FILE *F1;
	char FNAME[256];
	unsigned int i, j;
	int c;
	HANDLE hStdout = GetStdHandle (STD_OUTPUT_HANDLE);

	// Заголовок программы
	system ("Title " SS_PRODUCT);
	SetConsoleTextAttribute (hStdout, 0x0B);
	printf ("\n      \x11 %s \x10\n\n", SS_PRODUCT);
	printf ("  %s                         %s  \n\n", SS_COMPANY, SS_VERSION_S);
	SetConsoleTextAttribute (hStdout, 0x0F);

	///////////////////////////////////////////////////////////////////////////
	// Открытие файла по заданным параметрам
	switch (argc)
		{
		// Корректные параметры
		case 2:
		case 3:
			sprintf (FNAME, "%s", argv [1]);
			if ((F1 = fopen (FNAME, "r")) == NULL)
				{
				SetConsoleTextAttribute (hStdout, 0x0C);
				printf (" \x13 Input file \"%s\" is not available\n\n", FNAME);
				GETCH
				return -1;
				}

			if (argc == 2)
				{
				printf (" \x10 Output file not specified.\n   Result will be presented only on the screen\n\n");
				}
			else
				{
				sprintf (FNAME, "%s", argv [2]);
				}

			break;

		// Программа вызвана без параметров или с неверным их числом
		default:
			printf (" \x10 Usage: SudokuSolver <input_file> [output_file]\n\n");
			GETCH
			return 1;
		}

	///////////////////////////////////////////////////////////////////////////
	// Чтение файла
	FOR_I
		{
		FOR_J
			{
			// Не хватает символа
			if ((c = fgetc (F1)) == EOF)
				{
				SetConsoleTextAttribute (hStdout, 0x0C);
				printf (" \x13 Input table contains some mistakes or is incomplete\n\n");
				GETCH
				return -2;
				}
			// Не подходящий символ (нужно пропустить)
			else if (!((c >= '0') && (c <= '9')) && !(c == '-'))
				{
				j--;
				}
			// Всё нормально
			else
				{
				// Известная цифра матрицы
				if ((c >= '1') && (c <= '9'))
					{
					MtxS[i][j] = Mtx[i][j] = 1 << (c - 0x31);	// Выставляется номер соответствующего бита
					}
				// Неизвестная цифра (инициализируется нулём)
				else
					{
					MtxS[i][j] = Mtx[i][j] = SDS_FULL;			// Выставляются все биты (возможна любая цифра)
					}
				}
			}
		}

	fclose (F1);

	///////////////////////////////////////////////////////////////////////////
	// Подготовка матрицы
	//		Здесь выполняется первая прогонка матрицы, предполагающая удаление
	//		из клеток с заданным значением 0x1FF лишних бит. Для этого
	//		проверяются вертикальные и горизонтальные линии, а также квадраты
	//		Если эта процедура вызывает ошибку в вычислениях, значит, исходные
	//		данные неверны
	if (UpdateMatrix () == -1)
		{
		SetConsoleTextAttribute (hStdout, 0x0C);
		printf (" \x13 Input table causes an error on the first step of evaluation.\n   Probably, there is a mistake(s) in it\n\n");
		GETCH
		return -11;
		}

	///////////////////////////////////////////////////////////////////////////
	// Метод предположений
	//		Эта процедура начинает выполнять рекурсивные подстановки в
	//		невычисляемые простой прогонкой клетки, т.е., выполнив
	//		предположение о значении какой-то клетки (первой по порядку из
	//		невычисленных), она делает прогонку. Если она не даёт решения,
	//		функция вызывает саму себя, чтобы сделать ещё одно предположение,
	//		и т.д.
	//		Если предположение неверно, функция, обнаружившая это, завершает
	//		работу, давая возможность вызвавшему её экземпляру сделать
	//		другое предположение
	if (!Search ())
		{
		SetConsoleTextAttribute (hStdout, 0x0C);
		printf (" \x13 Input table has no correct solutions.\n   Probably, there is a mistake(s) in it\n\n");
		GETCH
		return -12;
		}

	///////////////////////////////////////////////////////////////////////////
	// Вывод результатов
	printf (" \x10 Solution completed successfully\n");
	if (argc == 3)
		{
		if ((F1 = fopen (FNAME, "w")) == NULL)
			{
			SetConsoleTextAttribute (hStdout, 0x0C);
			printf ("\n \x13 Cannot create output file \"%s\"\n\n", FNAME);
			GETCH
			return -3;
			}

		printf ("   Result also placed to file \"%s\"\n", FNAME);
		}

	FOR_I
		{
		printf ("\n\t");

		FOR_J
			{
			if (Mtx[i][j] != MtxS[i][j])
				{
				SetConsoleTextAttribute (hStdout, 0x0A);
				}
			else
				{
				SetConsoleTextAttribute (hStdout, 0x08);
				}

			if (argc == 3)
				{
				fprintf (F1, "%i", log2ui(2 * Mtx[i][j]));
				}
			_cprintf ("%i", log2ui(2 * Mtx[i][j]));

			// Выделение квадратов
			if ((j % SQ) == (SQ - 1))
				{
				printf (" ");
				if (argc == 3)
					{
					fprintf (F1, " ");
					}
				}
			}

		if (argc == 3)
			{
			fprintf (F1, "\n");
			}

		// Выделение квадратов
		if ((i % SQ) == (SQ - 1))
			{
			printf ("\n");
			if (argc == 3)
				{
				fprintf (F1, "\n");
				}
			}
		}
	if (argc == 3)
		{
		fclose (F1);
		}

	///////////////////////////////////////////////////////////////////////////
	// Завершено
	printf ("\n");
	GETCH
	return 0;
	}
