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
    public partial class Form7 : Form
    {
        private string connectionString = "Server=localhost;Uid=root;Pwd=123456789;Database=test;";
        public Form7()
        {
            InitializeComponent();
            LoadTeacherProjectData();
        }

        private void LoadTeacherProjectData()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Teacher_has_Project";
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind the DataTable to the DataGridView
                    dataGridView1.DataSource = dataTable;
                }

                string query2 = "SELECT ProjectID FROM Project ORDER BY ProjectID";
                using (MySqlCommand command = new MySqlCommand(query2, connection))
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
                string query3 = "SELECT TeacherID FROM teacher ORDER BY TeacherID";
                using (MySqlCommand command = new MySqlCommand(query3, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        comboBox1.Items.Clear(); // Clear existing items

                        while (reader.Read())
                        {
                            int projectId = Convert.ToInt32(reader["TeacherID"]);
                            comboBox1.Items.Add(projectId);
                        }
                    }
                }
            }
        }

        private bool IsTeacherProjectCombinationExists(int teacherId, int projectId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Teacher_has_Project WHERE TeacherID = @TeacherID AND ProjectID = @ProjectID";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TeacherID", teacherId);
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
                MessageBox.Show("Please select both a Teacher ID and a Project ID.");
                return;
            }

            int teacherId = Convert.ToInt32(comboBox1.SelectedItem);
            int projectId = Convert.ToInt32(comboBox2.SelectedItem);

            // Check if the combination of TeacherID and ProjectID already exists
            if (IsTeacherProjectCombinationExists(teacherId, projectId))
            {
                MessageBox.Show("This Teacher-Project combination already exists.");
                return;
            }

            // Perform the insertion
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO Teacher_has_Project (TeacherID, ProjectID) VALUES (@TeacherID, @ProjectID)";
                using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@TeacherID", teacherId);
                    insertCommand.Parameters.AddWithValue("@ProjectID", projectId);

                    try
                    {
                        insertCommand.ExecuteNonQuery();
                        MessageBox.Show("Teacher-Project relationship inserted successfully!");

                        // Reload data and update the DataGridView
                        LoadTeacherProjectData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error inserting Teacher-Project relationship: " + ex.Message);
                    }
                }
            }
        }
    }
}
