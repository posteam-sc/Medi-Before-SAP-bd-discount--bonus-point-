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
    public partial class PurchaseDetailList : Form
    {
        #region Variables

        POSEntities entity = new POSEntities();
        public int mainPurchaseId;
        #endregion
        #region Event

        public PurchaseDetailList()
        {
            InitializeComponent();
        }

        private void PurchaseDetailList_Load(object sender, EventArgs e)
        {
            dgvProductList.AutoGenerateColumns = false;
            MainPurchase currentMP = (from mp in entity.MainPurchases where mp.Id == mainPurchaseId select mp).FirstOrDefault();
            lblSupplerName.Text = (currentMP.Supplier == null) ? "-" : currentMP.Supplier.Name;
            lblDate.Text = currentMP.Date.ToString();
            lblVoucherNo.Text = (currentMP.VoucherNo == null) ? "-" : currentMP.VoucherNo;
            lblTotalAmount.Text = currentMP.TotalAmount.ToString();
            lblcash.Text = currentMP.Cash.ToString();
            lblOldCredit.Text = currentMP.OldCreditAmount.ToString();
            dgvProductList.DataSource = (from pd in entity.PurchaseDetails where pd.MainPurchaseId == mainPurchaseId select pd).ToList();
        }

        private void dgvProductList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvProductList.Rows)
            {
                PurchaseDetail purchaseDetailObj = (PurchaseDetail)row.DataBoundItem;
                row.Cells[0].Value = purchaseDetailObj.Product.Name;
                row.Cells[1].Value = purchaseDetailObj.Qty;
                row.Cells[2].Value = purchaseDetailObj.UnitPrice;
            }
        }

        #endregion
        #region Function


        #endregion
    }
}
