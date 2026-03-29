using GlobalClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace DVLD.Global_classes
{


    public partial class FilterBy : UserControl
    {
        public class FilterResultsEventArgs : EventArgs
        {
            public DataView FilteredView { get; }
            public int FoundRows { get; }

            public FilterResultsEventArgs(DataView FilteredView, int FoundRows)
            {
                this.FilteredView = FilteredView;
                this.FoundRows = FoundRows;
            }
        }

        public event EventHandler<FilterResultsEventArgs> FilterResultsChanged;

        private void OnFilterChanged(DataView FilteredView, int FoundRows)
        {
            FilterResultsChanged?.Invoke(this, new FilterResultsEventArgs(FilteredView, FoundRows));
        }

        // Internal DataView used to apply RowFilter expressions against the underlying DataTable.
        private DataView View;

        public FilterBy() => InitializeComponent();

        // Prepare the IsActive combo box with three options: All, Yes, No.
        private void InitializeQuestionBox()
        {
            QuestionBox.DisplayMember = "Key";
            QuestionBox.ValueMember = "Value";

            QuestionBox.Items.Clear();
            QuestionBox.Items.Add(new KeyValuePair<string, bool?>("All", null)); // no filter
            QuestionBox.Items.Add(new KeyValuePair<string, bool?>("Yes", true)); // true filter
            QuestionBox.Items.Add(new KeyValuePair<string, bool?>("No", false)); // false filter
            QuestionBox.SelectedIndex = 0;
        }

        public void LoadColumn(DataGridView Grid)
        {
            FilterBox.DisplayMember = "Key";
            FilterBox.ValueMember = "Value";

            // If the grid is bound to a DataTable, keep a DataView to filter.
            if (Grid.DataSource is DataTable dt)
                View = new DataView(dt);

            // Start with a "None" option that clears filtering.
            FilterBox.Items.Clear();
            FilterBox.Items.Add(new KeyValuePair<string, string>("None", "None"));
            QuestionBox.Visible = false;

            if (Grid != null)
                foreach (DataGridViewColumn Cln in Grid.Columns)
                {
                    if (Cln.ValueType != typeof(DateTime) && Cln.ValueType != typeof(decimal) && Cln.HeaderText != "Gender")
                        FilterBox.Items.Add(new KeyValuePair<string, string>(Cln.HeaderText, Cln.DataPropertyName));
                }

            // Default to the first item and hide the text filter until a real column is selected.
            if (FilterBox.Items.Count > 0)
            {
                FilterBox.SelectedIndex = 0;
                Filtertext.Visible = false;
            }
        }

        // When the selected filter column changes, adjust the UI and handlers accordingly.
        private void FilterBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (View?.Table == null)
                return;

            Filtertext.Clear();
            var selected = ((KeyValuePair<string, string>)FilterBox.SelectedItem).Value;

            // Show text box only when a real column is selected.
            Filtertext.Visible = selected != "None";
            QuestionBox.Visible = false;

            // If no valid column or no data, clear filter and exit.
            if (selected == "None")
            {
                View.RowFilter = "";
                return;
            }

            // Inspect the column type to decide filter behavior.
            var Type = View.Table.Columns[selected].DataType;

            // Ensure numeric-only key handler is attached only for integer columns.
            Filtertext.KeyPress -= Filtertext_KeyPress;
            if (Type == typeof(int))
                Filtertext.KeyPress += Filtertext_KeyPress;

            // For boolean columns show the QuestionBox combo instead of a text input.
            if (Type == typeof(bool))
            {
                Filtertext.Visible = false;
                QuestionBox.Visible = true;
                InitializeQuestionBox();
            }
        }

        // Restrict text input to digits for integer columns.
        private void Filtertext_KeyPress(object sender, KeyPressEventArgs e)
        {
            Filtertext.MaxLength = 9;

            // Allow control characters (backspace etc.) and digits only.
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void Filtertext_TextChanged(object sender, EventArgs e)
        {
            // Require a selected column and a valid DataView table.
            if (((KeyValuePair<string, string>)FilterBox.SelectedItem).Value == "None" || View?.Table == null) return;

            string selectedColumn = ((KeyValuePair<string, string>)FilterBox.SelectedItem).Value;
            var type = View.Table.Columns[selectedColumn].DataType;
            string filterValue = Filtertext.Text.Trim();
            try
            {
                if (!string.IsNullOrEmpty(filterValue) && View != null)
                {
                    if (type == typeof(int) && int.TryParse(filterValue, out int number))
                        // Exact match for integers.
                        View.RowFilter = $"{selectedColumn} = {number}";
                    else
                        // Prefix match for string-like columns; escape single quotes.
                        View.RowFilter = $"{selectedColumn} LIKE '{filterValue.Replace("'", "''")}%'";
                }
                else
                    View.RowFilter = "";
            }                     // Log exceptions and clear filter to keep UI stable.
            catch (Exception Ex) { ClsLogger.Log(Ex); View.RowFilter = ""; }

            int RowCount = View?.Count ?? 0;
            OnFilterChanged(View, RowCount);
        }

        // Apply boolean filter when QuestionBox combo value changes.
        private void QuestionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (QuestionBox.SelectedItem == null) return;

            bool? QuestionBoxFilter = ((KeyValuePair<string, bool?>)QuestionBox.SelectedItem).Value;
            string selectedColumn = ((KeyValuePair<string, string>)FilterBox.SelectedItem).Value;
            // If "All" selected, clear filter; otherwise filter by QuestionBox value.
            View.RowFilter = QuestionBoxFilter.HasValue ? $"{selectedColumn} = {QuestionBoxFilter.Value}" : "";

            // Update the found rows count and notify listeners of the filter change.
            int RowCount = View?.Count ?? 0;

            OnFilterChanged(View, RowCount);
        }
    }
}