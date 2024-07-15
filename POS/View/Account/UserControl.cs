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
    public partial class UserControl : Form
    {
        private POSEntities entity = new POSEntities();

        public UserControl()
        {
            InitializeComponent();
            dgvSalesPersonList.AutoGenerateColumns = false;
        }

        private void btnAddSalesPerson_Click(object sender, EventArgs e)
        {
            NewUser form = new NewUser();
            form.isEdit = false;
            form.ShowDialog();
        }

        private void UserControl_Load(object sender, EventArgs e)
        {
            dgvSalesPersonList.DataSource = entity.Users.ToList();
        }

        private void dgvSalesPersonList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvSalesPersonList.Rows)
            {                
                User user = (User)row.DataBoundItem;             
                row.Cells[3].Value = user.UserRole.RoleName;
            }
        }

        private void UserControl_Activated(object sender, EventArgs e)
        {
           
            dgvSalesPersonList.DataSource = entity.Users.ToList();
        }

        private void dgvSalesPersonList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //Delete
                if (e.ColumnIndex == 5)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {
                        DataGridViewRow row = dgvSalesPersonList.Rows[e.RowIndex];
                        User user = (User)row.DataBoundItem;
                        user = (from c in entity.Users where c.Id == user.Id select c).FirstOrDefault<User>();

                        //Need to recheck
                        if (user.Transactions.Count > 0)
                        {
                            MessageBox.Show("This user is already in use.", "Cannot Delete");
                            return;
                        }
                        else
                        {
                            entity.Users.Remove(user);
                            entity.SaveChanges();
                            dgvSalesPersonList.DataSource = entity.Users.ToList();
                        }
                    }
                }
                //Edit
                else if (e.ColumnIndex == 4)
                {
                    NewUser form = new NewUser();
                    form.isEdit = true;
                    form.Text = "Edit User";
                    form.UserId = Convert.ToInt32(dgvSalesPersonList.Rows[e.RowIndex].Cells[0].Value);
                    form.Show();
                }
            }
        }
    }
}
