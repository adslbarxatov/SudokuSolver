///////////////////////////////////////////////////////////////////////////
// ���������� ��������������
#define _CRT_SECURE_NO_WARNINGS

// �������: ������ � ������ ������������ ����������
#define	ASSEMBLYCOMPANY	"RD AAOW"				// EXE Company name
#define	ASSEMBLYNAME	"Sudoku solver"			// EXE Product name
#define	ASSEMBLYVERSION	1,5,0,0					// EXE File / Product version
#define	ASSEMBLYCOPYRIGHT	"(C) Barhatov N."	// EXE Copyright
#define	ASSEMBLYDESCRIPTION	ASSEMBLYNAME		// EXE Description
#define	ASSEMBLYUPDATE	"14.08.2017; 21:21"		// EXE Last update

///////////////////////////////////////////////////////////////////////////
// ������������ �����
#include <stdio.h>
#include <conio.h>
#include <math.h>
#include <string.h>
#include <windows.h>

///////////////////////////////////////////////////////////////////////////
// ������� ���������
#define SDS					9					// ������ ������
/*
	������ ��������� ���������� �������� ������� ��������� � ��������
	�������� �������� ������� ������. �.�. ���� �������� ����� �������
	�����������	� �������� ����, �� ������ ���, ������������� � 1, �����
	���� �������, ������� � ������ ������ �������������� ���������� � 
	��������������� ������ ������
	��������������, ���� ���, ������ 1, � �������� ������ ������� ����
	����, ������ ��������� �����������. ���	���� ��������, ��� �
	�������� ����� �������� ����� 2. ����������� ����� ���������
	�������� ��������� ������� ������
*/
#define SDS_FULL			((1 << SDS) - 1)			// 123456789

///////////////////////////////////////////////////////////////////////////
// �������
#define MAX_ITER			50							// ����������, ���� ��������� �������� ������ ����� ��������

#define SQ					((int)sqrt ((float)SDS))	// �������� ������ �������� �������
#define SQI					(j % SQ + (i % SQ) * SQ)	// ��������������� ���������, ����������� ������������
#define SQJ					(j / SQ + (i / SQ) * SQ)	// �������� ������������ ��� ���������������� ��������
														// �� ��������

#define FOR_I				for (i = 0; i < SDS; i++)	// �����
#define FOR_J				for (j = 0; j < SDS; j++)
#define FOR_K				for (k = 0; k < SDS; k++)

#define IS_MIJ_POW2			IsPowOf2 (Mtx[i][j])		// ����������, �������� �� ������� ������� �������
#define IS_MSIJ_POW2		IsPowOf2 (Mtx[SQI][SQJ])	// �������� ����� 2

#define GETCH				SetConsoleTextAttribute (hStdout, 0x07); _getch ();