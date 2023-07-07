using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

/*
 Для ускорения сохранения больших файлов попробовать виртуальный режим.
 */

namespace CsvEditor
{
    public partial class Form1 : Form
    {
        string progName = "CsvEditor";
        string fileName = "";
        string tableName = "SuperTable";
        string FileOpenPath;

        string delimEqual = "=";
        string nextLine = "\r\n";

        string error1 = "Таблица CSV не загружена.";
        string error2 = "Таблица CSV не загружена или введено некорректное имя.";
        string error3 = "Ошибка сохранения файла.";
        string error4 = "Ошибка загрузки файла.";
        string error5 = "Ошибка создания cfg файла.";
        string warning1 = "При открытии CSV возникли ошибки:";
        string warning2 = "- число заголовков CSV таблицы больше числа ячеек таблицы";
        string warning3 = "- число заголовков CSV таблицы меньше числа ячеек таблицы";
        string warning4 = "- CSV таблица содержит разное число ячеек в разных строчках";
        string warning5 = "Убедитесь, что в настройках корректно установлены разделитель и кодировка.";
        string warning6 = "- к повторяющимся именам столбцов символ \"*\"";

        string attention1Quest = "Все несохранённые данные будут потеряны. Продолжить?";
        string attention1Title = "Внимание!";
        string mesLastRow = "Вы не можете удалить последнюю строчку.";
        string mesLastCol = "Вы не можете удалить последний столбец.";
        private readonly string CSV_CONFIG_PATH = $"{Application.StartupPath}\\csveditor.cfg";

        /*
        string error1 = "CSV table is not loaded.";
        string error2 = "CSV table is not loaded or you entered an incorrect name.";
        string error3 = "File saving error.";
        string error4 = "File load error.";
        string error5 = "cfg-file creating error.";
        string attention1Quest = "All unsaved data will be lost. Continue?";
        string attention1Title = "Attention!";
        string mesLastRow = "You cannot delete the last row.";
        string mesLastCol = "You cannot delete the last column.";
        */

        MyBigClass myBigClass = new MyBigClass();
        MyBigClass[] myHistory = new MyBigClass[20];
        int indexHistory = 0;
        int maxForwardIndex = 0;
        int currentHystoryIndex = 0;

        bool freeAction = false; //переменная для отслеживания выполнения функций по нажатию кнопок  
                                 //для отключения fSaveHistory(), отслеживающего изменения содержимого ячеек,
                                 //при добавлении/удалении строк и ячеек
        public Form1(string[] args)
        {
            InitializeComponent();
            if (args.Length > 0) FileOpenPath = args[0];
            else FileOpenPath = "";
        }

        //-------------------------------Menu----------------------------------------------------------------------->

        void fCreatIniFile(string FilePath)
        {
            string contentIni = 
                "delimiter" + delimEqual + CsvPreferences.delimiter + nextLine +
                "delimiter_write" + delimEqual + CsvPreferences.delimiter_write + nextLine +
                "read_encoding" + delimEqual + CsvPreferences.read_encoding + nextLine +
                "write_encoding" + delimEqual + CsvPreferences.write_encoding;

            try
            {
                File.WriteAllText(FilePath, contentIni, Encoding.GetEncoding(1251)); //Encoding.Default
            }
            catch (Exception)
            {
                MessageBox.Show(error5);
            }
        }
    

        void fStartNewHistory()
        {
            indexHistory = 0;
            maxForwardIndex = 0;
            currentHystoryIndex = 0;
            myHistory[indexHistory] = myBigClass.CloneF();
            button11.Text = ""; //forward
            button12.Text = ""; //back
        }


        void fPaintRowColumn()
        {
            for (int i = 0; i < myBigClass.TableDataBuff.Tables[tableName].Rows.Count; i++)
            {
                for (int j = 0; j < myBigClass.TableDataBuff.Tables[tableName].Columns.Count; j++)
                {
                    doubleBufferedDataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                }
            }


            if (myBigClass.RowPaint.Length !=0 )
            {
                for (int i =0 ; i< myBigClass.RowPaint.Length; i++)
                {
                    for (int j = 0; j < myBigClass.TableDataBuff.Tables[tableName].Columns.Count; j++)
                    {
                        doubleBufferedDataGridView1.Rows[myBigClass.RowPaint[i]].Cells[j].Style.BackColor = Color.LightBlue;
                    }
                }
            }

            if (myBigClass.ColumnsPaint.Length != 0)
            {
                for (int i = 0; i < myBigClass.ColumnsPaint.Length; i++)
                {
                    for (int j = 0; j < myBigClass.TableDataBuff.Tables[tableName].Rows.Count; j++)
                    {
                        doubleBufferedDataGridView1.Rows[j].Cells[myBigClass.ColumnsPaint[i]].Style.BackColor = Color.LightGreen;
                    }
                }
            }

            //проверим есть ли текст для поиска и подсветим нужные ячейки
            string textForFind = textBox1.Text;
            if (textForFind != "" && textForFind != null)
            {
                fFindTextChanged(textForFind);
            }
        }
        
        
        void fButtonDisable()
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            hlght_row.Enabled = false;
            hlght_col.Enabled = false;
            button9.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;
            textBox1.Enabled = false;
            toolStripMenuItem3.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            closeToolStripMenuItem.Enabled = false;
        }

