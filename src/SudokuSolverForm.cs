using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает главную форму приложения
	/// </summary>
	public partial class SudokuSolverForm: Form
		{
		// Переменные и константы
		private const int buttonSize = 30;
		private List<Button> buttons = [];
		private Button newGameButton, checkButton, clearButton;

		private ContextMenuStrip appearanceMenu;
		private ContextMenuStrip colorSchemeMenu;
		private ContextMenuStrip gameModeMenu;
		private ContextMenuStrip highlightingMenu;
		private Point menuPoint;

		/// <summary>
		/// Конструктор. Настраивает главную форму приложения
		/// </summary>
		public SudokuSolverForm ()
			{
			// Инициализация
			InitializeComponent ();
			RDGenerics.LoadWindowDimensions (this);

			this.Text = ProgramDescription.AssemblyTitle;

			appearanceMenu = new ContextMenuStrip ();
			appearanceMenu.ShowImageMargin = false;
			colorSchemeMenu = new ContextMenuStrip ();
			colorSchemeMenu.ShowImageMargin = false;
			gameModeMenu = new ContextMenuStrip ();
			gameModeMenu.ShowImageMargin = false;
			highlightingMenu = new ContextMenuStrip ();
			highlightingMenu.ShowImageMargin = false;

			// Формирование поля
			for (int r = 0; r < SudokuSolverMath.SideSize; r++)
				{
				for (int c = 0; c < SudokuSolverMath.SideSize; c++)
					{
					Button lb = new Button ();

					SudokuSolverMath.SetProperty (lb, PropertyTypes.EmptyValue);
					SudokuSolverMath.SetProperty (lb, PropertyTypes.OldColor);
					lb.TextAlign = ContentAlignment.MiddleCenter;
					lb.Width = lb.Height = buttonSize;
					lb.Left = lb.Width * (c + 1) + 3 * (c / SudokuSolverMath.SquareSize -
						SudokuSolverMath.SquareSize / 2);
					lb.Top = lb.Height * (r + 1) + 3 * (r / SudokuSolverMath.SquareSize -
						SudokuSolverMath.SquareSize / 2) + buttonSize;
					lb.Cursor = Cursors.UpArrow;
					lb.KeyDown += Lb_KeyDown;
					lb.FlatStyle = FlatStyle.Popup;
					lb.MouseWheel += Lb_MouseWheel;
					lb.MouseDown += Lb_MouseClick;
					lb.MouseHover += Lb_MouseHover;

					this.Controls.Add (lb);
					buttons.Add (lb);
					}
				}

			menuPoint = new Point (buttons[3].Left, buttons[3].Top - buttons[3].Height);

			// Формирование вспомогательных кнопок
			newGameButton = new Button ();
			checkButton = new Button ();
			clearButton = new Button ();

			LocalizeForm ();

			newGameButton.TextAlign = checkButton.TextAlign = clearButton.TextAlign =
				ContentAlignment.MiddleCenter;
			newGameButton.Width = checkButton.Width = clearButton.Width = 3 * buttonSize;
			newGameButton.Height = checkButton.Height = clearButton.Height = buttonSize;

			newGameButton.Left = buttonSize - 3;
			checkButton.Left = 4 * buttonSize;
			clearButton.Left = 7 * buttonSize + 3;

			newGameButton.Top = checkButton.Top = clearButton.Top =
				buttons[buttons.Count - 1].Top + 2 * buttonSize;
			newGameButton.FlatStyle = checkButton.FlatStyle = clearButton.FlatStyle = FlatStyle.Flat;
			newGameButton.Font = checkButton.Font = clearButton.Font = MainMenu.Font;

			newGameButton.Click += NewGame_Click;
			checkButton.Click += MCheck_Click;
			clearButton.Click += MClear_Click;

			this.Controls.Add (newGameButton);
			this.Controls.Add (checkButton);
			this.Controls.Add (clearButton);

			// Загрузка настроек
			ChangeAppearance (null, null);  // Загружает сохранённую матрицу
			ChangeColorScheme (null, null);
			ChangeAppMode (null, null);
			}

		// Подсветка простреливаемых ячеек
		private void Lb_MouseHover (object sender, EventArgs e)
			{
			if (!SudokuSolverMath.ShowAffectedCells)
				return;

			uint idx = (uint)buttons.IndexOf ((Button)sender);
			for (int i = 0; i < buttons.Count; i++)
				{
				if (i == idx)
					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.SelectedCell);
				else if (SudokuSolverMath.IsCellAffected (idx, (uint)i))
					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.AffectedCell);
				else
					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.DeselectedCell);
				}
			}

		// Метод локализует форму
		private void LocalizeForm ()
			{
			// Меню
			MActivities.Text = RDLocale.GetText ("MainMenu_MActivities");
			MSettings.Text = RDLocale.GetText ("MainMenu_MSettings");
			MInfo.Text = RDLocale.GetText ("MainMenu_MInfo");

			for (int i = 0; i < MActivities.DropDownItems.Count; i++)
				MActivities.DropDownItems[i].Text = RDLocale.GetText (MActivities.Name + "_" +
					MActivities.DropDownItems[i].Name);
			for (int i = 0; i < MGenerate.DropDownItems.Count; i++)
				MGenerate.DropDownItems[i].Text = RDLocale.GetText (MGenerate.Name + "_" +
					MGenerate.DropDownItems[i].Name);
			for (int i = 0; i < MSettings.DropDownItems.Count; i++)
				MSettings.DropDownItems[i].Text = RDLocale.GetText (MSettings.Name + "_" +
					MSettings.DropDownItems[i].Name);
			for (int i = 0; i < MInfo.DropDownItems.Count; i++)
				MInfo.DropDownItems[i].Text = RDLocale.GetText (MInfo.Name + "_" +
					MInfo.DropDownItems[i].Name);

			MExit.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Exit);

			// Диалоги
			OFDialog.Title = RDLocale.GetText ("OFName");
			SFDialog.Title = RDLocale.GetText ("SFName");
			OFDialog.Filter = SFDialog.Filter = RDLocale.GetText ("OFFilter");

			// Контекстные меню
			appearanceMenu.Items.Clear ();
			/*for (uint i = 0; i < SudokuSolverMath.CellsAppearancesCount; i++)
				appearanceMenu.Items.Add (SudokuSolverMath.GetCellsAppearanceName (i), null,
					ChangeAppearance);*/
			string[] appearances = SudokuSolverMath.CellsAppearancesNames;
			for (int i = 0; i < appearances.Length; i++)
				appearanceMenu.Items.Add (appearances[i], null, ChangeAppearance);

			colorSchemeMenu.Items.Clear ();
			/*for (uint i = 0; i < SudokuSolverMath.ColorSchemesCount; i++)
				colorSchemeMenu.Items.Add (RDLocale.GetText ("Color" + i.ToString ()), null,
					ChangeColorScheme);*/
			string[] colorSchemes = SudokuSolverMath.ColorSchemesNames;
			for (int i = 0; i < colorSchemes.Length; i++)
				colorSchemeMenu.Items.Add (colorSchemes[i], null, ChangeColorScheme);

			gameModeMenu.Items.Clear ();
			gameModeMenu.Items.Add (RDLocale.GetDefaultText (RDLDefaultTexts.Button_Yes), null, ChangeAppMode);
			gameModeMenu.Items.Add (RDLocale.GetDefaultText (RDLDefaultTexts.Button_No), null, ChangeAppMode);

			highlightingMenu.Items.Clear ();
			highlightingMenu.Items.Add (RDLocale.GetDefaultText (RDLDefaultTexts.Button_Yes), null, ChangeHighlighting);
			highlightingMenu.Items.Add (RDLocale.GetDefaultText (RDLDefaultTexts.Button_No), null, ChangeHighlighting);

			// Вспомогательные кнопки
			newGameButton.Text = RDLocale.GetText ("NewGameButton");
			checkButton.Text = RDLocale.GetText ("CheckButton");
			clearButton.Text = RDLocale.GetText ("ClearButton");
			}

		/// <summary>
		/// Метод переопределяет обработку клавиатуры формой
		/// </summary>
		protected override bool ProcessCmdKey (ref Message msg, Keys keyData)
			{
			switch (keyData)
				{
				// Перенаправление движения по кнопкам
				case Keys.Up:
					for (int i = 0; i < SudokuSolverMath.SideSize; i++)
						this.SelectNextControl (this.ActiveControl, false, true, false, true);
					return true;

				case Keys.Down:
					for (int i = 0; i < SudokuSolverMath.SideSize; i++)
						this.SelectNextControl (this.ActiveControl, true, true, false, true);
					return true;

				case Keys.Left:
					if (buttons.IndexOf ((Button)this.ActiveControl) % SudokuSolverMath.SideSize == 0)
						{
						for (int i = 1; i < SudokuSolverMath.SideSize; i++)
							this.SelectNextControl (this.ActiveControl, true, true, false, true);
						}
					else
						{
						this.SelectNextControl (this.ActiveControl, false, true, false, true);
						}
					return true;

				case Keys.Right:
					if ((buttons.IndexOf ((Button)this.ActiveControl) + 1) % SudokuSolverMath.SideSize == 0)
						{
						for (int i = 1; i < SudokuSolverMath.SideSize; i++)
							this.SelectNextControl (this.ActiveControl, false, true, false, true);
						}
					else
						{
						this.SelectNextControl (this.ActiveControl, true, true, false, true);
						}
					return true;

				// Остальные клавиши обрабатываются стандартной процедурой
				default:
					return base.ProcessCmdKey (ref msg, keyData);
				}
			}

		// Действия из меню программы

		// Решение из текущего состояния
		private void MSolve_Click (object sender, EventArgs e)
			{
			if (!Solve (true))
				RDInterface.LocalizedMessageBox (RDMessageTypes.Error_Center, "SolutionIsIncorrect", 1000);
			}

		// Проверка решения в текущем состоянии
		private void MCheck_Click (object sender, EventArgs e)
			{
			if (!Solve (false))
				ApplyPenalty ();
			}

		// Сброс решения (без очистки всех полей)
		private void MClear_Click (object sender, EventArgs e)
			{
			bool game = (SudokuSolverMath.GameMode != MatrixDifficulty.None);

			for (int i = 0; i < buttons.Count; i++)
				{
				if (SudokuSolverMath.CheckCondition (buttons[i], ConditionTypes.ContainsFoundValue) ||
					game && SudokuSolverMath.CheckCondition (buttons[i], ConditionTypes.ContainsNewValue))
					{
					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.EmptyValue);
					}
				}
			}

		// Полный сброс
		private void MReset_Click (object sender, EventArgs e)
			{
			if (RDInterface.LocalizedMessageBox (RDMessageTypes.Warning_Center, "ResetWarning",
				RDLDefaultTexts.Button_YesNoFocus, RDLDefaultTexts.Button_No) !=
				RDMessageButtons.ButtonOne)
				return;

			for (int i = 0; i < buttons.Count; i++)
				SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.EmptyValue);
			SudokuSolverMath.GameMode = MatrixDifficulty.None;
			}

		// Справка
		private void MHelp_Click (object sender, EventArgs e)
			{
			RDInterface.ShowAbout (false);
			}

		// Язык интерфейса
		private void MLanguage_Click (object sender, EventArgs e)
			{
			if (RDInterface.MessageBox ())
				LocalizeForm ();
			}

		// Закрытие окна
		private void MExit_Click (object sender, EventArgs e)
			{
			this.Close ();
			}

		// Загрузка таблицы из файла
		private void MLoad_Click (object sender, EventArgs e)
			{
			OFDialog.ShowDialog ();
			}

		private void OFDialog_FileOk (object sender, CancelEventArgs e)
			{
			// Попытка считывания файла
			string file;
			try
				{
				file = File.ReadAllText (OFDialog.FileName, RDGenerics.GetEncoding (RDEncodings.UTF8));
				}
			catch
				{
				RDInterface.LocalizedMessageBox (RDMessageTypes.Warning_Center,
					string.Format (RDLocale.GetDefaultText (RDLDefaultTexts.Message_LoadFailure_Fmt),
					OFDialog.FileName));
				return;
				}

			// Обработка
			string line = SudokuSolverMath.ParseMatrixFromFile (file);
			if (string.IsNullOrWhiteSpace (line))
				{
				RDInterface.LocalizedMessageBox (RDMessageTypes.Warning_Center, "MessageNotEnough");
				return;
				}

			// Загрузка
			for (int i = 0; i < buttons.Count; i++)
				{
				buttons[i].Text = SudokuSolverMath.GetAppearance (line[i].ToString ());
				if (SudokuSolverMath.CheckCondition (buttons[i], ConditionTypes.IsEmpty))
					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.NewColor);
				else
					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.OldColor);
				}

			// Сброс игрового режима
			SudokuSolverMath.GameMode = MatrixDifficulty.None;
			}

		// Выгрузка таблицы в файл
		private void MSave_Click (object sender, EventArgs e)
			{
			SFDialog.ShowDialog ();
			}

		private void SFDialog_FileOk (object sender, CancelEventArgs e)
			{
			// Выгрузка данных
			FlushMatrix ();
			string file = SudokuSolverMath.BuildMatrixToSave (SudokuSolverMath.SudokuField);

			// Сохранение
			try
				{
				File.WriteAllText (SFDialog.FileName, file, RDGenerics.GetEncoding (RDEncodings.UTF8));
				}
			catch
				{
				RDInterface.MessageBox (RDMessageTypes.Warning_Center,
					string.Format (RDLocale.GetDefaultText (RDLDefaultTexts.Message_SaveFailure_Fmt),
					SFDialog.FileName));
				return;
				}
			}

		// Выбор поля ввода
		private void Lb_KeyDown (object sender, KeyEventArgs e)
			{
			Button lb = (Button)sender;

			// В игровом режиме изменение проверенных ячеек запрещено
			if ((SudokuSolverMath.GameMode != MatrixDifficulty.None) &&
				!SudokuSolverMath.CheckCondition (lb, ConditionTypes.IsEmpty) &&
				!SudokuSolverMath.CheckCondition (lb, ConditionTypes.ContainsNewValue))
				return;

			switch (e.KeyCode)
				{
				case Keys.D1:
				case Keys.D2:
				case Keys.D3:
				case Keys.D4:
				case Keys.D5:
				case Keys.D6:
				case Keys.D7:
				case Keys.D8:
				case Keys.D9:
					SudokuSolverMath.SetProperty (lb, PropertyTypes.NewColor);
					lb.Text = SudokuSolverMath.GetAppearance ((Byte)(e.KeyCode - 48));
					break;

				default:
					SudokuSolverMath.SetProperty (lb, PropertyTypes.EmptyValue);
					break;
				}
			}

		// Нажание кнопок и прокрутка
		private void Lb_MouseWheel (object sender, MouseEventArgs e)
			{
			Lb_MouseClick (sender, e);
			}

		private void Lb_MouseClick (object sender, MouseEventArgs e)
			{
			Button b = (Button)sender;

			// В игровом режиме изменение проверенных ячеек запрещено
			if ((SudokuSolverMath.GameMode != MatrixDifficulty.None) &&
				!SudokuSolverMath.CheckCondition (b, ConditionTypes.IsEmpty) &&
				!SudokuSolverMath.CheckCondition (b, ConditionTypes.ContainsNewValue))
				return;

			int v = -1;
			bool plus;
			if ((e.Delta < 0) || (e.Button == MouseButtons.Right))
				plus = false;
			else
				plus = true;

			try
				{
				v = SudokuSolverMath.GetDigit (b.Text);
				v += (plus ? 1 : -1);
				}
			catch { }

			if (plus)
				{
				if (v < 0)
					b.Text = SudokuSolverMath.GetAppearance (1);
				else if (v > 9)
					SudokuSolverMath.SetProperty (b, PropertyTypes.EmptyValue);
				else
					b.Text = SudokuSolverMath.GetAppearance ((Byte)v);
				}
			else
				{
				if (v < 0)
					b.Text = SudokuSolverMath.GetAppearance (9);
				else if (v < 1)
					SudokuSolverMath.SetProperty (b, PropertyTypes.EmptyValue);
				else
					b.Text = SudokuSolverMath.GetAppearance ((Byte)v);
				}

			SudokuSolverMath.SetProperty (b, PropertyTypes.NewColor);
			}

		// Решение задачи
		private bool Solve (bool LoadResults)
			{
			// Сборка массива
			Byte[,] matrix = new Byte[SudokuSolverMath.SideSize, SudokuSolverMath.SideSize];
			for (int r = 0; r < SudokuSolverMath.SideSize; r++)
				{
				for (int c = 0; c < SudokuSolverMath.SideSize; c++)
					{
					Button ct = buttons[r * (int)SudokuSolverMath.SideSize + c];
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
						for (int i = 0; i < buttons.Count; i++)
							SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.ErrorColor);

					return false;
				}

			// Решение задачи
			SudokuSolverMath.FindSolution ();
			switch (SudokuSolverMath.CurrentStatus)
				{
				case SolutionResults.NoSolutionsFound:
				case SolutionResults.NotInited:
					if (LoadResults)
						for (int i = 0; i < buttons.Count; i++)
							SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.ErrorColor);

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

				for (int i = 0; i < buttons.Count; i++)
					{
					if (gameMode)
						{
						if (SudokuSolverMath.CheckCondition (buttons[i], ConditionTypes.IsEmpty))
							emptyCellsCount++;
						else if (SudokuSolverMath.CheckCondition (buttons[i], ConditionTypes.ContainsNewValue))
							newCellsCount++;
						}

					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.OldColor);
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
							RDInterface.MessageBox (RDMessageTypes.Success_Center, achiText);
							RDGenerics.TipsState |= tip;
							}
						}

					// Обновление счёта
					SudokuSolverMath.UpdateGameScore (false, score);

					// Отображение результата и отключение игрового режима до следующей генерации
					string msgText = RDLocale.GetText ("SolutionIsCorrect") + RDLocale.RNRN +
						"+" + score.ToString () + " 💎";
					if (!string.IsNullOrWhiteSpace (achiLine))
						msgText += "\t+" + achiLine;
					RDInterface.MessageBox (RDMessageTypes.Success_Center, msgText, 1500);

					// Отобразить решение в случае выигрыша (без return; режим игры отключается далее)
					if (win)
						MStats_Click (null, null);

					// Иначе продолжить игру
					else
						return true;
					}

				// Не отображать решение вне игрового режима
				else
					{
					RDInterface.MessageBox (RDMessageTypes.Success_Center,
						RDLocale.GetText ("SolutionIsCorrect"), 1000);
					return true;
					}
				}

			// Отображение решения
			SudokuSolverMath.GameMode = MatrixDifficulty.None;
			for (int r = 0; r < SudokuSolverMath.SideSize; r++)
				{
				for (int c = 0; c < SudokuSolverMath.SideSize; c++)
					{
					Button ct = buttons[r * (int)SudokuSolverMath.SideSize + c];
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

		// Закрытие окна
		private void SudokuSolverForm_FormClosing (object sender, FormClosingEventArgs e)
			{
			// Отмена текущего решения
			MClear_Click (null, null);

			// Сохранение поля судоку
			FlushMatrix ();

			// Сохранение окна
			RDGenerics.SaveWindowDimensions (this);
			}

		// Сохранение текущей матрицы
		private void FlushMatrix ()
			{
			string sudoku = "";
			for (int i = 0; i < buttons.Count; i++)
				sudoku += SudokuSolverMath.GetDigit (buttons[i].Text).ToString ();

			SudokuSolverMath.SudokuField = sudoku;
			}

		// Генерация матрицы судоку
		private void MGenerate_Click (object sender, EventArgs e)
			{
			// Запуск
			ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
			string n = tsmi.Name.Substring (tsmi.Name.Length - 1);
			MatrixDifficulty diff = (MatrixDifficulty)uint.Parse (n);
			SudokuSolverMath.SetGenerationDifficulty (diff);

			SudokuSolverMath.GenerateMatrix ();

			// Отображение результата
			for (int r = 0; r < SudokuSolverMath.SideSize; r++)
				{
				for (int c = 0; c < SudokuSolverMath.SideSize; c++)
					{
					Button ct = buttons[r * SudokuSolverMath.SideSize + c];
					if (SudokuSolverMath.ResultMatrix[r, c] == 0)
						SudokuSolverMath.SetProperty (ct, PropertyTypes.EmptyValue);
					else
						ct.Text = SudokuSolverMath.GetAppearance (SudokuSolverMath.ResultMatrix[r, c]);
					SudokuSolverMath.SetProperty (ct, PropertyTypes.OldColor);
					}
				}

			// Взведение игрового режима
			SudokuSolverMath.GameMode = diff;

			// Завершено
			}

		// Метод применяет штраф
		private static void ApplyPenalty ()
			{
			uint score = SudokuSolverMath.GetScore (ScoreTypes.Penalty);
			SudokuSolverMath.UpdateGameScore (true, score);

			string text = RDLocale.GetText ("SolutionIsIncorrect");
			if (SudokuSolverMath.GameMode != MatrixDifficulty.None)
				{
				text += (RDLocale.RNRN + "–" + score.ToString () + " 💎");
				RDInterface.MessageBox (RDMessageTypes.Error_Center, text, 1500);
				}
			else
				{
				RDInterface.MessageBox (RDMessageTypes.Error_Center, text, 1000);
				}
			}

		// Метод отображает игровую статистику
		private void MStats_Click (object sender, EventArgs e)
			{
			string text = "";

			if (sender == null)
				text += (RDLocale.GetText ("SolvedText") + RDLocale.RNRN);

			string[] stats = SudokuSolverMath.StatsValues;
			string spl = "   -   ";
			text += string.Format (RDLocale.GetText ("StatsText"), stats[0],
				stats[1] + spl + stats[2] + spl + stats[3],
				stats[4] + spl + stats[5] + spl + stats[6],
				stats[7] + spl + stats[8] + spl + stats[9]) + RDLocale.RNRN;

			text += string.Format (RDLocale.GetText ("StatsTextAchi"),
				stats[10], stats[11]);

			RDInterface.MessageBox (RDMessageTypes.Success_Center, text);
			}

		// Выбор представления ячеек
		private void MAppearance_Click (object sender, EventArgs e)
			{
			appearanceMenu.Show (this, menuPoint);
			}

		private void ChangeAppearance (object sender, EventArgs e)
			{
			if (sender != null)
				{
				int res = appearanceMenu.Items.IndexOf ((ToolStripItem)sender);

				// Подготовка к настройке для неначального вызова
				FlushMatrix ();
				SudokuSolverMath.CellsAppearance = (CellsAppearances)res;
				}

			// Настройка
			string line = SudokuSolverMath.SudokuField;
			for (int i = 0; i < buttons.Count; i++)
				{
				buttons[i].Text = SudokuSolverMath.GetAppearance (line[i].ToString ());

				if ((sender == null) && !SudokuSolverMath.CheckCondition (buttons[i], ConditionTypes.IsEmpty))
					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.OldColor);
				}
			}

		// Выбор цветовой схемы приложения
		private void MColorScheme_Click (object sender, EventArgs e)
			{
			colorSchemeMenu.Show (this, menuPoint);
			}

		private void ChangeColorScheme (object sender, EventArgs e)
			{
			if (sender != null)
				{
				int res = colorSchemeMenu.Items.IndexOf ((ToolStripItem)sender);
				SudokuSolverMath.ColorScheme = (ColorSchemes)res;
				}

			// Настройка
			this.BackColor = SudokuSolverMath.BackgroundColor;
			for (int i = 0; i < buttons.Count; i++)
				{
				SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.DeselectedCell);

				// Переназначение цветов для дальнейшей корректной работы метода CheckCondition
				if (SudokuSolverMath.CheckCondition (buttons[i], ConditionTypes.ContainsFoundValue))
					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.SuccessColor);
				else if (SudokuSolverMath.CheckCondition (buttons[i], ConditionTypes.ContainsNewValue))
					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.NewColor);
				else if (SudokuSolverMath.CheckCondition (buttons[i], ConditionTypes.ContainsErrorValue))
					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.ErrorColor);
				else
					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.OldColor);
				}

			newGameButton.ForeColor = checkButton.ForeColor = clearButton.ForeColor = buttons[0].ForeColor;
			}

		// Выбор режима работы приложения
		private void MAppMode_Clicked (object sender, EventArgs e)
			{
			gameModeMenu.Show (this, menuPoint);
			}

		private void ChangeAppMode (object sender, EventArgs e)
			{
			// Запрос
			if (sender != null)
				SudokuSolverMath.AppMode = (AppModes)(1 - gameModeMenu.Items.IndexOf ((ToolStripItem)sender));
			bool game = (SudokuSolverMath.AppMode == AppModes.Game);

			/*if (sender != null)
				{
				switch (RDInterface.LocalizedMessageBox (RDMessageTypes.Question_Left,
					"AppModeMessage", RDLDefaultTexts.Button_Yes, RDLDefaultTexts.Button_No,
					RDLDefaultTexts.Button_Cancel))
					{
					case RDMessageButtons.ButtonOne:
						SudokuSolverMath.AppMode = AppModes.Game;
						game = true;
						break;

					case RDMessageButtons.ButtonTwo:
						SudokuSolverMath.AppMode = AppModes.SolutionOnly;
						game = false;
						break;

					default:
						return;
					}
				}
			else
				{
				game = (SudokuSolverMath.AppMode == AppModes.Game);
				}*/

			// Оформление
			this.ClientSize = new Size ((int)(SudokuSolverMath.SideSize + 2) * buttonSize,
				(int)(SudokuSolverMath.SideSize + 2) * buttonSize + (game ? 3 : 1) * buttonSize);
			newGameButton.Visible = checkButton.Visible = clearButton.Visible = game;

			if (!game)
				SudokuSolverMath.GameMode = MatrixDifficulty.None;
			}

		// Запуск новой игры из интерфейсной кнопки
		private void NewGame_Click (object sender, EventArgs e)
			{
			RDMessageButtons res = RDInterface.MessageBox (RDMessageTypes.Question_Center,
				RDLocale.GetText ("DifficultyMessage"), RDLocale.GetText ("MGenerate_MDifficulty0"),
				RDLocale.GetText ("MGenerate_MDifficulty1"), RDLocale.GetText ("MGenerate_MDifficulty2"));

			MGenerate_Click (MGenerate.DropDownItems[(int)res - 1], e);
			}

		// Включение / выключение подсветки
		private void MHighlighting_Clicked (object sender, EventArgs e)
			{
			highlightingMenu.Show (this, menuPoint);
			}

		private void ChangeHighlighting (object sender, EventArgs e)
			{
			SudokuSolverMath.ShowAffectedCells = (highlightingMenu.Items.IndexOf ((ToolStripItem)sender) == 0);
			ChangeColorScheme (null, null);
			}
		}
	}
