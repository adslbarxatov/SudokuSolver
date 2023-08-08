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
		private const string emptySign = " ";

		private Color backgroundColor = Color.FromArgb (255, 255, 248);
		private Color buttonsColor = Color.FromArgb (255, 255, 200);
		private Color newTextColor = Color.FromArgb (0, 0, 200);
		private Color errorTextColor = Color.FromArgb (200, 0, 0);
		private Color foundTextColor = Color.FromArgb (0, 200, 0);
		private Color oldTextColor = Color.FromArgb (0, 0, 0);

		private string[] splitters = new string[] { "\r", "\n", "\t", " ", ";" };
		private const string checker = "123456789";

		private List<Control> buttons = new List<Control> ();

		/// <summary>
		/// Конструктор. Настраивает главную форму приложения
		/// </summary>
		public SudokuSolverForm ()
			{
			// Инициализация
			InitializeComponent ();
			LocalizeForm ();

			this.Text = ProgramDescription.AssemblyTitle;
			this.ClientSize = new Size ((int)(SudokuSolverClass.SudokuSideSize + 2) * buttonSize,
				(int)(SudokuSolverClass.SudokuSideSize + 2) * buttonSize);
			this.BackColor = backgroundColor;

			// Формирование поля
			int sqrt = (int)Math.Sqrt (SudokuSolverClass.SudokuSideSize);
			for (int r = 0; r < SudokuSolverClass.SudokuSideSize; r++)
				{
				for (int c = 0; c < SudokuSolverClass.SudokuSideSize; c++)
					{
					Button lb = new Button ();

					lb.Text = emptySign;
					lb.BackColor = buttonsColor;
					lb.TextAlign = ContentAlignment.MiddleCenter;
					lb.Width = lb.Height = buttonSize;
					lb.Left = lb.Width * (c + 1) + 3 * (c / sqrt - sqrt / 2);
					lb.Top = lb.Height * (r + 1) + 3 * (r / sqrt - sqrt / 2);

					lb.Cursor = Cursors.UpArrow;
					lb.KeyDown += Lb_KeyDown;
					lb.Click += Lb_Click;
					lb.FlatStyle = FlatStyle.Flat;

					this.Controls.Add (lb);
					buttons.Add (lb);
					}
				}
			}

		// Метод локализует форму
		private void LocalizeForm ()
			{
			Localization.SetControlsText (MainMenu);
			Localization.SetControlsText (MActivities);
			MExit.Text = Localization.GetDefaultText (LzDefaultTextValues.Button_Exit);

			OFDialog.Title = Localization.GetText ("OFName");
			SFDialog.Title = Localization.GetText ("SFName");
			OFDialog.Filter = SFDialog.Filter = Localization.GetText ("OFFilter");
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
					for (int i = 0; i < SudokuSolverClass.SudokuSideSize; i++)
						this.SelectNextControl (this.ActiveControl, false, true, false, true);
					return true;

				case Keys.Down:
					for (int i = 0; i < SudokuSolverClass.SudokuSideSize; i++)
						this.SelectNextControl (this.ActiveControl, true, true, false, true);
					return true;

				case Keys.Left:
					if (buttons.IndexOf (this.ActiveControl) % SudokuSolverClass.SudokuSideSize == 0)
						{
						for (int i = 1; i < SudokuSolverClass.SudokuSideSize; i++)
							this.SelectNextControl (this.ActiveControl, true, true, false, true);
						}
					else
						{
						this.SelectNextControl (this.ActiveControl, false, true, false, true);
						}
					return true;

				case Keys.Right:
					if ((buttons.IndexOf (this.ActiveControl) + 1) % SudokuSolverClass.SudokuSideSize == 0)
						{
						for (int i = 1; i < SudokuSolverClass.SudokuSideSize; i++)
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
			Solve ();
			}

		// Сброс решения (без очистки всех полей)
		private void MClear_Click (object sender, EventArgs e)
			{
			for (int i = 0; i < buttons.Count; i++)
				if ((buttons[i].ForeColor != oldTextColor) && (buttons[i].ForeColor != newTextColor))
					buttons[i].Text = emptySign;
			}

		// Полный сброс
		private void MReset_Click (object sender, EventArgs e)
			{
			if (RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "ResetWarning",
				LzDefaultTextValues.Button_YesNoFocus, LzDefaultTextValues.Button_No) !=
				RDMessageButtons.ButtonOne)
				return;

			for (int i = 0; i < buttons.Count; i++)
				buttons[i].Text = emptySign;
			}

		// Справка
		private void MHelp_Click (object sender, EventArgs e)
			{
			RDGenerics.ShowAbout (false);
			}

		// Язык интерфейса
		private void MLanguage_Click (object sender, EventArgs e)
			{
			if (RDGenerics.MessageBox ())
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
				file = File.ReadAllText (OFDialog.FileName, RDGenerics.GetEncoding (SupportedEncodings.UTF8));
				}
			catch
				{
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center,
					Localization.GetFileProcessingMessage (OFDialog.FileName,
					LzFileProcessingMessageTypes.Load_Failure));
				return;
				}

			// Обработка
			for (int i = 0; i < splitters.Length; i++)
				file = file.Replace (splitters[i], "");

			if (file.Length < SudokuSolverClass.SudokuSideSize * SudokuSolverClass.SudokuSideSize)
				{
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "MessageNotEnough");
				return;
				}

			// Загрузка
			for (int i = 0; i < buttons.Count; i++)
				{
				string c = file[i].ToString ();
				if (checker.Contains (c))
					{
					buttons[i].Text = c;
					buttons[i].ForeColor = oldTextColor;
					}
				else
					{
					buttons[i].Text = emptySign;
					buttons[i].ForeColor = newTextColor;
					}
				}

			// Решение
			Solve ();
			}

		// Выгрузка таблицы в файл
		private void MSave_Click (object sender, EventArgs e)
			{
			SFDialog.ShowDialog ();
			}

		private void SFDialog_FileOk (object sender, CancelEventArgs e)
			{
			// Выгрузка данных
			string file = "";
			int sqrt = (int)Math.Sqrt (SudokuSolverClass.SudokuSideSize);
			int cubedSqrt = sqrt * sqrt * sqrt;

			for (int i = 1; i <= buttons.Count; i++)
				{
				file += buttons[i - 1].Text.Replace (emptySign, "-");

				if (i % cubedSqrt == 0)
					file += "\r\n\r\n";
				else if (i % SudokuSolverClass.SudokuSideSize == 0)
					file += "\r\n";
				else if (i % sqrt == 0)
					file += " ";
				}

			// Сохранение
			try
				{
				File.WriteAllText (SFDialog.FileName, file, RDGenerics.GetEncoding (SupportedEncodings.UTF8));
				}
			catch
				{
				RDGenerics.MessageBox (RDMessageTypes.Warning_Center,
					Localization.GetFileProcessingMessage (SFDialog.FileName,
					LzFileProcessingMessageTypes.Save_Failure));
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
					lb.ForeColor = newTextColor;
					lb.Text = ((uint)e.KeyCode - 48).ToString ();
					break;

				default:
					lb.Text = emptySign;
					break;
				}
			}

		// Нажание кнопок
		private void Lb_Click (object sender, EventArgs e)
			{
			Button b = (Button)sender;
			uint v = 0;

			try
				{
				v = uint.Parse (b.Text);
				}
			catch { }

			v++;
			if (v > 9)
				v = 1;
			b.Text = v.ToString ();
			b.ForeColor = newTextColor;
			}

		// Решение задачи
		private void Solve ()
			{
			// Сборка массива
			Byte[,] matrix = new Byte[SudokuSolverClass.SudokuSideSize, SudokuSolverClass.SudokuSideSize];
			for (int r = 0; r < SudokuSolverClass.SudokuSideSize; r++)
				{
				for (int c = 0; c < SudokuSolverClass.SudokuSideSize; c++)
					{
					Control ct = buttons[r * (int)SudokuSolverClass.SudokuSideSize + c];
					if ((ct.Text != emptySign) &&
						((ct.ForeColor == oldTextColor) || (ct.ForeColor == newTextColor) ||
						(ct.ForeColor == errorTextColor)))
						matrix[r, c] = Byte.Parse (ct.Text);
					else
						matrix[r, c] = 0;
					}
				}

			// Решение задачи
			SudokuSolverClass ss = new SudokuSolverClass (matrix);
			if (ss.InitResult != SudokuSolverClass.InitResults.OK)
				{
				for (int i = 0; i < buttons.Count; i++)
					buttons[i].ForeColor = errorTextColor;

				return;
				}

			// Отображение решения
			for (int r = 0; r < SudokuSolverClass.SudokuSideSize; r++)
				{
				for (int c = 0; c < SudokuSolverClass.SudokuSideSize; c++)
					{
					Control ct = buttons[r * (int)SudokuSolverClass.SudokuSideSize + c];
					if ((ct.Text != emptySign) &&
						((ct.ForeColor == oldTextColor) || (ct.ForeColor == newTextColor) || ct.ForeColor == errorTextColor))
						{
						ct.ForeColor = oldTextColor;
						}
					else
						{
						ct.Text = ss.ResultMatrix[r, c].ToString ();
						ct.ForeColor = foundTextColor;
						}
					}
				}

			// Выполнено
			}
		}
	}
