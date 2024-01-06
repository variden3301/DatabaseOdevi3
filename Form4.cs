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
            }
        }
    }
}
