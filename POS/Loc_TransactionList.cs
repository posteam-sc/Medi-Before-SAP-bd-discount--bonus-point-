using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.APP_Data;

namespace POS
{
    public partial class Loc_TransactionList : Form
    {
        #region Variables

        private POSEntities entity = new POSEntities();

        #endregion
        #region Function
        public void LoadData()
        {
            

            if (rdbDate.Checked)
            {
                DateTime fromDate = dtpFrom.Value.Date;
                DateTime toDate = dtpTo.Value.Date;
                List<Transaction> transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale select t).ToList<Transaction>();
                dgvTransactionList.DataSource = transList.Where(x => x.IsDeleted != true).ToList();
            }
            else
            {
                string Id = txtId.Text;
                if (Id.Trim() != string.Empty)
                {
                    List<Transaction> transList = (from t in entity.Transactions where t.Id == Id select t).ToList().Where(x => x.IsDeleted != true).ToList();
                    if (transList.Count > 0)
                    {
                        dgvTransactionList.DataSource = transList;
                    }
                    else
                    {
                        dgvTransactionList.DataSource = "";
                        MessageBox.Show("Item not found!", "Cannot find");
                    }
                }
                else
                {
                    dgvTransactionList.DataSource = "";
                }
            }
        }

        #endregion
        public Loc_TransactionList()
        {
            InitializeComponent();
        }

        private void Loc_TransactionList_Load(object sender, EventArgs e)
        {
            dgvTransactionList.AutoGenerateColumns = false;
            LoadData();
        }

        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdbDate_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbDate.Checked)
            {
                gbDate.Enabled = true;
                gbId.Enabled = false;
            }
            else
            {
                gbDate.Enabled = false;
                gbId.Enabled = true;
            }
            LoadData();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();

        }

        private void dgvTransactionList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string currentTransactionId = dgvTransactionList.Rows[e.RowIndex].Cells[0].Value.ToString();
                //Refund
                if (e.ColumnIndex == 7)
                {
                    Transaction tObj = (Transaction)dgvTransactionList.Rows[e.RowIndex].DataBoundItem;
                    if (tObj.PaymentTypeId == 4)
                    {
                        MessageBox.Show("Non Refundable!", "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        RefundTransaction newForm = new RefundTransaction();
                        newForm.transactionId = currentTransactionId;
                        newForm.ShowDialog();
                    }
                }
                //to print
                //else if (e.ColumnIndex == 8)
                //{
                //    //to print
                //}
                //View Detail
                else if (e.ColumnIndex == 8)
                {
                    TransactionDetailForm newForm = new TransactionDetailForm();
                    newForm.transactionId = currentTransactionId;
                    newForm.ShowDialog();
                }
                //Delete the record and add delete log
                else if (e.ColumnIndex == 9)
                {
                    Transaction ts = entity.Transactions.Where(x => x.Id == currentTransactionId).FirstOrDefault();
                    if (ts.Transaction1.Count > 0)
                    {
                        MessageBox.Show("This transaction already make refund. So it can't be delete!");
                    }
                    else
                    {

                        DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {

                            ts.IsDeleted = true;

                            foreach (TransactionDetail detail in ts.TransactionDetails)
                            {
                                detail.IsDeleted = false;
                                detail.Product.Qty = detail.Product.Qty + detail.Qty;
                            }

                            DeleteLog dl = new DeleteLog();
                            dl.DeletedDate = DateTime.Now;
                            dl.CounterId = MemberShip.CounterId;
                            dl.UserId = MemberShip.UserId;
                            dl.IsParent = true;
                            dl.TransactionId = ts.Id;

                            entity.DeleteLogs.Add(dl);

                            entity.SaveChanges();

                            LoadData();
                        }
                    }
                }
            }
        }

        private void dgvTransactionList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvTransactionList.Rows)
            {
                Transaction currentt = (Transaction)row.DataBoundItem;
                row.Cells[0].Value = currentt.Id;
                row.Cells[1].Value = currentt.Type;
                row.Cells[2].Value = currentt.PaymentType.Name;
                row.Cells[3].Value = currentt.DateTime.ToString("dd-MM-yyyy");
                row.Cells[4].Value = currentt.DateTime.ToString("hh:mm");
                row.Cells[5].Value = currentt.User.Name;
                row.Cells[6].Value = currentt.TotalAmount;
            }
        }

       

        private void rdbDebt_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbDebt.Checked)
            {
                gbPaymentType.Enabled = false;
            }
            
        }

        private void rdbSummary_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbSummary.Checked)
            {
                gbPaymentType.Enabled = false;
            }
        }

        private void rdbSale_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbSale.Checked)
            {
                gbPaymentType.Enabled = true;
            }
        }

        private void rdbRefund_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbRefund.Checked)
            {
                gbPaymentType.Enabled = false;
            }
        }
    }
}
