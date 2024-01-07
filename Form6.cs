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
    public partial class Form6 : Form
    {
        private string connectionString = "Server=localhost;Uid=root;Pwd=123456789;Database=test;";
        public Form6()
        {
            InitializeComponent();
            LoadStudentIDs();
            LoadProjectIDs();
            LoadProjectStudentData();
        }

        private void LoadStudentIDs()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT StudentID FROM Student ORDER BY StudentID";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        comboBox1.Items.Clear(); // Clear existing items

                        while (reader.Read())
                        {
                            int studentId = Convert.ToInt32(reader["StudentID"]);
                            comboBox1.Items.Add(studentId);
                        }
                    }
                }
            }
        }

        private void LoadProjectIDs()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ProjectID FROM Project ORDER BY ProjectID";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        comboBox2.Items.Clear(); // Clear existing items

                        while (reader.Read())
                        {
                            int projectId = Convert.ToInt32(reader["ProjectID"]);
                            comboBox2.Items.Add(projectId);
                        }
                    }
                }
            }
        }

        private void LoadProjectStudentData()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Project_has_Student";
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind the DataTable to the DataGridView
                    dataGridView1.DataSource = dataTable;
                }
            }
        }

        private bool IsStudentProjectCombinationExists(int studentId, int projectId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Project_has_Student WHERE StudentID = @StudentID AND ProjectID = @ProjectID";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", studentId);
                    command.Parameters.AddWithValue("@ProjectID", projectId);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Please select both a Student ID and a Project ID.");
                return;
            }

            int studentId = Convert.ToInt32(comboBox1.SelectedItem);
            int projectId = Convert.ToInt32(comboBox2.SelectedItem);

            // Check if the combination of StudentID and ProjectID already exists
            if (IsStudentProjectCombinationExists(studentId, projectId))
            {
                MessageBox.Show("This Student-Project combination already exists.");
                return;
            }

            // Perform the assignment
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO Project_has_Student (ProjectID, StudentID) VALUES (@ProjectID, @StudentID)";
                using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@ProjectID", projectId);
                    insertCommand.Parameters.AddWithValue("@StudentID", studentId);

                    try
                    {
                        insertCommand.ExecuteNonQuery();
                        MessageBox.Show("Student assigned to project successfully!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error assigning student to project: " + ex.Message);
                    }
                }
            }
        }
    }
}
