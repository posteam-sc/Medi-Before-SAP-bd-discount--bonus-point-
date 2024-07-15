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
    public partial class CustomerList : Form
    {
        #region Variables

        POSEntities posEntity = new POSEntities();

        #endregion

        #region Event

        public CustomerList()
        {
            InitializeComponent();
        }

        private void CustomerList_Load(object sender, EventArgs e)
        {
            
            dgvCustomerList.AutoGenerateColumns = false;
            LoadData();
        }        

        private void rdoVIP_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdoNonVIP_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdoAll_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }        

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
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
                //View detail information of customer
                if (e.ColumnIndex == 6)
                {
                    if (System.Windows.Forms.Application.OpenForms["CustomerDetail"] != null)
                    {
                        CustomerDetail newForm = (CustomerDetail)System.Windows.Forms.Application.OpenForms["CustomerDetail"];
                        newForm.customerId = Convert.ToInt32(dgvCustomerList.Rows[e.RowIndex].Cells[0].Value);
                        newForm.ShowDialog();
                    }
                    else
                    {
                        CustomerDetail newForm = new CustomerDetail();
                        newForm.customerId = Convert.ToInt32(dgvCustomerList.Rows[e.RowIndex].Cells[0].Value);
                        newForm.ShowDialog();
                    }
                }
                //Delete this User
                else if (e.ColumnIndex == 7)
                {
                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.Customer.EditOrDelete || MemberShip.isAdmin)
                    {

                        DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {
                            DataGridViewRow row = dgvCustomerList.Rows[e.RowIndex];
                            Customer cust = (Customer)row.DataBoundItem;
                            cust = (from c in posEntity.Customers where c.Id == cust.Id select c).FirstOrDefault<Customer>();

                            //Need to recheck
                            if (cust.Transactions.Count > 0)
                            {
                                MessageBox.Show("This customer already made transactions!", "Unable to Delete");
                                return;
                            }
                            else
                            {
                                posEntity.Customers.Remove(cust);
                                posEntity.SaveChanges();
                                LoadData();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to delete customer", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            lblSearchTitle.Text = "Customer Name";
            rdoCustomerName.Checked = true;
            LoadData();
        }

        #endregion

        #region Function

        public void LoadData()
        {
            List<Customer> customerList = new List<Customer>();

            //Filter By Customer Type
            if (rdoAll.Checked)
            {
                customerList = posEntity.Customers.ToList();
            }
            else if (rdoVIP.Checked)
            {
                customerList = posEntity.Customers.Where(x => x.CustomerTypeId == 1).ToList();
            }
            else
            {
                customerList = posEntity.Customers.Where(x => x.CustomerTypeId == 2).ToList();
            }

            //User make a search
            if (txtSearch.Text.Trim() != string.Empty)
            {
                //Search BY Customer Name
                if (rdoCustomerName.Checked)
                {
                    customerList = customerList.Where(x => x.Name.Trim().ToLower().Contains(txtSearch.Text.Trim().ToLower())).ToList();
                }
                //Search By Email
                else if (rdoEmail.Checked)
                {
                    customerList = customerList.Where(x => x.Email.Contains(txtSearch.Text.Trim())).ToList();
                }
                //Search BY NIRC
                else if (rdoNIRC.Checked)
                {
                    customerList = customerList.Where(x => x.NRC.Contains(txtSearch.Text.Trim())).ToList();
                }
                //Search By Phone Number
                else
                {
                    customerList = customerList.Where(x => x.PhoneNumber.Contains(txtSearch.Text.Trim())).ToList();
                }
            }
            
            dgvCustomerList.DataSource = customerList;
        }

        #endregion 

        
    }
}
