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
    public partial class ProductSubCategory : Form
    {
        #region Variable

        POSEntities posEntity = new POSEntities();
        private ToolTip tp = new ToolTip();
        private Boolean isEdit = false;
        private int SubCategoryId = 0;
        private int CategoryId = 0;

        #endregion

        #region Event

        public ProductSubCategory()
        {
            InitializeComponent();
        }

        private void ProductType_Load(object sender, EventArgs e)
        {
            
            List<APP_Data.ProductCategory> pMCategoriesList = new List<APP_Data.ProductCategory>();
            APP_Data.ProductCategory pMObj = new APP_Data.ProductCategory();
            pMObj.Id = 0;
            pMObj.Name = "None";
            pMCategoriesList.Add(pMObj);
            pMCategoriesList.AddRange((from mCat in posEntity.ProductCategories select mCat).ToList());
            cboMCategory.DataSource = pMCategoriesList;
            cboMCategory.DisplayMember = "Name";
            cboMCategory.ValueMember = "Id";
            cboMCategory.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboMCategory.AutoCompleteSource = AutoCompleteSource.ListItems;
            dgvProductList.AutoGenerateColumns = false;
            dgvProductList.DataSource = (from pType in posEntity.ProductSubCategories orderby pType.Id descending select pType).ToList();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            int MainCategoryId = Convert.ToInt32(cboMCategory.SelectedValue);
            if (MainCategoryId > 0)
            {
                if (txtName.Text.Trim() != string.Empty)
                {
                    APP_Data.ProductSubCategory pTypeObj = new APP_Data.ProductSubCategory();
                    APP_Data.ProductSubCategory pSubCat = (from psCat in posEntity.ProductSubCategories where psCat.Name == txtName.Text && psCat.ProductCategoryId == MainCategoryId select psCat).FirstOrDefault();
                    if (pSubCat == null)
                    {
                        //Role Management
                        RoleManagementController controller = new RoleManagementController();
                        controller.Load(MemberShip.UserRoleId);


                        //New
                        if (!isEdit)
                        {
                            if (controller.SubCategory.Add || MemberShip.isAdmin)
                            {
                                dgvProductList.DataSource = "";
                                pTypeObj.Name = txtName.Text;
                                pTypeObj.ProductCategoryId = Int32.Parse(cboMCategory.SelectedValue.ToString());
                                posEntity.ProductSubCategories.Add(pTypeObj);
                                posEntity.SaveChanges();
                            }
                            else
                            {
                                MessageBox.Show("You are not allowed to add new sub category", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                        }
                        //Edit
                        else
                        {
                            if (controller.SubCategory.Add || MemberShip.isAdmin)
                            {
                                APP_Data.ProductSubCategory EditCat = posEntity.ProductSubCategories.Where(x => x.Id == SubCategoryId).FirstOrDefault();
                                EditCat.Name = txtName.Text.Trim();
                                EditCat.ProductCategoryId = Int32.Parse(cboMCategory.SelectedValue.ToString());
                                posEntity.SaveChanges();
                                Clear();
                            }
                            else
                            {
                                MessageBox.Show("You are not allowed to edit sub category", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                        }
                        dgvProductList.DataSource = (from pType in posEntity.ProductSubCategories orderby pType.Id descending select pType).ToList();
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
                        tp.Show("This sub category name is already exist!", txtName);
                    }
                }
                else
                {
                    tp.SetToolTip(txtName, "Error");
                    tp.Show("Please fill up product type name!", txtName);
                }
                txtName.Text = "";
            }
            else
            {
                tp.SetToolTip(cboMCategory, "Error");
                tp.Show("Main Categry cann't be empty!", cboMCategory);
            }



        }

        private void dgvProductList_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            int currentId;
            if (e.RowIndex >= 0)
            {
                //Delete
                if (e.ColumnIndex == 4)
                {
                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.SubCategory.EditOrDelete || MemberShip.isAdmin)
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {
                            DataGridViewRow row = dgvProductList.Rows[e.RowIndex];
                            currentId = Convert.ToInt32(row.Cells[0].Value);
                            int count = (from p in posEntity.Products where p.ProductSubCategoryId == currentId select p).ToList().Count;
                            if (count < 1)
                            {
                                dgvProductList.DataSource = "";
                                APP_Data.ProductSubCategory pType = (from pT in posEntity.ProductSubCategories where pT.Id == currentId select pT).FirstOrDefault();
                                posEntity.ProductSubCategories.Remove(pType);
                                posEntity.SaveChanges();
                                dgvProductList.DataSource = (from pt in posEntity.ProductSubCategories select pt).ToList();
                            }
                            else
                            {
                                //To show message box 
                                MessageBox.Show("This product type is currently in use!", "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            }
                        }

                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to delete sub category", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                //Edit
                else if (e.ColumnIndex == 3)
                {
                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.SubCategory.EditOrDelete || MemberShip.isAdmin)
                    {

                        DataGridViewRow row = dgvProductList.Rows[e.RowIndex];
                        currentId = Convert.ToInt32(row.Cells[0].Value);
                        APP_Data.ProductSubCategory pType = (from pT in posEntity.ProductSubCategories where pT.Id == currentId select pT).FirstOrDefault();
                        SubCategoryId = pType.Id;
                        txtName.Text = pType.Name;
                        cboMCategory.SelectedValue = pType.ProductCategoryId.ToString();
                        cboMCategory.Text = pType.ProductCategory.Name;
                        isEdit = true;
                        this.Text = "Edit SubSegment";
                        btnAdd.Image = Properties.Resources.save_small;
                        
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to edit sub category", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void ProductType_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtName);
            tp.Hide(cboMCategory);
        }

        private void dgvProductList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvProductList.Rows)
            {

                APP_Data.ProductSubCategory currentCategory = (APP_Data.ProductSubCategory)row.DataBoundItem;
                row.Cells[0].Value = (object)currentCategory.Id;
                row.Cells[1].Value = (object)currentCategory.ProductCategory.Name;
                row.Cells[2].Value = (object)currentCategory.Name;

            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        #endregion        

        #region Function

        private void Clear()
        {
            isEdit = false;
            SubCategoryId = 0;
            CategoryId = 0;
            txtName.Text = string.Empty;
            cboMCategory.SelectedIndex = 0;
            btnAdd.Image = Properties.Resources.add_small;
            this.Text = "SubSegment";
        }

        #endregion

    }
}
