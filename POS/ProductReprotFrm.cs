using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using POS.APP_Data;

namespace POS
{
    public partial class ProductReprotFrm : Form
    {
        public ProductReprotFrm()
        {
            InitializeComponent();
            CenterToScreen();
        }

        #region 
       
        POSEntities entity = new POSEntities();
        List<ProductReportController> pdlist = new List<ProductReportController>();
        int totalQty = 0;
        int MainCategoryId = 0, SubCategoryId = 0, BrandId = 0;
        string skuCode, BrandName, SegName, SubSegName = " ";
        Boolean _status = false;
        #endregion

        private void ProductReprotFrm_Load(object sender, EventArgs e)
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
            pMainCatList.Add(MainCategoryObj1);
            pMainCatList.AddRange((from MainCategory in entity.ProductCategories select MainCategory).ToList());
            cboMainCategory.DataSource = pMainCatList;
            cboMainCategory.DisplayMember = "Name";
            cboMainCategory.ValueMember = "Id";
            cboMainCategory.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboMainCategory.AutoCompleteSource = AutoCompleteSource.ListItems;

            _status = true;
            LoadData();
        }

        private void LoadData()
        {
            if (_status == true)
            {
                List<APP_Data.Product> plist = new List<Product>();
                totalQty = 0;
                MainCategoryId = 0; SubCategoryId = 0; BrandId = 0;
                SegName = " "; SubSegName = " "; BrandName = " "; skuCode = " ";
                pdlist.Clear();
                lblCurrentDate.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
                string SkuCode = "";

                if (cboMainCategory.SelectedIndex > 0)
                {
                    MainCategoryId = Convert.ToInt32(cboMainCategory.SelectedValue);
                }
                if (cboSubCategory.SelectedIndex > 0)
                {
                    SubCategoryId = Convert.ToInt32(cboSubCategory.SelectedValue);
                }
                if (cboBrand.SelectedIndex > 0)
                {
                    BrandId = Convert.ToInt32(cboBrand.SelectedValue);
                }
                if (txtSKUCode.Text.Trim() != null || txtSKUCode.Text.Trim() != "")
                {
                    SkuCode = txtSKUCode.Text.Trim();
                }


                if (MainCategoryId == 0 && SubCategoryId == 0 && BrandId == 0)
                {
                    System.Data.Objects.ObjectResult<GetProductReport_Result> pdResult = entity.GetProductReport();
                    foreach (GetProductReport_Result pr in pdResult)
                    {
                        ProductReportController pdCon = new ProductReportController();
                        pdCon.SKUCode = pr.ProductCode;
                        pdCon.ProductName = pr.Name;
                        pdCon.BrandName = pr.Brand_Name;
                        pdCon.Segment = pr.Segment_Name;
                        pdCon.SubSegment = pr.SubSegment_Name;
                        pdCon.TotalQty = Convert.ToInt32(pr.Qty);
                        pdCon.IsDiscontinous = Convert.ToBoolean(pr.IsDiscontinue);

                        totalQty += Convert.ToInt32(pr.Qty);
                        pdlist.Add(pdCon);
                    }
                }
                else if (MainCategoryId > 0 && SubCategoryId == 0 && BrandId == 0)
                {
                    System.Data.Objects.ObjectResult<ProductReportByCId_Result> pdResult = entity.ProductReportByCId(MainCategoryId);
                    foreach (ProductReportByCId_Result pr in pdResult)
                    {
                        ProductReportController pdCon = new ProductReportController();
                        pdCon.SKUCode = pr.ProductCode;
                        pdCon.ProductName = pr.Name;
                        pdCon.BrandName = pr.Brand_Name;
                        pdCon.TotalQty = Convert.ToInt32(pr.Qty);
                        pdCon.Segment = pr.Segment_Name;
                        pdCon.SubSegment = pr.SubSegment_Name;
                        pdCon.IsDiscontinous = Convert.ToBoolean(pr.IsDiscontinue);
                        totalQty += Convert.ToInt32(pr.Qty);
                        pdlist.Add(pdCon);
                    }
                }

                else if (MainCategoryId > 0 && SubCategoryId > 0 && BrandId == 0)
                {
                    System.Data.Objects.ObjectResult<ProductReportBySCIdAndCId_Result> pdResult = entity.ProductReportBySCIdAndCId(MainCategoryId, SubCategoryId);
                    foreach (ProductReportBySCIdAndCId_Result pr in pdResult)
                    {
                        ProductReportController pdCon = new ProductReportController();
                        pdCon.SKUCode = pr.ProductCode;
                        pdCon.ProductName = pr.Name;
                        pdCon.BrandName = pr.Brand_Name;
                        pdCon.TotalQty = Convert.ToInt32(pr.Qty);
                        pdCon.Segment = pr.Segment_Name;
                        pdCon.SubSegment = pr.SubSegment_Name;
                        pdCon.IsDiscontinous = Convert.ToBoolean(pr.IsDiscontinue);
                        totalQty += Convert.ToInt32(pr.Qty);
                        pdlist.Add(pdCon);
                    }
                }

                else if (MainCategoryId > 0 && SubCategoryId > 0 && BrandId > 0)
                {
                    System.Data.Objects.ObjectResult<ProductReportBySCIdAndCIdAndBId_Result> pdResult = entity.ProductReportBySCIdAndCIdAndBId(MainCategoryId, SubCategoryId, BrandId);
                    foreach (ProductReportBySCIdAndCIdAndBId_Result pr in pdResult)
                    {
                        ProductReportController pdCon = new ProductReportController();
                        pdCon.SKUCode = pr.ProductCode;
                        pdCon.ProductName = pr.Name;
                        pdCon.BrandName = pr.Brand_Name;
                        pdCon.TotalQty = Convert.ToInt32(pr.Qty);
                        pdCon.Segment = pr.Segment_Name;
                        pdCon.SubSegment = pr.SubSegment_Name;
                        pdCon.IsDiscontinous = Convert.ToBoolean(pr.IsDiscontinue);
                        totalQty += Convert.ToInt32(pr.Qty);
                        pdlist.Add(pdCon);
                    }
                }
                else if (MainCategoryId > 0 && SubCategoryId == 0 && BrandId > 0)
                {
                    System.Data.Objects.ObjectResult<ProductReportByCIdAndBId_Result> pdResult = entity.ProductReportByCIdAndBId(BrandId, MainCategoryId);
                    foreach (ProductReportByCIdAndBId_Result pr in pdResult)
                    {
                        ProductReportController pdCon = new ProductReportController();
                        pdCon.SKUCode = pr.ProductCode;
                        pdCon.ProductName = pr.Name;
                        pdCon.BrandName = pr.Brand_Name;
                        pdCon.TotalQty = Convert.ToInt32(pr.Qty);
                        pdCon.Segment = pr.Segment_Name;
                        pdCon.SubSegment = pr.SubSegment_Name;
                        pdCon.IsDiscontinous = Convert.ToBoolean(pr.IsDiscontinue);
                        totalQty += Convert.ToInt32(pr.Qty);
                        pdlist.Add(pdCon);
                    }
                }

                else if (MainCategoryId == 0 && SubCategoryId == 0 && BrandId > 0)
                {
                    System.Data.Objects.ObjectResult<ProductReportByBId_Result> pdResult = entity.ProductReportByBId(BrandId);
                    foreach (ProductReportByBId_Result pr in pdResult)
                    {
                        ProductReportController pdCon = new ProductReportController();
                        pdCon.SKUCode = pr.ProductCode;
                        pdCon.ProductName = pr.Name;
                        pdCon.BrandName = pr.Brand_Name;
                        pdCon.TotalQty = Convert.ToInt32(pr.Qty);
                        pdCon.Segment = pr.Segment_Name;
                        pdCon.SubSegment = pr.SubSegment_Name;
                        pdCon.IsDiscontinous = Convert.ToBoolean(pr.IsDiscontinue);
                        totalQty += Convert.ToInt32(pr.Qty);
                        pdlist.Add(pdCon);
                    }
                }

                if (chkDiscontinous.Checked)
                {
                    totalQty = 0;
                    List<ProductReportController> productlist = new List<ProductReportController>();

                    foreach (ProductReportController p in pdlist.Where(x => x.IsDiscontinous == true))
                    {
                        ProductReportController pdCon = new ProductReportController();
                        pdCon.SKUCode = p.SKUCode;
                        pdCon.ProductName = p.ProductName;
                        pdCon.BrandName = p.BrandName;
                        pdCon.TotalQty = p.TotalQty;
                        pdCon.Segment = p.Segment;
                        pdCon.SubSegment = p.SubSegment;
                        pdCon.IsDiscontinous = p.IsDiscontinous;
                        totalQty += p.TotalQty;
                        productlist.Add(pdCon);
                    }
                    pdlist.Clear();
                    pdlist = productlist;
                }

                if (SkuCode != "")
                {
                    cboBrand.SelectedIndex = 0;
                    cboMainCategory.SelectedIndex = 0;
                    cboSubCategory.SelectedIndex = 0;
                    pdlist.Clear();
                    totalQty = 0;
                    plist = (from p in entity.Products where p.ProductCode == SkuCode select p).ToList();
                    foreach (Product pr in plist)
                    {
                        ProductReportController pdCon = new ProductReportController();
                        pdCon.SKUCode = pr.ProductCode;
                        pdCon.ProductName = pr.Name;
                        pdCon.BrandName = pr.Brand.Name;
                        pdCon.Segment = pr.ProductCategory.Name;

                        if (pr.ProductSubCategory != null)
                        {
                            pdCon.SubSegment = pr.ProductSubCategory.Name;
                        }


                        pdCon.TotalQty = Convert.ToInt32(pr.Qty);
                        totalQty += Convert.ToInt32(pr.Qty);
                        pdlist.Add(pdCon);
                    }
                }
                ShowReportViewer();
            }
          
        }

