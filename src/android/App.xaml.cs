using Microsoft.Maui.Controls;
using System.ComponentModel;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает функционал приложения
	/// </summary>
	public partial class App: Application
		{
		#region Общие переменные и константы

		// Прочие параметры
		private RDAppStartupFlags flags;

		// Цветовая схема
		private Color solutionMasterBackColor = Color.FromArgb ("#ffffe7");
		private Color solutionFieldBackColor = Color.FromArgb ("#ffffde");
		private Color aboutMasterBackColor = Color.FromArgb ("#F0FFF0");
		private Color aboutFieldBackColor = Color.FromArgb ("#D0FFD0");

		private Color newTextColor = Color.FromArgb ("#0000C8");
		private Color errorTextColor = Color.FromArgb ("#C80000");
		private Color foundTextColor = Color.FromArgb ("#00C800");
		private Color oldTextColor;
		private Color selectedButtonColor = Color.FromArgb ("#00FFFF");
		private Color deselectedButtonColor = Color.FromArgb ("#C0FFFF");

		/*private const string emptySign = " ";*/

		private List<string> keyboardVariants = new List<string> ();
		private List<string> menuVariants = new List<string> ();

		#endregion

		#region Переменные страниц

		private ContentPage solutionPage, aboutPage;

		private Label aboutFontSizeField;

		private Button languageButton, solveButton, keyboardButton, menuButton;
		private List<Button> numberButtons = new List<Button> ();
		private List<Button> inputButtons = new List<Button> ();

		private StackLayout masterField;
		private StackLayout inputField = new StackLayout ();

		#endregion

		#region Запуск и настройка

		/// <summary>
		/// Конструктор. Точка входа приложения
		/// </summary>
		public App ()
			{
			// Инициализация
			InitializeComponent ();
			flags = RDGenerics.GetAppStartupFlags (RDAppStartupFlags.DisableXPUN);

			// Общая конструкция страниц приложения
			MainPage = new MasterPage ();

			solutionPage = RDInterface.ApplyPageSettings (new SolutionPage (), "SolutionPage",
				RDLocale.GetText ("SolutionPage"), solutionMasterBackColor);
			aboutPage = RDInterface.ApplyPageSettings (new AboutPage (), "AboutPage",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout),
				aboutMasterBackColor);

			RDInterface.SetMasterPage (MainPage, solutionPage, solutionMasterBackColor);

			#region Основная страница

			masterField = (StackLayout)solutionPage.FindByName ("MasterField");

			StackLayout numbersField = new StackLayout ();
			numbersField.HorizontalOptions = numbersField.VerticalOptions = LayoutOptions.Center;
			numbersField.Orientation = StackOrientation.Vertical;

			// Сборка поля ввода матрицы
			uint sideSize = SudokuSolverMath.SudokuSideSize;
			uint sq = (uint)Math.Sqrt (sideSize);
			List<StackLayout> numbersSL = new List<StackLayout> ();
			oldTextColor = RDInterface.GetInterfaceColor (RDInterfaceColors.AndroidTextColor);

			for (int i = 0; i < sideSize * sideSize; i++)
				{
				// Добавление горизонтальных пробелов
				if ((i != 0) && (i % (sq * sideSize) == 0))
					{
					Label l = new Label ();
					l.WidthRequest = l.HeightRequest = 5;
					numbersField.Add (l);
					}

				// Добавление строковых полей и вертикальных пробелов
				if (i % sideSize == 0)
					{
					StackLayout sl = new StackLayout ();
					sl.Orientation = StackOrientation.Horizontal;
					sl.HorizontalOptions = LayoutOptions.Center;
					numbersSL.Add (sl);
					numbersField.Add (sl);
					}
				else if (i % sq == 0)
					{
					Label l = new Label ();
					l.WidthRequest = l.HeightRequest = 5;
					numbersSL[numbersSL.Count - 1].Add (l);
					}

				// Добавление кнопок
				Button b = new Button ();
				b.BackgroundColor = deselectedButtonColor;
				b.FontAttributes = FontAttributes.None;
				b.FontSize = 5 * RDInterface.MasterFontSize / 4;
				b.FontAttributes = FontAttributes.Bold;
				b.FontFamily = RDGenerics.MonospaceFont;
				b.TextColor = oldTextColor;
				b.LineBreakMode = LineBreakMode.WordWrap;
				b.WidthRequest = b.HeightRequest = RDInterface.MasterFontSize * 2.25;
				b.Padding = Thickness.Zero;
				b.Margin = new Thickness (1);
				b.Text = SudokuSolverMath.EmptySign;
				b.TextTransform = TextTransform.None;
				b.Clicked += SelectCurrentButton;

				numberButtons.Add (b);
				numbersSL[numbersSL.Count - 1].Add (b);
				}

			masterField.Add (numbersField);

			// Загрузка сохранённого состояния
			string sudoku = SudokuField;
			if (sudoku.Length == sideSize * sideSize)
				{
				for (int i = 0; i < numberButtons.Count; i++)
					{
					numberButtons[i].Text = sudoku[i].ToString ();
					if (numberButtons[i].Text != SudokuSolverMath.EmptySign)
						numberButtons[i].TextColor = newTextColor;
					}
				}

			// Разделитель
			Label sp = new Label ();
			sp.WidthRequest = sp.HeightRequest = 15;
			masterField.Add (sp);

			// Сборка вспомогательной клавиатуры
			inputField.HorizontalOptions = inputField.VerticalOptions = LayoutOptions.Center;
			inputField.Orientation = StackOrientation.Vertical;

			List<StackLayout> inputSL = new List<StackLayout> ();

			for (int i = 9; i >= 0; i--)
				{
				// Добавление строковых полей
				if (i % 3 == 0)
					{
					StackLayout sl = new StackLayout ();
					sl.Orientation = StackOrientation.Horizontal;
					sl.HorizontalOptions = LayoutOptions.Center;
					inputSL.Add (sl);
					inputField.Add (sl);
					}

				Button b = new Button ();
				b.BackgroundColor = solutionFieldBackColor;
				b.FontAttributes = FontAttributes.None;
				b.FontSize = 5 * RDInterface.MasterFontSize / 4;
				b.TextColor = RDInterface.GetInterfaceColor (RDInterfaceColors.AndroidTextColor);
				b.LineBreakMode = LineBreakMode.WordWrap;
				b.Padding = Thickness.Zero;
				b.Margin = new Thickness (3);
				b.Text = (i == 0) ? RDLocale.GetText ("EmptyButton") : i.ToString ();
				b.TextTransform = TextTransform.None;
				b.Clicked += SetValueForCurrentButton;

				if (i != 0)
					b.WidthRequest = b.HeightRequest = RDInterface.MasterFontSize * 2.75;

				if (inputButtons.Count > 0)
					inputButtons.Insert (0, b);
				else
					inputButtons.Add (b);

				if (inputSL[inputSL.Count - 1].Count > 0)
					inputSL[inputSL.Count - 1].Insert (0, b);
				else
					inputSL[inputSL.Count - 1].Add (b);
				}

			masterField.Add (inputField);

			// Загрузка расположения клавиатуры
			SelectKeyboardPlacement (null, null);

			// Остальные кнопки
			keyboardButton = RDInterface.ApplyButtonSettings (solutionPage, "KeyboardButton",
				RDLocale.GetText ("KeyboardButton"), solutionFieldBackColor, SelectKeyboardPlacement, false);
			solveButton = RDInterface.ApplyButtonSettings (solutionPage, "SolveButton", RDLocale.GetText ("SolveButton"),
				solutionFieldBackColor, SolveSudoku, false);
			menuButton = RDInterface.ApplyButtonSettings (solutionPage, "MenuButton", RDDefaultButtons.Menu,
				solutionFieldBackColor, MenuButton_Clicked);

			#endregion

			#region Страница "О программе"

			RDInterface.ApplyLabelSettings (aboutPage, "AboutLabel",
				RDGenerics.AppAboutLabelText, RDLabelTypes.AppAbout);

			RDInterface.ApplyButtonSettings (aboutPage, "ManualsButton",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_ReferenceMaterials),
				aboutFieldBackColor, ReferenceButton_Click, false);
			RDInterface.ApplyButtonSettings (aboutPage, "HelpButton",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_HelpSupport),
				aboutFieldBackColor, HelpButton_Click, false);
			RDInterface.ApplyLabelSettings (aboutPage, "GenericSettingsLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_GenericSettings),
				RDLabelTypes.HeaderLeft);

			RDInterface.ApplyLabelSettings (aboutPage, "RestartTipLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Message_RestartRequired),
				RDLabelTypes.TipCenter);

			RDInterface.ApplyLabelSettings (aboutPage, "LanguageLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_InterfaceLanguage),
				RDLabelTypes.DefaultLeft);
			languageButton = RDInterface.ApplyButtonSettings (aboutPage, "LanguageSelector",
				RDLocale.LanguagesNames[(int)RDLocale.CurrentLanguage],
				aboutFieldBackColor, SelectLanguage_Clicked, false);

			RDInterface.ApplyLabelSettings (aboutPage, "FontSizeLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_InterfaceFontSize),
				RDLabelTypes.DefaultLeft);
			RDInterface.ApplyButtonSettings (aboutPage, "FontSizeInc",
				RDDefaultButtons.Increase, aboutFieldBackColor, FontSizeButton_Clicked);
			RDInterface.ApplyButtonSettings (aboutPage, "FontSizeDec",
				RDDefaultButtons.Decrease, aboutFieldBackColor, FontSizeButton_Clicked);
			aboutFontSizeField = RDInterface.ApplyLabelSettings (aboutPage, "FontSizeField",
				" ", RDLabelTypes.DefaultCenter);

			RDInterface.ApplyLabelSettings (aboutPage, "HelpHeaderLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout),
				RDLabelTypes.HeaderLeft);
			Label htl = RDInterface.ApplyLabelSettings (aboutPage, "HelpTextLabel",
				RDGenerics.GetAppHelpText (), RDLabelTypes.SmallLeft);
			htl.TextType = TextType.Html;

			FontSizeButton_Clicked (null, null);

			#endregion

			// Отображение подсказок первого старта
			ShowStartupTips ();
			}

		// Метод отображает подсказки при первом запуске
		private async void ShowStartupTips ()
			{
			// Контроль XPUN
			if (!flags.HasFlag (RDAppStartupFlags.DisableXPUN))
				await RDInterface.XPUNLoop ();

			// Требование принятия Политики
			if (TipsState.HasFlag (TipTypes.PolicyTip))
				return;

			await RDInterface.PolicyLoop ();
			TipsState |= TipTypes.PolicyTip;
			}

		/// <summary>
		/// Сохранение настроек программы
		/// </summary>
		protected override void OnSleep ()
			{
			string sudoku = "";
			for (int i = 0; i < numberButtons.Count; i++)
				sudoku += numberButtons[i].Text;

			SudokuField = sudoku;
			}

		/// <summary>
		/// Возвращает или задаёт состав флагов просмотра справочных сведений
		/// </summary>
		public static TipTypes TipsState
			{
			get
				{
				return (TipTypes)RDGenerics.GetSettings (tipsStatePar, 0);
				}
			set
				{
				RDGenerics.SetSettings (tipsStatePar, (uint)value);
				}
			}
		private const string tipsStatePar = "TipsState";

		/// <summary>
		/// Доступные типы уведомлений
		/// </summary>
		public enum TipTypes
			{
			/// <summary>
			/// Принятие Политики и первая подсказка
			/// </summary>
			PolicyTip = 0x0001,
			}

		#endregion

		#region О приложении

		// Выбор языка приложения
		private async void SelectLanguage_Clicked (object sender, EventArgs e)
			{
			languageButton.Text = await RDInterface.CallLanguageSelector ();
			}

		// Вызов справочных материалов
		private async void ReferenceButton_Click (object sender, EventArgs e)
			{
			await RDInterface.CallHelpMaterials (RDHelpMaterials.ReferenceMaterials);
			}

		private async void HelpButton_Click (object sender, EventArgs e)
			{
			await RDInterface.CallHelpMaterials (RDHelpMaterials.HelpAndSupport);
			}

		// Изменение размера шрифта интерфейса
		private void FontSizeButton_Clicked (object sender, EventArgs e)
			{
			if (sender != null)
				{
				Button b = (Button)sender;
				if (RDInterface.IsNameDefault (b.Text, RDDefaultButtons.Increase))
					RDInterface.MasterFontSize += 0.5;
				else if (RDInterface.IsNameDefault (b.Text, RDDefaultButtons.Decrease))
					RDInterface.MasterFontSize -= 0.5;
				}

			aboutFontSizeField.Text = RDInterface.MasterFontSize.ToString ("F1");
			aboutFontSizeField.FontSize = RDInterface.MasterFontSize;
			}

		#endregion

		#region Рабочая зона

		// Метод открывает страницу О программе
		private async void MenuButton_Clicked (object sender, EventArgs e)
			{
			// Выбор варианта
			if (menuVariants.Count < 1)
				{
				menuVariants.Add ("🔄\t " + RDLocale.GetText ("ResetSolution"));
				menuVariants.Add ("❌\t " + RDLocale.GetText ("ResetField"));
				menuVariants.Add ("📄\t " + RDLocale.GetText ("LoadFromFile"));
				menuVariants.Add ("💾\t " + RDLocale.GetText ("SaveToFile"));
				menuVariants.Add ("ℹ️\t " + RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout));
				}
			int res = await RDInterface.ShowList (RDLocale.GetText ("MenuButton"),
				RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel), menuVariants);
			if (res < 0)
				return;

			// Выполнение
			switch (res)
				{
				// Сброс решения
				case 0:
					for (int i = 0; i < numberButtons.Count; i++)
						if ((numberButtons[i].TextColor != oldTextColor) && (numberButtons[i].TextColor != newTextColor))
							numberButtons[i].Text = SudokuSolverMath.EmptySign;
					break;

				// Полный сброс
				case 1:
					if (!await RDInterface.ShowMessage (RDLocale.GetText ("ResetWarning"),
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_Yes),
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_No)))
						return;

					for (int i = 0; i < numberButtons.Count; i++)
						numberButtons[i].Text = SudokuSolverMath.EmptySign;
					break;

				// Загрузка из файла
				case 2:
					await LoadFromFile ();
					break;

				// Сохранение в файл
				case 3:
					await SaveToFile ();
					break;

				// О приложении
				case 4:
					RDInterface.SetCurrentPage (aboutPage, aboutMasterBackColor);
					break;
				}
			}

		// Выбор текущей кнопки в матрице
		private void SelectCurrentButton (object sender, EventArgs e)
			{
			currentButtonIndex = numberButtons.IndexOf ((Button)sender);

			// Кнопка уже была выбрана – выполнить приращение
			if (numberButtons[currentButtonIndex].BackgroundColor == selectedButtonColor)
				{
				uint v = 0;
				Button b = numberButtons[currentButtonIndex];
				try
					{
					v = uint.Parse (b.Text) + 1;
					}
				catch { }

				if (v == 0)
					b.Text = "1";
				else if (v > 9)
					b.Text = SudokuSolverMath.EmptySign;
				else
					b.Text = v.ToString ();
				b.TextColor = newTextColor;
				}

			// Кнопка выбрана впервые – выполнить изменение цветов
			else
				{
				for (int i = 0; i < numberButtons.Count; i++)
					{
					if (i == currentButtonIndex)
						numberButtons[i].BackgroundColor = selectedButtonColor;
					else
						numberButtons[i].BackgroundColor = deselectedButtonColor;
					}
				}
			}
		private int currentButtonIndex = -1;

		// Выбор значения для текущей кнопки в матрице
		private void SetValueForCurrentButton (object sender, EventArgs e)
			{
			if (currentButtonIndex < 0)
				return;

			int idx = inputButtons.IndexOf ((Button)sender);
			Button b = numberButtons[currentButtonIndex];
			if (idx > 0)
				b.Text = idx.ToString ();
			else
				b.Text = SudokuSolverMath.EmptySign;
			b.TextColor = newTextColor;
			}

		/// <summary>
		/// Сохраняет или загружает ориентацию элементов экрана
		/// </summary>
		private static KeyboardPlacements KeyboardPlacement
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
		private const string keyboardPlacementsPar = "KeyboardPlacements";

		/// <summary>
		/// Сохраняет или загружает текущее состояние поля ввода
		/// </summary>
		private static string SudokuField
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
		private const string sudokuFieldPar = "SudokuField";

		// Возможные расположения клавиатуры
		private enum KeyboardPlacements
			{
			// Не отображается
			None,

			// Справа
			Right,

			// Снизу
			Bottom,
			}

		// Выбор расположения клавиатуры
		private async void SelectKeyboardPlacement (object sender, EventArgs e)
			{
			// Выбор варианта
			if (keyboardVariants.Count < 1)
				{
				keyboardVariants.Add ("❌\t " + RDLocale.GetText ("KeyboardVariant0"));
				keyboardVariants.Add ("➡️\t " + RDLocale.GetText ("KeyboardVariant1"));
				keyboardVariants.Add ("⬇️\t " + RDLocale.GetText ("KeyboardVariant2"));
				}

			int res;
			if (sender == null)
				{
				res = (int)KeyboardPlacement;
				}
			else
				{
				res = await RDInterface.ShowList (RDLocale.GetText ("KeyboardPlacement"),
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel), keyboardVariants);
				if (res < 0)
					return;
				KeyboardPlacement = (KeyboardPlacements)res;
				}

			// Настройка
			inputField.IsVisible = (res > 0);
			if (KeyboardPlacement == KeyboardPlacements.Right)
				masterField.Orientation = StackOrientation.Horizontal;
			else
				masterField.Orientation = StackOrientation.Vertical;
			}

		// Метод выполняет решение судоку
		private async void SolveSudoku (object sender, EventArgs e)
			{
			// Остановка решения
			if (!masterField.IsEnabled)
				{
				SudokuSolverMath.RequestStop ();
				return;
				}

			// Сборка массива
			Byte[,] matrix = new Byte[SudokuSolverMath.SudokuSideSize, SudokuSolverMath.SudokuSideSize];
			for (int r = 0; r < SudokuSolverMath.SudokuSideSize; r++)
				{
				for (int c = 0; c < SudokuSolverMath.SudokuSideSize; c++)
					{
					Button ct = numberButtons[r * (int)SudokuSolverMath.SudokuSideSize + c];
					if ((ct.Text != SudokuSolverMath.EmptySign) &&
						((ct.TextColor == oldTextColor) || (ct.TextColor == newTextColor) ||
						(ct.TextColor == errorTextColor)))
						matrix[r, c] = Byte.Parse (ct.Text);
					else
						matrix[r, c] = 0;
					}
				}

			// Инициализация задачи
			SudokuSolverMath.InitializeSolution (matrix);
			switch (SudokuSolverMath.CurrentStatus)
				{
				case SolutionResults.InitialMatrixIsInvalid:
					throw new Exception ("Invalid initialization of the solution, debug is required");

				case SolutionResults.InitialMatrixIsUnsolvable:
					for (int i = 0; i < numberButtons.Count; i++)
						numberButtons[i].TextColor = errorTextColor;
					return;
				}

			// Решение задачи
			masterField.IsEnabled = keyboardButton.IsVisible = menuButton.IsVisible = false;
			inputField.IsVisible = false;
			solveButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel);

			await Task.Run<bool> (SudokuSolverMath.FindSolution);

			masterField.IsEnabled = keyboardButton.IsVisible = menuButton.IsVisible = true;
			SelectKeyboardPlacement (null, null);
			solveButton.Text = RDLocale.GetText ("SolveButton");

			// Разбор решения
			switch (SudokuSolverMath.CurrentStatus)
				{
				case SolutionResults.NoSolutionsFound:
				case SolutionResults.NotInited:
					for (int i = 0; i < numberButtons.Count; i++)
						numberButtons[i].TextColor = errorTextColor;
					return;

				case SolutionResults.SearchAborted: // Не перекрашивать поле
					return;
				}

			// Отображение решения
			for (int r = 0; r < SudokuSolverMath.SudokuSideSize; r++)
				{
				for (int c = 0; c < SudokuSolverMath.SudokuSideSize; c++)
					{
					Button ct = numberButtons[r * (int)SudokuSolverMath.SudokuSideSize + c];
					if ((ct.Text != SudokuSolverMath.EmptySign) &&
						((ct.TextColor == oldTextColor) || (ct.TextColor == newTextColor) ||
						(ct.TextColor == errorTextColor)))
						{
						ct.TextColor = oldTextColor;
						}
					else
						{
						ct.Text = SudokuSolverMath.ResultMatrix[r, c].ToString ();
						ct.TextColor = foundTextColor;
						}
					}
				}

			// Выполнено
			}

		// Метод загружает матрицу из файла
		private async Task<bool> LoadFromFile ()
			{
			// Попытка считывания файла
			string file = await RDGenerics.LoadFromFile (RDEncodings.UTF8);
			if (string.IsNullOrWhiteSpace (file))
				return false;

			// Обработка
			string line = SudokuSolverMath.ParseMatrixFromFile (file);
			if (string.IsNullOrWhiteSpace (line))
				{
				await RDInterface.ShowMessage (RDLocale.GetText ("MessageNotEnough"),
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));
				return false;
				}

			// Загрузка
			for (int i = 0; i < numberButtons.Count; i++)
				{
				numberButtons[i].Text = line[i].ToString ();
				if (numberButtons[i].Text == SudokuSolverMath.EmptySign)
					numberButtons[i].TextColor = newTextColor;
				else
					numberButtons[i].TextColor = oldTextColor;
				}

			return true;
			}

		// Метод сохраняет матрицу в файл
		private async Task<bool> SaveToFile ()
			{
			// Выгрузка данных
			OnSleep ();
			string file = SudokuSolverMath.BuildMatrixToSave (SudokuField);

			// Сохранение
			return await RDGenerics.SaveToFile ("Sudoku.txt", file, RDEncodings.UTF8);
			}

		#endregion
		}
	}
