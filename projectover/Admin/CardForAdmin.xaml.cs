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
using System.IO;
using MySql.Data.MySqlClient;

namespace projectover
{
    /// <summary>
    /// Interaction logic for CardForAdmin.xaml
    /// </summary>
    public partial class CardForAdmin : UserControl
    {
        public CardForAdmin()
        {
            InitializeComponent();
        }
        // DisplayName
        public static readonly DependencyProperty DisplayNameProperty =
            DependencyProperty.Register("DisplayName", typeof(string), typeof(CardForAdmin));

        public string DisplayName
        {
            get => (string)GetValue(DisplayNameProperty);
            set => SetValue(DisplayNameProperty, value);
        }

        public static readonly DependencyProperty FullNameProperty =
             DependencyProperty.Register("Username", typeof(string), typeof(CardForAdmin));

        public string Username
        {
            get => (string)GetValue(FullNameProperty);
            set => SetValue(FullNameProperty, value);
        }

        public static readonly DependencyProperty ImagePathProperty =
    DependencyProperty.Register("ImagePath", typeof(string), typeof(CardForAdmin),
        new PropertyMetadata("", OnImagePathChanged));

        public string ImagePath
        {
            get => (string)GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        private static void OnImagePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CardForAdmin;
            string path = e.NewValue as string;
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    control.CardImage.Source = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
                }
                catch { }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            string username = this.Username;

            if (MessageBox.Show($"คุณต้องการลบผู้ใช้ {username} หรือไม่?", "ยืนยัน", MessageBoxButton.YesNo)
                == MessageBoxResult.Yes)
            {
                string connectionString = "server=localhost;user id=root;password=;database=student;charset=utf8;";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // ลบข้อความที่ส่ง/รับโดย user นี้
                    string deleteMessagesQuery = @"DELETE FROM messages 
                                           WHERE SenderId = @username 
                                              OR ReceiverId = @username";

                    using (MySqlCommand cmd = new MySqlCommand(deleteMessagesQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.ExecuteNonQuery();
                    }

                    // ลบข้อมูลจาก consulter โดยใช้ Username
                    string deleteConsulterQuery = "DELETE FROM consulter WHERE Username = @username";
                    using (MySqlCommand cmd = new MySqlCommand(deleteConsulterQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.ExecuteNonQuery();
                    }

                    // ลบ user จาก student
                    string deleteUserQuery = "DELETE FROM student WHERE username = @username";
                    using (MySqlCommand cmd = new MySqlCommand(deleteUserQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }



        private void Ban_Click(object sender, RoutedEventArgs e)
        {
            // เปิด dialog ให้เลือกจำนวนวัน
            var dialog = new BanDialog();
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                int banDays = dialog.BanDays;
                string reason = dialog.ReasonTextBox.Text?.Trim() ?? "";
                DateTime banUntil = DateTime.Now.AddDays(banDays);

                string username = this.Username;

                string connectionString = "server=localhost;user id=root;password=;database=student;charset=utf8;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                UPDATE student
                SET is_banned = 1,
                    ban_until = @ban_until,
                    reason = @reason
                WHERE username = @username;
            ";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ban_until", banUntil);
                        cmd.Parameters.AddWithValue("@reason", reason);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    $"แบนผู้ใช้: {username}\n" +
                    $"ระยะเวลา: {banDays} วัน (จนถึง {banUntil:dd/MM/yyyy})\n" +
                    $"เหตุผล: {reason}",
                    "แบนผู้ใช้สำเร็จ",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                    );
            }
        }
    }
}
