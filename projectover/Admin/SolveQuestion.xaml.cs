using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace projectover
{
    /// <summary>
    /// Interaction logic for SolveQuestion.xaml
    /// </summary>
    public partial class SolveQuestion : UserControl
    {
        private int QuestionId; // เก็บ id ของคำถามที่ต้องการแก้
        public SolveQuestion(int questionId)
        {
            InitializeComponent();
            QuestionId = questionId;

            LoadQuestionFromDatabase();
        }
        private void LoadQuestionFromDatabase()
        {
            string connectionString = "server=localhost;user id=root;password=;database=student;charset=utf8;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT Question, dimension FROM question WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", QuestionId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TBQuestion.Text = reader["Question"].ToString();

                                // ตั้งค่า combobox ให้ตรงกับค่าประเภทใน DB
                                string type = reader["dimension"].ToString();
                                foreach (ComboBoxItem item in CBType.Items)
                                {
                                    if (item.Content.ToString() == type)
                                    {
                                        CBType.SelectedItem = item;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาดในการโหลดคำถาม: " + ex.Message);
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            string newQuestion = TBQuestion.Text;
            string newType = ((ComboBoxItem)CBType.SelectedItem).Content.ToString();

            string connectionString = "server=localhost;user id=root;password=;database=student;charset=utf8;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "UPDATE question SET Question = @question, dimension = @dimension WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@question", newQuestion);
                        cmd.Parameters.AddWithValue("@dimension", newType);
                        cmd.Parameters.AddWithValue("@id", QuestionId);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                            MessageBox.Show("อัปเดตข้อมูลสำเร็จ!");
                        else
                            MessageBox.Show("ไม่พบข้อมูลให้แก้ไข");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาดในการบันทึก: " + ex.Message);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Content = new AddQuestion();
            }
        }
    }
}
