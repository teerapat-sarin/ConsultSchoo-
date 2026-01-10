using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;



namespace projectover
{
    /// <summary>
    /// Interaction logic for sati.xaml
    /// </summary>
    public partial class ChooseStuorTea : Page
    {
        public string CurrentUserId { get; set; }
        public ChooseStuorTea()
        {
            InitializeComponent();
        }

        private void StudentButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Content = new Mainmenu();
            }
            string connectionString = "Server=127.0.0.1;Port=3306;Uid=root;Pwd=;Database=student;";
            string studentId = mainWindow?.CurrentStudentId; // ดึง StudentId จาก MainWindow
            string role = "Student";  // หรือ "Teacher" ตามปุ่มที่กด

            if (string.IsNullOrEmpty(studentId))
            {
                MessageBox.Show("ไม่พบ StudentId ของผู้ใช้งาน");
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // แก้ชื่อคอลัมน์ Student/Teacher ให้ใช้ backticks หรือเปลี่ยนชื่อคอลัมน์เป็น Role
                    string sql = "UPDATE student SET `Role` = @Role WHERE id = @Id";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Role", role);
                        cmd.Parameters.AddWithValue("@Id", studentId);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving data: " + ex.Message);
                }
            }
        }
        private void ConsulterButton_Clicks(object sender, RoutedEventArgs e)
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Content = new ConsulterForm();
            }
            string connectionString = "Server=127.0.0.1;Port=3306;Uid=root;Pwd=;Database=student;";
            string studentId = mainWindow?.CurrentStudentId; // ดึง StudentId จาก MainWindow
            string role = "Consultant";  // หรือ "Teacher" ตามปุ่มที่กด

            if (string.IsNullOrEmpty(studentId))
            {
                MessageBox.Show("ไม่พบ StudentId ของผู้ใช้งาน");
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // แก้ชื่อคอลัมน์ Student/Teacher ให้ใช้ backticks หรือเปลี่ยนชื่อคอลัมน์เป็น Role
                    string sql = "UPDATE student SET `Role` = @Role WHERE id = @Id";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Role", role);
                        cmd.Parameters.AddWithValue("@Id", studentId);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving data: " + ex.Message);
                }
            }
        }
        public const string FontIconFileNameFAB = "fa-brands-400.ttf";

        private void Satiuu_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                // หากหน้าจอเดิม (StartView2) ถูกโหลดด้วย Navigate()
                if (this.NavigationService.CanGoBack)
                {
                    this.NavigationService.GoBack(); // ย้อนกลับไปหน้าก่อนหน้า
                }
                else
                {
                    // หากไม่มีประวัติการนำทาง ให้โหลด StartView2 เข้าไปใหม่
                    this.NavigationService.Navigate(new LoginPage());
                }
            }
        }

    }
}
