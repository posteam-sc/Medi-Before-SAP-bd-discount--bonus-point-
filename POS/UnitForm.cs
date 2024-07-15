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
    public partial class UnitForm : Form
    {
        #region Variables

        POSEntities entity = new POSEntities();
        private ToolTip tp = new ToolTip();

        #endregion
        #region Event
        public UnitForm()
        {
            InitializeComponent();
        }

        private void Unit_Load(object sender, EventArgs e)
        {
            dgvUnitList.AutoGenerateColumns = false;
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
                tp.Show("Please fill up unit name!", txtName);
                hasError = true;
            }
            if (!hasError)
            {
                APP_Data.Unit unitObj1 = new APP_Data.Unit();
                APP_Data.Unit unitObj2 = (from u in entity.Units where u.UnitName == txtName.Text select u).FirstOrDefault();
                if (unitObj2 == null)
                {
                    unitObj1.UnitName = txtName.Text;
                    entity.Units.Add(unitObj1);
                    entity.SaveChanges();
                    DataBind();
                    MessageBox.Show("Successfully Saved!", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    #region active new Product
                    if (System.Windows.Forms.Application.OpenForms["NewProduct"] != null)
                    {
                        NewProduct newForm = (NewProduct)System.Windows.Forms.Application.OpenForms["NewProduct"];
                        newForm.ReloadUnit();
                    }
                    #endregion
                }
                else
                {
                    tp.SetToolTip(txtName, "Error");
                    tp.Show("This unit name is already exist!", txtName);
                }
                txtName.Text = "";
            }
        }

        #endregion 

        #region Function

        private void DataBind()
        {
           
            dgvUnitList.DataSource = (from u in entity.Units orderby u.Id descending select u).ToList();
        }

        #endregion

        private void dgvUnitList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int currentId;
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 2)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {
                        DataGridViewRow row = dgvUnitList.Rows[e.RowIndex];
                        currentId = Convert.ToInt32(row.Cells[0].Value);
                        int count = (from p in entity.Products where p.UnitId == currentId select p).ToList().Count;
                        if (count < 1)
                        {
                            APP_Data.Unit DeleteObj = (from u in entity.Units where u.Id == currentId select u).FirstOrDefault();
                            entity.Units.Remove(DeleteObj);
                            entity.SaveChanges();
                            DataBind();
                            MessageBox.Show("Successfully Deleted!", "Delete Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("This unit name is currently in use!", "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }
        }

        private void UnitForm_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtName);
        }
    }
}
