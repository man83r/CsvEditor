using System;
using System.Data;

namespace CsvEditor
{
    class MyBigClass
    {
        private string tableName = "SuperTable";

        private int[] _RowPaint = new int[0]; //строки, выделенные цветом
        private int[] _ColumnsPaint = new int[0]; //столбцы, выделенные цветом
        private DataSet _TableDataBuff = new DataSet();
        private int _indexMyColumn = 0; //переменная для добавления NewColumn с уникальным индексом

        private int _LoadState = 0; //переменная для отслеживания корректности загрузки таблицы. 0- всё ОК, 1 - число ячеек больше заголовков, 2 - число ячеек меньше заголовков
        private int _LoadStateMinMax = 0; //переменная для отслеживания корректности загрузки таблицы. 0 - Min и Max равны; 1- не равны.
        private int _LoadStateColumnsName = 0; //0 - в таблице нет повторяющихся заголовков; 1 - в таблице есть повторяющиеся заголовки, изменены на *ИМЯ_ЗАГОЛОВКА

        private int _NumberRowForCopy;
        private int _NumberRowForPast;

        public DataSet TableDataBuff
        {
            get { return _TableDataBuff; }
            set { _TableDataBuff = value; }
        }

        public int[] RowPaint
        {
            get { return _RowPaint; }
            set { _RowPaint = value; }
        }

        public int[] ColumnsPaint
        {
            get { return _ColumnsPaint; }
            set { _ColumnsPaint = value; }
        }

        public int LoadState
        {
            get { return _LoadState; }
        }

        public int LoadStateMinMax
        {
            get { return _LoadStateMinMax; }
        }

        public int LoadStateColumnsName
        {
            get { return _LoadStateColumnsName; }
        }

        public int NumberRowForCopy
        {
            set { _NumberRowForCopy = value; }
        }

        public int NumberRowForPast
        {
            set { _NumberRowForPast = value; }
        }

        public void ResetState()
        {
            _indexMyColumn = 0;
            _LoadState = 0;
            _LoadStateMinMax = 0;
            _LoadStateColumnsName = 0;
        }

        static void cfRemoveAt<T>(ref T[] array, int index)
        {
            T[] newArray = new T[array.Length - 1];

            for (int i = 0; i < index; i++)
            {
                newArray[i] = array[i];
            }

            for (int i = index + 1; i < array.Length; i++)
            {
                newArray[i - 1] = array[i];
            }

            array = newArray;
        }

        public void cfPaintReset(ref int[] PaintIndicatorArray)
        {
            int[] newPaintIndicatorArray = new int[0];
            PaintIndicatorArray = newPaintIndicatorArray;
        }

