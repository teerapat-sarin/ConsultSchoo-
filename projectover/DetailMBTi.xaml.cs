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
    /// Interaction logic for DetailMBTi.xaml
    /// </summary>
    public partial class DetailMBTi : UserControl
    {
        public string CurrentStudentId { get; set; }

        // Properties สำหรับ Binding
        public string MBTI { get; set; }
        public string MBTIDetail { get; set; }

        public DetailMBTi(string studentId)
        {
            InitializeComponent();
            CurrentStudentId = studentId;

            // ดึงข้อมูล MBTI และ Detail
            LoadMBTIInfo();
            // ตั้ง DataContext ให้ Binding ใช้งาน
            this.DataContext = this;
        }
        private void LoadMBTIInfo()
        {
            string connectionString = "server=localhost;user id=root;password=;database=student;charset=utf8;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // 1️⃣ ดึง MBTI ของนักเรียน
                string studentQuery = "SELECT MBTI FROM student WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(studentQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@id", CurrentStudentId);
                    var result = cmd.ExecuteScalar();
                    if (result != null && !string.IsNullOrEmpty(result.ToString()))
                    {
                        MBTI = result.ToString();
                    }
                    else
                    {
                        MBTI = "ยังไม่มี MBTI";
                    }
                }

                // 2️⃣ ดึง Detail ของ MBTI จาก table 16personalities
                string detailQuery = "SELECT Detail FROM `16personalities` WHERE MBTI = @mbti";
                using (MySqlCommand cmd = new MySqlCommand(detailQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@mbti", MBTI);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        MBTIDetail = result.ToString();
                    }
                    else
                    {
                        MBTIDetail = "ไม่พบรายละเอียด MBTI";
                    }
                }
            }
        }

        // ✅ ตัวอย่าง Event สำหรับปุ่ม Confirm
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Content = new Mainmenu();
            }
        }

        private void Retest_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow == null) return;

            string connectionString = "server=localhost;user id=root;password=;database=student;charset=utf8;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE student SET MBTI = NULL WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", mainWindow.CurrentStudentId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            
                        }
                        else
                        {
                            MessageBox.Show("ไม่พบรหัสนักเรียน");
                        }
                    }
                }

                // ไปหน้า FindYourSelf พร้อมส่ง CurrentStudentId
                var findYourSelfPage = new FindYourSelf(mainWindow.CurrentStudentId);
                mainWindow.MainFrame.Content = findYourSelfPage;
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
            }
        }
    }
    }
