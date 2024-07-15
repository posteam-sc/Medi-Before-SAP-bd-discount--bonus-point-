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
    public partial class SaleBreakDown : Form
    {
        #region Variables
        POSEntities entity = new POSEntities();
        List<SaleBreakDownController> ResultList = new List<SaleBreakDownController>();
        List<SaleBreakDownController> FinalResultList = new List<SaleBreakDownController>();
        List<SaleBreakDownController> spresultList = new List<SaleBreakDownController>();
        string name = string.Empty;
        #endregion
        #region Event
        public SaleBreakDown()
        {
            InitializeComponent();
        }

        private void SaleBreakDown_Load(object sender, EventArgs e)
        {
            rdbAll.Checked = true;
            rdbRange.Checked = true;
            rdbSaleTrueValue.Checked = true;
            City_Bind();
            Counter_Bind();
            LoadData();
        }

        private void cboCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void cboCounter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdbRange_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdbSegment_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdbSaleTrueValue_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdbUnitPrice_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdbAll_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdbVIP_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }
        #endregion
        #region Function

        private void City_Bind()
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

        private void Counter_Bind()
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

        private void LoadData()
        {

            int CityId = 0, CountryId = 0;

            if (cboCity.SelectedIndex != 0)
            {
                CityId = Convert.ToInt32(cboCity.SelectedValue);
            }

            if (cboCounter.SelectedIndex != 0)
            {
                CountryId = Convert.ToInt32(cboCounter.SelectedValue);
            }
            DateTime fromDate = dtFrom.Value.Date;
            DateTime toDate = dtTo.Value.Date;
            bool IsVIP = rdbVIP.Checked;
            bool IsRange = rdbRange.Checked;
            bool IsSaleTrue = rdbSaleTrueValue.Checked;
            int cusTypeId = 0;
            if (IsVIP)
            {
                cusTypeId = 1;
            }
            
            decimal total = 0;
            decimal spTotal = 0;
            ResultList.Clear();
            spresultList.Clear();
            #region Range
            if (IsRange)
            {
                name = "Range";
                if (IsSaleTrue)
                {
                    System.Data.Objects.ObjectResult<SaleBreakDownByRangeWithSaleTrueValue_Result> rList = entity.SaleBreakDownByRangeWithSaleTrueValue(fromDate, toDate,cusTypeId, false,CityId,CountryId);
                    List<SaleBreakDownController> ResultList2 = new List<SaleBreakDownController>();
                    foreach (SaleBreakDownByRangeWithSaleTrueValue_Result r in rList)
                    {
                        SaleBreakDownController saleObj = new SaleBreakDownController();
                        saleObj.bId = Convert.ToInt32(r.Id);
                        saleObj.Name = r.Name;
                        saleObj.Sales = (r.TotalSale == null) ? 0 : Convert.ToDecimal(r.TotalSale);
                        saleObj.saleQty = (r.SaleQty == null) ? 0 : Convert.ToInt32(r.SaleQty);
                        saleObj.Refund = (r.TotalRefund == null) ? 0 : Convert.ToDecimal(r.TotalRefund);
                        saleObj.refundQty = (r.RefundQty == null) ? 0 : Convert.ToInt32(r.RefundQty);
                        total += Convert.ToInt32(r.TotalSale);
                        ResultList.Add(saleObj);
                        ResultList2.Add(saleObj);
                    }

                    System.Data.Objects.ObjectResult<SaleBreakDownByRangeWithSaleTrueValue_Result> specialPromotionList = entity.SaleBreakDownByRangeWithSaleTrueValue(fromDate, toDate, cusTypeId, true, CityId, CountryId);
                    
                    foreach (SaleBreakDownByRangeWithSaleTrueValue_Result sp in specialPromotionList)
                    {
                        SaleBreakDownController saleObj = new SaleBreakDownController();
                        saleObj.bId = Convert.ToInt32(sp.Id);
                        saleObj.Name = sp.Name;
                        saleObj.Sales = (sp.TotalSale == null) ? 0 : Convert.ToDecimal(sp.TotalSale);
                        saleObj.saleQty = (sp.SaleQty == null) ? 0 : Convert.ToInt32(sp.SaleQty);
                        saleObj.Refund = (sp.TotalRefund == null) ? 0 : Convert.ToDecimal(sp.TotalRefund);
                        saleObj.refundQty = (sp.RefundQty == null) ? 0 : Convert.ToInt32(sp.RefundQty);
                        spTotal += Convert.ToInt32(sp.TotalSale);
                        spresultList.Add(saleObj);
                        
                    }

                    
                    //ResultList2 = ResultList;
                    int i = 0;
                    foreach(SaleBreakDownController sb in ResultList2)
                    {

                        System.Data.Objects.ObjectResult<GetSaleSpecialPromotionByCustomerId_Result> splist = entity.GetSaleSpecialPromotionByCustomerId(fromDate, toDate, cusTypeId, sb.bId, true, CityId, CountryId);
                        List<SpecialPromotionController> SPCList = new List<SpecialPromotionController>();
                       
                            foreach (GetSaleSpecialPromotionByCustomerId_Result sp in splist)
                            {
                                SpecialPromotionController sObj = new SpecialPromotionController();
                                sObj.Name = sp.Name;
                                sObj.saleQty = Convert.ToInt32(sp.SaleQty);
                                sObj.Sales = Convert.ToInt32(sp.TotalSale);
                                sObj.refundQty = Convert.ToInt32(sp.RefundQty);
                                sObj.Refund = Convert.ToInt32(sp.TotalRefund);
                                SPCList.Add(sObj);
                            }
                            if (SPCList.Count > 0)
                            {
                                ResultList[i].saleQty += SPCList[0].saleQty;
                                ResultList[i].Sales += Convert.ToDecimal(SPCList[0].Sales);
                                ResultList[i].refundQty += SPCList[0].refundQty;
                                ResultList[i].Refund += Convert.ToDecimal(SPCList[0].Refund);
                                total += Convert.ToInt32(SPCList[0].Sales);
                            }
                        
                        i++;
                            
                    }
                    
                }
                else
                {
                    System.Data.Objects.ObjectResult<SaleBreakDownByRangeWithUnitValue_Result> rList = entity.SaleBreakDownByRangeWithUnitValue(fromDate, toDate, cusTypeId, false, CityId, CountryId);
                   List<SaleBreakDownController> ResultList2 = new List<SaleBreakDownController>();
                   foreach (SaleBreakDownByRangeWithUnitValue_Result r in rList)
                   {
                       SaleBreakDownController saleObj = new SaleBreakDownController();
                       saleObj.bId = Convert.ToInt32(r.Id);
                       saleObj.Name = r.Name;
                       saleObj.Sales = (r.TotalSale == null) ? 0 : Convert.ToDecimal(r.TotalSale);
                       saleObj.saleQty = (r.SaleQty == null) ? 0 : Convert.ToInt32(r.SaleQty);
                       saleObj.Refund = (r.TotalRefund == null) ? 0 : Convert.ToDecimal(r.TotalRefund);
                       saleObj.refundQty = (r.RefundQty == null) ? 0 : Convert.ToInt32(r.RefundQty);
                       total += Convert.ToInt32(r.TotalSale);
                       ResultList.Add(saleObj);
                       ResultList2.Add(saleObj);
                   }

                   System.Data.Objects.ObjectResult<SaleBreakDownByRangeWithUnitValue_Result> specialPromotionList = entity.SaleBreakDownByRangeWithUnitValue(fromDate, toDate, cusTypeId, true, CityId, CountryId);

                   foreach (SaleBreakDownByRangeWithUnitValue_Result sp in specialPromotionList)
                   {
                       SaleBreakDownController saleObj = new SaleBreakDownController();
                       saleObj.bId = Convert.ToInt32(sp.Id);
                       saleObj.Name = sp.Name;
                       saleObj.Sales = (sp.TotalSale == null) ? 0 : Convert.ToDecimal(sp.TotalSale);
                       saleObj.saleQty = (sp.SaleQty == null) ? 0 : Convert.ToInt32(sp.SaleQty);
                       saleObj.Refund = (sp.TotalRefund == null) ? 0 : Convert.ToDecimal(sp.TotalRefund);
                       saleObj.refundQty = (sp.RefundQty == null) ? 0 : Convert.ToInt32(sp.RefundQty);
                       spTotal += Convert.ToInt32(sp.TotalSale);
                       spresultList.Add(saleObj);

                   }
                   int i = 0;
                   foreach (SaleBreakDownController sb in ResultList2)
                   {

                       System.Data.Objects.ObjectResult<GetSaleSpecialPromotionByCustomerId_Result> splist = entity.GetSaleSpecialPromotionByCustomerId(fromDate, toDate, cusTypeId, sb.bId, false, CityId, CountryId);
                       List<SpecialPromotionController> SPCList = new List<SpecialPromotionController>();
                      
                       foreach (GetSaleSpecialPromotionByCustomerId_Result sp in splist)
                       {
                           SpecialPromotionController sObj = new SpecialPromotionController();
                           sObj.Name = sp.Name;
                           sObj.saleQty = Convert.ToInt32(sp.SaleQty);
                           sObj.Sales = Convert.ToInt32(sp.TotalSale);
                           sObj.refundQty = Convert.ToInt32(sp.RefundQty);
                           sObj.Refund = Convert.ToInt32(sp.TotalRefund);
                           SPCList.Add(sObj);
                       }
                       if (SPCList.Count > 0)
                       {
                           ResultList[i].saleQty += SPCList[0].saleQty;
                           ResultList[i].Sales += Convert.ToDecimal(SPCList[0].Sales);
                           ResultList[i].refundQty += SPCList[0].refundQty;
                           ResultList[i].Refund += Convert.ToDecimal(SPCList[0].Refund);
                           total += Convert.ToInt32(SPCList[0].Sales);
                       }

                       i++;

                   }
                }
            }
            #endregion

            #region Segment
            else 
            {
                name = "Segment";
                if (IsSaleTrue)
                {
                    System.Data.Objects.ObjectResult<SaleBreakDownBySegmentWithSaleTrueValue_Result> rList = entity.SaleBreakDownBySegmentWithSaleTrueValue(fromDate, toDate, cusTypeId, false, CityId, CountryId);
                    List<SaleBreakDownController> ResultList2 = new List<SaleBreakDownController>();
                    foreach (SaleBreakDownBySegmentWithSaleTrueValue_Result r in rList)
                    {
                        SaleBreakDownController saleObj = new SaleBreakDownController();
                        saleObj.bId = Convert.ToInt32(r.Id);
                        saleObj.Name = r.Name;
                        saleObj.Sales = (r.TotalSale == null) ? 0 : Convert.ToDecimal(r.TotalSale);
                        saleObj.saleQty = (r.SaleQty == null) ? 0 : Convert.ToInt32(r.SaleQty);
                        saleObj.Refund = (r.TotalRefund == null) ? 0 : Convert.ToDecimal(r.TotalRefund);
                        saleObj.refundQty = (r.RefundQty == null) ? 0 : Convert.ToInt32(r.RefundQty);
                        total += Convert.ToInt32(r.TotalSale);
                        ResultList.Add(saleObj);
                        ResultList2.Add(saleObj);
                    }

                    System.Data.Objects.ObjectResult<SaleBreakDownBySegmentWithSaleTrueValue_Result> specialPromotionList = entity.SaleBreakDownBySegmentWithSaleTrueValue(fromDate, toDate, cusTypeId, true, CityId, CountryId);

                    foreach (SaleBreakDownBySegmentWithSaleTrueValue_Result sp in specialPromotionList)
                    {
                        SaleBreakDownController saleObj = new SaleBreakDownController();
                        saleObj.bId = Convert.ToInt32(sp.Id);
                        saleObj.Name = sp.Name;
                        saleObj.Sales = (sp.TotalSale == null) ? 0 : Convert.ToDecimal(sp.TotalSale);
                        saleObj.saleQty = (sp.SaleQty == null) ? 0 : Convert.ToInt32(sp.SaleQty);
                        saleObj.Refund = (sp.TotalRefund == null) ? 0 : Convert.ToDecimal(sp.TotalRefund);
                        saleObj.refundQty = (sp.RefundQty == null) ? 0 : Convert.ToInt32(sp.RefundQty);
                        spTotal += Convert.ToInt32(sp.TotalSale);
                        spresultList.Add(saleObj);

                    }

                    int i = 0;
                    foreach (SaleBreakDownController sb in ResultList2)
                    {

                        System.Data.Objects.ObjectResult<GetSaleSpecialPromotionSegmentByCustomerId_Result> splist = entity.GetSaleSpecialPromotionSegmentByCustomerId(fromDate, toDate, cusTypeId, sb.bId, true, CityId, CountryId);
                        List<SpecialPromotionController> SPCList = new List<SpecialPromotionController>();

                        foreach (GetSaleSpecialPromotionSegmentByCustomerId_Result sp in splist)
                        {
                            SpecialPromotionController sObj = new SpecialPromotionController();
                            
                            sObj.Name = sp.Name;
                            sObj.saleQty = Convert.ToInt32(sp.SaleQty);
                            sObj.Sales = Convert.ToInt32(sp.TotalSale);
                            sObj.refundQty = Convert.ToInt32(sp.RefundQty);
                            sObj.Refund = Convert.ToInt32(sp.TotalRefund);
                            SPCList.Add(sObj);

                        }
                        if (SPCList.Count > 0)
                        {
                            ResultList[i].saleQty += SPCList[0].saleQty;
                            ResultList[i].Sales += Convert.ToDecimal(SPCList[0].Sales);
                            ResultList[i].refundQty += SPCList[0].refundQty;
                            ResultList[i].Refund += Convert.ToDecimal(SPCList[0].Refund);
                            total += Convert.ToInt32(SPCList[0].Sales);
                        }

                        i++;

                    }

                }
                else
                {
                    System.Data.Objects.ObjectResult<SaleBreakDownBySegmentWithUnitValue_Result> rList = entity.SaleBreakDownBySegmentWithUnitValue(fromDate, toDate, cusTypeId, false, CityId, CountryId);
                    List<SaleBreakDownController> ResultList2 = new List<SaleBreakDownController>();
                    foreach (SaleBreakDownBySegmentWithUnitValue_Result r in rList)
                    {
                        SaleBreakDownController saleObj = new SaleBreakDownController();
                        saleObj.bId = Convert.ToInt32(r.Id);
                        saleObj.Name = r.Name;
                        saleObj.Sales = (r.TotalSale == null) ? 0 : Convert.ToDecimal(r.TotalSale);
                        saleObj.saleQty = (r.SaleQty == null) ? 0 : Convert.ToInt32(r.SaleQty);
                        saleObj.Refund = (r.TotalRefund == null) ? 0 : Convert.ToDecimal(r.TotalRefund);
                        saleObj.refundQty = (r.RefundQty == null) ? 0 : Convert.ToInt32(r.RefundQty);
                        total += Convert.ToInt32(r.TotalSale);
                        ResultList.Add(saleObj);
                        ResultList2.Add(saleObj);
                    }

                    System.Data.Objects.ObjectResult<SaleBreakDownBySegmentWithUnitValue_Result> specialPromotionList = entity.SaleBreakDownBySegmentWithUnitValue(fromDate, toDate, cusTypeId, true, CityId, CountryId);

                    foreach (SaleBreakDownBySegmentWithUnitValue_Result sp in specialPromotionList)
                    {
                        SaleBreakDownController saleObj = new SaleBreakDownController();
                        saleObj.bId = Convert.ToInt32(sp.Id);
                        saleObj.Name = sp.Name;
                        saleObj.Sales = (sp.TotalSale == null) ? 0 : Convert.ToDecimal(sp.TotalSale);
                        saleObj.saleQty = (sp.SaleQty == null) ? 0 : Convert.ToInt32(sp.SaleQty);
                        saleObj.Refund = (sp.TotalRefund == null) ? 0 : Convert.ToDecimal(sp.TotalRefund);
                        saleObj.refundQty = (sp.RefundQty == null) ? 0 : Convert.ToInt32(sp.RefundQty);
                        spTotal += Convert.ToInt32(sp.TotalSale);
                        spresultList.Add(saleObj);

                    }

                    int i = 0;
                    foreach (SaleBreakDownController sb in ResultList2)
                    {

                        System.Data.Objects.ObjectResult<GetSaleSpecialPromotionSegmentByCustomerId_Result> splist = entity.GetSaleSpecialPromotionSegmentByCustomerId(fromDate, toDate, cusTypeId, sb.bId, false, CityId, CountryId);
                        List<SpecialPromotionController> SPCList = new List<SpecialPromotionController>();

                        foreach (GetSaleSpecialPromotionSegmentByCustomerId_Result sp in splist)
                        {
                            SpecialPromotionController sObj = new SpecialPromotionController();

                            sObj.Name = sp.Name;
                            sObj.saleQty = Convert.ToInt32(sp.SaleQty);
                            sObj.Sales = Convert.ToInt32(sp.TotalSale);
                            sObj.refundQty = Convert.ToInt32(sp.RefundQty);
                            sObj.Refund = Convert.ToInt32(sp.TotalRefund);
                            SPCList.Add(sObj);

                        }
                        if (SPCList.Count > 0)
                        {
                            ResultList[i].saleQty += SPCList[0].saleQty;
                            ResultList[i].Sales += Convert.ToDecimal(SPCList[0].Sales);
                            ResultList[i].refundQty += SPCList[0].refundQty;
                            ResultList[i].Refund += Convert.ToDecimal(SPCList[0].Refund);
                            total += Convert.ToInt32(SPCList[0].Sales);
                        }

                        i++;

                    }
                }
            }
            #endregion

            FinalResultList.Clear();
            foreach (SaleBreakDownController s in ResultList)
            {
                SaleBreakDownController sObj = new SaleBreakDownController();
                //sObj.Name = s.Name;
                //sObj.Sales = s.Sales;
                //sObj.saleQty = s.saleQty;
                //sObj.refundQty = s.refundQty;
                //sObj.Refund = s.Refund;
                s.BreakDown = (s.Sales == 0)? 0:Math.Round((s.Sales / total) * 100, 2);
                FinalResultList.Add(s);
            }

            ShowReportViewer();
        }

        private void ShowReportViewer()
        {
            dsReportTemp dsReport = new dsReportTemp();          
            dsReportTemp.SaleBreakDownDataTable dtSaleBreakReport = (dsReportTemp.SaleBreakDownDataTable)dsReport.Tables["SaleBreakDown"];          
            foreach (SaleBreakDownController r in FinalResultList)
            {
                dsReportTemp.SaleBreakDownRow newRow = dtSaleBreakReport.NewSaleBreakDownRow();
                newRow.Name = r.Name;
                newRow.ToalSales = r.Sales;
                newRow.SaleQty = r.saleQty;
                newRow.BreakDown = r.BreakDown;
                newRow.TotalRefund = r.Refund;
                newRow.RefundQty = r.refundQty; dtSaleBreakReport.AddSaleBreakDownRow(newRow);
            }
            ReportDataSource rds1 = new ReportDataSource("DataSet1", dsReport.Tables["SaleBreakDown"]);

            dsReportTemp SPdsReport = new dsReportTemp();
            dsReportTemp.SaleBreakDownDataTable dtSPSaleBreakReport = (dsReportTemp.SaleBreakDownDataTable)SPdsReport.Tables["SaleBreakDown"];
            foreach (SaleBreakDownController r in spresultList)
            {
                dsReportTemp.SaleBreakDownRow newRow = dtSPSaleBreakReport.NewSaleBreakDownRow();
                newRow.Name = r.Name;
                newRow.ToalSales = r.Sales;
                newRow.SaleQty = r.saleQty;
                newRow.BreakDown = 100;
                newRow.TotalRefund = r.Refund;
                newRow.RefundQty = r.refundQty; dtSPSaleBreakReport.AddSaleBreakDownRow(newRow);
            }
            ReportDataSource rds2 = new ReportDataSource("DataSet2", SPdsReport.Tables["SaleBreakDown"]);


            string reportPath = Application.StartupPath + "\\Reports\\SaleBreakDown.rdlc";
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds1);
            reportViewer1.LocalReport.DataSources.Add(rds2);

            string stDate = dtFrom.Value.Date.ToString("dd/MM/yy");
            string endDate = dtTo.Value.Date.ToString("dd/MM/yy");

            ReportParameter Header = new ReportParameter("Header", stDate + " To " + endDate + " Sale Break Down");
            reportViewer1.LocalReport.SetParameters(Header);

            ReportParameter Title = new ReportParameter("Title", name);
            reportViewer1.LocalReport.SetParameters(Title);           

            reportViewer1.RefreshReport();
            
        }
        #endregion

    }
}
