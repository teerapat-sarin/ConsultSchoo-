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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.IO;
using System.Windows.Controls.Primitives;

namespace projectover
{
    /// <summary>
    /// Interaction logic for MenuPanel.xaml
    /// </summary>
    public partial class MenuPanel : UserControl
    {
        string connectionString = "Server=127.0.0.1;Port=3306;Uid=root;Pwd=;Database=student;";
        string studentId;
        public string CurrentStudentId { get; set; }
        public MenuPanel()
        {
            InitializeComponent();
            LoadUserInfo();
            LoadProfileImage();
        }
        private bool isMenuOpen = false;

        private void LoadUserInfo()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            if (mainWindow == null || string.IsNullOrEmpty(mainWindow.CurrentStudentId))
                return;

            string studentId = mainWindow.CurrentStudentId;
            string connStr = "Server=127.0.0.1;Port=3306;Uid=root;Pwd=;Database=student;";
            string query = "SELECT name, Role FROM student WHERE id = @Id";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", studentId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // ✅ ใช้ name จาก database
                                UsernameLabel.Content = reader["name"].ToString();

                                StatusLabel.Content = reader["Role"].ToString();

                                // ✅ ถ้าต้องการเก็บส่งต่อหน้าอื่น
                                App.Current.Properties["StudentName"] = reader["name"].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading user info: " + ex.Message);
                }
            }
        }

        private void LoadProfileImage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            if (mainWindow == null || string.IsNullOrEmpty(mainWindow.CurrentStudentId))
                return;

            studentId = mainWindow.CurrentStudentId;

            string query = "SELECT image_path FROM student WHERE id = @Id";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", studentId);
                        var result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            string imagePath = result.ToString();
                            if (File.Exists(imagePath))
                            {
                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.EndInit();
                                bitmap.Freeze();

                                // ✅ สร้าง ImageBrush พร้อมเลื่อนภาพลงเล็กน้อย
                                ImageBrush brush = new ImageBrush(bitmap)
                                {
                                    Stretch = Stretch.UniformToFill,
                                    AlignmentX = AlignmentX.Center,
                                    AlignmentY = AlignmentY.Top,
                                    ViewboxUnits = BrushMappingMode.RelativeToBoundingBox,
                                    ViewportUnits = BrushMappingMode.RelativeToBoundingBox,
                                    Viewbox = new Rect(0, 0.06, 1, 0.94) // ← ปรับตรงนี้เพื่อเลื่อนลง (0.06 = 6%)
                                };

                                ProfileEllipse.Fill = brush;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading profile image: " + ex.Message);
                }
            }
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Content = new Mainmenu();
            }

        }
        private void Findurshelf_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                string connectionString = "server=localhost;user id=root;password=;database=student;charset=utf8;";
                string mbti = null;

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "SELECT MBTI FROM student WHERE id = @id";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", mainWindow.CurrentStudentId);
                            var result = cmd.ExecuteScalar();
                            if (result != null)
                            {
                                mbti = result.ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("เกิดข้อผิดพลาดในการเชื่อมต่อฐานข้อมูล: " + ex.Message);
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(mbti))
                {
                    // ✅ ถ้ามีค่า MBTI ให้ไปหน้า DetailMBTi
                    var detailPage = new DetailMBTi(mainWindow.CurrentStudentId);
                    mainWindow.MainFrame.Content = detailPage;
                }
                else
                {
                    // ✅ ถ้ายังไม่มี MBTI ให้ไปหน้า FindYourSelf
                    var findYourSelfPage = new FindYourSelf(mainWindow.CurrentStudentId);
                    mainWindow.MainFrame.Content = findYourSelfPage;
                }
            }

        }
        private void Consulter_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Content = new Consulter();
            }
        }
        private void Chat_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Content = new ChatPage();
            }
        }
        private void Report_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Content = new ReportPage();
            }
        }
        
        private void About_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Content = new Aboutpage();
            }

        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Properties["Username"] = null;

            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Content = new LoginPage();
            }
        }

        private void ChangeImg_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow == null) return;

            string studentId = mainWindow.CurrentStudentId;
            string connectionString = "Server=127.0.0.1;Port=3306;Uid=root;Pwd=;Database=student;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "SELECT role FROM student WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", studentId);
                        var roleObj = cmd.ExecuteScalar();

                        if (roleObj == null)
                        {
                            MessageBox.Show("ไม่พบข้อมูลผู้ใช้", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        string role = roleObj.ToString();

                        // ✅ เช็ค role แล้วเปิดหน้านั้น
                        if (role == "Student")
                        {
                            mainWindow.MainFrame.Content = new ChangeImg();
                        }
                        else if (role == "Consultant")
                        {
                            mainWindow.MainFrame.Content = new ChangeImgConsultant();
                        }
                        else
                        {
                            MessageBox.Show("ไม่รู้จักประเภทผู้ใช้: " + role);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาดในการโหลดข้อมูล: " + ex.Message);
            }
        }
    }
}
