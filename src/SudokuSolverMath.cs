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
		EmptyValue = 0x0001,

		/// <summary>
		/// Задать цвет успешного решения задачи
		/// </summary>
		SuccessColor = 0x0010,

		/// <summary>
		/// Задать цвет ошибки
		/// </summary>
		ErrorColor = 0x0020,

		/// <summary>
		/// Задать цвет ранее известного значения
		/// </summary>
		OldColor = 0x0040,

		/// <summary>
		/// Задать цвет нового указанного значения
		/// </summary>
		NewColor = 0x0080,

		/// <summary>
		/// Маска изменения цвета текста
		/// </summary>
		TextColorMask = 0x00F0,

		/// <summary>
		/// Невыбранная ячейка
		/// </summary>
		DeselectedCell = 0x0100,

		/// <summary>
		/// Выбранная ячейка
		/// </summary>
		SelectedCell = 0x0200,

		/// <summary>
		/// Другая кнопка интерфейса
		/// </summary>
		OtherButton = 0x0800,

		/// <summary>
		/// Простреливаемые ячейки
		/// </summary>
		AffectedCell = 0x0400,

		/// <summary>
		/// Маска изменения цвета фона
		/// </summary>
		BackColorMask = 0x0F00,
		}

	/// <summary>
	/// Возможные варианты вознаграждения
	/// </summary>
	public enum ScoreTypes
		{
		/// <summary>
		/// Текущий выигрыш
		/// </summary>
		RegularWinning,

		/// <summary>
		/// Штраф
		/// </summary>
		Penalty,

		/// <summary>
		/// Окончание игры
		/// </summary>
		GameCompletion,
		}

	/// <summary>
	/// Возможные варианты представления значений ячеек
	/// </summary>
	public enum CellsAppearances
		{
		/// <summary>
		/// Цифры
		/// </summary>
		Digits,

		/// <summary>
		/// Латинские строчные буквы
		/// </summary>
		LatinLowercase,

		/// <summary>
		/// Кириллические строчные буквы
		/// </summary>
		CyrillicLowercase,

		/// <summary>
		/// Греческие строчные буквы
		/// </summary>
		GreekLowercase,

		/// <summary>
		/// Римские цифры
		/// </summary>
		RomanNumerals,

#if ANDROID

		/// <summary>
		/// Точки
		/// </summary>
		Dots,

		/// <summary>
		/// Радуга
		/// </summary>
		Rainbow,

		/// <summary>
		/// Еда
		/// </summary>
		Food,

#endif
		};

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
		/// Светлая серая
		/// </summary>
		LightGrey,

		/// <summary>
		/// Светлая жёлтая
		/// </summary>
		LightYellow,

		/// <summary>
		/// Тёмная серая
		/// </summary>
		DarkGrey,

		/// <summary>
		/// Тёмная фиолетовая
		/// </summary>
		DarkPurple,
		}

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

		// Признак незаполненной ячейки матрицы
		private const string EmptySign = " ";

		// Количество секунд, по истечении которого выполняется прерывание решения,
		// если активен режим сброса слишком затянувшихся вычислений
		private const uint dropSolutionLimit = 5;

		// Смещения полей хранения выигрышей
		private const byte gameScore_TotalScore = 0;
		private const byte gameScore_WinsBase = 1;
		private const byte gameScore_ChainBase = 6;
		private const byte gameScore_TimeBase = 9;
		private const byte gameScore_AchiBase = 12;

		// Длина массива храрения выигрышей
		private const int gameScoreSize = 14;

		// Ограничение поля Лучшее время (не более недели)
		private const uint gameScore_TimeLimit = 60 * 60 * 24 * 7;

		// Имена ключей, используемые для хранения настроек
#if ANDROID
		private const string replaceBalloonsPar = "ReplaceBalloons";
