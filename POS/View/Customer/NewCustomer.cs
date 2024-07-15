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
using System.Text.RegularExpressions;

namespace POS
{
    public partial class NewCustomer : Form
    {
        #region Variables

        public Boolean isEdit { get; set; }

        public int CustomerId { get; set; }

        private POSEntities entity = new POSEntities();

        private ToolTip tp = new ToolTip();

        #endregion

        public NewCustomer()
        {
            InitializeComponent();
        }

        private void New_Customer_Load(object sender, EventArgs e)
        {
            cboTitle.Items.Add("Mr");
            cboTitle.Items.Add("Mrs");
            cboTitle.Items.Add("Miss");
            cboTitle.Items.Add("Ms");
            cboTitle.Items.Add("U");
            cboTitle.Items.Add("Daw");
            cboTitle.Items.Add("Ko");
            cboTitle.Items.Add("Ma");
            cboTitle.SelectedIndex = 0;
            cboPreferContact.SelectedIndex = 0;
            
            List<APP_Data.City> cityList = new List<APP_Data.City>();
            APP_Data.City city1 = new APP_Data.City();
            city1.Id = 0;
            city1.CityName = "Select";
            cityList.Add(city1);
            cityList.AddRange(entity.Cities.ToList());
            cboCity.DataSource = cityList;
            cboCity.DisplayMember = "CityName";
            cboCity.ValueMember = "Id";


            


            List<APP_Data.CustomerType> customerTypeList = new List<APP_Data.CustomerType>();
            APP_Data.CustomerType ctype = new APP_Data.CustomerType();
            ctype.Id = 0;
            ctype.TypeName = "Select";
            customerTypeList.Add(ctype);
            customerTypeList.AddRange(entity.CustomerTypes.ToList());

            cboCustomerType.DataSource = customerTypeList;
            cboCustomerType.DisplayMember = "TypeName";
            cboCustomerType.ValueMember = "Id";

            plError.Visible = false;
            
            if (isEdit)
            {
                //Editing here
                Customer currentCustomer = (from c in entity.Customers where c.Id == CustomerId select c).FirstOrDefault<Customer>();
                
                txtName.Text = currentCustomer.Name;
                txtPhoneNumber.Text = currentCustomer.PhoneNumber;
                txtNRC.Text = currentCustomer.NRC;
                txtAddress.Text = currentCustomer.Address;
                txtEmail.Text = currentCustomer.Email;
                cboTitle.Text = currentCustomer.Title;
                cboCity.Text = currentCustomer.City.CityName;

                if (currentCustomer.PreferContact == null)
                {
                    cboPreferContact.SelectedIndex = 0;
                }
                else
                {
                    cboPreferContact.Text = currentCustomer.PreferContact;
                }
                //cboMemberRule.Text = currentCustomer.VIPMemberRule.RuleName;
                //txtRemark.Text = currentCustomer.VIPMemberRule.Remark;
                if (currentCustomer.CustomerType != null)
                {
                    cboCustomerType.Text = currentCustomer.CustomerType.TypeName;
                }

                if (currentCustomer.Gender == "Male")
                {
                    rdbMale.Checked = true;
                }
                else
                {
                    rdbFemale.Checked = true;
                }
                
                if (currentCustomer.Birthday == null)
                {
                    dtpBirthday.Value = DateTime.Now.Date;
                }
                else
                {
                    dtpBirthday.Value = currentCustomer.Birthday.Value.Date;
                }

                //If user is a vip, we might need to input/update vip become date
                if (currentCustomer.CustomerTypeId == 1)
                {
                    if (currentCustomer.PromoteDate != null)
                    {
                        dtpVIPDate.Value = currentCustomer.PromoteDate.Value.Date;
                    }
                    else
                    {
                        dtpVIPDate.Value = DateTime.Now;
                    }

                    txtVIPID.Text = currentCustomer.VIPMemberId;
                    cboMemberRule.Text = currentCustomer.VIPMemberRule.RuleName;
                    cboMemberRule.Enabled = true;
                    txtVIPID.Enabled = true;
                    dtpVIPDate.Enabled = true;

                }
                else
                {
                    txtVIPID.Enabled = false;
                    dtpVIPDate.Enabled = false;
                }
               

                btnSubmit.Image = POS.Properties.Resources.update_big;
            }
            else
            {
                int cityId = 0;
                cityId = SettingController.DefaultCity;                
                APP_Data.City cus2 = (from c in entity.Cities where c.Id == cityId select c).FirstOrDefault();
                cboCity.Text = cus2.CityName;
                rdbMale.Checked = true;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Boolean hasError = false;

            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            //Validation
            if (txtName.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtName, "Error");
                tp.Show("Please fill up customer name!", txtName);
                hasError = true;
            }
       
                switch (cboPreferContact.SelectedIndex)
                {
                      
                    case 0:
                        tp.SetToolTip(cboPreferContact, "Error");
                tp.Show("Please select Prefer Contact!", cboPreferContact);
                hasError = true;
                        break;
                    case 1:
                        if (txtEmail.Text.Trim() == string.Empty)
                        {
                            tp.SetToolTip(txtEmail, "Error");
                            tp.Show("Please fill up Email!", txtEmail);
                            plError.Visible = true;
                            hasError = true;
                        }
                        break;
                    case 2:
                        if (txtPhoneNumber.Text == string.Empty)
                        {
                            tp.SetToolTip(txtPhoneNumber, "Error");
                            tp.Show("Please fill up Phone No.!", txtPhoneNumber);
                            plError.Visible = true;
                            hasError = true;
                        }
                        break;
                    case 3:
                        if (txtPhoneNumber.Text == string.Empty)
                        {
                            tp.SetToolTip(txtPhoneNumber, "Error");
                            tp.Show("Please fill up Phone No.!", txtPhoneNumber);
                            plError.Visible = true;
                            hasError = true;
                        }
                        break;
                }
            

            if (txtPhoneNumber.Text.Trim() == string.Empty && txtEmail.Text.Trim() == string.Empty && txtNRC.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(plError, "Error");
                tp.Show("Please fill up one information for these three requirement!", plError);
                plError.Visible = true;
                hasError = true;
            }
            
            if (cboCity.SelectedIndex == 0)
            {
                tp.SetToolTip(cboCity, "Error");
                tp.Show("Please fill up city name!", cboCity);
                hasError = true;
            }
            if (cboCustomerType.SelectedIndex == 0)
            {
                tp.SetToolTip(cboCustomerType, "Error");
                tp.Show("Please fill up Customer Type!", cboCustomerType);
                hasError = true;
            }

            if (cboCustomerType.SelectedIndex == 1)
            {
                if (txtVIPID.Text.Trim() == string.Empty)
                {
                    tp.SetToolTip(txtVIPID, "Error");
                    tp.Show("Please fill up VIP ID!", txtVIPID);
                    hasError = true;
                }
                
            }
             if (cboCustomerType.SelectedIndex == 1)
            {
                if (cboMemberRule.SelectedIndex == 0)
                {
                    tp.SetToolTip(cboMemberRule, "Error");
                    tp.Show("Please fill up VIP Member Rule Name!", cboMemberRule);
                    hasError = true;
                }
            }
            Customer currentCustomer = new Customer();

            if (!hasError)
            {
                //Edit
                if (isEdit)
                {
                    currentCustomer = (from c in entity.Customers where c.Id == CustomerId select c).FirstOrDefault<Customer>();
                    currentCustomer.Title = cboTitle.Text;
                    currentCustomer.Name = txtName.Text;
                    currentCustomer.PhoneNumber = txtPhoneNumber.Text;
                    currentCustomer.NRC = txtNRC.Text;
                    currentCustomer.Address = txtAddress.Text;
                    currentCustomer.Email = txtEmail.Text;
                    currentCustomer.CustomerTypeId = Convert.ToInt32(cboCustomerType.SelectedValue.ToString());
               
                    if (rdbMale.Checked == true)
                    {
                        currentCustomer.Gender = "Male";
                    }
                    else
                    {
                        currentCustomer.Gender = "Female";
                    }

                    currentCustomer.Birthday = dtpBirthday.Value.Date;

                    if (cboCustomerType.Text.ToUpper() != "VIP")
                    {
                        currentCustomer.PromoteDate = null;
                        currentCustomer.VIPMemberId = null;

                    }
                    else
                    {
                        currentCustomer.PromoteDate = dtpVIPDate.Value;
                        currentCustomer.RuleId = Convert.ToInt32(cboMemberRule.SelectedValue.ToString());

                    }
                    int count = 0;
                    string VID = txtVIPID.Text;
                    count = entity.Customers.Where(x => x.VIPMemberId == VID && x.Id != currentCustomer.Id).ToList().Count;
                    if (count > 0)
                    {
                        tp.SetToolTip(txtVIPID, "Error");
                        tp.Show("This VIP ID is used other VIP menber!", txtVIPID);
                        hasError = true;
                    }
                    else
                    {
                        currentCustomer.VIPMemberId = VID;
                        if (currentCustomer.VipStartedShop == null)
                        {
                            if (VID != "")
                            {
                                currentCustomer.VipStartedShop = SettingController.DefaultShop.ShortCode;
                            }
                        }
                    }
                    currentCustomer.CityId = Convert.ToInt32(cboCity.SelectedValue.ToString());
                    currentCustomer.PreferContact = cboPreferContact.Text;
                    entity.Entry(currentCustomer).State = EntityState.Modified;
                    entity.SaveChanges();

                    MessageBox.Show("Successfully Update!", "Update");

                    #region active PaidByCreditWithPrePaidDebt
                    if (System.Windows.Forms.Application.OpenForms["PaidByCreditWithPrePaidDebt"] != null)
                    {
                        PaidByCreditWithPrePaidDebt newForm = (PaidByCreditWithPrePaidDebt)System.Windows.Forms.Application.OpenForms["PaidByCreditWithPrePaidDebt"];
                        newForm.LoadForm();
                    }
                    #endregion

                    #region active PaidByCredit
                    if (System.Windows.Forms.Application.OpenForms["PaidByCredit"] != null)
                    {
                        PaidByCredit newForm = (PaidByCredit)System.Windows.Forms.Application.OpenForms["PaidByCredit"];
                        newForm.LoadForm();
                    }
                    #endregion  


                 
                    #region active CustomerList
                    if (System.Windows.Forms.Application.OpenForms["Loc_CustomerList"] != null)
                    {
                        Loc_CustomerList newForm = (Loc_CustomerList)System.Windows.Forms.Application.OpenForms["Loc_CustomerList"];
                        newForm.LoadData();
                    }
                    #endregion


                    #region active Gift Card Control
                    if (System.Windows.Forms.Application.OpenForms["Loc_GiftCardRegister"] != null)
                    {
                        Loc_GiftCardRegister newForm = (Loc_GiftCardRegister)System.Windows.Forms.Application.OpenForms["Loc_GiftCardRegister"];
                        newForm.Customer_Bind();
                        newForm.SetCurrentCustomer(currentCustomer.Id);
                    }
                    #endregion

                    //refresh sales form's customer list
                    if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                    {
                        Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
                        newForm.ReloadCustomerList();
                        newForm.SetCurrentCustomer(currentCustomer.Id);

                    }

                    this.Dispose();
                }
                else
                {
                    int CustomerName=0;
                    CustomerName = (from p in entity.Customers where p.Name.Trim() == txtName.Text.Trim() select p).ToList().Count;
                    if (CustomerName == 0)
                    {
                        var CustomerCode = entity.CustomerAutoID(DateTime.Now, SettingController.DefaultShop.ShortCode);
                        currentCustomer.Title = cboTitle.Text;
                        currentCustomer.Name = txtName.Text;
                        currentCustomer.PhoneNumber = txtPhoneNumber.Text;
                        currentCustomer.NRC = txtNRC.Text;
                        currentCustomer.Email = txtEmail.Text;
                        currentCustomer.Address = txtAddress.Text;
                        currentCustomer.CustomerTypeId = Convert.ToInt32(cboCustomerType.SelectedValue.ToString());
                        currentCustomer.CustomerCode = CustomerCode.FirstOrDefault().ToString(); ;
                        if (rdbMale.Checked == true)
                        {
                            currentCustomer.Gender = "Male";
                        }
                        else
                        {
                            currentCustomer.Gender = "Female";
                        }

                        currentCustomer.Birthday = dtpBirthday.Value.Date;

                        if (cboCustomerType.Text.ToUpper() != "VIP")
                        {
                            currentCustomer.PromoteDate = null;
                        }
                        if (cboCustomerType.Text.ToUpper() == "VIP")
                        {
                            currentCustomer.PromoteDate = dtpVIPDate.Value;
                            currentCustomer.RuleId = Convert.ToInt32(cboMemberRule.SelectedValue.ToString());

                        }
                        int count = 0;
                        string VID = txtVIPID.Text;
                        if (VID != string.Empty)
                        {
                            count = entity.Customers.Where(x => x.VIPMemberId == VID).ToList().Count;
                        }
                        if (count > 0)
                        {
                            tp.SetToolTip(txtVIPID, "Error");
                            tp.Show("This VIP ID is used other VIP menber!", txtVIPID);
                            hasError = true;

                        }
                        else
                        {
                            currentCustomer.VIPMemberId = VID;
                            if (currentCustomer.VipStartedShop == null)
                            {
                                if (VID != "")
                                {
                                    currentCustomer.VipStartedShop = SettingController.DefaultShop.ShortCode;
                                }
                            }
                        }

                        if (!hasError)
                        {
                            currentCustomer.CityId = Convert.ToInt32(cboCity.SelectedValue.ToString());
                            currentCustomer.PreferContact = cboPreferContact.Text;
                            entity.Customers.Add(currentCustomer);
                            entity.SaveChanges();
                            MessageBox.Show("Successfully Saved!", "Save");
                            this.Dispose();
                            #region active PaidByCreditWithPrePaidDebt
                            if (System.Windows.Forms.Application.OpenForms["PaidByCreditWithPrePaidDebt"] != null)
                            {
                                PaidByCreditWithPrePaidDebt newForm = (PaidByCreditWithPrePaidDebt)System.Windows.Forms.Application.OpenForms["PaidByCreditWithPrePaidDebt"];
                                newForm.LoadForm();
                            }
                            #endregion

                            #region active CustomerList
                            if (System.Windows.Forms.Application.OpenForms["Loc_CustomerList"] != null)
                            {
                                Loc_CustomerList newForm = (Loc_CustomerList)System.Windows.Forms.Application.OpenForms["Loc_CustomerList"];
                                newForm.LoadData();
                            }
                            #endregion

                            #region active PaidByCredit
                            if (System.Windows.Forms.Application.OpenForms["PaidByCredit"] != null)
                            {
                                PaidByCredit newForm = (PaidByCredit)System.Windows.Forms.Application.OpenForms["PaidByCredit"];
                                newForm.LoadForm();
                            }
                            #endregion
                            //refresh sales form's customer list
                            if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                            {
                                Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
                                newForm.ReloadCustomerList();
                                newForm.SetCurrentCustomer(currentCustomer.Id);

                            }


                            #region active Gift Card Control
                            if (System.Windows.Forms.Application.OpenForms["Loc_GiftCardRegister"] != null)
                            {
                                Loc_GiftCardRegister newForm = (Loc_GiftCardRegister)System.Windows.Forms.Application.OpenForms["Loc_GiftCardRegister"];
                                newForm.Customer_Bind();
                                newForm.SetCurrentCustomer(currentCustomer.Id);
                            }
                            #endregion
                        }                        
                    }
                    else if (CustomerName > 0)
                    {
                        tp.SetToolTip(txtName, "Error");
                        tp.Show("This Customer Name is already exist!", txtName);
                    }

                }
                

            }
        }

