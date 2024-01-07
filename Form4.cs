using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseOdevi3
{
    public partial class Form4 : Form
    {
        private string connectionString = "Server=localhost;Uid=root;Pwd=123456789;Database=test;";
        public Form4()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            LoadProjectData();
        }
        private void LoadProjectData()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Project";
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind the DataTable to the DataGridView
                    dataGridView1.DataSource = dataTable;
                }
                string projectIdsQuery = "SELECT ProjectID FROM Project";
                using (MySqlCommand command = new MySqlCommand(projectIdsQuery, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        comboBoxProjectID.Items.Clear(); // Clear existing items in comboBoxProjectID
                        comboBox1.Items.Clear();
                        comboBoxProjectIDs.Items.Clear();

                        while (reader.Read())
                        {
                            // Assuming ProjectID is an integer, adjust the type accordingly
                            int projectId = Convert.ToInt32(reader["ProjectID"]);
                            comboBoxProjectID.Items.Add(projectId);
                            comboBox1.Items.Add(projectId);
                            comboBoxProjectIDs.Items.Add(projectId);
                        }
                    }
                }


            }
            UpdateAverageBudgetLabel();
            UpdateProjectCountLabel();
            UpdateSumOfBudgetsLabel();
            UpdateAvailableProjectsCountLabel();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Validate input before proceeding
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Please fill in all the required fields.");
                return;
            }

            int projectID;
            if (!int.TryParse(textBox1.Text, out projectID))
            {
                MessageBox.Show("Project ID must be an integer.");
                return;
            }

            string projectName = textBox2.Text;
            string projectDescription = textBox3.Text;

            int budget;
            if (!int.TryParse(textBox4.Text, out budget))
            {
                MessageBox.Show("Budget must be an integer.");
                return;
            }

            // Get the availability from the checkbox
            int availability = checkBox1.Checked ? 1 : 0;

            // Get the start date and end date from date pickers
            DateTime startDate = dateTimePicker1.Value;
            DateTime endDate = dateTimePicker2.Value;

            // Insert the new project into the Project table
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO Project (ProjectID, ProjectName, ProjectDescription, StartDate, EndDate, Budget, Availability) " +
                                     "VALUES (@ProjectID, @ProjectName, @ProjectDescription, @StartDate, @EndDate, @Budget, @Availability)";

                using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ProjectID", projectID);
                    command.Parameters.AddWithValue("@ProjectName", projectName);
                    command.Parameters.AddWithValue("@ProjectDescription", projectDescription);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    command.Parameters.AddWithValue("@Budget", budget);
                    command.Parameters.AddWithValue("@Availability", availability);

                    try
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Project added successfully!");

                        // Reload data and update the DataGridView
                        LoadProjectData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error adding project: " + ex.Message);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            {
                // Validate input before proceeding
                if (comboBoxProjectID.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(textBox6.Text) ||
                    string.IsNullOrWhiteSpace(textBox7.Text) ||
                    string.IsNullOrWhiteSpace(textBox8.Text))
                {
                    MessageBox.Show("Please fill in all the required fields.");
                    return;
                }

                int projectID = (int)comboBoxProjectID.SelectedItem;
                string projectName = textBox6.Text;
                string projectDescription = textBox7.Text;

                int budget;
                if (!int.TryParse(textBox8.Text, out budget))
                {
                    MessageBox.Show("Budget must be an integer.");
                    return;
                }

                // Get the availability from the checkbox
                int availability = checkBox3.Checked ? 1 : 0;

                // Get the start date and end date from date pickers
                DateTime startDate = dateTimePicker4.Value;
                DateTime endDate = dateTimePicker3.Value;

                // Update the project in the Project table
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string updateQuery = "UPDATE Project " +
                                         "SET ProjectName = @ProjectName, " +
                                         "    ProjectDescription = @ProjectDescription, " +
                                         "    StartDate = @StartDate, " +
                                         "    EndDate = @EndDate, " +
                                         "    Budget = @Budget, " +
                                         "    Availability = @Availability " +
                                         "WHERE ProjectID = @ProjectID";

                    using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ProjectID", projectID);
                        command.Parameters.AddWithValue("@ProjectName", projectName);
                        command.Parameters.AddWithValue("@ProjectDescription", projectDescription);
                        command.Parameters.AddWithValue("@StartDate", startDate);
                        command.Parameters.AddWithValue("@EndDate", endDate);
                        command.Parameters.AddWithValue("@Budget", budget);
                        command.Parameters.AddWithValue("@Availability", availability);

                        try
                        {
                            command.ExecuteNonQuery();
                            MessageBox.Show("Project updated successfully!");

                            // Reload data and update the DataGridView
                            LoadProjectData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error updating project: " + ex.Message);
                        }
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Check if an item is selected in ComboBox1
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a Project ID to delete.");
                return;
            }

            // Get the selected Project ID
            int selectedProjectID = (int)comboBox1.SelectedItem;

            // Confirm deletion with the user
            DialogResult result = MessageBox.Show("Are you sure you want to delete this project?", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                // Perform the deletion
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Execute the DELETE query
                    string deleteQuery = "DELETE FROM Project WHERE ProjectID = @ProjectID";
                    using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@ProjectID", selectedProjectID);

                        try
                        {
                            deleteCommand.ExecuteNonQuery();
                            MessageBox.Show("Project deleted successfully!");

                            // Reload data and update the DataGridView
                            LoadProjectData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error deleting project: " + ex.Message);
                        }
                    }
                }
            }
        }
        private void UpdateAverageBudgetLabel()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT AVG(Budget) FROM Project";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        double averageBudget = Convert.ToDouble(result);
                        label17.Text = $"Average Budget: {averageBudget:C2}";
                    }
                    else
                    {
                        label17.Text = "No data available";
                    }
                }
            }
        }
        private void UpdateSumOfBudgetsLabel()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT SUM(Budget) FROM Project";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        double sumOfBudgets = Convert.ToDouble(result);
                        label18.Text = $"Sum of Budgets: {sumOfBudgets:C2}";
                    }
                    else
                    {
                        label18.Text = "No data available";
                    }
                }
            }
        }

        private void UpdateProjectCountLabel()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Project";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    int projectCount = Convert.ToInt32(command.ExecuteScalar());
                    label19.Text = $"Project Count: {projectCount}";
                }
            }
        }

        private void UpdateAvailableProjectsCountLabel()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Project WHERE Availability = 1";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    int availableProjectsCount = Convert.ToInt32(command.ExecuteScalar());
                    label20.Text = $"Available Projects Count: {availableProjectsCount}";
                }
            }
        }
    }
}
