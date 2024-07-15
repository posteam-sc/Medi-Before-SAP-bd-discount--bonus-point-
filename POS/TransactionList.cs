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
using System.Data.SqlClient;

namespace POS
{
    public partial class TransactionList : Form
    {
        #region Variables

        private POSEntities entity = new POSEntities();

        #endregion

        #region Event

        public TransactionList()
        {
            InitializeComponent();
        }

        private void TransactionList_Load(object sender, EventArgs e)
        {
            dgvTransactionList.AutoGenerateColumns = false;
            Counter_BInd();
            LoadData();
        }

        private void cboCounter_SelectedValueChanged(object sender, EventArgs e)
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
                //Point calculate or not

                if (e.ColumnIndex == 8)
                {
                    Boolean PointCalculate = !Convert.ToBoolean(dgvTransactionList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);

                    Transaction currentTransaction = entity.Transactions.Where(x => x.Id == currentTransactionId).FirstOrDefault();
                    currentTransaction.Loc_IsCalculatePoint = PointCalculate;
                    entity.SaveChanges();

                }
                //Refund
                else if (e.ColumnIndex == 9)
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
                else if (e.ColumnIndex == 10)
                {
                    TransactionDetailForm newForm = new TransactionDetailForm();
                    newForm.transactionId = currentTransactionId;
                    newForm.ShowDialog();
                }
                //Delete the record and add delete log
                else if (e.ColumnIndex == 11)
                {
                    Transaction ts = entity.Transactions.Where(x => x.Id == currentTransactionId).FirstOrDefault();
                    List<Transaction> rlist = new List<Transaction>();
                    if (ts.Transaction1.Count > 0)
                    {
                        rlist = ts.Transaction1.Where(x => x.IsDeleted == false).ToList();
                    }
                    if (rlist.Count > 0)
                    {
                        MessageBox.Show("This transaction already made a refund. It cannot be deleted!");
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {

                            ts.IsDeleted = true;//ZMH
                            ts.UpdatedDate = DateTime.Now;
                            if (ts.PaymentTypeId == 3)
                            {
                                List<GiftCardInTransaction> gList = entity.GiftCardInTransactions.Where(x => x.TransactionId == currentTransactionId).ToList();
                                foreach (GiftCardInTransaction gt in gList)
                                {
                                    GiftCard g = entity.GiftCards.Where(x => x.Id == gt.GiftCardId).FirstOrDefault();
                                    g.IsUsed = false;                                    
                             

                                    // By SYM
                                   // SqlCommand cmd = new SqlCommand();
                                   //tity.GiftCardInTransactions.Remove (gt);
                                   entity.SaveChanges();

                                }
                            }
                            foreach (TransactionDetail detail in ts.TransactionDetails)
                            {
                                detail.IsDeleted = true;//ZMH
                                detail.Product.Qty = detail.Product.Qty + detail.Qty;

                                if (detail.Product.IsWrapper == true)
                                {
                                    List<WrapperItem> wplist = detail.Product.WrapperItems.ToList();

                                    foreach (WrapperItem wp in wplist)
                                    {
                                        wp.Product1.Qty = wp.Product1.Qty + detail.Qty;
                                    }
                                }
                            }
                            decimal amout=0;
                            Transaction parenttransaction = entity.Transactions.Where(x => x.Id == currentTransactionId).FirstOrDefault();
                            foreach (TransactionDetail td in parenttransaction.TransactionDetails)
                            {
                                 amout+=Convert.ToDecimal( td.TotalAmount - parenttransaction.TaxAmount);
                            }
                            parenttransaction.TotalAmount = amout;
                            entity.SaveChanges();

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
                //Delete the record and copy this record
                else if (e.ColumnIndex == 12)
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
                            if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                            {
                                Sales openedForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
                                openedForm.DeleteCopy(currentTransactionId);
                                this.Dispose();
                            }
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
                row.Cells[6].Value = currentt.Counter.Name;
                row.Cells[7].Value = currentt.TotalAmount;
            }
        }

        private void dgvTransactionList_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvTransactionList.IsCurrentCellDirty)
            {
                dgvTransactionList.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        #endregion

        #region Function


        private void Counter_BInd()
        {
            List<APP_Data.Counter> counterList = new List<APP_Data.Counter>();
            APP_Data.Counter counterObj = new APP_Data.Counter();
            counterObj.Id = 0;
            counterObj.Name = "Select";
            counterList.Add(counterObj);
            counterList.AddRange((from c in entity.Counters orderby c.Id select c).ToList());
            cboCounter.DataSource = counterList;
            cboCounter.DisplayMember = "Name";
            cboCounter.ValueMember = "Id";
        }

        public void LoadData()
        {
            int CounterId = 0;
            if (cboCounter.SelectedIndex > 0)
            {
                CounterId = Convert.ToInt32(cboCounter.SelectedValue);
            }

            if (rdbDate.Checked)
            {
                DateTime fromDate = dtpFrom.Value.Date;
                DateTime toDate = dtpTo.Value.Date;
                List<Transaction> transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale  && ((CounterId==0 && 1==1) || (CounterId!=0 && t.CounterId==CounterId)) select t).ToList<Transaction>();
                dgvTransactionList.DataSource = transList.Where(x => x.IsDeleted != true).ToList();
            }
            else
            {
                string Id = txtId.Text;
                if (Id.Trim() != string.Empty)
                {
                    List<Transaction> transList = (from t in entity.Transactions where t.Id == Id select t).ToList().Where(x => x.IsDeleted != true && ((CounterId == 0 && 1 == 1) || (CounterId != 0 && x.CounterId == CounterId))).ToList();
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
    }
}
