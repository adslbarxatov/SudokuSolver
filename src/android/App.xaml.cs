/*using Microsoft.Maui.Controls;
using System.ComponentModel;*/

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
		private Color settingsMasterBackColor = Color.FromArgb ("#FFFFF0");
		private Color settingsFieldBackColor = Color.FromArgb ("#FFFFD0");
		private Color stubColor = RDInterface.GetInterfaceColor (RDInterfaceColors.MediumGrey);

		// Контекстные меню
		private List<string> difficultyVariants = [];
		private List<string> colorSchemeVariants = [];
		private List<List<string>> menuVariants = [];
		private List<string> appearanceVariants = [];
		private List<string> highlightVariants = [];

		// Номер текущей выбранной кнопки
		private int currentButtonIndex = -1;

		// Префиксы режимов
		private const string easyPrefix = "🟢\t ";
		private const string mediumPrefix = "🟡\t ";
		private const string hardPrefix = "🔴\t ";
		private const string gemSuffix = "💎";

		#endregion

		#region Переменные страниц

		private ContentPage solutionPage, aboutPage, settingsPage;

		private Label aboutFontSizeField, solutionTipLabel;

		private Button languageButton, solutionButton, checkButton, generateButton,
			clearButton, menuButton, colorSchemeButton, cellsAppearanceButton,
			highlightButton, freeDigitsTipButton;
		private List<Button> numberButtons = [];
		private List<Button> inputButtons = [];

		private StackLayout masterField;
		private StackLayout numbersField = [];
		private StackLayout inputField = [];

		private Switch gameModeSwitch, keepScreenOnSwitch, replaceBalloonsSwitch, showFreeDigitsSwitch;

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
			solutionPage = RDInterface.ApplyPageSettings (new SolutionPage (),
				RDLocale.GetText ("SolutionPage"), stubColor);
			settingsPage = RDInterface.ApplyPageSettings (new SettingsPage (),
				RDLocale.GetText ("SettingsPage"), settingsMasterBackColor);
			aboutPage = RDInterface.ApplyPageSettings (new AboutPage (),
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout),
				aboutMasterBackColor);

			RDInterface.SetMasterPage (mainPage, solutionPage, stubColor);

			#region Основная страница

			// Ориентация экрана
			DeviceDisplay.Current.MainDisplayInfoChanged += Current_MainDisplayInfoChanged;
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
				if (RDGenerics.IsTV)
					b.Focused += FocusButton;

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
					sl.IsVisible = !RDGenerics.IsTV;
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

				b.WidthRequest = b.HeightRequest = RDInterface.MasterFontSize * 2.75;
				if (i == 0)
					b.Text = " ";

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
			if (!RDGenerics.IsTV)
				{
				Label msp = new Label ();
				msp.WidthRequest = msp.HeightRequest = 10;
				inputField.Add (msp);
				}

			StackLayout msl = [];
			msl.Orientation = RDGenerics.IsTV ? StackOrientation.Vertical : StackOrientation.Horizontal;
			msl.HorizontalOptions = LayoutOptions.Center;
			inputSL.Add (msl);
			inputField.Add (msl);

			// Добавление управляющих кнопок
			freeDigitsTipButton = RDInterface.ApplyButtonSettings (solutionPage, null, RDDefaultButtons.Menu,
				stubColor, null);
			freeDigitsTipButton.Text = "";
			freeDigitsTipButton.FontFamily = RDGenerics.MonospaceFont;
			freeDigitsTipButton.FontSize /= 1.75;
			inputSL[inputSL.Count - 1].Add (freeDigitsTipButton);

			generateButton = RDInterface.ApplyButtonSettings (solutionPage, null, RDDefaultButtons.Menu,
				stubColor, GenerateMatrix_Clicked);
			generateButton.Text = "🆕";
			inputSL[inputSL.Count - 1].Add (generateButton);

			checkButton = RDInterface.ApplyButtonSettings (solutionPage, null, RDDefaultButtons.Menu,
				stubColor, CheckSolution_Clicked);
			checkButton.Text = "☑️";

			if (RDGenerics.IsTV)
				checkButton.HeightRequest *= 3;
			else
				checkButton.WidthRequest *= 2;

			inputSL[inputSL.Count - 1].Add (checkButton);

			clearButton = RDInterface.ApplyButtonSettings (solutionPage, null, RDDefaultButtons.Menu,
				stubColor, ClearSolution_Clicked);
			clearButton.Text = "↩️";
			inputSL[inputSL.Count - 1].Add (clearButton);

			solutionButton = RDInterface.ApplyButtonSettings (solutionPage, null, RDDefaultButtons.Menu,
				stubColor, SolveSudoku_Clicked);
			solutionButton.Text = "✅";

			if (RDGenerics.IsTV)
				solutionButton.HeightRequest *= 3;
			else
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

			// Этот список нужен в нескольких местах
			difficultyVariants.Add (easyPrefix + RDLocale.GetText ("Difficulty0"));
			difficultyVariants.Add (mediumPrefix + RDLocale.GetText ("Difficulty1"));
			difficultyVariants.Add (hardPrefix + RDLocale.GetText ("Difficulty2"));

			#endregion

			#region Страница настроек

			gameModeSwitch = RDInterface.ApplySwitchSettings (settingsPage, "GameModeSwitch", false,
				settingsFieldBackColor, GameModeSwitch_Toggled, SudokuSolverMath.AppMode == AppModes.Game);
			RDInterface.ApplyLabelSettings (settingsPage, "GameModeLabel", RDLocale.GetText ("GameModeLabel"),
				RDLabelTypes.DefaultLeft);
			RDInterface.ApplyLabelSettings (settingsPage, "GameModeTip", RDLocale.GetText ("GameModeTip"),
				RDLabelTypes.TipJustify);
			GameModeSwitch_Toggled (null, null);

			keepScreenOnSwitch = RDInterface.ApplySwitchSettings (settingsPage, "KeepScreenOnSwitch", false,
				settingsFieldBackColor, KeepScreenOnSwitch_Toggled, RDInterface.KeepScreenOn);
			RDInterface.ApplyLabelSettings (settingsPage, "KeepScreenOnLabel", RDLocale.GetText ("KeepScreenOnLabel"),
				RDLabelTypes.DefaultLeft);
			RDInterface.ApplyLabelSettings (settingsPage, "KeepScreenOnTip", RDLocale.GetText ("KeepScreenOnTip"),
				RDLabelTypes.TipJustify);

			replaceBalloonsSwitch = RDInterface.ApplySwitchSettings (settingsPage, "ReplaceBalloonsSwitch", false,
				settingsFieldBackColor, ReplaceBalloonsSwitch_Toggled, SudokuSolverMath.ReplaceBalloons);
			RDInterface.ApplyLabelSettings (settingsPage, "ReplaceBalloonsLabel", RDLocale.GetText ("ReplaceBalloonsLabel"),
				RDLabelTypes.DefaultLeft);
			RDInterface.ApplyLabelSettings (settingsPage, "ReplaceBalloonsTip", RDLocale.GetText ("ReplaceBalloonsTip"),
				RDLabelTypes.TipJustify);

			showFreeDigitsSwitch = RDInterface.ApplySwitchSettings (settingsPage, "ShowFreeDigitsSwitch", false,
				settingsFieldBackColor, ShowFreeDigits_Toggled, SudokuSolverMath.ShowFreeDigitsFlag);
			RDInterface.ApplyLabelSettings (settingsPage, "ShowFreeDigitsLabel", RDLocale.GetText ("ShowFreeDigitsLabel"),
				RDLabelTypes.DefaultLeft);
			RDInterface.ApplyLabelSettings (settingsPage, "ShowFreeDigitsTip", RDLocale.GetText ("ShowFreeDigitsTip"),
				RDLabelTypes.TipJustify);

			RDInterface.ApplyLabelSettings (settingsPage, "RestartTipLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Message_RestartRequired),
				RDLabelTypes.TipCenter);

			RDInterface.ApplyLabelSettings (settingsPage, "LanguageLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_InterfaceLanguage),
				RDLabelTypes.DefaultLeft);
			languageButton = RDInterface.ApplyButtonSettings (settingsPage, "LanguageSelector",
				RDLocale.LanguagesNames[(int)RDLocale.CurrentLanguage],
				settingsFieldBackColor, SelectLanguage_Clicked, false);
			RDInterface.ApplyLabelSettings (settingsPage, "LanguageTip", RDLocale.GetText ("LanguageTip"),
				RDLabelTypes.TipJustify);

			RDInterface.ApplyLabelSettings (settingsPage, "FontSizeLabel",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_InterfaceFontSize),
				RDLabelTypes.DefaultLeft);
			RDInterface.ApplyButtonSettings (settingsPage, "FontSizeInc",
				RDDefaultButtons.Increase, settingsFieldBackColor, FontSizeButton_Clicked);
			RDInterface.ApplyButtonSettings (settingsPage, "FontSizeDec",
				RDDefaultButtons.Decrease, settingsFieldBackColor, FontSizeButton_Clicked);
			aboutFontSizeField = RDInterface.ApplyLabelSettings (settingsPage, "FontSizeField",
				" ", RDLabelTypes.DefaultCenter);
			RDInterface.ApplyLabelSettings (settingsPage, "FontSizeTip", RDLocale.GetText ("FontSizeTip"),
				RDLabelTypes.TipJustify);

			highlightButton = RDInterface.ApplyButtonSettings (settingsPage, "HighlightAffectedButton", " ",
				settingsFieldBackColor, HighlightAffectedButton_Clicked, false);
			RDInterface.ApplyLabelSettings (settingsPage, "HighlightAffectedLabel",
				RDLocale.GetText ("HighlightAffectedLabel"), RDLabelTypes.DefaultLeft);
			RDInterface.ApplyLabelSettings (settingsPage, "HighlightAffectedTip",
				RDLocale.GetText ("HighlightAffectedTip"), RDLabelTypes.TipJustify);
			HighlightAffectedButton_Clicked (null, null);

			colorSchemeButton = RDInterface.ApplyButtonSettings (settingsPage, "ColorSchemeButton", " ",
				settingsFieldBackColor, ColorSchemeButton_Clicked, false);
			RDInterface.ApplyLabelSettings (settingsPage, "ColorSchemeLabel", RDLocale.GetText ("ColorSchemeLabel"),
				RDLabelTypes.DefaultLeft);
			RDInterface.ApplyLabelSettings (settingsPage, "ColorSchemeTip", RDLocale.GetText ("ColorSchemeTip"),
				RDLabelTypes.TipJustify);
			ColorSchemeButton_Clicked (null, null);

			cellsAppearanceButton = RDInterface.ApplyButtonSettings (settingsPage, "CellsAppearanceButton", " ",
				settingsFieldBackColor, CellsAppearanceButton_Clicked, false);
			RDInterface.ApplyLabelSettings (settingsPage, "CellsAppearanceLabel", RDLocale.GetText ("CellsAppearanceLabel"),
				RDLabelTypes.DefaultLeft);
			RDInterface.ApplyLabelSettings (settingsPage, "CellsAppearanceTip", RDLocale.GetText ("CellsAppearanceTip"),
				RDLabelTypes.TipJustify);
			CellsAppearanceButton_Clicked (null, null);

			#endregion

			#region Страница "О программе"

			RDInterface.ApplyLabelSettings (aboutPage, "AboutLabel",
				RDGenerics.AppAboutLabelText, RDLabelTypes.AppAbout);

			RDInterface.ApplyButtonSettings (aboutPage, "ManualsButton",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_ReferenceMaterials),
				aboutFieldBackColor, ReferenceButton_Click, false);

			Button hlp = RDInterface.ApplyButtonSettings (aboutPage, "HelpButton",
				RDLocale.GetDefaultText (RDLDefaultTexts.Control_HelpSupport),
				aboutFieldBackColor, HelpButton_Click, false);
			hlp.IsVisible = !RDGenerics.IsTV;

			Image qrImage = (Image)aboutPage.FindByName ("QRImage");
			qrImage.IsVisible = RDGenerics.IsTV;

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

		// Изменение ориентации экрана
		private async void Current_MainDisplayInfoChanged (object sender, DisplayInfoChangedEventArgs e)
			{
			await Task.Delay (500);

			if (RDGenerics.IsTV)
				{
				masterField.Orientation = StackOrientation.Horizontal;
				}
			else
				{
				bool portrait = Windows[0].Width < Windows[0].Height;
				masterField.Orientation = (portrait ? StackOrientation.Vertical : StackOrientation.Horizontal);
				}
			}

		protected override void OnStart ()
			{
			Current_MainDisplayInfoChanged (null, null);
			base.OnStart ();
			}

		protected override void OnResume ()
			{
			Current_MainDisplayInfoChanged (null, null);
			base.OnResume ();
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
			if (RDGenerics.IsTV)
				{
				await RDInterface.ShowMessage (RDLocale.GetText ("HelpQRTip"),
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));
				return;
				}

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
				menuVariants[0].Add ("ℹ️\t " + RDLocale.GetText ("Menu3"));
				menuVariants[0].Add ("⚙️\t " + RDLocale.GetText ("Menu2"));

				menuVariants.Add ([]);
				menuVariants[1].Add ("✅\t " + RDLocale.GetText ("SolveButton"));
				menuVariants[1].Add ("❌\t " + RDLocale.GetText ("ResetField"));
				menuVariants[1].Add ("📄\t " + RDLocale.GetText ("LoadFromFile"));
				menuVariants[1].Add ("💾\t " + RDLocale.GetText ("SaveToFile"));

				menuVariants.Add ([]);
				menuVariants[2].Add ("🆕\t " + RDLocale.GetText ("GenerateMatrix"));
				menuVariants[2].Add ("☑️\t " + RDLocale.GetText ("CheckSolutionButton"));
				menuVariants[2].Add ("↩️\t " + RDLocale.GetText ("ClearSolution"));

				menuVariants.Add ([]);
				menuVariants[3].Add ("🎛\t " + RDLocale.GetText ("StateButton"));
				menuVariants[3].Add ("📊\t " + RDLocale.GetText ("StatsButton"));
				menuVariants[3].Add ("🔀\t " + RDLocale.GetText ("ExchangeButton"));
				menuVariants[3].Add ("ℹ️\t " + RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout));
				}
			List<List<int>> indirectMenu = [
				[0, 1, 2],
				[1, 2, 3],
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
				// Настройки
				case 3:
					RDInterface.SetCurrentPage (settingsPage, settingsMasterBackColor);
					break;

				// Выполнить решение
				case 10:
					if (!await FindSolution (true))
						await ShowRBControlledMessage ("❌ " + RDLocale.GetText ("SolutionIsIncorrect"));
					break;

				// Полный сброс
				case 11:
					if (!await RDInterface.ShowMessage (RDLocale.GetText ("ResetWarning"),
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_Yes),
						RDLocale.GetDefaultText (RDLDefaultTexts.Button_No)))
						return;

					for (int i = 0; i < numberButtons.Count; i++)
						SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.EmptyValue);
					SudokuSolverMath.GameMode = MatrixDifficulty.None;
					break;

				// Загрузка из файла
				case 12:
					await LoadFromFile ();
					break;

				// Сохранение в файл
				case 13:
					await SaveToFile ();
					break;

				// Генерация матрицы
				case 20:
					await GenerateMatrix ();
					break;

				// Проверить корректность решения
				case 21:
					if (!await FindSolution (false))
						await ApplyPenalty ();
					break;

				// Сброс решения
				case 22:
					ClearSolution_Clicked (null, null);
					break;

				// Состояние программы
				case 30:
					await ShowState ();
					break;

				// Статистика игры
				case 31:
					await ShowScore (false);
					break;

				// Перенос выигрышей
				case 32:
					await ExchangeScores ();
					break;

				// О приложении
				case 33:
					RDInterface.SetCurrentPage (aboutPage, aboutMasterBackColor);
					break;
				}
			}

		// Метод применяет штраф
		private static async Task<bool> ApplyPenalty ()
			{
			uint score = SudokuSolverMath.GetScore (ScoreTypes.Penalty);
			SudokuSolverMath.UpdateGameScore (true, score);

			string text = "❌ " + RDLocale.GetText ("SolutionIsIncorrect");
			if (SudokuSolverMath.GameMode != MatrixDifficulty.None)
				text += (RDLocale.RNRN + "–" + score.ToString () + " " + gemSuffix);

			return await ShowRBControlledMessage (text);
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
			bool condition = SudokuSolverMath.CheckCondition (numberButtons[currentButtonIndex], ConditionTypes.SelectedCell);
			if (condition || RDGenerics.IsTV)
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
			if (!condition || RDGenerics.IsTV)
				PaintButtons ();
			}

		// Переход между кнопками на Android TV
		private void FocusButton (object sender, EventArgs e)
			{
			currentButtonIndex = numberButtons.IndexOf ((Button)sender);
			PaintButtons ();
			}

		// Метод отвечает за обновление цветов кнопок основного поля
		private void PaintButtons ()
			{
			bool showAffected = (SudokuSolverMath.HighlightType != HighlightTypes.None);

			// Обновление цветов
			if (showAffected)
				{
				bool squaresToo = (SudokuSolverMath.HighlightType == HighlightTypes.LinesAndSquares);
				for (int i = 0; i < numberButtons.Count; i++)
					{
					if (i == currentButtonIndex)
						SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.SelectedCell);
					else if (showAffected && SudokuSolverMath.IsCellAffected ((uint)currentButtonIndex,
						(uint)i, squaresToo))
						SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.AffectedCell);
					else
						SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.DeselectedCell);
					}
				}

			// Обновление подсказки
			if (SudokuSolverMath.ShowFreeDigitsFlag)
				{
				if (!SudokuSolverMath.CheckCondition (numberButtons[currentButtonIndex], ConditionTypes.IsEmpty))
					{
					freeDigitsTipButton.Text = "";
					return;
					}

				string existing = "";
				for (int i = 0; i < numberButtons.Count; i++)
					{
					if (SudokuSolverMath.IsCellAffected ((uint)currentButtonIndex, (uint)i, true))
						if (!existing.Contains (numberButtons[i].Text))
							existing += numberButtons[i].Text;
					}

				freeDigitsTipButton.Text = SudokuSolverMath.GetFreeDigitsForCell (existing);
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

		// Метод выполняет решение судоку или его проверку
		private async void SolveSudoku_Clicked (object sender, EventArgs e)
			{
			if (!await FindSolution (true))
				await ShowRBControlledMessage ("❌ " + RDLocale.GetText ("SolutionIsIncorrect"));

			if (RDGenerics.IsTV && (currentButtonIndex >= 0) && (numberButtons.Count > currentButtonIndex))
				numberButtons[currentButtonIndex].Focus ();
			}

		private async void CheckSolution_Clicked (object sender, EventArgs e)
			{
			if (!await FindSolution (false))
				await ApplyPenalty ();

			if (RDGenerics.IsTV && (currentButtonIndex >= 0) && (numberButtons.Count > currentButtonIndex))
				numberButtons[currentButtonIndex].Focus ();
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
					bool win = (emptyCellsCount < 2);
					if (win)
						score += SudokuSolverMath.GetScore (ScoreTypes.GameCompletion);

					// Отображение сведений о достижениях (обязательно до обновления очков)
					string achiLine = "";
					for (uint i = 0; i < SudokuSolverMath.AchievementsCount; i++)
						{
						if (!win)
							break;

						if (!SudokuSolverMath.CheckAchievement (i))
							continue;

						string achiText = RDLocale.GetText ("Achi" + i.ToString ());
						int left = achiText.IndexOf (RDLocale.RN);
						achiLine += " " + achiText.Substring (0, left);

						uint tip = 1u << (8 + (int)i);
						if ((RDGenerics.TipsState & tip) == 0)
							{
							await RDInterface.ShowMessage (achiText, RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));
							RDGenerics.TipsState |= tip;
							}
						}

					// Обновление счёта
					SudokuSolverMath.UpdateGameScore (false, score);

					// Отображение результата и отключение игрового режима до следующей генерации
					string msgText = "✅ " + RDLocale.GetText ("SolutionIsCorrect") + RDLocale.RNRN +
						"+" + score.ToString () + " " + gemSuffix;
					if (!string.IsNullOrWhiteSpace (achiLine))
						msgText += "\t\t+" + achiLine;
					await ShowRBControlledMessage (msgText);

					// Отобразить решение в случае выигрыша (без return; режим игры отключается далее)
					if (win)
						await ShowScore (true);

					// Иначе продолжить игру
					else
						return true;
					}

				// Не отображать решение вне игрового режима
				else
					{
					await ShowRBControlledMessage ("✅ " + RDLocale.GetText ("SolutionIsCorrect"));
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

			string[] stats = SudokuSolverMath.StatsValues;
			text += (string.Format (RDLocale.GetText ("StatsText"), gemSuffix + "\t " + stats[0],
				easyPrefix + stats[1] + "\t\t" + mediumPrefix + stats[2] + "\t\t" + hardPrefix + stats[3],
				easyPrefix + stats[4] + "\t\t" + mediumPrefix + stats[5] + "\t\t" + hardPrefix + stats[6],
				easyPrefix + stats[7] + "\t\t" + mediumPrefix + stats[8] + "\t\t" + hardPrefix + stats[9])) +
				RDLocale.RNRN;

			text += string.Format (RDLocale.GetText ("StatsTextAchi"),
				stats[10], stats[11]);

			// Отображение
			bool ans;
			if (RDGenerics.IsTV)
				{
				await RDInterface.ShowMessage (text, RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));
				ans = true;
				}
			else
				{
				ans = await RDInterface.ShowMessage (text, RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK),
					RDLocale.GetText ("ShareButton"));
				}

			// Отправка
			if (ans)
				return true;

			await Share.RequestAsync (ProgramDescription.AssemblyVisibleName + RDLocale.RNRN +
				text, ProgramDescription.AssemblyVisibleName);
			return true;
			}

		// Статус программы
		private async Task<bool> ShowState ()
			{
			bool gameMode = (SudokuSolverMath.GameMode != MatrixDifficulty.None);
			string yesNo = RDLocale.GetDefaultText (gameMode ? RDLDefaultTexts.Button_Yes : RDLDefaultTexts.Button_No).ToLower ();
			string diff = gameMode ? difficultyVariants[(int)SudokuSolverMath.GameMode].ToLower () : "—";

			string msg = string.Format (RDLocale.GetText ("AppModeMessage"), diff, yesNo);
			await RDInterface.ShowMessage (msg, RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));
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
				generateButton.IsVisible = clearButton.IsVisible = checkButton.IsVisible = freeDigitsTipButton.IsVisible = false;
				solutionButton.IsVisible = true;
				solutionButton.Text = "❌";
				}
			else
				{
				solutionButton.Text = "✅";
				GameModeSwitch_Toggled (null, null);
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
			// Контроль
			if (SudokuSolverMath.GameMode != MatrixDifficulty.None)
				{
				if (!await RDInterface.ShowMessage (RDLocale.GetText ("GameIsNotCompletedMessage"),
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Yes),
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_No)))
					return false;
				}

			// Выбор сложности
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
		private void GameModeSwitch_Toggled (object sender, ToggledEventArgs e)
			{
			if (sender != null)
				SudokuSolverMath.AppMode = (gameModeSwitch.IsToggled ? AppModes.Game : AppModes.SolutionOnly);

			// Настройка
			bool game = (SudokuSolverMath.AppMode == AppModes.Game);

			generateButton.IsVisible = checkButton.IsVisible = freeDigitsTipButton.IsVisible = game;
			solutionButton.IsVisible = !game;

			if (!game || !SudokuSolverMath.ShowFreeDigitsFlag)
				freeDigitsTipButton.Text = "";

			clearButton.IsVisible = true;
			if (!game)
				SudokuSolverMath.GameMode = MatrixDifficulty.None;
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
		private async void ColorSchemeButton_Clicked (object sender, EventArgs e)
			{
			// Выбор варианта
			if (colorSchemeVariants.Count < 1)
				{
				string[] names = SudokuSolverMath.ColorSchemesNames;
				for (int i = 0; i < names.Length; i++)
					colorSchemeVariants.Add (names[i]);
				}

			int res;
			if (sender != null)
				{
				res = await RDInterface.ShowList (RDLocale.GetText ("ColorSchemeLabel") + ":",
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel), colorSchemeVariants);
				if (res < 0)
					return;
				SudokuSolverMath.ColorScheme = (ColorSchemes)res;
				}
			else
				{
				res = (int)SudokuSolverMath.ColorScheme;
				}

			// Настройка
			colorSchemeButton.Text = colorSchemeVariants[res];

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
			SudokuSolverMath.SetProperty (freeDigitsTipButton, PropertyTypes.OtherButton);
			SudokuSolverMath.SetProperty (freeDigitsTipButton, PropertyTypes.OldColor);
			}

		// Выбор цветовой схемы приложения
		private async void CellsAppearanceButton_Clicked (object sender, EventArgs e)
			{
			// Выбор варианта
			if (appearanceVariants.Count < 1)
				{
				string[] names = SudokuSolverMath.CellsAppearancesNames;
				for (int i = 0; i < names.Length; i++)
					appearanceVariants.Add (names[i]);
				}

			int res;
			if (sender != null)
				{
				res = await RDInterface.ShowList (RDLocale.GetText ("CellsAppearanceLabel") + ":",
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel), appearanceVariants);
				if (res < 0)
					return;

				// Подготовка к настройке для неначального вызова
				FlushMatrix ();
				SudokuSolverMath.CellsAppearance = (CellsAppearances)res;
				}
			else
				{
				res = (int)SudokuSolverMath.CellsAppearance;
				}

			// Настройка
			cellsAppearanceButton.Text = appearanceVariants[res];

			string line = SudokuSolverMath.SudokuField;
			for (int i = 0; i < numberButtons.Count; i++)
				{
				numberButtons[i].Text = SudokuSolverMath.GetAppearance (line[i].ToString ());
				numberButtons[i].FontSize = SudokuSolverMath.CellsAppearancesFontSize;
				numberButtons[i].FontAttributes = SudokuSolverMath.CellsAppearancesBoldFont ?
					FontAttributes.Bold : FontAttributes.None;

				if ((sender == null) && !SudokuSolverMath.CheckCondition (numberButtons[i], ConditionTypes.IsEmpty))
					SudokuSolverMath.SetProperty (numberButtons[i], PropertyTypes.OldColor);
				}
			for (int i = 1; i < inputButtons.Count; i++)
				{
				inputButtons[i].Text = SudokuSolverMath.GetAppearance ((Byte)i);
				inputButtons[i].FontSize = SudokuSolverMath.CellsAppearancesFontSize;
				}
			}

		// Включение / выключение фиксации экрана
		private void KeepScreenOnSwitch_Toggled (object sender, ToggledEventArgs e)
			{
			RDInterface.KeepScreenOn = keepScreenOnSwitch.IsToggled;
			}

		// Включение / выключение замены всплывающих сообщений
		private void ReplaceBalloonsSwitch_Toggled (object sender, ToggledEventArgs e)
			{
			SudokuSolverMath.ReplaceBalloons = replaceBalloonsSwitch.IsToggled;
			}

		// Включение / выключение отображения цифр, доступных для выбранной ячейки
		private void ShowFreeDigits_Toggled (object sender, ToggledEventArgs e)
			{
			SudokuSolverMath.ShowFreeDigitsFlag = showFreeDigitsSwitch.IsToggled;
			}

		private static async Task<bool> ShowRBControlledMessage (string Text)
			{
			if (SudokuSolverMath.ReplaceBalloons)
				await RDInterface.ShowMessage (Text, RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK));
			else
				RDInterface.ShowBalloon (Text, true);

			return true;
			}

		// Включение / выключение подсветки простреливаемых ячеек
		private async void HighlightAffectedButton_Clicked (object sender, EventArgs e)
			{
			// Выбор варианта
			if (highlightVariants.Count < 1)
				{
				for (int i = 0; i < 3; i++)
					highlightVariants.Add (RDLocale.GetText ("Highlight" + i.ToString ()));
				}

			int res;
			if (sender != null)
				{
				res = await RDInterface.ShowList (RDLocale.GetText ("HighlightAffectedLabel") + ":",
					RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel), highlightVariants);
				if (res < 0)
					return;
				SudokuSolverMath.HighlightType = (HighlightTypes)res;
				}
			else
				{
				res = (int)SudokuSolverMath.HighlightType;
				}

			// Настройка и выполнение
			highlightButton.Text = highlightVariants[res];

			// При запуске приложения этот вызов выполняется далее по сценарию загрузки страницы,
			// поэтому не требует повторения
			if (sender != null)
				ColorSchemeButton_Clicked (null, null);
			}

		// Перенос выигрышей
		private static async Task<bool> ExchangeScores ()
			{
			int res = await RDInterface.ShowList (RDLocale.GetText ("ScoresExchangeMessage"),
				RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel),
				[RDLocale.GetText ("ScoresExchangeCopy"), RDLocale.GetText ("ScoresExchangeLoad")]);

			switch (res)
				{
				case 0:
					RDGenerics.SendToClipboard (SudokuSolverMath.GetPortableScoresLine (), true);
					break;

				case 1:
					if (SudokuSolverMath.SetPortableScoresLine (await RDGenerics.GetFromClipboard ()))
						await ShowScore (false);
					else
						RDInterface.ShowBalloon (RDLocale.GetText ("ScoresExchangeError"), true);
					break;
				}

			return true;
			}

		#endregion
		}
	}
