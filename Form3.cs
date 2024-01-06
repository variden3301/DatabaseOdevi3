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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DatabaseOdevi3
{
    public partial class Form3 : Form
    {
        private string connectionString = "Server=localhost;Uid=root;Pwd=123456789;Database=test;";
        public Form3()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Student";
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
                {
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);

                    // Bind the DataTable to the DataGridView
                    dataGridView1.DataSource = dataTable;
                }
                string studentIdQuery = "SELECT StudentID FROM Student ORDER BY StudentID ASC";
                using (MySqlCommand studentIdCommand = new MySqlCommand(studentIdQuery, connection))
                {
                    using (MySqlDataReader reader = studentIdCommand.ExecuteReader())
                    {
                        comboBox1.Items.Clear(); // Clear existing items
                        comboBox2.Items.Clear();

                        while (reader.Read())
                        {
                            // Assuming StudentID is an integer, adjust the type accordingly
                            int studentId = Convert.ToInt32(reader["StudentID"]);
                            comboBox1.Items.Add(studentId);
                            comboBox2.Items.Add(studentId);
                        }
                    }
                }
            }
            UpdateAverageSemesterLabel();
            UpdateStudentCountLabel();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int intValue1;
            if (!int.TryParse(textBox1.Text, out intValue1))
            {
                MessageBox.Show("Please enter a valid integer for TextBox1.");
                return;
            }

            string stringValue1 = textBox2.Text;
            string stringValue2 = textBox3.Text;

            int intValue2;
            if (!int.TryParse(textBox4.Text, out intValue2))
            {
                MessageBox.Show("Please enter a valid integer for TextBox4.");
                return;
            }

            string stringValue3 = textBox5.Text;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO student (StudentID , StudentName, Email, Semester, Role) VALUES (@Value1, @Value2, @Value3, @Value4, @Value5)";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Value1", intValue1);
                    command.Parameters.AddWithValue("@Value2", stringValue1);
                    command.Parameters.AddWithValue("@Value3", stringValue2);
                    command.Parameters.AddWithValue("@Value4", intValue2);
                    command.Parameters.AddWithValue("@Value5", stringValue3);

                    try
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Record inserted successfully!");
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error inserting record: " + ex.Message);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a student ID to delete.");
                return;
            }

            // Get the selected student ID
            int selectedStudentID = (int)comboBox1.SelectedItem;

            // Confirm deletion with the user
            DialogResult result = MessageBox.Show("Are you sure you want to delete this student?", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                // Perform the deletion
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the record can be deleted without violating foreign key constraints
                    string checkForeignKeyQuery = "SELECT COUNT(*) FROM Project_has_Student WHERE StudentID = @StudentID";
                    using (MySqlCommand checkForeignKeyCommand = new MySqlCommand(checkForeignKeyQuery, connection))
                    {
                        checkForeignKeyCommand.Parameters.AddWithValue("@StudentID", selectedStudentID);

                        int recordsCount = Convert.ToInt32(checkForeignKeyCommand.ExecuteScalar());

                        if (recordsCount > 0)
                        {
                            MessageBox.Show("Cannot delete this student because it is referenced in the Project_has_Student table.");
                            return;
                        }
                    }

                    // Execute the DELETE query
                    string deleteQuery = "DELETE FROM Student WHERE StudentID = @StudentID";
                    using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@StudentID", selectedStudentID);

                        try
                        {
                            deleteCommand.ExecuteNonQuery();
                            MessageBox.Show("Student deleted successfully!");

                            // Reload data and update the DataGridView
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error deleting student: " + ex.Message);
                        }
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Check if an item is selected in ComboBox2
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Please select a student ID to update.");
                return;
            }


            // Get updated values from textboxes or other controls
            int selectedStudentID = Convert.ToInt32(comboBox2.SelectedItem);
            string updatedStudentName = textBox6.Text;
            string updatedEmail = textBox7.Text;
            string updatedRole = textBox8.Text;
            string updatedSemester = textBox9.Text;

            // Perform the update
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Execute the UPDATE query
                string updateQuery = "UPDATE Student SET StudentName = @StudentName, Email = @Email, Role = @Role, Semester = @Semester WHERE StudentID = @StudentID";
                using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@StudentID", selectedStudentID);
                    updateCommand.Parameters.AddWithValue("@StudentName", updatedStudentName);
                    updateCommand.Parameters.AddWithValue("@Email", updatedEmail);
                    updateCommand.Parameters.AddWithValue("@Role", updatedRole);
                    updateCommand.Parameters.AddWithValue("@Semester", updatedSemester);

                    try
                    {
                        updateCommand.ExecuteNonQuery();
                        MessageBox.Show("Student updated successfully!");

                        // Reload data and update the DataGridView
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error updating student: " + ex.Message);
                    }
                }
            }
        }

        private void UpdateAverageSemesterLabel()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT AVG(Semester) FROM Student";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        double averageSemester = Convert.ToDouble(result);
                        labelAverageSemester.Text = $"Average Semester: {averageSemester:F2}";
                    }
                    else
                    {
                        labelAverageSemester.Text = "No data available";
                    }
                }
            }
        }

        private void UpdateStudentCountLabel()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Student";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    int studentCount = Convert.ToInt32(command.ExecuteScalar());
                    labelStudentCount.Text = $"Student Count: {studentCount}";
                }
            }
        }

    }
}
