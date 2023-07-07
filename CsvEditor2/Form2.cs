using System;
using System.Windows.Forms;

namespace CsvEditor
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void bRenameCol(object sender, EventArgs e)
        {
            StaticData.DataBuffer = textBox1.Text;
            Close();
        }
    }
}
