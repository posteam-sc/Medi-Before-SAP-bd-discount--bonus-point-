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
    public partial class City : Form
    {
        #region Variable

        POSEntities entity = new POSEntities();
        private ToolTip tp = new ToolTip();

        #endregion
        public City()
        {
            InitializeComponent();
        }

        private void City_Load(object sender, EventArgs e)
        {
            dgvCityList.AutoGenerateColumns = false;
            dgvCityList.DataSource = entity.Cities.ToList();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            bool HaveError = false;
            if (txtName.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtName, "Error");
                tp.Show("Please fill up brand name!", txtName);
                HaveError = true;
            }
            if (!HaveError)
            {
                string CityName = txtName.Text.Trim();
                APP_Data.City CityObj = new APP_Data.City();
                APP_Data.City alredyCityObj = entity.Cities.Where(x => x.CityName.Trim() == CityName).FirstOrDefault();
                if (alredyCityObj == null)
                {
                    dgvCityList.DataSource ="";
                    CityObj.CityName = txtName.Text;
                    entity.Cities.Add(CityObj);
                    entity.SaveChanges();
                    dgvCityList.DataSource = entity.Cities.ToList();
                    txtName.Text = "";
                    MessageBox.Show("Successfully Saved!", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    #region active setting
                    if (System.Windows.Forms.Application.OpenForms["Setting"] != null)
                    {
                        Setting newForm = (Setting)System.Windows.Forms.Application.OpenForms["Setting"];
                        newForm.ReLoadData();
                    }
                    #endregion
                }
                else
                {
                    tp.SetToolTip(txtName, "Error");
                    tp.Show("This city name is already exist!", txtName);
                }
            }
        }

        private void dgvCityList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int currentId;
            if (e.RowIndex > 0)
            {
                if (e.ColumnIndex == 2)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {
                        DataGridViewRow row = dgvCityList.Rows[e.RowIndex];
                        currentId = Convert.ToInt32(row.Cells[0].Value);
                        int count = (from Cus in entity.Customers where Cus.CityId == currentId select Cus).ToList().Count;
                        if (count < 1)
                        {
                            dgvCityList.DataSource = "";
                            APP_Data.City city = (from c in entity.Cities where c.Id == currentId select c).FirstOrDefault();
                            entity.Cities.Remove(city);
                            entity.SaveChanges();
                            dgvCityList.DataSource = entity.Cities.ToList();
                            MessageBox.Show("Successfully Deleted!", "Delete Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            //To show message box 
                            MessageBox.Show("This city name is currently in use!", "Enable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }
        }

        private void dgvCityList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
