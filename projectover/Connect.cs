using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySqlConnector; // หรือ using MySql.Data.MySqlClient; ถ้าใช้ MySql.Data

namespace projectover
{
    internal class Connect
    {
        // 1. กำหนด Connection String
        //    * เปลี่ยน 'your_database_name' เป็นชื่อฐานข้อมูลจริงของคุณใน XAMPP
        private readonly string connectionString =
            "Server=127.0.0.1;Port=3306;Uid=root;Pwd=;Database=student;";
        public Connect()
        {
            // ไม่ต้องทำอะไรในนี้ก็ได้ หรือจะตั้งค่าเริ่มต้นก็ได้
        }
        public string ConnectionString => connectionString;

        // 2. Property สำหรับส่งคืน Object การเชื่อมต่อ
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
        // ************************************************************
        // 4. (ตัวอย่าง) Method สำหรับดึงข้อมูล หรือรันคำสั่งต่างๆ
        // ************************************************************

        public DataTable ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader); // โหลดข้อมูลจาก Reader เข้าสู่ DataTable
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("ข้อผิดพลาดในการ Query: " + ex.Message);
                }
            }
            return dt;
        }
    }
}