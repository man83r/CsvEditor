using System;
using System.Windows.Forms;

namespace CsvEditor
{
    public partial class aboutForm : Form
    {
        public aboutForm()
        {
            InitializeComponent();
        }

        private void eLoadForm(object sender, EventArgs e)
        {
            this.Text = "О CsvEditor";
            label1.Text = "Программа предназначена для";
            label2.Text = "редактирования CSV файлов.";
            label3.Text = "НЕ ПОДДЕРЖИВАЕТСЯ";
            label4.Text = "использование разделителей ";
            label5.Text = "внутри одной ячейки.";
        }
    }
}
