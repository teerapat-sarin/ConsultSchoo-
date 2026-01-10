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
    /// Interaction logic for CardForBanned.xaml
    /// </summary>
    public partial class CardForBanned : UserControl
    {
        public CardForBanned()
        {
            InitializeComponent();
        }
        // DisplayName
        public static readonly DependencyProperty DisplayNameProperty =
            DependencyProperty.Register("DisplayName", typeof(string), typeof(CardForBanned));

        public string DisplayName
        {
            get => (string)GetValue(DisplayNameProperty);
            set => SetValue(DisplayNameProperty, value);
        }

        public static readonly DependencyProperty FullNameProperty =
             DependencyProperty.Register("Username", typeof(string), typeof(CardForBanned));

        public string Username
        {
            get => (string)GetValue(FullNameProperty);
            set => SetValue(FullNameProperty, value);
        }

        public static readonly DependencyProperty ImagePathProperty =
    DependencyProperty.Register("ImagePath", typeof(string), typeof(CardForBanned),
        new PropertyMetadata("", OnImagePathChanged));

        public string ImagePath
        {
            get => (string)GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        private static void OnImagePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CardForBanned;
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

        private void ReBan_Click(object sender, RoutedEventArgs e)
        {
            string username = this.Username;

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("ไม่พบ Username ของผู้ใช้งาน", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string connectionString = "server=localhost;user id=root;password=;database=student;charset=utf8;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"
                UPDATE student
                SET is_banned = 0,
                    ban_until = NULL
                WHERE username = @username;
            ";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show($"ปลดแบนผู้ใช้ {username} เรียบร้อยแล้ว", "สำเร็จ", MessageBoxButton.OK, MessageBoxImage.Information);

                    // ✅ ลบตัว Card นี้ออกจาก WrapPanel
                    if (this.Parent is Panel panel)
                    {
                        panel.Children.Remove(this);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
