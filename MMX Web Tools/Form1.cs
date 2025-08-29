using MMX_Web_Tools.Forms;
using MMX_Web_Tools.Models;
using MMX_Web_Tools.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using MMX_Web_Tools.Controls;
using AutoUpdaterDotNET;

namespace MMX_Web_Tools
{
    public partial class Form1 : Form
    {
        private enum DataFormat { Unknown, LineBasedDocument, ProductListXml }

        private SortableBindingList<Product> _products = new SortableBindingList<Product>();
        private List<Product> _allProducts = new List<Product>();
        private string _currentFile = string.Empty;
        private DataFormat _lastLoadedFormat = DataFormat.Unknown;

        // state
        private bool _searchFilterMode = true; // true=filter results, false=navigate
        private bool _detailedLog = false;

        // Sale templates
        private List<SaleTemplate> _templates = new List<SaleTemplate>();
        private SaleTemplate _activeTemplate = null;

        // variants binding for details panel
        private BindingList<Variant> _variantBinding = null;

        // themed status progress bar host
        private ThemedProgressBar _statusProgressBar;
        private ToolStripControlHost _statusProgressHost;

        // Central place to configure the update manifest URL
        private const string UpdateXmlUrl = "https://github.com/Sup3rTun3R/MMX-Web-Tools/releases/latest/download/update.xml"; // TODO: set your real URL

        public Form1()
        {
            InitializeComponent();
            InitGrid();
            InitVariantGrid();
            toolStripTextBoxSearch.KeyDown += toolStripTextBoxSearch_KeyDown;
            panelBulk.Visible = false;
            panelDetails.Visible = true;
            chkDetailedLog.CheckedChanged += chkDetailedLog_CheckedChanged;

            // Replace default ToolStripProgressBar with themed one
            SetupThemedStatusProgressBar();

            // Apply default theme and listen for theme changes
            ThemeManager.ApplyTheme(this, AppTheme.Light);
            ThemeManager.ThemeChanged += t => ThemeManager.ApplyTheme(this, t);

            // Set modern glyph icons
            ApplyToolStripIcons();
        }

        private void SetupThemedStatusProgressBar()
        {
            try
            {
                _statusProgressBar = new ThemedProgressBar
                {
                    Minimum = 0,
                    Maximum = 100,
                    Value = 0,
                    Size = new Size(200, 16)
                };
                _statusProgressHost = new ToolStripControlHost(_statusProgressBar)
                {
                    Alignment = ToolStripItemAlignment.Right,
                    AutoSize = false,
                    Size = new Size(200, 16),
                    Margin = new Padding(4, 3, 4, 3)
                };

                if (toolStripProgressBar != null)
                {
                    // remove default progress bar to avoid the light background
                    statusStrip1.Items.Remove(toolStripProgressBar);
                    toolStripProgressBar.Visible = false;
                }

                statusStrip1.Items.Add(_statusProgressHost);
            }
            catch
            {
                // ignore setup errors; fallback to default progress bar
            }
        }

        private void ApplyToolStripIcons()
        {
            var p = ThemeManager.GetPalette(ThemeManager.CurrentTheme);
            // Use circle backgrounds with themed colors and white/green foregrounds
            toolStripButtonOpen.Image = IconFactory.CreateCircularIcon("⭳", p.ButtonBack, p.ButtonFore, 18);
            toolStripButtonSave.Image = IconFactory.CreateCircularIcon("💾", p.ButtonBack, p.ButtonFore, 18);
            toolStripButtonEdit.Image = IconFactory.CreateCircularIcon("✎", p.ButtonBack, p.ButtonFore, 18);
            toolStripButtonBulk.Image = IconFactory.CreateCircularIcon("⇧", p.ButtonBack, p.ButtonFore, 18);
            toolStripButtonSearch.Image = IconFactory.CreateCircularIcon("🔎", p.ButtonBack, p.ButtonFore, 18);

            // Refresh icons on theme change
            ThemeManager.ThemeChanged += _ => ApplyToolStripIcons();
        }

        private void chkDetailedLog_CheckedChanged(object sender, EventArgs e)
        {
            _detailedLog = chkDetailedLog.Checked;
            detailedLogToolStripMenuItem.Checked = _detailedLog;
        }

