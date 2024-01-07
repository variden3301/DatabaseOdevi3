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
    public partial class Form5 : Form
    {
        private string connectionString = "Server=localhost;Uid=root;Pwd=123456789;Database=test;";
        public Form5()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM teacher";
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
                {
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);

                    // Bind the DataTable to the DataGridView
                    dataGridView1.DataSource = dataTable;
                }

                // Load Teacher IDs into the ComboBox
                string teacherIdsQuery = "SELECT TeacherID FROM teacher ORDER BY TeacherID";
                using (MySqlCommand command = new MySqlCommand(teacherIdsQuery, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        comboBox1.Items.Clear();
                        comboBox2.Items.Clear();

                        while (reader.Read())
                        {
                            int teacherId = Convert.ToInt32(reader["TeacherID"]);
                            comboBox1.Items.Add(teacherId);
                            comboBox2.Items.Add(teacherId);
                        }
                    }
                }
            }
            UpdateAveragePasswordLengthLabel();
            UpdateTeacherCountLabel();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int intValue1;
            if (!int.TryParse(textBox1.Text, out intValue1))
            {
                MessageBox.Show("Please enter a valid integer for Teacher ID.");
                return;
            }

            string stringValue1 = textBox2.Text;
            string stringValue2 = textBox3.Text;

            int intValue2;
            if (!int.TryParse(textBox5.Text, out intValue2))
            {
                MessageBox.Show("Please enter a valid integer for password.");
                return;
            }

            string stringValue3 = textBox4.Text;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO teacher (TeacherID, TeacherName, Email, Department, password) VALUES (@Value1, @Value2, @Value3, @Value4, @Value5)";
                using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@Value1", intValue1);
                    insertCommand.Parameters.AddWithValue("@Value2", stringValue1);
                    insertCommand.Parameters.AddWithValue("@Value3", stringValue2);
                    insertCommand.Parameters.AddWithValue("@Value4", stringValue3);
                    insertCommand.Parameters.AddWithValue("@Value5", intValue2 );

                    try
                    {
                        insertCommand.ExecuteNonQuery();
                        MessageBox.Show("Record inserted successfully!");

                        // Reload data and update the DataGridView
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
            int teacherId;
            if (!int.TryParse(comboBox1.Text, out teacherId))
            {
                MessageBox.Show("Please select a valid Teacher ID.");
                return;
            }

            // Get values from TextBoxes
            string teacherName = textBox9.Text;
            string email = textBox8.Text;
            string department = textBox7.Text;
            int password;
            if (!int.TryParse(textBox6.Text, out password))
            {
                MessageBox.Show("Please enter a valid integer for password.");
                return;
            }

            // Update the teacher in the Teacher table
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE teacher SET TeacherName = @TeacherName, Email = @Email, Department = @Department, Password = @Password WHERE TeacherID = @TeacherID";
                using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@TeacherID", teacherId);
                    updateCommand.Parameters.AddWithValue("@TeacherName", teacherName);
                    updateCommand.Parameters.AddWithValue("@Email", email);
                    updateCommand.Parameters.AddWithValue("@Department", department);
                    updateCommand.Parameters.AddWithValue("@Password", password);

                    try
                    {
                        updateCommand.ExecuteNonQuery();
                        MessageBox.Show("Teacher updated successfully!");

                        // Reload data and update the DataGridView
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error updating teacher: " + ex.Message);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Please select a Teacher ID to delete.");
                return;
            }

            int teacherId = Convert.ToInt32(comboBox2.SelectedItem);

            // Confirm deletion with the user
            DialogResult result = MessageBox.Show("Are you sure you want to delete this teacher?", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                // Perform the deletion
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Execute the DELETE query
                    string deleteQuery = "DELETE FROM teacher WHERE TeacherID = @TeacherID";
                    using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@TeacherID", teacherId);

                        try
                        {
                            deleteCommand.ExecuteNonQuery();
                            MessageBox.Show("Teacher deleted successfully!");

                            // Reload data and update the DataGridView
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error deleting teacher: " + ex.Message);
                        }
                    }
                }
            }
        }

        private void UpdateAveragePasswordLengthLabel()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT AVG(LENGTH(password)) FROM teacher";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        double averagePasswordLength = Convert.ToDouble(result);
                        labelAveragePasswordLength.Text = $"Average Password Length: {averagePasswordLength:F2}";
                    }
                    else
                    {
                        labelAveragePasswordLength.Text = "No data available";
                    }
                }
            }
        }

        private void UpdateTeacherCountLabel()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM teacher";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        int teacherCount = Convert.ToInt32(result);
                        labelTeacherCount.Text = $"Teacher Count: {teacherCount}";
                    }
                    else
                    {
                        labelTeacherCount.Text = "No data available";
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6();

            form6.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();

            form7.ShowDialog();
        }
    }


}
