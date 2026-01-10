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

namespace projectover
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : UserControl
    {
        private const double FloatY = -18; // ตำแหน่ง Y ที่ Label จะลอยไป
        private const double NormalY = 0;  // ตำแหน่ง Y ปกติ (ตรงกลาง)
        private const double AnimationDuration = 0.2; // ความเร็ว Animation (วินาที)
        public RegisterPage()
        {
            InitializeComponent();
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
        private void PbConFrimPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // 1. Cast sender เป็น PasswordBox
            PasswordBox passwordBox = sender as PasswordBox;

            // 2. ตรวจสอบว่ามีข้อความอยู่ใน PasswordBox หรือไม่ โดยใช้ .Password
            bool hasText = !string.IsNullOrWhiteSpace(passwordBox.Password); // ✅ แก้ไขตรงนี้

            if (hasText)
            {
                AnimateLabel(ConfrimPasswordTranslate, FloatingLabelConfirmPassword, FloatY);
            }
            // 3. ตรวจสอบสถานะ Focus ของ PasswordBox
            else if (!passwordBox.IsFocused) // ✅ แก้ไขตรงนี้
            {
                AnimateLabel(ConfrimPasswordTranslate, FloatingLabelConfirmPassword, NormalY);
            }
        }

        // =================================
        // 2. จัดการเมื่อได้รับ/เสีย Focus (สำรอง)
        // =================================

        private void PbConfirmPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            // เมื่อคลิกเข้าไป: ให้ Label ลอยขึ้น
            AnimateLabel(ConfrimPasswordTranslate, FloatingLabelConfirmPassword, FloatY);
        }

        private void PbConfirmPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            // เมื่อคลิกออก: ถ้าไม่มีข้อความ (ว่างเปล่า) ให้ Label ลงมา
            if (string.IsNullOrWhiteSpace(PbConfirmPassword.Password))
            {
                AnimateLabel(ConfrimPasswordTranslate, FloatingLabelConfirmPassword, NormalY);
            }
        }

        // =================================
        // 3. Method สำหรับสร้าง Animation
        // =================================

        private void TbEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ตรวจสอบว่ามีข้อความอยู่ใน TextBox หรือไม่
            bool hasText = !string.IsNullOrWhiteSpace(tbEmail.Text);

            if (hasText)
            {
                // ถ้ามีข้อความ: ให้ Label ลอยขึ้นทันที
                AnimateLabel(EmailTranslate, FloatingLabelEmail, FloatY);
            }
            else if (!tbUsername.IsFocused)
            {
                // ถ้าไม่มีข้อความ และไม่ได้อยู่ในสถานะ Focus: ให้ Label ลงมา
                AnimateLabel(EmailTranslate, FloatingLabelEmail, NormalY);
            }
        }

        // =================================
        // 2. จัดการเมื่อได้รับ/เสีย Focus (สำรอง)
        // =================================

        private void TbEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            // เมื่อคลิกเข้าไป: ให้ Label ลอยขึ้น
            AnimateLabel(EmailTranslate, FloatingLabelEmail, FloatY);
        }

        private void TbEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            // เมื่อคลิกออก: ถ้าไม่มีข้อความ (ว่างเปล่า) ให้ Label ลงมา
            if (string.IsNullOrWhiteSpace(tbEmail.Text))
            {
                AnimateLabel(EmailTranslate, FloatingLabelEmail, NormalY);
            }
        }
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
        private void btnToggleRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = tbUsername.Text.Trim();
            string password = tbPassword.Password.Trim();
            string confirmPassword = PbConfirmPassword.Password.Trim();
            string email = tbEmail.Text.Trim();

            // ✅ ตรวจสอบช่องว่าง
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("กรุณากรอกข้อมูลให้ครบทุกช่อง");
                return;
            }


            if (username.Length < 8)
            {
                MessageBox.Show("รหัสผ่านต้องมี 8 ตัวอักษรขึ้นไป");
                return;
            }

            if (password.Length < 8)
            {
                MessageBox.Show("รหัสผ่านต้องมี 8 ตัวอักษรขึ้นไป");
                return;
            }


            // ✅ ตรวจสอบรหัสผ่านตรงกันไหม
            if (password != confirmPassword)
            {
                MessageBox.Show("รหัสผ่านและยืนยันรหัสผ่านไม่ตรงกัน");
                return;
            }


            // ✅ ตรวจสอบโดเมนอีเมล
            string[] allowedDomains = { "@gmail.com", "@kkumail.com", "@zati.com" };
            bool validDomain = allowedDomains.Any(domain => email.EndsWith(domain, StringComparison.OrdinalIgnoreCase));

            if (!validDomain)
            {
                MessageBox.Show("อีเมลต้องลงท้ายด้วย @gmail.com, @kkumail.com หรือ @zati.com เท่านั้น");
                return;
            }

            try
            {
                // ✅ สร้าง Connection
                string connectionString = "server=localhost;user=root;password=;database=student;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // ✅ ตรวจสอบซ้ำว่ามี username หรือ email นี้แล้วไหม
                    string checkQuery = "SELECT COUNT(*) FROM student WHERE Username = @Username OR Email = @Email";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", username);
                        checkCmd.Parameters.AddWithValue("@Email", email);

                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show("ชื่อผู้ใช้หรืออีเมลนี้มีอยู่แล้วในระบบ");
                            return;
                        }
                    }

                    // ✅ เพิ่มข้อมูลใหม่เข้า Database
                    string insertQuery = "INSERT INTO student (Username, Password, Email) VALUES (@Username, @Password, @Email)";
                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.ExecuteNonQuery();
                    }

                    // ✅ สมัครสำเร็จ แสดงข้อความ
                    MessageBox.Show("สมัครสมาชิกสำเร็จ!");

                    // ✅ เริ่มเล่น Storyboard หลังสมัครสำเร็จ
                    Storyboard sb = (Storyboard)this.Resources["SlideToLogin"];

                    Panel.SetZIndex(RegisterForm, 0);
                    Panel.SetZIndex(RightImage, 1);

                    foreach (var timeline in sb.Children)
                    {
                        if (timeline is DoubleAnimation da)
                        {
                            da.EasingFunction = new QuinticEase
                            {
                                EasingMode = EasingMode.EaseInOut
                            };
                        }
                    }

                    // ✅ เมื่อ Animation จบ → เปลี่ยนไปหน้า LoginPage
                    sb.Completed += (s, ev) =>
                    {
                        var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
                        if (mainWindow != null)
                        {
                            mainWindow.MainFrame.Content = new LoginPage();
                        }
                    };

                    sb.Begin();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
            }
        }
        
        private void backbutton_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (Storyboard)this.Resources["SlideToLogin"];

            Panel.SetZIndex(RegisterForm, 0);
            Panel.SetZIndex(RightImage, 1);

            foreach (var timeline in sb.Children)
            {
                if (timeline is DoubleAnimation da)
                {
                    da.EasingFunction = new QuinticEase
                    {
                        EasingMode = EasingMode.EaseInOut
                    };
                }
            }

            // ✅ เมื่อ Animation จบ → เปลี่ยนไปหน้า LoginPage
            sb.Completed += (s, ev) =>
            {
                var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Content = new LoginPage();
                }
            };

            sb.Begin();
        }
    }
}
