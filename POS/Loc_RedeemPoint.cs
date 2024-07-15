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
    public partial class Loc_RedeemPoint : Form
    {
        #region Variables

        POSEntities entity = new POSEntities();
        public int customerId;
        private int redeemAmount = 12000;
        private int redeemPoint = 12;
 
        #endregion

        #region Events

        public Loc_RedeemPoint()
        {
            InitializeComponent();
        }

        private void Loc_RedeemPoint_Load(object sender, EventArgs e)
        {
             cboRedeemPointAmount.SelectedIndex = 0;
            Customer cust = (from c in entity.Customers where c.Id == customerId select c).FirstOrDefault<Customer>();            
            lblCustomerName.Text = cust.Title + " " + cust.Name;
            lblTotalPoint.Text = Loc_CustomerPointSystem.GetPointFromCustomerId(cust.Id).ToString();
        }

        private void btnRedeem_Click(object sender, EventArgs e)
        {

          
                int currentPoint = Loc_CustomerPointSystem.GetPointFromCustomerId(customerId);
                if (currentPoint >= redeemPoint)
                {
                    if (DialogResult.Yes == MessageBox.Show("Are you sure to redeem " + cboRedeemPointAmount.Text + " ? ", "Giftcards Register", MessageBoxButtons.YesNo))
                    {
                        Loc_PointRedeemHistory history = new Loc_PointRedeemHistory();
                        history.CustomerId = customerId;
                        history.CounterId = MemberShip.CounterId;
                        history.CasherId = MemberShip.UserId;
                        history.DateTime = DateTime.Now;
                        history.RedeemAmount = redeemAmount;
                        history.RedeemPoint = redeemPoint;
                        entity.Loc_PointRedeemHistory.Add(history);
                        entity.SaveChanges();
                        lblTotalPoint.Text = Loc_CustomerPointSystem.GetPointFromCustomerId(customerId).ToString();

                        //////Update Customer Point in other forms
                        ////if (System.Windows.Forms.Application.OpenForms["Loc_CustomerDetail"] != null)
                        ////{
                        ////    Loc_CustomerDetail newForm = (Loc_CustomerDetail)System.Windows.Forms.Application.OpenForms["Loc_CustomerDetail"];
                        ////    newForm.updateCustomerPoint();
                        ////}
                        if (System.Windows.Forms.Application.OpenForms["Loc_CustomerList"] != null)
                        {
                            Loc_CustomerList newForm = (Loc_CustomerList)System.Windows.Forms.Application.OpenForms["Loc_CustomerList"];
                            newForm.updateCustomerPoint();
                        }

                        MessageBox.Show("Redeem process completed!");

                        if (DialogResult.Yes == MessageBox.Show("Do you want to register the giftcard now?", "Giftcards Register", MessageBoxButtons.YesNo))
                        {
                            Loc_GiftCardRegister newRegisterForm = new Loc_GiftCardRegister();
                            newRegisterForm.CurrentCustomerId = customerId;
                            newRegisterForm.redeempointhistory = redeemAmount;
                            newRegisterForm.ShowDialog();
                        }
                   }
                }
                else
                {
                    MessageBox.Show("Customer does not have enough point to redeem.");
                }
            
        }

        private void cboRedeemPointAmount_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cboRedeemPointAmount.SelectedIndex)
            {
                case 0: redeemAmount = 12000; redeemPoint = 12; break;
                case 1: redeemAmount = 24000; redeemPoint = 20; break;
                case 2: redeemAmount = 36000; redeemPoint = 30; break;
                case 3: redeemAmount = 72000; redeemPoint = 50; break;
                case 4: redeemAmount = 120000; redeemPoint = 80; break;                
            }
            lblGiftAmount.Text = String.Format("{0:#,##0}", redeemAmount);
            lblTotalGiftCard.Text = "1";
            giftcertificate.Text = "Gift Certificate (" + redeemAmount + ")";
        }

        #endregion

        
    }
}