#endif

		private const string appModePar = "AppMode";
		private const string sudokuFieldPar = "SudokuField";
		private const string gameModePar = "GameMode";
		private const string gameScorePar = "GameScore";
		private const string colorSchemePar = "ColorScheme";
		private const string cellsAppearancePar = "CellsAppearance";
		private const string gameStartDatePar = "GameStartDate";
		private const string showAffectedCellsPar = "ShowAffectedCells";

		#endregion

		#region Поля

		// Главная расчётная матрица
		private static UInt16[,] mtx = new UInt16[SideSize, SideSize];

		// Результирующая матрица
		private static Byte[,] resultMatrix;

		// Поля, обеспечивающие разбор сохранённой статистики игры
		private static uint[] gameScore;

		// Разделитель полей в строке хранения выигрышей
		private static char[] gameScoreSplitter = ['\t'];

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
		private static string[][] colorsNames = [
			["Светлая серая", "Light grey"],
			["Светлая жёлтая", "Light yellow"],
			["Тёмная серая", "Dark grey"],
			["Тёмная фиолетовая", "Dark violet"],
			];
		private static byte[][][] colorsV4 = [
			// Цвет новых ячеек
			[ [0, 0, 255], [0, 0, 255], [255, 255, 64], [255, 255, 64], ],

			// Цвет ошибочных ячеек
			[ [200, 0, 0], [200, 0, 0], [255, 64, 64], [255, 64, 64], ],

			// Цвет решённых ячеек
			[ [0, 180, 0], [0, 180, 0], [64, 255, 64], [64, 255, 64], ],

			// Цвет имеющегося значения
			[ [32, 32, 32], [32, 48, 32], [196, 196, 196], [172, 160, 172], ],

			// Цвет фона страницы или окна
			[ [231, 231, 231], [255, 255, 231], [28, 28, 28], [30, 28, 40], ],
			
			// Цвет обычных кнопок
			[ [240, 240, 240], [255, 255, 222], [34, 34, 34], [37, 34, 40], ],

			// Цвет невыбранных ячеек
			[ [208, 208, 208], [208, 255, 255], [56, 56, 56], [56, 28, 56], ],

			// Цвет выбранных ячеек
			[ [156, 156, 156], [0, 240, 255], [120, 120, 120], [112, 60, 112], ],

			// Цвет простреливаемых ячеек
			[ [184, 184, 184], [116, 255, 255], [84, 84, 84], [80, 42, 80], ],
			];

		// Индекс текущей цветовой схемы
		private static int colorIndex = 0;

		// Файловые разделители
		private static string[] fileSplitters = ["\r", "\n", "\t", " ", ";"];

		// Уровень сложности генерируемой матрицы
		private static MatrixDifficulty difficulty;

		// Варианты представления значений в ячейках
		private static List<List<string>> cellsApps = [
			[ "1", "2", "3", "4", "5", "6", "7", "8", "9" ],
			[ "a", "b", "c", "d", "e", "f", "g", "h", "i" ],
			[ "а", "б", "в", "г", "д", "е", "ж", "з", "и" ],
			[ "α", "β", "γ", "δ", "ε", "ζ", "η", "θ", "ι" ],
			[ "Ⅰ", "Ⅱ", "Ⅲ", "Ⅳ", "Ⅴ", "Ⅵ", "Ⅶ", "Ⅷ", "Ⅸ" ],
#if ANDROID
			[
			"     \n  ●  \n     ",
			"    ●\n     \n●    ",
			"    ●\n  ●  \n●    ",
			"●   ●\n     \n●   ●",
			"●   ●\n  ●  \n●   ●",
			"●   ●\n●   ●\n●   ●",
			"●   ●\n● ● ●\n●   ●",
			"● ● ●\n●   ●\n● ● ●",
			"● ● ●\n● ● ●\n● ● ●",
			],
			[ "❤️", "🧡", "💛", "💚", "🩵", "💙", "💜", "🩷", "🤍" ],
			[ "🍎", "🍊", "🍋", "🍏", "🧊", "🫐", "🍇", "🍗", "🥚" ],
#endif
			];
		private static string[][] cellsAppsNames = [
			[ "Цифры", "Digits" ],
			[ "Латинские буквы", "Latin letters" ],
			[ "Русские буквы", "Cyrillic letters" ],
			[ "Греческие буквы", "Greek letters" ],
			[ "Римские цифры", "Roman numerals" ],
#if ANDROID
			[ "Точки", "Dots" ],
			[ "Радуга", "Rainbow" ],
			[ "Еда", "Food" ],
#endif
			];

