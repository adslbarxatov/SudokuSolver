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
		private List<Button> buttons = new List<Button> ();

		private ContextMenu appearanceMenu = new ContextMenu ();
		private ContextMenu colorSchemeMenu = new ContextMenu ();

		/// <summary>
		/// Конструктор. Настраивает главную форму приложения
		/// </summary>
		public SudokuSolverForm ()
			{
			// Инициализация
			InitializeComponent ();
			LocalizeForm ();

			RDGenerics.LoadWindowDimensions (this);

			this.Text = ProgramDescription.AssemblyTitle;
			this.ClientSize = new Size ((int)(SudokuSolverMath.SideSize + 2) * buttonSize,
				(int)(SudokuSolverMath.SideSize + 2) * buttonSize + buttonSize);
			/*this.BackColor = SudokuSolverMath.BackgroundColor;*/

			// Формирование поля
			/*int sqrt = (int)Math.Sqrt (SudokuSolverMath.SideSize);*/
			for (int r = 0; r < SudokuSolverMath.SideSize; r++)
				{
				for (int c = 0; c < SudokuSolverMath.SideSize; c++)
					{
					Button lb = new Button ();

					SudokuSolverMath.SetProperty (lb, PropertyTypes.EmptyValue);
					SudokuSolverMath.SetProperty (lb, PropertyTypes.OldColor);
					/*lb.BackColor = buttonsColor;
					SudokuSolverMath.SetProperty (lb, PropertyTypes.DeselectedCell);*/
					lb.TextAlign = ContentAlignment.MiddleCenter;
					lb.Width = lb.Height = buttonSize;
					/*lb.Left = lb.Width * (c + 1) + 3 * (c / sqrt - sqrt / 2);
					lb.Top = lb.Height * (r + 1) + 3 * (r / sqrt - sqrt / 2) + buttonSize;*/
					lb.Left = lb.Width * (c + 1) + 3 * (c / SudokuSolverMath.SquareSize -
						SudokuSolverMath.SquareSize / 2);
					lb.Top = lb.Height * (r + 1) + 3 * (r / SudokuSolverMath.SquareSize -
						SudokuSolverMath.SquareSize / 2) + buttonSize;
					lb.Cursor = Cursors.UpArrow;
					lb.KeyDown += Lb_KeyDown;
					lb.FlatStyle = FlatStyle.Flat;
					lb.MouseWheel += Lb_MouseWheel;
					lb.MouseDown += Lb_MouseClick;

					this.Controls.Add (lb);
					buttons.Add (lb);
					}
				}

			// Загрузка настроек
			ChangeAppearance (null, null);  // Загружает сохранённую матрицу
			ChangeColorScheme (null, null);

			/*// Загрузка сохранённого состояния
			string sudoku = SudokuSolverMath.SudokuField;
			if (sudoku.Length == SudokuSolverMath.FullSize)
				{
				for (int i = 0; i < buttons.Count; i++)
					{
					buttons[i].Text = sudoku[i].ToString ();
					if (!SudokuSolverMath.CheckCondition (buttons[i], ConditionTypes.IsEmpty))
						SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.OldColor);
					}
				}*/
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
			appearanceMenu.MenuItems.Clear ();
			for (uint i = 0; i < SudokuSolverMath.CellsAppearancesCount; i++)
				appearanceMenu.MenuItems.Add (new MenuItem (SudokuSolverMath.GetCellsAppearanceName (i),
					ChangeAppearance));
			
			colorSchemeMenu.MenuItems.Clear ();
			for (uint i = 0; i < SudokuSolverMath.ColorSchemesCount; i++)
				colorSchemeMenu.MenuItems.Add (new MenuItem (RDLocale.GetText ("Color" + i.ToString ()),
					ChangeColorScheme));
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
			Solve (true);
			}

		// Проверка решения в текущем состоянии
		private void MCheck_Click (object sender, EventArgs e)
			{
			/*if (Solve (false))
				RDInterface.LocalizedMessageBox (RDMessageTypes.Success_Center, "SolutionIsCorrect", 1000);
			else*/

			if (!Solve (false))
				/*RDInterface.LocalizedMessageBox (RDMessageTypes.Error_Center, "SolutionIsIncorrect", 1000);*/
				ApplyPenalty ();
			}

		// Сброс решения (без очистки всех полей)
		private void MClear_Click (object sender, EventArgs e)
			{
			for (int i = 0; i < buttons.Count; i++)
				if (SudokuSolverMath.CheckCondition (buttons[i], ConditionTypes.ContainsFoundValue))
					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.EmptyValue);
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
			string file = "";
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
				/*buttons[i].Text = line[i].ToString ();*/
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
			/*SudokuSolverForm_ FormClosing (null, null);*/
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
					/*lb.Text = ((uint)e.KeyCode - 48).ToString ();*/
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
				/*v = int.Parse (b.Text) + (plus ? 1 : -1);*/
				v = SudokuSolverMath.GetDigit (b.Text);
				v += (plus ? 1 : -1);
				}
			catch { }

			if (plus)
				{
				if (v < 0)
					/*b.Text = "1";*/
					b.Text = SudokuSolverMath.GetAppearance (1);
				else if (v > 9)
					SudokuSolverMath.SetProperty (b, PropertyTypes.EmptyValue);
				else
					/*b.Text = v.ToString ();*/
					b.Text = SudokuSolverMath.GetAppearance ((Byte)v);
				}
			else
				{
				if (v < 0)
					/*b.Text = "9";*/
					b.Text = SudokuSolverMath.GetAppearance (9);
				else if (v < 1)
					SudokuSolverMath.SetProperty (b, PropertyTypes.EmptyValue);
				else
					/*b.Text = v.ToString ();*/
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
						/*matrix[r, c] = Byte.Parse (ct.Text);*/
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
					return false;
				}

			/*// Отображение решения
			if (!LoadResults)
				{
				for (int i = 0; i < buttons.Count; i++)
					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.OldColor);

				return true;
				}*/

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
					if (emptyCellsCount < 2)
						score += SudokuSolverMath.GetScore (ScoreTypes.GameCompletion);

					SudokuSolverMath.TotalScore += score;

					// Отображение результата и отключение игрового режима до следующей генерации
					RDInterface.MessageBox (RDMessageTypes.Success_Center,
						RDLocale.GetText ("SolutionIsCorrect") + RDLocale.RNRN +
						"+" + score.ToString () + " 💎", 1500);

					// Отобразить решение в случае выигрыша (без return; режим игры отключается далее)
					if (emptyCellsCount < 2)
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
						/*ct.Text = SudokuSolverMath.ResultMatrix[r, c].ToString ();*/
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
						/*ct.Text = SudokuSolverMath.ResultMatrix[r, c].ToString ();*/
						ct.Text = SudokuSolverMath.GetAppearance (SudokuSolverMath.ResultMatrix[r, c]);
					SudokuSolverMath.SetProperty (ct, PropertyTypes.OldColor);
					}
				}

			// Взведение игрового режима
			SudokuSolverMath.GameMode = diff;

			// Завершено
			}

		// Метод применяет штраф
		private void ApplyPenalty ()
			{
			uint score = SudokuSolverMath.GetScore (ScoreTypes.Penalty);
			if (score > SudokuSolverMath.TotalScore)
				score = SudokuSolverMath.TotalScore;

			SudokuSolverMath.TotalScore -= score;
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

			/*text += (RDLocale.GetText ("StatsText") + RDLocale.RNRN);
			text += ("💎\t" + SudokuSolverMath.TotalScore.ToString ("#,#0") + "\t\t");
			text += ("🟢\t" + SudokuSolverMath.EasyScore.ToString () + "\t\t");
			text += ("🟡\t" + SudokuSolverMath.MediumScore.ToString () + "\t\t");
			text += ("🔴\t" + SudokuSolverMath.HardScore.ToString ());*/
			text += string.Format (RDLocale.GetText ("StatsText"), SudokuSolverMath.TotalScore.ToString ("#,#0"),
				SudokuSolverMath.EasyScore, SudokuSolverMath.MediumScore, SudokuSolverMath.HardScore);

			RDInterface.MessageBox (RDMessageTypes.Success_Center, text);
			}

		// Выбор представления ячеек
		private void MAppearance_Click (object sender, EventArgs e)
			{
			appearanceMenu.Show (this, Point.Empty);
			}

		private void ChangeAppearance (object sender, EventArgs e)
			{
			int res;
			if (sender == null)
				{
				res = (int)SudokuSolverMath.CellsAppearance;
				}
			else
				{
				res = appearanceMenu.MenuItems.IndexOf ((MenuItem)sender);

				// Подготовка к настройке для неначального вызова
				FlushMatrix ();
				SudokuSolverMath.CellsAppearance = (CellsAppearances)res;
				}

			// Настройка
			string line = SudokuSolverMath.SudokuField;
			for (int i = 0; i < buttons.Count; i++)
				{
				buttons[i].Text = SudokuSolverMath.GetAppearance (line[i].ToString ());
				/*buttons[i].FontSize = SudokuSolverMath.CellsAppearancesFontSize;
				buttons[i].FontAttributes = SudokuSolverMath.CellsAppearancesBoldFont ?
					FontAttributes.Bold : FontAttributes.None;*/

				if ((sender == null) && !SudokuSolverMath.CheckCondition (buttons[i], ConditionTypes.IsEmpty))
					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.OldColor);
				}
			}

		// Выбор цветовой схемы приложения
		private void MColorScheme_Click (object sender, EventArgs e)
			{
			colorSchemeMenu.Show (this, Point.Empty);
			}

		private void ChangeColorScheme (object sender, EventArgs e)
			{
			int res;
			if (sender==null)
				{
				res = (int)SudokuSolverMath.ColorScheme;
				}
			else
				{
				res = colorSchemeMenu.MenuItems.IndexOf ((MenuItem)sender);
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
			}
		}
	}
