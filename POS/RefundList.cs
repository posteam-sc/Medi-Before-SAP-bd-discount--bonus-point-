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
    public partial class RefundList : Form
    {
        #region Variables

        private POSEntities entity = new POSEntities();
        public string transactionId;

        #endregion

        #region Event

        public RefundList()
        {
            InitializeComponent();
        }

        private void RefundList_Load(object sender, EventArgs e)
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

        private void dgvRefundList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvRefundList.Rows)
            {
                Transaction currentt = (Transaction)row.DataBoundItem;
                row.Cells[0].Value = currentt.Id;
                row.Cells[1].Value = currentt.DateTime.ToString("dd-MM-yyyy");
                row.Cells[2].Value = currentt.DateTime.ToString("hh:mm");
                row.Cells[3].Value = currentt.User.Name;
                row.Cells[4].Value = currentt.TotalAmount - currentt.DiscountAmount;
            }
        }

        private void dgvRefundList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string currentTransactionId = dgvRefundList.Rows[e.RowIndex].Cells[0].Value.ToString();
                //if (e.ColumnIndex == 5)
                //{ 
                //    //to print
                //}
                if (e.ColumnIndex == 5)
                {
                    RefundDetail newForm = new RefundDetail();
                    newForm.transactionId = currentTransactionId;
                    newForm.IsRefund = false;
                    newForm.ShowDialog();
                }
                //Delete
                else if (e.ColumnIndex == 6)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {
                        Transaction DeleteTransactionObj = (from t in entity.Transactions where t.Id == currentTransactionId select t).FirstOrDefault();
                        DeleteTransactionObj.IsDeleted = true;
                        DeleteTransactionObj.IsActive = false;

                        DeleteLog dl = new DeleteLog();
                        dl.DeletedDate = DateTime.Now;
                        dl.CounterId = MemberShip.CounterId;
                        dl.UserId = MemberShip.UserId;
                        dl.IsParent = true;
                        dl.TransactionId = DeleteTransactionObj.Id;

                        entity.DeleteLogs.Add(dl);

                        foreach (TransactionDetail td in DeleteTransactionObj.TransactionDetails)
                        {
                            td.Product.Qty = td.Product.Qty - td.Qty;

                            if (td.Product.IsWrapper == true)
                            {
                                List<WrapperItem> wplist = td.Product.WrapperItems.ToList();
                                foreach (WrapperItem wp in wplist)
                                {
                                    wp.Product1.Qty = wp.Product1.Qty - td.Qty;
                                }
                            }
                        }
                        entity.SaveChanges();
                        LoadData();
                    }
                }
            }
        }

        #endregion

        #region Function

        private void LoadData()
        {
            DateTime fromDate = dtpFrom.Value.Date;
            DateTime toDate = dtpTo.Value.Date;
            dgvRefundList.AutoGenerateColumns = false;
            if (transactionId == null)
            {
                //List<Transaction> transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true select t).ToList<Transaction>();
                List<Transaction> transactionList = (from t in entity.Transactions where (EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsActive != false) && (t.Type == TransactionType.Refund || t.Type == TransactionType.CreditRefund) select t).ToList().Where(x => x.IsDeleted != true).ToList<Transaction>();
                dgvRefundList.DataSource = transactionList;
            }
            else
            {
                //List<Transaction> transactionList = (from t in entity.Transactions where (EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsActive != false && t.ParentId == transactionId) && (t.Type == TransactionType.Refund || t.Type == TransactionType.CreditRefund) select t).ToList<Transaction>();
                List<Transaction> transactionList = (from t in entity.Transactions where (t.IsActive != false && t.ParentId == transactionId) && (t.Type == TransactionType.Refund || t.Type == TransactionType.CreditRefund) select t).ToList().Where(x => x.IsDeleted != true).ToList<Transaction>();
                dgvRefundList.DataSource = transactionList;
            }
        }

        #endregion




    }
}
