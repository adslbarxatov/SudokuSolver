using System;
using System.ComponentModel;

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
	/// Класс описывает основной функционал поиска решения
	/// </summary>
	public static class SudokuSolverMath
		{
		//////////////////////////////
		// SudokuSolver.h

		// Базовые параметры

		// РАЗМЕР СУДОКУ
		private const UInt16 SDS = 9;

		/// <summary>
		/// Размер стороны судоку
		/// </summary>
		public const uint SudokuSideSize = SDS;

		//	Данная программа использует двоичную систему счисления в качестве
		//	базового средства решения задачи. Т.е. если значения ячеек матрицы
		//	представить	в двоичном виде, то номера бит, установленных в 1, будут
		//	теми цифрами, которые в данный момент предполагается проставить в 
		//	соответствующую клетку судоку
		//	Соответственно, если бит, равный 1, в значении ячейки остался лишь
		//	один, клетка считается вычисленной. При	этом очевидно, что её
		//	значение будет степенью числа 2. Определение этого состояния
		//	является критерием решения задачи

		// Полная флаговая переменная (123456789), используемая для решения
		private const UInt16 SDS_FULL = ((1 << SDS) - 1);

		// Линейный размер квадрата матрицы
		private static UInt16 SQ = (UInt16)Math.Sqrt (SDS);

		// Максимальное количество итераций, рассматриваемое как нормальный поиск
		private const UInt16 MAX_ITER = 50;

		//////////////////////////////
		// SudokuSolver.c

		// Главная матрица
		private static UInt16[,] Mtx = new UInt16[SDS, SDS];

		//////////////////////////////
		// IsPowOf2.cc

		// Определение чисел, являющихся степенью числа 2
		//
		// Если аргумент является степенью числа 2, функция возвращает true
		// (иначе - false)
		//
		private static bool IsPowOf2 (UInt16 Value)
			{
			/*UInt16 s;

			// Чтобы число было степенью числа 2, бит, установленный в 1, должен
			// встречаться в нём лишь единожды
			for (UInt16 i = s = 0; i < 8 * sizeof (UInt16); i++)
				s += (UInt16)((Value >> i) & 1u);

			return (s == 1);*/

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

		//////////////////////////////
		// CheckErrors.cc

		// Проверка на ошибочность предположения (повторения цифр)
		//
		// Возвращает:	true, если ошибки были найдены (есть дублирующиеся цифры);
		//				false, если ошибок нет
		//
		private static bool HasErrors ()
			{
			UInt16 s;

			// Горизонтальные линии
			for (UInt16 i = 0; i < SDS; i++)
				{
				for (UInt16 j = s = 0; j < SDS; j++)
					{
					// Чтобы определить, были ли повторы, создаётся переменная s, в
					// которую заносятся уже встреченные биты вычисленных ячеек.
					// Если ячейка обозначена как вычисленная (is_pow_2 == 1), но
					// операция дизъюнкции этой ячейки с s не изменяет s, значит,
					// эта цифра уже есть в строке, что означает наличие ошибки
					if (IsPowOf2 (Mtx[i, j]) && ((s | Mtx[i, j]) == s))
						return true;

					if (IsPowOf2 (Mtx[i, j]))
						s |= Mtx[i, j]; // Добавляет бит в s, если он получен из вычисленной ячейки
					}
				}

			// Вертикальные линии
			for (UInt16 j = 0; j < SDS; j++)
				{
				for (UInt16 i = s = 0; i < SDS; i++)
					{
					if (IsPowOf2 (Mtx[i, j]) && ((s | Mtx[i, j]) == s))
						return true;

					if (IsPowOf2 (Mtx[i, j]))
						s |= Mtx[i, j];
					}
				}

			// Квадраты
			for (UInt16 i = 0; i < SDS; i++)
				{
				for (UInt16 j = s = 0; j < SDS; j++)
					{
					// Пересчитываемые параметры, позволяющие использовать линейные коэффициенты для зигзагообразного движения
					UInt16 SQI = (UInt16)(j % SQ + (i % SQ) * SQ);
					UInt16 SQJ = (UInt16)(j / SQ + (i / SQ) * SQ);

					if (IsPowOf2 (Mtx[SQI, SQJ]) && ((s | Mtx[SQI, SQJ]) == s))
						return true;

					if (IsPowOf2 (Mtx[SQI, SQJ]))
						s |= Mtx[SQI, SQJ];
					}
				}

			// Ошибок нет
			return false;
			}

		//////////////////////////////
		// CheckResult.cc

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
			for (UInt16 i = 0; i < SDS; i++)
				{
				for (UInt16 j = 0; j < SDS; j++)
					{
					s &= IsPowOf2 (Mtx[i, j]);

					if (!s)
						return s;   // Быстрый выход
					}
				}

			return s;
			}

		//////////////////////////////
		// UpdateMatrix.cc

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
				for (UInt16 i = 0; i < SDS; i++)
					{
					// Здесь происходит удаление бит, неудовлетворяющих условиям задачи.
					// Для этого в числе p, заданном изначально значением 0x1FF,
					// «выкалываются» биты, уже встречающиеся в данной строке (далее -
					// столбце и квадрате). Причём, для этого используются только
					// вычисленные ячейки (is_pow_2 == true)
					// Затем полученное число конкатенируется с каждой ячейкой, где
					// вычисление ещё не было завершено (is_pow_2 == false)
					p = SDS_FULL;

					for (UInt16 j = 0; j < SDS; j++)
						{
						if (IsPowOf2 (Mtx[i, j]))
							p &= (UInt16)(~Mtx[i, j]);  // Mij = 010000000; p = 111111111; [&=~] = 101111111
						}

					for (UInt16 j = 0; j < SDS; j++)
						{
						if (!IsPowOf2 (Mtx[i, j]))
							Mtx[i, j] &= p;     // Mij = 010010001; p = 101111111; Mij' = 000010001
						}
					}

				// Вертикальные линии
				for (UInt16 j = 0; j < SDS; j++)
					{
					p = SDS_FULL;

					for (UInt16 i = 0; i < SDS; i++)
						{
						if (IsPowOf2 (Mtx[i, j]))
							p &= (UInt16)(~Mtx[i, j]);
						}

					for (UInt16 i = 0; i < SDS; i++)
						{
						if (!IsPowOf2 (Mtx[i, j]))
							Mtx[i, j] &= p;
						}
					}

				// Квадраты
				for (UInt16 i = 0; i < SDS; i++)
					{
					p = SDS_FULL;

					for (UInt16 j = 0; j < SDS; j++)
						{
						UInt16 SQI = (UInt16)(j % SQ + (i % SQ) * SQ);
						UInt16 SQJ = (UInt16)(j / SQ + (i / SQ) * SQ);

						if (IsPowOf2 (Mtx[SQI, SQJ]))
							p &= (UInt16)(~Mtx[SQI, SQJ]);
						}

					for (UInt16 j = 0; j < SDS; j++)
						{
						UInt16 SQI = (UInt16)(j % SQ + (i % SQ) * SQ);
						UInt16 SQJ = (UInt16)(j / SQ + (i / SQ) * SQ);

						if (!IsPowOf2 (Mtx[SQI, SQJ]))
							Mtx[SQI, SQJ] &= p;
						}
					}
				}

			// Результат прогонки
			if (HasErrors ())
				return -1;      // Прогонка дала ошибочный результат (имеются повторяющиеся значения)
			else if (iterations < MAX_ITER)
				return 1;       // Прогонка не дала конечного результата
			else
				return 0;       // Получен ответ
			}

		////////////////////////
		// Search.cc

