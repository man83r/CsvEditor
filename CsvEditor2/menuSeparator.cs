using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CsvEditor
{
    public partial class menuSeparator : Form
    {
        private string nextLine = "\r\n";
        private string error5 = "Ошибка сохранения INI файла.";

        public menuSeparator()
        {
            InitializeComponent();
        }

        private void eLoadForm(object sender, EventArgs e)
        {
            //Read
            string delim = CsvPreferences.delimiter;
            if (delim == "") delim = " ";
            switch (delim)
            {
                case ";":
                    radioButton1.Checked = true;
                    break;

                case ":":
                    radioButton2.Checked = true;
                    break;

                case "|":
                    radioButton3.Checked = true;
                    break;

                case "\t":  //TAB
                    radioButton4.Checked = true;
                    break;

                case " ":  //SPACE
                    radioButton5.Checked = true;
                    break;

                case ",":
                    radioButton6.Checked = true;
                    break;

                default:
                    radioButton7.Checked = true;
                    textBox1.Text = delim;
                    break;
            }

            //Write
            delim = CsvPreferences.delimiter_write;
            if (delim == "") delim = " ";
            switch (delim)
            {
                case ";":
                    radioButton8.Checked = true;
                    break;

                case ":":
                    radioButton10.Checked = true;
                    break;

                case "|":
                    radioButton12.Checked = true;
                    break;

                case "\t":  //TAB
                    radioButton14.Checked = true;
                    break;

                case " ":  //SPACE
                    radioButton13.Checked = true;
                    break;

                case ",":
                    radioButton11.Checked = true;
                    break;

                default:
                    radioButton9.Checked = true;
                    textBox2.Text = delim;
                    break;
            }

            this.Text = "Разделитель";

            tabPage1.Text = "Чтение";
            groupBox1.Text = "Выберете разделитель";
            radioButton1.Text = "точка с запятой (;)";
            radioButton2.Text = "двоеточие (:)";
            radioButton3.Text = "верт. линия (|)";
            radioButton4.Text = "TAB";
            radioButton5.Text = "пробел";
            radioButton6.Text = "запятая (,)";
            radioButton7.Text = "пользовательский --->";
            button1.Text = "Ок";
            button2.Text = "Отмена";

            tabPage2.Text = "Запись";
            groupBox2.Text = "Выберете разделитель";
            radioButton8.Text = "точка с запятой (;)";
            radioButton10.Text = "двоеточие (:)";
            radioButton12.Text = "верт. линия (|)";
            radioButton14.Text = "TAB";
            radioButton13.Text = "пробел";
            radioButton11.Text = "запятая (,)";
            radioButton9.Text = "пользовательский --->";
        }

        private void bCancel(object sender, EventArgs e)
        {
            Close();
        }

        private void bDone(object sender, EventArgs e)
        {
            //Read
            string delim;
            if (radioButton1.Checked == true) delim = ";";
            else
            {
                if (radioButton2.Checked == true) delim = ":";
                else
                {
                    if (radioButton3.Checked == true) delim = "|";
                    else
                    {
                        if (radioButton4.Checked == true) delim = "\t";
                        else
                        {
                            if (radioButton5.Checked == true) delim = " ";
                            else
                            {
                                if (radioButton6.Checked == true) delim = ",";
                                else
                                {
                                    delim = textBox1.Text;
                                }
                            }
                        }
                    }
                }
            }

            //write
            string delim2;
            if (radioButton8.Checked == true) delim2 = ";";
            else
            {
                if (radioButton10.Checked == true) delim2 = ":";
                else
                {
                    if (radioButton12.Checked == true) delim2 = "|";
                    else
                    {
                        if (radioButton14.Checked == true) delim2 = "\t";
                        else
                        {
                            if (radioButton13.Checked == true) delim2 = " ";
                            else
                            {
                                if (radioButton11.Checked == true) delim2 = ",";
                                else
                                {
                                    delim2 = textBox2.Text;
                                }
                            }
                        }
                    }
                }
            }

            CsvPreferences.delimiter = delim;

            CsvPreferences.delimiter_write = delim2;

            string contentIni = "delimiter=" + CsvPreferences.delimiter + nextLine +
                "delimiter_write=" + CsvPreferences.delimiter_write + nextLine +
                "read_encoding=" + CsvPreferences.read_encoding + nextLine +
                "write_encoding=" + CsvPreferences.write_encoding;

            try
            {
                File.WriteAllText(StaticData.CSV_CONFIG_PATH, contentIni, Encoding.GetEncoding(1251)); //Encoding.Default
            }
            catch (Exception)
            {
                MessageBox.Show(error5);
            }

            Close();
        }
    }
}