        public static bool IsEmail(string s)
        {
            Regex EmailExpression = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.Compiled | RegexOptions.Singleline);


            if (!EmailExpression.IsMatch(s))
            {

                return false;

            }

            else
            {

                return true;

            }         
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            //DialogResult result = MessageBox.Show("Are you sure you want to cancel?", "Cancel", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            //if (result.Equals(DialogResult.OK))
            //{
            //    this.Dispose();
            //}
            cboTitle.Text = "Mr";
            txtAddress.Text = "";
            txtEmail.Text = "";
            txtName.Text = "";
            txtNRC.Text = "";
            txtPhoneNumber.Text = "";
            dtpBirthday.Value = DateTime.Now.Date;
            rdbMale.Checked = true;
            int cityId = 0;
            cityId = SettingController.DefaultCity;
            APP_Data.City cus2 = (from c in entity.Cities where c.Id == cityId select c).FirstOrDefault();
            cboCity.Text = cus2.CityName;
            btnSubmit.Image = POS.Properties.Resources.save_big;
        }

        private void New_Customer_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtName);
            tp.Hide(txtPhoneNumber);
            tp.Hide(plError);
            tp.Hide(txtVIPID);
            tp.Hide(txtEmail);
            tp.Hide(cboPreferContact);
        }

        private void cboCustomerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<APP_Data.VIPMemberRule> RuleList = new List<APP_Data.VIPMemberRule>();
            APP_Data.VIPMemberRule rule1 = new APP_Data.VIPMemberRule();
            rule1.Id = 0;
            rule1.RuleName = "Select";
            RuleList.Add(rule1);
            RuleList.AddRange(entity.VIPMemberRules.ToList());
            cboMemberRule.DataSource = RuleList;
            cboMemberRule.DisplayMember = "RuleName";
            cboMemberRule.ValueMember = "Id";
            if (cboCustomerType.SelectedValue.ToString() == "1")
            {
                dtpVIPDate.Enabled = true;
                txtVIPID.Enabled = true;
                cboMemberRule.Enabled = true;
                int id = 0;
                VIPMemberRule curreObj = new VIPMemberRule();
                if (SettingController.DefaultVIPMemberRule != 0)
                {
                    id = Convert.ToInt32(SettingController.DefaultVIPMemberRule);
                    curreObj = entity.VIPMemberRules.FirstOrDefault(x => x.Id == id);
                    cboMemberRule.Text = curreObj.RuleName;
                }
            }
            else
            {
                dtpVIPDate.Enabled = false;
                txtVIPID.Enabled = false;
                cboMemberRule.Enabled = false;
                txtRemark.Enabled = false;
                txtAddress.Focus();
            }
        }

        private void txtNRC_TextChanged(object sender, EventArgs e)
        {

        }

        private void cboMemberRule_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            VIPMemberRule currentrule = (VIPMemberRule)cboMemberRule.SelectedItem;
            txtRemark.Text = currentrule.Remark;
        }

        private void rdbMale_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cboCity_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        
    }
}
