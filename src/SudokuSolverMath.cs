using System;
using System.Collections.Generic;

#if ANDROID
	using Microsoft.Maui.Controls;
#else
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;
#endif

namespace RD_AAOW
	{
	/// <summary>
	/// Возможные результаты инициализации
	/// </summary>
	public enum SolutionResults
		{
		/// <summary>
		/// Экземпляр не инициализирован
		/// </summary>
		NotInited = 1,

		/// <summary>
		/// Поиск был прерван пользователем
		/// </summary>
		SearchAborted = 2,

		/// <summary>
		/// Матрица загружена и готова к решению
		/// </summary>
		ReadyForSearch = 3,

		/// <summary>
		/// Задача решена
		/// </summary>
		SolutionFound = 0,

		/// <summary>
		/// Исходная матрица некорректна
		/// </summary>
		InitialMatrixIsInvalid = -1,

		/// <summary>
		/// Исходная матрица содержит противоречие
		/// </summary>
		InitialMatrixIsUnsolvable = -2,

		/// <summary>
		/// Решение не найдено
		/// </summary>
		NoSolutionsFound = -3
		}

	/// <summary>
	/// Возможные уровни сложности генерируемой матрицы
	/// </summary>
	public enum MatrixDifficulty
		{
		/// <summary>
		/// Простой
		/// </summary>
		Easy = 0,

		/// <summary>
		/// Средний
		/// </summary>
		Medium = 1,

		/// <summary>
		/// Сложный
		/// </summary>
		Hard = 2,

		/// <summary>
		/// Игровой режим отключён
		/// </summary>
		None = 255,
		}

	/// <summary>
	/// Возможные варианты проверки условий для элементов интерфейса
	/// </summary>
	public enum ConditionTypes
		{
		/// <summary>
		/// Элемент содержит значение, полученное в результате решения
		/// </summary>
		ContainsFoundValue,

		/// <summary>
		/// Элемент не содержит значения
		/// </summary>
		IsEmpty,

		/// <summary>
		/// Элемент содержит новое введённое значение
		/// </summary>
		ContainsNewValue,

		/// <summary>
		/// Ячейка уже была выбрана ранее
		/// </summary>
		SelectedCell,

		/// <summary>
		/// Элемент содержит определённое ранее значение
		/// </summary>
		ContainsOldValue,

		/// <summary>
		/// Элемент содержит ошибочное значение
		/// </summary>
		ContainsErrorValue,
		}

	/// <summary>
	/// Возможные варинаты настройки элемента интерфейса
	/// </summary>
	public enum PropertyTypes
		{
		/// <summary>
		/// Задать пустое значение
		/// </summary>
		EmptyValue,

		/// <summary>
		/// Задать цвет успешного решения задачи
		/// </summary>
		SuccessColor,

		/// <summary>
		/// Задать цвет ошибки
		/// </summary>
		ErrorColor,

		/// <summary>
		/// Задать цвет ранее известного значения
		/// </summary>
		OldColor,

		/// <summary>
		/// Задать цвет нового указанного значения
		/// </summary>
		NewColor,

		/// <summary>
		/// Невыбранная ячейка
		/// </summary>
		DeselectedCell,

		/// <summary>
		/// Выбранная ячейка
		/// </summary>
		SelectedCell,

		/// <summary>
		/// Другая кнопка интерфейса
		/// </summary>
		OtherButton,
		}

#if ANDROID

	/// <summary>
	/// Возможные расположения числовой клавиатуры
	/// </summary>
	public enum KeyboardPlacements
		{
		/// <summary>
		/// Не отображается
		/// </summary>
		None,

		/// <summary>
		/// Справа
		/// </summary>
		Right,

		/// <summary>
		/// Снизу
		/// </summary>
		Bottom,
		}

	/// <summary>
	/// Возможные режимы работы программы
	/// </summary>
	public enum AppModes
		{
		/// <summary>
		/// Только решение
		/// </summary>
		SolutionOnly,

		/// <summary>
		/// Игра
		/// </summary>
		Game,
		}

	/// <summary>
	/// Возможные цветовые схемы приложения
	/// </summary>
	public enum ColorSchemes
		{
		/// <summary>
		/// Светлая
		/// </summary>
		Light,

		/// <summary>
		/// Тёмная
		/// </summary>
		Dark,
		}

#endif

	/// <summary>
	/// Класс описывает основной функционал поиска решения
	/// </summary>
	public static class SudokuSolverMath
		{
		//	Данная программа использует двоичную систему счисления в качестве
		//	базового средства решения задачи. Т.е. если значения ячеек матрицы
		//	представить	в двоичном виде, то номера бит, установленных в 1, будут
		//	теми цифрами, которые в данный момент предполагается проставить в 
		//	соответствующую клетку судоку.
		//	Соответственно, если бит, равный 1, в значении ячейки остался лишь
		//	один, клетка считается вычисленной. При	этом очевидно, что её
		//	значение будет степенью числа 2. Определение этого состояния
		//	является критерием решения задачи

		#region Константы

		/// <summary>
		/// Размер стороны судоку
		/// </summary>
		public const UInt16 SideSize = 9;   // SquareSize ^ 2
		/*private const UInt16 SDS = 9;*/

		/// <summary>
		/// Полный размер судоку
		/// </summary>
		public const UInt16 FullSize = 81;  // SideSize ^ 2

		/// <summary>
		/// Размер стороны квадрата судоку
		/// </summary>
		public const UInt16 SquareSize = 3; // SideSize ^ 0.5

		// Полная флаговая переменная (123456789), используемая для решения
		private const UInt16 SDS_FULL = ((1 << SideSize) - 1);

		// Максимальное количество итераций, рассматриваемое как нормальный поиск
		private const UInt16 MAX_ITER = 50;

		// Контрольная строка заполнения ячейки
		private const string fileDataChecker = "123456789";

		// Признак незаполненной ячейки матрицы
		private const string EmptySign = " ";

		// Количество секунд, по истечении которого выполняется прерывание решения,
		// если активен режим сброса слишком затянувшихся вычислений
		private const uint dropSolutionLimit = 5;

		// Имена ключей, используемые для хранения настроек
#if ANDROID
		private const string keyboardPlacementsPar = "KeyboardPlacements";
		private const string appModePar = "AppMode";
		private const string colorSchemePar = "ColorScheme";
#endif

		private const string sudokuFieldPar = "SudokuField";
		private const string gameModePar = "GameMode";
		private const string gameScorePar = "GameScore";

		#endregion

		#region Поля

		/*// Линейный размер квадрата матрицы
		private static UInt16 SQ = (UInt16)Math.Sqrt (SDS);*/

		// Главная расчётная матрица
		private static UInt16[,] mtx = new UInt16[SideSize, SideSize];

		// Результирующая матрица
		private static Byte[,] resultMatrix;

		// Поля, обеспечивающие разбор сохранённой статистики игры
		private static uint[] gameScore = new uint[] { 0, 0, 0, 0, 0 };
		private const int gameScoreSize = 4;
		private static char[] gameScoreSplitter = new char[] { '\t' };

#if !ANDROID

		// Оператор нагрузочных процессов
		private static BackgroundWorker bw;

#endif

		// Флаг запроса прерывания вычислений
		private static bool stopRequested = false;

		// Флаг ограничения времени выполнения расчёта
		private static bool dropLongSolutions = false;

		// Временная метка начала вычисления
		private static DateTime searchStart;

		// Результат инициализации или решения
		private static SolutionResults currentStatus = SolutionResults.NotInited;

		// Цветовая схема
		private static Color[][] colors = new Color[][] {
			// Цвета новых, ошибочных и решённых ячеек
#if ANDROID
			new Color[] { Color.FromArgb ("#0000FF"), Color.FromArgb ("#FFFF40") },
			new Color[] { Color.FromArgb ("#C80000"), Color.FromArgb ("#FF4040") },
			new Color[] { Color.FromArgb ("#00C800"), Color.FromArgb ("#40FF40") },
#else
			new Color[] { Color.FromArgb (0, 0, 255), Color.FromArgb (255, 255, 64) },
			new Color[] { Color.FromArgb (200, 0, 0), Color.FromArgb (255, 64, 64) },
			new Color[] { Color.FromArgb (0, 200, 0), Color.FromArgb (64, 255, 64) },
#endif

			// Цвет имеющегося значения
			new Color[] { RDInterface.GetInterfaceColor (RDInterfaceColors.AndroidTextColor),
				RDInterface.GetInterfaceColor (RDInterfaceColors.MediumGrey) },

			// Цвет фона страницы или окна; цвет кнопок; цвет невыбранных и выбранных ячеек
#if ANDROID
			new Color[] { Color.FromArgb ("#FFFFE7"), Color.FromArgb ("#1C201C"), },
			new Color[] { Color.FromArgb ("#FFFFDE"), Color.FromArgb ("#222822"), },
			new Color[] { Color.FromArgb ("#C0FFFF"), Color.FromArgb ("#381C38"), },
			new Color[] { Color.FromArgb ("#00FFFF"), Color.FromArgb ("#603060"), },
#else
			new Color[] { Color.FromArgb (255, 255, 231), Color.FromArgb (28, 32, 28), },
			new Color[] { Color.FromArgb (255, 255, 222), Color.FromArgb (34, 40, 34), },
			new Color[] { Color.FromArgb (192, 255, 255), Color.FromArgb (56, 28, 56), },
			new Color[] { Color.FromArgb (0, 255, 255), Color.FromArgb (96, 48, 96), },
#endif
			};

		// Индекс текущей цветовой схемы
		private static int colorIndex = 0;

		// Файловые разделители
		private static string[] fileSplitters = new string[] { "\r", "\n", "\t", " ", ";" };

		// Уровень сложности генерируемой матрицы
		private static MatrixDifficulty difficulty;

		#endregion

		#region Свойства

		/// <summary>
		/// Возвращает текущий результат инициализации или решения задачи
		/// </summary>
		public static SolutionResults CurrentStatus
			{
			get
				{
				return currentStatus;
				}
			}

		/// <summary>
		/// Возвращает матрицу, полученную при решении задачи, или null, если решение не было найдено
		/// </summary>
		public static Byte[,] ResultMatrix
			{
			get
				{
				return resultMatrix;
				}
			}

		// Внутреннее свойство, возвращающее число известных значений по уровню сложности
		private static uint KnownValues
			{
			get
				{
				uint knownValues;
				switch (difficulty)
					{
					case MatrixDifficulty.Hard:
						knownValues = 24;
						break;

					case MatrixDifficulty.Medium:
						knownValues = 30;
						break;

					case MatrixDifficulty.Easy:
					default:
						knownValues = 36;
						break;
					}

				knownValues += (uint)RDGenerics.RND.Next (6);
				return knownValues;
				}
			}

#if ANDROID

		/// <summary>
		/// Возвращает или задаёт ориентацию элементов экрана
		/// </summary>
		public static KeyboardPlacements KeyboardPlacement
			{
			get
				{
				return (KeyboardPlacements)RDGenerics.GetSettings (keyboardPlacementsPar,
					(uint)KeyboardPlacements.Bottom);
				}
			set
				{
				RDGenerics.SetSettings (keyboardPlacementsPar, (uint)value);
				}
			}

		/// <summary>
		/// Возвращает или задаёт режим работы приложения
		/// </summary>
		public static AppModes AppMode
			{
			get
				{
				return (AppModes)RDGenerics.GetSettings (appModePar,
					(uint)AppModes.SolutionOnly);
				}
			set
				{
				RDGenerics.SetSettings (appModePar, (uint)value);
				}
			}

		/// <summary>
		/// Возвращает или задаёт цветовую схему приложения
		/// </summary>
		public static ColorSchemes ColorScheme
			{
			get
				{
				colorIndex = (int)RDGenerics.GetSettings (colorSchemePar, (uint)ColorSchemes.Light);
				return (ColorSchemes)colorIndex;
				}
			set
				{
				colorIndex = (int)value;
				RDGenerics.SetSettings (colorSchemePar, (uint)colorIndex);
				}
			}

#endif

		/// <summary>
		/// Возвращает или задаёт текущее состояние поля ввода
		/// </summary>
		public static string SudokuField
			{
			get
				{
				return RDGenerics.GetSettings (sudokuFieldPar, "");
				}
			set
				{
				RDGenerics.SetSettings (sudokuFieldPar, value);
				}
			}

		/// <summary>
		/// Возвращает или задаёт последний использованный игровой режим
		/// </summary>
		public static MatrixDifficulty GameMode
			{
			get
				{
				return (MatrixDifficulty)RDGenerics.GetSettings (gameModePar,
					(uint)MatrixDifficulty.None);
				}
			set
				{
				RDGenerics.SetSettings (gameModePar, (uint)value);
				}
			}

		/// <summary>
		/// Возвращает или задаёт суммарный счёт игрового режима
		/// </summary>
		public static uint TotalScore
			{
			get
				{
				return GetGameScore (0);
				}
			set
				{
				SetGameScore (0, value);
				}
			}

		/// <summary>
		/// Возвращает или задаёт количество собранных таблиц на простом уровне
		/// </summary>
		public static uint EasyScore
			{
			get
				{
				return GetGameScore (1);
				}
			set
				{
				SetGameScore (1, value);
				}
			}

		/// <summary>
		/// Возвращает или задаёт количество собранных таблиц на среднем уровне
		/// </summary>
		public static uint MediumScore
			{
			get
				{
				return GetGameScore (2);
				}
			set
				{
				SetGameScore (2, value);
				}
			}

		/// <summary>
		/// Возвращает или задаёт количество собранных таблиц на сложном уровне
		/// </summary>
		public static uint HardScore
			{
			get
				{
				return GetGameScore (3);
				}
			set
				{
				SetGameScore (3, value);
				}
			}

		/// <summary>
		/// Возвращает цвет фона страницы или окна для текущей цветовой схемы
		/// </summary>
		public static Color BackgroundColor
			{
			get
				{
				return colors[4][colorIndex];
				}
			}

		/*/// <summary>
		/// Возвращает цвет элементов страницы или окна для текущей цветовой схемы
		/// </summary>
		public static Color ElementsColor
			{
			get
				{
				return colors2[5][colorIndex];
				}
			}*/

		#endregion

		#region Вспомогательные методы

		// Определение чисел, являющихся степенью числа 2
		//
		// Если аргумент является степенью числа 2, функция возвращает true
		// (иначе - false)
		//
		private static bool IsPowOf2 (UInt16 Value)
			{
			// Защита
			if (Value == 0)
				return false;

			// Приведение к нечётной позиции
			uint v = Value;
			while (v % 2 == 0)
				v = v >> 1;

			// Нечётный бит должен быть единственным
			return (v == 1);
			}

		// Проверка на ошибочность предположения (повторения цифр)
		//
		// Возвращает:	true, если ошибки были найдены (есть дублирующиеся цифры);
		//				false, если ошибок нет
		//
		private static bool HasErrors ()
			{
			UInt16 s;

			// Горизонтальные линии
			for (UInt16 i = 0; i < SideSize; i++)
				{
				for (UInt16 j = s = 0; j < SideSize; j++)
					{
					// Чтобы определить, были ли повторы, создаётся переменная s, в
					// которую заносятся уже встреченные биты вычисленных ячеек.
					// Если ячейка обозначена как вычисленная (is_pow_2 == 1), но
					// операция дизъюнкции этой ячейки с s не изменяет s, значит,
					// эта цифра уже есть в строке, что означает наличие ошибки
					if (IsPowOf2 (mtx[i, j]) && ((s | mtx[i, j]) == s))
						return true;

					if (IsPowOf2 (mtx[i, j]))
						s |= mtx[i, j]; // Добавляет бит в s, если он получен из вычисленной ячейки
					}
				}

			// Вертикальные линии
			for (UInt16 j = 0; j < SideSize; j++)
				{
				for (UInt16 i = s = 0; i < SideSize; i++)
					{
					if (IsPowOf2 (mtx[i, j]) && ((s | mtx[i, j]) == s))
						return true;

					if (IsPowOf2 (mtx[i, j]))
						s |= mtx[i, j];
					}
				}

			// Квадраты
			for (UInt16 i = 0; i < SideSize; i++)
				{
				for (UInt16 j = s = 0; j < SideSize; j++)
					{
					// Пересчитываемые параметры, позволяющие использовать линейные коэффициенты для зигзагообразного движения
					/*UInt16 SQI = (UInt16)(j % SQ + (i % SQ) * SQ);
					UInt16 SQJ = (UInt16)(j / SQ + (i / SQ) * SQ);*/
					UInt16 SQI = (UInt16)(j % SquareSize + (i % SquareSize) * SquareSize);
					UInt16 SQJ = (UInt16)(j / SquareSize + (i / SquareSize) * SquareSize);

					if (IsPowOf2 (mtx[SQI, SQJ]) && ((s | mtx[SQI, SQJ]) == s))
						return true;

					if (IsPowOf2 (mtx[SQI, SQJ]))
						s |= mtx[SQI, SQJ];
					}
				}

			// Ошибок нет
			return false;
			}

		// Проверка матрицы на готовность в качестве конечного результата
		//
		// Возвращает true, если матрица получила законченный вид (нет «вилок»)
		//
		private static bool Finished ()
			{
			bool s = true;

			// Конечный результат считается полученным тогда, когда все
			// клетки вычислены, т.е. все их значения являются степенями
			// числа 2. Соответственно, переменная s не должна изменить
			// своего значения после всех конъюнкций
			for (UInt16 i = 0; i < SideSize; i++)
				{
				for (UInt16 j = 0; j < SideSize; j++)
					{
					s &= IsPowOf2 (mtx[i, j]);

					if (!s)
						return s;   // Быстрый выход
					}
				}

			return s;
			}

		// Пересчёт матрицы
		//
		// Возвращает:	1, если получен конечный результат;
		//				-1, если обнаружена ошибка в вычислениях;
		//				0, если простой прогонкой получить результат не удаётся
		//
		private static Int16 UpdateMatrix ()
			{
			UInt16 p, iterations = 0;

			while (!Finished () && (iterations < MAX_ITER))
				{
				// Счётчик итераций для определения момента завершения процесса
				iterations++;

				// Горизонтальные линии
				for (UInt16 i = 0; i < SideSize; i++)
					{
					// Здесь происходит удаление бит, неудовлетворяющих условиям задачи.
					// Для этого в числе p, заданном изначально значением 0x1FF,
					// «выкалываются» биты, уже встречающиеся в данной строке (далее -
					// столбце и квадрате). Причём, для этого используются только
					// вычисленные ячейки (is_pow_2 == true)
					// Затем полученное число конкатенируется с каждой ячейкой, где
					// вычисление ещё не было завершено (is_pow_2 == false)
					p = SDS_FULL;

					for (UInt16 j = 0; j < SideSize; j++)
						{
						if (IsPowOf2 (mtx[i, j]))
							p &= (UInt16)(~mtx[i, j]);  // Mij = 010000000; p = 111111111; [&=~] = 101111111
						}

					for (UInt16 j = 0; j < SideSize; j++)
						{
						if (!IsPowOf2 (mtx[i, j]))
							mtx[i, j] &= p;     // Mij = 010010001; p = 101111111; Mij' = 000010001
						}
					}

				// Вертикальные линии
				for (UInt16 j = 0; j < SideSize; j++)
					{
					p = SDS_FULL;

					for (UInt16 i = 0; i < SideSize; i++)
						{
						if (IsPowOf2 (mtx[i, j]))
							p &= (UInt16)(~mtx[i, j]);
						}

					for (UInt16 i = 0; i < SideSize; i++)
						{
						if (!IsPowOf2 (mtx[i, j]))
							mtx[i, j] &= p;
						}
					}

				// Квадраты
				for (UInt16 i = 0; i < SideSize; i++)
					{
					p = SDS_FULL;

					for (UInt16 j = 0; j < SideSize; j++)
						{
						/*UInt16 SQI = (UInt16)(j % SQ + (i % SQ) * SQ);
						UInt16 SQJ = (UInt16)(j / SQ + (i / SQ) * SQ);*/
						UInt16 SQI = (UInt16)(j % SquareSize + (i % SquareSize) * SquareSize);
						UInt16 SQJ = (UInt16)(j / SquareSize + (i / SquareSize) * SquareSize);

						if (IsPowOf2 (mtx[SQI, SQJ]))
							p &= (UInt16)(~mtx[SQI, SQJ]);
						}

					for (UInt16 j = 0; j < SideSize; j++)
						{
						/*UInt16 SQI = (UInt16)(j % SQ + (i % SQ) * SQ);
						UInt16 SQJ = (UInt16)(j / SQ + (i / SQ) * SQ);*/
						UInt16 SQI = (UInt16)(j % SquareSize + (i % SquareSize) * SquareSize);
						UInt16 SQJ = (UInt16)(j / SquareSize + (i / SquareSize) * SquareSize);

						if (!IsPowOf2 (mtx[SQI, SQJ]))
							mtx[SQI, SQJ] &= p;
						}
					}
				}

			// Прогонка дала ошибочный результат (имеются повторяющиеся значения)
			if (HasErrors ())
				return -1;

			// Прогонка дала конечный результат
			else if (iterations < MAX_ITER)
				return 1;

			// Прогонка прервана по превышению количества итераций
			else
				return 0;
			}

#if ANDROID

		/// <summary>
		/// Метод взводит флаг досрочного завершения операции
		/// </summary>
		public static void RequestStop ()
			{
			stopRequested = true;
			}

#else

		// Метод переносит флаг досрочного прерывания из оператора задачи в свойство stopRequested
		private static void CheckBW ()
			{
			if (bw.CancellationPending)
				stopRequested = true;
			}

#endif

		// Метод взводит или отключает пропуск решений, занимающих слишком много времени
		private static void SetDroppingLongSolutions (bool Enable)
			{
			dropLongSolutions = Enable;
			if (dropLongSolutions)
				searchStart = DateTime.Now;
			}

		// Функция изучения предположений
		//
		// Рекурсивная функция
		// Возвращает:	0, если результат получен
		//				-1, если ответ не был найден
		//				1, если поиск был отменён
		private static short Search ()
			{
			// Защита (запрос прерывания)
#if !ANDROID
			CheckBW ();
#endif

			if (stopRequested || dropLongSolutions &&
				((DateTime.Now - searchStart).Seconds >= dropSolutionLimit))
				{
				stopRequested = false;
				return 1;
				}

			// Создание копии исходной матрицы для данного предположения
			UInt16[,] mtc = (UInt16[,])mtx.Clone ();

			// Поиск первой невычисленной ячейки
			UInt16 i = 0, j = 0;
			for (i = 0; i < SideSize; i++)
				{
				for (j = 0; j < SideSize; j++)
					{
					if (!IsPowOf2 (mtx[i, j]))
						goto m1;
					}
				}

			// Решение уже найдено
			return 0;

			m1:
			for (UInt16 k = 0; k < SideSize; k++)
				{
				UInt16 v = (UInt16)(1 << k);
				if ((mtx[i, j] & v) == 0)   // Пропускать заведомо недопустимые значения
					continue;
				mtx[i, j] = v;

				// Прогонка матрицы с данным предположением
				// (копия матрицы с каждым вызовом функции является вынужденной мерой, т.к.
				// каждая прогонка почти полностью переписывает исходную матрицу)
				switch (UpdateMatrix ())
					{
					// Если получен конечный результат, функция его возвращает
					case 1:
						return 0;

					// Если результат требует уточнения
					case 0:
						// Делается новое предположение
						short res = Search ();
						switch (res)
							{
							// Если оно дало конечный результат, нужно вернуть его наверх
							case 0:

							// Если поиск отменён, нужно прервать всё дерево вызовов
							case 1:
								return res;

							// Если нет, нужно восстановить матрицу
							default:
								mtx = (UInt16[,])mtc.Clone ();
								break;
							}
						break;

					// Если получена ошибка при прогонке, нужно восстановить матрицу
					default:
						mtx = (UInt16[,])mtc.Clone ();
						break;
					}
				}

			// Правильных вариантов не найдено
			return -1;
			}

		// Метод выгружает полученную матрицу в результат
		private static void UploadResultMatrix ()
			{
			resultMatrix = new Byte[SideSize, SideSize];
			for (UInt16 i = 0; i < SideSize; i++)
				{
				for (UInt16 j = 0; j < SideSize; j++)
					if (IsPowOf2 (mtx[i, j]))
						resultMatrix[i, j] = (Byte)(Math.Log (mtx[i, j], 2) + 1);
					else
						resultMatrix[i, j] = 0;
				}
			}

#if !ANDROID

		// Образец метода, выполняющего длительные вычисления
		private static void DoSearch (object sender, DoWorkEventArgs e)
			{
			bw = (BackgroundWorker)sender;

			switch (Search ())
				{
				case -1:
					e.Result = (int)SolutionResults.NoSolutionsFound;
					return;

				case 1:
					e.Result = (int)SolutionResults.SearchAborted;
					return;
				}

			e.Result = (int)SolutionResults.SolutionFound;
			}

#endif

		// Метод выполняет заполнение матрицы неконфликтующими значениями.
		// Полученная матрица может не иметь решения, несмотря на первичный контроль
		private static bool FillMatrix ()
			{
			// Заполнение всех возможных вероятностей
			List<int> cells = new List<int> ();
			for (UInt16 i = 0; i < SideSize; i++)
				{
				for (UInt16 j = 0; j < SideSize; j++)
					{
					mtx[i, j] = SDS_FULL;
					cells.Add (i * SideSize + j);
					}
				}
			UInt16[,] mtc = (UInt16[,])mtx.Clone ();    // Чистая копия, не содержащая прогонов

			// Заполнение
			for (int k = 0; k < KnownValues; k++)
				{
				// Выбор заполняемой ячейки
				int idx = RDGenerics.RND.Next (cells.Count);
				int cell = cells[idx];
				cells.RemoveAt (idx);

				// Выбор значения для ячейки
				int i = cell / SideSize;
				int j = cell % SideSize;
				uint value = mtx[i, j];
				if (value == 0) // Такого не должно быть, но почему-то бывает
					return false;

				List<uint> digits = new List<uint> ();
				for (int b = 0; b < SideSize; b++)
					if ((value & (1 << b)) != 0)
						digits.Add ((uint)b + 1);

				value = digits[RDGenerics.RND.Next (digits.Count)];
				mtx[i, j] = (UInt16)(1 << ((int)value - 1));
				mtc[i, j] = mtx[i, j];

				// Допускать только матрицы, решаемые прогонами
				int res = UpdateMatrix ();
				if (res < 0)
					return false;
				}

			// Пока что успешно. Подмена просчитанной матрицы чистой копией
			mtx = (UInt16[,])mtc.Clone ();
			return true;
			}

		// Методы сохранения и загрузки статистики игрового режима
		private static uint GetGameScore (byte Item)
			{
			if (gameScore[gameScoreSize] != 1)
				{
				// Флаг инициализации
				gameScore[gameScoreSize] = 1;

				// Извлечение
				string v = RDGenerics.GetSettings (gameScorePar, "");
				string[] values = v.Split (gameScoreSplitter, StringSplitOptions.RemoveEmptyEntries);
				if (values.Length != gameScoreSize)
					return gameScore[Item];

				try
					{
					for (int i = 0; i < gameScoreSize; i++)
						gameScore[i] = uint.Parse (values[i]);
					}
				catch
					{
					for (int i = 0; i < gameScoreSize; i++)
						gameScore[i] = 0;
					}

				// Успешно
				}

			return gameScore[Item];
			}

		private static void SetGameScore (byte Item, uint Value)
			{
			gameScore[Item] = Value;

			string line = "";
			string sp = gameScoreSplitter[0].ToString ();

			for (int i = 0; i < gameScoreSize - 1; i++)
				line += (gameScore[i].ToString () + sp);
			line += gameScore[gameScoreSize - 1].ToString ();

			RDGenerics.SetSettings (gameScorePar, line);
			}

		#endregion

		/// <summary>
		/// Метод инициализирует матрицу и готовит класс к выполнению решения
		/// </summary>
		/// <param name="SourceMatrix">Исходная таблица чисел; должна иметь высоту и ширину, равные 9;
		/// из значений извлекаются только младшие разряды; нулевые значения рассматриваются как те, 
		/// которые нужно найти</param>
		public static SolutionResults InitializeSolution (Byte[,] SourceMatrix)
			{
			// Контроль
			if ((SourceMatrix == null) || (SourceMatrix.GetLength (0) != SideSize) ||
				(SourceMatrix.GetLength (1) != SideSize))
				{
				currentStatus = SolutionResults.InitialMatrixIsInvalid;
				return currentStatus;
				}

			// Инициализация
			for (UInt16 i = 0; i < SideSize; i++)
				{
				for (UInt16 j = 0; j < SideSize; j++)
					{
					Byte digit = (Byte)(SourceMatrix[i, j] % 10);
					if (digit != 0)
						mtx[i, j] = (UInt16)(1 << (digit - 1));
					else
						mtx[i, j] = SDS_FULL;
					}
				}

			///////////////////////////////////////////////////////////////////////////
			// Подготовка матрицы
			//		Здесь выполняется первый прогон матрицы, предполагающий удаление
			//		из клеток с заданным значением 0x1FF лишних бит. Для этого
			//		проверяются вертикальные и горизонтальные линии, а также квадраты.
			//		Если эта процедура вызывает ошибку в вычислениях, значит, исходные
			//		данные неверны
			if (UpdateMatrix () == -1)
				{
				currentStatus = SolutionResults.InitialMatrixIsUnsolvable;
				return currentStatus;
				}

			// Готово к решению
			currentStatus = SolutionResults.ReadyForSearch;
			return currentStatus;
			}

#if ANDROID

		/// <summary>
		/// Метод выполняет поиск решения для ранее инициализированной матрицы
		/// </summary>
		/// <returns>Возвращает результат поиска или статус NotInited, если не была выполнена инициализация</returns>
		public static bool FindSolution ()
			{
			// Контроль
			if (currentStatus != SolutionResults.ReadyForSearch)
				{
				currentStatus = SolutionResults.NotInited;
				return false;
				}

			// Метод предположений
			SetDroppingLongSolutions (false);
			switch (Search ())
				{
				case -1:
					currentStatus = SolutionResults.NoSolutionsFound;
					return false;

				case 1:
					currentStatus = SolutionResults.SearchAborted;
					return false;
				}

			// Успешно. Возврат результата
			UploadResultMatrix ();
			currentStatus = SolutionResults.SolutionFound;
			return true;
			}

#else

		/// <summary>
		/// Метод выполняет поиск решения для ранее инициализированной матрицы
		/// </summary>
		/// <returns>Возвращает результат поиска или статус NotInited, если не была выполнена инициализация</returns>
		public static SolutionResults FindSolution ()
			{
			// Контроль
			if (currentStatus != SolutionResults.ReadyForSearch)
				{
				currentStatus = SolutionResults.NotInited;
				return currentStatus;
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
			SetDroppingLongSolutions (false);
			RDInterface.RunWork (DoSearch, null, RDLocale.GetText ("DoingSearch"),
				RDRunWorkFlags.CaptionInTheMiddle | RDRunWorkFlags.AllowOperationAbort);
			SolutionResults res = (SolutionResults)RDInterface.WorkResultAsInteger;

			switch (res)
				{
				case SolutionResults.NoSolutionsFound:
				case SolutionResults.SearchAborted:
					currentStatus = res;
					return currentStatus;
				}

			// Успешно. Возврат результата
			UploadResultMatrix ();
			currentStatus = SolutionResults.SolutionFound;
			return currentStatus;
			}

#endif

		/// <summary>
		/// Метод извлекает матрицу из содержимого файла и возвращает её, если это возможно,
		/// в виде сплошной строки длиной FullSize
		/// </summary>
		/// <param name="FileContents">Содержимое текстового файла</param>
		public static string ParseMatrixFromFile (string FileContents)
			{
			// Обработка
			if (string.IsNullOrWhiteSpace (FileContents))
				return "";
			string data = FileContents;

			for (int i = 0; i < fileSplitters.Length; i++)
				data = data.Replace (fileSplitters[i], "");

			if (data.Length < /*SudokuSideSize * SudokuSideSize*/ FullSize)
				return "";

			// Загрузка
			string resultLine = "";
			for (int i = 0; i < /*SudokuSideSize * SudokuSideSize*/ FullSize; i++)
				{
				string c = data[i].ToString ();
				if (fileDataChecker.Contains (c))
					resultLine += c;
				else
					resultLine += EmptySign;
				}

			return resultLine;
			}

		/// <summary>
		/// Метод оформляет матрицу для сохранения в файл
		/// </summary>
		/// <param name="Line">Сплошная строка значений матрицы</param>
		public static string BuildMatrixToSave (string Line)
			{
			string file = "";
			/*int sqrt = (int)Math.Sqrt (SudokuSideSize);
			int cubedSqrt = sqrt * sqrt * sqrt;*/

			for (int i = 1; i <= Line.Length; i++)
				{
				file += Line[i - 1].ToString ().Replace (EmptySign, "-");

				/*if (i % cubedSqrt == 0)*/
				if ((i % (SquareSize * SideSize)) == 0)
					file += RDLocale.RNRN;
				/*else if (i % SudokuSideSize == 0)*/
				else if ((i % SideSize) == 0)
					file += RDLocale.RN;
				/*else if (i % sqrt == 0)*/
				else if ((i % SquareSize) == 0)
					file += " ";
				}

			return file;
			}

		/// <summary>
		/// Метод устанавливает сложность для метода генерации матриц судоку
		/// </summary>
		/// <param name="Difficulty">Требуемый уровень сложности</param>
		public static void SetGenerationDifficulty (MatrixDifficulty Difficulty)
			{
			difficulty = Difficulty;
			}

#if ANDROID

		/// <summary>
		/// Метод формирует матрицу судоку с указанным уровнем сложности
		/// </summary>
		public static bool GenerateMatrix ()
			{
			// Поиск решаемой матрицы
			bool solved = false;
			UInt16[,] mtc = (UInt16[,])mtx.Clone ();

			while (!solved)
				{
				// Генерация потенциально решаемой матрицы
				solved = true;
				while (!FillMatrix ())
					;

				// Поиск решения для полученной матрицы
				mtc = (UInt16[,])mtx.Clone ();

				// Слишком долгие решения игнорируются
				SetDroppingLongSolutions (true);

				// Любая проблема рассматривается как дефект матрицы и требует повторной генерации
				switch (Search ())
					{
					case -1:
						currentStatus = SolutionResults.NoSolutionsFound;
						solved = false;
						break;

					case 1:
						currentStatus = SolutionResults.SearchAborted;
						solved = false;
						break;
					}
				}

			// Успешно. Возврат результата
			mtx = (UInt16[,])mtc.Clone ();
			UploadResultMatrix ();
			currentStatus = SolutionResults.SolutionFound;
			return true;
			}

#else

		/// <summary>
		/// Метод формирует матрицу судоку с указанным уровнем сложности
		/// </summary>
		public static void GenerateMatrix ()
			{
			// Поиск решаемой матрицы
			bool solved = false;
			UInt16[,] mtc = (UInt16[,])mtx.Clone ();

			while (!solved)
				{
				// Генерация потенциально решаемой матрицы
				solved = true;
				while (!FillMatrix ())
					;

				// Поиск решения для полученной матрицы
				mtc = (UInt16[,])mtx.Clone ();

				// Слишком долгие решения игнорируются
				SetDroppingLongSolutions (true);
				RDInterface.RunWork (DoSearch, null, RDLocale.GetText ("DoingSearch"),
					RDRunWorkFlags.CaptionInTheMiddle);
				SolutionResults res = (SolutionResults)RDInterface.WorkResultAsInteger;

				// Любая проблема рассматривается как дефект матрицы и требует повторной генерации
				switch (res)
					{
					case SolutionResults.NoSolutionsFound:
					case SolutionResults.SearchAborted:
						solved = false;
						break;
					}
				}

			// Успешно. Возврат результата
			mtx = (UInt16[,])mtc.Clone ();
			UploadResultMatrix ();
			currentStatus = SolutionResults.SolutionFound;
			}

#endif

		/// <summary>
		/// Метод проверяет указанное условие на истинность
		/// </summary>
		/// <param name="InterfaceElement">Элемент интерфейса, для которого выполняется контроль</param>
		/// <param name="Condition">Проверяемое условие</param>
		public static bool CheckCondition (Button InterfaceElement, ConditionTypes Condition)
			{
			string text = InterfaceElement.Text;
#if ANDROID
			Color textColor = InterfaceElement.TextColor;
			Color backColor = InterfaceElement.BackgroundColor;
#else
			Color textColor = InterfaceElement.ForeColor;
			Color backColor = InterfaceElement.BackColor;
#endif

			// Проверка условия
			for (int i = 0; i < colors[0].Length; i++)
				{
				switch (Condition)
					{
					case ConditionTypes.ContainsFoundValue:
						if (textColor == colors[2][i])
							return true;
						break;

					case ConditionTypes.ContainsNewValue:
						if (textColor == colors[0][i])
							return true;
						break;

					case ConditionTypes.IsEmpty:
						return (text == EmptySign);

					case ConditionTypes.SelectedCell:
						if (backColor == colors[7][i])
							return true;
						break;

					case ConditionTypes.ContainsOldValue:
						if (textColor == colors[3][i])
							return true;
						break;

					case ConditionTypes.ContainsErrorValue:
						if (textColor == colors[1][i])
							return true;
						break;
					}
				}

			// Неприменимое условие
			return false;
			}

		/// <summary>
		/// Метод настраивает указанный параметр элемента интерфейса
		/// </summary>
		/// <param name="InterfaceElement">Элемент интерфейса, для которого выполняется настройка</param>
		/// <param name="Property">Проверяемое условие</param>
		public static void SetProperty (Button InterfaceElement, PropertyTypes Property)
			{
			// Проверка условия
			bool setForeColor = false, setBackColor = false;
			Color color = colors[3][colorIndex];

			switch (Property)
				{
				case PropertyTypes.EmptyValue:
					InterfaceElement.Text = EmptySign;
					break;

				case PropertyTypes.SuccessColor:
					color = colors[2][colorIndex];
					setForeColor = true;
					break;

				case PropertyTypes.ErrorColor:
					color = colors[1][colorIndex];
					setForeColor = true;
					break;

				case PropertyTypes.NewColor:
					color = colors[0][colorIndex];
					setForeColor = true;
					break;

				case PropertyTypes.OldColor:
					setForeColor = true;
					break;

				case PropertyTypes.DeselectedCell:
					color = colors[6][colorIndex];
					setBackColor = true;
					break;

				case PropertyTypes.SelectedCell:
					color = colors[7][colorIndex];
					setBackColor = true;
					break;

				case PropertyTypes.OtherButton:
					color = colors[5][colorIndex];
					setBackColor = true;
					break;
				}

			if (setForeColor)
#if ANDROID
				InterfaceElement.TextColor = color;
#else
				InterfaceElement.ForeColor = color;
#endif

			else if (setBackColor)
#if ANDROID
				InterfaceElement.BackgroundColor = color;
#else
				InterfaceElement.BackColor = color;
#endif
			}
		}
	}
