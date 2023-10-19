// Troy Magbag HW4.
// 11536867

namespace Spreadsheet_Troy_Magbag
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using SpreadsheetEngine;

    /// <summary>
    /// Creates spreadsheet.
    /// </summary>
    public partial class Spreadsheet : Form
    {
        private Spreadsheets newSpreadsheet = new Spreadsheets(50, 26);

        /// <summary>
        /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
        /// </summary>
        public Spreadsheet()
        {
            this.newSpreadsheet.PropertyChanged += new PropertyChangedEventHandler(this.CellPropertyChanged);

            string[] alphabet =
            {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P",
                "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            };

            this.InitializeComponent();

            for (int loop = 0; loop < alphabet.Length; loop++)
            {
                this.dataGridView1.Columns.Add(alphabet[loop], alphabet[loop]); // Add column names.
             }

            this.dataGridView1.RowCount = 50;
            this.dataGridView1.RowHeadersVisible = true;
            for (int rows = 0; rows < 50; rows++)
            {
                this.dataGridView1.Rows[rows].HeaderCell.Value = (rows + 1).ToString(); // Number rows.
            }
         }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void Spreadsheet_Load(object sender, EventArgs e)
        {
            newSpreadsheet.PropertyChanged += new PropertyChangedEventHandler(CellPropertyChanged);
            dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
            dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);


        }

        /// <summary>
        /// Updates the cell in the datagridview when a cell’s
        /// value changes.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">Property changed.</param>
        private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Cell cell = (Cell)sender;
            if (cell != null && e.PropertyName == "Values")
            {
                this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value = cell.Text;
            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            Cell cell = newSpreadsheet.GetCell(e.RowIndex, e.ColumnIndex);
            dataGridView1[e.ColumnIndex, e.RowIndex].Value = cell.Text;
        }

        //Enters this when hitting enter after editting or leaving the cell
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int column = e.ColumnIndex;
            string text = "";
            Cell cell = newSpreadsheet.GetCell(row, column);

            try
            {
                text = dataGridView1.Rows[row].Cells[column].Value.ToString();
            }
            catch (NullReferenceException)
            {
                text = "";
            }
            cell.Text = text;
            dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value = cell.Values;
        }

        /// <summary>
        /// When demo button is pressed all cells in B has the message "This is cell B" + row number.
        /// Also column A has B's row numbers for each row in A cells.
        /// Lastly a "random message" is placed in 50 random celss.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">Event argument.</param>
        private void demoButton_Click(object sender, EventArgs e)
        {
            Random rand = new Random();

            for (int randomLoop = 0; randomLoop < 50; randomLoop++)
            {
                int randomCol = rand.Next(0, 25);
                int randomRow = rand.Next(0, 49);

                Cell randomCell = this.newSpreadsheet.GetCell(randomRow, randomCol);
                randomCell.Text = "Random Message!";
                this.newSpreadsheet.spreadsheetCells[randomRow, randomCol] = randomCell;
            }

            for (int rowA = 0; rowA < 50; rowA++)
            {
                this.newSpreadsheet.spreadsheetCells[rowA, 0].Text = "=B" + (rowA + 1).ToString();
            }

            for (int rowB = 0; rowB < 50; rowB++)
            {
                this.newSpreadsheet.spreadsheetCells[rowB, 1].Text = "This is cell B" + (rowB + 1).ToString();
            }
        }
    }
}
