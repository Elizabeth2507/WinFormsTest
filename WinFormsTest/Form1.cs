using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;
using Microsoft.Office;
using ClosedXML.Excel;
using System.IO;

namespace WinFormsTest
{
    public partial class Form1 : Form
    {
        private SqlConnection conn = new SqlConnection();
        private string conString = "Server=(local); Database=TestDB; Trusted_Connection=Yes";
        private SqlCommand cmd;
        DataTable dt;

        public Form1()
        {
            InitializeComponent();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            cmbPosition.SelectedIndex = 0;
            refreshData();
        }

        private void handleException(Exception ex)
        {
            string msg = ex.Message.ToString();
            string caption = "Error";

            MessageBox.Show(msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void refreshData()
        {
            conn.ConnectionString = conString;
            cmd = conn.CreateCommand();

            try
            {
                string query = "select * from EmployeeInfo;";
                cmd.CommandText = query;

                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                dt = new DataTable();
                //dv = new DataView(dt);
                dt.Load(reader);

                dataGridView1.DataSource = dt;
                
               
            }
            catch (Exception ex)
            {
                handleException(ex);
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string empName = tbName.Text;
            string empSurname = tbSurname.Text;
            string empBirthYear = tbBirthYear.Text;
            string empPosition = cmbPosition.SelectedItem.ToString();
            string empSalary = tbSalary.Text.ToString();

            if ((empName == "") || (empSurname == "") || (empBirthYear == "") || (empPosition == "")
                || (empSalary == ""))
            {
                string msg = " No textbox can be empty";
                string caption = "Error";
                MessageBox.Show(msg, caption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                conn.ConnectionString = conString;
                cmd = conn.CreateCommand();
            }
            try
            {
                string query = "Insert into EmployeeInfo values('"
                                + empName + "','"
                                + empSurname + "','"
                                + empBirthYear + "','"
                                + empPosition + "','"
                                + empSalary + "');";
                cmd.CommandText = query;
                conn.Open();
                cmd.ExecuteScalar();

                MessageBox.Show("New Employee Added Successfully", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                handleException(ex);
            }
            finally
            {
                cmd.Dispose();
                conn.Close();

                refreshData();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int selectedIndex = dataGridView1.SelectedRows[0].Index;

            // gets the RowID from the first column in the grid
            int flID = int.Parse(dataGridView1[0, selectedIndex].Value.ToString());
            conn.ConnectionString = conString;
            cmd = conn.CreateCommand();

            try
            {
                string query = "Delete from EmployeeInfo where ID=" + flID;
                cmd.CommandText = query;
                conn.Open();
                cmd.ExecuteScalar();

                MessageBox.Show("Employee Deleted", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                handleException(ex);
            }
            finally
            {
                cmd.Dispose();
                conn.Close();

                refreshData();
            }
        }

        private void cmbSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
                     
        }

        private void tbFilter_TextChanged(object sender, EventArgs e)
        {
            DataView dv = new DataView(dt);
            dv.RowFilter = string.Format("Position like '%{0}%'", tbFilter.Text);
            dataGridView1.DataSource = dv;
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            conn.ConnectionString = conString;
            cmd = conn.CreateCommand();
            //Creating new DataTable
            DataTable dtb = new DataTable();
            try
            {
                string query = "select  Position, Avg(Salary) as AvarageSalary from EmployeeInfo group by Position; ";
                cmd.CommandText = query;
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                //dv = new DataView(dt);
                dtb.Clear();
                dtb.Load(reader);

                
            }
            catch (Exception ex)
            {
                handleException(ex);
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }

            //Exporting to Excel
            
            string folderPath = "C:\\Excel\\";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dtb, "Report");
                wb.SaveAs(folderPath + "PositionSalary.xlsx");

                MessageBox.Show("Report was Generated", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void tbBirthYear_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(tbBirthYear.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                tbBirthYear.Text = tbBirthYear.Text.Remove(tbBirthYear.Text.Length - 1);
            }
        }

        private void tbSalary_TextChanged(object sender, EventArgs e)
        {
            
           
        }
    }
}
