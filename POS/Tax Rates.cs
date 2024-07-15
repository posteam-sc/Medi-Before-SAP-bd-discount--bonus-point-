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
    public partial class Taxes : Form
    {
        #region Variables

        POSEntities posEntity = new POSEntities();
        private ToolTip tp = new ToolTip();
        int currentId;
        #endregion
        #region Event

        public Taxes()
        {
            InitializeComponent();
        }

        private void Taxes_Load(object sender, EventArgs e)
        {
            dgvTaxList.AutoGenerateColumns = false;
            dgvTaxList.DataSource = posEntity.Taxes.ToList();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Boolean hasError = false;

            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            //Validation
            if (txtName.Text.Trim() == string.Empty) 
            {
                tp.SetToolTip(txtName, "Error");
                tp.Show("Please fill up tax name!", txtName);
                hasError = true;
            }

            if (txtPercent.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtPercent, "Error");
                tp.Show("Please fill up tax percent!", txtPercent);
                hasError = true;
            }
            if (!hasError)
            {
                if (lblStatus.Text == "Add")
                {
                    Tax taxObj = new Tax();
                    Tax taxObj2 = (from t in posEntity.Taxes where t.Name == txtName.Text select t).FirstOrDefault();
                    if (taxObj2 == null)
                    {
                        
                        taxObj.Name = txtName.Text;
                        taxObj.TaxPercent = Convert.ToDecimal(txtPercent.Text);
                        posEntity.Taxes.Add(taxObj);
                        posEntity.SaveChanges();                        
                        MessageBox.Show("Successfully Saved!", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dgvTaxList.DataSource = "";
                        dgvTaxList.DataSource = posEntity.Taxes.ToList();
                        Clear();
                        #region active PaidByCreditWithPrePaidDebt
                        if (System.Windows.Forms.Application.OpenForms["Setting"] != null)
                        {
                            Setting newForm = (Setting)System.Windows.Forms.Application.OpenForms["Setting"];
                            newForm.ReLoadData();
                        }
                        #endregion
                    }
                    else
                    {
                        tp.SetToolTip(txtName, "Error");
                        tp.Show("This tax name is already exist!", txtName);
                    }
                }
                else
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to update?", "Update", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {
                        int count;
                        Tax taxObj = (from t in posEntity.Taxes where t.Id == currentId select t).FirstOrDefault();
                        count = (from t in posEntity.Taxes where t.Id != currentId && t.Name == txtName.Text select t).ToList().Count;
                        if (count == 0)
                        {
                           
                            taxObj.Name = txtName.Text;
                            taxObj.TaxPercent = Convert.ToDecimal(txtPercent.Text);
                            posEntity.Entry(taxObj).State = EntityState.Modified;                            
                            MessageBox.Show("Successfully Update!", "Update Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            dgvTaxList.DataSource = "";
                            dgvTaxList.DataSource = posEntity.Taxes.ToList();
                            posEntity.SaveChanges();
                            Clear();
                        }
                        else
                        {
                            tp.SetToolTip(txtName, "Error");
                            tp.Show("This tax name is already exist!", txtName);
                        }
                        
                    }
                    else
                    {
                        Clear();
                    }

                }
            }
        }

        private void dgvTaxList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex >= 0)
            {
                //to edit
                if (e.ColumnIndex == 3)
                {
                    DataGridViewRow row = dgvTaxList.Rows[e.RowIndex];
                    currentId = Convert.ToInt32(row.Cells[0].Value);
                    Tax taxObj1 = (from t in posEntity.Taxes where t.Id == currentId select t).FirstOrDefault();
                    txtName.Text = taxObj1.Name;
                    txtPercent.Text = taxObj1.TaxPercent.ToString();
                    lblStatus.Text = "UPDATE";
                    btnAdd.Image = Properties.Resources.save_small;
                    this.Text = "Edit Tax Rate";

                }
                if (e.ColumnIndex == 4)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {
                        DataGridViewRow row = dgvTaxList.Rows[e.RowIndex];
                        currentId = Convert.ToInt32(row.Cells[0].Value);
                        Tax taxObj1 = (from t in posEntity.Taxes where t.Id == currentId select t).FirstOrDefault();
                        int count = (from p in posEntity.Products where p.TaxId == currentId select p).ToList().Count;
                        string cId = currentId.ToString();
                        APP_Data.Setting sObj = (from s in posEntity.Settings where s.Value == cId && s.Key == "default_tax_rate" select s).FirstOrDefault();
                        if (count > 0 || sObj != null)
                        {
                            //To show message box 
                            MessageBox.Show("This tax rate is currently in use!", "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            dgvTaxList.DataSource = "";
                            posEntity.Taxes.Remove(taxObj1);
                            posEntity.SaveChanges();
                            dgvTaxList.DataSource = posEntity.Taxes.ToList();
                        }
                    }
                }
            }
        }

        private void Taxes_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtName);
            tp.Hide(txtPercent);
        }
        #endregion

        #region Function

        private void Clear()
        {
            txtName.Text = "";
            txtPercent.Text = "";
            btnAdd.Text = "";
        }
        #endregion




    }
}