        private void InitGrid()
        {
            dataGridViewProducts.AutoGenerateColumns = false;
            dataGridViewProducts.Columns.Clear();
            dataGridViewProducts.RowHeadersVisible = false;
            dataGridViewProducts.AllowUserToResizeRows = false;
            dataGridViewProducts.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            var colName = new DataGridViewTextBoxColumn { DataPropertyName = nameof(Product.Name), HeaderText = "Name", FillWeight = 250, MinimumWidth = 220 };
            var colSku = new DataGridViewTextBoxColumn { DataPropertyName = nameof(Product.Sku), HeaderText = "SKU", FillWeight = 140 };
            var colProdId = new DataGridViewTextBoxColumn { DataPropertyName = nameof(Product.SubCategoryId), HeaderText = "Product ID", FillWeight = 90 };
            var colCatId = new DataGridViewTextBoxColumn { DataPropertyName = nameof(Product.CategoryId), HeaderText = "Category ID", FillWeight = 90 };
            var colCost = new DataGridViewTextBoxColumn { DataPropertyName = nameof(Product.Cost1), HeaderText = "Cost", FillWeight = 80 };
            var colRetail = new DataGridViewTextBoxColumn { DataPropertyName = nameof(Product.Retail), HeaderText = "Retail Price", FillWeight = 100 };
            var colSale = new DataGridViewTextBoxColumn { DataPropertyName = nameof(Product.Sale), HeaderText = "Sale Price", FillWeight = 100 };
            var colStock = new DataGridViewTextBoxColumn { DataPropertyName = nameof(Product.Stock), HeaderText = "Stock", FillWeight = 70 };

            dataGridViewProducts.Columns.AddRange(new DataGridViewColumn[]
            {
                colName, colSku, colProdId, colCatId, colCost, colRetail, colSale, colStock
            });

            dataGridViewProducts.DataSource = _products;

            // Row color coding: main items vs attached to another (variants exist?)
            dataGridViewProducts.RowPrePaint += (s, e) =>
            {
                if (e.RowIndex < 0 || e.RowIndex >= dataGridViewProducts.Rows.Count) return;
                var row = dataGridViewProducts.Rows[e.RowIndex];
                var product = row.DataBoundItem as Product;
                if (product == null) return;
                var palette = ThemeManager.GetPalette(ThemeManager.CurrentTheme);
                if (product.Variants != null && product.Variants.Count > 0)
                {
                    row.DefaultCellStyle.BackColor = palette.GridVariantRowBack;
                    row.DefaultCellStyle.ForeColor = palette.GridFore;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = palette.GridBack;
                    row.DefaultCellStyle.ForeColor = palette.GridFore;
                }
            };
        }

        private void InitVariantGrid()
        {
            if (dataGridViewVariants == null) return;
            dataGridViewVariants.AutoGenerateColumns = false;
            dataGridViewVariants.Columns.Clear();
            dataGridViewVariants.RowHeadersVisible = false;
            dataGridViewVariants.AllowUserToAddRows = false; // we use explicit Add button
            dataGridViewVariants.AllowUserToResizeRows = false;

            var colVarId = new DataGridViewTextBoxColumn { DataPropertyName = nameof(Variant.VariantId), HeaderText = "Variant ID", FillWeight = 60, MinimumWidth = 70 };
            var colVarName = new DataGridViewTextBoxColumn { DataPropertyName = nameof(Variant.Name), HeaderText = "Name", FillWeight = 150, MinimumWidth = 120 };
            var colGroup = new DataGridViewTextBoxColumn { DataPropertyName = nameof(Variant.Group), HeaderText = "Group", FillWeight = 100, MinimumWidth = 100 };
            var colPrice = new DataGridViewTextBoxColumn { DataPropertyName = nameof(Variant.Price), HeaderText = "Price", FillWeight = 70, MinimumWidth = 80, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } };

            dataGridViewVariants.Columns.AddRange(new DataGridViewColumn[]
            {
                colVarId, colVarName, colGroup, colPrice
            });
        }

        private void Log(string message)
        {
            var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var prefix = _detailedLog ? $"[{ts}] " : string.Empty;
            richTextBoxLog.AppendText(prefix + message + Environment.NewLine);
        }

        private void SetProgress(int value)
        {
            int v = Math.Min(100, Math.Max(0, value));
            if (_statusProgressBar != null)
            {
                // avoid exception when setting value to 0 when min=0 and max=0
                _statusProgressBar.Value = v;
                _statusProgressBar.Invalidate();
            }
            if (toolStripProgressBar != null)
            {
                toolStripProgressBar.Value = v;
            }
        }

        // File menu
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    _currentFile = ofd.FileName;
                    toolStripStatusLabel.Text = $"Loading {_currentFile}...";
                    SetProgress(0);
                    _products.Clear();
                    _allProducts.Clear();
                    richTextBoxLog.Clear();
                    _lastLoadedFormat = DataFormat.Unknown;