        private void cfEditPaintScript(int AddDelChange, int Index, ref int[] PaintScript)  //AddDelChange: 0 - добавил строку/столбец, 1 - удалил строку/столбец, 2 изменил цвет руками
        {                                                                           //Index - индекс удалённой/добавленной/изменённой строки/столбца
            bool IndexContain = false; //для проверки есть ли в массиве данный индекс
            int[] newPaintScript;
            switch (AddDelChange)
            {
                case 0:
                    newPaintScript = new int[PaintScript.Length + 1];

                    for (int i = 0; i < PaintScript.Length; i++)
                    {
                        if (PaintScript[i] < Index)
                        {
                            newPaintScript[i] = PaintScript[i];
                        }
                        else
                        {
                            newPaintScript[i] = PaintScript[i] + 1;
                        }
                    }

                    newPaintScript[PaintScript.Length] = Index;

                    PaintScript = newPaintScript;

                    break;


                case 1:
                    for (int i = 0; i < PaintScript.Length; i++)
                    {
                        if (PaintScript[i] == Index) IndexContain = true;
                    }
                    if (IndexContain)
                    {
                        newPaintScript = new int[PaintScript.Length - 1];

                        for (int i = 0, j = 0; i < newPaintScript.Length; i++, j++)
                        {
                            if (PaintScript[j] != Index)
                            {
                                if (PaintScript[j] < Index)
                                {
                                    newPaintScript[i] = PaintScript[j];
                                }
                                else
                                {
                                    newPaintScript[i] = PaintScript[j] - 1;
                                }
                            }
                            else
                            {
                                j++;
                                if (PaintScript[j] < Index)
                                {
                                    newPaintScript[i] = PaintScript[j];
                                }
                                else
                                {
                                    newPaintScript[i] = PaintScript[j] - 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        newPaintScript = new int[PaintScript.Length];

                        for (int i = 0; i < newPaintScript.Length; i++)
                        {
                            if (PaintScript[i] < Index)
                            {
                                newPaintScript[i] = PaintScript[i];
                            }
                            else
                                newPaintScript[i] = PaintScript[i] - 1;

                        }
                    }
                    PaintScript = newPaintScript;
                    break;


                case 2:
                    for (int i = 0; i < PaintScript.Length; i++)
                    {
                        if (PaintScript[i] == Index) IndexContain = true;
                    }
                    if (IndexContain)
                    {
                        newPaintScript = new int[PaintScript.Length - 1];

                        for (int i = 0, j = 0; i < newPaintScript.Length; i++, j++)
                        {
                            if (PaintScript[j] != Index)
                            {
                                newPaintScript[i] = PaintScript[j];
                            }
                            else
                            {
                                j++;
                                newPaintScript[i] = PaintScript[j];
                            }

                        }
                    }
                    else
                    {
                        newPaintScript = new int[PaintScript.Length + 1];

                        for (int i = 0; i < PaintScript.Length; i++)
                        {
                            newPaintScript[i] = PaintScript[i];
                        }

                        newPaintScript[PaintScript.Length] = Index;
                    }

                    PaintScript = newPaintScript;

                    break;
                default:
                    break;
            }
        }

        public void cfLoadCsvTable(string[] StrokiRows)
        {
            cfPaintReset(ref _RowPaint);
            cfPaintReset(ref _ColumnsPaint);
            string delimiter = CsvPreferences.delimiter;

            string[] MassStringForName = StrokiRows[0].Split(delimiter.ToCharArray());

            bool firstValue = true;

            foreach (string NameColums in MassStringForName)
            {
                string NameColumsPovtor = NameColums;
                bool flagUniqueColumnName = false;

                if (firstValue == false)
                {

                    while (flagUniqueColumnName == false)
                    {
                        for (int j = 0; j < _TableDataBuff.Tables[tableName].Columns.Count; j++)
                        {
                            if (_TableDataBuff.Tables[tableName].Columns[j].ColumnName == NameColumsPovtor)
                            {
                                flagUniqueColumnName = false;
                                NameColumsPovtor = "*" + NameColumsPovtor;
                                _LoadStateColumnsName = 1;
                                break;
                            }
                            else
                            {
                                flagUniqueColumnName = true;
                            }
                        }
                    }
                } else { firstValue = false;  }
                _TableDataBuff.Tables[tableName].Columns.Add(NameColumsPovtor);
            }

            cfRemoveAt(ref StrokiRows, 0);

            int maxCellInRowCount = 0;
            int minCellInRowCount = _TableDataBuff.Tables[tableName].Columns.Count; //число ячеек в заголовке

            foreach (string anyThing in StrokiRows)
            {
                string[] items = anyThing.Split(delimiter.ToCharArray());

                if (maxCellInRowCount < items.Length) maxCellInRowCount = items.Length;
                if (minCellInRowCount > items.Length) 
                {
                    minCellInRowCount = items.Length;
                    if (minCellInRowCount < _TableDataBuff.Tables[tableName].Columns.Count && _LoadState != 1) _LoadState = 2;
                }

                if (_TableDataBuff.Tables[tableName].Columns.Count < items.Length)
                {
                    int raznica = items.Length - _TableDataBuff.Tables[tableName].Columns.Count;
                    for (int i=0; i< raznica; i++)
                    {
                        bool flagUniqueColumnName = false;
                        while (flagUniqueColumnName == false)
                        {
                            for (int j = 0; j < _TableDataBuff.Tables[tableName].Columns.Count; j++)
                            {
                                if (_TableDataBuff.Tables[tableName].Columns[j].ColumnName == $"NewColumn{_indexMyColumn}")
                                {
                                    flagUniqueColumnName = false;
                                    _indexMyColumn++;
                                }
                                else
                                {
                                    flagUniqueColumnName = true;
                                }
                            }
                        }
                        _LoadState = 1;
                        _TableDataBuff.Tables[tableName].Columns.Add($"NewColumn{_indexMyColumn}");
                    }
                }
                _TableDataBuff.Tables[tableName].Rows.Add(items);
            }
            if (minCellInRowCount != maxCellInRowCount) _LoadStateMinMax = 1;
        }

        public void cfReplace()
        {
            int RowCount = _TableDataBuff.Tables[tableName].Rows.Count; //определяем число строк
            int ColCount = _TableDataBuff.Tables[tableName].Columns.Count;
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColCount; j++)
                {
                    if (_TableDataBuff.Tables[tableName].Rows[i][j] != null && _TableDataBuff.Tables[tableName].Rows[i][j].ToString() != "")
                    {
                        string buffer111;
                        buffer111 = _TableDataBuff.Tables[tableName].Rows[i][j].ToString();
                        buffer111 = buffer111.Replace(StaticData.SourceText, StaticData.TextForReplace);
                        _TableDataBuff.Tables[tableName].Rows[i][j] = buffer111;
                    }
                }
            }
        }

        public void cmNew()
        {
            cfPaintReset(ref _RowPaint);
            cfPaintReset(ref _ColumnsPaint);
            for (int i = 1; i < 4; i++) _TableDataBuff.Tables[tableName].Columns.Add($"Column{i}");

            for (int i = 1; i < 4; i++) _TableDataBuff.Tables[tableName].Rows.Add("", "");
        }

        public void cfInsertRow(bool dupl, int RowNumb)
        {
            DataRow NewRow = _TableDataBuff.Tables[tableName].NewRow(); //создаём новую строку
            _TableDataBuff.Tables[tableName].Rows.Add(NewRow); //добавляем новую строку в таблицу
            int tableSize = _TableDataBuff.Tables[tableName].Rows.Count; //определяем число строк
                                                                         //копируем все строки на 1 вниз
            for (int i = tableSize - 1; i > RowNumb; i--)
            {
                for (int j = 0; j < _TableDataBuff.Tables[tableName].Columns.Count; j++)
                {
                    _TableDataBuff.Tables[tableName].Rows[i][j] = _TableDataBuff.Tables[tableName].Rows[i - 1][j]; //индексы ячейки [строка, столбец]
                }
            }
            //созданную строку заполним пустыми значениями или дубликат строки
            if (dupl)
            {
                _TableDataBuff.Tables[tableName].Rows[RowNumb + 1][0] += "Duplic_"; //RowNumb+1, чтобы дубликат "вставлялся" ниже 
                cfEditPaintScript(0, RowNumb + 1, ref _RowPaint);
            }
            else
            {
                for (int j = 0; j < _TableDataBuff.Tables[tableName].Columns.Count; j++)
                {
                    _TableDataBuff.Tables[tableName].Rows[RowNumb][j] = "";
                }
                cfEditPaintScript(0, RowNumb, ref _RowPaint);
            }
        }

        public void cbDelete_Click(int cellNumb)
        {
            _TableDataBuff.Tables[tableName].Rows.RemoveAt(cellNumb); //удаляем строчку по ID
            cfEditPaintScript(1, cellNumb, ref _RowPaint);
        }

        public void cmPastRow()
        {
            try
            {
                if (_NumberRowForCopy <= _TableDataBuff.Tables[tableName].Rows.Count && _NumberRowForPast <= _TableDataBuff.Tables[tableName].Rows.Count &&
               _NumberRowForCopy >= 0 && _NumberRowForPast >= 0 && _NumberRowForCopy != _NumberRowForPast)
                {
                    for (int j = 0; j < _TableDataBuff.Tables[tableName].Columns.Count; j++)
                        _TableDataBuff.Tables[tableName].Rows[_NumberRowForPast][j] = _TableDataBuff.Tables[tableName].Rows[_NumberRowForCopy][j];
                }
                cfEditPaintScript(2, _NumberRowForPast, ref _RowPaint);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void cbDuplToEndClick(int RowNumb)
        {
            DataRow NewRow = _TableDataBuff.Tables[tableName].NewRow(); //создаём новую строку
            _TableDataBuff.Tables[tableName].Rows.Add(NewRow); //добавляем новую строку в таблицу
            int tableSize = _TableDataBuff.Tables[tableName].Rows.Count; //определяем число строк
            for (int j = 0; j < _TableDataBuff.Tables[tableName].Columns.Count; j++)
            {
                _TableDataBuff.Tables[tableName].Rows[tableSize - 1][j] = _TableDataBuff.Tables[tableName].Rows[RowNumb][j];
            }
            _TableDataBuff.Tables[tableName].Rows[tableSize - 1][0] += "Duplic_";
            cfEditPaintScript(0, tableSize - 1, ref _RowPaint);
        }

        public void cbHighlightRow(int RowNumb)
        {
            cfEditPaintScript(2, RowNumb, ref _RowPaint);
        }

        public void cfInsertColumn(int bias, int ColNumb)
        {
            _TableDataBuff.Tables[tableName].Columns.Add();      //добавим столбец
            int ColCount = _TableDataBuff.Tables[tableName].Columns.Count; //определим количество столбцов
            string bufString; //переменная для промежуточного хранения заголовка столбца

            for (int i = ColCount - 1; i > ColNumb + bias; i--)
            {
                //переместим все ячейки до i-й на 1 вправо
                for (int j = 0; j < _TableDataBuff.Tables[tableName].Rows.Count; j++)
                {
                    _TableDataBuff.Tables[tableName].Rows[j][i] = _TableDataBuff.Tables[tableName].Rows[j][i - 1]; //индексы ячейки [строка, столбец]
                }


                //заголовки столбцов переместим на 1 вправо
                bufString = _TableDataBuff.Tables[tableName].Columns[i - 1].ColumnName;

                if (i > ColNumb + 1 + bias)
                {
                    _TableDataBuff.Tables[tableName].Columns[i - 1].ColumnName = $"noname{i - 1}";
                }
                else //последний заголовок переименуем в NewColumn
                {
                    //проверим все заголовки столбцов и выберем наибольший _indexMyColumn, чтобы имена не повторялись
                    bool flagUniqueColumnName = false;
                    while (flagUniqueColumnName == false)
                    {
                        for (int j = 0; j < _TableDataBuff.Tables[tableName].Columns.Count; j++)
                        {
                            if (_TableDataBuff.Tables[tableName].Columns[j].ColumnName == $"NewColumn{_indexMyColumn}")
                            {
                                flagUniqueColumnName = false;
                                _indexMyColumn++;
                            }
                            else
                            {
                                flagUniqueColumnName = true;
                            }
                        }

                    }
                    _TableDataBuff.Tables[tableName].Columns[i - 1].ColumnName = $"NewColumn{_indexMyColumn}";
                }

                _TableDataBuff.Tables[tableName].Columns[i].ColumnName = bufString;
            }

            _indexMyColumn++;

            //заполним ячейки "созданного" столбца пробелами
            for (int i = 0; i < _TableDataBuff.Tables[tableName].Rows.Count; i++)
            {
                _TableDataBuff.Tables[tableName].Rows[i][ColNumb + bias] = "";
            }

            cfEditPaintScript(0, ColNumb + bias, ref _ColumnsPaint);
        }


        public void cfInsertCell(int RowNumb, int ColNumb)
        {
            int ColCount = _TableDataBuff.Tables[tableName].Columns.Count; //определим количество столбцов

            for (int i = ColCount - 1; i > ColNumb; i--)
            {
                _TableDataBuff.Tables[tableName].Rows[RowNumb][i] = _TableDataBuff.Tables[tableName].Rows[RowNumb][i - 1]; //индексы ячейки [строка, столбец]
            }
            _TableDataBuff.Tables[tableName].Rows[RowNumb][ColNumb] = String.Empty;
        }

        public void cfDeleteCell(int RowNumb, int ColNumb)
        {
            int ColCount = _TableDataBuff.Tables[tableName].Columns.Count; //определим количество столбцов

            for (int i = ColNumb; i < ColCount-1; i++)
            {
                _TableDataBuff.Tables[tableName].Rows[RowNumb][i] = _TableDataBuff.Tables[tableName].Rows[RowNumb][i + 1]; //индексы ячейки [строка, столбец]
            }
            _TableDataBuff.Tables[tableName].Rows[RowNumb][ColCount-1] = String.Empty;
        }

        public void cbHighlightCol(int ColNumb)
        {
            cfEditPaintScript(2, ColNumb, ref _ColumnsPaint);
        }

        public void cRunfEditPaintScript(int ColNumb)
        {
            cfEditPaintScript(1, ColNumb, ref _ColumnsPaint);
        }

        public MyBigClass CloneF()
        {
            return new MyBigClass()
            {
                _TableDataBuff = _TableDataBuff.Copy(),
                _RowPaint = this._RowPaint,
                _ColumnsPaint = this._ColumnsPaint,
                _indexMyColumn = this._indexMyColumn,
            };
        }
    }
}