        void fButtonEnable()
        {
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            hlght_row.Enabled = true;
            hlght_col.Enabled = true;
            button9.Enabled = true;
            button10.Enabled = true;
            button11.Enabled = true;
            button12.Enabled = true;
            textBox1.Enabled = true;
            toolStripMenuItem3.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
            closeToolStripMenuItem.Enabled = true;
        }


        void fSetWidthRowHeader()
        {
            //всё это шаманство для того, чтобы не отображалась стрелочка в заголовках строк
            doubleBufferedDataGridView1.RowHeadersWidth = 50;
            Padding newPadding = new Padding(1, 3, 3, 3);
            doubleBufferedDataGridView1.RowHeadersDefaultCellStyle.Padding = newPadding;
        }

        void fNumbRows()
        {
            for (int i = 1; doubleBufferedDataGridView1.Rows.Count >= i; i++)
            {
                doubleBufferedDataGridView1.Rows[i - 1].HeaderCell.Value = string.Format((i).ToString());
            }
        }

        void fResetTable()
        {
            //при загрузке нового CSV счётчик вновь добавленных столбцов сбросим в 0, обнулим индикаторы ошибок
            //проверим если таблица с таким именем уже открыта, то удалим её
            //сбросим состояния загрузки объекта по умолчанию
            myBigClass.ResetState();
            if (myBigClass.TableDataBuff.Tables.Contains(tableName))
                myBigClass.TableDataBuff.Tables.Remove(tableName);
            myBigClass.TableDataBuff.Tables.Add(tableName);

            fileName = "";
        }

        void fExitProg()
        {
            Application.Exit();
        }

        void fSortDisable()
        {
            for (int i = 0; i < myBigClass.TableDataBuff.Tables[tableName].Columns.Count; i++)
            {
                this.doubleBufferedDataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void fSaveAs()
        {
            toolStripMenuItem3.Enabled = true;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "csv files (*.csv)|*.csv";
            sfd.ShowDialog();
            if (sfd.FileName != "")
            {
                fSaveMyFile(sfd.FileName);
            }
        }

        void fSaveMyFile(string FilePath)
        {
            Task.Run(() =>
            {
                //функционал для проверки скорости сохранения файла
                //Stopwatch timeLooker = new Stopwatch();
                //timeLooker.Start();
                freeAction = false;

                int i, j;
                int RowCount, ColCount;

                RowCount = this.doubleBufferedDataGridView1.Rows.Count;
                ColCount = this.doubleBufferedDataGridView1.Columns.Count;
                string NewDataCSV = "";

                Invoke(new Action(() =>
                {
                    toolStripMenuItem1.Enabled = false;
                    toolStripMenuItem4.Enabled = false;
                    toolStripMenuItem6.Enabled = false;

                    doubleBufferedDataGridView1.ReadOnly = true;
                }));

                //читаем заголовки
                for (i = 0; i < ColCount; i++)
                {
                    NewDataCSV = NewDataCSV + this.doubleBufferedDataGridView1.Columns[i].HeaderText;
                    if (i == ColCount - 1)
                    {
                        NewDataCSV = NewDataCSV + nextLine;
                    }
                    else
                    {
                        NewDataCSV = NewDataCSV + CsvPreferences.delimiter_write;
                    }
                }

                int MaxProgressBar = RowCount * ColCount;
                string[] SavedStringOfTable = new string[RowCount];

                Invoke(new Action(() =>
                {
                    fButtonDisable();
                    progressBar1.Maximum = MaxProgressBar;
                    progressBar1.Value = 0;
                }));

                //читаем содержимое по ячейкам [ряд, строчка]
                for (i = 0; i < RowCount; i++)
               {
                    for (j = 0; j < ColCount; j++)
                    {
                        //сохраним каждую строчку таблицы в элемент массива строк 
                        SavedStringOfTable[i] = SavedStringOfTable[i] + doubleBufferedDataGridView1[j, i].Value;
                        if (j == ColCount - 1)
                        {
                            SavedStringOfTable[i] = SavedStringOfTable[i] + nextLine;
                        }
                        else
                        {
                            SavedStringOfTable[i] = SavedStringOfTable[i] + CsvPreferences.delimiter_write;
                        }
                        this.Invoke(new Action(() =>
                        {
                            progressBar1.Value++;
                        }));
                    }
                }

                Invoke(new Action(() =>
                {
                    fButtonDisable();
                    progressBar1.Maximum = RowCount;
                    progressBar1.Value = 0;
                }));

                //из массива строк все данные сохраним в одну строковую переменную
                for (int k=0; k< RowCount; k++)
                {
                    NewDataCSV = NewDataCSV + SavedStringOfTable[k];

                    this.Invoke(new Action(() =>
                    {
                        progressBar1.Value++;
                    }));
                }

                try
                {
                    if (FilePath != "")
                        fileName = FilePath;

                    File.WriteAllText(fileName, NewDataCSV, Encoding.GetEncoding(CsvPreferences.write_encoding)); //Encoding.Default

                    this.Invoke(new Action(() =>
                    {
                        this.Text = progName + " - " + Path.GetFileName(fileName);
                    }));
                }
                catch (Exception)
                {
                    MessageBox.Show(error3);
                }
                Invoke(new Action(() =>
                {
                    progressBar1.Value = 0;
                    fButtonEnable();
                    toolStripMenuItem1.Enabled = true;
                    toolStripMenuItem4.Enabled = true;
                    toolStripMenuItem6.Enabled = true;
                    doubleBufferedDataGridView1.ReadOnly = false;
                }));

                freeAction = true;
                //Функционал для проверки скорости сохранения файла
                //timeLooker.Stop();
                //string OutTime = Convert.ToString(timeLooker.Elapsed.TotalSeconds);
                //MessageBox.Show(OutTime);
            });
        }


        private void fLoadCsvTable()
        {
            freeAction = false;
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "csv files (*.csv)|*.csv";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    fResetTable();

                    fButtonEnable();
                    fileName = ofd.FileName;

                    string[] StrokiRows = File.ReadAllLines(fileName, System.Text.Encoding.GetEncoding(CsvPreferences.read_encoding)); //массив строк, прочитанный из файла

                    this.Text = progName + " - " + Path.GetFileName(fileName);

                    myBigClass.cfLoadCsvTable(StrokiRows);

                    this.doubleBufferedDataGridView1.DataSource = myBigClass.TableDataBuff.Tables[tableName].DefaultView;
                    
                    //отключим сортировку столбцов
                    fSortDisable();
                    fSetWidthRowHeader();
                    fNumbRows();
                    fStartNewHistory();
                }
            }
            catch (Exception)
            {
                MessageBox.Show(error4);
            }

            //Анализ статуса загрузки и подготовка сообщения Warning
            string warningMessages = warning1 + nextLine;

            if (myBigClass.LoadState!=0) 
            {
                switch (myBigClass.LoadState)
                {
                    case 1: warningMessages = warningMessages + warning3 + nextLine;
                        break;
                    case 2: warningMessages = warningMessages + warning2 + nextLine;
                        break;
                    default:
                        break;
                }
            }

            if (myBigClass.LoadStateMinMax == 1) warningMessages = warningMessages + warning4 + nextLine;

            if (myBigClass.LoadStateColumnsName == 1) warningMessages = warningMessages + warning6 + nextLine;

            if (myBigClass.LoadState != 0 || myBigClass.LoadStateMinMax != 0 || myBigClass.LoadStateColumnsName != 0) 
            {
                warningMessages = warningMessages + warning5;
                MessageBox.Show(warningMessages);
            }

            freeAction = true;
        }

