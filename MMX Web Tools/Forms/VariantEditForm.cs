using MMX_Web_Tools.Models;
using System;
using System.ComponentModel;
using System.Windows.Forms;

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
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
