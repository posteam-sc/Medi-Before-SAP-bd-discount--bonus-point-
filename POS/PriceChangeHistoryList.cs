using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.APP_Data;

namespace POS
{
    public partial class PriceChangeHistoryList : Form
    {
        #region Variables

        private POSEntities entity = new POSEntities();
        private bool IsCategoryId = false;
        private bool IsSubCategoryId = false;
        private bool IsBrandId = false;
        private bool Isname = false;
        private int CategoryId;
        private int subCategoryId;
        private int BrandId;
        private string name;
        private List<ProductPriceChange> transList = new List<ProductPriceChange>();
        #endregion

        public PriceChangeHistoryList()
        {
            InitializeComponent();
        }

        private void PriceChangeHistoryList_Load(object sender, EventArgs e)
        {
            dgvItemList.AutoGenerateColumns = false;
            rdbAll.Checked = true;
            gbBarCode.Enabled = false;

            List<APP_Data.Brand> BrandList = new List<APP_Data.Brand>();
            APP_Data.Brand brandObj1 = new APP_Data.Brand();
            brandObj1.Id = 0;
            brandObj1.Name = "Select";
            APP_Data.Brand brandObj2 = new APP_Data.Brand();
            brandObj2.Id = 1;
            brandObj2.Name = "None";
            BrandList.Add(brandObj1);
            BrandList.Add(brandObj2);
            BrandList.AddRange((from bList in entity.Brands select bList).ToList());
            cboBrand.DataSource = BrandList;
            cboBrand.DisplayMember = "Name";
            cboBrand.ValueMember = "Id";
            cboBrand.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboBrand.AutoCompleteSource = AutoCompleteSource.ListItems;

            List<APP_Data.ProductSubCategory> pSubCatList = new List<APP_Data.ProductSubCategory>();
            APP_Data.ProductSubCategory SubCategoryObj1 = new APP_Data.ProductSubCategory();
            SubCategoryObj1.Id = 0;
            SubCategoryObj1.Name = "Select";
            APP_Data.ProductSubCategory SubCategory2 = new APP_Data.ProductSubCategory();
            SubCategory2.Id = 1;
            SubCategory2.Name = "None";
            pSubCatList.Add(SubCategoryObj1);
            pSubCatList.Add(SubCategory2);
            //pSubCatList.AddRange((from c in entity.ProductSubCategories where c.ProductCategoryId == Convert.ToInt32(cboMainCategory.SelectedValue) select c).ToList());
            cboSubCategory.DataSource = pSubCatList;
            cboSubCategory.DisplayMember = "Name";
            cboSubCategory.ValueMember = "Id";
            cboSubCategory.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboSubCategory.AutoCompleteSource = AutoCompleteSource.ListItems;

            List<APP_Data.ProductCategory> pMainCatList = new List<APP_Data.ProductCategory>();
            APP_Data.ProductCategory MainCategoryObj1 = new APP_Data.ProductCategory();
            MainCategoryObj1.Id = 0;
            MainCategoryObj1.Name = "Select";
            APP_Data.ProductCategory MainCategoryObj2 = new APP_Data.ProductCategory();
            MainCategoryObj2.Id = 1;
            MainCategoryObj2.Name = "None";
            pMainCatList.Add(MainCategoryObj1);
            pMainCatList.Add(MainCategoryObj2);
            pMainCatList.AddRange((from MainCategory in entity.ProductCategories select MainCategory).ToList());
            cboMainCategory.DataSource = pMainCatList;
            cboMainCategory.DisplayMember = "Name";
            cboMainCategory.ValueMember = "Id";
            cboMainCategory.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboMainCategory.AutoCompleteSource = AutoCompleteSource.ListItems;

            DataBind();
        }
        #region Function
        public void LoadData()
        {
            IsCategoryId = false;
            CategoryId = 0;
            IsSubCategoryId = false;
            subCategoryId = 0;
            IsBrandId = false;
            BrandId = 0;
            Isname = false;
            name = string.Empty;
            transList.Clear();
            if (cboMainCategory.SelectedIndex > 1)
            {
                IsCategoryId = true;
                CategoryId = Convert.ToInt32(cboMainCategory.SelectedValue);
            }
            if (cboSubCategory.SelectedIndex > 0)
            {
                IsSubCategoryId = true;
                subCategoryId = Convert.ToInt32(cboSubCategory.SelectedValue);
            }
            if (cboBrand.SelectedIndex > 0)
            {
                IsBrandId = true;
                BrandId = Convert.ToInt32(cboBrand.SelectedValue);
            }
            if (txtName.Text.Trim() != string.Empty)
            {
                Isname = true;
                name = txtName.Text;
            }
            // find product code id
            if (IsCategoryId == true && IsSubCategoryId == true && IsBrandId == true && Isname == true)
            {
                if (BrandId == 0)
                {
                    if (subCategoryId == 0)
                    {
                      
                        transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId && t.Product.ProductSubCategoryId == null && t.Product.BrandId == null && t.Product.Name == name select t).ToList());

                    }
                    else
                    {
                        transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId && t.Product.ProductSubCategoryId == subCategoryId && t.Product.BrandId == null && t.Product.Name == name select t).ToList());
                    }
                }
                else
                {
                    if (subCategoryId == 0)
                    {
                        transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId && t.Product.ProductSubCategoryId == null && t.Product.BrandId == BrandId && t.Product.Name == name select t).ToList());
                    }
                    else
                    {
                        transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId && t.Product.ProductSubCategoryId == subCategoryId && t.Product.BrandId == BrandId && t.Product.Name == name select t).ToList());

                    }
                }

                foundDataBind();
            }
            else if (IsCategoryId == true && IsSubCategoryId == true && IsBrandId == true && Isname == false)
            {
                if (BrandId == 0)
                {
                    if (subCategoryId == 0)
                    {
                        transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId && t.Product.ProductSubCategoryId == null && t.Product.BrandId == null  select t).ToList());
                    }
                    else
                    {
                        transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId && t.Product.ProductSubCategoryId == subCategoryId && t.Product.BrandId == null select t).ToList());
                    }
                }
                else
                {
                    if (subCategoryId == 0)
                    {
                        transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId && t.Product.ProductSubCategoryId == null && t.Product.BrandId == BrandId select t).ToList());

                    }
                    else
                    {
                        transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId && t.Product.ProductSubCategoryId == subCategoryId && t.Product.BrandId == BrandId select t).ToList());
                    }
                }

                foundDataBind();
            }
            else if (IsCategoryId == true && IsSubCategoryId == true && IsBrandId == false && Isname == false)
            {
                if (subCategoryId == 0)
                {
                    transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId && t.Product.ProductSubCategoryId == null select t).ToList());
                }
                else
                {
                    transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId && t.Product.ProductSubCategoryId == subCategoryId select t).ToList());
                }

                foundDataBind();
            }
            else if (IsCategoryId == true && IsSubCategoryId == true && IsBrandId == false && Isname == true)
            {
                if (subCategoryId == 0)
                {
                    transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId && t.Product.ProductSubCategoryId == null && t.Product.Name == name select t).ToList());

                }
                else
                {
                    transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId && t.Product.ProductSubCategoryId == subCategoryId && t.Product.Name == name select t).ToList());
                }

                foundDataBind();
            }
            else if (IsCategoryId == false && IsSubCategoryId == false && IsBrandId == true && Isname == true)
            {
                if (BrandId == 0)
                {
                    transList.AddRange((from t in entity.ProductPriceChanges where t.Product.BrandId == null && t.Product.Name == name select t).ToList());

                }
                else
                {
                    transList.AddRange((from t in entity.ProductPriceChanges where t.Product.BrandId == BrandId && t.Product.Name == name select t).ToList());
                }

                foundDataBind();
            }
            else if (IsCategoryId == false && IsSubCategoryId == false && IsBrandId == true && Isname == false)
            {
                if (BrandId == 0)
                {
                    transList.AddRange((from t in entity.ProductPriceChanges where t.Product.BrandId == null  select t).ToList());
                }
                else
                {
                    transList.AddRange((from t in entity.ProductPriceChanges where t.Product.BrandId == BrandId select t).ToList());
                }

                foundDataBind();
            }
            else if (IsCategoryId == false && IsSubCategoryId == false && IsBrandId == false && Isname == true)
            {
                transList.AddRange((from t in entity.ProductPriceChanges where t.Product.Name == name select t).ToList());
                foundDataBind();
            }
            else if (IsCategoryId == true && IsSubCategoryId == false && IsBrandId == false && Isname == false)
            {
                transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId select t).ToList());
                foundDataBind();
            }
            else if (IsCategoryId == true && IsSubCategoryId == false && IsBrandId == true && Isname == true)
            {
                if (BrandId == 0)
                {
                    transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId && t.Product.BrandId == null && t.Product.Name == name select t).ToList());

                }
                else
                {
                    transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId && t.Product.BrandId == BrandId && t.Product.Name == name select t).ToList());
                }

                foundDataBind();
            }
            else if (IsCategoryId == true && IsSubCategoryId == false && IsBrandId == true && Isname == false)
            {
                if (BrandId == 0)
                {
                    transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId && t.Product.BrandId == null  select t).ToList());
                }
                else
                {
                    transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId && t.Product.BrandId == BrandId select t).ToList());
                }

                foundDataBind();
            }
            else if (IsCategoryId == true && IsSubCategoryId == false && IsBrandId == false && Isname == true)
            {
                transList.AddRange((from t in entity.ProductPriceChanges where t.Product.ProductCategoryId == CategoryId && t.Product.Name == Name select t).ToList());
                foundDataBind();
            }
            else if (IsCategoryId == false && IsSubCategoryId == false && IsBrandId == false && Isname == false)
            {
                transList.AddRange(entity.ProductPriceChanges.ToList());
                foundDataBind();
            }
            else
            {
                foundDataBind();
            }
        }

        #endregion

        private void btnSearch2_Click(object sender, EventArgs e)
        {

            transList.Clear();
            transList.AddRange((from t in entity.ProductPriceChanges where t.Product.Barcode.Trim() == txtBarcode.Text.Trim() select t).ToList());
            foundDataBind();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }

       
        private void dgvItemList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            int count = 1;
            foreach (DataGridViewRow row in dgvItemList.Rows)
            {
                ProductPriceChange currentt = (ProductPriceChange)row.DataBoundItem;
                row.Cells[0].Value = currentt.Id;
                row.Cells[1].Value = count.ToString();
                row.Cells[2].Value = currentt.Product.ProductCode;
                row.Cells[3].Value = currentt.Product.Name;
                row.Cells[4].Value = currentt.OldPrice;
                row.Cells[5].Value = currentt.Price;
                row.Cells[6].Value = currentt.User.Name;
                row.Cells[7].Value = currentt.UpdateDate;
                count++;
            }
        }
        private void foundDataBind()
        {
            dgvItemList.DataSource = "";

            if (transList.Count < 1)
            {
                MessageBox.Show("Item not found!", "Cannot find");
                dgvItemList.DataSource = "";
                return;
            }
            else
            {
                dgvItemList.DataSource = transList;
            }
        }
        public void DataBind()
        {

             transList.Clear();
             transList.AddRange((from t in entity.ProductPriceChanges orderby t.Id descending  select t).Take(100).ToList());
             dgvItemList.DataSource = transList;
        }

        private void cboBrand_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboMainCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMainCategory.SelectedIndex != 0 && cboMainCategory.SelectedIndex != 1)
            {
                int productCategoryId = Int32.Parse(cboMainCategory.SelectedValue.ToString());
                List<APP_Data.ProductSubCategory> pSubCatList = new List<APP_Data.ProductSubCategory>();
                APP_Data.ProductSubCategory SubCategoryObj1 = new APP_Data.ProductSubCategory();
                SubCategoryObj1.Id = 0;
                SubCategoryObj1.Name = "Select";
                pSubCatList.Add(SubCategoryObj1);
                pSubCatList.AddRange((from c in entity.ProductSubCategories where c.ProductCategoryId == productCategoryId select c).ToList());
                cboSubCategory.DataSource = pSubCatList;
                cboSubCategory.DisplayMember = "Name";
                cboSubCategory.ValueMember = "Id";
                cboSubCategory.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cboSubCategory.AutoCompleteSource = AutoCompleteSource.ListItems;
                cboSubCategory.Enabled = true;
            }
            else
            {
                cboSubCategory.SelectedIndex = 0;
                cboSubCategory.Enabled = false;
            }
        }

        private void rdbBarCode_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbBarCode.Checked)
            {
                gbBarCode.Enabled = true;
                dgvItemList.DataSource = "";
                Clear();
            }
        }

        private void rdbAll_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbAll.Checked)
            {
                gbType.Enabled = true;
                DataBind();
                Clear();
            }
        }
        private void Clear()
        {
            txtBarcode.Text = "";
            txtName.Text = "";


        }

    }
}
