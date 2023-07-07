using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CsvEditor
{
    public partial class menuEncoding : Form
    {
        private string nextLine = "\r\n";
        private string error5 = "Ошибка сохранения INI файла.";

        public menuEncoding()
        {
            InitializeComponent();
        }

        private void eLoadForm(object sender, EventArgs e)
        {
            //Read
            string encoding = CsvPreferences.read_encoding;
            switch (encoding)
            {
                case "windows-1251":
                    radioButton1.Checked = true;
                    break;

                case "utf-8":
                    radioButton2.Checked = true;
                    break;

                case "ascii":
                    radioButton3.Checked = true;
                    break;

                case "unicode":
                    radioButton4.Checked = true;
                    break;

                default:
                    radioButton1.Checked = true;
                    break;
            }

            //Write
            encoding = CsvPreferences.write_encoding;
            switch (encoding)
            {
                case "windows-1251":
                    radioButton5.Checked = true;
                    break;

                case "utf-8":
                    radioButton6.Checked = true;
                    break;

                case "ascii":
                    radioButton7.Checked = true;
                    break;

                case "unicode":
                    radioButton8.Checked = true;
                    break;

                default:
                    radioButton5.Checked = true;
                    break;
            }

            this.Text = "Кодировка";

            tabPage1.Text = "Чтение";
            groupBox1.Text = "Выберете кодировку";
            button1.Text = "Ок";
            button2.Text = "Отмена";

            tabPage2.Text = "Запись";
            groupBox2.Text = "Выберете кодировку";
        }

        private void bDone(object sender, EventArgs e)
        {
            //Read
            string encoding;
            if (radioButton1.Checked == true) encoding = "windows-1251";
            else
            {
                if (radioButton2.Checked == true) encoding = "utf-8";
                else
                {
                    if (radioButton3.Checked == true) encoding = "ascii";
                    else encoding = "unicode";
                }
            }

            //Write
            string encoding_write;
            if (radioButton5.Checked == true) encoding_write = "windows-1251";
            else
            {
                if (radioButton6.Checked == true) encoding_write = "utf-8";
                else
                {
                    if (radioButton7.Checked == true) encoding_write = "ascii";
                    else encoding_write = "unicode";
                }
            }

            CsvPreferences.read_encoding = encoding;
            CsvPreferences.write_encoding = encoding_write;

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

        private void bCancel(object sender, EventArgs e)
        {
            Close();
        }
    }
}
