using System;
using System.Drawing;
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

		private Color backgroundColor = Color.FromArgb (255, 255, 248),
			buttonsColor = Color.FromArgb (255, 255, 200),
			newTextColor = Color.FromArgb (0, 0, 200),
			errorTextColor = Color.FromArgb (200, 0, 0),
			foundTextColor = Color.FromArgb (0, 200, 0),
			oldTextColor = Color.FromArgb (0, 0, 0);

		/// <summary>
		/// Метод переопределяет обработку клавиатуры формой
		/// </summary>
		protected override bool ProcessCmdKey (ref Message msg, Keys keyData)
			{
			switch (keyData)
				{
				// Перенаправление движения по кнопкам
				case Keys.Up:
					for (int i = 0; i < SudokuSolver.SudokuSideSize; i++)
						this.SelectNextControl (this.ActiveControl, false, true, false, true);
					return true;

				case Keys.Down:
					for (int i = 0; i < SudokuSolver.SudokuSideSize; i++)
						this.SelectNextControl (this.ActiveControl, true, true, false, true);
					return true;

				case Keys.Left:
					if (this.Controls.IndexOf (this.ActiveControl) % SudokuSolver.SudokuSideSize == 0)
						{
						for (int i = 1; i < SudokuSolver.SudokuSideSize; i++)
							this.SelectNextControl (this.ActiveControl, true, true, false, true);
						}
					else
						{
						this.SelectNextControl (this.ActiveControl, false, true, false, true);
						}
					return true;

				case Keys.Right:
					if ((this.Controls.IndexOf (this.ActiveControl) + 1) % SudokuSolver.SudokuSideSize == 0)
						{
						for (int i = 1; i < SudokuSolver.SudokuSideSize; i++)
							this.SelectNextControl (this.ActiveControl, false, true, false, true);
						}
					else
						{
						this.SelectNextControl (this.ActiveControl, true, true, false, true);
						}
					return true;

				// Запуск решения
				case Keys.Return:
					Solve ();
					return true;

				// Полный и частичный сброс поля
				case Keys.Escape:
					for (int i = 0; i < this.Controls.Count; i++)
						this.Controls[i].Text = emptySign;
					return true;

				case Keys.Back:
					for (int i = 0; i < this.Controls.Count; i++)
						if ((this.Controls[i].ForeColor != oldTextColor) && (this.Controls[i].ForeColor != newTextColor))
							this.Controls[i].Text = emptySign;
					return true;

				// Отображение справки
				case Keys.F1:
					ProgramDescription.ShowAbout (false);
					return true;

				// Остальные клавиши обрабатываются стандартной процедурой
				default:
					return base.ProcessCmdKey (ref msg, keyData);
				}
			}

		/// <summary>
		/// Конструктор. Настраивает главную форму приложения
		/// </summary>
		public SudokuSolverForm ()
			{
			// Инициализация
			InitializeComponent ();

			this.Text = ProgramDescription.AssemblyTitle;
			this.ClientSize = new Size ((int)(SudokuSolver.SudokuSideSize + 2) * buttonSize,
				(int)(SudokuSolver.SudokuSideSize + 2) * buttonSize);
			this.BackColor = backgroundColor;

			// Формирование поля
			int sqrt = (int)Math.Sqrt (SudokuSolver.SudokuSideSize);
			for (int r = 0; r < SudokuSolver.SudokuSideSize; r++)
				{
				for (int c = 0; c < SudokuSolver.SudokuSideSize; c++)
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

					this.Controls.Add (lb);
					}
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

		// Решение задачи
		private void Solve ()
			{
			// Сборка массива
			Byte[,] matrix = new Byte[SudokuSolver.SudokuSideSize, SudokuSolver.SudokuSideSize];
			for (int r = 0; r < SudokuSolver.SudokuSideSize; r++)
				{
				for (int c = 0; c < SudokuSolver.SudokuSideSize; c++)
					{
					Control ct = this.Controls[r * (int)SudokuSolver.SudokuSideSize + c];
					if ((ct.Text != emptySign) &&
						((ct.ForeColor == oldTextColor) || (ct.ForeColor == newTextColor) || ct.ForeColor == errorTextColor))
						matrix[r, c] = Byte.Parse (ct.Text);
					else
						matrix[r, c] = 0;
					}
				}

			// Решение задачи
			SudokuSolver ss = new SudokuSolver (matrix);
			if (ss.InitResult != SudokuSolver.InitResults.OK)
				{
				for (int i = 0; i < this.Controls.Count; i++)
					this.Controls[i].ForeColor = errorTextColor;

				return;
				}

			// Отображение решения
			for (int r = 0; r < SudokuSolver.SudokuSideSize; r++)
				{
				for (int c = 0; c < SudokuSolver.SudokuSideSize; c++)
					{
					Control ct = this.Controls[r * (int)SudokuSolver.SudokuSideSize + c];
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
