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
    public partial class Counter : Form
    {
        #region Variables

        POSEntities posEntity = new POSEntities();
        private ToolTip tp = new ToolTip();

        #endregion

        #region Event
        public Counter()
        {
            InitializeComponent();
        }
        private void Counter_Load(object sender, EventArgs e)
        {
            dgvCounterList.AutoGenerateColumns = false;
            dgvCounterList.DataSource = (from c in posEntity.Counters select c).ToList();
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Counter.Add || MemberShip.isAdmin)
            {

                tp.RemoveAll();
                tp.IsBalloon = true;
                tp.ToolTipIcon = ToolTipIcon.Error;
                tp.ToolTipTitle = "Error";
                if (txtName.Text.Trim() != string.Empty)
                {
                    APP_Data.Counter cObj = new APP_Data.Counter();
                    APP_Data.Counter counter = (from counterobj in posEntity.Counters where counterobj.Name == txtName.Text select counterobj).FirstOrDefault();
                    if (counter == null)
                    {
                        dgvCounterList.DataSource = "";
                        cObj.Name = txtName.Text;
                        posEntity.Counters.Add(cObj);
                        posEntity.SaveChanges();
                        dgvCounterList.DataSource = (from c in posEntity.Counters select c).ToList();
                        MessageBox.Show("Successfully Saved!", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        tp.SetToolTip(txtName, "Error");
                        tp.Show("This counter name is already exist!", txtName);
                    }
                }
                else
                {
                    tp.SetToolTip(txtName, "Error");
                    tp.Show("Please fill up counter name!", txtName);
                }
                txtName.Text = "";
            }
            else
            {
                MessageBox.Show("You are not allowed to add new counter", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);     
            }
        }

        private void dgvCounterList_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            int currentId;
            List<APP_Data.Counter> cList = (from c in posEntity.Counters select c).ToList();

            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 2)
                {

                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.Counter.EditOrDelete || MemberShip.isAdmin)
                    {

                        if (cList.Count == 1)
                        {
                            MessageBox.Show("Counter table should have at least one counter!", "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                            if (result.Equals(DialogResult.OK))
                            {
                                DataGridViewRow row = dgvCounterList.Rows[e.RowIndex];
                                currentId = Convert.ToInt32(row.Cells[0].Value);
                                int count = (from t in posEntity.Transactions where t.CounterId == currentId select t).ToList().Count;
                                if (count < 1)
                                {
                                    dgvCounterList.DataSource = "";
                                    APP_Data.Counter Brand = (from c in posEntity.Counters where c.Id == currentId select c).FirstOrDefault();
                                    posEntity.Counters.Remove(Brand);
                                    posEntity.SaveChanges();
                                    dgvCounterList.DataSource = (from c in posEntity.Counters select c).ToList();
                                    MessageBox.Show("Successfully Deleted!", "Delete Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }

                                else
                                {
                                    //To show message box 
                                    MessageBox.Show("This counter name is currently in use!", "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to edit counters", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);     
                    }
                }

            }
        }

        private void Counter_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtName);
        }

        #endregion


    }
}
