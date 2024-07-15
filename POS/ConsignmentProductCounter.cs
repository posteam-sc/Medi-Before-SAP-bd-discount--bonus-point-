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
    public partial class ConsignmentProductCounter : Form
    {
        #region Variables

        POSEntities entity = new POSEntities();

        private ToolTip tp = new ToolTip();

        #endregion

        #region Event
        public ConsignmentProductCounter()
        {
            InitializeComponent();
        }

        private void ConsignmentProductCounter_Load(object sender, EventArgs e)
        {
            DataBind();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            bool hasError = false;
            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";

            if (txtName.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtName, "Error");
                tp.Show("Please fill up consignment counter name!", txtName);
                hasError = true;
            }
            else if (txtCounterLocation.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtCounterLocation, "Error");
                tp.Show("Please fill up counter location!", txtCounterLocation);
                hasError = true;
            }

            if (!hasError)
            {
                ConsignmentCounter consignmentCounterObj1 = new ConsignmentCounter();
                ConsignmentCounter consignmentCounterObj2 = (from c in entity.ConsignmentCounters where c.Name == txtName.Text select c).FirstOrDefault();
                if (consignmentCounterObj2 == null)
                {
                    consignmentCounterObj1.Name = txtName.Text;
                    consignmentCounterObj1.CounterLocation = txtCounterLocation.Text;
                    entity.ConsignmentCounters.Add(consignmentCounterObj1);
                    entity.SaveChanges();
                    DataBind();
                    MessageBox.Show("Successfully Saved!", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    tp.SetToolTip(txtName, "Error");
                    tp.Show("This consignment counter name is already exist!", txtName);
                }
            }
        }

        private void dgvCosignmentCounterList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int currentId;
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 3)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {
                        DataGridViewRow row = dgvCosignmentCounterList.Rows[e.RowIndex];
                        currentId = Convert.ToInt32(row.Cells[0].Value);
                        int count = (from p in entity.Products where p.ConsignmentCounterId == currentId select p).ToList().Count;
                        if (count < 1)
                        {
                            ConsignmentCounter DeleteObj = (from c in entity.ConsignmentCounters where c.Id == currentId select c).FirstOrDefault();
                            entity.ConsignmentCounters.Remove(DeleteObj);
                            entity.SaveChanges();
                            DataBind();
                            MessageBox.Show("Successfully Deleted!", "Delete Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("This consignment counter name is currently in use!", "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }
        }

        #endregion
        #region Function

        private void DataBind()
        {
            
            dgvCosignmentCounterList.DataSource = entity.ConsignmentCounters.ToList();
        }

        #endregion

    }
}
