using System;
using System.Windows.Forms;

namespace CsvEditor
{
    public partial class menuReplace : Form
    {
        public menuReplace()
        {
            InitializeComponent();
        }

        private void menuReplace_Load(object sender, EventArgs e)
        {
            Text = "Найти и заменить все";
            button1.Text = "Заменить";
            button2.Text = "Отмена";
            label1.Text = "Найти текст";
            label2.Text = "заменить на";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (label1.Text.Length != 0 && label2.Text.Length !=0 && label1.Text != label2.Text)
            {
                StaticData.SourceText = textBox1.Text;
                StaticData.TextForReplace = textBox2.Text;
                StaticData.replaceMarker = true;
            }
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
