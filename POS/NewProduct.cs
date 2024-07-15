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
        public partial class NewProduct : Form
        {
            #region Variables

            public Boolean isEdit { get; set; }

            public int ProductId { get; set; }

            private POSEntities entity = new POSEntities();

            private ToolTip tp = new ToolTip();

            List<Product> productList = new List<Product>();

            List<WrapperItem> wrapperList = new List<WrapperItem>();

            List<WrapperItem> delwrapperList = new List<WrapperItem>();

            Product currentProduct;

            #endregion

            #region Event

            public NewProduct()
            {
                InitializeComponent();
            }

            private void NewProduct_Load(object sender, EventArgs e)
            {
                dgvChildItems.AutoGenerateColumns = false;

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
                pSubCatList.Add(SubCategoryObj1);
                APP_Data.ProductSubCategory SubCategoryObj2 = new APP_Data.ProductSubCategory();
                SubCategoryObj2.Id = 1;
                SubCategoryObj2.Name = "None";
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
                //APP_Data.ProductCategory MainCategoryObj2 = new APP_Data.ProductCategory();
                //MainCategoryObj2.Id = 1;
                //MainCategoryObj2.Name = "None";
                pMainCatList.Add(MainCategoryObj1);
                //pMainCatList.Add(MainCategoryObj2);
                pMainCatList.AddRange((from MainCategory in entity.ProductCategories select MainCategory).ToList());
                cboMainCategory.DataSource = pMainCatList;
                cboMainCategory.DisplayMember = "Name";
                cboMainCategory.ValueMember = "Id";
                cboMainCategory.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cboMainCategory.AutoCompleteSource = AutoCompleteSource.ListItems;

                List<Product> productList1 = new List<Product>();

                Product productObj = new Product();
                productObj.Name = "Select";
                productObj.Id = 0;
                productList1.Add(productObj);
                productList1.AddRange((from pList in entity.Products select pList).ToList());
                cboProductList.DataSource = productList1;
                cboProductList.DisplayMember = "Name";
                cboProductList.ValueMember = "Id";
                cboProductList.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cboProductList.AutoCompleteSource = AutoCompleteSource.ListItems;

                List<Unit> unitList = new List<Unit>();
                Unit unitObj = new Unit();
                unitObj.Id = 0;
                unitObj.UnitName = "Select";
                unitList.Add(unitObj);
                unitList.AddRange((from u in entity.Units select u).ToList());
                cboUnit.DataSource = unitList;
                cboUnit.DisplayMember = "UnitName";
                cboUnit.ValueMember = "Id";
                cboUnit.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cboUnit.AutoCompleteSource = AutoCompleteSource.ListItems;

                List<ConsignmentCounter> consignmentCounterList = new List<ConsignmentCounter>();
                ConsignmentCounter consignmentObj = new ConsignmentCounter();
                consignmentObj.Id = 0;
                consignmentObj.Name = "Select";
                consignmentCounterList.Add(consignmentObj);
                consignmentCounterList.AddRange((from c in entity.ConsignmentCounters select c).ToList());
                cboConsignmentCounter.DataSource = consignmentCounterList;
                cboConsignmentCounter.DisplayMember = "Name";
                cboConsignmentCounter.ValueMember = "Id";
                cboConsignmentCounter.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cboConsignmentCounter.AutoCompleteSource = AutoCompleteSource.ListItems;

                List<Tax> taxList = entity.Taxes.ToList();
                List<Tax> result = new List<Tax>();
                foreach (Tax r in taxList)
                {
                    Tax t = new Tax();
                    t.Id = r.Id;
                    t.Name = r.Name + " and " + r.TaxPercent + "%";
                    t.TaxPercent = r.TaxPercent;
                    result.Add(t);
                }
                cboTaxList.DataSource = result;
                cboTaxList.DisplayMember = "Name";
                cboTaxList.ValueMember = "Id";
                if (SettingController.DefaultTaxRate != null)
                {
                    int id = Convert.ToInt32(SettingController.DefaultTaxRate);
                    Tax defaultTax = (from t in entity.Taxes where t.Id == id select t).FirstOrDefault();
                    cboTaxList.Text = defaultTax.Name + " and " + defaultTax.TaxPercent + "%";
                }

                wrapperList.Clear();
                productList.Clear();
                delwrapperList.Clear();
                if (isEdit)
                {
                    //Editing here
                    currentProduct = (from p in entity.Products where p.Id == ProductId select p).FirstOrDefault();
                    txtBarcode.Text = currentProduct.Barcode;
                    txtProductCode.Text = currentProduct.ProductCode;
                    txtName.Text = currentProduct.Name;

                    txtUnitPrice.Text = currentProduct.Price.ToString();
                    if (currentProduct.Brand != null)
                    {
                        cboBrand.Text = currentProduct.Brand.Name;
                    }
                    else
                    {
                        cboBrand.Text = "None";
                    }
                    if (currentProduct.ProductCategory != null)
                    {
                        cboMainCategory.Text = currentProduct.ProductCategory.Name;
                        if (currentProduct.ProductSubCategory != null)
                        {
                            cboSubCategory.Text = currentProduct.ProductSubCategory.Name;
                        }
                        else
                        {
                            cboSubCategory.Text = "None";
                        }
                        cboSubCategory.Enabled = true;
                    }
                    else
                    {
                        cboMainCategory.Text = "Select";
                        cboSubCategory.Enabled = false;
                    }
                    cboTaxList.Text = currentProduct.Tax.Name + " and " + currentProduct.Tax.TaxPercent + "%";
                    txtDiscount.Text = currentProduct.DiscountRate.ToString();

                    cboUnit.Text = currentProduct.Unit.UnitName;
                    if (chkDisContinue.Checked != null)
                    {
                        chkDisContinue.Checked = currentProduct.IsDiscontinue.Value;
                    }
                    txtLocation.Text = currentProduct.ProductLocation;
                    chkMinStock.Checked = currentProduct.IsNotifyMinStock.Value;
                    if (chkMinStock.Checked)
                    {
                        txtQty.Text = currentProduct.Qty.ToString();
                        txtMinStockQty.Text = currentProduct.MinStockQty.ToString();
                        txtMinStockQty.Enabled = true;
                    }
                    else
                    {
                        txtQty.Text = currentProduct.Qty.ToString();
                    }
                    txtPurchasePrice.Text = currentProduct.PurchasePrice.ToString();
                    txtSize.Text = currentProduct.Size;
                    chkIsWrapper.Checked = currentProduct.IsWrapper.Value;
                    if (chkIsWrapper.Checked)
                    {
                        chkIsWrapper.Enabled = false;
                        if (currentProduct.Brand != null)
                        {
                            if (currentProduct.Brand.Name == "Special Promotion")
                            {
                                rdoSP.Checked = true;
                                txtUnitPrice.ReadOnly = true;
                                txtUnitPrice.Text = currentProduct.Price.ToString();
                               
                            }
                        }

                        if (currentProduct.ProductCategory != null)
                        {
                            if (currentProduct.ProductCategory.Name == "GWP")
                            {
                                rdoGWP.Checked = true;
                                txtUnitPrice.ReadOnly = true;
                                txtUnitPrice.Text = currentProduct.Price.ToString();
                                if (currentProduct.Brand != null)
                                {
                                    cboBrand.Text = currentProduct.Brand.Name;
                                }
                                else
                                {
                                    cboBrand.Text = "None";
                                }
                                if (currentProduct.ProductCategory != null)
                                {
                                    cboMainCategory.Text = currentProduct.ProductCategory.Name;
                                    if (currentProduct.ProductSubCategory != null)
                                    {
                                        cboSubCategory.Text = currentProduct.ProductSubCategory.Name;
                                    }
                                    else
                                    {
                                        cboSubCategory.Text = "None";
                                    }
                                    cboSubCategory.Enabled = true;
                                }
                                else
                                {
                                    cboMainCategory.Text = "Select";
                                    cboSubCategory.Enabled = false;
                                }
                              
                            }
                        }
                      
                        wrapperList.AddRange(currentProduct.WrapperItems.ToList());
                        foreach (WrapperItem w in wrapperList)
                        {
                            productList.Add((from p in entity.Products where p.Id == w.ChildProductId select p).FirstOrDefault());
                        }
                        cboProductList.Enabled = true;
                        btnAddItem.Enabled = true;
                        dgvChildItems.DataSource = productList;

                    }
                    chkIsConsignment.Checked = currentProduct.IsConsignment.Value;
                    if (chkIsConsignment.Checked)
                    {
                        cboConsignmentCounter.Text = currentProduct.ConsignmentCounter.Name;
                        txtConsignmentPrice.Text = currentProduct.ConsignmentPrice.ToString();
                        cboConsignmentCounter.Enabled = true;
                        txtConsignmentPrice.Enabled = true;
                    }
                    else
                    {
                        cboConsignmentCounter.Enabled = false;
                        txtConsignmentPrice.Enabled = false;
                    }
                    btnSubmit.Image = POS.Properties.Resources.update_big;
                }
            }

            private void btnSubmit_Click(object sender, EventArgs e)
            {
                Boolean hasError = false;

                tp.RemoveAll();
                tp.IsBalloon = true;
                tp.ToolTipIcon = ToolTipIcon.Error;
                tp.ToolTipTitle = "Error";
                //Validation
                if (txtBarcode.Text.Trim() == string.Empty)
                {
                    tp.SetToolTip(txtBarcode, "Error");
                    tp.Show("Please fill up barcode!", txtBarcode);
                    hasError = true;
                }
                else if (txtProductCode.Text.Trim() == string.Empty)
                {
                    tp.SetToolTip(txtProductCode, "Error");
                    tp.Show("Please fill up SKU!", txtProductCode);
                    hasError = true;
                }
                else if (txtName.Text.Trim() == string.Empty)
                {
                    tp.SetToolTip(txtName, "Error");
                    tp.Show("Please fill up product name!", txtName);
                    hasError = true;
                }
                else if (cboBrand.SelectedIndex == 0)
                {
                    tp.SetToolTip(cboBrand, "Error");
                    tp.Show("Please select brand name!", cboBrand);
                    hasError = true;
                }
                else if (txtUnitPrice.Text.Trim() == string.Empty)
                {
                    tp.SetToolTip(txtUnitPrice, "Error");
                    tp.Show("Please fill up product price!", txtUnitPrice);
                    hasError = true;
                }
                else if (cboMainCategory.SelectedIndex == 0)
                {
                    tp.SetToolTip(cboMainCategory, "Error");
                    tp.Show("Please select main category name!", cboMainCategory);
                    hasError = true;
                }
                else if (cboMainCategory.SelectedIndex > 0 && cboSubCategory.SelectedIndex == 0)
                {
                    tp.SetToolTip(cboSubCategory, "Error");
                    tp.Show("Please select sub category name!", cboSubCategory);
                    hasError = true;
                }
                else if (txtDiscount.Text.Trim() != string.Empty && Convert.ToDouble(txtDiscount.Text) > 100.00)
                {
                    tp.SetToolTip(txtDiscount, "Error");
                    tp.Show("Discount percent must not over 100!", txtDiscount);
                    hasError = true;
                }
                //else if (txtMinStockQty.Text.Trim() == string.Empty)
                //{
                //    txtMinStockQty.Text = "0";
                //}
                else if (cboUnit.SelectedIndex == 0)
                {
                    tp.SetToolTip(cboUnit, "Error");
                    tp.Show("Please select unit!", cboUnit);
                    hasError = true;
                }
                else if (chkIsWrapper.Checked == true && productList.Count == 0)
                {
                    tp.SetToolTip(cboProductList, "Error");
                    tp.Show("Please select wrapper product item!", cboProductList);
                    hasError = true;
                }
                else if (chkIsConsignment.Checked == true && cboConsignmentCounter.SelectedIndex == 0)
                {
                    tp.SetToolTip(cboConsignmentCounter, "Error");
                    tp.Show("Please select consignment counter name!", cboConsignmentCounter);
                    hasError = true;
                }
                else if (txtQty.Text.Trim() == string.Empty)
                {
                    if (chkIsWrapper.Checked)
                    {
                        txtQty.Text = "0";
                    }
                    else
                    {
                        tp.SetToolTip(txtQty, "Error");
                        tp.Show("Please fill up product quantity!", txtQty);
                        hasError = true;
                    }
                }
          

                if (!hasError)
                {
                    //Edit product
                    if (isEdit)
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to update?", "Update", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {

                            APP_Data.Product editProductObj = (from p in entity.Products where p.Id == ProductId select p).FirstOrDefault();

                            int ProductCodeCount = 0, BarcodeCount = 0;
                            int oldPrice = Convert.ToInt32(editProductObj.Price); int currentPrice = 0; int differentPrice = 0;
                            //count = (from p in entity.Products where p.Name == txtName.Text select p).ToList().Count;
                            if (txtProductCode.Text.Trim().ToUpper() != editProductObj.ProductCode.Trim().ToUpper())
                            {
                                ProductCodeCount = (from p in entity.Products where p.ProductCode.Trim().ToUpper() == txtProductCode.Text.Trim().ToUpper() select p).ToList().Count;
                            }
                            if (txtBarcode.Text.Trim().ToUpper() != editProductObj.Barcode.Trim().ToUpper())
                            {
                                BarcodeCount = (from p in entity.Products where p.Barcode.Trim().ToUpper() == txtBarcode.Text.Trim().ToUpper() select p).ToList().Count;
                            }
                            if (ProductCodeCount == 0 && BarcodeCount == 0)
                            {

                                editProductObj.Barcode = txtBarcode.Text.Trim();
                                editProductObj.ProductCode = txtProductCode.Text.Trim();
                                editProductObj.Name = txtName.Text;
                                editProductObj.UpdateDate = DateTime.Now;
                                //if (cboBrand.SelectedIndex == 1)
                                //{
                                //    editProductObj.BrandId = null;
                                //}
                                //else
                                //{
                                    editProductObj.BrandId = Convert.ToInt32(cboBrand.SelectedValue.ToString());
                               // }

                                txtUnitPrice.Text = txtUnitPrice.Text.Trim().Replace(",", "");
                                txtQty.Text = txtQty.Text.Trim().Replace(",", "");
                                txtMinStockQty.Text = txtMinStockQty.Text.Trim().Replace(",", "");
                                editProductObj.IsDiscontinue = chkDisContinue.Checked;
                                editProductObj.Price = Convert.ToInt32(txtUnitPrice.Text);
                                //get price different
                                currentPrice = Convert.ToInt32(txtUnitPrice.Text);
                                differentPrice = currentPrice - oldPrice;
                                //if (oldPrice > currentPrice)
                                //{
                                //    differentPrice = oldPrice - currentPrice;
                                //}
                                //else
                                //{
                                //    differentPrice = currentPrice - oldPrice;
                                //}
                                //get price different
                                editProductObj.Size = txtSize.Text;
                                if (txtPurchasePrice.Text.Trim() != string.Empty)
                                {
                                    editProductObj.PurchasePrice = Convert.ToInt32(txtPurchasePrice.Text);
                                }

                                //if discount is null, add default value
                                editProductObj.TaxId = Convert.ToInt32(cboTaxList.SelectedValue);
                                if (txtDiscount.Text.Trim() == string.Empty)
                                {
                                    editProductObj.DiscountRate = 0;
                                }
                                else
                                {
                                    editProductObj.DiscountRate = Convert.ToDecimal(txtDiscount.Text);
                                }
                                editProductObj.IsNotifyMinStock = chkMinStock.Checked;
                                editProductObj.Qty = Convert.ToInt32(txtQty.Text);
                                //if minstock qty is null, add default value
                                if (txtMinStockQty.Text.Trim() == string.Empty)
                                {
                                    editProductObj.MinStockQty = 0;
                                }
                                else
                                {
                                    editProductObj.MinStockQty = Convert.ToInt32(txtMinStockQty.Text);
                                }
                                editProductObj.UnitId = Convert.ToInt32(cboUnit.SelectedValue);

                                editProductObj.ProductCategoryId = Convert.ToInt32(cboMainCategory.SelectedValue);
                                if (cboSubCategory.SelectedIndex > 1)
                                {
                                    editProductObj.ProductSubCategoryId = Convert.ToInt32(cboSubCategory.SelectedValue);
                                }
                                else if (cboSubCategory.SelectedIndex == 1)
                                {
                                    editProductObj.ProductSubCategoryId = null;
                                }

                                if (txtLocation.Text.Trim() == string.Empty)
                                {
                                    editProductObj.ProductLocation = string.Empty;
                                }
                                else
                                {
                                    editProductObj.ProductLocation = txtLocation.Text;
                                }
                                editProductObj.WrapperItems.Clear();
                                //Remove associated row for wrapperItems table       
                                //var wrapper = entity.WrapperItems.Where(d => d.ParentProductId == editProductObj.Id);
                                //foreach (var d in wrapper)
                                //{
                                //    entity.WrapperItems.Remove(d);
                                //}
                                editProductObj.IsWrapper = chkIsWrapper.Checked;
                                if (editProductObj.IsWrapper.Value)
                                {
                                    foreach (WrapperItem wrapperObj in wrapperList)
                                    {
                                        editProductObj.WrapperItems.Add(wrapperObj);
                                    }
                                }
                                //if product is child product of special
                                if (differentPrice != 0)
                                {
                                    List<WrapperItem> wList = entity.WrapperItems.Where(p => p.ChildProductId == editProductObj.Id).ToList();
                                    if (wList.Count > 0)
                                    {
                                        foreach (WrapperItem w in wList)
                                        {
                                            Product parentProduct = entity.Products.Where(x => x.Id == w.ParentProductId).FirstOrDefault();
                                            if (parentProduct.IsWrapper == true && parentProduct.ProductCategory.Name == "GWP")
                                            {

                                            }
                                            else
                                            {
                                                int spOldPrice = Convert.ToInt32(parentProduct.Price);
                                                int newPrice = Convert.ToInt32(parentProduct.Price) + differentPrice;
                                                parentProduct.Price = newPrice;

                                                ProductPriceChange Spc = new ProductPriceChange();
                                                //Spc.ProductId =
                                                Spc.ProductId = parentProduct.Id;
                                                Spc.Price = newPrice;
                                                Spc.UserID = MemberShip.UserId;
                                                Spc.UpdateDate = DateTime.Now;
                                                Spc.OldPrice = spOldPrice;
                                                parentProduct.ProductPriceChanges.Add(Spc);
                                         }
                                        }
                                    }
                                }
                                editProductObj.IsConsignment = chkIsConsignment.Checked;
                                if (editProductObj.IsConsignment.Value)
                                {
                                    editProductObj.ConsignmentCounterId = Convert.ToInt32(cboConsignmentCounter.SelectedValue);
                                    editProductObj.ConsignmentPrice = Convert.ToInt32(txtConsignmentPrice.Text);
                                }
                                else
                                {
                                    editProductObj.ConsignmentCounterId = null;
                                    editProductObj.ConsignmentPrice = null;
                                }                               
                                
                                if (editProductObj.MinStockQty != null)
                                {
                                    if (editProductObj.Qty > editProductObj.MinStockQty || chkMinStock.Checked == false)
                                    {
                                        entity.Entry(editProductObj).State = System.Data.EntityState.Modified;
                                        if (differentPrice != 0)
                                        {
                                            ProductPriceChange pc = new ProductPriceChange();
                                            pc.ProductId = editProductObj.Id;
                                            pc.OldPrice = oldPrice;
                                            pc.Price = editProductObj.Price;
                                            pc.UserID = MemberShip.UserId;
                                            pc.UpdateDate = DateTime.Now;
                                            entity.ProductPriceChanges.Add(pc);
                                        }
                                        if (delwrapperList.Count > 0)
                                        {
                                            foreach (WrapperItem wp in delwrapperList)
                                            {
                                                entity.WrapperItems.Remove(wp);
                                                entity.SaveChanges();
                                            }
                                        }
                                        entity.SaveChanges();
                                        MessageBox.Show("Successfully Updated!", "Update");
                                        Clear();
                                        if (System.Windows.Forms.Application.OpenForms["ItemList"] != null)
                                        {
                                            ItemList newForm = (ItemList)System.Windows.Forms.Application.OpenForms["ItemList"];
                                            newForm.DataBind();
                                        }
                                        if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                                        {
                                            Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
                                            newForm.Clear();
                                        }
                                        this.Dispose();

                                    }
                                    else
                                    {
                                        MessageBox.Show("Available quantity must be greater than minimum stock quantity!");
                                        return;
                                    }
                                }
                            }
                            else if (BarcodeCount < 0)
                            {
                                tp.SetToolTip(txtBarcode, "Error");
                                tp.Show("This barcode is already exist!", txtBarcode);
                            }
                            else if (ProductCodeCount < 0)
                            {
                                tp.SetToolTip(txtProductCode, "Error");
                                tp.Show("This product code is already exist!", txtProductCode);
                            }
                        }
                    }
                    //add new product
                    else
                    {
                        int ProductCodeCount = 0, BarcodeCount = 0;
                        //count = (from p in entity.Products where p.Name == txtName.Text select p).ToList().Count;
                        ProductCodeCount = (from p in entity.Products where p.ProductCode.Trim() == txtProductCode.Text.Trim() select p).ToList().Count;
                        BarcodeCount = (from p in entity.Products where p.Barcode.Trim() == txtBarcode.Text.Trim() select p).ToList().Count;
                        if (ProductCodeCount == 0 && BarcodeCount == 0)
                        {
                            APP_Data.Product productObj = new APP_Data.Product();

                            productObj.Barcode = txtBarcode.Text;
                            productObj.ProductCode = txtProductCode.Text;

                            productObj.Name = txtName.Text;
                            if (cboBrand.SelectedIndex == 1)
                            {
                                productObj.BrandId = null;
                            }
                            else
                            {
                          
                                productObj.BrandId = Convert.ToInt32(cboBrand.SelectedValue.ToString());
                            }

                            txtUnitPrice.Text = txtUnitPrice.Text.Trim().Replace(",", "");
                            txtQty.Text = txtQty.Text.Trim().Replace(",", "");
                            txtMinStockQty.Text = txtMinStockQty.Text.Trim().Replace(",", "");
                            productObj.Price = Convert.ToInt32(txtUnitPrice.Text);

                            productObj.TaxId = Convert.ToInt32(cboTaxList.SelectedValue);
                            //if discount is null, add default value
                            if (txtDiscount.Text.Trim() == string.Empty)
                            {
                                productObj.DiscountRate = 0;
                            }
                            else
                            {
                                productObj.DiscountRate = Convert.ToDecimal(txtDiscount.Text);
                            }
                            productObj.Size = txtSize.Text;
                            if (txtPurchasePrice.Text.Trim() != string.Empty)
                            {
                                productObj.PurchasePrice = Convert.ToInt32(txtPurchasePrice.Text);
                            }

                            productObj.IsNotifyMinStock = chkMinStock.Checked;
                            productObj.Qty = Convert.ToInt32(txtQty.Text);
                            //if minstock qty is null, add default value
                            if (txtMinStockQty.Text.Trim() == string.Empty)
                            {
                                productObj.MinStockQty = 0;
                            }
                            else
                            {
                                productObj.MinStockQty = Convert.ToInt32(txtMinStockQty.Text);
                            }
                            productObj.UnitId = Convert.ToInt32(cboUnit.SelectedValue);
                            productObj.IsDiscontinue = chkDisContinue.Checked;
                            if (txtLocation.Text.Trim() == string.Empty)
                            {
                                productObj.ProductLocation = string.Empty;
                            }
                            else
                            {
                                productObj.ProductLocation = txtLocation.Text;
                            }
                            productObj.ProductCategoryId = Convert.ToInt32(cboMainCategory.SelectedValue);
                            if (cboSubCategory.SelectedIndex > 1)
                            {
                                productObj.ProductSubCategoryId = Convert.ToInt32(cboSubCategory.SelectedValue);
                            }
                            else if (cboSubCategory.SelectedIndex == 1)
                            {
                                productObj.ProductSubCategoryId = null;
                            }
                            productObj.IsWrapper = chkIsWrapper.Checked;
                            if (productObj.IsWrapper.Value)
                            {
                                foreach (WrapperItem wrapperObj in wrapperList)
                                {
                                    productObj.WrapperItems.Add(wrapperObj);
                                }
                            }

                            productObj.IsConsignment = chkIsConsignment.Checked;
                            if (productObj.IsConsignment.Value)
                            {
                                productObj.ConsignmentCounterId = Convert.ToInt32(cboConsignmentCounter.SelectedValue);
                                productObj.ConsignmentPrice = Convert.ToInt32(txtConsignmentPrice.Text);
                            }
                            else
                            {
                                productObj.ConsignmentCounterId = null;
                                productObj.ConsignmentPrice = null;
                            }
                            if (productObj.MinStockQty != null)
                            {
                                if (productObj.Qty > productObj.MinStockQty || chkMinStock.Checked == false)
                                {
                                    productObj.UpdateDate = DateTime.Now;
                                    entity.Products.Add(productObj);
                                    entity.SaveChanges();
                                    
                                    //Product np = (from p in entity.Products orderby p.Id descending select p).FirstOrDefault();
                                    //ProductPriceChange pc = new ProductPriceChange();
                                    //pc.ProductId = np.Id;
                                    //pc.Price = np.Price;
                                    //pc.UserID = MemberShip.UserId;
                                    //pc.UpdateDate = DateTime.Now;
                                    //entity.ProductPriceChanges.Add(pc);
                                    entity.SaveChanges();
                                    MessageBox.Show("Successfully Saved!", "Save");
                                   
                                    Clear();
                                    if (System.Windows.Forms.Application.OpenForms["ItemList"] != null)
                                    {
                                        ItemList newForm = (ItemList)System.Windows.Forms.Application.OpenForms["ItemList"];
                                        newForm.DataBind();
                                    }
                                    if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                                    {
                                        Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
                                        newForm.Clear();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Available quantity must be greater than minimum stock quantity!");
                                    return;
                                }
                            }
                        }
                        else if (BarcodeCount > 0)
                        {
                            tp.SetToolTip(txtName, "Error");
                            tp.Show("This barcode is already exist!", txtBarcode);
                        }
                        else if (ProductCodeCount > 0)
                        {
                            tp.SetToolTip(txtName, "Error");
                            tp.Show("This product code is already exist!", txtProductCode);
                        }
                        List<Product> productList1 = new List<Product>();

                        Product productObj1 = new Product();
                        productObj1.Name = "Select";
                        productObj1.Id = 0;
                        productList1.Add(productObj1);
                        productList1.AddRange((from pList in entity.Products select pList).ToList());
                        cboProductList.DataSource = productList1;
                        cboProductList.DisplayMember = "Name";
                        cboProductList.ValueMember = "Id";
                        cboProductList.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        cboProductList.AutoCompleteSource = AutoCompleteSource.ListItems;
                    }


                    //Clear Data
                    //ClearInputs();
                }

            }

            private void btnAddItem_Click(object sender, EventArgs e)
            {
                Product pObj = new Product();
                bool isHave = false;
                int totalAmount = 0;
               
                int id = Convert.ToInt32(cboProductList.SelectedValue);
                if (id > 0)
                {
                    foreach (Product p in productList)
                    {
                        if (p.Id == id) isHave = true;//
                    }
                   
                    if (!isHave)
                    {
                        WrapperItem wrapperItemObj = new WrapperItem();
                        wrapperItemObj.ChildProductId = id;
                        wrapperList.Add(wrapperItemObj);
                        dgvChildItems.DataSource = "";
                        //dgvChildItems.DataSource = wrapperList;
                        pObj = (from p in entity.Products where p.Id == id select p).FirstOrDefault();
                        productList.Add(pObj);
                        dgvChildItems.DataSource = productList;
                        totalAmount += Convert.ToInt32(pObj.Price);

                        if (txtUnitPrice.Text == "")
                        {
                            txtUnitPrice.Text = "0";
                        }

                        if (rdoSP.Checked == true)
                        {
                            txtUnitPrice.Text = (Convert.ToInt32(txtUnitPrice.Text) + totalAmount).ToString(); 
                        }
                        else if (rdoGWP.Checked == true)
                        {
                            txtUnitPrice.Text = "0";
                        }                                 
                       
                    }
                    else
                    {
                        //to show meassage for duplicate 
                        MessageBox.Show("This product is already include!");
                    }
                }
                else
                {
                    //to show message
                }

            }

            private void cboMainCategory_SelectedValueChanged(object sender, EventArgs e)
            {
                if (cboMainCategory.SelectedIndex > 0)
                {
                    int productCategoryId = Int32.Parse(cboMainCategory.SelectedValue.ToString());
                    List<APP_Data.ProductSubCategory> pSubCatList = new List<APP_Data.ProductSubCategory>();
                    APP_Data.ProductSubCategory SubCategoryObj1 = new APP_Data.ProductSubCategory();
                    SubCategoryObj1.Id = 0;
                    SubCategoryObj1.Name = "Select";
                    APP_Data.ProductSubCategory SubCategoryObj2 = new APP_Data.ProductSubCategory();
                    SubCategoryObj2.Id = 1;
                    SubCategoryObj2.Name = "None";
                    pSubCatList.Add(SubCategoryObj1);
                    pSubCatList.Add(SubCategoryObj2);
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

            private void dgvChildItems_CellClick(object sender, DataGridViewCellEventArgs e)
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == 2)
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {    
                            DataGridViewRow row = dgvChildItems.Rows[e.RowIndex];

                            string pdcode = row.Cells[0].Value.ToString();

                            APP_Data.Product p = entity.Products.Where(x => x.ProductCode == pdcode).FirstOrDefault();

                            if (rdoGWP.Checked==false)
                            {
                                if (txtUnitPrice.Text != "")
                                {
                                    txtUnitPrice.Text = (Convert.ToInt32(txtUnitPrice.Text) - p.Price).ToString();
                                } 
                            }                            
                            dgvChildItems.DataSource = "";     
                            wrapperList.RemoveAt(e.RowIndex);
                            productList.RemoveAt(e.RowIndex);

                            if (ProductId != 0)
                            {
                                WrapperItem wpObj = entity.WrapperItems.Where(x => x.ParentProductId == ProductId && x.ChildProductId == p.Id).FirstOrDefault();

                                if (wpObj != null)
                                {
                                    delwrapperList.Add(wpObj);
                                }
                            }

                            dgvChildItems.DataSource = productList;
                            // dgvChildItems.DataSource = wrapperList;
                        }
                    }
                }
            }

            private void chkIsWrapper_CheckedChanged(object sender, EventArgs e)
            {
                if (chkIsWrapper.Checked == true)
                {
                    rdoGWP.Enabled = true;
                    rdoSP.Enabled = true;
                    rdoSP.Checked = true;
                    rdoGWP.Checked = false;

                    txtUnitPrice.Text = "0";
                    txtUnitPrice.ReadOnly = true;
                   
                    cboProductList.Enabled = true;
                    btnAddItem.Enabled = true;
                }
                else
                {
                    txtUnitPrice.Clear();
                    txtUnitPrice.ReadOnly = false;
                    rdoGWP.Enabled = false;
                    rdoSP.Enabled = false;
                    cboProductList.Enabled = false;
                    btnAddItem.Enabled = false;
                    cboProductList.SelectedIndex = 0;
                }
            }

            private void chkIsConsignment_CheckedChanged(object sender, EventArgs e)
            {
                if (chkIsConsignment.Checked)
                {
                    cboConsignmentCounter.Enabled = true;
                    txtConsignmentPrice.Enabled = true;
                }
                else
                {
                    cboConsignmentCounter.Enabled = false;
                    txtConsignmentPrice.Enabled = false;
                    cboConsignmentCounter.SelectedIndex = 0;
                    txtConsignmentPrice.Text = "";
                }
            }

            private void NewProduct_MouseMove(object sender, MouseEventArgs e)
            {
                tp.Hide(txtName);
                tp.Hide(txtQty);
                tp.Hide(txtUnitPrice);
                tp.Hide(cboBrand);
                tp.Hide(cboMainCategory);
                tp.Hide(cboProductList);
                tp.Hide(cboSubCategory);
                tp.Hide(cboUnit);
                tp.Hide(cboConsignmentCounter);
            }

            private void txtUnitPrice_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                } 
            }

            private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                } 
            }

            private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                } 
            }

            private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                } 
            }

            private void txtMinStockQty_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                } 
            }

            private void chkMinStock_CheckedChanged(object sender, EventArgs e)
            {
                if (chkMinStock.Checked)
                {
                    txtMinStockQty.Enabled = true;
                }
                else
                {
                    txtMinStockQty.Enabled = false;
                    txtMinStockQty.Text = "";
                }
            }

            private void btnCancel_Click(object sender, EventArgs e)
            {
                txtLocation.Text = string.Empty;
                txtPurchasePrice.Text = string.Empty;
                txtLocation.Text = "";
                txtBarcode.Text = "";
                txtProductCode.Text = "";
                txtConsignmentPrice.Text = "";
                txtConsignmentPrice.Enabled = false;
                txtDiscount.Text = "";
                txtMinStockQty.Text = "";
                txtMinStockQty.Enabled = false;
                txtName.Text = "";
                txtPurchasePrice.Text = "";
                txtQty.Text = "";
                txtSize.Text = "";
                txtUnitPrice.Text = "";
                chkIsConsignment.Checked = false;
                chkIsWrapper.Checked = false;
                chkMinStock.Checked = false;
                cboBrand.SelectedIndex = 0;
                cboConsignmentCounter.SelectedIndex = 0;
                cboConsignmentCounter.Enabled = false;
                cboMainCategory.SelectedIndex = 0;
                cboSubCategory.SelectedIndex = 0;
                cboSubCategory.Enabled = false;
                cboTaxList.SelectedIndex = 0;
                cboUnit.SelectedIndex = 0;
                cboProductList.SelectedIndex = 0;
                cboProductList.Enabled = false;
                btnAddItem.Enabled = false;
                dgvChildItems.DataSource = "";
                productList.Clear();
                isEdit = false;
                wrapperList.Clear();
            }


            private void btnNewBrand_Click(object sender, EventArgs e)
            {
                Brand newForm = new Brand();
                newForm.ShowDialog();
            }

            private void btnNewCategory_Click(object sender, EventArgs e)
            {
                ProductCategory newForm = new ProductCategory();
                newForm.ShowDialog();
            }

            private void btnNewSubCategofry_Click(object sender, EventArgs e)
            {
                ProductSubCategory newFrom = new ProductSubCategory();
                newFrom.ShowDialog();
            }

            private void btnNewUnit_Click(object sender, EventArgs e)
            {
                UnitForm newForm = new UnitForm();
                newForm.ShowDialog();

            }

            //private void dgvChildItems_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
            //{
            //    foreach (DataGridViewRow row in dgvChildItems.Rows)
            //    {
            //        WrapperItem wp = (WrapperItem)row.DataBoundItem;
            //         row.Cells[0].Value = wp.Product1.Id;
            //        row.Cells[1].Value = wp.Product1.Name;
            //    }
            //}

            //delete child item at child list


            #endregion      

            #region Function

            private void ClearInputs()
            {
                txtBarcode.Text = string.Empty;
                txtProductCode.Text = string.Empty;
                txtName.Text = string.Empty;
                txtUnitPrice.Text = string.Empty;
                txtDiscount.Text = string.Empty;
                txtQty.Text = "0";
                txtLocation.Text = string.Empty;
                txtPurchasePrice.Text = string.Empty;
                txtSize.Text = string.Empty;
                isEdit = false;
                dgvChildItems.DataSource = "";
                productList.Clear();
                //txtLocation.Text = "";
                //txtBarcode.Text = "";
                //txtProductCode.Text = "";
                //txtConsignmentPrice.Text = "";
                //txtConsignmentPrice.Enabled = false;
                //txtDiscount.Text = "";
                //txtMinStockQty.Text = "";
                //txtMinStockQty.Enabled = false;
                //txtName.Text = "";
                //txtPurchasePrice.Text = "";
                //txtQty.Text = "";
                //txtSize.Text = "";
                //txtUnitPrice.Text = "";
                //chkIsConsignment.Checked = false;
                //chkIsWrapper.Checked = false;
                //chkMinStock.Checked = false;
                //cboBrand.SelectedIndex = 0;
                //cboConsignmentCounter.SelectedIndex = 0;
                //cboConsignmentCounter.Enabled = false;
                //cboMainCategory.SelectedIndex = 0;
                //cboSubCategory.SelectedIndex = 0;
                //cboSubCategory.Enabled = false;
                //cboTaxList.SelectedIndex = 0;
                //cboUnit.SelectedIndex = 0;
                //cboProductList.SelectedIndex = 0;
                //cboProductList.Enabled = false;
                //btnAddItem.Enabled = false;
                //dgvChildItems.DataSource = "";
                //productList.Clear();
                isEdit = false;
            }

            public void Clear()
            {
                txtBarcode.Text = string.Empty;
                txtProductCode.Text = string.Empty;
                txtName.Text = string.Empty;
                txtUnitPrice.Text = string.Empty;
                txtDiscount.Text = string.Empty;          
                txtLocation.Text = string.Empty;
                txtPurchasePrice.Text = string.Empty;
                txtSize.Text = string.Empty;
                isEdit = false;
                dgvChildItems.DataSource = "";
                productList.Clear();  
                txtConsignmentPrice.Text = "";
                txtConsignmentPrice.Enabled = false;            
                txtMinStockQty.Text = "";
                txtMinStockQty.Enabled = false;
                txtQty.Text = "";
                txtSize.Text = "";
                chkIsConsignment.Checked = false;
                chkIsWrapper.Checked = false;
                chkMinStock.Checked = false;
                cboBrand.SelectedIndex = 0;
                cboConsignmentCounter.SelectedIndex = 0;
                cboConsignmentCounter.Enabled = false;
                cboMainCategory.SelectedIndex = 0;
                cboSubCategory.SelectedIndex = 0;
                cboSubCategory.Enabled = false;
                cboTaxList.SelectedIndex = 0;
                cboUnit.SelectedIndex = 0;
                cboProductList.SelectedIndex = 0;
                cboProductList.Enabled = false;
                btnAddItem.Enabled = false;
                dgvChildItems.DataSource = "";
                productList.Clear();
                isEdit = false;
                wrapperList.Clear();
            }
            public void ReloadBrand()
            {
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
                if (isEdit)
                {
                    if (currentProduct.Brand != null)
                    {
                        cboBrand.Text = currentProduct.Brand.Name;
                    }
                    else
                    {
                        cboBrand.Text = "None";
                    }
                }
            }
            public void ReloadCategory()
            {
                List<APP_Data.ProductSubCategory> pSubCatList = new List<APP_Data.ProductSubCategory>();
                APP_Data.ProductSubCategory SubCategoryObj1 = new APP_Data.ProductSubCategory();
                SubCategoryObj1.Id = 0;
                SubCategoryObj1.Name = "Select";
                pSubCatList.Add(SubCategoryObj1);
                APP_Data.ProductSubCategory SubCategoryObj2 = new APP_Data.ProductSubCategory();
                SubCategoryObj2.Id = 1;
                SubCategoryObj2.Name = "None";
                //pSubCatList.AddRange((from c in entity.ProductSubCategories where c.ProductCategoryId == Convert.ToInt32(cboMainCategory.SelectedValue) select c).ToList());
                cboSubCategory.DataSource = pSubCatList;
                cboSubCategory.DisplayMember = "Name";
                cboSubCategory.ValueMember = "Id";

                List<APP_Data.ProductCategory> pMainCatList = new List<APP_Data.ProductCategory>();
                APP_Data.ProductCategory MainCategoryObj1 = new APP_Data.ProductCategory();
                MainCategoryObj1.Id = 0;
                MainCategoryObj1.Name = "Select";
                //APP_Data.ProductCategory MainCategoryObj2 = new APP_Data.ProductCategory();
                //MainCategoryObj2.Id = 1;
                //MainCategoryObj2.Name = "None";
                pMainCatList.Add(MainCategoryObj1);
                //pMainCatList.Add(MainCategoryObj2);
                pMainCatList.AddRange((from MainCategory in entity.ProductCategories select MainCategory).ToList());
                cboMainCategory.DataSource = pMainCatList;
                cboMainCategory.DisplayMember = "Name";
                cboMainCategory.ValueMember = "Id";
                cboMainCategory.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cboMainCategory.AutoCompleteSource = AutoCompleteSource.ListItems;
                if (isEdit)
                {
                    if (currentProduct.ProductCategory != null)
                    {
                        cboMainCategory.Text = currentProduct.ProductCategory.Name;
                        if (currentProduct.ProductSubCategory != null)
                        {
                            cboSubCategory.Text = currentProduct.ProductSubCategory.Name;
                        }
                        else
                        {
                            cboSubCategory.Text = "None";
                        }
                        cboSubCategory.Enabled = true;
                    }
                    else
                    {
                        cboMainCategory.Text = "Select";
                        cboSubCategory.Enabled = false;
                    }
                }
            }
            public void ReloadSubCategory()
            {
                List<APP_Data.ProductSubCategory> pSubCatList = new List<APP_Data.ProductSubCategory>();
                APP_Data.ProductSubCategory SubCategoryObj1 = new APP_Data.ProductSubCategory();
                SubCategoryObj1.Id = 0;
                SubCategoryObj1.Name = "Select";
                pSubCatList.Add(SubCategoryObj1);
                APP_Data.ProductSubCategory SubCategoryObj2 = new APP_Data.ProductSubCategory();
                SubCategoryObj2.Id = 1;
                SubCategoryObj2.Name = "None";
                //pSubCatList.AddRange((from c in entity.ProductSubCategories where c.ProductCategoryId == Convert.ToInt32(cboMainCategory.SelectedValue) select c).ToList());
                cboSubCategory.DataSource = pSubCatList;
                cboSubCategory.DisplayMember = "Name";
                cboSubCategory.ValueMember = "Id";

                List<APP_Data.ProductCategory> pMainCatList = new List<APP_Data.ProductCategory>();
                APP_Data.ProductCategory MainCategoryObj1 = new APP_Data.ProductCategory();
                MainCategoryObj1.Id = 0;
                MainCategoryObj1.Name = "Select";
                //APP_Data.ProductCategory MainCategoryObj2 = new APP_Data.ProductCategory();
                //MainCategoryObj2.Id = 1;
                //MainCategoryObj2.Name = "None";
                pMainCatList.Add(MainCategoryObj1);
                //pMainCatList.Add(MainCategoryObj2);
                pMainCatList.AddRange((from MainCategory in entity.ProductCategories select MainCategory).ToList());
                cboMainCategory.DataSource = pMainCatList;
                cboMainCategory.DisplayMember = "Name";
                cboMainCategory.ValueMember = "Id";
                cboMainCategory.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cboMainCategory.AutoCompleteSource = AutoCompleteSource.ListItems;
                if (isEdit)
                {
                    if (currentProduct.ProductCategory != null)
                    {
                        cboMainCategory.Text = currentProduct.ProductCategory.Name;
                        if (currentProduct.ProductSubCategory != null)
                        {
                            cboSubCategory.Text = currentProduct.ProductSubCategory.Name;
                        }
                        else
                        {
                            cboSubCategory.Text = "None";
                        }
                        cboSubCategory.Enabled = true;
                    }
                    else
                    {
                        cboMainCategory.Text = "Select";
                        cboSubCategory.Enabled = false;
                    }
                }
            }
       
            public void ReloadUnit()
            {
            
                List<Unit> unitList = new List<Unit>();
                Unit unitObj = new Unit();
                unitObj.Id = 0;
                unitObj.UnitName = "Select";
                unitList.Add(unitObj);
                unitList.AddRange((from u in entity.Units select u).ToList());
                cboUnit.DataSource = unitList;
                cboUnit.DisplayMember = "UnitName";
                cboUnit.ValueMember = "Id";
                cboUnit.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cboUnit.AutoCompleteSource = AutoCompleteSource.ListItems;

                if (isEdit)
                {                
                
                    cboUnit.Text = currentProduct.Unit.UnitName;
                }
            }
            #endregion

            private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
            {

            }

            private void rdoSP_CheckedChanged(object sender, EventArgs e)
            {
                if (rdoSP.Checked)
                {
                    txtUnitPrice.Clear();
                    cboProductList.Enabled = true;
                    btnAddItem.Enabled = true;
                    cboBrand.SelectedValue = 1266;
                    cboMainCategory.SelectedValue = 2152;
                    cboSubCategory.SelectedValue = 1434;

                }
                else if(rdoGWP.Checked==true)
                {
                    txtUnitPrice.Clear();
                    cboProductList.Enabled = false;
                    btnAddItem.Enabled = false;
                    productList.Clear();
                    wrapperList.Clear();
                    dgvChildItems.DataSource = "";

                    cboBrand.SelectedValue = 0;
                    cboMainCategory.SelectedValue = 0;
                    cboSubCategory.SelectedValue = 0;
                }
            }

            private void rdoGWP_CheckedChanged(object sender, EventArgs e)
            {
                if (rdoGWP.Checked)
                {
                    txtUnitPrice.Clear();
                    cboProductList.Enabled = true;
                    btnAddItem.Enabled = true;
                    cboMainCategory.SelectedValue = 2157;

                }
                else
                {
                    txtUnitPrice.Clear();
                    cboProductList.Enabled = false;
                    btnAddItem.Enabled = false;
                    //productList.Clear();
                    //wrapperList.Clear();
                    dgvChildItems.DataSource = "";
                }

            }

           
        }
    }
