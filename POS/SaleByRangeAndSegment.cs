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
    public partial class SaleByRangeAndSegment : Form
    {
        #region Variables

        POSEntities entity = new POSEntities();
        List<SaleByRangeOrSegmentController> RangeList = new List<SaleByRangeOrSegmentController>();
        List<SaleByRangeOrSegmentController> SegmentList = new List<SaleByRangeOrSegmentController>();
        List<SaleByRangeOrSegmentController> SubSegmentList = new List<SaleByRangeOrSegmentController>();

        public bool _status=false;
        #endregion
        #region Event
        public SaleByRangeAndSegment()
        {
            InitializeComponent();
        }

        private void SaleByRangeAndSegment_Load(object sender, EventArgs e)
        {
            rdbDisSaleTrueValue.Checked = true;
            City_Bind();
            Counter_Bind();
            _status = true;
            //LoadData();
        }

        private void cboCity_SelectedValueChanged(object sender, EventArgs e)
        {
            //LoadData();
        }

  
        private void cboCounter_SelectedValueChanged(object sender, EventArgs e)
        {
            //LoadData();
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            //LoadData();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            //LoadData();
        }

        private void rdbSaleTrueValue_CheckedChanged(object sender, EventArgs e)
        {
            //LoadData();
        }

        private void rdbDisSaleTrueValue_CheckedChanged(object sender, EventArgs e)
        {
            //LoadData();
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

            if (_status == true)
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
                string Year = dtTo.Value.Year.ToString();
                DateTime toDate = dtTo.Value.Date;
                String Month = dtTo.Value.Month.ToString();
                int m = Convert.ToInt32(Month);
                int y = Convert.ToInt32(Year);
                if (m < 4)
                {
                    y -= 1;
                }
                Year = y.ToString();

                bool IsSaleTrueValue = rdbSaleTrueValue.Checked;

                string t = "4-1-" + Year;
                DateTime yt = Convert.ToDateTime(t);
                RangeList.Clear();
                SegmentList.Clear();
                SubSegmentList.Clear();
                if (IsSaleTrueValue)
                {
                    #region Range
                    System.Data.Objects.ObjectResult<GetSaleByRangeWithSaleTrueValue_Result> RList = entity.GetSaleByRangeWithSaleTrueValue(fromDate, toDate, Year, CityId, CountryId);
                    List<SaleByRangeOrSegmentController> ResultList2 = new List<SaleByRangeOrSegmentController>();
                    foreach (GetSaleByRangeWithSaleTrueValue_Result r in RList)
                    {
                        SaleByRangeOrSegmentController resultObj = new SaleByRangeOrSegmentController();
                        resultObj.Id = Convert.ToInt32(r.Id);
                        resultObj.BrandName = r.Name;
                        resultObj.PeriodTotal = (r.PeriodTotal == null) ? 0 : Convert.ToDecimal(r.PeriodTotal);
                        resultObj.StartYearlyTotal = (r.StartYearlyTotal == null) ? 0 : Convert.ToDecimal(r.StartYearlyTotal);
                        RangeList.Add(resultObj);
                        ResultList2.Add(resultObj);
                    }

                    int i = 0;
                    foreach (SaleByRangeOrSegmentController sb in ResultList2)
                    {

                        System.Data.Objects.ObjectResult<GetSaleSpecialPromotionByCustomerId_Result> splist = entity.GetSaleSpecialPromotionByCustomerId(fromDate, toDate, 0, sb.Id, false, CityId, CountryId);
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
                            RangeList[i].PeriodTotal += SPCList[0].Sales;
                            //total += Convert.ToInt32(SPCList[0].Sales);
                        }

                        System.Data.Objects.ObjectResult<GetSaleSpecialPromotionByCustomerId_Result> splist2 = entity.GetSaleSpecialPromotionByCustomerId(yt, toDate, 0, sb.Id, false, CityId, CountryId);
                        List<SpecialPromotionController> SPCList2 = new List<SpecialPromotionController>();

                        foreach (GetSaleSpecialPromotionByCustomerId_Result sp in splist2)
                        {
                            SpecialPromotionController sObj = new SpecialPromotionController();
                            sObj.Name = sp.Name;
                            sObj.saleQty = Convert.ToInt32(sp.SaleQty);
                            sObj.Sales = Convert.ToInt32(sp.TotalSale);
                            sObj.refundQty = Convert.ToInt32(sp.RefundQty);
                            sObj.Refund = Convert.ToInt32(sp.TotalRefund);
                            SPCList2.Add(sObj);
                        }
                        if (SPCList2.Count > 0)
                        {
                            RangeList[i].StartYearlyTotal += SPCList2[0].Sales;
                            //total += Convert.ToInt32(SPCList[0].Sales);
                        }
                        i++;

                    }
                    #endregion

                    #region Segment
                    System.Data.Objects.ObjectResult<GetSaleBySegmentWithSaleTrueValue_Result> SList = entity.GetSaleBySegmentWithSaleTrueValue(fromDate, toDate, Year, CityId, CountryId);
                    List<SaleByRangeOrSegmentController> ResultSegmentList = new List<SaleByRangeOrSegmentController>();
                    foreach (GetSaleBySegmentWithSaleTrueValue_Result r in SList)
                    {
                        SaleByRangeOrSegmentController resultObj = new SaleByRangeOrSegmentController();
                        resultObj.Id = Convert.ToInt32(r.Id);
                        resultObj.BrandName = r.Name;
                        resultObj.PeriodTotal = (r.PeriodTotal == null) ? 0 : Convert.ToDecimal(r.PeriodTotal);
                        resultObj.StartYearlyTotal = (r.StartYearlyTotal == null) ? 0 : Convert.ToDecimal(r.StartYearlyTotal);
                        SegmentList.Add(resultObj);
                        ResultSegmentList.Add(resultObj);
                    }

                    int j = 0;
                    foreach (SaleByRangeOrSegmentController sb in ResultSegmentList)
                    {

                        System.Data.Objects.ObjectResult<GetSaleSpecialPromotionSegmentByCustomerId_Result> splist = entity.GetSaleSpecialPromotionSegmentByCustomerId(fromDate, toDate, 0, sb.Id, false, CityId, CountryId);
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
                            SegmentList[j].PeriodTotal += SPCList[0].Sales;
                            //total += Convert.ToInt32(SPCList[0].Sales);
                        }

                        System.Data.Objects.ObjectResult<GetSaleSpecialPromotionSegmentByCustomerId_Result> splist2 = entity.GetSaleSpecialPromotionSegmentByCustomerId(yt, toDate, 0, sb.Id, false, CityId, CountryId);
                        List<SpecialPromotionController> SPCList2 = new List<SpecialPromotionController>();

                        foreach (GetSaleSpecialPromotionSegmentByCustomerId_Result sp in splist2)
                        {
                            SpecialPromotionController sObj = new SpecialPromotionController();
                            sObj.Name = sp.Name;
                            sObj.saleQty = Convert.ToInt32(sp.SaleQty);
                            sObj.Sales = Convert.ToInt32(sp.TotalSale);
                            sObj.refundQty = Convert.ToInt32(sp.RefundQty);
                            sObj.Refund = Convert.ToInt32(sp.TotalRefund);
                            SPCList2.Add(sObj);
                        }
                        if (SPCList2.Count > 0)
                        {
                            SegmentList[j].StartYearlyTotal += SPCList2[0].Sales;
                            //total += Convert.ToInt32(SPCList[0].Sales);
                        }
                        j++;

                    }
                    #endregion

                    #region SubSegment
                    System.Data.Objects.ObjectResult<GetSaleBySubSegmentWithSaleTrueValue_Result> SubSList = entity.GetSaleBySubSegmentWithSaleTrueValue(fromDate, toDate, Year);
                    List<SaleByRangeOrSegmentController> ResultSubSegmentList = new List<SaleByRangeOrSegmentController>();
                    foreach (GetSaleBySubSegmentWithSaleTrueValue_Result r in SubSList)
                    {
                        SaleByRangeOrSegmentController resultObj = new SaleByRangeOrSegmentController();
                        resultObj.Id = Convert.ToInt32(r.Id);
                        resultObj.BrandName = r.Name;
                        resultObj.PeriodTotal = (r.PeriodTotal == null) ? 0 : Convert.ToDecimal(r.PeriodTotal);
                        resultObj.StartYearlyTotal = (r.StartYearlyTotal == null) ? 0 : Convert.ToDecimal(r.StartYearlyTotal);
                        SubSegmentList.Add(resultObj);
                        ResultSubSegmentList.Add(resultObj);
                    }

                    int k = 0;
                    foreach (SaleByRangeOrSegmentController sb in ResultSubSegmentList)
                    {

                        System.Data.Objects.ObjectResult<GetSaleSpecialPromotionSubSegmentByCustomerId_Result> splist = entity.GetSaleSpecialPromotionSubSegmentByCustomerId(fromDate, toDate, 0, sb.Id, false, CityId, CountryId);
                        List<SpecialPromotionController> SPCList = new List<SpecialPromotionController>();

                        foreach (GetSaleSpecialPromotionSubSegmentByCustomerId_Result sp in splist)
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
                            SubSegmentList[k].PeriodTotal += SPCList[0].Sales;
                            //total += Convert.ToInt32(SPCList[0].Sales);
                        }

                        System.Data.Objects.ObjectResult<GetSaleSpecialPromotionSubSegmentByCustomerId_Result> splist2 = entity.GetSaleSpecialPromotionSubSegmentByCustomerId(yt, toDate, 0, sb.Id, false, CityId, CountryId);
                        List<SpecialPromotionController> SPCList2 = new List<SpecialPromotionController>();

                        foreach (GetSaleSpecialPromotionSubSegmentByCustomerId_Result sp in splist2)
                        {
                            SpecialPromotionController sObj = new SpecialPromotionController();
                            sObj.Name = sp.Name;
                            sObj.saleQty = Convert.ToInt32(sp.SaleQty);
                            sObj.Sales = Convert.ToInt32(sp.TotalSale);
                            sObj.refundQty = Convert.ToInt32(sp.RefundQty);
                            sObj.Refund = Convert.ToInt32(sp.TotalRefund);
                            SPCList2.Add(sObj);
                        }
                        if (SPCList2.Count > 0)
                        {
                            SubSegmentList[k].StartYearlyTotal += SPCList2[0].Sales;
                            //total += Convert.ToInt32(SPCList[0].Sales);
                        }
                        k++;

                    }
                    #endregion
                }
                else
                {
                    #region Range
                    System.Data.Objects.ObjectResult<GetSaleByRangeWithDiscountedValue_Result> RList = entity.GetSaleByRangeWithDiscountedValue(fromDate, toDate, Year, CityId, CountryId);
                    List<SaleByRangeOrSegmentController> ResultList2 = new List<SaleByRangeOrSegmentController>();
                    foreach (GetSaleByRangeWithDiscountedValue_Result r in RList)
                    {
                        SaleByRangeOrSegmentController resultObj = new SaleByRangeOrSegmentController();
                        resultObj.Id = Convert.ToInt32(r.Id);
                        resultObj.BrandName = r.Name;
                        resultObj.PeriodTotal = (r.PeriodTotal == null) ? 0 : Convert.ToDecimal(r.PeriodTotal);
                        resultObj.StartYearlyTotal = (r.StartYearlyTotal == null) ? 0 : Convert.ToDecimal(r.StartYearlyTotal);
                        RangeList.Add(resultObj);
                        ResultList2.Add(resultObj);
                    }

                    int i = 0;
                    foreach (SaleByRangeOrSegmentController sb in ResultList2)
                    {

                        System.Data.Objects.ObjectResult<GetSaleSpecialPromotionByCustomerId_Result> splist = entity.GetSaleSpecialPromotionByCustomerId(fromDate, toDate, 0, sb.Id, true, CityId, CountryId);
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
                            RangeList[i].PeriodTotal += SPCList[0].Sales;
                            //total += Convert.ToInt32(SPCList[0].Sales);
                        }

                        System.Data.Objects.ObjectResult<GetSaleSpecialPromotionByCustomerId_Result> splist2 = entity.GetSaleSpecialPromotionByCustomerId(yt, toDate, 0, sb.Id, true, CityId, CountryId);
                        List<SpecialPromotionController> SPCList2 = new List<SpecialPromotionController>();

                        foreach (GetSaleSpecialPromotionByCustomerId_Result sp in splist2)
                        {
                            SpecialPromotionController sObj = new SpecialPromotionController();
                            sObj.Name = sp.Name;
                            sObj.saleQty = Convert.ToInt32(sp.SaleQty);
                            sObj.Sales = Convert.ToInt32(sp.TotalSale);
                            sObj.refundQty = Convert.ToInt32(sp.RefundQty);
                            sObj.Refund = Convert.ToInt32(sp.TotalRefund);
                            SPCList2.Add(sObj);
                        }
                        if (SPCList2.Count > 0)
                        {
                            RangeList[i].StartYearlyTotal += SPCList2[0].Sales;
                            //total += Convert.ToInt32(SPCList[0].Sales);
                        }
                        i++;

                    }
                    #endregion
                    #region Segment
                    System.Data.Objects.ObjectResult<GetSaleBySegmentWithDiscountedValue_Result> SList = entity.GetSaleBySegmentWithDiscountedValue(fromDate, toDate, Year, CityId, CountryId);
                    List<SaleByRangeOrSegmentController> ResultSegmentList = new List<SaleByRangeOrSegmentController>();
                    foreach (GetSaleBySegmentWithDiscountedValue_Result r in SList)
                    {
                        SaleByRangeOrSegmentController resultObj = new SaleByRangeOrSegmentController();
                        resultObj.Id = Convert.ToInt32(r.Id);
                        resultObj.BrandName = r.Name;
                        resultObj.PeriodTotal = (r.PeriodTotal == null) ? 0 : Convert.ToDecimal(r.PeriodTotal);
                        resultObj.StartYearlyTotal = (r.StartYearlyTotal == null) ? 0 : Convert.ToDecimal(r.StartYearlyTotal);
                        SegmentList.Add(resultObj);
                        ResultSegmentList.Add(resultObj);
                    }

                    int j = 0;
                    foreach (SaleByRangeOrSegmentController sb in ResultSegmentList)
                    {

                        System.Data.Objects.ObjectResult<GetSaleSpecialPromotionSegmentByCustomerId_Result> splist = entity.GetSaleSpecialPromotionSegmentByCustomerId(fromDate, toDate, 0, sb.Id, true, CityId, CountryId);
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
                            SegmentList[j].PeriodTotal += SPCList[0].Sales;
                            //total += Convert.ToInt32(SPCList[0].Sales);
                        }

                        System.Data.Objects.ObjectResult<GetSaleSpecialPromotionSegmentByCustomerId_Result> splist2 = entity.GetSaleSpecialPromotionSegmentByCustomerId(yt, toDate, 0, sb.Id, true, CityId, CountryId);
                        List<SpecialPromotionController> SPCList2 = new List<SpecialPromotionController>();

                        foreach (GetSaleSpecialPromotionSegmentByCustomerId_Result sp in splist2)
                        {
                            SpecialPromotionController sObj = new SpecialPromotionController();
                            sObj.Name = sp.Name;
                            sObj.saleQty = Convert.ToInt32(sp.SaleQty);
                            sObj.Sales = Convert.ToInt32(sp.TotalSale);
                            sObj.refundQty = Convert.ToInt32(sp.RefundQty);
                            sObj.Refund = Convert.ToInt32(sp.TotalRefund);
                            SPCList2.Add(sObj);
                        }
                        if (SPCList2.Count > 0)
                        {
                            SegmentList[j].StartYearlyTotal += SPCList2[0].Sales;
                            //total += Convert.ToInt32(SPCList[0].Sales);
                        }
                        j++;

                    }
                    #endregion
                    #region SubSegement
                    System.Data.Objects.ObjectResult<GetSaleBySubSegmentWithDiscountedValue_Result> SubSList = entity.GetSaleBySubSegmentWithDiscountedValue(fromDate, toDate, Year, CityId, CountryId);
                    List<SaleByRangeOrSegmentController> ResultSubSegmentList = new List<SaleByRangeOrSegmentController>();
                    foreach (GetSaleBySubSegmentWithDiscountedValue_Result r in SubSList)
                    {
                        SaleByRangeOrSegmentController resultObj = new SaleByRangeOrSegmentController();
                        resultObj.Id = Convert.ToInt32(r.Id);
                        resultObj.BrandName = r.Name;
                        resultObj.PeriodTotal = (r.PeriodTotal == null) ? 0 : Convert.ToDecimal(r.PeriodTotal);
                        resultObj.StartYearlyTotal = (r.StartYearlyTotal == null) ? 0 : Convert.ToDecimal(r.StartYearlyTotal);
                        SubSegmentList.Add(resultObj);
                        ResultSubSegmentList.Add(resultObj);
                    }

                    int k = 0;
                    foreach (SaleByRangeOrSegmentController sb in ResultSubSegmentList)
                    {

                        System.Data.Objects.ObjectResult<GetSaleSpecialPromotionSubSegmentByCustomerId_Result> splist = entity.GetSaleSpecialPromotionSubSegmentByCustomerId(fromDate, toDate, 0, sb.Id, true, CityId, CountryId);
                        List<SpecialPromotionController> SPCList = new List<SpecialPromotionController>();

                        foreach (GetSaleSpecialPromotionSubSegmentByCustomerId_Result sp in splist)
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
                            SubSegmentList[k].PeriodTotal += SPCList[0].Sales;
                            //total += Convert.ToInt32(SPCList[0].Sales);
                        }

                        System.Data.Objects.ObjectResult<GetSaleSpecialPromotionSubSegmentByCustomerId_Result> splist2 = entity.GetSaleSpecialPromotionSubSegmentByCustomerId(yt, toDate, 0, sb.Id, true, CityId, CountryId);
                        List<SpecialPromotionController> SPCList2 = new List<SpecialPromotionController>();

                        foreach (GetSaleSpecialPromotionSubSegmentByCustomerId_Result sp in splist2)
                        {
                            SpecialPromotionController sObj = new SpecialPromotionController();
                            sObj.Name = sp.Name;
                            sObj.saleQty = Convert.ToInt32(sp.SaleQty);
                            sObj.Sales = Convert.ToInt32(sp.TotalSale);
                            sObj.refundQty = Convert.ToInt32(sp.RefundQty);
                            sObj.Refund = Convert.ToInt32(sp.TotalRefund);
                            SPCList2.Add(sObj);
                        }
                        if (SPCList2.Count > 0)
                        {
                            SubSegmentList[k].StartYearlyTotal += SPCList2[0].Sales;
                            //total += Convert.ToInt32(SPCList[0].Sales);
                        }
                        k++;

                    }
                    #endregion
                }
                ShowReportViewer();
            }
        }

        private void ShowReportViewer()
        {

            dsReportTemp dsReport = new dsReportTemp();
            //dsReportTemp.VIP_MemberDataTable dtVIPReport = (dsReportTemp.VIP_MemberDataTable)dsReport.Tables["VIP_Member"];
            dsReportTemp.SaleByRangeOrSegmentDataTable dtSaleReport = (dsReportTemp.SaleByRangeOrSegmentDataTable)dsReport.Tables["SaleByRangeOrSegment"];
            foreach (SaleByRangeOrSegmentController r in RangeList)
            {
                dsReportTemp.SaleByRangeOrSegmentRow newRow = dtSaleReport.NewSaleByRangeOrSegmentRow();
                newRow.BrandName = r.BrandName;
                newRow.PeriodTotal = r.PeriodTotal;
                newRow.StartYearlyTotal = r.StartYearlyTotal;
                dtSaleReport.AddSaleByRangeOrSegmentRow(newRow);
            }
            ReportDataSource rds1 = new ReportDataSource("DataSetRange", dsReport.Tables["SaleByRangeOrSegment"]);

            dsReportTemp dsSegmentReport = new dsReportTemp();
            dsReportTemp.SaleByRangeOrSegmentDataTable dtSaleSegmentReport = (dsReportTemp.SaleByRangeOrSegmentDataTable)dsSegmentReport.Tables["SaleByRangeOrSegment"];
            foreach (SaleByRangeOrSegmentController r in SegmentList)
            {
                dsReportTemp.SaleByRangeOrSegmentRow newRow = dtSaleSegmentReport.NewSaleByRangeOrSegmentRow();
                newRow.BrandName = r.BrandName;
                newRow.PeriodTotal = r.PeriodTotal;
                newRow.StartYearlyTotal = r.StartYearlyTotal;
                dtSaleSegmentReport.AddSaleByRangeOrSegmentRow(newRow);
            }
            ReportDataSource rds2 = new ReportDataSource("DataSetSegment", dsSegmentReport.Tables["SaleByRangeOrSegment"]);

            dsReportTemp dsSubSegmentReport = new dsReportTemp();
            dsReportTemp.SaleByRangeOrSegmentDataTable dtSubSegmentReport = (dsReportTemp.SaleByRangeOrSegmentDataTable)dsSubSegmentReport.Tables["SaleByRangeOrSegment"];
            foreach (SaleByRangeOrSegmentController r in SubSegmentList)
            {
                dsReportTemp.SaleByRangeOrSegmentRow newRow = dtSubSegmentReport.NewSaleByRangeOrSegmentRow();
                newRow.BrandName = r.BrandName;
                newRow.PeriodTotal = r.PeriodTotal;
                newRow.StartYearlyTotal = r.StartYearlyTotal;
                dtSubSegmentReport.AddSaleByRangeOrSegmentRow(newRow);
            }
            ReportDataSource rds3 = new ReportDataSource("DataSetSubSegment", dsSubSegmentReport.Tables["SaleByRangeOrSegment"]);

            string reportPath = Application.StartupPath + "\\Reports\\SaleByRangeAndSegmentAndSubSegment.rdlc";

            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds1);
            reportViewer1.LocalReport.DataSources.Add(rds2);
            reportViewer1.LocalReport.DataSources.Add(rds3);

            DateTime fromDate = dtFrom.Value.Date;
            string fDate = fromDate.Date.ToString("MM/dd/yy");
            DateTime toDate = dtTo.Value.Date;
            string tDate = toDate.Date.ToString("MM/dd/yy");
            string fromDay = fromDate.Day.ToString("dd");
            string fromMonth = fromDate.Month.ToString("MM");
            string fromYear = fromDate.Year.ToString();

            string toDay = toDate.Day.ToString("dd");
            string toMonth = toDate.Month.ToString("MM");
            string toYear = toDate.Year.ToString();
            int tYear = fromDate.Year + 1;


            ReportParameter Title = new ReportParameter("Title", "FY "+fromYear+"-"+tYear.ToString()+" Sales By Range and Segment");
            reportViewer1.LocalReport.SetParameters(Title);

            ReportParameter SubTitle = new ReportParameter("SubTitle", "( "+ fDate+ " - " + tDate+ " & 4-1-"+fromYear+" -" + tDate+" )");
            reportViewer1.LocalReport.SetParameters(SubTitle);

            ReportParameter Date1Header = new ReportParameter("Date1Header", fDate+"-"+tDate);
            reportViewer1.LocalReport.SetParameters(Date1Header);

            ReportParameter Date2Header = new ReportParameter("Date2Header", "4-1-"+fromYear+"-" + tDate);
            reportViewer1.LocalReport.SetParameters(Date2Header);

            reportViewer1.RefreshReport();
        }



        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
