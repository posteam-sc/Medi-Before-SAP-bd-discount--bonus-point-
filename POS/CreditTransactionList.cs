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
    public partial class CreditTransactionList : Form
    {
        #region Variables

        private POSEntities entity = new POSEntities();
 
        #endregion

        #region Event

        public CreditTransactionList()
        {
            InitializeComponent();
        }

        private void CreditTransactionList_Load(object sender, EventArgs e)
        {
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

        private void dgvTransactionList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string currentTransactionId = dgvTransactionList.Rows[e.RowIndex].Cells[0].Value.ToString();
                //Refund
                if (e.ColumnIndex == 6)
                {
                    RefundTransaction newForm = new RefundTransaction();
                    newForm.transactionId = currentTransactionId;
                    newForm.Show();
                }
                //to print
                //else if (e.ColumnIndex == 7)
                //{
                //    //to print
                //}
                //View Detail
                else if (e.ColumnIndex == 7)
                {
                    TransactionDetailForm newForm = new TransactionDetailForm();
                    newForm.transactionId = currentTransactionId;
                    newForm.ShowDialog();
                }
                //Delete
                else if (e.ColumnIndex == 8)
                {
                    Transaction ts = entity.Transactions.Where(x => x.Id == currentTransactionId).FirstOrDefault();
                    List<Transaction> rlist = new List<Transaction>();
                    if (ts.Transaction1.Count > 0)
                    {
                        rlist = ts.Transaction1.Where(x => x.IsDeleted == false).ToList();
                    }
                    if (rlist.Count > 0)
                    {
                        MessageBox.Show("This transaction already make refund. So it can't be delete!");
                    }                
                   
                    else
                    {

                        DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {
                            ts.IsDeleted = true;
                            ts.UpdatedDate = DateTime.Now;
                            foreach (TransactionDetail detail in ts.TransactionDetails)
                            {
                                detail.IsDeleted = true;
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
                row.Cells[1].Value = currentt.DateTime.ToString("dd-MM-yyyy");
                row.Cells[2].Value = currentt.DateTime.ToString("hh:mm");
                row.Cells[3].Value = currentt.User.Name;                
                row.Cells[4].Value = (currentt.Customer == null) ? "-" : currentt.Customer.Name;
                row.Cells[5].Value = currentt.TotalAmount-currentt.UsePrePaidDebts.Sum(x=>x.UseAmount).Value- currentt.RecieveAmount;
            }
        }


        #endregion

        #region Function

        public void LoadData()
        {
            List<Transaction> transList = new List<Transaction>();
            dgvTransactionList.DataSource = "";
            DateTime fromDate = dtpFrom.Value.Date;
            DateTime toDate = dtpTo.Value.Date;            
            transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Credit select t).ToList<Transaction>();   
            dgvTransactionList.AutoGenerateColumns = false;
            dgvTransactionList.DataSource = transList.Where(x=>x.IsDeleted != true).ToList();
        }
        #endregion

        

    }
}
