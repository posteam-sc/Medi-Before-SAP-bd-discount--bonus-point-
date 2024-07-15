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
    public partial class CustomerSearch : Form
    {
        #region Variables

        private POSEntities entity = new POSEntities();

        private ToolTip tp = new ToolTip();

        #endregion

        #region Event

        public CustomerSearch()
        {
            InitializeComponent();
        }

        private void CustomerSearch_Load(object sender, EventArgs e)
        {
            List<APP_Data.CustomerType> customerTypeList = new List<APP_Data.CustomerType>();
            APP_Data.CustomerType ctype = new APP_Data.CustomerType();
            ctype.Id = 0;
            ctype.TypeName = "All";
            customerTypeList.Add(ctype);
            customerTypeList.AddRange(entity.CustomerTypes.ToList());

            cboCustomerType.DataSource = customerTypeList;
            cboCustomerType.DisplayMember = "TypeName";
            cboCustomerType.ValueMember = "Id";

            dgvCustomerList.AutoGenerateColumns = false;
        }       

        private void btnSearch_Click(object sender, EventArgs e)
        {
            List<APP_Data.Customer> customerList = entity.Customers.ToList();
            if (rdoCustomerName.Checked)
            {
                customerList = customerList.Where(x=>x.Name.ToLower().Contains(textBox1.Text.Trim().ToLower())).ToList();
            }
            else if (rdoEmail.Checked)
            {
                customerList = customerList.Where(x => x.Email.Contains(textBox1.Text.Trim())).ToList();
            }            
            else if (rdoNIRC.Checked)
            {
                customerList = customerList.Where(x => x.NRC.Contains(textBox1.Text.Trim())).ToList();
            }
            //Search By Phone Number
            else
            {
                customerList = customerList.Where(x => x.PhoneNumber.Contains(textBox1.Text.Trim())).ToList();
            }

            //Filter By Customer Type
            if (cboCustomerType.SelectedIndex != 0)
            {
                int CustomerTypeId = Convert.ToInt32(cboCustomerType.SelectedValue);
                customerList = customerList.Where(x => x.CustomerTypeId == CustomerTypeId).ToList();
            }

            dgvCustomerList.DataSource = customerList;

        }

        private void rdoCustomerName_CheckedChanged(object sender, EventArgs e)
        {
            lblSearchTitle.Text = "Customer Name";
        }

        private void rdoPhoneNumber_CheckedChanged(object sender, EventArgs e)
        {
            lblSearchTitle.Text = "Phone Number";
        }

        private void rdoNIRC_CheckedChanged(object sender, EventArgs e)
        {
            lblSearchTitle.Text = "NIRC";
        }

        private void rdoEmail_CheckedChanged(object sender, EventArgs e)
        {
            lblSearchTitle.Text = "Email";
        }

        private void dgvCustomerList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {                
                //Use this customer
                if (e.ColumnIndex == 5)
                {
                    if (System.Windows.Forms.Application.OpenForms["Loc_GiftCardRegister"] != null)
                    {
                        Loc_GiftCardRegister newForm = (Loc_GiftCardRegister)System.Windows.Forms.Application.OpenForms["Loc_GiftCardRegister"];
                        newForm.SetCurrentCustomer(Convert.ToInt32(dgvCustomerList.Rows[e.RowIndex].Cells[0].Value));
                        this.Dispose();
                    }
                    else if (System.Windows.Forms.Application.OpenForms["TransactionDetailForm"] != null)
                    {
                        TransactionDetailForm newForm = (TransactionDetailForm)System.Windows.Forms.Application.OpenForms["TransactionDetailForm"];
                        newForm.SetCurrentCustomer(Convert.ToInt32(dgvCustomerList.Rows[e.RowIndex].Cells[0].Value));
                        this.Dispose();
                    }
                    else if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                    {
                        Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
                        newForm.SetCurrentCustomer(Convert.ToInt32(dgvCustomerList.Rows[e.RowIndex].Cells[0].Value));
                        this.Dispose();
                    }
                    
                }                
            }
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


        #endregion
    }
}
