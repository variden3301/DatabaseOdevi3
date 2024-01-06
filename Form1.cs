using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;




namespace DatabaseOdevi3
{
    public partial class Form1 : Form
    {
        private string connectionString = "Server=localhost;Uid=root;Pwd=123456789;Database=test;";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {


            string email = textBox1.Text;
            string password = textBox2.Text;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Teacher WHERE Email = @Email AND Password = @Password";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Login successful
                            MessageBox.Show("Login successful!");

                            // Close Form1
                            this.Hide();

                            // Open Form2
                            Form2 form2 = new Form2();

                            form2.ShowDialog(); // Use ShowDialog to make Form2 modal

                               
                            
                        }
                        else
                        {
                            // Invalid credentials
                            MessageBox.Show("Invalid email or password. Please try again.");
                        }
                    }
                }
            }
        }
    }
}