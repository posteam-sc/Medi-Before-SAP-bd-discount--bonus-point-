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
    public partial class Loc_CustomerDetail : Form
    {
        #region Variables

        POSEntities entity = new POSEntities();
        public int customerId;

        #endregion

        #region Events

        public Loc_CustomerDetail()
        {
            InitializeComponent();
        }

        private void CustomerDetail_Load(object sender, EventArgs e)
        {
            Customer cust = (from c in entity.Customers where c.Id == customerId select c).FirstOrDefault<Customer>();

            lblName.Text = cust.Title + " " + cust.Name;
            lblPhoneNumber.Text = cust.PhoneNumber;
            lblNrc.Text = cust.NRC;
            lblAddress.Text = cust.Address;
            lblEmail.Text = cust.Email;
            lblGender.Text = cust.Gender;
            string shopcode = cust.CustomerCode.Substring(2, 2).ToString();
            string vipshop = entity.Shops.Where(x => x.ShortCode == shopcode).Select(x => x.ShopName).FirstOrDefault();
            if (vipshop != null && cust.CustomerTypeId == 1)
            {
                label16.Visible = true;
                memstartedin.Visible = true;
                memstartedin.Text = vipshop;
            }
            else
            {
                label16.Visible = false;
                memstartedin.Visible = false;
            }
            lblBirthday.Text = cust.Birthday != null ? Convert.ToDateTime(cust.Birthday).ToString("dd-MM-yyyy") : "-";
            lblCity.Text = cust.City != null ? cust.City.CityName : "-";

            //Loyalty Program
            lblTotalPoint.Text = Loc_CustomerPointSystem.GetPointFromCustomerId(cust.Id).ToString();

            //Calculate redeem point for current year(eg. from 1-April-2014  to 31-March-2015)--zmh
            #region Calculate Current Redeem Point
            lblCurrentRedeemedPoint.Text = "0";
            lblOldReedemPoint.Text = "0";
            int currentRedeemPoint = 0;

            List<Loc_PointRedeemHistory> plist = cust.Loc_PointRedeemHistory.ToList();
            if (plist.Count > 0)
            {
                //if (DateTime.Now.Month < 4)
                if (DateTime.Now.Year == 2024 && DateTime.Now.Month < 7) // Updated by Lele 2024-Mar-11, extend Pts expired date from 2023-Apr-01 to 2024-May-31  
                {
                    foreach (Loc_PointRedeemHistory po in plist)
                    {

                        if (po.DateTime.Year == ((DateTime.Now.Year - 1)) && po.DateTime.Month > 3)
                        {
                            currentRedeemPoint += po.RedeemPoint;
                        }
                        //if (po.DateTime.Year == (DateTime.Now.Year) && po.DateTime.Month < 4)
                        if (po.DateTime.Year == (DateTime.Now.Year) && po.DateTime.Month < 7) // Updated by Lele 2024-Mar-11, extend Pts expired date from 2023-Apr-01 to 2024-May-31
                        {
                            currentRedeemPoint += po.RedeemPoint;
                        }
                    }
                }
                else
                {
                    foreach (Loc_PointRedeemHistory po in plist)
                    {
                        if (po.DateTime.Year == ((DateTime.Now.Year)) && po.DateTime.Month > 3)
                        {
                            currentRedeemPoint += po.RedeemPoint;
                        }
                        if (po.DateTime.Year == (DateTime.Now.Year + 1) && po.DateTime.Month < 4)
                        {
                            currentRedeemPoint += po.RedeemPoint;
                        }
                    }
                }
                lblCurrentRedeemedPoint.Text = currentRedeemPoint.ToString();
            }
            #endregion

            //Calculate redeem point for last  year(eg.  from 1-April-2013  to 31-March-2014 , current year is 2015)
            #region Calculate redeem point for last year
            int oldReedemPoint = 0;

            List<Loc_PointRedeemHistory> oplist = cust.Loc_PointRedeemHistory.ToList();
            if (oplist.Count > 0)
            {
                if (DateTime.Now.Month < 4)
                {
                    foreach (Loc_PointRedeemHistory po in oplist)
                    {

                        if (po.DateTime.Year == ((DateTime.Now.Year - 2)) && po.DateTime.Month > 3)
                        {
                            oldReedemPoint += po.RedeemPoint;
                        }
                        if (po.DateTime.Year == (DateTime.Now.Year - 1) && po.DateTime.Month < 4)
                        {
                            oldReedemPoint += po.RedeemPoint;
                        }
                    }
                }
                else
                {
                    foreach (Loc_PointRedeemHistory po in oplist)
                    {
                        if (po.DateTime.Year == ((DateTime.Now.Year - 1)) && po.DateTime.Month > 3)
                        {
                            oldReedemPoint += po.RedeemPoint;
                        }
                        if (po.DateTime.Year == (DateTime.Now.Year) && po.DateTime.Month < 4)
                        {
                            oldReedemPoint += po.RedeemPoint;
                        }
                    }
                }
                lblOldReedemPoint.Text = oldReedemPoint.ToString();
            }

            #endregion


            //Calculate avaliable point for last  year(eg.  from 1-April-2013  to 31-March-2014, current year is 2015)
            #region Calculate Avaliable Point for last year

            long totalSale = 0; long FirstVIPInvoiceTotal = 0; int point = 0;
            //Only Calculate point for transactions which user allowed to point
            List<Transaction> tlist = cust.Transactions.Where(x => x.Loc_IsCalculatePoint == true && x.IsDeleted == false && x.IsComplete == true).ToList();
            Transaction tranObj = new Transaction();
            //If current customer's promoted date is null,
            //customer must be VIP from the very start
            if (cust.PromoteDate != null)
            {
                tlist = tlist.Where(x => x.DateTime >= cust.PromoteDate).ToList();
            }
            //All bonus points must be redeemed on/before 31 March of every year.

            tlist = tlist.Where(x => (x.DateTime.Month > 3 && x.DateTime.Year == (DateTime.Now.Year - 1)) || (x.DateTime.Month < 4 && x.DateTime.Year == DateTime.Now.Year)).ToList();

            if (cust.VIPMemberRule != null)
            {
                if (cust.VIPMemberRule.IsCalculatePoints == false)
                {
                    tranObj = tlist.Where(x => x.PaymentTypeId != 3 && x.PaymentTypeId != 4 && x.Type != TransactionType.Refund).FirstOrDefault();
                }
            }

            if (tranObj != null)
            {
                if (cust.PromoteDate != null)
                {
                    if (tranObj.DateTime.Date == cust.PromoteDate.Value.Date)
                    {
                        FirstVIPInvoiceTotal = Convert.ToInt32(tranObj.TotalAmount);
                    }
                }
            }
            //Exclude gift card payment for now
            //Also Exclude RefundTransactions
            //FOC Shouldn't include too


            totalSale = (long)tlist.Where(x => x.PaymentTypeId != 3 && x.PaymentTypeId != 4 && x.Type != TransactionType.Refund).Sum(x => x.TotalAmount);
            //Reduce refund transactions amount
            totalSale = totalSale - (long)(tlist.Where(x => x.Type == TransactionType.Refund).Sum(x => x.TotalAmount));

            //Giftcard payment are not point calculatable,
            //Just extra amount from giftcard payment are able to convert to point
            totalSale -= FirstVIPInvoiceTotal;
            //Every 10,000 Kyats spent at shop earns you 1 point
            point = (int)totalSale / 10000;

            lblAvailablePoint.Text = point.ToString();

            lblDateDuration.Text = "From  01-April-" + (DateTime.Now.Year - 1).ToString() + "  To   " + "31-March-" + DateTime.Now.Year.ToString();

            #endregion


            dgvNormalTransaction.AutoGenerateColumns = false;
            dgvVIPTransaction.AutoGenerateColumns = false;
            dgvRedeemHistory.AutoGenerateColumns = false;
            dgvGiftCards.AutoGenerateColumns = false;
            dgvUsedGiftCards.AutoGenerateColumns = false;
            List<Transaction> transList = cust.Transactions.Where(trans => trans.IsDeleted == false && trans.IsComplete == true).OrderByDescending(trans => trans.DateTime).ToList();

            if (cust.PromoteDate != null)
            {
                //Transactions after becoming VIP
                DateTime PromoteDate = (DateTime)cust.PromoteDate;
                try
                {
                    dgvVIPTransaction.DataSource = transList.Where(x => x.DateTime.Date >= PromoteDate.Date).ToList();

                }
                catch
                {
                }
                //Transactions before becoming VIP
                dgvNormalTransaction.DataSource = transList.Where(x => x.DateTime.Date < PromoteDate.Date).ToList();
            }
            //Customer is VIP and he/she didn't have non vip record
            else if (cust.CustomerTypeId == 1)
            {
                dgvVIPTransaction.DataSource = transList;
            }
            //Customer is Non VIP
            else
            {
                dgvNormalTransaction.DataSource = transList;
            }

            //Redeem History
            dgvRedeemHistory.DataSource = cust.Loc_PointRedeemHistory.Distinct().ToList();
            //GiftCards
            dgvGiftCards.DataSource = cust.GiftCards.Where(x => x.IsUsed == false && (x.IsDeleted == null || x.IsDeleted == false)).ToList();
            dgvUsedGiftCards.DataSource = cust.GiftCards.Where(x => x.IsUsed == true).ToList();
        }

        private void dgvVIPTransaction_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvVIPTransaction.Rows)
            {
                Transaction ts = (Transaction)row.DataBoundItem;
                row.Cells[0].Value = ts.Id;
                row.Cells[1].Value = ts.PaymentType.Name;
                row.Cells[2].Value = ts.DateTime.ToString("dd-MM-yyyy");
                row.Cells[3].Value = ts.DateTime.ToString("hh:mm");
                row.Cells[4].Value = ts.User.Name;
                row.Cells[5].Value = ts.TotalAmount;
                row.Cells[6].Value = ts.Type;

            }
        }

        private void dgvOldTransaction_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvNormalTransaction.Rows)
            {
                Transaction ts = (Transaction)row.DataBoundItem;
                row.Cells[0].Value = ts.Id;
                row.Cells[1].Value = ts.PaymentType.Name;
                row.Cells[2].Value = ts.DateTime.ToString("dd-MM-yyyy");
                row.Cells[3].Value = ts.DateTime.ToString("hh:mm");
                row.Cells[4].Value = ts.User.Name;
                row.Cells[5].Value = ts.TotalAmount;
                row.Cells[7].Value = ts.Type;
            }
        }

        private void dgvRedeemHistory_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvRedeemHistory.Rows)
            {
                Loc_PointRedeemHistory redeemHistory = (Loc_PointRedeemHistory)row.DataBoundItem;
                row.Cells[0].Value = redeemHistory.DateTime.ToString("dd-MM-yyyy");
                row.Cells[1].Value = redeemHistory.DateTime.ToString("hh:mm");
                row.Cells[2].Value = redeemHistory.Counter.Name;
                row.Cells[3].Value = redeemHistory.User.Name;
            }
        }

        private void dgvGiftCards_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvGiftCards.Rows)
            {
                GiftCard gInC = (GiftCard)row.DataBoundItem;
                row.Cells[0].Value = gInC.CardNumber;
                row.Cells[1].Value = gInC.Amount;
                row.Cells[2].Value = gInC.ExpireDate != null ? Convert.ToDateTime(gInC.ExpireDate).ToString("dd-MM-yyyy") : "-";
            }
        }

        private void dgvVIPTransaction_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string currentTransactionId = dgvVIPTransaction.Rows[e.RowIndex].Cells[0].Value.ToString();
                string transactionType = dgvVIPTransaction.Rows[e.RowIndex].Cells[6].Value.ToString();
                if (e.ColumnIndex == 7)
                {
                    if (transactionType == TransactionType.Sale)
                    {

                        TransactionDetailForm newForm = new TransactionDetailForm();
                        newForm.transactionId = currentTransactionId;
                        newForm.ShowDialog();
                    }
                    else if (transactionType == TransactionType.Refund)
                    {
                        RefundDetail newForm = new RefundDetail();
                        newForm.transactionId = currentTransactionId;
                        newForm.ShowDialog();
                    }
                }
            }
        }

        private void dgvNormalTransaction_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string currentTransactionId = dgvNormalTransaction.Rows[e.RowIndex].Cells[0].Value.ToString();
                if (e.ColumnIndex == 6)
                {
                    TransactionDetailForm newForm = new TransactionDetailForm();
                    newForm.transactionId = currentTransactionId;
                    newForm.ShowDialog();
                }
            }
        }

        private void btnRedeemPoint_Click(object sender, EventArgs e)
        {
            Loc_RedeemPoint newForm = new Loc_RedeemPoint();
            newForm.customerId = customerId;
            newForm.Show();
        }

        private void btnRegisterGiftCards_Click(object sender, EventArgs e)
        {
            Loc_GiftCardRegister giftCardRegister = new Loc_GiftCardRegister();
            giftCardRegister.CurrentCustomerId = customerId;
            giftCardRegister.ShowDialog();
        }

        private void dgvUsedGiftCards_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvUsedGiftCards.Rows)
            {
                GiftCard gInC = (GiftCard)row.DataBoundItem;
                row.Cells[0].Value = gInC.CardNumber;
                row.Cells[1].Value = gInC.Amount;
            }
        }

        #endregion

        #region Function

        public void updateCustomerPoint()
        {
            //Loyalty Program
            entity = new POSEntities();
            lblTotalPoint.Text = Loc_CustomerPointSystem.GetPointFromCustomerId(customerId).ToString();
            lblCurrentRedeemedPoint.Text = (from c in entity.Customers where c.Id == customerId select c).FirstOrDefault<Customer>().Loc_PointRedeemHistory.Sum(x => x.RedeemPoint).ToString();


            Customer cust = (from c in entity.Customers where c.Id == customerId select c).FirstOrDefault<Customer>();
            dgvRedeemHistory.DataSource = cust.Loc_PointRedeemHistory.OrderByDescending(x => x.DateTime).ToList();

        }

        public void updateAvailableGiftCards()
        {
            entity = new POSEntities();
            Customer cust = (from c in entity.Customers where c.Id == customerId select c).FirstOrDefault<Customer>();
            dgvGiftCards.DataSource = cust.GiftCards.Where(x => x.IsUsed == false).ToList();

        }

        #endregion



    }
}
