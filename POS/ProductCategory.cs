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
    public partial class ProductCategory : Form
    {
        #region Variable

        POSEntities posEntity = new POSEntities();
        private ToolTip tp = new ToolTip();
        private Boolean isEdit = false;
        private int categoryId = 0;

        #endregion

        #region Event
        public ProductCategory()
        {
            InitializeComponent();
        }
        private void ProductCategory_Load(object sender, EventArgs e)
        {
            dgvProductCList.AutoGenerateColumns = false;
            dgvProductCList.DataSource = (from pType in posEntity.ProductCategories orderby pType.Id descending select pType).ToList();
            //this.txtName.GotFocus += new System.EventHandler(this.txtName_GotFocus);
            //this.txtName.LostFocus += new System.EventHandler(this.txtName_LostFocus);

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Category.Add || MemberShip.isAdmin)
            {
                tp.RemoveAll();
                tp.IsBalloon = true;
                tp.ToolTipIcon = ToolTipIcon.Error;
                tp.ToolTipTitle = "Error";
                if (txtName.Text.Trim() != string.Empty && txtName.Text.Trim() != "eg. Baby Corner")
                {
                    APP_Data.ProductCategory pCategory = new APP_Data.ProductCategory();
                    APP_Data.ProductCategory pCObj = (from pC in posEntity.ProductCategories where pC.Name == txtName.Text select pC).FirstOrDefault();
                    if (pCObj == null)
                    {
                        //New Product
                        if (!isEdit)
                        {
                            dgvProductCList.DataSource = "";
                            pCategory.Name = txtName.Text;
                            posEntity.ProductCategories.Add(pCategory);
                            posEntity.SaveChanges();
                        }
                        else
                        {
                            APP_Data.ProductCategory EditCat = posEntity.ProductCategories.Where(x => x.Id == categoryId).FirstOrDefault();
                            EditCat.Name = txtName.Text.Trim();
                            posEntity.SaveChanges();
                        }

                        dgvProductCList.DataSource = (from pType in posEntity.ProductCategories orderby pType.Id descending select pType).ToList();
                        MessageBox.Show("Successfully Saved!", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        #region active new Product
                        if (System.Windows.Forms.Application.OpenForms["NewProduct"] != null)
                        {
                            NewProduct newForm = (NewProduct)System.Windows.Forms.Application.OpenForms["NewProduct"];
                            newForm.ReloadCategory();
                        }
                        #endregion
                    }
                    else
                    {
                        tp.SetToolTip(txtName, "Error");
                        tp.Show("This category name is already exist!", txtName);
                    }
                }
                else
                {
                    tp.SetToolTip(txtName, "Error");
                    tp.Show("Please fill up product category name!", txtName);
                }
                txtName.Text = "eg. Baby Corner";
            }
            else
            {
                MessageBox.Show("You are not allowed to add new category", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void dgvProductCList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int currentId;
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 3)
                { //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.Category.EditOrDelete || MemberShip.isAdmin)
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {
                            DataGridViewRow row = dgvProductCList.Rows[e.RowIndex];
                            currentId = Convert.ToInt32(row.Cells[0].Value);
                            int subCatCount = posEntity.ProductSubCategories.Where(x => x.ProductCategoryId == currentId).Count();

                            if (subCatCount > 0)
                            {
                                MessageBox.Show("Please delete this product's sub category first!", "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            int count = (from p in posEntity.Products where p.ProductCategoryId == currentId select p).ToList().Count;
                            if (count < 1)
                            {
                                dgvProductCList.DataSource = "";
                                APP_Data.ProductCategory pC = (from pCat in posEntity.ProductCategories where pCat.Id == currentId select pCat).FirstOrDefault();
                                posEntity.ProductCategories.Remove(pC);
                                posEntity.SaveChanges();
                                dgvProductCList.DataSource = (from pt in posEntity.ProductCategories select pt).ToList();
                            }
                            else
                            {
                                //To show message box 
                                MessageBox.Show("This product category is currently in use!", "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to delete category", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else if(e.ColumnIndex == 2)
                {
                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.Category.EditOrDelete || MemberShip.isAdmin)
                    {

                        DataGridViewRow row = dgvProductCList.Rows[e.RowIndex];
                        currentId = Convert.ToInt32(row.Cells[0].Value);

                        APP_Data.ProductCategory pC = (from pCat in posEntity.ProductCategories where pCat.Id == currentId select pCat).FirstOrDefault();
                        txtName.Text = pC.Name;
                        isEdit = true;
                        categoryId = pC.Id;
                        btnAdd.Image = Properties.Resources.save_small;
                        this.Text = "Edit Segment";
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to edit category", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void ProductCategory_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtName);
        }

        private void txtName_Click(object sender, EventArgs e)
        {
            txtName.Text = "";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            isEdit = false;
            categoryId = 0;
            txtName.Text = string.Empty;
            btnAdd.Image = Properties.Resources.add_small;
            this.Text = "Segment";
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }

         //protected void txtName_GotFocus(object sender, EventArgs e)
         //{
         //    txtName.Text = "Baby Corner";
         //}

         //protected void txtName_LostFocus(object sender, EventArgs e)
         //{
         //    txtName.Text = "eg. Baby Corner";
         //}
        #endregion                                        
    }
}
