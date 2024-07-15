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
    public partial class TransactionDetailByItem : Form
    {

        #region Variable

        POSEntities entity = new POSEntities();
        private ToolTip tp = new ToolTip();
        List<Product> productList = new List<Product>();
        //System.Data.Objects.ObjectResult<TransactionDetailByItem_Result> resultList;
        List<TransactionDetailByItemHolder> DetailLists = new List<TransactionDetailByItemHolder>();
               
        
        #endregion

        #region Event
        public TransactionDetailByItem()
        {
            InitializeComponent();
           
        }

        private void TransactionDetailByItem_Load(object sender, EventArgs e)
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

            List<Product> productList = new List<Product>();
            Product productObj = new Product();
            productObj.Id = 0;
            productObj.Name = "Select";
            productList.Add(productObj);
            productList.AddRange(entity.Products.ToList());
            cboProductName.DataSource = productList;
            cboProductName.DisplayMember = "Name";
            cboProductName.ValueMember = "ProductCode";
            cboProductName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboProductName.AutoCompleteSource = AutoCompleteSource.ListItems;

            this.reportViewer1.RefreshReport();

            City_BInd();
            Counter_BInd();
            LoadData();
        }

        private void cboCity_SelectedValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void cboCounter_SelectedValueChanged(object sender, EventArgs e)
        {
            LoadData();
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
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            rdbSale.Checked = true;
            cboBrand.SelectedIndex = 0;
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
            cboSubCategory.SelectedIndex = 0;
            cboMainCategory.SelectedIndex = 0;
            cboProductName.SelectedIndex = -1;
            dtFrom.Value = DateTime.Now;
            dtTo.Value = DateTime.Now;
            LoadData();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            dsReportTemp dsReport = new dsReportTemp();
            dsReportTemp.TransactionDetailByItemDataTable dtTransactionReport = (dsReportTemp.TransactionDetailByItemDataTable)dsReport.Tables["TransactionDetailByItem"];

            foreach (TransactionDetailByItemHolder r in DetailLists)
            {
                dsReportTemp.TransactionDetailByItemRow newRow = dtTransactionReport.NewTransactionDetailByItemRow();
                newRow.ItemId = r.ItemId.ToString();
                newRow.Name = r.Name;
                newRow.TransactionId = r.TransactionId.ToString();
                newRow.TransactionType = r.TransactionType;
                newRow.Qty = Convert.ToInt32(r.Qty);
                newRow.TotalAmount = Convert.ToInt64(r.TotalAmount);
                newRow.DateTime = Convert.ToDateTime(r.date);
                dtTransactionReport.AddTransactionDetailByItemRow(newRow);
            }

            ReportViewer rv = new ReportViewer();
            ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["TransactionDetailByItem"]);
            string reportPath = Application.StartupPath + "\\Reports\\TransactionDetailByItem.rdlc";
            rv.Reset();
            rv.LocalReport.ReportPath = reportPath;
            rv.LocalReport.DataSources.Clear();
            rv.LocalReport.DataSources.Add(rds);

            ReportParameter TitlePara = new ReportParameter("TitlePara", gbList.Text + " for " + SettingController.DefaultShop.ShopName);
            rv.LocalReport.SetParameters(TitlePara);

            ReportParameter Date = new ReportParameter("Date", " from " + dtFrom.Value.Date.ToString("dd/MM/yyyy") + " To " + dtTo.Value.Date.ToString("dd/MM/yyyy"));
            rv.LocalReport.SetParameters(Date);

            PrintDoc.PrintReport(rv);
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void cboBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void cboSubCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdbRefund_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdbSale_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void cboProductName_SelectedIndexChanged(object sender, EventArgs e)
        {   
            LoadData();
        }

        #endregion

        #region Function

        private void Counter_BInd()
        {
            List<APP_Data.Counter> counterList = new List<APP_Data.Counter>();
            APP_Data.Counter counterObj = new APP_Data.Counter();
            counterObj.Id = 0;
            counterObj.Name = "Select";
            counterList.Add(counterObj);
            counterList.AddRange((from c in entity.Counters orderby c.Id select c).ToList());
            cboCounter.DataSource = counterList;
            cboCounter.DisplayMember = "Name";
            cboCounter.ValueMember = "Id";
        }


        private void City_BInd()
        {
            List<APP_Data.City> cityList = new List<APP_Data.City>();
            APP_Data.City city1 = new APP_Data.City();
            city1.Id = 0;
            city1.CityName = "Select";
            cityList.Add(city1);
            cityList.AddRange(entity.Cities.ToList());
            cboCity.DataSource = cityList;
            cboCity.DisplayMember = "CityName";
            cboCity.ValueMember = "Id";
        }

        private void LoadData()
        {
            //Boolean hasError = false;
            int MainCategoryId = 0, SubCategoryId = 0, BrandId = 0;
            string ProductCode = "";
            DateTime fromDate = dtFrom.Value.Date;
            DateTime toDate = dtTo.Value.Date;
            bool IsSale = rdbSale.Checked;


            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";

            DetailLists.Clear();

            int CityId = 0, CounterId = 0;
            if (cboCity.SelectedIndex > 0)
            {
                CityId = Convert.ToInt32(cboCity.SelectedValue);
            }

            if (cboCounter.SelectedIndex > 0)
            {
                CounterId = Convert.ToInt32(cboCounter.SelectedValue);
            }

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
            if (cboProductName.SelectedIndex > 0)
            {
                ProductCode = cboProductName.SelectedValue.ToString();
            }
            //resultList = entity.TransactionDetailByItem(fromDate, toDate, IsSale, MainCategoryId, SubCategoryId, BrandId);
            if (MainCategoryId == 0 && SubCategoryId == 0 && BrandId == 0)
            {
                System.Data.Objects.ObjectResult<TransactionDetailReport_Result> resultList;
                resultList = entity.TransactionDetailReport(fromDate, toDate, IsSale,CityId,CounterId);
                foreach (TransactionDetailReport_Result r in resultList)
                {
                    TransactionDetailByItemHolder t = new TransactionDetailByItemHolder();
                    t.ItemId = r.ItemId;
                    t.Name = r.ItemName;
                    t.TransactionId = r.TransactionId;
                    t.TransactionType = r.TransactionType;
                    t.Qty = Convert.ToInt32(r.Qty);
                    t.TotalAmount = Convert.ToInt32(r.TotalAmount);
                    t.date = Convert.ToDateTime(r.TransactionDate);
                    DetailLists.Add(t);
                }
            }
            else if (MainCategoryId > 0 && SubCategoryId == 0 && BrandId == 0)
            {
                System.Data.Objects.ObjectResult<TransactionDetailReportByCId_Result> resultList;
                resultList = entity.TransactionDetailReportByCId(fromDate, toDate, IsSale, MainCategoryId,CityId,CounterId);
                foreach (TransactionDetailReportByCId_Result r in resultList)
                {
                    TransactionDetailByItemHolder t = new TransactionDetailByItemHolder();
                    t.ItemId = r.ItemId;
                    t.Name = r.ItemName;
                    t.TransactionId = r.TransactionId;
                    t.TransactionType = r.TransactionType;
                    t.Qty = Convert.ToInt32(r.Qty);
                    t.TotalAmount = Convert.ToInt32(r.TotalAmount);
                    t.date = Convert.ToDateTime(r.TransactionDate);
                    DetailLists.Add(t);
                }
            }
            else if (MainCategoryId > 0 && SubCategoryId > 0 && BrandId == 0)
            {
                System.Data.Objects.ObjectResult<TransactionDetailReportBySCIdAndCId_Result> resultList;
                resultList = entity.TransactionDetailReportBySCIdAndCId(fromDate, toDate, IsSale, SubCategoryId, MainCategoryId,CityId,CounterId);
                foreach (TransactionDetailReportBySCIdAndCId_Result r in resultList)
                {
                    TransactionDetailByItemHolder t = new TransactionDetailByItemHolder();
                    t.ItemId = r.ItemId;
                    t.Name = r.ItemName;
                    t.TransactionId = r.TransactionId;
                    t.TransactionType = r.TransactionType;
                    t.Qty = Convert.ToInt32(r.Qty);
                    t.TotalAmount = Convert.ToInt32(r.TotalAmount);
                    t.date = Convert.ToDateTime(r.TransactionDate);
                    DetailLists.Add(t);
                }
            }
            else if (MainCategoryId > 0 && SubCategoryId == 0 && BrandId > 0)
            {
                System.Data.Objects.ObjectResult<TransactionDetailReportByBIdAndCId_Result> resultList;
                resultList = entity.TransactionDetailReportByBIdAndCId(fromDate, toDate, IsSale, BrandId, MainCategoryId, CityId, CounterId);
                foreach (TransactionDetailReportByBIdAndCId_Result r in resultList)
                {
                    TransactionDetailByItemHolder t = new TransactionDetailByItemHolder();
                    t.ItemId = r.ItemId;
                    t.Name = r.ItemName;
                    t.TransactionId = r.TransactionId;
                    t.TransactionType = r.TransactionType;
                    t.Qty = Convert.ToInt32(r.Qty);
                    t.TotalAmount = Convert.ToInt32(r.TotalAmount);
                    t.date = Convert.ToDateTime(r.TransactionDate);
                    DetailLists.Add(t);
                }
            }
            else if (MainCategoryId == 0 && SubCategoryId == 0 && BrandId > 0)
            {
                System.Data.Objects.ObjectResult<TransactionDetailReportByBId_Result> resultList;
                resultList = entity.TransactionDetailReportByBId(fromDate, toDate, IsSale, BrandId, CityId, CounterId);
                foreach (TransactionDetailReportByBId_Result r in resultList)
                {
                    TransactionDetailByItemHolder t = new TransactionDetailByItemHolder();
                    t.ItemId = r.ItemId;
                    t.Name = r.ItemName;
                    t.TransactionId = r.TransactionId;
                    t.TransactionType = r.TransactionType;
                    t.Qty = Convert.ToInt32(r.Qty);
                    t.TotalAmount = Convert.ToInt32(r.TotalAmount);
                    t.date = Convert.ToDateTime(r.TransactionDate);
                    DetailLists.Add(t);
                }
            }
            else if (MainCategoryId > 0 && SubCategoryId > 0 && BrandId > 0)
            {
                System.Data.Objects.ObjectResult<TransactionDetailReportByBIdAndCIdAndSCId_Result> resultList;
                resultList = entity.TransactionDetailReportByBIdAndCIdAndSCId(fromDate, toDate, IsSale, BrandId, MainCategoryId, SubCategoryId, CityId, CounterId);
                foreach (TransactionDetailReportByBIdAndCIdAndSCId_Result r in resultList)
                {
                    TransactionDetailByItemHolder t = new TransactionDetailByItemHolder();
                    t.ItemId = r.ItemId;
                    t.Name = r.ItemName;
                    t.TransactionId = r.TransactionId;
                    t.TransactionType = r.TransactionType;
                    t.Qty = Convert.ToInt32(r.Qty);
                    t.TotalAmount = Convert.ToInt32(r.TotalAmount);
                    t.date = Convert.ToDateTime(r.TransactionDate);
                    DetailLists.Add(t);
                }
            }
            if (ProductCode != "")
            {
                DetailLists = DetailLists.Where(x => x.ItemId.Equals(ProductCode)).ToList();
            }
            if (IsSale)
            {
                gbList.Text = "Sale Transaction Detail Report";
            }
            else
            {
                gbList.Text = "Refund Transaction Detail Report";
            }
            ShowReportViewer();

            // }
        }
        private void ShowReportViewer()
        {

            dsReportTemp dsReport = new dsReportTemp();
            dsReportTemp.TransactionDetailByItemDataTable dtTransactionReport = (dsReportTemp.TransactionDetailByItemDataTable)dsReport.Tables["TransactionDetailByItem"];

            foreach (TransactionDetailByItemHolder r in DetailLists)
            {
                dsReportTemp.TransactionDetailByItemRow newRow = dtTransactionReport.NewTransactionDetailByItemRow();
                newRow.ItemId = r.ItemId.ToString();
                newRow.Name = r.Name;
                newRow.TransactionId = r.TransactionId.ToString();
                newRow.TransactionType = r.TransactionType;
                newRow.Qty = Convert.ToInt32(r.Qty);
                newRow.TotalAmount =Convert.ToInt64(r.TotalAmount);
                newRow.DateTime = Convert.ToDateTime(r.date);
                dtTransactionReport.AddTransactionDetailByItemRow(newRow);
            }
           
            ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["TransactionDetailByItem"]);
            string reportPath = Application.StartupPath + "\\Reports\\TransactionDetailByItem.rdlc";
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            //ReportParameter TitlePara = new ReportParameter("TitlePara", gbList.Text + " for " + SettingController.DefaultShop.ShopName);
            string _counter = Utility.Counter_Check(cboCounter);
            ReportParameter TitlePara = new ReportParameter("TitlePara", gbList.Text + " for " + _counter);
            reportViewer1.LocalReport.SetParameters(TitlePara);

            ReportParameter Date = new ReportParameter("Date", " from " + dtFrom.Value.Date.ToString("dd/MM/yyyy") + " To " + dtTo.Value.Date.ToString("dd/MM/yyyy"));
            reportViewer1.LocalReport.SetParameters(Date);

            reportViewer1.RefreshReport();
        }
        #endregion               

        private void cboProductName_SelectedValueChanged(object sender, EventArgs e)
        {
    
        }
    }
}
