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
    /// Interaction logic for CardForQuestionAdmin.xaml
    /// </summary>
    public partial class CardForQuestionAdmin : UserControl
    {
        public int QuestionId { get; set; }  // id ของ question

        // DependencyProperty สำหรับ binding TextBlock หรือ TextBox ใน XAML
        public static readonly DependencyProperty QuestionProperty =
            DependencyProperty.Register("Question", typeof(string), typeof(CardForQuestionAdmin));

        public string Question
        {
            get => (string)GetValue(QuestionProperty);
            set => SetValue(QuestionProperty, value);
        }

        public CardForQuestionAdmin()
        {
            InitializeComponent();
        }
        // Question


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionId == 0) return;

            var result = MessageBox.Show("คุณต้องการลบคำถามนี้จริงหรือไม่?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            string connectionString = "server=localhost;user id=root;password=;database=student;charset=utf8;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "DELETE FROM question WHERE id=@id";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", QuestionId);
                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("ลบสำเร็จ!");
                            // ลบ UI card ออกจาก WrapPanel
                            if (this.Parent is Panel parentPanel)
                                parentPanel.Children.Remove(this);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
            }
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            var solvePage = new SolveQuestion(QuestionId);

            // สมมติคุณมี mainWindow ที่มี MainFrame
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Content = solvePage;

        }
    }
}
