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
using System.Text.RegularExpressions;

namespace SimpleDataApp
{
    public partial class FillOrCancel : Form
    {
        public FillOrCancel()
        {
            InitializeComponent();
        }

        private int parsedOrderID;

        private bool IsOrderIDValid()
        {
            if (tbOrderID.Text == "")
            {
                MessageBox.Show("Please specify the order ID.");
                return false;
            }
            else if (Regex.IsMatch(tbOrderID.Text, @"^\D*$"))
            {
                MessageBox.Show("Order ID must contain only numbers.");
                tbOrderID.Clear();
                return false;
            }
            else
            {
                parsedOrderID = Int32.Parse(tbOrderID.Text);
                return true;
            }
        }



        private void btnFindByOrderID_Click(object sender, EventArgs e)
        {
            if (IsOrderIDValid())
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.connString))
                {
                    const string sql = "SELECT * FROM Sales.Orders WHERE orderID = @OrderID";

                    using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                    {
                        sqlCommand.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.Int));
                        sqlCommand.Parameters["@OrderID"].Value = parsedOrderID;

                        try
                        {
                            connection.Open();

                            // Run the query with ExecuteReader
                            using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                            {
                                // Create a data table to hold the returned data
                                DataTable dataTable = new DataTable();

                                // Load the data from the reader into the table
                                dataTable.Load(dataReader);

                                // Display the table data into the dataGridView control
                                dgvCustomerOrders.DataSource = dataTable;

                                dataReader.Close();
                            }
                        }
                        catch
                        {
                            MessageBox.Show("The requested order could not be loaded.");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            if (IsOrderIDValid())
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.connString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand("Sales.uspCancelOrder", connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add(new SqlParameter("@orderID", SqlDbType.Int));
                        sqlCommand.Parameters["@orderID"].Value = parsedOrderID;
                    }
                }
            }
        }

        private void btnFillOrder_Click(object sender, EventArgs e)
        {

        }

        private void btnFinishUpdates_Click(object sender, EventArgs e)
        {

        }
    }
}
