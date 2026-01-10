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
using System.Windows.Media.Animation;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;

namespace projectover
{
    /// <summary>
    /// Interaction logic for StartView2.xaml
    /// </summary>
    /// 
    public partial class LoginPage : UserControl
    {
        private const double FloatY = -18; // ตำแหน่ง Y ที่ Label จะลอยไป
        private const double NormalY = 0;  // ตำแหน่ง Y ปกติ (ตรงกลาง)
        private const double AnimationDuration = 0.2; // ความเร็ว Animation (วินาที)


        private MySqlConnection databaseConnection()
        {
            string connectString = "Server=localhost;Port=3306;Database=student;Uid=root;Pwd=;";
            MySqlConnection conn = new MySqlConnection(connectString);
            return conn;
        }

        bool running = false;

        public LoginPage()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, LoginCommandExecuted));
        }

        private void LoginCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            btnToggleRun_Click(sender, e);
        }
        public class UserInfo
        {
            public string StudentId { get; set; }
            public string Role { get; set; }
        }

        private UserInfo ValidateLoginAndRole(string username, string password)
        {
            using (MySqlConnection conn = databaseConnection())
            {
                string query = "SELECT id, role FROM student WHERE BINARY Username=@user AND BINARY Password=@pass LIMIT 1";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password); // แนะนำว่าในระบบจริงควร Hash Password

                    try
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new UserInfo
                                {
                                    StudentId = reader["id"].ToString(),
                                    Role = reader["role"].ToString()
                                };
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
            return null;
        }

        private bool IsUserBanned(string username)
        {
            using (MySqlConnection conn = databaseConnection())
            {
                string query = @"
            SELECT is_banned, ban_until, reason
            FROM student 
            WHERE username = @username 
            LIMIT 1;
        ";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bool isBanned = Convert.ToBoolean(reader["is_banned"]);
                            DateTime? banUntil = reader["ban_until"] == DBNull.Value ? null : (DateTime?)reader["ban_until"];
                            string reason = reader["reason"] == DBNull.Value ? "ไม่ระบุเหตุผล" : reader["reason"].ToString();

                            // ✅ ถ้ามีวันหมดอายุและเลยเวลาแล้ว → ปลดแบน
                            if (isBanned && banUntil.HasValue && DateTime.Now > banUntil.Value)
                            {
                                reader.Close(); // ต้องปิด reader ก่อน update
                                UnbanUser(username);
                                return false; // ปลดแบนแล้ว อนุญาตให้เข้า
                            }

                            // ❌ ถ้ายังโดนแบนอยู่
                            if (isBanned)
                            {
                                string untilText = banUntil.HasValue
                                    ? banUntil.Value.ToString("dd/MM/yyyy HH:mm")
                                    : "ไม่มีกำหนด";

                                new BanMessageBox($"บัญชีนี้ถูกแบนจนถึง {untilText}\nเหตุผล: {reason}").ShowDialog();

                                return true;
                            }
                        }
                    }
                }
            }

            return false; // ไม่โดนแบน
        }
        private void UnbanUser(string username)
        {
            using (MySqlConnection conn = databaseConnection())
            {
                string update = "UPDATE student SET is_banned = 0, ban_until = NULL WHERE username = @username;";
                using (MySqlCommand cmd = new MySqlCommand(update, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        private void btnToggleRun_Click(object sender, RoutedEventArgs e)
        {
            // 1. ดึงค่าจาก Control
            string enteredUsername = tbUsername.Text;
            string enteredPassword = tbPassword.Password;

            // 🔹 กรณีพิเศษ: ถ้าเป็น Admin
            if (enteredUsername == "Admin" && enteredPassword == "1234")
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    // กำหนดค่า Admin (ถ้าต้องการ)
                    mainWindow.CurrentUsername = "Admin";
                    mainWindow.CurrentStudentId = "ADMIN"; // ใส่ค่าเทียมได้

                    // ไปหน้า AdminPageMain
                    mainWindow.MainFrame.Content = new AdminPageMain();
                }

                return; // ออกจาก method ไม่ต้องตรวจต่อ
            }
            // ✅✅ เพิ่มส่วนนี้เพื่อเช็คว่า user ถูกแบนไหม
            if (IsUserBanned(enteredUsername))
            {
                // ถ้าถูกแบน ให้ return เลย ไม่ต้องล็อกอินต่อ
                return;
            }

            // 2. ตรวจสอบกับฐานข้อมูลตามปกติ
            var userInfo = ValidateLoginAndRole(enteredUsername, enteredPassword);

            // 3. ตรวจสอบความถูกต้อง
            if (userInfo != null)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.CurrentStudentId = userInfo.StudentId;
                    mainWindow.CurrentUsername = enteredUsername;

                    if (!string.IsNullOrEmpty(userInfo.Role))
                    {
                        // ✅ role มีค่าแล้ว ไป MainMenu
                        mainWindow.MainFrame.Content = new Mainmenu();
                    }
                    else
                    {
                        // role ยังว่าง ไป ChooseStuorTea
                        mainWindow.MainFrame.Content = new ChooseStuorTea();
                    }
                }
            }
            else
            {
                // ❌ สถานะ: ล็อกอินไม่สำเร็จ
                if (string.IsNullOrWhiteSpace(enteredUsername) || string.IsNullOrWhiteSpace(enteredPassword))
                {
                    MessageBox.Show("กรุณากรอก Username และ Password ให้ครบถ้วน", "ข้อมูลไม่สมบูรณ์", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("Username หรือ Password ไม่ถูกต้อง", "ล็อกอินไม่สำเร็จ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // อัปเดตสถานะ (ถ้าต้องการให้ปุ่ม Run สลับสถานะ)
            running = !running;
        }
        private void TbUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ตรวจสอบว่ามีข้อความอยู่ใน TextBox หรือไม่
            bool hasText = !string.IsNullOrWhiteSpace(tbUsername.Text);

            if (hasText)
            {
                // ถ้ามีข้อความ: ให้ Label ลอยขึ้นทันที
                AnimateLabel(UserTranslate, FloatingLabelUser, FloatY);
            }
            else if (!tbUsername.IsFocused)
            {
                // ถ้าไม่มีข้อความ และไม่ได้อยู่ในสถานะ Focus: ให้ Label ลงมา
                AnimateLabel(UserTranslate, FloatingLabelUser, NormalY);
            }
        }

        // =================================
        // 2. จัดการเมื่อได้รับ/เสีย Focus (สำรอง)
        // =================================

        private void TbUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            // เมื่อคลิกเข้าไป: ให้ Label ลอยขึ้น
            AnimateLabel(UserTranslate, FloatingLabelUser, FloatY);
        }

        private void TbUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            // เมื่อคลิกออก: ถ้าไม่มีข้อความ (ว่างเปล่า) ให้ Label ลงมา
            if (string.IsNullOrWhiteSpace(tbUsername.Text))
            {
                AnimateLabel(UserTranslate, FloatingLabelUser, NormalY);
            }
        }
        private void PbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // 1. Cast sender เป็น PasswordBox
            PasswordBox passwordBox = sender as PasswordBox;

            // 2. ตรวจสอบว่ามีข้อความอยู่ใน PasswordBox หรือไม่ โดยใช้ .Password
            bool hasText = !string.IsNullOrWhiteSpace(passwordBox.Password); // ✅ แก้ไขตรงนี้

            if (hasText)
            {
                AnimateLabel(PasswordTranslate, FloatingLabelPassword, FloatY);
            }
            // 3. ตรวจสอบสถานะ Focus ของ PasswordBox
            else if (!passwordBox.IsFocused) // ✅ แก้ไขตรงนี้
            {
                AnimateLabel(PasswordTranslate, FloatingLabelPassword, NormalY);
            }
        }

        // =================================
        // 2. จัดการเมื่อได้รับ/เสีย Focus (สำรอง)
        // =================================

        private void PbPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            // เมื่อคลิกเข้าไป: ให้ Label ลอยขึ้น
            AnimateLabel(PasswordTranslate, FloatingLabelPassword, FloatY);
        }

        private void PbPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            // เมื่อคลิกออก: ถ้าไม่มีข้อความ (ว่างเปล่า) ให้ Label ลงมา
            if (string.IsNullOrWhiteSpace(tbPassword.Password))
            {
                AnimateLabel(PasswordTranslate, FloatingLabelPassword, NormalY);
            }
        }

        // =================================
        // 3. Method สำหรับสร้าง Animation
        // =================================


        private static void AnimateLabel(TranslateTransform transform, TextBlock label, double targetY)
        {
            // ตั้งค่าเป้าหมาย Animation
            DoubleAnimation animation = new DoubleAnimation(targetY, TimeSpan.FromSeconds(AnimationDuration))
            {
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
            };

            // เริ่ม Animation
            transform.BeginAnimation(TranslateTransform.YProperty, animation);

            // เปลี่ยนขนาด/สีของ Label เพื่อให้ดูเหมือนลอยขึ้นจริง (Optional)
            if (targetY == FloatY)
            {
                label.FontSize = 12; // ทำให้ตัวอักษรเล็กลงเมื่อลอยขึ้น
                label.Foreground = Brushes.DeepSkyBlue; // เปลี่ยนสีเมื่อ Active
            }
            else
            {
                label.FontSize = 16;
                label.Foreground = Brushes.Gray; // กลับไปเป็นสีเทาเมื่อ Inactive
            }
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            // ดึง Storyboard
            Storyboard sb = (Storyboard)this.Resources["SlideToRegister"];
            Panel.SetZIndex(LoginForm, 0);
            Panel.SetZIndex(LeftImage, 1);

            foreach (var timeline in sb.Children)
            {
                if (timeline is DoubleAnimation da)
                {
                    // เปลี่ยน easing function ให้ลื่นขึ้น
                    da.EasingFunction = new QuinticEase
                    {
                        EasingMode = EasingMode.EaseInOut
                    };
                }
            }

            // เมื่อ Animation จบ ให้โหลด RegisterPage
            sb.Completed += (s, ev) =>
            {
                var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Content = new RegisterPage();
                }
            };

            // เริ่ม Animation
            sb.Begin();
        }
        private void ForgotBtn_Click(object sender, RoutedEventArgs e)
        {
            // ดึง Storyboard
            Storyboard sb = (Storyboard)this.Resources["SlideToRegister"];
            Panel.SetZIndex(LoginForm, 0);
            Panel.SetZIndex(LeftImage, 1);

            foreach (var timeline in sb.Children)
            {
                if (timeline is DoubleAnimation da)
                {
                    // เปลี่ยน easing function ให้ลื่นขึ้น
                    da.EasingFunction = new QuinticEase
                    {
                        EasingMode = EasingMode.EaseInOut
                    };
                }
            }

            // เมื่อ Animation จบ ให้โหลด ForgetPassPage
            sb.Completed += (s, ev) =>
            {
                var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Content = new ForgetPassPage();
                }
            };

            // เริ่ม Animation
            sb.Begin();
        }
    }
}