#if ANDROID

		/// <summary>
		/// Метод взводит флаг досрочного завершения выполнения операции
		/// </summary>
		public static void RequestStop ()
			{
			stopRequested = true;
			}

#else

		private static void CheckBW ()
			{
			if (bw.CancellationPending)
				stopRequested = true;
			}
		private static BackgroundWorker bw;

#endif

		private static bool stopRequested = false;

		// Функция изучения предположений
		//
		// Рекурсивная функция
		// Возвращает:	0, если результат получен
		//				-1, если ответ не был найден
		//				1, если поиск был отменён
		private static short Search (/*BackgroundWorker Worker*/)
			{
			// Защита (запрос прерывания)
			/*if (Worker.CancellationPending)*/

#if !ANDROID
			CheckBW ();
#endif

			if (stopRequested)
				{
				stopRequested = false;
				return 1;
				}

			// Переменные
			UInt16[,] Mtc = new UInt16[SDS, SDS];

			// Создание копии исходной матрицы для данного предположения
			Mtc = (UInt16[,])Mtx.Clone ();

			// Поиск первой невычисленной ячейки
			UInt16 i = 0, j = 0;
			for (i = 0; i < SDS; i++)
				{
				for (j = 0; j < SDS; j++)
					{
					if (!IsPowOf2 (Mtx[i, j]))
						goto m1;
					}
				}

			// Решение уже найдено
			return 0;

		m1:
			for (UInt16 k = 0; k < SDS; k++)
				{
				// Выполнение предположения (все цифры по порядку)
				Mtx[i, j] = (UInt16)(1 << k);

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
						short res = Search (/*Worker*/);
						switch (res)
							{
							// Если оно дало конечный результат, нужно вернуть его наверх
							case 0:

							// Если поиск отменён, нужно прервать всё дерево вызовов
							case 1:
								return res;

							// Если нет, нужно восстановить матрицу
							default:
								Mtx = (UInt16[,])Mtc.Clone ();
								break;
							}
						break;

					// Если получена ошибка при прогонке, нужно восстановить матрицу
					default:
						Mtx = (UInt16[,])Mtc.Clone ();
						break;
					}
				}

			// Правильных вариантов не найдено
			return -1;
			}

		//////////////////////////////
		// Собственный интерфейс

		/// <summary>
		/// Возвращает результат решения задачи
		/// </summary>
		public static SolutionResults CurrentStatus
			{
			get
				{
				return currentStatus;
				}
			}
		private static SolutionResults currentStatus = SolutionResults.NotInited;

		/// <summary>
		/// Метод инициализирует матрицу и готовит класс к выполнению решения
		/// </summary>
		/// <param name="SourceMatrix">Исходная таблица чисел; должна иметь высоту и ширину, равные 9;
		/// из значений извлекаются только младшие разряды; нулевые значения рассматриваются как те, 
		/// которые нужно найти</param>
		public static SolutionResults InitializeSolution (Byte[,] SourceMatrix)
			{
			// Контроль
			if ((SourceMatrix == null) || (SourceMatrix.GetLength (0) != SDS) || (SourceMatrix.GetLength (1) != SDS))
				{
				currentStatus = SolutionResults.InitialMatrixIsInvalid;
				return currentStatus;
				}

			// Инициализация
			for (UInt16 i = 0; i < SDS; i++)
				{
				for (UInt16 j = 0; j < SDS; j++)
					{
					Byte digit = (Byte)(SourceMatrix[i, j] % 10);
					if (digit != 0)
						Mtx[i, j] = (UInt16)(1 << (digit - 1));
					else
						Mtx[i, j] = SDS_FULL;
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
		public static async Task<bool> FindSolution ()
			{
			// Контроль
			if (currentStatus != SolutionResults.ReadyForSearch)
				{
				currentStatus = SolutionResults.NotInited;
				return false;
				}

			// Метод предположений
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

		// Метод выгружает полученную матрицу в результат
		private static void UploadResultMatrix ()
			{
			resultMatrix = new Byte[SDS, SDS];
			for (UInt16 i = 0; i < SDS; i++)
				{
				for (UInt16 j = 0; j < SDS; j++)
					resultMatrix[i, j] = (Byte)(Math.Log (Mtx[i, j], 2) + 1);
				}
			}

		/// <summary>
		/// Возвращает матрицу, полученную при решении задачи или null, если решение не было найдено
		/// </summary>
		public static Byte[,] ResultMatrix
			{
			get
				{
				return resultMatrix;
				}
			}
		private static Byte[,] resultMatrix;

#if !ANDROID

		// Образец метода, выполняющего длительные вычисления
		private static void DoSearch (object sender, DoWorkEventArgs e)
			{
			bw = (BackgroundWorker)sender;

			switch (Search (/*(BackgroundWorker)sender)*/))
				{
				case -1:
					e.Result = (int)SolutionResults.NoSolutionsFound;
					return;

				case 1:
					e.Result = (int)SolutionResults.SearchAborted;
					return;
				}

			e.Result = SolutionResults.SolutionFound;
			}

#endif

		/// <summary>
		/// Метод извлекает матрицу из содержимого файла и возвращает её, если это возможно,
		/// в виде сплошной строки длиной SDS * SDS
		/// </summary>
		/// <param name="FileContents">Содержимое текстового файла</param>
		public static string ParseMatrixFromFile (string FileContents)
			{
			// Обработка
			if (string.IsNullOrWhiteSpace (FileContents))
				return "";
			string data = FileContents;

			for (int i = 0; i < splitters.Length; i++)
				data = data.Replace (splitters[i], "");

			if (data.Length < SudokuSideSize * SudokuSideSize)
				return "";

			// Загрузка
			string resultLine = "";
			for (int i = 0; i < SudokuSideSize * SudokuSideSize; i++)
				{
				string c = data[i].ToString ();
				if (checker.Contains (c))
					resultLine += c;
				else
					resultLine += EmptySign;
				}

			return resultLine;
			}
		private static string[] splitters = new string[] { "\r", "\n", "\t", " ", ";" };
		private const string checker = "123456789";

		/// <summary>
		/// Метод оформляет матрицу для сохранения в файл
		/// </summary>
		/// <param name="Line">Сплошная строка значений матрицы</param>
		public static string BuildMatrixToSave (string Line)
			{
			string file = "";
			int sqrt = (int)Math.Sqrt (SudokuSideSize);
			int cubedSqrt = sqrt * sqrt * sqrt;

			for (int i = 1; i <= Line.Length; i++)
				{
				file += Line[i - 1].ToString ().Replace (EmptySign, "-");

				if (i % cubedSqrt == 0)
					file += RDLocale.RNRN;
				else if (i % SudokuSideSize == 0)
					file += RDLocale.RN;
				else if (i % sqrt == 0)
					file += " ";
				}

			return file;
			}

		/// <summary>
		/// Возвращает признак незаполненной ячейки матрицы
		/// </summary>
		public const string EmptySign = " ";
		}
	}
