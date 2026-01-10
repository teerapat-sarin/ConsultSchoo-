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
using System.Net.Mail;
using System.Net;


namespace projectover
{
    /// <summary>
    /// Interaction logic for ForgetPassPage.xaml
    /// </summary>
    public partial class ForgetPassPage : UserControl
    {


        private const double FloatY = -18; // ตำแหน่ง Y ที่ Label จะลอยไป
        private const double NormalY = 0;  // ตำแหน่ง Y ปกติ (ตรงกลาง)
        private const double AnimationDuration = 0.2; // ความเร็ว Animation (วินาที)
        public ForgetPassPage()
        {
            InitializeComponent();
        }
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
        private void TbOtp_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool hasText = !string.IsNullOrWhiteSpace(tbOtp.Text);

            if (hasText)
            {
                AnimateLabel(OtpTranslate, FloatingLabelOtp, FloatY);
            }
            else if (!tbOtp.IsFocused)
            {
                AnimateLabel(OtpTranslate, FloatingLabelOtp, NormalY);
            }
        }

        private void TbOtp_GotFocus(object sender, RoutedEventArgs e)
        {
            AnimateLabel(OtpTranslate, FloatingLabelOtp, FloatY);
        }

        private void TbOtp_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbOtp.Text))
            {
                AnimateLabel(OtpTranslate, FloatingLabelOtp, NormalY);
            }
        }
        private void btnToggleConfirm_Click(object sender, RoutedEventArgs e)
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

            if (password.Length < 8)
            {
                MessageBox.Show("รหัสผ่านต้องมีอย่างน้อย 8 ตัวอักษร");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("รหัสผ่านและยืนยันรหัสผ่านไม่ตรงกัน");
                return;
            }

            try
            {
                string connectionString = "server=localhost;user=root;password=;database=student;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // ✅ ตรวจสอบว่า email นี้มีอยู่ไหม
                    string checkQuery = "SELECT Username FROM student WHERE Email = @Email";
                    string oldUsername = null;

                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        var result = checkCmd.ExecuteScalar();
                        if (result == null)
                        {
                            MessageBox.Show("ไม่พบอีเมลนี้ในระบบ กรุณาตรวจสอบอีกครั้ง");
                            return;
                        }
                        oldUsername = result.ToString();
                    }

                    // ✅ อัปเดต Username และ Password ใน student
                    string updateStudent = "UPDATE student SET Username = @NewUsername, Password = @Password WHERE Email = @Email";
                    using (MySqlCommand cmd = new MySqlCommand(updateStudent, conn))
                    {
                        cmd.Parameters.AddWithValue("@NewUsername", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.ExecuteNonQuery();
                    }

                    // ✅ อัปเดตใน messages (SenderId และ ReceiverId)
                    string updateMessagesSender = "UPDATE messages SET SenderId = @NewUsername WHERE SenderId = @OldUsername";
                    using (MySqlCommand cmd = new MySqlCommand(updateMessagesSender, conn))
                    {
                        cmd.Parameters.AddWithValue("@NewUsername", username);
                        cmd.Parameters.AddWithValue("@OldUsername", oldUsername);
                        cmd.ExecuteNonQuery();
                    }

                    string updateMessagesReceiver = "UPDATE messages SET ReceiverId = @NewUsername WHERE ReceiverId = @OldUsername";
                    using (MySqlCommand cmd = new MySqlCommand(updateMessagesReceiver, conn))
                    {
                        cmd.Parameters.AddWithValue("@NewUsername", username);
                        cmd.Parameters.AddWithValue("@OldUsername", oldUsername);
                        cmd.ExecuteNonQuery();
                    }

                    // ✅ อัปเดตใน consulter
                    string updateConsulter = "UPDATE consulter SET Username = @NewUsername WHERE Username = @OldUsername";
                    using (MySqlCommand cmd = new MySqlCommand(updateConsulter, conn))
                    {
                        cmd.Parameters.AddWithValue("@NewUsername", username);
                        cmd.Parameters.AddWithValue("@OldUsername", oldUsername);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("อัปเดตข้อมูลทั้งหมดสำเร็จ!");

                    // ✅ กลับหน้า Login พร้อม Animation
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
        private string generatedOtp; // เก็บรหัส OTP ชั่วคราว


        private void SendOtp_Click(object sender, RoutedEventArgs e)
        {
            string email = tbEmail.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("กรุณากรอกอีเมลก่อนส่ง OTP");
                return;
            }

            try
            {
                // ✅ ใช้ TLS1.2
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // ✅ สร้างรหัส OTP
                Random random = new Random();
                generatedOtp = random.Next(100000, 999999).ToString();

                // ✅ สร้างข้อความอีเมล
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("kokomjmj456789@gmail.com");
                mail.To.Add(email);
                mail.Subject = "รหัส OTP สำหรับรีเซ็ตรหัสผ่าน";
                mail.Body = $"รหัส OTP ของคุณคือ: {generatedOtp}";
                mail.IsBodyHtml = false;

                // ✅ ตั้งค่า SMTP
                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587, // ใช้ TLS port
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("kokomjmj456789@gmail.com", "risfbheifkqrtyog")
                };

                smtpServer.Send(mail);
                MessageBox.Show("ส่ง OTP สำเร็จ! กรุณาตรวจสอบอีเมลของคุณ");
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาดในการส่งอีเมล:\n" + ex.ToString());
            }
        }

        private void VerifyOtp_Click(object sender, RoutedEventArgs e)
        {
            string enteredOtp = tbOtp.Text.Trim();

            if (string.IsNullOrEmpty(enteredOtp))
            {
                MessageBox.Show("กรุณากรอก OTP ก่อน");
                return;
            }

            if (enteredOtp == generatedOtp)
            {
                MessageBox.Show("ยืนยัน OTP สำเร็จ! คุณสามารถเปลี่ยนรหัสผ่านได้แล้ว");
                // ✅ อาจเพิ่ม logic เพื่อเปิดส่วนเปลี่ยนรหัสผ่าน เช่น enable ช่อง Password
                tbPassword.IsEnabled = true;
                PbConfirmPassword.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("รหัส OTP ไม่ถูกต้อง กรุณาลองใหม่");
            }
        }
        // ======= จบส่วนที่เพิ่ม =======
    }

}