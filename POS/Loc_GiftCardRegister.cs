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
    public partial class Loc_GiftCardRegister : Form
    {
        #region Variables

        POSEntities entity = new POSEntities();

        private ToolTip tp = new ToolTip();
        public int redeempointhistory;

        private int CustomerId = 0;

        public int CurrentCustomerId = 0;
        public Boolean IsStart = false;

        #endregion

        #region Events

        public Loc_GiftCardRegister()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //Role Management   
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.GiftCard.Add || MemberShip.isAdmin)
            {

                Boolean hasError = false;
                tp.RemoveAll();
                tp.Hide(txtCardNumber);
                tp.Hide(cboCustomer);
                tp.IsBalloon = true;
                tp.ToolTipIcon = ToolTipIcon.Error;
                tp.ToolTipTitle = "Error";
                if (txtCardNumber.Text.Trim() == string.Empty)
                {
                    tp.SetToolTip(txtCardNumber, "Error");
                    tp.Show("Please fill up gift card number!", txtCardNumber);
                    hasError = true;
                }
                else if (cboCustomer.SelectedIndex == 0)
                {
                    tp.SetToolTip(cboCustomer, "Error");
                    tp.Show("Please choose the customer!", cboCustomer);
                    hasError = true;
                }

                if (!hasError)
                {                    

                    //Check if giftcard is already register
                    GiftCard giftCardObj2 = (from gC in entity.GiftCards where gC.CardNumber == txtCardNumber.Text && gC.IsDeleted==false select gC).FirstOrDefault();

                    //Check if giftcard is already register.
                    if (giftCardObj2 == null)
                    {
                        AddGiftCard();
                    }
                    //Card already register
                    else if (giftCardObj2 != null)
                    {
                        
                        if (giftCardObj2.IsUsed == true && giftCardObj2.GiftCardInTransactions.Count > 0)
                        {
                            Boolean IsAvailable = false;
                            List<Transaction> tList = new List<Transaction>();
                            foreach(GiftCardInTransaction g in giftCardObj2.GiftCardInTransactions)
                            {
                                Transaction t = entity.Transactions.Where(x => x.Id == g.TransactionId).FirstOrDefault();
                                tList.Add(t);
                            }
                            foreach (Transaction ts in tList)
                            {
                                if (ts.IsDeleted) IsAvailable = true;
                            }

                            if (IsAvailable)
                            {
                                AddGiftCard();
                            }
                            else
                            {
                                tp.SetToolTip(txtCardNumber, "Error");
                                tp.Show("This card number is already exist!", txtCardNumber);
                            }
                        }
                        else
                        {
                            tp.SetToolTip(txtCardNumber, "Error");
                            tp.Show("This card number is already exist!", txtCardNumber);
                        }
                    }
                    //Card is expire
                    else
                    {
                        tp.SetToolTip(txtCardNumber, "Error");
                        tp.Show("This gift card is expire!", txtCardNumber);
                    }
                }
            }
            else
            {
                MessageBox.Show("You are not allowed to add new giftcard", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void AddGiftCard()
        {
            GiftCard giftCardObj1 = new GiftCard();
            giftCardObj1.CardNumber = txtCardNumber.Text;
            giftCardObj1.CustomerId = CustomerId;
            if (rdo6000.Checked)
            {
                giftCardObj1.Amount = 6000;
            }
            else if (rdo12000.Checked)
            {
                giftCardObj1.Amount = 12000;
            }
            else if (rdo24000.Checked)
            {
                giftCardObj1.Amount = 24000;
            }
            else if (rdo36000.Checked)
            {
                giftCardObj1.Amount = 36000;
            }
            else if (rdo72000.Checked)
            {
                giftCardObj1.Amount = 72000;
            }
            else if (rdo120000.Checked)
            {
                giftCardObj1.Amount = 120000;
            }
            else
            {
                giftCardObj1.Amount = 20000;
            }

            giftCardObj1.ExpireDate = DateTime.Now.AddMonths(1);
            giftCardObj1.IsUsedDate = DateTime.Now;
            giftCardObj1.IsDeleted = false;

            entity.GiftCards.Add(giftCardObj1);
            entity.SaveChanges();

            if (System.Windows.Forms.Application.OpenForms["Loc_CustomerDetail"] != null)
            {
                Loc_CustomerDetail newForm = (Loc_CustomerDetail)System.Windows.Forms.Application.OpenForms["Loc_CustomerDetail"];
                newForm.updateAvailableGiftCards();
            }

            //Update Customer Point in other forms
            if (System.Windows.Forms.Application.OpenForms["Loc_CustomerDetail"] != null)
            {
                Loc_CustomerDetail newForm = (Loc_CustomerDetail)System.Windows.Forms.Application.OpenForms["Loc_CustomerDetail"];
                newForm.updateCustomerPoint();
            }


            //dgvBrandList.DataSource = (from b in posEntity.Brands orderby b.Id descending select b).ToList();
            MessageBox.Show("Successfully Saved!", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DataBind();
            txtCardNumber.Text = "";
        }

        private void Loc_GiftCardRegister_Load(object sender, EventArgs e)
        {
            Customer_Bind();

            if (CurrentCustomerId != 0)
            {
                SetCurrentCustomer(CurrentCustomerId);
            }
            if(redeempointhistory==12000)
            {
                rdo12000.Checked = true;
                
            }
            else if (redeempointhistory == 24000)
            {
                rdo24000.Checked = true;
            }
            else if (redeempointhistory == 36000)
            {
                rdo36000.Checked = true;
            }
            else if (redeempointhistory == 72000)
            {
                rdo72000.Checked = true;
            }
            else if (redeempointhistory == 120000)
            {
                rdo120000.Checked = true;
            }
            IsStart = true;
            DataBind();
        }        

        private void rdoFilter20k_CheckedChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        private void rdoFilter6k_CheckedChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        private void rdoFilterAll_CheckedChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        private void dgvGiftCardList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int currentGiftCardId = Convert.ToInt32(dgvGiftCardList.Rows[e.RowIndex].Cells[0].Value);
                int currentGiftCardAmount = Convert.ToInt32(dgvGiftCardList.Rows[e.RowIndex].Cells[2].Value);
               
                //Delete
                if (e.ColumnIndex == 4)
                {
                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.GiftCard.EditOrDelete || MemberShip.isAdmin)
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete? This card has " + currentGiftCardAmount, "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {
                            DataGridViewRow row = dgvGiftCardList.Rows[e.RowIndex];
                            GiftCard giftCardObj = (GiftCard)row.DataBoundItem;
                            bool IsAllowDelete=false;
                            var giftCardInTransaction = (from gt in entity.GiftCardInTransactions where gt.GiftCardId == giftCardObj.Id select gt).FirstOrDefault();
                            
                            if(giftCardInTransaction != null )
                            {
                                if(giftCardInTransaction.Transaction.IsDeleted == true)
                                {
                                    IsAllowDelete=true;
                                }
                                else
                                {
                                    IsAllowDelete=false;
                                }

                            }
                            else
                            {
                                  IsAllowDelete=true;
                            }
                         //   if (giftCardObj.GiftCardInTransactions.Count == 0)
                            if(IsAllowDelete == true)
                            {
                               // entity.GiftCards.Remove(giftCardObj);
                                giftCardObj.IsDeleted = true;
                                entity.Entry(giftCardObj).State = EntityState.Modified;
                                entity.SaveChanges();
                                DataBind();
                                MessageBox.Show("Successfully Deleted!", "Delete Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("The Card is already used in transaction", "Unable to Delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to delete giftcards", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void rdoFilter12k_CheckedChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        private void cboCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Customer_Bind();
            if (cboCustomer.SelectedIndex != 0)
            {
                SetCurrentCustomer(Convert.ToInt32(cboCustomer.SelectedValue.ToString()));
               
            }
            else
            {
                //Clear customer data
                CustomerId = 0;
                lblCustomerName.Text = "-";
                lblEmail.Text = "-";
                lblNRIC.Text = "-";
                lblPhoneNumber.Text = "-";
            }
                DataBind();
        }

        private void btnAddNewCustomer_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Customer.Add || MemberShip.isAdmin)
            {

                NewCustomer form = new NewCustomer();
                form.isEdit = false;
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add new customer", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void lbAdvanceSearch_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CustomerSearch form = new CustomerSearch();
            form.ShowDialog();
        }

        public void SetCurrentCustomer(Int32 CId)
        {
            CustomerId = CId;
            Customer currentCustomer = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();
            lblCustomerName.Text = currentCustomer.Name;
            lblNRIC.Text = currentCustomer.NRC;
            lblPhoneNumber.Text = currentCustomer.PhoneNumber;
            lblEmail.Text = currentCustomer.VIPMemberId;
            cboCustomer.SelectedItem = currentCustomer;
        }

        private void Loc_GiftCardRegister_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtCardNumber);
            tp.Hide(cboCustomer);
        }

        private void dgvGiftCardList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvGiftCardList.Rows)
            {
                GiftCard gc = (GiftCard)row.DataBoundItem;
                row.Cells[3].Value = gc.Customer.Name;
            }
        }

        #endregion

        #region Functions

        private void DataBind()
        {
            if (IsStart == true)
            {
                long customerId = Convert.ToInt64(cboCustomer.SelectedValue);
          
                List<GiftCard> GiftCardList = entity.GiftCards.Where(x =>( x.IsUsed != true)  && (x.IsDeleted==false) && ((customerId == 0 && 1 == 1) || (customerId > 0 && x.CustomerId == customerId)) ).Distinct().ToList();
                if (rdoFilter6k.Checked)
                {
                    GiftCardList = GiftCardList.Where(x => x.Amount == 6000).ToList();
                }
                else if (rdoFilter12k.Checked)
                {
                    GiftCardList = GiftCardList.Where(x => x.Amount == 12000).ToList();
                }
                else if (rdoFilter24k.Checked)
                {
                    GiftCardList = GiftCardList.Where(x => x.Amount == 24000).ToList();
                }
                else if (rdoFilter36k.Checked)
                {
                    GiftCardList = GiftCardList.Where(x => x.Amount == 36000).ToList();
                }
                else if (rdoFilter72k.Checked)
                {
                    GiftCardList = GiftCardList.Where(x => x.Amount == 72000).ToList();
                }
                else if (rdoFilter120k.Checked)
                {
                    GiftCardList = GiftCardList.Where(x => x.Amount == 120000).ToList();
                }
                //Filter by 20k worth giftcards
                else if (rdoFilter20k.Checked)
                {
                    GiftCardList = GiftCardList.Where(x => x.Amount == 20000).ToList();
                }

                dgvGiftCardList.AutoGenerateColumns = false;
                dgvGiftCardList.DataSource = GiftCardList;

                //Customer_Bind();

                //if (CurrentCustomerId != 0)
                //{
                //    SetCurrentCustomer(CurrentCustomerId);
                //}
            }
        }

        public void Customer_Bind()
        {     // By SYM
            //Add Customer List with default option
            List<APP_Data.Customer> customerList = new List<APP_Data.Customer>();
            APP_Data.Customer customer = new APP_Data.Customer();
            customer.Id = 0;
            customer.Name = "ALL";
            customerList.Add(customer);
          
            customerList.AddRange(entity.Customers.OrderBy(x=>x.Name).ToList());
       
           // cboCustomer.DataSource = customerList.Where (x=>x.VIPMemberId!=null || x.VIPMemberId!="");
            cboCustomer.DataSource = customerList;
            cboCustomer.DisplayMember = "Name";
            cboCustomer.ValueMember = "Id";

            //cboCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //cboCustomer.AutoCompleteSource = AutoCompleteSource.ListItems;
          
        

           
        }

        #endregion                

        private void cboCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            cboCustomer.DroppedDown = true;

        }

        private void cboCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            cboCustomer.DroppedDown = true;

        }

        private void rdoFilter24k_CheckedChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        private void rdoFilter36k_CheckedChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        private void rdoFilter72k_CheckedChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        private void rdoFilter120k_CheckedChanged(object sender, EventArgs e)
        {
            DataBind();
        }

       

       
    }
}