        private void fReplace()
        {
            freeAction = false;
            try
            {
                fButtonDisable();
                int ColNumb = doubleBufferedDataGridView1.CurrentCell.ColumnIndex; //получим индекс столбца текущей ячейки
                int RowNumb = doubleBufferedDataGridView1.CurrentCell.RowIndex; //номер строки в которую вернем курсор после добавления столбца

                int saveRow = 0;
                int saveCol = 0;
                if (doubleBufferedDataGridView1.Rows.Count > 0 && doubleBufferedDataGridView1.FirstDisplayedCell != null)
                    saveRow = doubleBufferedDataGridView1.FirstDisplayedCell.RowIndex;

                if (doubleBufferedDataGridView1.Columns.Count > 0 && doubleBufferedDataGridView1.FirstDisplayedCell != null)
                    saveCol = doubleBufferedDataGridView1.FirstDisplayedCell.ColumnIndex;

                //на время добавления столбца отключим отображение TableDataBuff от dataGridView, чтобы не тормозило
                ArrayList ZeroData = new ArrayList();
                doubleBufferedDataGridView1.DataSource = ZeroData;

                myBigClass.cfReplace();

                //после добавления столбца в TableDataBuff подключим его
                this.doubleBufferedDataGridView1.DataSource = myBigClass.TableDataBuff.Tables[tableName].DefaultView;
                
                fButtonEnable();
                fSortDisable();

                doubleBufferedDataGridView1.Rows[RowNumb].Cells[0].Selected = false; //убрать выделение первой ячейки строки
                doubleBufferedDataGridView1.Rows[RowNumb].Cells[ColNumb].Selected = true; //вернем выделение ячейки на место

                fNumbRows();
                fPaintRowColumn();
                doubleBufferedDataGridView1.FirstDisplayedScrollingRowIndex = saveRow;
                doubleBufferedDataGridView1.FirstDisplayedScrollingColumnIndex = saveCol;

                fSaveHistory();
            }
            catch (Exception)
            {
                MessageBox.Show(error1);
            }

            freeAction = true;
            
            StaticData.replaceMarker = false;
        }

        private void fNew()
        {
            fButtonEnable();
            fResetTable();

            myBigClass.cmNew();
            //отключим сортировку столбцов
            this.doubleBufferedDataGridView1.DataSource = myBigClass.TableDataBuff.Tables[tableName].DefaultView;
            
            fSortDisable();
            fSetWidthRowHeader();
            fNumbRows();
            fStartNewHistory();
        }

        private void mNew(object sender, EventArgs e)
        {
            freeAction = false;
            if (myBigClass.TableDataBuff.Tables.Contains(tableName)) {

            DialogResult dialogResult = MessageBox.Show(attention1Quest, attention1Title, MessageBoxButtons.YesNo);
                if (dialogResult != DialogResult.No)
                {
                    fNew();
                }
            } else
            {
                fNew();
            }
            freeAction = true;
        }

        private void mLoadCsvTable(object sender, EventArgs e)
        {
            fLoadCsvTable();
        }


