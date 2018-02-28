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

namespace SimpleDataApp
{
    public partial class NewCustomer : Form
    {
        public NewCustomer()
        {
            InitializeComponent();
        }

        private int parsedCustomerID;
        private int orderID;

        private bool IsCustomerNameValid()
        {
            if (tbCustomerName.Text == "")
            {
                MessageBox.Show("Please enter a name.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsOrderDataValid()
        {
            if (tbCustomerID.Text == "")
            {
                MessageBox.Show("Please create account before placing an order.");
                return false;
            }
            else if ((numOrderAmount.Value < 1))
            {
                MessageBox.Show("Please specify an order amount.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ClearForm()
        {
            tbCustomerName.Clear();
            tbCustomerID.Clear();
            dtpOrderDate.Value = DateTime.Now;
            numOrderAmount.Value = 0;
            parsedCustomerID = 0;
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            if (IsCustomerNameValid())
            {
                // Create connection
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.connString))
                {
                    //Create a command for the stored procedure
                    using (SqlCommand sqlCommand = new SqlCommand("Sales.uspNewCustomer", connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        // Add input parameters and their values to the procedure
                        sqlCommand.Parameters.Add(new SqlParameter("@CustomerName", SqlDbType.NVarChar, 40));
                        sqlCommand.Parameters["@CustomerName"].Value = tbCustomerName.Text;

                        // Add output parameter
                        sqlCommand.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int));
                        sqlCommand.Parameters["@CustomerID"].Direction = ParameterDirection.Output;

                        try
                        {
                            connection.Open();

                            // Run the procedure...
                            sqlCommand.ExecuteNonQuery();

                            // From the Sales.uspNewCustomer procedure: @CustomerID = SCOPE_IDENTITY();  
                            parsedCustomerID = (int)sqlCommand.Parameters["@CustomerID"].Value;

                            // Put the customerID in the textBox
                            tbCustomerID.Text = Convert.ToString(parsedCustomerID);
                        }
                        catch
                        {
                            MessageBox.Show("Customer ID was not returned.  Account could not be created.");
                        }
                        finally
                        {
                            connection.Close();
                        }

                    }
                }

            }
        }

        private void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (IsOrderDataValid())
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.connString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand("Sales.uspPlaceNewOrder", connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add((new SqlParameter("@CustomerID", SqlDbType.Int)));
                        sqlCommand.Parameters["@CustomerID"].Value = parsedCustomerID;

                        sqlCommand.Parameters.Add((new SqlParameter("@OrderDate", SqlDbType.DateTime)));
                        sqlCommand.Parameters["@OrderDate"].Value = dtpOrderDate.Value;

                        sqlCommand.Parameters.Add((new SqlParameter("@Amount", SqlDbType.Int)));
                        sqlCommand.Parameters["@Amount"].Value = numOrderAmount.Value;

                        sqlCommand.Parameters.Add((new SqlParameter("@Status", SqlDbType.Char, 1)));
                        sqlCommand.Parameters["@Status"].Value = "O";

                        sqlCommand.Parameters.Add(new SqlParameter("@RC", SqlDbType.Int));
                        sqlCommand.Parameters["@RC"].Direction = ParameterDirection.ReturnValue;

                        try
                        {
                            connection.Open();
                            sqlCommand.ExecuteNonQuery();

                            orderID = (int)sqlCommand.Parameters["@RC"].Value;

                            MessageBox.Show("Order Number {0} has been submitted.", orderID.ToString());
                        }
                        catch
                        {
                            MessageBox.Show("Order could not be placed.");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        private void btnAddAnotherAccount_Click(object sender, EventArgs e)
        {

        }

        private void btnFinish_Click(object sender, EventArgs e)
        {

        }
    }
}
