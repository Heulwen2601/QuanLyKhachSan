﻿using HotelManager.DAO;
using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace HotelManager
{
    public partial class fReport : Form
    {
        private int month = 1;
        private int year = 1990;
        public fReport()
        {
            InitializeComponent();
            dataGridReport.Font = new System.Drawing.Font("Segoe UI", 9.75F);
        }

        #region Load
        private void LoadFullReport(int month, int year)
        {
            this.month = month;
            this.year = year;
            DataTable table = GetFulReport(month, year);
            BindingSource source = new BindingSource();
            //ChangePrice(table);
            source.DataSource = table;
            dataGridReport.DataSource = source;
            bindingReport.BindingSource = source;
            DrawChart(source);
            GC.Collect();
        }
        #endregion

        #region Click
        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadFullReport(int.Parse(comboBoxMonth.Text), (int)(numericYear.Value));
        }
        private void ToolStripLabel1_Click(object sender, EventArgs e)
        {
            saveReport.FileName = "Doanh thu tháng " + month + '-' + year;
            if (saveReport.ShowDialog() == DialogResult.Cancel)
                return;
            else
            {
                bool check;
                try
                {
                    switch (saveReport.FilterIndex)
                    {
                        case 2:
                            check = ExportToExcel.Instance.Export(dataGridReport, saveReport.FileName, ModeExportToExcel.XLSX);
                            break;
                        case 3:
                            check = ExportToExcel.Instance.Export(dataGridReport, saveReport.FileName, ModeExportToExcel.PDF);
                            break;
                        default:
                            check = ExportToExcel.Instance.Export(dataGridReport, saveReport.FileName, ModeExportToExcel.XLS);
                            break;
                    }
                    if (check)
                        MessageBox.Show( "Xuất thành công", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show( "Lỗi xuất thất bại", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch
                {
                    MessageBox.Show( "Lỗi (Cần cài đặt Office)", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Data
        private DataTable GetFulReport(int month, int year)
        {
            return ReportDAO.Instance.LoadFullReport(month, year);
        }

        #endregion

        #region Chart
        private void DrawChart(BindingSource source)
        {   
            chartReport.DataSource = source;
            chartReport.DataBind();
            foreach (DataPoint item in chartReport.Series[0].Points)
            {
                if(item.YValues[0] == 0)
                {
                    item.Label = " ";
                }
            }
        }
        #endregion

        #region Form
        private void FReport_Load(object sender, EventArgs e)
        {
            LoadFullReport(DateTime.Now.Month, DateTime.Now.Year);
            comboBoxMonth.Text = DateTime.Now.Month.ToString();
            numericYear.Value = DateTime.Now.Year;
        }
        #endregion

        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chartReport_Click(object sender, EventArgs e)
        {

        }
    }
}