#if ANDROID

		private static double[] cellsAppsFontSizes = [
			1.25,
			1.25,
			1.25,
			1.25,
			1.25,
			0.55,
			1.55,
			1.55,
			];

#endif

		// Индекс текущего представления значений в ячейках
		private static int cellsAppIndex = 0;

		// Счётчики успешных и ошибочных проверок
		// (заглушены, чтобы не допускать сброса перезапуском приложения)
		private static uint successfulChecks = 100;
		private static uint failedChecks = 100;

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
		/// Возвращает или задаёт флаг замены всплывающих оповещений на полноценные сообщения
		/// </summary>
		public static bool ReplaceBalloons
			{
			get
				{
				return RDGenerics.GetSettings (replaceBalloonsPar, false);
				}
			set
				{
				RDGenerics.SetSettings (replaceBalloonsPar, value);
				}
			}

#endif

		/// <summary>
		/// Возвращает или задаёт флаг подсветки простреливаемых ячеек
		/// </summary>
		public static bool ShowAffectedCells
			{
			get
				{
				return RDGenerics.GetSettings (showAffectedCellsPar, true);
				}
			set
				{
				RDGenerics.SetSettings (showAffectedCellsPar, value);
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
					(uint)AppModes.Game);
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
				colorIndex = (int)RDGenerics.GetSettings (colorSchemePar, (uint)ColorSchemes.LightYellow);
				if (colorIndex > colorsNames.Length)
					colorIndex = 0;
				return (ColorSchemes)colorIndex;
				}
			set
				{
				colorIndex = (int)value;
				RDGenerics.SetSettings (colorSchemePar, (uint)colorIndex);
				}
			}

		/// <summary>
		/// Возвращает или задаёт представление значений в ячейках
		/// </summary>
		public static CellsAppearances CellsAppearance
			{
			get
				{
				cellsAppIndex = (int)RDGenerics.GetSettings (cellsAppearancePar, (uint)CellsAppearances.Digits);
				if (cellsAppIndex >= cellsApps.Count)
					cellsAppIndex = 0;
				return (CellsAppearances)cellsAppIndex;
				}
			set
				{
				cellsAppIndex = (int)value;
				RDGenerics.SetSettings (cellsAppearancePar, (uint)cellsAppIndex);
				}
			}

		/// <summary>
		/// Возвращает или задаёт текущее состояние поля ввода в представлении «цифры».
		/// Всегда возвращает строку длиной FullSize
		/// </summary>
		public static string SudokuField
			{
			get
				{
				// Защита верхних вызовов
				string line = RDGenerics.GetSettings (sudokuFieldPar, "");
#if !ANDROID
				try
					{
					byte[] conv = Convert.FromBase64String (line.Replace ('А', 'A').Replace ('М', 'M'));
					line = RDGenerics.GetEncoding (RDEncodings.UTF8).GetString (conv);
					}
				catch
					{
					line = "";
					}
#endif

				if (line.Length == FullSize)
					return line;

				return EmptySign.PadLeft (FullSize, EmptySign[0]);
				}
			set
				{
				string line = value;
#if !ANDROID
				byte[] conv = RDGenerics.GetEncoding (RDEncodings.UTF8).GetBytes (line);
				line = Convert.ToBase64String (conv, Base64FormattingOptions.None);
				line = line.Replace ('A', 'А').Replace ('M', 'М');
#endif

				RDGenerics.SetSettings (sudokuFieldPar, line);
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
		/// Возвращает цвет фона страницы или окна для текущей цветовой схемы
		/// </summary>
		public static Color BackgroundColor
			{
			get
				{
				_ = ColorScheme;    // Загрузка значения
				return BytesToColor (colorsV4[4][colorIndex]);
				}
			}

		private static Color BytesToColor (byte[] Bytes)
			{
#if ANDROID
			uint v = ((uint)Bytes[0] << 16) | ((uint)Bytes[1] << 8) | (uint)Bytes[2];
			return Color.FromArgb ("#" + v.ToString ("X6"));
#else
			return Color.FromArgb (Bytes[0], Bytes[1], Bytes[2]);
#endif
			}

		/// <summary>
		/// Возвращает названия цветовых схем для текущего языка интерфейса
		/// </summary>
		public static string[] ColorSchemesNames
			{
			get
				{
				int idx = (int)RDLocale.CurrentLanguage;
				List<string> res = [];

				for (int i = 0; i < colorsNames.Length; i++)
					res.Add (colorsNames[i][idx]);
				return res.ToArray ();
				}
			}

		/// <summary>
		/// Возвращает названия представлений ячеек для текущего языка интерфейса
		/// </summary>
		public static string[] CellsAppearancesNames
			{
			get
				{
				int idx = (int)RDLocale.CurrentLanguage;
				List<string> res = [];

				for (int i = 0; i < cellsAppsNames.Length; i++)
					res.Add (cellsAppsNames[i][idx]);
				return res.ToArray ();
				}
			}


#if ANDROID

		/// <summary>
		/// Возвращает множитель размера шрифта для текущего представления значений в ячейках
		/// </summary>
		public static double CellsAppearancesFontSize
			{
			get
				{
				_ = CellsAppearance;    // Загрузка значения
				return cellsAppsFontSizes[cellsAppIndex] * RDInterface.MasterFontSize;
				}
			}

		/// <summary>
		/// Возвращает флаг жирного начертания для текущего представления значений в ячейках
		/// </summary>
		public static bool CellsAppearancesBoldFont
			{
			get
				{
				_ = CellsAppearance;
				return (cellsAppsFontSizes[cellsAppIndex] < 1.5);
				}
			}

#endif

		// Возвращает или задаёт временной штамп начала игры
		private static DateTime GameStartDate
			{
			get
				{
				string date = RDGenerics.GetSettings (gameStartDatePar, "");
				try
					{
					return DateTime.Parse (date, RDLocale.GetCulture (RDLanguages.en_us));
					}
				catch
					{
					return new DateTime (2025, 1, 1, 0, 0, 0);
					}
				}
			set
				{
				RDGenerics.SetSettings (gameStartDatePar,
					value.ToString (RDLocale.GetCulture (RDLanguages.en_us)));
				}
			}

		/// <summary>
		/// Возвращает список из 10 полей статистики в следующем порядке:
		/// - общий выигрыш игрока
		/// - число завершённых простых игр
		/// - число завершённых средних игр
		/// - число завершённых сложных игр
		/// - лучшее время прохождения среди простых игр
		/// - лучшее время прохождения среди средних игр
		/// - лучшее время прохождения среди сложных игр
		/// - самая длинная цепочка без проверок среди простых игр
		/// - самая длинная цепочка без проверок среди средних игр
		/// - самая длинная цепочка без проверок среди сложных игр
		/// - число достижений «угадай с трёх раз»
		/// - число достижений «ни одной ошибки»
		/// </summary>
		public static string[] StatsValues
			{
			get
				{
				List<string> values = [];
				values.Add (GetGameScore (gameScore_TotalScore).ToString ("#,#0"));
				for (byte i = gameScore_WinsBase; i < gameScore_WinsBase + 3; i++)
					values.Add (GetGameScore (i).ToString ());

				for (byte i = gameScore_TimeBase; i < gameScore_TimeBase + 3; i++)
					{
					uint bestTime = GetGameScore (i);
					bool showTime = (bestTime <= gameScore_TimeLimit);

					if (!showTime)
						{
						values.Add ("—");
						continue;
						}

					string s = (bestTime % 60).ToString ("D2");
					bestTime /= 60;
					string m = (bestTime % 60).ToString ("D2");
					bestTime /= 60;
					string h = bestTime.ToString ();
					values.Add (h + ":" + m + ":" + s);
					}

				for (byte i = gameScore_ChainBase; i < gameScore_ChainBase + 3; i++)
					values.Add (GetGameScore (i).ToString ());

				for (byte i = gameScore_AchiBase; i < gameScore_AchiBase + AchievementsCount; i++)
					values.Add (GetGameScore (i).ToString ());

				return values.ToArray ();
				}
			}

		/// <summary>
		/// Возвращает число доступных достижений
		/// </summary>
		public static uint AchievementsCount
			{
			get
				{
				return gameScoreSize - gameScore_AchiBase;
				}
			}

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
						UInt16 SQI = (UInt16)(j % SquareSize + (i % SquareSize) * SquareSize);
						UInt16 SQJ = (UInt16)(j / SquareSize + (i / SquareSize) * SquareSize);

						if (IsPowOf2 (mtx[SQI, SQJ]))
							p &= (UInt16)(~mtx[SQI, SQJ]);
						}

					for (UInt16 j = 0; j < SideSize; j++)
						{
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
			List<int> cells = [];
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

				List<uint> digits = [];
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
			if (gameScore == null)
				{
				// Инициализация
				gameScore = new uint[gameScoreSize];
				for (int i = 0; i < gameScoreSize; i++)
					{
					if ((i < gameScore_TimeBase) || (i >= gameScore_AchiBase))
						gameScore[i] = 0;   // Чем больше, тем лучше
					else
						gameScore[i] = uint.MaxValue;   // Наоборот
					}

				// Извлечение
				string line = RDGenerics.GetSettings (gameScorePar, "");
#if !ANDROID
				try
					{
					byte[] conv = Convert.FromBase64String (line.Replace ('А', 'A'));
					line = RDGenerics.GetEncoding (RDEncodings.Unicode32).GetString (conv);
					}
				catch
					{
					line = "";
					}
#endif
				string[] values = line.Split (gameScoreSplitter, StringSplitOptions.RemoveEmptyEntries);
				if (values.Length < 4)
					return gameScore[Item];

				// Поля 4 и 5 забракованы ошибочной версией 5.0.0.0, не используются
				for (int i = 0; i < gameScoreSize; i++)
					try
						{
						gameScore[i] = uint.Parse (values[i]);
						}
					catch
						{
						if ((i < gameScore_TimeBase) || (i >= gameScore_AchiBase))
							gameScore[i] = 0;   // Чем больше, тем лучше
						else
							gameScore[i] = uint.MaxValue;   // Наоборот
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

#if !ANDROID
			byte[] conv = RDGenerics.GetEncoding (RDEncodings.Unicode32).GetBytes (line);
			line = Convert.ToBase64String (conv, Base64FormattingOptions.None).Replace ('A', 'А');
#endif
			RDGenerics.SetSettings (gameScorePar, line);
			}

		// Метод рассчитывает выигрыш по типу расчёта и количеству найденных ячеек
		private static uint GetScore (ScoreTypes ScoreType, uint Value)
			{
			// Контроль
			if (GameMode == MatrixDifficulty.None)
				return 0;
			byte baseOffset = (byte)GameMode;
			uint multiplier = (uint)(baseOffset + 1);
			byte item;
			uint v;

			switch (ScoreType)
				{
				// Обычный выигрыш
				case ScoreTypes.RegularWinning:
				default:
					item = (byte)(gameScore_ChainBase + baseOffset);
					v = GetGameScore (item);
					if (Value > v)
						SetGameScore (item, Value);

					return multiplier * Value * Value;

				// Штраф
				case ScoreTypes.Penalty:
					return 10 * (4 - multiplier);

				// Победа
				case ScoreTypes.GameCompletion:
					// Количество выигранных игр
					item = (byte)(gameScore_WinsBase + baseOffset);
					v = GetGameScore (item);
					SetGameScore (item, v + 1);

					// Лучшее время
					item = (byte)(gameScore_TimeBase + baseOffset);
					v = GetGameScore (item);

					double seconds = (DateTime.Now - GameStartDate).TotalSeconds;
					if ((seconds <= gameScore_TimeLimit) && (seconds < v))
						SetGameScore (item, (uint)seconds);

					// Проверка достижений
					for (uint i = 0; i < AchievementsCount; i++)
						{
						if (CheckAchievement (i))
							{
							item = (byte)(gameScore_AchiBase + i);
							v = GetGameScore (item);
							SetGameScore (item, v + 1);
							}
						}

					return 1000 * multiplier;
				}
			}

		/// <summary>
		/// Метод обновляет суммарный выигрыш
		/// </summary>
		/// <param name="Penalty">Режим штрафа</param>
		/// <param name="Value">Величина выигрыша или штрафа</param>
		public static void UpdateGameScore (bool Penalty, uint Value)
			{
			// Загрузка значения
			uint v = GetGameScore (gameScore_TotalScore);

			// Обновление
			if (Penalty)
				{
				if (v > Value)
					v -= Value;
				else
					v = 0;

				failedChecks++;
				}
			else
				{
				v += Value;

				successfulChecks++;
				}

			// Запись значения
			SetGameScore (gameScore_TotalScore, v);
			}

		/// <summary>
		/// Метод проверяет наличие факта достижения по его номеру
		/// </summary>
		/// <param name="AchiNumber">Номер достижения</param>
		/// <returns>Возвращает false, если достижение не выполнено, или номер указан некорректно</returns>
		public static bool CheckAchievement (uint AchiNumber)
			{
			switch (AchiNumber)
				{
				// Угадай с трёх раз
				case 0:
					return (successfulChecks + failedChecks <= (uint)difficulty + 1);

				// Без ошибок
				case 1:
					return (failedChecks == 0);
				}

			return false;
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

			if (data.Length < FullSize)
				return "";

			// Загрузка
			string resultLine = "";
			for (int i = 0; i < FullSize; i++)
				{
				string c = data[i].ToString ();
				if (cellsApps[0].Contains (c))
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

			for (int i = 1; i <= Line.Length; i++)
				{
				file += Line[i - 1].ToString ().Replace (EmptySign, "-");

				if ((i % (SquareSize * SideSize)) == 0)
					file += RDLocale.RNRN;
				else if ((i % SideSize) == 0)
					file += RDLocale.RN;
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
			GameStartDate = DateTime.Now;

			successfulChecks = 0;
			failedChecks = 0;
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
			PropertyTypes prop = GetPropertyType (InterfaceElement);
			switch (Condition)
				{
				case ConditionTypes.ContainsFoundValue:
					return prop.HasFlag (PropertyTypes.SuccessColor);

				case ConditionTypes.ContainsNewValue:
					return prop.HasFlag (PropertyTypes.NewColor);

				case ConditionTypes.IsEmpty:
					return (InterfaceElement.Text == EmptySign);

				case ConditionTypes.SelectedCell:
					return prop.HasFlag (PropertyTypes.SelectedCell);

				case ConditionTypes.ContainsOldValue:
					return prop.HasFlag (PropertyTypes.OldColor);

				case ConditionTypes.ContainsErrorValue:
					return prop.HasFlag (PropertyTypes.ErrorColor);
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
			_ = ColorScheme;    // Загрузка значения
			byte[] color = colorsV4[3][colorIndex];

			PropertyTypes prop = GetPropertyType (InterfaceElement);
			switch (Property)
				{
				case PropertyTypes.EmptyValue:
					InterfaceElement.Text = EmptySign;
					break;

				case PropertyTypes.SuccessColor:
					color = colorsV4[2][colorIndex];
					break;

				case PropertyTypes.ErrorColor:
					color = colorsV4[1][colorIndex];
					break;

				case PropertyTypes.NewColor:
					color = colorsV4[0][colorIndex];
					break;

				case PropertyTypes.OldColor:
					break;

				case PropertyTypes.DeselectedCell:
					color = colorsV4[6][colorIndex];
					break;

				case PropertyTypes.SelectedCell:
					color = colorsV4[7][colorIndex];
					break;

				case PropertyTypes.OtherButton:
					color = colorsV4[5][colorIndex];
					break;

				case PropertyTypes.AffectedCell:
					color = colorsV4[8][colorIndex];
					break;
				}

			if ((Property & PropertyTypes.TextColorMask) != 0)
				{
#if ANDROID
				InterfaceElement.TextColor = BytesToColor (color);
#else
				InterfaceElement.ForeColor = BytesToColor (color);
#endif
				prop &= ~PropertyTypes.TextColorMask;
				prop |= Property;
				}

			else if ((Property & PropertyTypes.BackColorMask) != 0)
				{
#if ANDROID
				InterfaceElement.BackgroundColor = BytesToColor (color);
#else
				InterfaceElement.BackColor = BytesToColor (color);
#endif
				prop &= ~PropertyTypes.BackColorMask;
				prop |= Property;
				}

			SetPropertyType (InterfaceElement, prop);
			}

		private static PropertyTypes GetPropertyType (Button InterfaceElement)
			{
#if ANDROID
			if (!string.IsNullOrWhiteSpace (InterfaceElement.ClassId))
				return (PropertyTypes)int.Parse (InterfaceElement.ClassId);

#else
			if (InterfaceElement.Tag != null)
				return (PropertyTypes)InterfaceElement.Tag;
#endif

			return 0;
			}

		private static void SetPropertyType (Button InterfaceElement, PropertyTypes Value)
			{
#if ANDROID
			InterfaceElement.ClassId = ((int)Value).ToString ();
#else
			InterfaceElement.Tag = (int)Value;
#endif
			}

		/// <summary>
		/// Метод получает размер текущего выигрыша по указанному количеству найденных ячеек
		/// </summary>
		/// <param name="Value">Количество найденных ячеек</param>
		/// <returns>Возвращает количество очков или 0, если игровой режим неактивен</returns>
		public static uint GetScore (uint Value)
			{
			return GetScore (ScoreTypes.RegularWinning, Value);
			}

		/// <summary>
		/// Метод получает размер выигрыша по указанному типу расчёта.
		/// В случае победы в игре также обновляет счётчик побед
		/// </summary>
		/// <param name="ScoreType">Тип рассчитываемого выигрыша</param>
		/// <returns>Возвращает количество очков или 0, если игровой режим неактивен,
		/// или выбран режим RegularScore (для него существует отдельная перегрузка)</returns>
		public static uint GetScore (ScoreTypes ScoreType)
			{
			return GetScore (ScoreType, 0);
			}

		/// <summary>
		/// Метод возвращает представление ячейки в текущей настройке
		/// </summary>
		/// <param name="Value">Цифра, для которой требуется представление (1 – 9)</param>
		/// <returns>Возвращает представление или EmptySign, если переданная цифра некорректна
		/// или является представлением пустой ячейки</returns>
		public static string GetAppearance (Byte Value)
			{
			// Контроль
			if ((Value < 1) || (Value > SideSize))
				return EmptySign;

			// Результат
			_ = CellsAppearance;
			return cellsApps[cellsAppIndex][Value - 1];
			}

		/// <summary>
		/// Метод возвращает представление ячейки в текущей настройке
		/// </summary>
		/// <param name="Value">Цифра, для которой требуется представление (1 – 9)</param>
		/// <returns>Возвращает представление или EmptySign, если переданная цифра некорректна
		/// или является представлением пустой ячейки</returns>
		public static string GetAppearance (string Value)
			{
			// Контроль
			int idx = cellsApps[0].IndexOf (Value);
			if (idx < 0)
				return EmptySign;

			// Результат
			_ = CellsAppearance;
			return cellsApps[cellsAppIndex][idx];
			}

		/// <summary>
		/// Возвращает цифру по её представлению
		/// </summary>
		/// <param name="Appearance">Представление цифры в любой настройке</param>
		/// <returns>Возвращает цифру или 0, если указанное представление не определено</returns>
		public static Byte GetDigit (string Appearance)
			{
			_ = CellsAppearance;
			int idx = cellsApps[cellsAppIndex].IndexOf (Appearance);
			if (idx < 0)
				return 0;

			return (Byte)(idx + 1);
			}

		/// <summary>
		/// Метод определяет, влияет ли ячейка TestCell на значение SelectedCell
		/// </summary>
		/// <param name="SelectedCellIndex">Выбранная ячейка (индекс от 0 до 80 включительно)</param>
		/// <param name="TestCellIndex">Проверяемая ячейка (индекс от 0 до 80 включительно)</param>
		/// <returns>Возвращает true, если влияние подтверждается</returns>
		public static bool IsCellAffected (uint SelectedCellIndex, uint TestCellIndex)
			{
			// Если совпадают столбцы
			if ((SelectedCellIndex % SideSize) == (TestCellIndex % SideSize))
				return true;

			// Если совпадают строки
			if ((SelectedCellIndex / SideSize) == (TestCellIndex / SideSize))
				return true;

			// Если совпадают квадраты
			if ((SelectedCellIndex / (SquareSize * SideSize)) == (TestCellIndex / (SquareSize * SideSize)) &&
				(SelectedCellIndex / SquareSize) % SquareSize == (TestCellIndex / SquareSize) % SquareSize)
				return true;

			return false;
			}
		}
	}
