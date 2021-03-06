///////////////////////////////////////////////////////////////////////////
// Функция изучения предположений
//
// Рекурсивная функция
// Возвращает 1, если результат получен
//            0, если ответ не был найден
//

unsigned int Search (void)
	{
	int i, j, k, p;
	unsigned int Mtc[SDS][SDS];

	// Создание копии исходной матрицы для данного предположения
	memcpy (Mtc, Mtx, MTX_SIZE);

	// Поиск первой невычисленной ячейки
	FOR_I
		{
		FOR_J
			{
			if (!IS_MIJ_POW2)
				{
				goto m1;
				}
			}
		}
m1:
	FOR_K
		{
		// Выполнение предположения (все цифры по порядку)
		Mtx[i][j] = 1 << k;

		// Прогонка матрицы с данным предположением
		// (копия матрицы с каждым вызовом функции является вынужденной мерой, т.к.
		// каждая прогонка почти полностью переписывает исходную матрицу)
		p = UpdateMatrix ();

		switch (p)
			{
			// Если получен конечный результат, функция его возвращает
			case 1:
				return 1;

			// Если результат требует уточнения
			case 0:
				// Делается новое предположение
				if (Search () == 1)
					{
					// Если оно дало конечный результат, нужно вернуть его наверх
					return 1;
					}
				else
					{
					// Если нет, нужно восстановить матрицу
					memcpy (Mtx, Mtc, MTX_SIZE);
					}
				break;

			// Если получена ошибка при прогонке, нужно восстановить матрицу
			default:
				memcpy (Mtx, Mtc, MTX_SIZE);
				break;
			}
		}

	// Правильных вариантов не найдено
	return 0;
	}
