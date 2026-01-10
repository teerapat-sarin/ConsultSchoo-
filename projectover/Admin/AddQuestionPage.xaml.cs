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
    /// Interaction logic for AddQuestionPage.xaml
    /// </summary>
    public partial class AddQuestionPage : UserControl
    {
        private string connectionString = "server=localhost;user id=root;password=;database=student;charset=utf8;";
        public AddQuestionPage()
        {
            InitializeComponent();
        }
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            string questionText = TBQuestion.Text.Trim();
            string mbti = (CBType.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(questionText))
            {
                MessageBox.Show("กรุณากรอกคำถาม", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(mbti))
            {
                MessageBox.Show("กรุณาเลือกประเภท MBTI", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "INSERT INTO question (Question, dimension) VALUES (@question, @dimension)";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@question", questionText);
                        cmd.Parameters.AddWithValue("@dimension", mbti);

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("บันทึกสำเร็จ!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            TBQuestion.Clear();
                            CBType.SelectedIndex = 0;
                        }
                        else
                        {
                            MessageBox.Show("เกิดข้อผิดพลาดในการบันทึก", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
