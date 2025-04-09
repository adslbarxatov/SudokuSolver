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

		private Color backgroundColor = Color.FromArgb (255, 255, 248);
		private Color buttonsColor = Color.FromArgb (255, 255, 200);

		private List<Button> buttons = new List<Button> ();

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
			this.ClientSize = new Size ((int)(SudokuSolverMath.SudokuSideSize + 2) * buttonSize,
				(int)(SudokuSolverMath.SudokuSideSize + 2) * buttonSize + buttonSize);
			this.BackColor = backgroundColor;

			// Формирование поля
			int sqrt = (int)Math.Sqrt (SudokuSolverMath.SudokuSideSize);
			for (int r = 0; r < SudokuSolverMath.SudokuSideSize; r++)
				{
				for (int c = 0; c < SudokuSolverMath.SudokuSideSize; c++)
					{
					Button lb = new Button ();

					SudokuSolverMath.SetProperty (lb, PropertyTypes.EmptyValue);
					SudokuSolverMath.SetProperty (lb, PropertyTypes.OldColor);
					lb.BackColor = buttonsColor;
					lb.TextAlign = ContentAlignment.MiddleCenter;
					lb.Width = lb.Height = buttonSize;
					lb.Left = lb.Width * (c + 1) + 3 * (c / sqrt - sqrt / 2);
					lb.Top = lb.Height * (r + 1) + 3 * (r / sqrt - sqrt / 2) + buttonSize;
					lb.Cursor = Cursors.UpArrow;
					lb.KeyDown += Lb_KeyDown;
					/*lb.Click += Lb_Click;*/
					lb.FlatStyle = FlatStyle.Flat;
					lb.MouseWheel += Lb_MouseWheel;
					/*lb.MouseClick += Lb_MouseClick;*/
					lb.MouseDown += Lb_MouseClick;

					this.Controls.Add (lb);
					buttons.Add (lb);
					}
				}

			// Загрузка сохранённого состояния
			string sudoku = SudokuSolverMath.SudokuField;
			if (sudoku.Length == SudokuSolverMath.SudokuSideSize * SudokuSolverMath.SudokuSideSize)
				{
				for (int i = 0; i < buttons.Count; i++)
					{
					buttons[i].Text = sudoku[i].ToString ();
					if (!SudokuSolverMath.CheckCondition (buttons[i], ConditionTypes.IsEmpty))
						SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.OldColor);
					}
				}
			}

		// Метод локализует форму
		private void LocalizeForm ()
			{
			MActivities.Text = RDLocale.GetText ("MainMenu_MActivities");

			for (int i = 0; i < MActivities.DropDownItems.Count; i++)
				MActivities.DropDownItems[i].Text = RDLocale.GetText (MActivities.Name + "_" +
					MActivities.DropDownItems[i].Name);
			for (int i = 0; i < MGenerate.DropDownItems.Count; i++)
				MGenerate.DropDownItems[i].Text = RDLocale.GetText (MGenerate.Name + "_" +
					MGenerate.DropDownItems[i].Name);

			MExit.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Exit);

			OFDialog.Title = RDLocale.GetText ("OFName");
			SFDialog.Title = RDLocale.GetText ("SFName");
			OFDialog.Filter = SFDialog.Filter = RDLocale.GetText ("OFFilter");
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
					for (int i = 0; i < SudokuSolverMath.SudokuSideSize; i++)
						this.SelectNextControl (this.ActiveControl, false, true, false, true);
					return true;

				case Keys.Down:
					for (int i = 0; i < SudokuSolverMath.SudokuSideSize; i++)
						this.SelectNextControl (this.ActiveControl, true, true, false, true);
					return true;

				case Keys.Left:
					if (buttons.IndexOf ((Button)this.ActiveControl) % SudokuSolverMath.SudokuSideSize == 0)
						{
						for (int i = 1; i < SudokuSolverMath.SudokuSideSize; i++)
							this.SelectNextControl (this.ActiveControl, true, true, false, true);
						}
					else
						{
						this.SelectNextControl (this.ActiveControl, false, true, false, true);
						}
					return true;

				case Keys.Right:
					if ((buttons.IndexOf ((Button)this.ActiveControl) + 1) % SudokuSolverMath.SudokuSideSize == 0)
						{
						for (int i = 1; i < SudokuSolverMath.SudokuSideSize; i++)
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
			if (Solve (false))
				RDInterface.LocalizedMessageBox (RDMessageTypes.Success_Center, "SolutionIsCorrect", 1000);
			else
				RDInterface.LocalizedMessageBox (RDMessageTypes.Error_Center, "SolutionIsIncorrect", 1000);
			}

		// Сброс решения (без очистки всех полей)
		private void MClear_Click (object sender, EventArgs e)
			{
			for (int i = 0; i < buttons.Count; i++)
				/*if (buttons[i].ForeColor == foundTextColor)*/
				if (SudokuSolverMath.CheckCondition (buttons[i], ConditionTypes.ContainsFoundValue))
					/*buttons[i].Text = SudokuSolverMath.EmptySign;*/
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
				/*buttons[i].Text = SudokuSolverMath.EmptySign;*/
				SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.EmptyValue);
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
				buttons[i].Text = line[i].ToString ();
				/*if (buttons[i].Text == SudokuSolverMath.Empty Sign)*/
				if (SudokuSolverMath.CheckCondition (buttons[i], ConditionTypes.IsEmpty))
					/*buttons[i].ForeColor = SudokuSolverMath.NewNumberColor;*/
					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.NewColor);
				else
					/*buttons[i].ForeColor = SudokuSolverMath.OldNumberColor;*/
					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.OldColor);
				}
			}

		// Выгрузка таблицы в файл
		private void MSave_Click (object sender, EventArgs e)
			{
			SFDialog.ShowDialog ();
			}

		private void SFDialog_FileOk (object sender, CancelEventArgs e)
			{
			// Выгрузка данных
			SudokuSolverForm_FormClosing (null, null);
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
					lb.Text = ((uint)e.KeyCode - 48).ToString ();
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
			int v = -1;

			bool plus;
			if ((e.Delta < 0) || (e.Button == MouseButtons.Right))
				plus = false;
			else
				plus = true;

			try
				{
				v = int.Parse (b.Text) + (plus ? 1 : -1);
				}
			catch { }

			if (plus)
				{
				if (v < 0)
					b.Text = "1";
				else if (v > 9)
					SudokuSolverMath.SetProperty (b, PropertyTypes.EmptyValue);
				else
					b.Text = v.ToString ();
				}
			else
				{
				if (v < 0)
					b.Text = "9";
				else if (v < 1)
					SudokuSolverMath.SetProperty (b, PropertyTypes.EmptyValue);
				else
					b.Text = v.ToString ();
				}

			SudokuSolverMath.SetProperty (b, PropertyTypes.NewColor);
			}

		/*private void Lb_Click (object sender, EventArgs e)
			{
			Button b = (Button)sender;
			uint v = 0;

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
			}*/

		// Решение задачи
		private bool Solve (bool LoadResults)
			{
			// Сборка массива
			Byte[,] matrix = new Byte[SudokuSolverMath.SudokuSideSize, SudokuSolverMath.SudokuSideSize];
			for (int r = 0; r < SudokuSolverMath.SudokuSideSize; r++)
				{
				for (int c = 0; c < SudokuSolverMath.SudokuSideSize; c++)
					{
					Button ct = buttons[r * (int)SudokuSolverMath.SudokuSideSize + c];
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

			// Отображение решения
			if (!LoadResults)
				{
				for (int i = 0; i < buttons.Count; i++)
					SudokuSolverMath.SetProperty (buttons[i], PropertyTypes.OldColor);

				return true;
				}

			for (int r = 0; r < SudokuSolverMath.SudokuSideSize; r++)
				{
				for (int c = 0; c < SudokuSolverMath.SudokuSideSize; c++)
					{
					Button ct = buttons[r * (int)SudokuSolverMath.SudokuSideSize + c];
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

		// Закрытие окна
		private void SudokuSolverForm_FormClosing (object sender, FormClosingEventArgs e)
			{
			// Сохранение поля судоку
			string sudoku = "";
			for (int i = 0; i < buttons.Count; i++)
				sudoku += buttons[i].Text;

			SudokuSolverMath.SudokuField = sudoku;

			// Сохранение окна
			RDGenerics.SaveWindowDimensions (this);
			}

		/*/// <summary>
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
		private const string sudokuFieldPar = "SudokuField";*/

		// Генерация матрицы судоку
		private void MGenerate_Click (object sender, EventArgs e)
			{
			// Запуск
			ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
			string n = tsmi.Name.Substring (tsmi.Name.Length - 1);
			SudokuSolverMath.SetGenerationDifficulty ((MatrixDifficulty)uint.Parse (n));

			SudokuSolverMath.GenerateMatrix ();

			// Отображение результата
			for (int r = 0; r < SudokuSolverMath.SudokuSideSize; r++)
				{
				for (int c = 0; c < SudokuSolverMath.SudokuSideSize; c++)
					{
					Button ct = buttons[r * (int)SudokuSolverMath.SudokuSideSize + c];
					if (SudokuSolverMath.ResultMatrix[r, c] == 0)
						SudokuSolverMath.SetProperty (ct, PropertyTypes.EmptyValue);
					else
						ct.Text = SudokuSolverMath.ResultMatrix[r, c].ToString ();
					SudokuSolverMath.SetProperty (ct, PropertyTypes.OldColor);
					}
				}

			// Завершено
			}
		}
	}
