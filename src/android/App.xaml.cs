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

		// Параметры запуска приложения
		private RDAppStartupFlags flags;

		// Цветовая схема
		private Color aboutMasterBackColor = Color.FromArgb ("#F0FFF0");
		private Color aboutFieldBackColor = Color.FromArgb ("#D0FFD0");
		private Color stubColor = RDInterface.GetInterfaceColor (RDInterfaceColors.MediumGrey);

		// Контекстные меню
		private List<string> keyboardVariants = [];
		private List<string> difficultyVariants = [];
		private List<string> appModeVariants = [];
		private List<string> colorSchemeVariants = [];
		private List<List<string>> menuVariants = [];
		private List<string> appearanceVariants = [];

		// Номер текущей выбранной кнопки
		private int currentButtonIndex = -1;

		// Префиксы режимов
		private const string easyPrefix = "🟢\t ";
		private const string mediumPrefix = "🟡\t ";
		private const string hardPrefix = "🔴\t ";
		private const string gemSuffix = "💎";

		#endregion

		#region Переменные страниц

		private ContentPage solutionPage, aboutPage;

		private Label aboutFontSizeField, solutionTipLabel;

		private Button languageButton, solutionButton, checkButton, generateButton,
			clearButton, menuButton;
		private List<Button> numberButtons = [];
		private List<Button> inputButtons = [];

		private StackLayout masterField;
		private StackLayout numbersField = [];
		private StackLayout inputField = [];

		#endregion

		#region Запуск и настройка

		/// <summary>
		/// Конструктор. Точка входа приложения
		/// </summary>
		public App ()
			{
			// Инициализация
			InitializeComponent ();
			}

		// Замена определению MainPage = new MasterPage ()
		protected override Window CreateWindow (IActivationState activationState)
			{
			return new Window (AppShell ());
			}

		// Инициализация разметки страниц
		private Page AppShell ()
			{
			Page mainPage = new MasterPage ();
			flags = RDGenerics.GetAppStartupFlags (RDAppStartupFlags.DisableXPUN);

			// Общая конструкция страниц приложения
			solutionPage = RDInterface.ApplyPageSettings (new SolutionPage (), "SolutionPage",
				RDLocale.GetText ("SolutionPage"), stubColor);
			aboutPage = RDInterface.ApplyPageSettings (new AboutPage (), "AboutPage",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout),
				aboutMasterBackColor);

			RDInterface.SetMasterPage (mainPage, solutionPage, stubColor);

			#region Основная страница

			masterField = (StackLayout)solutionPage.FindByName ("MasterField");

			numbersField.HorizontalOptions = numbersField.VerticalOptions = LayoutOptions.Center;
			numbersField.Orientation = StackOrientation.Vertical;

			// Сборка поля ввода матрицы
			List<StackLayout> numbersSL = [];

			for (int i = 0; i < SudokuSolverMath.FullSize; i++)
				{
				// Добавление горизонтальных пробелов
				if ((i != 0) && (i % (SudokuSolverMath.SquareSize * SudokuSolverMath.SideSize) == 0))
					{
					Label l = new Label ();
					l.WidthRequest = l.HeightRequest = 5;
					numbersField.Add (l);
					}

				// Добавление строковых полей и вертикальных пробелов
				if ((i % SudokuSolverMath.SideSize) == 0)
					{
					StackLayout sl = [];
					sl.Orientation = StackOrientation.Horizontal;
					sl.HorizontalOptions = LayoutOptions.Center;
					numbersSL.Add (sl);
					numbersField.Add (sl);
					}
				else if ((i % SudokuSolverMath.SquareSize) == 0)
					{
					Label l = new Label ();
					l.WidthRequest = l.HeightRequest = 5;
					numbersSL[numbersSL.Count - 1].Add (l);
					}

				// Добавление кнопок
				Button b = new Button ();
				b.FontAttributes = FontAttributes.None;
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

			List<StackLayout> inputSL = [];

			for (int i = 9; i >= 0; i--)
				{
				// Добавление строковых полей
				if (i % 3 == 0)
					{
					StackLayout sl = [];
					sl.Orientation = StackOrientation.Horizontal;
					sl.HorizontalOptions = LayoutOptions.Center;
					inputSL.Add (sl);
					inputField.Add (sl);
					}

				Button b = new Button ();
				b.FontAttributes = FontAttributes.None;
				b.FontFamily = RDGenerics.MonospaceFont;
				b.TextColor = RDInterface.GetInterfaceColor (RDInterfaceColors.AndroidTextColor);
				b.LineBreakMode = LineBreakMode.WordWrap;
				b.Padding = Thickness.Zero;
				b.Margin = new Thickness (3);
				b.TextTransform = TextTransform.None;
				b.Clicked += SetValueForCurrentButton;

				if (i != 0)
					{
					b.WidthRequest = b.HeightRequest = RDInterface.MasterFontSize * 2.75;
					}
				else
					{
					b.FontSize = 5 * RDInterface.MasterFontSize / 4;
					b.Text = RDLocale.GetText ("EmptyButton");
					}

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

			StackLayout msl = [];
			msl.Orientation = StackOrientation.Horizontal;
			msl.HorizontalOptions = LayoutOptions.Center;
			inputSL.Add (msl);
			inputField.Add (msl);

			// Добавление управляющих кнопок
			generateButton = RDInterface.ApplyButtonSettings (solutionPage, null, RDDefaultButtons.Menu,
				stubColor, GenerateMatrix_Clicked);
			generateButton.Text = "🆕";
			inputSL[inputSL.Count - 1].Add (generateButton);

			checkButton = RDInterface.ApplyButtonSettings (solutionPage, null, RDDefaultButtons.Menu,
				stubColor, CheckSolution_Clicked);
			checkButton.Text = "☑️";
			checkButton.WidthRequest *= 2;
			inputSL[inputSL.Count - 1].Add (checkButton);

			clearButton = RDInterface.ApplyButtonSettings (solutionPage, null, RDDefaultButtons.Menu,
				stubColor, ClearSolution_Clicked);
			clearButton.Text = "↩️";
			inputSL[inputSL.Count - 1].Add (clearButton);

			solutionButton = RDInterface.ApplyButtonSettings (solutionPage, null, RDDefaultButtons.Menu,
				stubColor, SolveSudoku_Clicked);
			solutionButton.Text = "✅";
			solutionButton.WidthRequest *= 2;
			inputSL[inputSL.Count - 1].Add (solutionButton);

			menuButton = RDInterface.ApplyButtonSettings (solutionPage, null, RDDefaultButtons.Menu,
				stubColor, MenuButton_Clicked);
			inputSL[inputSL.Count - 1].Add (menuButton);

			// Подсказка о выполнении решения
			solutionTipLabel = RDInterface.ApplyLabelSettings (solutionPage, null, RDLocale.GetText ("SolutionTip"),
				RDLabelTypes.TipCenter);
			inputField.Add (solutionTipLabel);

			masterField.Add (inputField);

			// Загрузка расположения клавиатуры
			SetInterfaceState (true);

			// Загрузка цветовой схемы и представления
			SelectColorScheme (true);
			SelectAppearance (true);    // Загружает сохранённое состояние

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
			return mainPage;
			}

		// Метод отображает подсказки при первом запуске
		private async void ShowStartupTips ()
			{
			// Контроль XPUN
			if (!flags.HasFlag (RDAppStartupFlags.DisableXPUN))
				await RDInterface.XPUNLoop ();

			// Требование принятия Политики
			await RDInterface.PolicyLoop ();

			// Приветствие
			if (!((TipTypes)RDGenerics.TipsState).HasFlag (TipTypes.WelcomeTip))
				{
				await RDInterface.ShowMessage (RDLocale.GetText ("WelcomeTip"),
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));
				RDGenerics.TipsState |= (uint)TipTypes.WelcomeTip;
				}
			}

		/// <summary>
		/// Сохранение настроек программы
		/// </summary>
		protected override void OnSleep ()
			{
			// Сброс текущего решения
			ClearSolution_Clicked (null, null);

			// Сохранение
			FlushMatrix ();
			}

		/// <summary>
		/// Доступные типы уведомлений
		/// </summary>
		public enum TipTypes
			{
			/// <summary>
			/// Первая подсказка
			/// </summary>
			WelcomeTip = 0x02,
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
				menuVariants.Add ([]);
				menuVariants[0].Add ("🔢\t " + RDLocale.GetText ("Menu0"));
				menuVariants[0].Add ("🕹\t " + RDLocale.GetText ("Menu1"));
				menuVariants[0].Add ("📱\t " + RDLocale.GetText ("ModeButton"));   // [8]
				menuVariants[0].Add ("⚙️\t " + RDLocale.GetText ("Menu2"));
				menuVariants[0].Add ("ℹ️\t " + RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout));

				menuVariants.Add ([]);
				menuVariants[1].Add ("✅\t " + RDLocale.GetText ("SolveButton"));   // [0]
				menuVariants[1].Add ("↩️\t " + RDLocale.GetText ("ClearSolution"));    // [2]
				menuVariants[1].Add ("❌\t " + RDLocale.GetText ("ResetField"));    // [3]
				menuVariants[1].Add ("📄\t " + RDLocale.GetText ("LoadFromFile")); // [5]
				menuVariants[1].Add ("💾\t " + RDLocale.GetText ("SaveToFile"));   // [6]

				menuVariants.Add ([]);
				menuVariants[2].Add ("🆕\t " + RDLocale.GetText ("GenerateMatrix"));   // [4]
				menuVariants[2].Add ("☑️\t " + RDLocale.GetText ("CheckSolutionButton"));  // [1]
				menuVariants[2].Add ("📊\t " + RDLocale.GetText ("StatsButton"));  // [9]

				menuVariants.Add ([]);
				menuVariants[3].Add ("🔢\t " + RDLocale.GetText ("KeyboardButton"));   // [7]
				menuVariants[3].Add ("🔳\t " + RDLocale.GetText ("ColorScheme"));  // [10]
				menuVariants[3].Add ("*️⃣\t " + RDLocale.GetText ("CellsAppearance"));  // [10]
				}
			List<List<int>> indirectMenu = [
				[ 0, 1, 3 ],
				[ 1, 2, 3 ],
				];

			// Верхнее меню
			int firstMenu = await RDInterface.ShowList (RDLocale.GetText ("MenuButton") + ":",
				RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel), menuVariants[0]);
			if (firstMenu < 0)
				return;

			// Второе меню
			if (indirectMenu[0].Contains (firstMenu))
				{
				firstMenu = indirectMenu[1][indirectMenu[0].IndexOf (firstMenu)];

				int secondMenu = await RDInterface.ShowList (RDLocale.GetText ("MenuButton") + ":",
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel), menuVariants[firstMenu]);
				if (secondMenu < 0)
					return;

				firstMenu = firstMenu * 10 + secondMenu;
				}

			// Выполнение
			switch (firstMenu)
				{
				// [8] Сменить режим работы
				case 2:
					await SelectAppMode (false);
					break;

				// [11] О приложении
				case 4:
					RDInterface.SetCurrentPage (aboutPage, aboutMasterBackColor);
					break;

				// [0] Выполнить решение
				case 10:
					if (!await FindSolution (true))
						RDInterface.ShowBalloon ("❌ " + RDLocale.GetText ("SolutionIsIncorrect"), true);
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
						ApplyPenalty ();
					break;

				// [9] Статистика игры
				case 22:
					await ShowScore (false);
					break;

				// [7] Сменить расположение клавиатуры
				case 30:
					await SelectKeyboardPlacement (false);
					break;

				// [10] Выбор цветовой схемы
				case 31:
					await SelectColorScheme (false);
					break;

				// Выбор представления ячеек
				case 32:
					await SelectAppearance (false);
					break;
				}
			}

		// Метод применяет штраф
		private static void ApplyPenalty ()
			{
			uint score = SudokuSolverMath.GetScore (ScoreTypes.Penalty);
			SudokuSolverMath.UpdateGameScore (true, score);

			string text = "❌ " + RDLocale.GetText ("SolutionIsIncorrect");
			if (SudokuSolverMath.GameMode != MatrixDifficulty.None)
				text += (RDLocale.RNRN + "–" + score.ToString () + " " + gemSuffix);

			RDInterface.ShowBalloon (text, true);
			}

		// Сброс полученного решения
		private void ClearSolution_Clicked (object sender, EventArgs e)
			{
			bool game = (SudokuSolverMath.GameMode != MatrixDifficulty.None);

			for (int i = 0; i < numberButtons.Count; i++)
				{
				if (SudokuSolverMath.CheckCondition (numberButtons[i], ConditionTypes.ContainsFoundValue) ||
					game && SudokuSolverMath.CheckCondition (numberButtons[i], ConditionTypes.ContainsNewValue))
					{
					SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.EmptyValue);
					}
				}
			}

		// Выбор текущей кнопки в матрице
		private void SelectCurrentButton (object sender, EventArgs e)
			{
			currentButtonIndex = numberButtons.IndexOf ((Button)sender);

			// Кнопка уже была выбрана – выполнить приращение
			if (SudokuSolverMath.CheckCondition (numberButtons[currentButtonIndex], ConditionTypes.SelectedCell))
				{
				Byte v = 0;
				Button b = numberButtons[currentButtonIndex];

				// В игровом режиме изменение проверенных ячеек запрещено
				if ((SudokuSolverMath.GameMode != MatrixDifficulty.None) &&
					!SudokuSolverMath.CheckCondition (b, ConditionTypes.IsEmpty) &&
					!SudokuSolverMath.CheckCondition (b, ConditionTypes.ContainsNewValue))
					return;

				// Задание значения
				try
					{
					v = SudokuSolverMath.GetDigit (b.Text);
					v++;
					}
				catch { }

				if (v == 0)
					b.Text = SudokuSolverMath.GetAppearance (1);
				else if (v > 9)
					SudokuSolverMath.SetProperty (b, PropertyTypes.EmptyValue);
				else
					b.Text = SudokuSolverMath.GetAppearance (v);

				SudokuSolverMath.SetProperty (b, PropertyTypes.NewColor);
				}

			// Кнопка выбрана впервые – выполнить изменение цветов
			else
				{
				for (int i = 0; i < numberButtons.Count; i++)
					{
					if (i == currentButtonIndex)
						SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.SelectedCell);
					else
						SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.DeselectedCell);
					}
				}
			}

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
				b.Text = SudokuSolverMath.GetAppearance ((Byte)idx);
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
			if (!await FindSolution (true))
				RDInterface.ShowBalloon ("❌ " + RDLocale.GetText ("SolutionIsIncorrect"), true);
			}

		private async void CheckSolution_Clicked (object sender, EventArgs e)
			{
			if (!await FindSolution (false))
				ApplyPenalty ();
			}

		private async Task<bool> FindSolution (bool LoadResults)
			{
			// Остановка решения
			if (!numbersField.IsEnabled)
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
						matrix[r, c] = SudokuSolverMath.GetDigit (ct.Text);
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
					return true;    // Не считать нарушением правил
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
					uint score = SudokuSolverMath.GetScore (newCellsCount);
					if (emptyCellsCount < 2)
						score += SudokuSolverMath.GetScore (ScoreTypes.GameCompletion);

					SudokuSolverMath.UpdateGameScore (false, score);

					// Отображение результата и отключение игрового режима до следующей генерации
					RDInterface.ShowBalloon ("✅ " + RDLocale.GetText ("SolutionIsCorrect") + RDLocale.RNRN +
						"+" + score.ToString () + " " + gemSuffix, true);

					// Отобразить решение в случае выигрыша (без return; режим игры отключается далее)
					if (emptyCellsCount < 2)
						await ShowScore (true);

					// Иначе продолжить игру
					else
						return true;
					}

				// Не отображать решение вне игрового режима
				else
					{
					RDInterface.ShowBalloon ("✅ " + RDLocale.GetText ("SolutionIsCorrect"), true);
					return true;
					}
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
						ct.Text = SudokuSolverMath.GetAppearance (SudokuSolverMath.ResultMatrix[r, c]);
						SudokuSolverMath.SetProperty (ct, PropertyTypes.SuccessColor);
						}
					}
				}

			// Выполнено
			return true;
			}

		// Метод отображает игровую статистику
		private static async Task<bool> ShowScore (bool AsWin)
			{
			// Сборка
			string text = "";

			if (AsWin)
				text += (RDLocale.GetText ("SolvedText") + RDLocale.RNRN);

			/*string[] stats = SudokuSolverMath.StatsValues;
			string s0 = ("💎\t" + stats[0] + "\t\t");
			s0 += ("🟢\t" + stats[1] + "\t\t");
			s0 += ("🟡\t" + stats[2] + "\t\t");
			s0 += ("🔴\t" + stats[3]);

			text += (string.Format (RDLocale.GetText ("StatsText"), s0, stats[4], stats[5]) + RDLocale.RNRN);*/
			string[] stats = SudokuSolverMath.StatsValuesV3;
			text += (string.Format (RDLocale.GetText ("StatsText"), gemSuffix + "\t " + stats[0],
				easyPrefix + stats[1] + "\t\t" + mediumPrefix + stats[2] + "\t\t" + hardPrefix + stats[3],
				easyPrefix + stats[4] + "\t\t" + mediumPrefix + stats[5] + "\t\t" + hardPrefix + stats[6],
				easyPrefix + stats[7] + "\t\t" + mediumPrefix + stats[8] + "\t\t" + hardPrefix + stats[9]));

			// Отображение
			if (AsWin)
				{
				await RDInterface.ShowMessage (text, RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));
				return true;
				}

			if (await RDInterface.ShowMessage (text, RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK),
				RDLocale.GetText ("ShareButton")))
				return true;

			// Отправка
			await Share.RequestAsync (ProgramDescription.AssemblyVisibleName + RDLocale.RNRN +
				text, ProgramDescription.AssemblyVisibleName);
			return true;
			}

		// Метод выполняет блокировку / разблокировку интерфейса
		private void SetInterfaceState (bool Enabled)
			{
			numbersField.IsEnabled = menuButton.IsVisible = Enabled;
			solutionTipLabel.IsVisible = !Enabled;
			for (int i = 0; i < inputButtons.Count; i++)
				inputButtons[i].IsEnabled = Enabled;

			if (!Enabled)
				{
				generateButton.IsVisible = clearButton.IsVisible = checkButton.IsVisible = false;
				solutionButton.IsVisible = true;
				solutionButton.Text = "❌";
				}
			else
				{
				solutionButton.Text = "✅";
				SelectKeyboardPlacement (true);
				SelectAppMode (true);
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
				numberButtons[i].Text = SudokuSolverMath.GetAppearance (line[i].ToString ());
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
				/*difficultyVariants.Add ("🟢\t " + RDLocale.GetText ("Difficulty0"));
				difficultyVariants.Add ("🟡\t " + RDLocale.GetText ("Difficulty1"));
				difficultyVariants.Add ("🔴\t " + RDLocale.GetText ("Difficulty2"));*/
				difficultyVariants.Add (easyPrefix + RDLocale.GetText ("Difficulty0"));
				difficultyVariants.Add (mediumPrefix + RDLocale.GetText ("Difficulty1"));
				difficultyVariants.Add (hardPrefix + RDLocale.GetText ("Difficulty2"));
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
						ct.Text = SudokuSolverMath.GetAppearance (SudokuSolverMath.ResultMatrix[r, c]);
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

			/*int res;
			if (Initial)
				{
				res = (int)SudokuSolverMath.AppMode;
				}
			else*/
			if (!Initial)
				{
				int res = await RDInterface.ShowList (RDLocale.GetText ("ModeButton") + ":",
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel), appModeVariants);
				if (res < 0)
					return false;
				SudokuSolverMath.AppMode = (AppModes)res;
				}

			// Настройка
			bool game = (SudokuSolverMath.AppMode == AppModes.Game);

			generateButton.IsVisible = checkButton.IsVisible = game;
			solutionButton.IsVisible = !game;
			clearButton.IsVisible = true;
			if (!game)
				SudokuSolverMath.GameMode = MatrixDifficulty.None;

			return true;
			}

		// Метод формирует из текущего состояния таблицы сплошную строку и отправляет её на сохранение
		private void FlushMatrix ()
			{
			string sudoku = "";
			for (int i = 0; i < numberButtons.Count; i++)
				sudoku += SudokuSolverMath.GetDigit (numberButtons[i].Text).ToString ();

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

			/*int res;
			if (Initial)
				{
				res = (int)SudokuSolverMath.ColorScheme;
				}
			else*/
			if (!Initial)
				{
				int res = await RDInterface.ShowList (RDLocale.GetText ("ColorScheme") + ":",
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
				else
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

		// Выбор цветовой схемы приложения
		private async Task<bool> SelectAppearance (bool Initial)
			{
			// Выбор варианта
			if (appearanceVariants.Count < 1)
				{
				for (uint i = 0; i < SudokuSolverMath.CellsAppearancesCount; i++)
					appearanceVariants.Add (SudokuSolverMath.GetCellsAppearanceName (i));
				}

			/*int res;
			if (Initial)
				{
				res = (int)SudokuSolverMath.CellsAppearance;
				}
			else*/
			if (!Initial)
				{
				int res = await RDInterface.ShowList (RDLocale.GetText ("CellsAppearance") + ":",
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel), appearanceVariants);
				if (res < 0)
					return false;

				// Подготовка к настройке для неначального вызова
				FlushMatrix ();
				SudokuSolverMath.CellsAppearance = (CellsAppearances)res;
				}

			// Настройка
			string line = SudokuSolverMath.SudokuField;
			for (int i = 0; i < numberButtons.Count; i++)
				{
				numberButtons[i].Text = SudokuSolverMath.GetAppearance (line[i].ToString ());
				numberButtons[i].FontSize = SudokuSolverMath.CellsAppearancesFontSize;
				numberButtons[i].FontAttributes = SudokuSolverMath.CellsAppearancesBoldFont ?
					FontAttributes.Bold : FontAttributes.None;

				if (Initial && !SudokuSolverMath.CheckCondition (numberButtons[i], ConditionTypes.IsEmpty))
					SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.OldColor);
				}
			for (int i = 1; i < inputButtons.Count; i++)
				{
				inputButtons[i].Text = SudokuSolverMath.GetAppearance ((Byte)i);
				inputButtons[i].FontSize = SudokuSolverMath.CellsAppearancesFontSize;
				}

			return true;
			}

		#endregion
		}
	}
