using System.Windows.Forms;

namespace CsvEditor
{
    class DoubleBufferedDataGridView : DataGridView 
    {
        protected override bool DoubleBuffered { get => true; }
    }
}