        private void ShowReportViewer()
        {        

            if (cboMainCategory.SelectedIndex > 0)
            {
                APP_Data.ProductCategory pdC = (from p in entity.ProductCategories where p.Id == MainCategoryId select p).FirstOrDefault();

                if (cboBrand.SelectedIndex > 1)
                {
                    SegName = "," + pdC.Name.ToString();
                }
                else
                {
                    SegName =pdC.Name.ToString();

                }               
            }
            
            if (cboSubCategory.SelectedIndex > 0)
            {
                if (SubCategoryId == 1)
                {
                    SubSegName = "";
                }
                else
                {
                    if (cboMainCategory.SelectedIndex > 0)
                    {
                        APP_Data.ProductSubCategory pdCsub = (from p in entity.ProductSubCategories where p.Id == SubCategoryId select p).FirstOrDefault();
                        SubSegName = "," + pdCsub.Name;
                    }
                    else
                    {
                        APP_Data.ProductSubCategory pdCsub = (from p in entity.ProductSubCategories where p.Id == SubCategoryId select p).FirstOrDefault();
                        SubSegName =pdCsub.Name;
                    }                   
                }
            }           
            
            if (cboBrand.SelectedIndex > 1)
            {
                APP_Data.Brand bd = (from b in entity.Brands where b.Id == BrandId select b).FirstOrDefault();
                BrandName = bd.Name;
            }
            if (txtSKUCode.Text.Trim() != "")
            {
                skuCode =txtSKUCode.Text;
            }

            dsReportTemp dsReport = new dsReportTemp();
            dsReportTemp.ProductReportDataTable dtPdReport = (dsReportTemp.ProductReportDataTable)dsReport.Tables["ProductReport"];
            foreach (ProductReportController pdCon in pdlist)
            {
                dsReportTemp.ProductReportRow newRow = dtPdReport.NewProductReportRow();
                newRow.Name = pdCon.ProductName;
                newRow.ProductCode = pdCon.SKUCode;
                newRow.BrandName = pdCon.BrandName;
                newRow.SegmentName = pdCon.Segment;
                newRow.SubSegmentName = pdCon.SubSegment;
                newRow.Qty = pdCon.TotalQty.ToString();
                dtPdReport.AddProductReportRow(newRow);
            }

            ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ProductReport"]);
            string reportPath=Application.StartupPath + "\\Reports\\ProductReport.rdlc";

            reportViewer1.LocalReport.ReportPath=reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            ReportParameter TotalQty = new ReportParameter("TotalQty",totalQty.ToString());
            reportViewer1.LocalReport.SetParameters(TotalQty);

            ReportParameter Header = new ReportParameter("Header", "Product report for " +  BrandName + SegName +  SubSegName + skuCode + " at "  +  DateTime.Now.Date.ToString("dd/MM/yyyy"));
            reportViewer1.LocalReport.SetParameters(Header);
            reportViewer1.RefreshReport();           
        }

        private void cboMainCategory_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboMainCategory.SelectedIndex != 0)
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
            //maythu
            //LoadData();
        }

        private void cboSubCategory_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void cboSubCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
            //txtSKUCode.Text = "";
        }

        private void cboBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
           // txtSKUCode.Text = "";
        }

        private void cboMainCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
            //txtSKUCode.Text = "";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
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
            pMainCatList.Add(MainCategoryObj1);
            pMainCatList.AddRange((from MainCategory in entity.ProductCategories select MainCategory).ToList());
            cboMainCategory.DataSource = pMainCatList;
            cboMainCategory.DisplayMember = "Name";
            cboMainCategory.ValueMember = "Id";
            cboMainCategory.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboMainCategory.AutoCompleteSource = AutoCompleteSource.ListItems;
            LoadData();
            txtSKUCode.Clear();
            chkDiscontinous.Checked = false;



        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void txtSKUCode_Enter(object sender, EventArgs e)
        {
            LoadData();
        }

        private void txtSKUCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSKUCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (txtSKUCode.Text != string.Empty)
                {
                    LoadData();
                }
            }
        }

        private void chkDiscontinous_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
