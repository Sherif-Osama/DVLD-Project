using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace DVLD.People.Controls
{
    // UserControl for filtering a DataGridView based on selected column and value
    public partial class FilterBy : UserControl
    {
        public event Action<DataView> FilterChanged; // Fired when filter changes
        private DataView View; // DataView for filtering

        public FilterBy()
        {
            InitializeComponent();
        }

        // Setup IsActive ComboBox options
        private void InitializeIsActiveBox()
        {
            IsActiveBox1.DisplayMember = "Key";
            IsActiveBox1.ValueMember = "Value";

            IsActiveBox1.Items.Clear();
            IsActiveBox1.Items.Add(new KeyValuePair<string, bool?>("All", null));
            IsActiveBox1.Items.Add(new KeyValuePair<string, bool?>("Yes", true));
            IsActiveBox1.Items.Add(new KeyValuePair<string, bool?>("No", false));
            IsActiveBox1.SelectedIndex = 0;
        }

        // Load filterable columns from a DataGridView
        public void LoadColumn(DataGridView Grid)
        {
            FilterBox.DisplayMember = "Key";
            FilterBox.ValueMember = "Value";

            if (Grid.DataSource is DataTable dt)
                View = new DataView(dt);

            FilterBox.Items.Clear();
            FilterBox.Items.Add(new KeyValuePair<string, string>("None", "None"));
            IsActiveBox1.Visible = false;

            if (Grid != null)
                foreach (DataGridViewColumn Cln in Grid.Columns)
                {
                    if (Cln.ValueType != typeof(DateTime) && Cln.HeaderText != "Gender")
                        FilterBox.Items.Add(new KeyValuePair<string, string>(Cln.HeaderText, Cln.DataPropertyName));
                }

            if (FilterBox.Items.Count > 0)
            {
                FilterBox.SelectedIndex = 0;
                Filtertext.Visible = false;
            }
        }

        // Handle column selection changes
        private void FilterBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = ((KeyValuePair<string, string>)FilterBox.SelectedItem).Value;

            Filtertext.Visible = selected != "None";
            IsActiveBox1.Visible = false;

            if (selected == "None" || View?.Table == null)
            {
                View.RowFilter = "";
                return;
            }

            var Type = View.Table.Columns[selected].DataType;

            Filtertext.KeyPress -= Filtertext_KeyPress;
            if (Type == typeof(int))
                Filtertext.KeyPress += Filtertext_KeyPress;

            if (Type == typeof(bool))
            {
                Filtertext.Visible = false;
                IsActiveBox1.Visible = true;
                InitializeIsActiveBox();
            }
        }

        // Allow only digits for integer columns
        private void Filtertext_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        // Update DataView filter when typing
        private void Filtertext_TextChanged(object sender, EventArgs e)
        {
            if (FilterBox.SelectedItem == null || View?.Table == null) return;

            string selectedColumn = ((KeyValuePair<string, string>)FilterBox.SelectedItem).Value;
            var type = View.Table.Columns[selectedColumn].DataType;
            string filterValue = Filtertext.Text.Trim();
            try
            {
                if (!string.IsNullOrEmpty(filterValue) && View != null)
                {
                    if (type == typeof(int) && int.TryParse(filterValue, out int number))
                        View.RowFilter = $"{selectedColumn} = {number}";
                    else
                        View.RowFilter = $"{selectedColumn} LIKE '{filterValue.Replace("'", "''")}%'";
                }
                else
                    View.RowFilter = "";
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); View.RowFilter = ""; }
            FilterChanged?.Invoke(View);
        }

        // Handle IsActive ComboBox filter
        private void IsActiveBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsActiveBox1.SelectedItem == null) return;

            bool? IsActiveFilter = ((KeyValuePair<string, bool?>)IsActiveBox1.SelectedItem).Value;

            View.RowFilter = IsActiveFilter.HasValue ? $"IsActive = {IsActiveFilter.Value}" : "";

            FilterChanged?.Invoke(View);
        }
    }
}
