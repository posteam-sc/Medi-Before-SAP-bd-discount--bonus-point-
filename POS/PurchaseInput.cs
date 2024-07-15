using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.APP_Data;

namespace POS
{
    public partial class PurchaseInput : Form
    {

        #region Variable

        POSEntities entity = new POSEntities();
        List<Product> PurchaseProductList = new List<Product>();
        private ToolTip tp = new ToolTip();
        private int totalmount = 0;
        private int DiscountAmount = 0;

        #endregion
        #region Event
        public PurchaseInput()
        {
            InitializeComponent();
        }

        private void PurchaseDetail_Load(object sender, EventArgs e)
        {
            dgvProductList.AutoGenerateColumns = false;
            PurchaseProductList.Clear();
            List<Product> productList = new List<Product>();
            Product productObj1 = new Product();
            productObj1.Id = 0;
            productObj1.Name = "Select";
            productList.Add(productObj1);
            productList.AddRange(entity.Products.ToList());
            cboProductName.DataSource = productList;
            cboProductName.DisplayMember = "Name";
            cboProductName.ValueMember = "Id";
            cboProductName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboProductName.AutoCompleteSource = AutoCompleteSource.ListItems;

            List<Supplier> supplierList = new List<Supplier>();
            Supplier supplierObj = new Supplier();
            supplierObj.Id = 0;
            supplierObj.Name = "Select";
            supplierList.Add(supplierObj);
            supplierList.AddRange(entity.Suppliers.ToList());
            cboSupplierName.DataSource = supplierList;
            cboSupplierName.DisplayMember = "Name";
            cboSupplierName.ValueMember = "Id";
            cboSupplierName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboSupplierName.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Product productObj = new Product();
            Boolean hasError = false;

            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            //Validation
            if (cboProductName.SelectedIndex == 0)
            {
                tp.SetToolTip(cboProductName, "Error");
                tp.Show("Please fill up product name!", cboProductName);
                hasError = true;
            }
            else if (txtQty.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtQty, "Error");
                tp.Show("Please fill up product quantity!", txtQty);
                hasError = true;
            }
            else if (txtUnitPrice.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtUnitPrice, "Error");
                tp.Show("Please fill up product unit price!", txtUnitPrice);
                hasError = true;
            }
            if (!hasError)
            {
                dgvProductList.DataSource = "";
                int productId = Convert.ToInt32(cboProductName.SelectedValue);
                productObj = (from p in entity.Products where p.Id == productId select p).FirstOrDefault();
                productObj.Qty = Convert.ToInt32(txtQty.Text.ToString());
                productObj.PurchasePrice = Convert.ToInt32(txtUnitPrice.Text.ToString());
                PurchaseProductList.Add(productObj);
                dgvProductList.DataSource = PurchaseProductList;
                totalmount += Convert.ToInt32(txtQty.Text.ToString()) * Convert.ToInt32(txtUnitPrice.Text.ToString());              
                txtTotalAmount.Text = totalmount.ToString();
                cboProductName.SelectedIndex = 0;
                txtQty.Text = "";
                txtUnitPrice.Text = "";
                cboProductName.Focus();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtCashAmount.Text = "";
            txtOldCredit.Text = "";
            txtQty.Text = "";
            txtTotalAmount.Text = "";
            txtUnitPrice.Text = "";
            txtVoucherNo.Text = "";
            txtDiscount.Text = "";
            cboSupplierName.SelectedIndex = 0;
            cboProductName.SelectedIndex = 0;
            PurchaseProductList.Clear();
            dgvProductList.DataSource = "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            MainPurchase mainPurchaseObj = new MainPurchase();
            Boolean hasError = false;

            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";

            
            if (cboSupplierName.SelectedIndex == 0)
            {
                tp.SetToolTip(cboSupplierName, "Error");
                tp.Show("Please fill up supplier name!", cboSupplierName);
                hasError = true;
            }
            else if (txtTotalAmount.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtTotalAmount, "Error");
                tp.Show("Please fill up total amount!", txtTotalAmount);
                hasError = true;
            }
            else if (txtCashAmount.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtCashAmount, "Error");
                tp.Show("Please fill up cash amount!", txtCashAmount);
                hasError = true;
            }
            else if (PurchaseProductList.Count == 0)
            {
                MessageBox.Show("Please fill up product list for purchase voucher!", "Error");
                return;
            }
            if (!hasError)
            {
                //Other purchase voucher's isActive change false for supplier

                int supplierId = Convert.ToInt32(cboSupplierName.SelectedValue);
                if (supplierId != 1)
                {
                    MainPurchase mpObj1 = (from mp in entity.MainPurchases where mp.SupplierId == supplierId && mp.IsActive == true select mp).FirstOrDefault();
                    if (mpObj1 != null)
                    {
                        mpObj1.IsActive = false;
                        entity.Entry(mpObj1).State = EntityState.Modified;
                    }
                }

                // add new mainpurchaseObj
                mainPurchaseObj.Date = dtDate.Value;
                mainPurchaseObj.VoucherNo = txtVoucherNo.Text;
                mainPurchaseObj.SupplierId = Convert.ToInt32(cboSupplierName.SelectedValue);
                mainPurchaseObj.OldCreditAmount = Convert.ToInt32(txtOldCredit.Text);
                mainPurchaseObj.TotalAmount = Convert.ToInt32(txtTotalAmount.Text);
                mainPurchaseObj.Cash = Convert.ToInt32(txtCashAmount.Text);
                mainPurchaseObj.DiscountAmount = (txtDiscount.Text == "") ? 0 : Convert.ToInt32(txtDiscount.Text);
                DiscountAmount = (txtDiscount.Text == "") ? 0 : Convert.ToInt32(txtDiscount.Text);
                mainPurchaseObj.IsActive = true;
                foreach (Product p in PurchaseProductList)
                {
                    APP_Data.PurchaseDetail purchaseDetailObj = new APP_Data.PurchaseDetail();
                    purchaseDetailObj.ProductId = p.Id;
                    purchaseDetailObj.Qty = p.Qty;
                    purchaseDetailObj.UnitPrice = Convert.ToInt32(p.PurchasePrice);
                    mainPurchaseObj.PurchaseDetails.Add(purchaseDetailObj);
                    
                }
                entity.MainPurchases.Add(mainPurchaseObj);

                // to edit product qty and purchase price
                foreach (Product p in PurchaseProductList)
                {
                    Product product = (from pr in entity.Products where pr.Id == p.Id select pr).FirstOrDefault();
                    product.Qty += p.Qty;
                    product.PurchasePrice = p.PurchasePrice;
                    entity.Entry(product).State = EntityState.Modified;
                }
                entity.SaveChanges();
                MessageBox.Show("Successfully Saved!", "Save");
                this.Dispose();
            }
        }

        private void dgvProductList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 4)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {
                        dgvProductList.DataSource = "";
                        PurchaseProductList.RemoveAt(e.RowIndex);
                        dgvProductList.DataSource = PurchaseProductList;
                    }
                }
            }
        }

        private void cboSupplierName_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboSupplierName.SelectedIndex > 0)
            {
                int supplierId = (int)cboSupplierName.SelectedValue;
                if (supplierId != 1)
                {
                    MainPurchase mainPurchaseObj = (from mp in entity.MainPurchases where mp.IsActive == true && mp.SupplierId == supplierId select mp).FirstOrDefault();
                    if (mainPurchaseObj != null)
                    {
                        txtOldCredit.Text = (((mainPurchaseObj.TotalAmount - mainPurchaseObj.DiscountAmount) + mainPurchaseObj.OldCreditAmount) - mainPurchaseObj.Cash).ToString();
                    }
                    else
                    {
                        txtOldCredit.Text = "0";
                        
                    }
                }
                else
                {
                    txtOldCredit.Text = "0";
                }
                
            }
        }

        #endregion

        
    }
}
