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
    public partial class SupplierList : Form
    {
        #region Variable

        POSEntities entity = new POSEntities();

        #endregion

        #region Event

        public SupplierList()
        {
            InitializeComponent();
            dgvSupplierList.AutoGenerateColumns = false;
        }

        private void SupplierList_Load(object sender, EventArgs e)
        {
            DataBind();
        }

        private void dgvSupplierList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex >= 0)
            {
                int currentSupplierId = Convert.ToInt32(dgvSupplierList.Rows[e.RowIndex].Cells[0].Value);
                string currentSupplierName = dgvSupplierList.Rows[e.RowIndex].Cells[1].Value.ToString();
                int previousBalance = Convert.ToInt32(dgvSupplierList.Rows[e.RowIndex].Cells[2].Value);

                if (e.ColumnIndex == 3)
                {
                    PurchaseListBySupplier newForm = new PurchaseListBySupplier();
                    newForm.supplierId = currentSupplierId;
                    newForm.supplierName = currentSupplierName;
                    newForm.ShowDialog();
                }
                else if (e.ColumnIndex == 4)
                {
                    SupplierInformation newForm = new SupplierInformation();
                    newForm.supplierId = currentSupplierId;
                    newForm.OldCreditAmount = previousBalance;
                    newForm.ShowDialog();
                }
                else if (e.ColumnIndex == 5)
                {
                    NewSupplier newForm = new NewSupplier();
                    newForm.isEdit = true;
                    newForm.SupplierId = currentSupplierId;
                    newForm.ShowDialog();
                }
                else if (e.ColumnIndex == 6)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {
                        DataGridViewRow row = dgvSupplierList.Rows[e.RowIndex];
                        Supplier supplier = (Supplier)row.DataBoundItem;
                        supplier = (from s in entity.Suppliers where s.Id == supplier.Id select s).FirstOrDefault();
                        if (supplier.MainPurchases.Count > 0)
                        {
                            MessageBox.Show("This supplier is used in transaction!", "Cannot Delete");
                            return;
                        }
                        else
                        {
                            entity.Suppliers.Remove(supplier);
                            entity.SaveChanges();
                            DataBind();
                            MessageBox.Show("Successfully Deleted!", "Delete Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        #endregion        

        #region Function

        private void DataBind()
        {
            dgvSupplierList.DataSource = (from s in entity.Suppliers where s.Id != 1 select s).ToList();
        }

        #endregion

        private void dgvSupplierList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvSupplierList.Rows)
            {
                Supplier sp = (Supplier)row.DataBoundItem;
                row.Cells[0].Value = sp.Id;
                row.Cells[1].Value = sp.Name;
                
                MainPurchase mp = (from m in entity.MainPurchases where m.SupplierId == sp.Id && m.IsActive == true select m).FirstOrDefault();
                row.Cells[2].Value = (mp == null)? 0 :((mp.TotalAmount - mp.DiscountAmount)+ mp.OldCreditAmount) - mp.Cash;
            }
        }
    }
}
