///////////////////////////////////////////////////////////////////////////
// Главный заголовок
#include "SudokuSlv.h"

// Главная матрица и её копия
unsigned int			MtxS[SDS][SDS], Mtx[SDS][SDS];

// Размер матрицы
#define MTX_SIZE		(SDS * SDS * sizeof (Mtx[0][0]))

// Общие функции
#include "log2.cas"
#include "is_pow_2.cc"
#include "CheckRes.cc"
#include "CheckErr.cc"
#include "UpdateMtx.cc"
#include "Search.cc"

///////////////////////////////////////////////////////////////////////////
// Логарифм по основанию 2 в целых числах
unsigned int log2ui (unsigned int x)
	{
	return (unsigned int)log2 ((double)x);
	}

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
	#define TIT ("Title " ASSEMBLYDESCRIPTION)
	system (TIT);
	SetConsoleTextAttribute (hStdout, 0x0B);
	printf ("\n                     \x11 %s \x10\n\n", ASSEMBLYDESCRIPTION);
	printf ("  %s                        %s  \n\n", ASSEMBLYCOPYRIGHT, ASSEMBLYUPDATE);
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
				printf (" \x13 Input file \"%s\" is not available\n\n", FNAME);
				GETCH
				return -1;
				}

			if (argc == 2)
				{
				printf (" \x10 Output file not specified. Result will be presented only on the screen\n\n");
				}
			else
				{
				sprintf (FNAME, "%s", argv [2]);
				}

			break;

		// Программа вызвана без параметров или с неверным их числом
		default:
			printf (" \x10 Usage: SudokuSlv <input_file> [output_file]\n\n");
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
	if (UpdateMtx () == -1)
		{
		printf (" \x13 Input table causes an error on the first step.\n   Probably, there is a mistake(s) in it\n\n");
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
	Search ();

	///////////////////////////////////////////////////////////////////////////
	// Вывод результатов
	printf (" \x10 Solution completed successfully\n");
	if (argc == 3)
		{
		if ((F1 = fopen (FNAME, "w")) == NULL)
			{
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
	SetConsoleTextAttribute (hStdout, 0x07);
	printf ("\n");
	GETCH
	return 0;
	}