        private void mSave(object sender, EventArgs e)
        {
            if (fileName != "")
                fSaveMyFile("");
            else fSaveAs();
        }


        private void mSaveAs(object sender, EventArgs e)
        {
            fSaveAs();
        }


        private void mCloseFile(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(attention1Quest, attention1Title, MessageBoxButtons.YesNo); //2 - титл 1 - месседж
            if (dialogResult == DialogResult.Yes)
            {
                fResetTable();
                this.Text = progName;
                ArrayList ZeroData = new ArrayList();
                doubleBufferedDataGridView1.DataSource = ZeroData;
                fButtonDisable();
                fStartNewHistory();
            }
        }


        private void mExitProg(object sender, EventArgs e)
        {
            fExitProg();
        }

        private void mReplace(object sender, EventArgs e)
        {
            menuReplace menuReplaceAll = new menuReplace();
            menuReplaceAll.StartPosition = FormStartPosition.Manual; //разрешаем ручной ввод координат окна
            menuReplaceAll.Location = MousePosition; //открываем окно рядом с мышкой
            menuReplaceAll.ShowDialog();

            if (StaticData.replaceMarker) fReplace();
        }

        private void mFileSeparator(object sender, EventArgs e)
        {
            menuSeparator menuSep = new menuSeparator();
            menuSep.StartPosition = FormStartPosition.Manual; //разрешаем ручной ввод координат окна
            menuSep.Location = MousePosition; //открываем окно рядом с мышкой
            menuSep.Show();
        }

        private void mEncoding(object sender, EventArgs e)
        {
            menuEncoding menuEnc = new menuEncoding();
            menuEnc.StartPosition = FormStartPosition.Manual; //разрешаем ручной ввод координат окна
            menuEnc.Location = MousePosition; //открываем окно рядом с мышкой
            menuEnc.Show();
        }

        private void mAbout(object sender, EventArgs e)
        {
            aboutForm aboutProgForm = new aboutForm();
            aboutProgForm.StartPosition = FormStartPosition.Manual; //разрешаем ручной ввод координат окна
            aboutProgForm.Location = MousePosition; //открываем окно рядом с мышкой
            aboutProgForm.Show(); 
        }

        //-------------------------------Rows_Edit----------------------------------------------------------------------->
        void fInsertRow(bool dupl)
        {
            freeAction = false;
            try
            {
                fButtonDisable();
                int RowNumb = doubleBufferedDataGridView1.CurrentRow.Index; //определяем индекс текущей строки

                myBigClass.cfInsertRow(dupl, RowNumb);

                fButtonEnable();
                fNumbRows();
                fPaintRowColumn();
                fSaveHistory();
            }
            catch (Exception)
            {
                MessageBox.Show(error1);
            }
            freeAction = true;
        }


        private void bInsertAbove_Click(object sender, EventArgs e)
        {
            fInsertRow(false);
        }


        private void bDelete_Click(object sender, EventArgs e)
        {
            freeAction = false;
            if (doubleBufferedDataGridView1.Rows.Count > 1)
            { 
            
            try
            {
                int cellNumb = doubleBufferedDataGridView1.CurrentRow.Index;

                myBigClass.cbDelete_Click(cellNumb);

                fNumbRows();

                fPaintRowColumn();

                fSaveHistory();
                }
            catch (Exception)
            {
                MessageBox.Show(error1);
            }
            } else MessageBox.Show(mesLastRow);
            freeAction = true;
        }


        private void bDuplToEndClick(object sender, EventArgs e)
        {
            freeAction = false;
            try
            {
                int RowNumb = doubleBufferedDataGridView1.CurrentRow.Index; //определяем индекс текущей строки

                myBigClass.cbDuplToEndClick(RowNumb);

                fNumbRows();

                fPaintRowColumn();

                fSaveHistory();
            }
            catch (Exception)
            {
                MessageBox.Show(error1);
            }
            freeAction = true;
        }


        private void bInsertDuplClick(object sender, EventArgs e)
        {
            fInsertRow(true);
        }

        private void bHighlightRow(object sender, EventArgs e)
        {
            freeAction = false;

            int RowNumb = doubleBufferedDataGridView1.CurrentRow.Index; //определяем индекс текущей строки

            myBigClass.cbHighlightRow(RowNumb);

            fPaintRowColumn();

            fSaveHistory();

            freeAction = true;
        }


