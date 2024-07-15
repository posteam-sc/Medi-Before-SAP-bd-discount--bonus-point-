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
    public partial class Brand : Form
    {
        #region Variables

        POSEntities posEntity = new POSEntities();
        private ToolTip tp = new ToolTip();

        private bool isEdit = false;
        private int BrandId = 0;

        #endregion

        #region Event
        public Brand()
        {
            InitializeComponent();
        }

        private void Brand_Load(object sender, EventArgs e)
        {
            dgvBrandList.AutoGenerateColumns = false;
            dgvBrandList.DataSource = (from b in posEntity.Brands orderby b.Id descending select b).ToList();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            if (txtName.Text.Trim() != string.Empty)
            {
                APP_Data.Brand Brand = new APP_Data.Brand();
                APP_Data.Brand bObj = (from b in posEntity.Brands where b.Name == txtName.Text select b).FirstOrDefault();
                if (bObj == null)
                {
                    dgvBrandList.DataSource = "";

                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);

                    //New Brand
                    if (!isEdit)
                    {
                        if (controller.Brand.Add || MemberShip.isAdmin)
                        {
                            Brand.Name = txtName.Text;
                            posEntity.Brands.Add(Brand);
                            posEntity.SaveChanges();
                            dgvBrandList.DataSource = (from b in posEntity.Brands orderby b.Id descending select b).ToList();

                        }
                        else
                        {
                            dgvBrandList.DataSource = (from b in posEntity.Brands orderby b.Id descending select b).ToList();
                            MessageBox.Show("You are not allowed to add new brand", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                    //Edit Current Brand
                    else
                    {
                        if (controller.Brand.EditOrDelete || MemberShip.isAdmin)
                        {

                            APP_Data.Brand EditBrand = posEntity.Brands.Where(x => x.Id == BrandId).FirstOrDefault();
                            EditBrand.Name = txtName.Text.Trim();
                            posEntity.SaveChanges();

                            dgvBrandList.DataSource = (from b in posEntity.Brands orderby b.Id descending select b).ToList();
                            Clear();
                        }
                        else
                        {
                            dgvBrandList.DataSource = (from b in posEntity.Brands orderby b.Id descending select b).ToList();
                            MessageBox.Show("You are not allowed to edit brand", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }

                    MessageBox.Show("Successfully Saved!", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    #region active new Product
                    if (System.Windows.Forms.Application.OpenForms["NewProduct"] != null)
                    {
                        NewProduct newForm = (NewProduct)System.Windows.Forms.Application.OpenForms["NewProduct"];
                        newForm.ReloadBrand();
                    }
                    #endregion
                }
                else
                {
                    tp.SetToolTip(txtName, "Error");
                    tp.Show("This brand name is already exist!", txtName);
                }
            }
            else
            {
                tp.SetToolTip(txtName, "Error");
                tp.Show("Please fill up brand name!", txtName);   
            }
            txtName.Text = "";
        }

        private void dgvBrandList_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            int currentId;
            if (e.RowIndex >= 0)
            {
                //Delete
                if (e.ColumnIndex == 3)
                {
                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.Brand.EditOrDelete || MemberShip.isAdmin)
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {
                            DataGridViewRow row = dgvBrandList.Rows[e.RowIndex];
                            currentId = Convert.ToInt32(row.Cells[0].Value);
                            int count = (from p in posEntity.Products where p.BrandId == currentId select p).ToList().Count;
                            if (count < 1)
                            {
                                dgvBrandList.DataSource = "";
                                APP_Data.Brand Brand = (from b in posEntity.Brands where b.Id == currentId select b).FirstOrDefault();
                                posEntity.Brands.Remove(Brand);
                                posEntity.SaveChanges();
                                dgvBrandList.DataSource = (from brand in posEntity.Brands select brand).ToList();
                                MessageBox.Show("Successfully Deleted!", "Delete Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                //To show message box 
                                MessageBox.Show("This brand name is currently in use!", "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to delete brand", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                //Edit
                else if (e.ColumnIndex == 2)
                {

                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.Brand.EditOrDelete || MemberShip.isAdmin)
                    {

                        DataGridViewRow row = dgvBrandList.Rows[e.RowIndex];
                        currentId = Convert.ToInt32(row.Cells[0].Value);

                        APP_Data.Brand Brand = (from b in posEntity.Brands where b.Id == currentId select b).FirstOrDefault();
                        txtName.Text = Brand.Name;
                        this.Text = "Edit Brand";
                        isEdit = true;
                        BrandId = Brand.Id;
                        btnAdd.Image = Properties.Resources.update_small;
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to edit brand", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void Brand_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtName);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        #endregion

      

        private void Clear()
        {
            isEdit = false;
            txtName.Text = string.Empty;
            BrandId = 0;
            btnAdd.Image = Properties.Resources.add_small;
            this.Text = "Add New Brand";
        }

        
    }
}
