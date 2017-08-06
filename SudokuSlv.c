///////////////////////////////////////////////////////////////////////////
// ������� ���������
#include "SudokuSlv.h"

// ������� ������� � � �����
unsigned int			MtxS[SDS][SDS], Mtx[SDS][SDS];

// ������ �������
#define MTX_SIZE		(SDS * SDS * sizeof (Mtx[0][0]))

// ����� �������
#include "log2.cas"
#include "is_pow_2.cc"
#include "CheckRes.cc"
#include "CheckErr.cc"
#include "UpdateMtx.cc"
#include "Search.cc"

///////////////////////////////////////////////////////////////////////////
// �������� �� ��������� 2 � ����� ������
unsigned int log2ui (unsigned int x)
	{
	return (unsigned int)log2 ((double)x);
	}

///////////////////////////////////////////////////////////////////////////
// ������� �������
int main (int argc, char *argv[])
	{
	// ����������
	FILE *F1;
	char FNAME[256];
	unsigned int i, j;
	int c;
	HANDLE hStdout = GetStdHandle (STD_OUTPUT_HANDLE);

	// ��������� ���������
	#define TIT ("Title " ASSEMBLYDESCRIPTION)
	system (TIT);
	SetConsoleTextAttribute (hStdout, 0x0B);
	printf ("\n                     \x11 %s \x10\n\n", ASSEMBLYDESCRIPTION);
	printf ("  %s                        %s  \n\n", ASSEMBLYCOPYRIGHT, ASSEMBLYUPDATE);
	SetConsoleTextAttribute (hStdout, 0x0F);

	///////////////////////////////////////////////////////////////////////////
	// �������� ����� �� �������� ����������
	switch (argc)
		{
		// ���������� ���������
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

		// ��������� ������� ��� ���������� ��� � �������� �� ������
		default:
			printf (" \x10 Usage: SudokuSlv <input_file> [output_file]\n\n");
			GETCH
			return 1;
		}

	///////////////////////////////////////////////////////////////////////////
	// ������ �����
	FOR_I
		{
		FOR_J
			{
			// �� ������� �������
			if ((c = fgetc (F1)) == EOF)
				{
				printf (" \x13 Input table contains some mistakes or is incomplete\n\n");
				GETCH
				return -2;
				}
			// �� ���������� ������ (����� ����������)
			else if (!((c >= '0') && (c <= '9')) && !(c == '-'))
				{
				j--;
				}
			// �� ���������
			else
				{
				// ��������� ����� �������
				if ((c >= '1') && (c <= '9'))
					{
					MtxS[i][j] = Mtx[i][j] = 1 << (c - 0x31);	// ������������ ����� ���������������� ����
					}
				// ����������� ����� (���������������� ����)
				else
					{
					MtxS[i][j] = Mtx[i][j] = SDS_FULL;			// ������������ ��� ���� (�������� ����� �����)
					}
				}
			}
		}

	fclose (F1);

	///////////////////////////////////////////////////////////////////////////
	// ���������� �������
	//		����� ����������� ������ �������� �������, �������������� ��������
	//		�� ������ � �������� ��������� 0x1FF ������ ���. ��� �����
	//		����������� ������������ � �������������� �����, � ����� ��������
	//		���� ��� ��������� �������� ������ � �����������, ������, ��������
	//		������ �������
	if (UpdateMtx () == -1)
		{
		printf (" \x13 Input table causes an error on the first step.\n   Probably, there is a mistake(s) in it\n\n");
		GETCH
		return -11;
		}

	///////////////////////////////////////////////////////////////////////////
	// ����� �������������
	//		��� ��������� �������� ��������� ����������� ����������� �
	//		������������� ������� ��������� ������, �.�., ��������
	//		������������� � �������� �����-�� ������ (������ �� ������� ��
	//		�������������), ��� ������ ��������. ���� ��� �� ��� �������,
	//		������� �������� ���� ����, ����� ������� ��� ���� �������������,
	//		� �.�.
	//		���� ������������� �������, �������, ������������ ���, ���������
	//		������, ����� ����������� ���������� � ���������� �������
	//		������ �������������
	Search ();

	///////////////////////////////////////////////////////////////////////////
	// ����� �����������
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

			// ��������� ���������
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

		// ��������� ���������
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
	// ���������
	SetConsoleTextAttribute (hStdout, 0x07);
	printf ("\n");
	GETCH
	return 0;
	}
