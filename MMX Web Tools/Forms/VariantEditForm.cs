using MMX_Web_Tools.Models;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using MMX_Web_Tools.Utils;

namespace MMX_Web_Tools.Forms
{
    public partial class VariantEditForm : Form
    {
        private BindingList<Variant> _variants;

        public VariantEditForm(BindingList<Variant> variants)
        {
            InitializeComponent();
            _variants = variants ?? throw new ArgumentNullException(nameof(variants));
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = _variants;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = false;
            dataGridView1.AllowUserToAddRows = true;
            dataGridView1.AllowUserToDeleteRows = true;

            // Hide the first (unused) column and unwanted IDs after binding
            dataGridView1.DataBindingComplete += DataGridView1_DataBindingComplete;

            // Apply theme and keep in sync
            ThemeManager.ApplyTheme(this, ThemeManager.CurrentTheme);
            System.Windows.Forms.FormClosedEventHandler onClosed = null;
            Action<AppTheme> handler = t => ThemeManager.ApplyTheme(this, t);
            ThemeManager.ThemeChanged += handler;
            onClosed = (s, e) => { ThemeManager.ThemeChanged -= handler; this.FormClosed -= onClosed; };
            this.FormClosed += onClosed;
        }

        private void DataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns[0].Visible = false;
            }
            if (dataGridView1.Columns.Contains("CategoryId"))
            {
                dataGridView1.Columns["CategoryId"].Visible = false;
            }
            if (dataGridView1.Columns.Contains("SubCategoryId"))
            {
                dataGridView1.Columns["SubCategoryId"].Visible = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