        //-------------------------------Columns_Edit----------------------------------------------------------------------->
        void fInsertColumn(int bias) //смещение bias =0 для столбца слева и 1 для столбца справа
        {
            freeAction = false;
            try
            {
                fButtonDisable();
                int ColNumb = doubleBufferedDataGridView1.CurrentCell.ColumnIndex; //получим индекс столбца текущей ячейки
                int RowNumb = doubleBufferedDataGridView1.CurrentCell.RowIndex; //номер строки в которую вернем курсор после добавления столбца

                int saveRow = 0;
                int saveCol = 0;
                if (doubleBufferedDataGridView1.Rows.Count > 0 && doubleBufferedDataGridView1.FirstDisplayedCell != null)
                    saveRow = doubleBufferedDataGridView1.FirstDisplayedCell.RowIndex;

                if (doubleBufferedDataGridView1.Columns.Count > 0 && doubleBufferedDataGridView1.FirstDisplayedCell != null)
                    saveCol = doubleBufferedDataGridView1.FirstDisplayedCell.ColumnIndex;

                //на время добавления столбца отключим отображение TableDataBuff от dataGridView, чтобы не тормозило
                ArrayList ZeroData = new ArrayList();
                doubleBufferedDataGridView1.DataSource = ZeroData;

                myBigClass.cfInsertColumn(bias, ColNumb);

                //после добавления столбца в TableDataBuff подключим его
                this.doubleBufferedDataGridView1.DataSource = myBigClass.TableDataBuff.Tables[tableName].DefaultView;
                

                fButtonEnable();
                fSortDisable();

                doubleBufferedDataGridView1.Rows[RowNumb].Cells[0].Selected = false; //убрать выделение первой ячейки строки
                doubleBufferedDataGridView1.Rows[RowNumb].Cells[ColNumb].Selected = true; //вернем выделение ячейки на место

                fNumbRows();
                fPaintRowColumn();
                doubleBufferedDataGridView1.FirstDisplayedScrollingRowIndex = saveRow;
                doubleBufferedDataGridView1.FirstDisplayedScrollingColumnIndex = saveCol;

                fSaveHistory();
            }
            catch (Exception)
            {
                MessageBox.Show(error1);
            }

            freeAction = true;
        }


        private void bInsertColLeft(object sender, EventArgs e)
        {
            fInsertColumn(0);
        }


        private void bRenameColumn(object sender, EventArgs e)
        {
            freeAction = false;

            try
            {
                int ColNumb = doubleBufferedDataGridView1.CurrentCell.ColumnIndex; //получим индекс столбца текущей ячейки

                Form2 NewNameDialog = new Form2();
                NewNameDialog.StartPosition = FormStartPosition.Manual; //разрешаем ручной ввод координат окна
                NewNameDialog.Location = MousePosition; //открываем окно рядом с мышкой
                NewNameDialog.ShowDialog(); //открываем окно как диалоговое, при закрытии окна обновляем данные в форме 1

                myBigClass.TableDataBuff.Tables[tableName].Columns[ColNumb].ColumnName = StaticData.DataBuffer; //присваиваем новое имя выбранному столбцу

                fPaintRowColumn();
                fNumbRows();
                fSaveHistory();
            }
            catch (Exception)
            {
                MessageBox.Show(error2);
            }
            freeAction = true;
        }


        private void bDeleteCol_Click(object sender, EventArgs e)
        {
            freeAction = false;

            if (doubleBufferedDataGridView1.Columns.Count > 1)
            {
                try
                {
                    fButtonDisable();
                    int RowNumb = doubleBufferedDataGridView1.CurrentCell.RowIndex; //номер строки в которую вернем курсор после добавления столбца
                    int ColNumb = doubleBufferedDataGridView1.CurrentCell.ColumnIndex; //получим индекс столбца текущей ячейки

                    int saveRow = 0;
                    int saveCol = 0;
                    if (doubleBufferedDataGridView1.Rows.Count > 0 && doubleBufferedDataGridView1.FirstDisplayedCell != null)
                        saveRow = doubleBufferedDataGridView1.FirstDisplayedCell.RowIndex;

                    if (doubleBufferedDataGridView1.Columns.Count > 0 && doubleBufferedDataGridView1.FirstDisplayedCell != null)
                        saveCol = doubleBufferedDataGridView1.FirstDisplayedCell.ColumnIndex;

                    myBigClass.TableDataBuff.Tables[tableName].Columns.RemoveAt(ColNumb);

                    if (ColNumb > 5)
                    doubleBufferedDataGridView1.FirstDisplayedScrollingColumnIndex = ColNumb - 1;
                    doubleBufferedDataGridView1.Rows[RowNumb].Cells[0].Selected = false; //убрать выделение первой ячейки строки

                    if (doubleBufferedDataGridView1.Columns.Count > ColNumb)
                    {
                        doubleBufferedDataGridView1.Rows[RowNumb].Cells[ColNumb].Selected = true; //вернем выделение ячейки на место
                    }
                    else 
                    {
                        doubleBufferedDataGridView1.Rows[RowNumb].Cells[ColNumb - 1].Selected = true; //если был оследниц столбец, то выделим ячейку на 1 столбец левее
                    }
                    fButtonEnable();

                    myBigClass.cRunfEditPaintScript(ColNumb);
                    
                    fNumbRows();
                    fPaintRowColumn();
                    doubleBufferedDataGridView1.FirstDisplayedScrollingRowIndex = saveRow;
                    doubleBufferedDataGridView1.FirstDisplayedScrollingColumnIndex = saveCol;
                    fSaveHistory();
                }
                catch (Exception)
                {
                    MessageBox.Show(error1);
                }
            } else MessageBox.Show(mesLastCol);

            freeAction = true;
        }


        private void bInsertColRight(object sender, EventArgs e)
        {
            fInsertColumn(1);
        }

        private void bHighlightCol(object sender, EventArgs e)
        {
            freeAction = false;
            int ColNumb = doubleBufferedDataGridView1.CurrentCell.ColumnIndex; //определяем индекс текущей строки
            myBigClass.cbHighlightCol(ColNumb);
            fPaintRowColumn();
            fSaveHistory();
            freeAction = true;
        }

        //---------------------FindText---------------------------------------------------------------------->

