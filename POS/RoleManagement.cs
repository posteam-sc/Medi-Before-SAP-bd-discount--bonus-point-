using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS
{
    public partial class RoleManagement : Form
    {        
        #region Events

        public RoleManagement()
        {
            InitializeComponent();
        }

        private void RoleManagement_Load(object sender, EventArgs e)
        {
            #region Load Data for Super Casher Role

            RoleManagementController controller = new RoleManagementController();
            //Super Casher Id
            controller.Load(2);            

            //Product
            chkViewProductSC.Checked = controller.Product.View;
            chkEditProductSC.Checked =controller.Product.EditOrDelete ;
            chkAddProductSC.Checked = controller.Product.Add ;

            //Brand
            chkViewBrandSC.Checked = controller.Brand.View ;
            chkEditBrandSC.Checked = controller.Brand.EditOrDelete;
            chkAddBrandSC.Checked = controller.Brand.Add ;

            //Giftcard
            chkViewGiftcardSC.Checked = controller.GiftCard.View ;
            chkAddGiftCardSC.Checked = controller.GiftCard.Add ;
            chkDeleteGiftCardSC.Checked = controller.GiftCard.EditOrDelete;

            //Customer
            chkViewCustomerSC.Checked = controller.Customer.View ;
            chkEditCustomerSC.Checked = controller.Customer.EditOrDelete ;
            chkAddCustomerSC.Checked = controller.Customer.Add ;

            //Category
            chkViewCategorySC.Checked = controller.Category.View ;
            chkEditCategorySC.Checked = controller.Category.EditOrDelete ;
            chkAddCategorySC.Checked = controller.Category.Add ;

            //SubCategory
            chkViewSubCategorySC.Checked = controller.SubCategory.View ;
            chkEditSubCategorySC.Checked = controller.SubCategory.EditOrDelete ;
            chkAddSubCategorySC.Checked = controller.SubCategory.Add ;

            //Counter            
            chkEditCounterSC.Checked = controller.Counter.EditOrDelete ;
            chkAddCounterSC.Checked = controller.Counter.Add ;

            //Supplier
            chkEditSupplierSC.Checked = controller.Supplier.EditOrDelete;
            chkAddSupplierSC.Checked = controller.Supplier.Add;
            chkViewSupplierSC.Checked = controller.Supplier.View;

            //Promotion
            chkEditPromotionSC.Checked = controller.Promotion.EditOrDelete;
            chkAddPromotionSC.Checked = controller.Promotion.Add;
            chkViewPromotionSC.Checked = controller.Promotion.View;

            //Novelty
            chkEditNoveltySC.Checked = controller.Novelty.EditOrDelete;
            chkAddNoveltySC.Checked = controller.Novelty.Add;
            chkViewNoveltySC.Checked = controller.Novelty.View;

            //Reports
            chkTransactionDetailSC.Checked = controller.TransactionDetailReport ;
            chkTransactionSC.Checked = controller.TransactionReport ;
            chkItemSummarySC.Checked = controller.ItemSummaryReport ;
            chkTaxSummarySC.Checked = controller.TaxSummaryReport ;
            chkReorderPointSC.Checked = controller.ReorderPointReport ;
            chkOutstandingSC.Checked = controller.OutstandingCustomerReport ;
            chkTopBestSellerSC.Checked = controller.TopBestSellerReport ;
            chkTransactionSummarySC.Checked = controller.TransactionSummaryReport;
            chkGWPSC.Checked = controller.GWPTransaction;
            chkMenberSC.Checked = controller.MemberWeekly;
            chkMenberInfoSC.Checked = controller.MemberInfo;
            chkEmailCompilationSC.Checked = controller.EmailCompilation;
            chkSaleBreakDownSC.Checked = controller.SaleBreakDown;
            chkSaleByRangeandSegmentSC.Checked = controller.SaleByRangeAndSegment;
            

            #endregion

            #region Load Data for Casher Role

            RoleManagementController controllerCasher = new RoleManagementController();

            //Super Casher Id
            controllerCasher.Load(3);

            //Product
            chkViewProductC.Checked = controllerCasher.Product.View;
            chkEditProductC.Checked = controllerCasher.Product.EditOrDelete;
            chkAddProductC.Checked = controllerCasher.Product.Add;

            //Brand
            chkViewBrandC.Checked = controllerCasher.Brand.View;
            chkEditBrandC.Checked = controllerCasher.Brand.EditOrDelete;
            chkAddBrandC.Checked = controllerCasher.Brand.Add;

            //Giftcard
            chkViewGiftcardC.Checked = controllerCasher.GiftCard.View;
            chkAddGiftcardC.Checked = controllerCasher.GiftCard.Add;
            chkDeleteGiftCardC.Checked = controllerCasher.GiftCard.EditOrDelete;

            //Customer
            chkViewCustomerC.Checked = controllerCasher.Customer.View;
            chkEditCustomerC.Checked = controllerCasher.Customer.EditOrDelete;
            chkAddCustomerC.Checked = controllerCasher.Customer.Add;

            //Category
            chkViewCategoryC.Checked = controllerCasher.Category.View;
            chkEditCategoryC.Checked = controllerCasher.Category.EditOrDelete;
            chkAddCategoryC.Checked = controllerCasher.Category.Add;

            //SubCategory
            chkViewSubCategoryC.Checked = controllerCasher.SubCategory.View;
            chkEditSubCategoryC.Checked = controllerCasher.SubCategory.EditOrDelete;
            chkAddSubCategoryC.Checked = controllerCasher.SubCategory.Add;

            //Counter            
            chkEditCounterC.Checked = controllerCasher.Counter.EditOrDelete;
            chkAddCounterC.Checked = controllerCasher.Counter.Add;

            //Supplier
            chkEditSupplierC.Checked = controllerCasher.Supplier.EditOrDelete;
            chkAddSupplierC.Checked = controllerCasher.Supplier.Add;
            chkViewSupplierC.Checked = controllerCasher.Supplier.View;

            //Promotion
            chkEditPromotionC.Checked = controllerCasher.Promotion.EditOrDelete;
            chkAddPromotionC.Checked = controllerCasher.Promotion.Add;
            chkViewPromotionC.Checked = controllerCasher.Promotion.View;

            //Novelty
            chkEditNoveltyC.Checked = controllerCasher.Novelty.EditOrDelete;
            chkAddNoveltyC.Checked = controllerCasher.Novelty.Add;
            chkViewNoveltyC.Checked = controllerCasher.Novelty.View;

            //Reportsg
            chkTransactionDetailC.Checked = controllerCasher.TransactionDetailReport;
            chkTransactionC.Checked = controllerCasher.TransactionReport;
            chkItemSummaryC.Checked = controllerCasher.ItemSummaryReport;
            chkTaxSummaryC.Checked = controllerCasher.TaxSummaryReport;
            chkReorderPointC.Checked = controllerCasher.ReorderPointReport;
            chkOutstandingC.Checked = controllerCasher.OutstandingCustomerReport;
            chkTopBestSellerC.Checked = controllerCasher.TopBestSellerReport;
            chkTransactionSummaryC.Checked = controllerCasher.TransactionSummaryReport;
            chkGWPC.Checked = controllerCasher.GWPTransaction;
            chkMenberC.Checked = controllerCasher.MemberWeekly;
            chkMenberInfoC.Checked = controllerCasher.MemberInfo;
            chkEmailCompilationC.Checked = controllerCasher.EmailCompilation;
            chkSaleBreakDownC.Checked = controllerCasher.SaleBreakDown;
            chkSaleByRangeandSegmentC.Checked = controllerCasher.SaleByRangeAndSegment;
            #endregion
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            #region Save for Super Casher Role

            RoleManagementController controller = new RoleManagementController();
                      
            //Product
            controller.Product.View = chkViewProductSC.Checked;
            controller.Product.EditOrDelete = chkEditProductSC.Checked;
            controller.Product.Add = chkAddProductSC.Checked;

            //Brand
            controller.Brand.View = chkViewBrandSC.Checked;
            controller.Brand.EditOrDelete = chkEditBrandSC.Checked;
            controller.Brand.Add = chkAddBrandSC.Checked;

            //Giftcard
            controller.GiftCard.View = chkViewGiftcardSC.Checked;
            controller.GiftCard.Add = chkAddGiftCardSC.Checked;
            controller.GiftCard.EditOrDelete = chkDeleteGiftCardSC.Checked;

            //Customer
            controller.Customer.View = chkViewCustomerSC.Checked;
            controller.Customer.EditOrDelete = chkEditCustomerSC.Checked;
            controller.Customer.Add = chkAddCustomerSC.Checked;

            //Category
            controller.Category.View = chkViewCategorySC.Checked;
            controller.Category.EditOrDelete = chkEditCategorySC.Checked;
            controller.Category.Add = chkAddCategorySC.Checked;

            //SubCategory
            controller.SubCategory.View = chkViewSubCategorySC.Checked;
            controller.SubCategory.EditOrDelete = chkEditSubCategorySC.Checked;
            controller.SubCategory.Add = chkAddSubCategorySC.Checked;

            //Counter            
            controller.Counter.EditOrDelete = chkEditCounterSC.Checked;
            controller.Counter.Add = chkAddCounterSC.Checked;

            //Supplier
            controller.Supplier.EditOrDelete = chkEditSupplierSC.Checked;
            controller.Supplier.Add = chkAddSupplierSC.Checked;
            controller.Supplier.View = chkViewSupplierSC.Checked;

            //Promotion
            controller.Promotion.EditOrDelete = chkEditPromotionSC.Checked;
            controller.Promotion.Add = chkAddPromotionSC.Checked;
            controller.Promotion.View = chkViewPromotionSC.Checked;

            //Novelty
            controller.Novelty.EditOrDelete = chkEditNoveltySC.Checked;
            controller.Novelty.Add = chkAddNoveltySC.Checked;
            controller.Novelty.View = chkViewNoveltySC.Checked;

            //Reports
            controller.TransactionDetailReport = chkTransactionDetailSC.Checked;
            controller.TransactionReport = chkTransactionSC.Checked;
            controller.ItemSummaryReport = chkItemSummarySC.Checked;
            controller.TaxSummaryReport = chkTaxSummarySC.Checked;
            controller.ReorderPointReport = chkReorderPointSC.Checked;
            controller.OutstandingCustomerReport = chkOutstandingSC.Checked;
            controller.TopBestSellerReport = chkTopBestSellerSC.Checked;
            controller.TransactionSummaryReport = chkTransactionSummarySC.Checked;
            controller.GWPTransaction = chkGWPSC.Checked;
            controller.MemberWeekly = chkMenberSC.Checked;
            controller.MemberInfo = chkMenberInfoSC.Checked;
            controller.EmailCompilation = chkEmailCompilationSC.Checked;
            controller.SaleBreakDown = chkSaleBreakDownSC.Checked;
            controller.SaleByRangeAndSegment = chkSaleByRangeandSegmentSC.Checked;
            //Super Casher Id
            controller.Save(2);

            #endregion

            #region Save for Casher Role

            RoleManagementController controllerCasher = new RoleManagementController();                        

            //Product
            controllerCasher.Product.View = chkViewProductC.Checked;
            controllerCasher.Product.EditOrDelete = chkEditProductC.Checked;
            controllerCasher.Product.Add = chkAddProductC.Checked;

            //Brand
            controllerCasher.Brand.View = chkViewBrandC.Checked;
            controllerCasher.Brand.EditOrDelete = chkEditBrandC.Checked;
            controllerCasher.Brand.Add = chkAddBrandC.Checked;

            //Giftcard
            controllerCasher.GiftCard.View = chkViewGiftcardC.Checked;
            controllerCasher.GiftCard.Add = chkAddGiftcardC.Checked;
            controllerCasher.GiftCard.EditOrDelete = chkDeleteGiftCardC.Checked;

            //Customer
            controllerCasher.Customer.View = chkViewCustomerC.Checked;
            controllerCasher.Customer.EditOrDelete = chkEditCustomerC.Checked;
            controllerCasher.Customer.Add = chkAddCustomerC.Checked;

            //Category
            controllerCasher.Category.View = chkViewCategoryC.Checked;
            controllerCasher.Category.EditOrDelete = chkEditCategoryC.Checked;
            controllerCasher.Category.Add = chkAddCategoryC.Checked;

            //SubCategory
            controllerCasher.SubCategory.View = chkViewSubCategoryC.Checked;
            controllerCasher.SubCategory.EditOrDelete = chkEditSubCategoryC.Checked;
            controllerCasher.SubCategory.Add = chkAddSubCategoryC.Checked;

            //Counter            
            controllerCasher.Counter.EditOrDelete = chkEditCounterC.Checked;
            controllerCasher.Counter.Add = chkAddCounterC.Checked;

            //Supplier
            controllerCasher.Supplier.EditOrDelete = chkEditSupplierC.Checked;
            controllerCasher.Supplier.Add = chkAddSupplierC.Checked;
            controllerCasher.Supplier.View = chkViewSupplierC.Checked;

            //Promotion
            controllerCasher.Promotion.EditOrDelete = chkEditPromotionC.Checked;
            controllerCasher.Promotion.Add = chkAddPromotionC.Checked;
            controllerCasher.Promotion.View = chkViewPromotionC.Checked;

            //Novelty
            controllerCasher.Novelty.EditOrDelete = chkEditNoveltyC.Checked;
            controllerCasher.Novelty.Add = chkAddNoveltyC.Checked;
            controllerCasher.Novelty.View = chkViewNoveltyC.Checked;

            //Reports
            controllerCasher.TransactionDetailReport = chkTransactionDetailC.Checked;
            controllerCasher.TransactionReport = chkTransactionC.Checked;
            controllerCasher.ItemSummaryReport = chkItemSummaryC.Checked;
            controllerCasher.TaxSummaryReport = chkTaxSummaryC.Checked;
            controllerCasher.ReorderPointReport = chkReorderPointC.Checked;
            controllerCasher.OutstandingCustomerReport = chkOutstandingC.Checked;
            controllerCasher.TopBestSellerReport = chkTopBestSellerC.Checked;
            controllerCasher.TransactionSummaryReport = chkTransactionSummaryC.Checked;
            controllerCasher.GWPTransaction = chkGWPC.Checked;
            controllerCasher.MemberWeekly = chkMenberC.Checked;
            controllerCasher.MemberInfo = chkMenberInfoC.Checked;
            controllerCasher.EmailCompilation = chkEmailCompilationC.Checked;
            controllerCasher.SaleBreakDown = chkSaleBreakDownC.Checked;
            controllerCasher.SaleByRangeAndSegment = chkSaleByRangeandSegmentC.Checked;
            //Casher Id
            controllerCasher.Save(3);

            #endregion

            MessageBox.Show(" Saving Completed!");
            this.Dispose();
        }

        #endregion               
    }
}
