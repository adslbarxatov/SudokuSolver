﻿using Microsoft.Maui.Controls;
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
		/*private Color solutionMasterBackColor = Color.FromArgb ("#ffffe7");
		private Color solutionFieldBackColor = Color.FromArgb ("#ffffde");*/
		private Color aboutMasterBackColor = Color.FromArgb ("#F0FFF0");
		private Color aboutFieldBackColor = Color.FromArgb ("#D0FFD0");
		/*private Color selectedButtonColor = Color.FromArgb ("#00FFFF");
		private Color deselectedButtonColor = Color.FromArgb ("#C0FFFF");*/

		private List<string> keyboardVariants = new List<string> ();
		/*private List<string> menuVariants = new List<string> ();*/
		private List<string> difficultyVariants = new List<string> ();
		private List<string> appModeVariants = new List<string> ();
		private List<string> colorSchemeVariants = new List<string> ();
		private List<List<string>> menuVariants = new List<List<string>> ();

		#endregion

		#region Переменные страниц

		private ContentPage solutionPage, aboutPage;

		private Label aboutFontSizeField;

		private Button languageButton, solutionButton, checkButton, generateButton,
			clearButton, menuButton;
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

			Color color = RDInterface.GetInterfaceColor (RDInterfaceColors.MediumGrey);
			solutionPage = RDInterface.ApplyPageSettings (new SolutionPage (), "SolutionPage",
				RDLocale.GetText ("SolutionPage"), /*solutionMasterBackColor*/ color);
			aboutPage = RDInterface.ApplyPageSettings (new AboutPage (), "AboutPage",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout),
				aboutMasterBackColor);

			RDInterface.SetMasterPage (MainPage, solutionPage, /*solutionMasterBackColor*/ color);

			#region Основная страница

			masterField = (StackLayout)solutionPage.FindByName ("MasterField");

			StackLayout numbersField = new StackLayout ();
			numbersField.HorizontalOptions = numbersField.VerticalOptions = LayoutOptions.Center;
			numbersField.Orientation = StackOrientation.Vertical;

			// Сборка поля ввода матрицы
			/*uint sideSize = SudokuSolverMath.SudokuSideSize;
			uint sq = (uint)Math.Sqrt (sideSize);*/
			List<StackLayout> numbersSL = new List<StackLayout> ();

			for (int i = 0; i < /*sideSize * sideSize*/ SudokuSolverMath.FullSize; i++)
				{
				// Добавление горизонтальных пробелов
				/*if ((i != 0) && (i % (sq * sideSize) == 0))*/
				if ((i != 0) && (i % (SudokuSolverMath.SquareSize * SudokuSolverMath.SideSize) == 0))
					{
					Label l = new Label ();
					l.WidthRequest = l.HeightRequest = 5;
					numbersField.Add (l);
					}

				// Добавление строковых полей и вертикальных пробелов
				/*if (i % sideSize == 0)*/
				if ((i % SudokuSolverMath.SideSize) == 0)
					{
					StackLayout sl = new StackLayout ();
					sl.Orientation = StackOrientation.Horizontal;
					sl.HorizontalOptions = LayoutOptions.Center;
					numbersSL.Add (sl);
					numbersField.Add (sl);
					}
				/*else if (i % sq == 0)*/
				else if ((i % SudokuSolverMath.SquareSize) == 0)
					{
					Label l = new Label ();
					l.WidthRequest = l.HeightRequest = 5;
					numbersSL[numbersSL.Count - 1].Add (l);
					}

				// Добавление кнопок
				Button b = new Button ();
				/*b.BackgroundColor = deselectedButtonColor;*/
				b.FontAttributes = FontAttributes.None;
				b.FontSize = 5 * RDInterface.MasterFontSize / 4;
				b.FontAttributes = FontAttributes.Bold;
				b.FontFamily = RDGenerics.MonospaceFont;
				SudokuSolverMath.SetProperty (b, PropertyTypes.OldColor);
				b.LineBreakMode = LineBreakMode.WordWrap;
				b.WidthRequest = b.HeightRequest = RDInterface.MasterFontSize * 2.25;
				b.Padding = Thickness.Zero;
				b.Margin = new Thickness (1);
				SudokuSolverMath.SetProperty (b, PropertyTypes.EmptyValue);
				b.TextTransform = TextTransform.None;
				b.Clicked += SelectCurrentButton;

				numberButtons.Add (b);
				numbersSL[numbersSL.Count - 1].Add (b);
				}

			masterField.Add (numbersField);

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
				/*b.BackgroundColor = solutionFieldBackColor;*/
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

			// Разделитель
			Label msp = new Label ();
			msp.WidthRequest = msp.HeightRequest = 10;
			inputField.Add (msp);

			StackLayout msl = new StackLayout ();
			msl.Orientation = StackOrientation.Horizontal;
			msl.HorizontalOptions = LayoutOptions.Center;
			inputSL.Add (msl);
			inputField.Add (msl);

			// Добавление управляющих кнопок
			generateButton = RDInterface.ApplyButtonSettings (solutionPage, null, RDDefaultButtons.Menu,
				/*solutionFieldBackColor*/ color, GenerateMatrix_Clicked);
			generateButton.Text = "🆕";
			inputSL[inputSL.Count - 1].Add (generateButton);

			clearButton = RDInterface.ApplyButtonSettings (solutionPage, null, RDDefaultButtons.Menu,
				/*solutionFieldBackColor*/ color, ClearSolution_Clicked);
			clearButton.Text = "🔄";
			inputSL[inputSL.Count - 1].Add (clearButton);

			solutionButton = RDInterface.ApplyButtonSettings (solutionPage, null, RDDefaultButtons.Menu,
				/*solutionFieldBackColor*/ color, SolveSudoku_Clicked);
			solutionButton.Text = "✅";
			solutionButton.WidthRequest *= 2;
			inputSL[inputSL.Count - 1].Add (solutionButton);

			checkButton = RDInterface.ApplyButtonSettings (solutionPage, null, RDDefaultButtons.Menu,
				/*solutionFieldBackColor*/ color, CheckSolution_Clicked);
			checkButton.Text = "☑️";
			checkButton.WidthRequest *= 2;
			inputSL[inputSL.Count - 1].Add (checkButton);

			menuButton = RDInterface.ApplyButtonSettings (solutionPage, null, RDDefaultButtons.Menu,
				/*solutionFieldBackColor*/ color, MenuButton_Clicked);
			inputSL[inputSL.Count - 1].Add (menuButton);

			masterField.Add (inputField);

			// Загрузка расположения клавиатуры
			SetInterfaceState (true);

			// Загрузка цветовой схемы
			SelectColorScheme (true);

			// Загрузка сохранённого состояния
			string sudoku = SudokuSolverMath.SudokuField;
			if (sudoku.Length == SudokuSolverMath.FullSize)
				{
				for (int i = 0; i < numberButtons.Count; i++)
					{
					numberButtons[i].Text = sudoku[i].ToString ();
					if (!SudokuSolverMath.CheckCondition (numberButtons[i], ConditionTypes.IsEmpty))
						SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.OldColor);
					}
				}

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
			FlushMatrix ();
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
				menuVariants.Add (new List<string> ());
				menuVariants[0].Add ("🔢\t " + RDLocale.GetText ("Menu0"));
				menuVariants[0].Add ("🕹\t " + RDLocale.GetText ("Menu1"));
				menuVariants[0].Add ("⚙️\t " + RDLocale.GetText ("Menu2"));
				menuVariants[0].Add ("ℹ️\t " + RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout));

				menuVariants.Add (new List<string> ());
				menuVariants[1].Add ("✅\t " + RDLocale.GetText ("SolveButton"));   // [0]
				menuVariants[1].Add ("🔄\t " + RDLocale.GetText ("ResetSolution"));    // [2]
				menuVariants[1].Add ("❌\t " + RDLocale.GetText ("ResetField"));    // [3]
				menuVariants[1].Add ("📄\t " + RDLocale.GetText ("LoadFromFile")); // [5]
				menuVariants[1].Add ("💾\t " + RDLocale.GetText ("SaveToFile"));   // [6]

				menuVariants.Add (new List<string> ());
				menuVariants[2].Add ("🆕\t " + RDLocale.GetText ("GenerateMatrix"));   // [4]
				menuVariants[2].Add ("☑️\t " + RDLocale.GetText ("CheckSolutionButton"));  // [1]
				menuVariants[2].Add ("📊\t " + RDLocale.GetText ("StatsButton"));  // [9]

				menuVariants.Add (new List<string> ());
				menuVariants[3].Add ("🔢\t " + RDLocale.GetText ("KeyboardButton"));   // [7]
				menuVariants[3].Add ("📱\t " + RDLocale.GetText ("ModeButton"));   // [8]
				menuVariants[3].Add ("🔳\t " + RDLocale.GetText ("ColorScheme"));  // [10]

				/*menuVariants.Add ("ℹ️\t " + RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout));    // [11]*/
				}

			// Верхнее меню
			int firstMenu = await RDInterface.ShowList (RDLocale.GetText ("MenuButton") + ":",
				RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel), menuVariants[0]);
			if (firstMenu < 0)
				return;

			// Второе меню
			if (firstMenu < 3)
				{
				/*firstMenu = 10 * (firstMenu + 1);*/
				firstMenu++;

				int secondMenu = await RDInterface.ShowList (RDLocale.GetText ("MenuButton") + ":",
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel), menuVariants[firstMenu]);
				if (secondMenu < 0)
					return;

				firstMenu = firstMenu * 10 + secondMenu;
				}

			// Выполнение
			switch (firstMenu)
				{
				// [11] О приложении
				case 3:
					RDInterface.SetCurrentPage (aboutPage, aboutMasterBackColor);
					break;

				// [0] Выполнить решение
				case 10:
					await FindSolution (true);
					break;

				// [2] Сброс решения
				case 11:
					ClearSolution_Clicked (null, null);
					break;

				// [3] Полный сброс
				case 12:
					if (!await RDInterface.ShowMessage (RDLocale.GetText ("ResetWarning"),
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_Yes),
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_No)))
						return;

					for (int i = 0; i < numberButtons.Count; i++)
						SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.EmptyValue);
					SudokuSolverMath.GameMode = MatrixDifficulty.None;
					break;

				// [5] Загрузка из файла
				case 13:
					await LoadFromFile ();
					break;

				// [6] Сохранение в файл
				case 14:
					await SaveToFile ();
					break;

				// [4] Генерация матрицы
				case 20:
					await GenerateMatrix ();
					break;

				// [1] Проверить корректность решения
				case 21:
					if (!await FindSolution (false))
						RDInterface.ShowBalloon ("❌ " + RDLocale.GetText ("SolutionIsIncorrect"), true);
					break;

				// [9] Статистика игры
				case 22:
					await ShowScore (false);
					break;

				// [7] Сменить расположение клавиатуры
				case 30:
					await SelectKeyboardPlacement (false);
					break;

				// [8] Сменить режим работы
				case 31:
					await SelectAppMode (false);
					break;

				// [10] Выбор цветовой схемы
				case 32:
					await SelectColorScheme (false);
					break;
				}
			}

		// Сброс полученного решения
		private void ClearSolution_Clicked (object sender, EventArgs e)
			{
			for (int i = 0; i < numberButtons.Count; i++)
				if (SudokuSolverMath.CheckCondition (numberButtons[i], ConditionTypes.ContainsFoundValue))
					SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.EmptyValue);
			}

		// Выбор текущей кнопки в матрице
		private void SelectCurrentButton (object sender, EventArgs e)
			{
			currentButtonIndex = numberButtons.IndexOf ((Button)sender);

			// Кнопка уже была выбрана – выполнить приращение
			/*if (numberButtons[currentButtonIndex].BackgroundColor == selectedButtonColor)*/
			if (SudokuSolverMath.CheckCondition (numberButtons[currentButtonIndex], ConditionTypes.SelectedCell))
				{
				uint v = 0;
				Button b = numberButtons[currentButtonIndex];

				// В игровом режиме изменение проверенных ячеек запрещено
				if ((SudokuSolverMath.GameMode != MatrixDifficulty.None) &&
					!SudokuSolverMath.CheckCondition (b, ConditionTypes.IsEmpty) &&
					!SudokuSolverMath.CheckCondition (b, ConditionTypes.ContainsNewValue))
					return;

				// Задание значения
				try
					{
					v = uint.Parse (b.Text) + 1;
					}
				catch { }

				if (v == 0)
					b.Text = "1";
				else if (v > 9)
					SudokuSolverMath.SetProperty (b, PropertyTypes.EmptyValue);
				else
					b.Text = v.ToString ();
				SudokuSolverMath.SetProperty (b, PropertyTypes.NewColor);
				}

			// Кнопка выбрана впервые – выполнить изменение цветов
			else
				{
				for (int i = 0; i < numberButtons.Count; i++)
					{
					if (i == currentButtonIndex)
						/*numberButtons[i].BackgroundColor = selectedButtonColor;*/
						SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.SelectedCell);
					else
						/*numberButtons[i].BackgroundColor = deselectedButtonColor;*/
						SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.DeselectedCell);
					}
				}
			}
		private int currentButtonIndex = -1;

		// Выбор значения для текущей кнопки в матрице
		private void SetValueForCurrentButton (object sender, EventArgs e)
			{
			// Контроль
			if (currentButtonIndex < 0)
				return;

			// Выполнение
			int idx = inputButtons.IndexOf ((Button)sender);
			Button b = numberButtons[currentButtonIndex];

			// В игровом режиме изменение проверенных ячеек запрещено
			if ((SudokuSolverMath.GameMode != MatrixDifficulty.None) &&
				!SudokuSolverMath.CheckCondition (b, ConditionTypes.IsEmpty) &&
				!SudokuSolverMath.CheckCondition (b, ConditionTypes.ContainsNewValue))
				return;

			if (idx > 0)
				b.Text = idx.ToString ();
			else
				SudokuSolverMath.SetProperty (b, PropertyTypes.EmptyValue);
			SudokuSolverMath.SetProperty (b, PropertyTypes.NewColor);
			}

		// Выбор расположения клавиатуры
		private async Task<bool> SelectKeyboardPlacement (bool Initial)
			{
			// Выбор варианта
			if (keyboardVariants.Count < 1)
				{
				keyboardVariants.Add ("❌\t " + RDLocale.GetText ("KeyboardVariant0"));
				keyboardVariants.Add ("➡️\t " + RDLocale.GetText ("KeyboardVariant1"));
				keyboardVariants.Add ("⬇️\t " + RDLocale.GetText ("KeyboardVariant2"));
				}

			int res;
			if (Initial)
				{
				res = (int)SudokuSolverMath.KeyboardPlacement;
				}
			else
				{
				res = await RDInterface.ShowList (RDLocale.GetText ("KeyboardPlacement"),
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel), keyboardVariants);
				if (res < 0)
					return false;
				SudokuSolverMath.KeyboardPlacement = (KeyboardPlacements)res;
				}

			// Настройка
			/*inputField.IsVisible = (res > 0);*/
			for (int i = 0; i < inputButtons.Count; i++)
				inputButtons[i].IsVisible = (res > 0);

			if (SudokuSolverMath.KeyboardPlacement == KeyboardPlacements.Right)
				masterField.Orientation = StackOrientation.Horizontal;
			else
				masterField.Orientation = StackOrientation.Vertical;
			return true;
			}

		// Метод выполняет решение судоку или его проверку
		private async void SolveSudoku_Clicked (object sender, EventArgs e)
			{
			await FindSolution (true);
			}

		private async void CheckSolution_Clicked (object sender, EventArgs e)
			{
			if (!await FindSolution (false))
				RDInterface.ShowBalloon ("❌ " + RDLocale.GetText ("SolutionIsIncorrect"), true);
			}

		private async Task<bool> FindSolution (bool LoadResults)
			{
			// Остановка решения
			if (!masterField.IsEnabled)
				{
				SudokuSolverMath.RequestStop ();
				return true;
				}

			// Сборка массива
			Byte[,] matrix = new Byte[SudokuSolverMath.SideSize, SudokuSolverMath.SideSize];
			for (int r = 0; r < SudokuSolverMath.SideSize; r++)
				{
				for (int c = 0; c < SudokuSolverMath.SideSize; c++)
					{
					Button ct = numberButtons[r * (int)SudokuSolverMath.SideSize + c];
					if (!SudokuSolverMath.CheckCondition (ct, ConditionTypes.IsEmpty) &&
						!SudokuSolverMath.CheckCondition (ct, ConditionTypes.ContainsFoundValue))
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
					if (LoadResults)
						for (int i = 0; i < numberButtons.Count; i++)
							SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.ErrorColor);

					return false;
				}

			// Решение задачи
			SetInterfaceState (false);

			await Task.Run<bool> (SudokuSolverMath.FindSolution);

			SetInterfaceState (true);

			// Разбор решения
			switch (SudokuSolverMath.CurrentStatus)
				{
				case SolutionResults.NoSolutionsFound:
				case SolutionResults.NotInited:
					if (LoadResults)
						for (int i = 0; i < numberButtons.Count; i++)
							SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.ErrorColor);

					return false;

				case SolutionResults.SearchAborted: // Не перекрашивать поле
					return false;
				}

			// Игровой режим
			if (!LoadResults)
				{
				// Цвет новых ячеек меняется на фоновый
				uint newCellsCount = 0, emptyCellsCount = 0;
				bool gameMode = (SudokuSolverMath.GameMode != MatrixDifficulty.None);

				for (int i = 0; i < numberButtons.Count; i++)
					{
					if (gameMode)
						{
						if (SudokuSolverMath.CheckCondition (numberButtons[i], ConditionTypes.IsEmpty))
							emptyCellsCount++;
						else if (SudokuSolverMath.CheckCondition (numberButtons[i], ConditionTypes.ContainsNewValue))
							newCellsCount++;
						}

					SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.OldColor);
					}

				// Контроль матрицы на неизменность
				if (gameMode)
					{
					// Расчёт очков
					uint score = ((uint)SudokuSolverMath.GameMode + 1) * newCellsCount * newCellsCount;
					if (emptyCellsCount < 2)
						{
						switch (SudokuSolverMath.GameMode)
							{
							case MatrixDifficulty.Easy:
								score += 1000;
								SudokuSolverMath.EasyScore++;
								break;

							case MatrixDifficulty.Medium:
								score += 2000;
								SudokuSolverMath.MediumScore++;
								break;

							case MatrixDifficulty.Hard:
								score += 3000;
								SudokuSolverMath.HardScore++;
								break;
							}
						}
					SudokuSolverMath.TotalScore += score;

					// Отображение результата и отключение игрового режима до следующей генерации
					RDInterface.ShowBalloon ("✅ " + RDLocale.GetText ("SolutionIsCorrect") + RDLocale.RNRN +
						"+" + score.ToString () + " 💎", true);

					// Отобразить решение в случае выигрыша (без return; режим игры отключается далее)
					if (emptyCellsCount < 2)
						{
						await ShowScore (true);
						/*SudokuSolverMath.GameMode = MatrixDifficulty.None;*/
						}

					// Иначе продолжить игру
					else
						{
						return true;
						}
					}

				// Не отображать решение вне игрового режима
				else
					{
					RDInterface.ShowBalloon ("✅ " + RDLocale.GetText ("SolutionIsCorrect"), true);
					return true;
					}

				/*return true;*/
				}

			// Отображение решения
			SudokuSolverMath.GameMode = MatrixDifficulty.None;
			for (int r = 0; r < SudokuSolverMath.SideSize; r++)
				{
				for (int c = 0; c < SudokuSolverMath.SideSize; c++)
					{
					Button ct = numberButtons[r * (int)SudokuSolverMath.SideSize + c];
					if (!SudokuSolverMath.CheckCondition (ct, ConditionTypes.IsEmpty) &&
						!SudokuSolverMath.CheckCondition (ct, ConditionTypes.ContainsFoundValue))
						{
						SudokuSolverMath.SetProperty (ct, PropertyTypes.OldColor);
						}
					else
						{
						ct.Text = SudokuSolverMath.ResultMatrix[r, c].ToString ();
						SudokuSolverMath.SetProperty (ct, PropertyTypes.SuccessColor);
						}
					}
				}

			// Выполнено
			return true;
			}

		// Метод отображает игровую статистику
		private async Task<bool> ShowScore (bool AsWin)
			{
			string text = "";

			if (AsWin)
				text += (RDLocale.GetText ("SolvedText") + RDLocale.RNRN);

			text += (RDLocale.GetText ("StatsText") + RDLocale.RNRN);
			text += ("💎\t" + SudokuSolverMath.TotalScore.ToString ("#,#0") + "\t\t");
			text += ("🟢\t" + SudokuSolverMath.EasyScore.ToString () + "\t\t");
			text += ("🟡\t" + SudokuSolverMath.MediumScore.ToString () + "\t\t");
			text += ("🔴\t" + SudokuSolverMath.HardScore.ToString ());

			await RDInterface.ShowMessage (text, RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));
			return true;
			}

		// Метод выполняет блокировку / разблокировку интерфейса
		private void SetInterfaceState (bool Enabled)
			{
			masterField.IsEnabled = menuButton.IsVisible = Enabled;
			if (!Enabled)
				{
				generateButton.IsVisible = clearButton.IsVisible = checkButton.IsVisible = false;
				solutionButton.IsVisible = true;
				solutionButton.Text = "❌";
				/*solveButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel);*/
				}
			else
				{
				solutionButton.Text = "✅";
				SelectKeyboardPlacement (true);
				SelectAppMode (true);
				/*solveButton.Text = RDLocale.GetText ("SolveButton");*/
				}
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
				if (SudokuSolverMath.CheckCondition (numberButtons[i], ConditionTypes.IsEmpty))
					SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.NewColor);
				else
					SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.OldColor);
				}

			// Сброс игрового режима
			SudokuSolverMath.GameMode = MatrixDifficulty.None;

			// Успешно
			return true;
			}

		// Метод сохраняет матрицу в файл
		private async Task<bool> SaveToFile ()
			{
			// Выгрузка данных
			FlushMatrix ();
			string file = SudokuSolverMath.BuildMatrixToSave (SudokuSolverMath.SudokuField);

			// Сохранение
			return await RDGenerics.SaveToFile ("Sudoku.txt", file, RDEncodings.UTF8);
			}

		// Генерация матрицы судоку
		private async void GenerateMatrix_Clicked (object sender, EventArgs e)
			{
			await GenerateMatrix ();
			}

		private async Task<bool> GenerateMatrix ()
			{
			// Выбор сложности
			if (difficultyVariants.Count < 1)
				{
				difficultyVariants.Add ("🟢\t " + RDLocale.GetText ("Difficulty0"));
				difficultyVariants.Add ("🟡\t " + RDLocale.GetText ("Difficulty1"));
				difficultyVariants.Add ("🔴\t " + RDLocale.GetText ("Difficulty2"));
				}

			int res = await RDInterface.ShowList (RDLocale.GetText ("DifficultyLevel"),
				RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel), difficultyVariants);
			if (res < 0)
				return false;

			// Запуск
			SetInterfaceState (false);
			solutionButton.IsVisible = false;

			SudokuSolverMath.SetGenerationDifficulty ((MatrixDifficulty)res);
			await Task.Run<bool> (SudokuSolverMath.GenerateMatrix);

			SetInterfaceState (true);

			// Отображение результата
			for (int r = 0; r < SudokuSolverMath.SideSize; r++)
				{
				for (int c = 0; c < SudokuSolverMath.SideSize; c++)
					{
					Button ct = numberButtons[r * (int)SudokuSolverMath.SideSize + c];
					if (SudokuSolverMath.ResultMatrix[r, c] == 0)
						SudokuSolverMath.SetProperty (ct, PropertyTypes.EmptyValue);
					else
						ct.Text = SudokuSolverMath.ResultMatrix[r, c].ToString ();
					SudokuSolverMath.SetProperty (ct, PropertyTypes.OldColor);
					}
				}

			// Взведение игрового режима
			SudokuSolverMath.GameMode = (MatrixDifficulty)res;

			// Завершено
			return true;
			}

		// Выбор режима приложения
		private async Task<bool> SelectAppMode (bool Initial)
			{
			// Выбор варианта
			if (appModeVariants.Count < 1)
				{
				appModeVariants.Add ("✅\t " + RDLocale.GetText ("Mode0"));
				appModeVariants.Add ("🕹\t " + RDLocale.GetText ("Mode1"));
				}

			int res;
			if (Initial)
				{
				res = (int)SudokuSolverMath.AppMode;
				}
			else
				{
				res = await RDInterface.ShowList (RDLocale.GetText ("ModeButton") + ":",
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel), appModeVariants);
				if (res < 0)
					return false;
				SudokuSolverMath.AppMode = (AppModes)res;
				}

			// Настройка
			bool game = (SudokuSolverMath.AppMode == AppModes.Game);

			generateButton.IsVisible = checkButton.IsVisible = game;
			solutionButton.IsVisible = clearButton.IsVisible = !game;
			if (!game)
				SudokuSolverMath.GameMode = MatrixDifficulty.None;

			return true;
			}

		// Метод формирует из текущего состояния таблицы сплошную строку и отправляет её на сохранение
		private void FlushMatrix ()
			{
			string sudoku = "";
			for (int i = 0; i < numberButtons.Count; i++)
				sudoku += numberButtons[i].Text;

			SudokuSolverMath.SudokuField = sudoku;
			}

		// Выбор цветовой схемы приложения
		private async Task<bool> SelectColorScheme (bool Initial)
			{
			// Выбор варианта
			if (colorSchemeVariants.Count < 1)
				{
				colorSchemeVariants.Add ("⚪️\t " + RDLocale.GetText ("Color0"));
				colorSchemeVariants.Add ("⚫️\t " + RDLocale.GetText ("Color1"));
				}

			int res;
			if (Initial)
				{
				res = (int)SudokuSolverMath.ColorScheme;
				}
			else
				{
				res = await RDInterface.ShowList (RDLocale.GetText ("ColorScheme") + ":",
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel), colorSchemeVariants);
				if (res < 0)
					return false;
				SudokuSolverMath.ColorScheme = (ColorSchemes)res;
				}

			// Настройка
			solutionPage.BackgroundColor = SudokuSolverMath.BackgroundColor;
			for (int i = 0; i < numberButtons.Count; i++)
				{
				SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.DeselectedCell);

				// Переназначение цветов для дальнейшей корректной работы метода CheckCondition
				if (SudokuSolverMath.CheckCondition (numberButtons[i], ConditionTypes.ContainsFoundValue))
					SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.SuccessColor);
				else if (SudokuSolverMath.CheckCondition (numberButtons[i], ConditionTypes.ContainsNewValue))
					SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.NewColor);
				else if (SudokuSolverMath.CheckCondition (numberButtons[i], ConditionTypes.ContainsErrorValue))
					SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.ErrorColor);
				else /*if (SudokuSolverMath.CheckCondition (numberButtons[i], ConditionTypes.ContainsOldValue))*/
					SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.OldColor);
				}

			for (int i = 0; i < inputButtons.Count; i++)
				{
				SudokuSolverMath.SetProperty (inputButtons[i], PropertyTypes.OtherButton);
				SudokuSolverMath.SetProperty (inputButtons[i], PropertyTypes.OldColor);
				}

			SudokuSolverMath.SetProperty (generateButton, PropertyTypes.OtherButton);
			SudokuSolverMath.SetProperty (clearButton, PropertyTypes.OtherButton);
			SudokuSolverMath.SetProperty (checkButton, PropertyTypes.OtherButton);
			SudokuSolverMath.SetProperty (solutionButton, PropertyTypes.OtherButton);
			SudokuSolverMath.SetProperty (menuButton, PropertyTypes.OtherButton);
			SudokuSolverMath.SetProperty (menuButton, PropertyTypes.OldColor);

			return true;
			}

		#endregion
		}
	}