        private void bFindNext(object sender, EventArgs e)
        {
            int RowNumb = doubleBufferedDataGridView1.CurrentRow.Index; //определяем индекс текущей строки
            int ColNumb = doubleBufferedDataGridView1.CurrentCell.ColumnIndex; //получим индекс столбца текущей ячейки
            int RowCount = doubleBufferedDataGridView1.Rows.Count;
            int ColCount = doubleBufferedDataGridView1.Columns.Count;
            bool tRigger = false;
            doubleBufferedDataGridView1.ClearSelection();
            ColNumb++;
            for (; RowNumb < RowCount && tRigger == false; RowNumb++)
            { 
                for (; ColNumb < ColCount && tRigger == false; ColNumb++)
                {
                    if (doubleBufferedDataGridView1.Rows[RowNumb].Cells[ColNumb].Value != null )
                    {
                        if (doubleBufferedDataGridView1.Rows[RowNumb].Cells[ColNumb].Value.ToString().Contains(textBox1.Text))
                        {
                            doubleBufferedDataGridView1.Rows[RowNumb].Cells[ColNumb].Selected = true;
                            if (ColNumb >= 2) doubleBufferedDataGridView1.FirstDisplayedScrollingColumnIndex = ColNumb - 2;
                            if (RowNumb >= 2) doubleBufferedDataGridView1.FirstDisplayedScrollingRowIndex = RowNumb - 2;
                            this.doubleBufferedDataGridView1.CurrentCell = this.doubleBufferedDataGridView1[ColNumb, RowNumb];
                            tRigger = true;
                        }
                    }
                }
                ColNumb = 0;
            }
        }

        private void bFindPrev(object sender, EventArgs e)
        {
            int RowNumb = doubleBufferedDataGridView1.CurrentRow.Index; //определяем индекс текущей строки
            int ColNumb = doubleBufferedDataGridView1.CurrentCell.ColumnIndex; //получим индекс столбца текущей ячейки
            int ColCount = doubleBufferedDataGridView1.Columns.Count;
            bool tRigger = false;
            doubleBufferedDataGridView1.ClearSelection();
            ColNumb--;
            for (; RowNumb >=0 && tRigger == false; RowNumb--)
            {
                for (; ColNumb >= 0 && tRigger == false; ColNumb--)
                {
                    if (doubleBufferedDataGridView1.Rows[RowNumb].Cells[ColNumb].Value != null)
                    {
                        if (doubleBufferedDataGridView1.Rows[RowNumb].Cells[ColNumb].Value.ToString().Contains(textBox1.Text))
                        {
                            doubleBufferedDataGridView1.Rows[RowNumb].Cells[ColNumb].Selected = true;
                            if (ColNumb >= 2) doubleBufferedDataGridView1.FirstDisplayedScrollingColumnIndex = ColNumb - 2;
                            if (RowNumb >= 2) doubleBufferedDataGridView1.FirstDisplayedScrollingRowIndex = RowNumb - 2;
                            this.doubleBufferedDataGridView1.CurrentCell = this.doubleBufferedDataGridView1[ColNumb, RowNumb];
                            tRigger = true;
                        }
                    }
                }
                ColNumb = ColCount-1;
            }
        }

        //----------------------------------------------------------------------------------------------------------------------->
        private void fSaveHistory()
        {
            if (indexHistory < 19) indexHistory++;
            else
            {
                for (int i = 0; i < 10; i++) myHistory[i] = myHistory[10 + i].CloneF();
                indexHistory = 10;
            }
            myHistory[indexHistory] = myBigClass.CloneF();
            if (currentHystoryIndex != maxForwardIndex)
            {
                currentHystoryIndex = 1;
                maxForwardIndex = 1;
            }
            else
            {
                if (maxForwardIndex < 9) maxForwardIndex++;
                if (currentHystoryIndex < 9) currentHystoryIndex++;
            }
            button11.Text = ""; //forward
            if (maxForwardIndex != 0) button12.Text = Convert.ToString(maxForwardIndex); //back
            else button12.Text = "";
        }


        private void bBackHistory(object sender, EventArgs e)
        {
            fBackHistory();
        }

        private void fBackHistory()
        {
            freeAction = false;
            if (currentHystoryIndex > 0)
            {
                int saveRow = 0;
                int saveCol = 0;
                if (doubleBufferedDataGridView1.Rows.Count > 0 && doubleBufferedDataGridView1.FirstDisplayedCell != null)
                    saveRow = doubleBufferedDataGridView1.FirstDisplayedCell.RowIndex;

                if (doubleBufferedDataGridView1.Columns.Count > 0 && doubleBufferedDataGridView1.FirstDisplayedCell != null)
                    saveCol = doubleBufferedDataGridView1.FirstDisplayedCell.ColumnIndex;
                //на время добавления столбца отключим отображение TableDataBuff от dataGridView, чтобы не тормозило
                ArrayList ZeroData = new ArrayList();
                doubleBufferedDataGridView1.DataSource = ZeroData;

                indexHistory--;
                myBigClass = myHistory[indexHistory].CloneF();
                currentHystoryIndex--;

                this.doubleBufferedDataGridView1.DataSource = myBigClass.TableDataBuff.Tables[tableName].DefaultView;

                fSortDisable();
                fNumbRows();
                fPaintRowColumn();

                doubleBufferedDataGridView1.FirstDisplayedScrollingRowIndex = saveRow;
                doubleBufferedDataGridView1.FirstDisplayedScrollingColumnIndex = saveCol;
                button11.Text = Convert.ToString(maxForwardIndex - currentHystoryIndex); //forward
                button12.Text = Convert.ToString(currentHystoryIndex); //back
            }
            freeAction = true;
        }

