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
    public partial class PurchaseListBySupplier : Form
    {
        #region Variable

        public int supplierId;
        public string supplierName;
        POSEntities entity = new POSEntities();

        #endregion
        #region Event

        public PurchaseListBySupplier()
        {
            InitializeComponent();
        }

        private void PurchaseListBySupplier_Load(object sender, EventArgs e)
        {
            dgvMainPurchaseList.AutoGenerateColumns = false;
            lblSupplierName.Text = supplierName;
            DataBind();
        }

        private void dgvMainPurchaseList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 6)
                {
                    int currentMainPurchaseId = Convert.ToInt32(dgvMainPurchaseList.Rows[e.RowIndex].Cells[0].Value);
                    PurchaseDetailList newform = new PurchaseDetailList();
                    newform.mainPurchaseId = currentMainPurchaseId;
                    newform.ShowDialog();
                }
            }
        }

        #endregion

        #region Function

        private void DataBind()
        {
            dgvMainPurchaseList.DataSource = (from mp in entity.MainPurchases where mp.SupplierId == supplierId select mp).ToList();
        }
        #endregion
    }
}
