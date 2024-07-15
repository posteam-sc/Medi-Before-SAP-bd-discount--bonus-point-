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
    public partial class MemberRule : Form
    {
        #region Variables

        public Boolean isEdit { get; set; }

        public int RuleID { get; set; }

        private POSEntities entity = new POSEntities();

        private ToolTip tp = new ToolTip();

        #endregion

        public MemberRule()
        {
            InitializeComponent();
        }

        private void MemberRule_Load(object sender, EventArgs e)
        {
            txtRuleName.Focus();
            txtRuleName.Select();
            dgvVIPRuleList.Columns[4].ReadOnly = true;
            dgvVIPRuleList.AutoGenerateColumns = false;
            dgvVIPRuleList.DataSource = entity.VIPMemberRules.ToList();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            bool HaveError = false;
            if (txtRuleName.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtRuleName, "Error");
                tp.Show("Please fill up VIP member rule name!", txtRuleName);
                HaveError = true;
            }
            else if (txtAmount.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtAmount, "Error");
                tp.Show("Please fill up amount!", txtAmount);
                HaveError = true;
            }
           
            if (!HaveError)
            {
                string RuleName = txtRuleName.Text.Trim();
                APP_Data.VIPMemberRule VIPRuleObj = new APP_Data.VIPMemberRule();
                APP_Data.VIPMemberRule alredyVIPMemberRuleObj = entity.VIPMemberRules.Where(x => x.RuleName.Trim() == RuleName).FirstOrDefault();
                if (alredyVIPMemberRuleObj == null)
                {
                    dgvVIPRuleList.DataSource = "";
                    VIPRuleObj.RuleName = txtRuleName.Text;
                    VIPRuleObj.Amount = Convert.ToInt32(txtAmount.Text);
                    VIPRuleObj.Remark = txtRemark.Text;
                    VIPRuleObj.IsCalculatePoints = chkCalculatePoint.Checked;
                    entity.VIPMemberRules.Add(VIPRuleObj);
                    entity.SaveChanges();
                    dgvVIPRuleList.DataSource = entity.VIPMemberRules.ToList();
                    txtRuleName.Text = "";
                    txtAmount.Text = "";
                    txtRemark.Text = "";
                    chkCalculatePoint.Checked = false;

                    MessageBox.Show("Successfully Saved!", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    #region active setting
                    if (System.Windows.Forms.Application.OpenForms["Setting"] != null)
                    {
                        Setting newForm = (Setting)System.Windows.Forms.Application.OpenForms["Setting"];
                        //newForm.ReLoadData();
                        newForm.Bind_VIPMember();
                    }
                    #endregion
                }
                else
                {
                    tp.SetToolTip(txtRuleName, "Error");
                    tp.Show("This city name is already exist!", txtRuleName);
                }
            }
        }

        private void dgvVIPRuleList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int currentId;
            if (e.RowIndex > 0)
            {
                if (e.ColumnIndex == 5)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {
                        DataGridViewRow row = dgvVIPRuleList.Rows[e.RowIndex];
                        currentId = Convert.ToInt32(row.Cells[0].Value);
                        int count = (from Cus in entity.Customers where Cus.RuleId == currentId select Cus).ToList().Count;
                        
                        if (count < 1)
                        {
                            dgvVIPRuleList.DataSource = "";
                            APP_Data.VIPMemberRule viprule = (from c in entity.VIPMemberRules where c.Id == currentId select c).FirstOrDefault();
                            entity.VIPMemberRules.Remove(viprule);
                            entity.SaveChanges();
                            dgvVIPRuleList.DataSource = entity.VIPMemberRules.ToList();
                            MessageBox.Show("Successfully Deleted!", "Delete Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            //To show message box 
                            MessageBox.Show("This VIP member rule name is currently in use!", "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }
        }
    }
}