        private void bForwardHistory(object sender, EventArgs e)
        {
            fForwardHistory();
        }

        private void fForwardHistory()
        {
            freeAction = false;
            if ((currentHystoryIndex < 9) && (currentHystoryIndex < maxForwardIndex))
            {
                int saveRow = 0;
                int saveCol = 0;
                if (doubleBufferedDataGridView1.Rows.Count > 0 && doubleBufferedDataGridView1.FirstDisplayedCell != null)
                    saveRow = doubleBufferedDataGridView1.FirstDisplayedCell.RowIndex;

                if (doubleBufferedDataGridView1.Columns.Count > 0 && doubleBufferedDataGridView1.FirstDisplayedCell != null)
                    saveCol = doubleBufferedDataGridView1.FirstDisplayedCell.ColumnIndex;

                ArrayList ZeroData = new ArrayList();
                doubleBufferedDataGridView1.DataSource = ZeroData;

                indexHistory++;
                myBigClass = myHistory[indexHistory].CloneF();
                currentHystoryIndex++;

                this.doubleBufferedDataGridView1.DataSource = myBigClass.TableDataBuff.Tables[tableName].DefaultView;

                fSortDisable();
                fNumbRows();
                fPaintRowColumn();

                doubleBufferedDataGridView1.FirstDisplayedScrollingRowIndex = saveRow;
                doubleBufferedDataGridView1.FirstDisplayedScrollingColumnIndex = saveCol;
                button11.Text = Convert.ToString(maxForwardIndex - currentHystoryIndex); //forward
                button12.Text = Convert.ToString(currentHystoryIndex); //back
            }
            freeAction = true;
        }
        //----------------------------------------------------------------------------------------------------------------------->