                    if (!backgroundWorker.IsBusy)
                    {
                        backgroundWorker.RunWorkerAsync(_currentFile);
                    }
                }
            }
        }

        private void saveChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_allProducts == null || _allProducts.Count == 0)
            {
                MessageBox.Show(this, "No data to save.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "XML (*.xml)|*.xml|CSV (*.csv)|*.csv";
                sfd.FileName = Path.GetFileNameWithoutExtension(_currentFile) + "-updated.xml";
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        if (Path.GetExtension(sfd.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                        {
                            ExportCsv(sfd.FileName);
                        }
                        else
                        {
                            if (_lastLoadedFormat == DataFormat.ProductListXml)
                                ExportProductListXml(sfd.FileName);
                            else
                                ExportXmlLike(sfd.FileName);
                        }
                        MessageBox.Show(this, "Saved successfully.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // About
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "MMX Product Pricing Manager\nWritten by MattG", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Export helpers
        private void ExportCsv(string fileName)
        {
            var list = _allProducts; // export full dataset
            var sb = new StringBuilder();
            sb.AppendLine("CategoryId,SubCategoryId,Name,SKU,Code,Cost1,Cost2,Retail,Sale,Stock,VariantId,VariantName,VariantGroup,VariantPrice");
            foreach (var p in list)
            {
                if (p.Variants != null && p.Variants.Count > 0)
                {
                    foreach (var v in p.Variants)
                    {
                        sb.AppendLine(string.Join(",",
                            p.CategoryId, p.SubCategoryId,
                            Csv(p.Name), Csv(p.Sku), Csv(p.Code),
                            p.Cost1.ToString(CultureInfo.InvariantCulture),
                            p.Cost2.ToString(CultureInfo.InvariantCulture),
                            p.Retail.ToString(CultureInfo.InvariantCulture),
                            p.Sale.ToString(CultureInfo.InvariantCulture),
                            p.Stock,
                            v.VariantId,
                            Csv(v.Name), Csv(v.Group),
                            v.Price.ToString(CultureInfo.InvariantCulture)));
                    }
                }
                else
                {
                    sb.AppendLine
                    (string.Join(",",
                        p.CategoryId, p.SubCategoryId,
                        Csv(p.Name), Csv(p.Sku), Csv(p.Code),
                        p.Cost1.ToString(CultureInfo.InvariantCulture),
                        p.Cost2.ToString(CultureInfo.InvariantCulture),
                        p.Retail.ToString(CultureInfo.InvariantCulture),
                        p.Sale.ToString(CultureInfo.InvariantCulture),
                        p.Stock,
                        "", "", "",
                        0.ToString(CultureInfo.InvariantCulture)));
                }
            }
            File.WriteAllText(fileName, sb.ToString());
        }

        private string Csv(string s)
        {
            if (s == null) return string.Empty;
            var needQuotes = s.Contains(",") || s.Contains("\"") || s.Contains("\n") || s.Contains("\r");
            s = s.Replace("\"", "\"\"");
            return needQuotes ? "\"" + s + "\"" : s;
        }

        private void ExportXmlLike(string fileName)
        {
            var list = _allProducts; // export full dataset
            var sb = new StringBuilder();
            sb.AppendLine("<DOCUMENT>");
            foreach (var p in list)
            {
                sb.AppendLine(p.CategoryId.ToString());
                sb.AppendLine(p.SubCategoryId.ToString());
                sb.AppendLine(string.Empty);
                sb.AppendLine(p.Name ?? string.Empty);
                sb.AppendLine(string.Empty);
                sb.AppendLine(p.Sku ?? string.Empty);
                sb.AppendLine(string.Empty);
                sb.AppendLine(p.Code ?? string.Empty);
                sb.AppendLine(p.Cost1.ToString(CultureInfo.InvariantCulture));
                sb.AppendLine(p.Cost2.ToString(CultureInfo.InvariantCulture));
                sb.AppendLine(p.Retail.ToString(CultureInfo.InvariantCulture));
                sb.AppendLine(p.Sale.ToString(CultureInfo.InvariantCulture));
                sb.AppendLine(p.Stock.ToString(CultureInfo.InvariantCulture));

                foreach (var v in p.Variants)
                {
                    sb.AppendLine(p.CategoryId.ToString());
                    sb.AppendLine(p.SubCategoryId.ToString());
                    sb.AppendLine(v.VariantId.ToString());
                    sb.AppendLine(v.Name ?? string.Empty);
                    sb.AppendLine(v.Group ?? string.Empty);
                    sb.AppendLine(string.Empty);
                    sb.AppendLine(string.Empty);
                    sb.AppendLine(string.Empty);
                    sb.AppendLine(string.Empty);
                    sb.AppendLine(string.Empty);
                    sb.AppendLine(v.Price.ToString(CultureInfo.InvariantCulture));
                    sb.AppendLine(string.Empty);
                    sb.AppendLine(string.Empty);
                }
            }
            sb.AppendLine("</DOCUMENT>");
            File.WriteAllText(fileName, sb.ToString());
        }

        private void ExportProductListXml(string fileName)
        {
            var list = _allProducts; // export full dataset
            var root = new XElement("productlist");
            foreach (var p in list)
            {
                root.Add(new XElement("productvariant",
                    new XElement("ProductID", p.CategoryId),
                    new XElement("VariantID", p.SubCategoryId),
                    new XElement("KitItemID"),
                    new XElement("Name", p.Name ?? string.Empty),
                    new XElement("KitGroup", " "),
                    new XElement("SKU", p.Sku ?? string.Empty),
                    new XElement("SKUSuffix"),
                    new XElement("ManufacturerPartNumber", p.Code ?? string.Empty),
                    new XElement("Cost", p.Cost1.ToString(CultureInfo.InvariantCulture)),
                    new XElement("MSRP", p.Cost2.ToString(CultureInfo.InvariantCulture)),
                    new XElement("Price", p.Retail.ToString(CultureInfo.InvariantCulture)),
                    new XElement("SalePrice", p.Sale.ToString(CultureInfo.InvariantCulture)),
                    new XElement("Inventory", p.Stock)));

                foreach (var v in p.Variants)
                {
                    root.Add(new XElement("productvariant",
                        new XElement("ProductID", p.CategoryId),
                        new XElement("VariantID", p.SubCategoryId),
                        new XElement("KitItemID", v.VariantId),
                        new XElement("Name", v.Name ?? string.Empty),
                        new XElement("KitGroup", v.Group ?? string.Empty),
                        new XElement("SKU"),
                        new XElement("SKUSuffix"),
                        new XElement("ManufacturerPartNumber"),
                        new XElement("Cost"),
                        new XElement("MSRP"),
                        new XElement("Price", v.Price.ToString(CultureInfo.InvariantCulture)),
                        new XElement("SalePrice"),
                        new XElement("Inventory")));
                }
            }

            var doc = new XDocument(root);
            doc.Save(fileName);
        }

        // Toolbar search
        private void toolStripButtonSearch_Click(object sender, EventArgs e)
        {
            ApplySearch(toolStripTextBoxSearch.Text);
        }

        private void toolStripTextBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                ApplySearch(toolStripTextBoxSearch.Text);
            }
        }

        // Toolbar search mode dropdown
        private void filterResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _searchFilterMode = true;
            filterResultsToolStripMenuItem.Checked = true;
            navigateToolStripMenuItem.Checked = false;
            filterResultsEditToolStripMenuItem.Checked = true;
            navigateEditToolStripMenuItem.Checked = false;
        }

        private void navigateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _searchFilterMode = false;
            navigateToolStripMenuItem.Checked = true;
            filterResultsToolStripMenuItem.Checked = false;
            navigateEditToolStripMenuItem.Checked = true;
            filterResultsEditToolStripMenuItem.Checked = false;
        }

        // Edit menu handlers
        private void filterResultsEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _searchFilterMode = true;
            filterResultsEditToolStripMenuItem.Checked = true;
            navigateEditToolStripMenuItem.Checked = false;
            filterResultsToolStripMenuItem.Checked = true;
            navigateToolStripMenuItem.Checked = false;
        }

        private void navigateEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _searchFilterMode = false;
            navigateEditToolStripMenuItem.Checked = true;
            filterResultsEditToolStripMenuItem.Checked = false;
            navigateToolStripMenuItem.Checked = true;
            filterResultsToolStripMenuItem.Checked = false;
        }

        private void detailedLogToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            _detailedLog = detailedLogToolStripMenuItem.Checked;
            chkDetailedLog.Checked = _detailedLog;
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new FindDialog(toolStripTextBoxSearch.Text))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    var term = dlg.SearchText;
                    toolStripTextBoxSearch.Text = term;
                    ApplySearch(term);
                }
            }
        }

        private void focusSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripTextBoxSearch.Focus();
            toolStripTextBoxSearch.SelectAll();
        }

        private void noneTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _activeTemplate = null;
            ApplySearch(toolStripTextBoxSearch.Text);
            toolStripStatusLabel.Text = "Sale template: None";
        }

        private void manageTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new TemplateManagerForm(_templates))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _templates = dlg.Templates.ToList();
                    // Rebuild the Sale Templates dropdown: first item is None, then templates, then Manage...
                    saleTemplatesToolStripMenuItem.DropDownItems.Clear();
                    saleTemplatesToolStripMenuItem.DropDownItems.Add(noneTemplateToolStripMenuItem);
                    foreach (var t in _templates)
                    {
                        var item = new ToolStripMenuItem(t.Name);
                        item.Tag = t;
                        item.Click += (s, _) => { _activeTemplate = (SaleTemplate)((ToolStripMenuItem)s).Tag; ApplySearch(toolStripTextBoxSearch.Text); UpdateTemplateStatus(); };
                        saleTemplatesToolStripMenuItem.DropDownItems.Add(item);
                    }
                    saleTemplatesToolStripMenuItem.DropDownItems.Add(manageTemplatesToolStripMenuItem);
                    UpdateTemplateStatus();
                }
            }
        }

        private void UpdateTemplateStatus()
        {
            if (_activeTemplate == null)
            {
                toolStripStatusLabel.Text = "Sale template: None";
                return;
            }
            // calculate unmatched terms
            var terms = (_activeTemplate.Terms ?? new List<string>()).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
            var lower = new HashSet<string>(terms.Select(t => t.ToLowerInvariant()));
            var matched = new HashSet<string>();
            foreach (var p in _allProducts)
            {
                var hay = string.Join("|", p.Name ?? string.Empty, p.Sku ?? string.Empty, p.Code ?? string.Empty).ToLowerInvariant();
                foreach (var term in lower)
                {
                    if (hay.Contains(term)) matched.Add(term);
                }
            }
            var missing = terms.Where(t => !matched.Contains(t.ToLowerInvariant())).ToList();
            toolStripStatusLabel.Text = missing.Count == 0 ? $"Sale template: {_activeTemplate.Name} (all matched)" : $"Sale template: {_activeTemplate.Name} (missing: {string.Join(", ", missing)})";
        }

        private IEnumerable<Product> ApplyTemplateFilter(IEnumerable<Product> source)
        {
            if (_activeTemplate == null) return source;
            var terms = (_activeTemplate.Terms ?? new List<string>()).Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.ToLowerInvariant()).ToList();
            if (terms.Count == 0) return source;
            // include products that match any term
            var filtered = source.Where(p =>
            {
                var hay = string.Join("|", p.Name ?? string.Empty, p.Sku ?? string.Empty, p.Code ?? string.Empty).ToLowerInvariant();
                return terms.Any(t => hay.Contains(t));
            });
            return filtered;
        }

        private void ApplySearch(string text)
        {
            var term = (text ?? string.Empty).Trim();
            IEnumerable<Product> filtered = _allProducts;
            if (_activeTemplate != null)
            {
                filtered = ApplyTemplateFilter(filtered);
            }
            if (!string.IsNullOrEmpty(term))
            {
                var t = term.ToLowerInvariant();
                filtered = filtered.Where(p =>
                    (p.Name ?? string.Empty).ToLowerInvariant().Contains(t) ||
                    (p.Sku ?? string.Empty).ToLowerInvariant().Contains(t) ||
                    (p.Code ?? string.Empty).ToLowerInvariant().Contains(t));
            }

            if (_searchFilterMode)
            {
                var sorted = filtered.OrderBy(p => p.Name ?? string.Empty).ToList();
                _products = new SortableBindingList<Product>(sorted);
                dataGridViewProducts.DataSource = _products;
                toolStripStatusLabel.Text = $"Showing {sorted.Count} of {_allProducts.Count}";
            }
            else
            {
                _products = new SortableBindingList<Product>(_allProducts.OrderBy(p => p.Name ?? string.Empty).ToList());
                dataGridViewProducts.DataSource = _products;
                if (!string.IsNullOrEmpty(term))
                {
                    var first = _products.FirstOrDefault(p =>
                        (p.Name ?? string.Empty).IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (p.Sku ?? string.Empty).IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (p.Code ?? string.Empty).IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
                    if (first != null)
                    {
                        var idx = _products.IndexOf(first);
                        if (idx >= 0)
                        {
                            dataGridViewProducts.ClearSelection();
                            dataGridViewProducts.Rows[idx].Selected = true;
                            dataGridViewProducts.FirstDisplayedScrollingRowIndex = Math.Max(0, idx);
                        }
                    }
                }
                toolStripStatusLabel.Text = $"Showing {_products.Count} items";
            }
            UpdateTemplateStatus();
        }

        // Theme
        private void darkGrayThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Map to Dim
            ThemeManager.CurrentTheme = AppTheme.Dim;
            ThemeManager.ApplyTheme(this);
        }

        private void matrixThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThemeManager.CurrentTheme = AppTheme.MatrixDark;
            ThemeManager.ApplyTheme(this);
        }

        private void lightThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThemeManager.CurrentTheme = AppTheme.Light;
            ThemeManager.ApplyTheme(this);
        }

        private void ApplyTheme(bool dark, bool matrix)
        {
            // Preserve old signature for compatibility with existing handlers if any other code calls it
            if (!dark)
            {
                ThemeManager.CurrentTheme = AppTheme.Light;
            }
            else if (matrix)
            {
                ThemeManager.CurrentTheme = AppTheme.MatrixDark;
            }
            else
            {
                ThemeManager.CurrentTheme = AppTheme.Dim;
            }
            ThemeManager.ApplyTheme(this);
        }

        // Selection and bulk
        private void dataGridViewProducts_SelectionChanged(object sender, EventArgs e)
        {
            var count = dataGridViewProducts.SelectedRows.Count;
            if (count <= 1)
            {
                panelDetails.Visible = true; panelBulk.Visible = false;
                var p = count == 1 ? dataGridViewProducts.SelectedRows[0].DataBoundItem as Product : null;
                UpdateDetails(p);
            }
            else
            {
                panelDetails.Visible = false; panelBulk.Visible = true;
            }
        }

        private void btnEditSelected_Click(object sender, EventArgs e)
        {
            if (dataGridViewProducts.SelectedRows.Count == 0) return;
            var row = dataGridViewProducts.SelectedRows[0];
            var product = row.DataBoundItem as Product;
            if (product == null) return;

            using (var dlg = new ProductEditForm(product))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    Log($"Edited {product.Sku}: Retail {dlg.OriginalRetail} -> {product.Retail}, Sale {dlg.OriginalSale} -> {product.Sale}");
                    dataGridViewProducts.Refresh();
                    UpdateDetails(product);
                }
            }
        }

        private void dataGridViewProducts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var product = dataGridViewProducts.Rows[e.RowIndex].DataBoundItem as Product;
            if (product == null) return;
            using (var dlg = new ProductEditForm(product))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    Log($"Edited {product.Sku}: Retail {dlg.OriginalRetail} -> {product.Retail}, Sale {dlg.OriginalSale} -> {product.Sale}");
                    dataGridViewProducts.Refresh();
                    UpdateDetails(product);
                }
            }
        }

        private void btnBulkUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show(this, "Select at least one row.", "Bulk Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selected = dataGridViewProducts.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(r => r.DataBoundItem as Product)
                .Where(p => p != null)
                .ToList();

            using (var dlg = new BulkUpdateForm())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    foreach (var p in selected)
                    {
                        var oldRetail = p.Retail;
                        var oldSale = p.Sale;
                        if (dlg.ApplyToRetail)
                        {
                            p.Retail = dlg.Apply(p.Retail);
                        }
                        if (dlg.ApplyToSale)
                        {
                            p.Sale = dlg.Apply(p.Sale);
                        }
                        Log($"Bulk updated {p.Sku}: Retail {oldRetail} -> {p.Retail}, Sale {oldSale} -> {p.Sale}");
                    }
                    dataGridViewProducts.Refresh();
                }
            }
        }

        private void btnBulkApply_Click(object sender, EventArgs e)
        {
            if (dataGridViewProducts.SelectedRows.Count < 2)
            { MessageBox.Show(this, "Select 2 or more rows to bulk update.", "Bulk Update", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            var applyToRetail = chkBulkRetail.Checked; var applyToSale = chkBulkSale.Checked;
            if (!applyToRetail && !applyToSale)
            { MessageBox.Show(this, "Select at least one target (Retail or Sale).", "Bulk Update", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            var usePercent = radioPercent.Checked; var increase = radioIncrease.Checked; var amount = numBulkAmount.Value;
            foreach (DataGridViewRow row in dataGridViewProducts.SelectedRows)
            {
                if (row.DataBoundItem is Product p)
                {
                    var oldRetail = p.Retail; var oldSale = p.Sale;
                    if (applyToRetail) p.Retail = ApplyBulk(p.Retail, amount, usePercent, increase);
                    if (applyToSale) p.Sale = ApplyBulk(p.Sale, amount, usePercent, increase);
                    Log($"Bulk updated {p.Sku}: Retail {oldRetail} -> {p.Retail}, Sale {oldSale} -> {p.Sale}");
                }
            }
            dataGridViewProducts.Refresh();
        }

        private decimal ApplyBulk(decimal value, decimal amount, bool percent, bool increase)
        {
            if (percent) { var delta = value * (amount / 100m); return increase ? value + delta : value - delta; }
            return increase ? value + amount : value - amount;
        }

        private void UpdateDetails(Product p)
        {
            if (dataGridViewVariants != null) { dataGridViewVariants.DataSource = null; }

            if (p == null)
            {
                txtDetName.Text = txtDetSku.Text = txtDetCode.Text = txtDetRetail.Text = txtDetSale.Text = txtDetStock.Text = string.Empty;
                _variantBinding = null;
                if (btnAddVariant != null) btnAddVariant.Enabled = false;
                if (btnRemoveVariant != null) btnRemoveVariant.Enabled = false;
                if (btnEditOptions != null) btnEditOptions.Enabled = false;
                return;
            }
            txtDetName.Text = p.Name; txtDetSku.Text = p.Sku; txtDetCode.Text = p.Code;
            txtDetRetail.Text = p.Retail.ToString(CultureInfo.InvariantCulture);
            txtDetSale.Text = p.Sale.ToString(CultureInfo.InvariantCulture);
            txtDetStock.Text = p.Stock.ToString(CultureInfo.InvariantCulture);

            _variantBinding = new BindingList<Variant>(p.Variants ?? new List<Variant>());
            if (dataGridViewVariants != null)
            {
                dataGridViewVariants.AutoGenerateColumns = true;
                dataGridViewVariants.DataSource = _variantBinding;
                if (dataGridViewVariants.Columns.Count > 0)
                {
                    if (dataGridViewVariants.Columns.Contains("VariantId")) dataGridViewVariants.Columns["VariantId"].HeaderText = "Variant ID";
                    if (dataGridViewVariants.Columns.Contains("Name")) dataGridViewVariants.Columns["Name"].HeaderText = "Name";
                    if (dataGridViewVariants.Columns.Contains("Group")) dataGridViewVariants.Columns["Group"].HeaderText = "Group";
                    if (dataGridViewVariants.Columns.Contains("Price")) { dataGridViewVariants.Columns["Price"].HeaderText = "Price"; dataGridViewVariants.Columns["Price"].DefaultCellStyle.Format = "N2"; }
                }
            }
            if (btnAddVariant != null) btnAddVariant.Enabled = true;
            if (btnRemoveVariant != null) btnRemoveVariant.Enabled = _variantBinding.Count > 0;
            if (btnEditOptions != null) btnEditOptions.Enabled = _variantBinding.Count > 0;
        }

        private void btnEditOptions_Click(object sender, EventArgs e)
        {
            var product = GetSelectedProduct();
            if (product == null || _variantBinding == null) return;
            using (var dlg = new VariantEditForm(_variantBinding))
            {
                dlg.Text = $"Edit Options - {product.Sku}";
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    dataGridViewVariants.Refresh();
                    Log($"Edited options for {product.Sku}");
                }
            }
        }

        // Background worker
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var filePath = e.Argument as string; var products = new List<Product>();
            try
            {
                var content = File.ReadAllText(filePath);
                if (LooksLikeProductListXml(content)) { _lastLoadedFormat = DataFormat.ProductListXml; products = ParseProductListXml(content, p => backgroundWorker.ReportProgress(p)); }
                else { _lastLoadedFormat = DataFormat.LineBasedDocument; products = ParseLineBasedDocument(content, p => backgroundWorker.ReportProgress(p)); }
                e.Result = products;
            }
            catch (Exception ex) { e.Result = ex; }
        }
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        { SetProgress(e.ProgressPercentage); }
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripStatusLabel.Text = "Ready"; SetProgress(0);
            if (e.Result is Exception ex) { MessageBox.Show(this, ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            _allProducts = e.Result as List<Product> ?? new List<Product>(); _allProducts = _allProducts.OrderBy(p => p.Name ?? string.Empty).ToList(); ApplySearch(toolStripTextBoxSearch.Text);
        }

        // Parsing helpers
        private static bool LooksLikeProductListXml(string content) { if (string.IsNullOrEmpty(content)) return false; var t = content.TrimStart(); return t.StartsWith("<productlist", StringComparison.OrdinalIgnoreCase) || t.Contains("<productvariant>"); }
        private static List<Product> ParseProductListXml(string content, Action<int> progress) { var doc = XDocument.Parse(content, LoadOptions.PreserveWhitespace); var nodes = doc.Descendants("productvariant").ToList(); var dict = new Dictionary<string, Product>(); int total = Math.Max(1, nodes.Count); int idx = 0; foreach (var n in nodes) { idx++; progress((int)(idx * 100f / total)); int pid = SafeInt(n.Element("ProductID")?.Value); int vid = SafeInt(n.Element("VariantID")?.Value); string key = pid + "|" + vid; if (!dict.TryGetValue(key, out var p)) { p = new Product { CategoryId = pid, SubCategoryId = vid, Variants = new List<Variant>() }; dict[key] = p; } var kit = n.Element("KitItemID")?.Value; bool isMain = string.IsNullOrWhiteSpace(kit); if (isMain) { p.Name = (n.Element("Name")?.Value ?? string.Empty).Trim(); p.Sku = (n.Element("SKU")?.Value ?? string.Empty).Trim(); p.Code = (n.Element("ManufacturerPartNumber")?.Value ?? string.Empty).Trim(); p.Cost1 = SafeDecimal(n.Element("Cost")?.Value); p.Cost2 = SafeDecimal(n.Element("MSRP")?.Value); p.Retail = SafeDecimal(n.Element("Price")?.Value); p.Sale = SafeDecimal(n.Element("SalePrice")?.Value); p.Stock = SafeInt(n.Element("Inventory")?.Value); } else { var v = new Variant { CategoryId = pid, SubCategoryId = vid, VariantId = SafeInt(kit), Name = (n.Element("Name")?.Value ?? string.Empty).Trim(), Group = (n.Element("KitGroup")?.Value ?? string.Empty).Trim(), Price = SafeDecimal(n.Element("Price")?.Value) }; p.Variants.Add(v); } } return dict.Values.ToList(); }
        private static List<Product> ParseLineBasedDocument(string content, Action<int> progress) { var list = new List<Product>(); var lines = content.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None).Select(l => (l ?? string.Empty).Trim()).ToArray(); int i = 0; while (i < lines.Length && (string.IsNullOrWhiteSpace(lines[i]) || lines[i].StartsWith("<", StringComparison.OrdinalIgnoreCase))) i++; Product cur = null; while (i < lines.Length) { progress((int)(i * 100f / Math.Max(1, lines.Length))); if (!int.TryParse(lines[i], out var cat)) { i++; continue; } i++; if (i >= lines.Length || !int.TryParse(lines[i], out var sub)) { i++; continue; } i++; if (i < lines.Length && string.IsNullOrEmpty(lines[i])) { i++; var name = ReadSafe(lines, ref i); i++; var sku = ReadSafe(lines, ref i); i++; var code = ReadSafe(lines, ref i); var cost1 = ReadDecimal(lines, ref i); var cost2 = ReadDecimal(lines, ref i); var retail = ReadDecimal(lines, ref i); var sale = ReadDecimal(lines, ref i); var stock = ReadInt(lines, ref i); cur = new Product { CategoryId = cat, SubCategoryId = sub, Name = name, Sku = sku, Code = code, Cost1 = cost1, Cost2 = cost2, Retail = retail, Sale = sale, Stock = stock }; list.Add(cur); } else { if (cur == null) { SkipVariant(lines, ref i); continue; } var varId = ReadInt(lines, ref i); var varName = ReadSafe(lines, ref i); var group = ReadSafe(lines, ref i); for (int k = 0; k < 5 && i < lines.Length; k++) i++; var price = ReadDecimal(lines, ref i); for (int k = 0; k < 2 && i < lines.Length; k++) i++; cur.Variants.Add(new Variant { CategoryId = cat, SubCategoryId = sub, VariantId = varId, Name = varName, Group = group, Price = price }); } } return list; }
        private static void SkipVariant(string[] lines, ref int i) { int skip = 1 + 1 + 1 + 5 + 1 + 2; i = Math.Min(lines.Length, i + skip); }
        private static string ReadSafe(string[] lines, ref int i) { if (i >= lines.Length) return string.Empty; return lines[i++]; }
        private static int ReadInt(string[] lines, ref int i) { if (i >= lines.Length) return 0; int.TryParse(lines[i++], out var v); return v; }
        private static decimal ReadDecimal(string[] lines, ref int i) { if (i >= lines.Length) return 0m; decimal.TryParse(lines[i++], NumberStyles.Any, CultureInfo.InvariantCulture, out var v); return v; }
        private static int SafeInt(string s) { if (int.TryParse((s ?? string.Empty).Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v)) return v; return 0; }
        private static decimal SafeDecimal(string s) { if (decimal.TryParse((s ?? string.Empty).Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var v)) return v; return 0m; }

        // Variant buttons
        private void btnAddVariant_Click(object sender, EventArgs e)
        {
            var product = GetSelectedProduct();
            if (product == null) return;
            if (_variantBinding == null)
            {
                _variantBinding = new BindingList<Variant>(product.Variants);
                dataGridViewVariants.DataSource = _variantBinding;
            }
            var nextId = (product.Variants?.Count > 0) ? product.Variants.Max(v => v.VariantId) + 1 : 1;
            var vnew = new Variant
            {
                CategoryId = product.CategoryId,
                SubCategoryId = product.SubCategoryId,
                VariantId = nextId,
                Name = "New Variant",
                Group = string.Empty,
                Price = 0m
            };
            _variantBinding.Add(vnew);
            Log($"Added variant {vnew.VariantId} to {product.Sku}");
            if (dataGridViewVariants.Rows.Count > 0)
            {
                var idx = dataGridViewVariants.Rows.Count - 1;
                dataGridViewVariants.ClearSelection();
                dataGridViewVariants.Rows[idx].Selected = true;
                dataGridViewVariants.CurrentCell = dataGridViewVariants.Rows[idx].Cells[1];
                dataGridViewVariants.BeginEdit(true);
            }
        }

        private void btnRemoveVariant_Click(object sender, EventArgs e)
        {
            var product = GetSelectedProduct();
            if (product == null || _variantBinding == null) return;
            if (dataGridViewVariants.CurrentRow == null) return;
            var row = dataGridViewVariants.CurrentRow;
            var variant = row.DataBoundItem as Variant;
            if (variant == null) return;
            var res = MessageBox.Show(this, $"Remove variant [{variant.VariantId}] {variant.Name}?", "Remove Variant", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res != DialogResult.Yes) return;
            _variantBinding.Remove(variant);
            Log($"Removed variant {variant.VariantId} from {product.Sku}");
        }

        private Product GetSelectedProduct()
        {
            if (dataGridViewProducts.SelectedRows.Count == 1)
            {
                return dataGridViewProducts.SelectedRows[0].DataBoundItem as Product;
            }
            return null;
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Optional: brand the updater window
                AutoUpdater.AppTitle = Text;
                AutoUpdater.ReportErrors = true;
                AutoUpdater.Synchronous = false;
                AutoUpdater.ShowSkipButton = false;
                AutoUpdater.RunUpdateAsAdmin = false;

                // If you serve over HTTPS with a self-signed cert, you may need additional config.
                AutoUpdater.Start(UpdateXmlUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Update Check", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
