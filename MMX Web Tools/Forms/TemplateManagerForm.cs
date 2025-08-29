using MMX_Web_Tools.Models;
using MMX_Web_Tools.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace MMX_Web_Tools.Forms
{
    public partial class TemplateManagerForm : Form
    {
        private BindingList<SaleTemplate> _templates;

        public IList<SaleTemplate> Templates => _templates;

        public TemplateManagerForm(IList<SaleTemplate> templates)
        {
            InitializeComponent();
            _templates = new BindingList<SaleTemplate>(templates?.ToList() ?? new List<SaleTemplate>());
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = _templates;

            ThemeManager.ApplyTheme(this, ThemeManager.CurrentTheme);
            FormClosed += (s, e) => { };
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