        private void eLoad_Form(object sender, EventArgs e)
        {
            fButtonDisable();
            splitContainer1.SplitterDistance = 60;
            button1.Text = "Вст. выше";
            button2.Text = "Дубл.ниже";
            button3.Text = "Вст.слева";
            button4.Text = "Вст.справ.";
            button5.Text = "Дубл.вкон.";
            button6.Text = "Удалить";
            button7.Text = "Переимен.";
            button8.Text = "Удалить";
            hlght_row.Text = "Подсв.";
            hlght_col.Text = "Подсв.";
            button9.Text = "<--Найти";
            button10.Text = "Найти-->";
            toolStripMenuItem1.Text = "Файл"; //- File
            newToolStripMenuItem.Text = "Новый"; //- New
            toolStripMenuItem2.Text = "Открыть"; //- Open
            toolStripMenuItem3.Text = "Сохранить"; //- Save
            saveAsToolStripMenuItem.Text = "Сохранить как"; //- Save As
            closeToolStripMenuItem.Text = "Закрыть"; //- Close
            exitToolStripMenuItem.Text = "Выход"; //- Exit
            toolStripMenuItem6.Text = "Настройки"; //- Options
            fileSeparatorToolStripMenuItem.Text = "Разделитель"; // - Field Separator
            toolStripMenuItem7.Text = "Кодировка"; // - Encoding
            toolStripMenuItem4.Text = "Помощь"; //- Help
            toolStripMenuItem5.Text = "О программе"; //- About
            label1.Text = "Столбцы";
            label2.Text = "Строки";

            toolStripMenuItem3.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            closeToolStripMenuItem.Enabled = false;

            KeyPreview = true; //разрешаем форме перехватывать нажатия кнопок

            StaticData.CSV_CONFIG_PATH = CSV_CONFIG_PATH;

            if (File.Exists(CSV_CONFIG_PATH))
            {
                try
                {
                    string iniData = File.ReadAllText(CSV_CONFIG_PATH, Encoding.GetEncoding(1251));

                    string[] iniDataSplit = iniData.Split(nextLine.ToCharArray()); //при такой разбивке строки на подстроки появляются пустые строки после каждого перевода каретки

                    foreach (string oneParamString in iniDataSplit)
                    {
                        if (oneParamString != "") //костыль для отсеивания пустых строк
                        {
                            string[] KeyValue = oneParamString.Split(delimEqual.ToCharArray());

                            if (KeyValue[1] == "") KeyValue[1] = " "; //этот кростыль решает следующий вопрос - вместо пробела при десериализации
                                                                      //присваивается пустое значение (когда разделитель - пробел)

                            for (int i = 0; i < KeyValue.Length; i++) KeyValue[i] = KeyValue[i].Trim();

                            switch (KeyValue[0])
                            {
                                case "delimiter":
                                    CsvPreferences.delimiter = KeyValue[1];
                                    break;

                                case "delimiter_write":
                                    CsvPreferences.delimiter_write = KeyValue[1];
                                    break;

                                case "read_encoding":
                                    CsvPreferences.read_encoding = KeyValue[1];
                                    break;

                                case "write_encoding":
                                    CsvPreferences.write_encoding = KeyValue[1];
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    File.Delete(CSV_CONFIG_PATH);
                    fCreatIniFile(CSV_CONFIG_PATH);
                }
            }
            else
            {
                fCreatIniFile(CSV_CONFIG_PATH);
            }

            //-----------Загрузка файла по аргументу--------------
            try
            {
                if (FileOpenPath != null && FileOpenPath != "")
                {
                    freeAction = false;
                    fResetTable();

                    fButtonEnable();

                    string[] StrokiRows = File.ReadAllLines(FileOpenPath, System.Text.Encoding.GetEncoding(CsvPreferences.read_encoding)); //массив строк, прочитанный из файла

                    this.Text = progName + " - " + Path.GetFileName(FileOpenPath);

                    myBigClass.cfLoadCsvTable(StrokiRows);

                    this.doubleBufferedDataGridView1.DataSource = myBigClass.TableDataBuff.Tables[tableName].DefaultView;

                    fileName = FileOpenPath;
                    //отключим сортировку столбцов
                    fSortDisable();
                    fSetWidthRowHeader();
                    fNumbRows();
                    fStartNewHistory();
                    //Анализ статуса загрузки и подготовка сообщения Warning
                    string warningMessages = warning1 + nextLine;

                    if (myBigClass.LoadState != 0)
                    {
                        switch (myBigClass.LoadState)
                        {
                            case 1:
                                warningMessages = warningMessages + warning3 + nextLine;
                                break;
                            case 2:
                                warningMessages = warningMessages + warning2 + nextLine;
                                break;
                            default:
                                break;
                        }
                    }

                    if (myBigClass.LoadStateMinMax == 1) warningMessages = warningMessages + warning4 + nextLine;

                    if (myBigClass.LoadStateColumnsName == 1) warningMessages = warningMessages + warning6 + nextLine;

                    if (myBigClass.LoadState != 0 || myBigClass.LoadStateMinMax != 0 || myBigClass.LoadStateColumnsName != 0)
                    {
                        warningMessages = warningMessages + warning5;
                        MessageBox.Show(warningMessages);
                    }

                    freeAction = true;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(error4);
            }
            
        }

        private void eResize(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = 60;
        }

        
        private void eFormClising(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(attention1Quest, attention1Title, MessageBoxButtons.YesNo);
            if (dialogResult != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
        }

        private void eCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (freeAction)
            {
                Task.Run(() =>
                {
                    Invoke(new Action(() =>
                    {
                        fButtonDisable();
                        Thread.Sleep(120);  //при изменении состояния ячейки в fSaveHistory() сохраняется предыдущая версия myBigClass - изменения не успевают в нём сохраниться
                        fSaveHistory();     //для того чтобы изменения, внесённые в DataGridView, успели внестись в myBigClass используем задержку 0,12с
                        fButtonEnable();
                    }));
                });
            }
        }

        private void eKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z && e.Control)
            {
                fBackHistory();
            }

            if (e.KeyCode == Keys.Y && e.Control)
            {
                fForwardHistory();
            }
        }

        private void eFindTextChanged(object sender, EventArgs e)
        {
            string textForFind = textBox1.Text;
            if (textForFind != "" && textForFind != null)
            {
                fPaintRowColumn();
                fFindTextChanged(textForFind);
            }
            else fPaintRowColumn();
        }

        private void fFindTextChanged(string textForFind)
        {
            int RowCount = doubleBufferedDataGridView1.Rows.Count;
            int ColCount = doubleBufferedDataGridView1.Columns.Count;
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColCount; j++)
                {
                    if (doubleBufferedDataGridView1.Rows[i].Cells[j].Value != null)
                    {
                        if (doubleBufferedDataGridView1.Rows[i].Cells[j].Value.ToString().Contains(textBox1.Text))
                        {
                            doubleBufferedDataGridView1.Rows[i].Cells[j].Style.BackColor = System.Drawing.Color.LightGoldenrodYellow;
                        }
                    }
                }
            }
        }

        private void doubleBufferedDataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            StaticData.numberRow = e.RowIndex;
            StaticData.numberCol = e.ColumnIndex;
        }

        //------------контекстное меню-----------------------------------------------------
        private void findAndReplaceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (freeAction) //если файл не в процессе сохранения
            {
                menuReplace menuReplaceAll = new menuReplace();
                menuReplaceAll.StartPosition = FormStartPosition.Manual; //разрешаем ручной ввод координат окна
                menuReplaceAll.Location = MousePosition; //открываем окно рядом с мышкой
                menuReplaceAll.ShowDialog();

                if (StaticData.replaceMarker) fReplace();
            }
        }

        private void copyRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (freeAction) //если файл не в процессе сохранения
                myBigClass.NumberRowForCopy = StaticData.numberRow;
        }

        private void pastRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (freeAction) //если файл не в процессе сохранения
            {
                myBigClass.NumberRowForPast = StaticData.numberRow;
                myBigClass.cmPastRow();
                fSaveHistory();
                fPaintRowColumn();
            }
        }

        private void InsertCellMenuStrip(object sender, EventArgs e)
        {
            myBigClass.cfInsertCell(StaticData.numberRow, StaticData.numberCol);
            fSaveHistory();
            fPaintRowColumn();
        }

        private void DeleteCellMenuStrip(object sender, EventArgs e)
        {
            myBigClass.cfDeleteCell(StaticData.numberRow, StaticData.numberCol);
            fSaveHistory();
            fPaintRowColumn();
        }
    }
}
