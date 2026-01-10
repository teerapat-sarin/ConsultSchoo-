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
using System.IO;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace projectover
{
    /// <summary>
    /// Interaction logic for AdminUser.xaml
    /// </summary>
    public partial class AdminUser : UserControl
    {
        public AdminUser()
        {
            InitializeComponent();
            LoadConsulters();
        }
        private void Card_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {

        }

        private bool isMenuOpen = false; // สถานะเมนูเปิดอยู่ไหม

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isMenuOpen)
            {
                // 🟢 สร้าง MenuPanel ถ้ายังไม่มี
                if (MenuPopup.Child == null)
                {
                    var menu = new AdminMenu();
                    menu.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    menu.Arrange(new Rect(menu.DesiredSize));
                    MenuPopup.Child = menu;
                }

                MenuPopup.AllowsTransparency = true;
                MenuPopup.StaysOpen = true; // ✅ ปิด auto-close (จะจับเอง)
                MenuPopup.PlacementTarget = this;
                MenuPopup.Placement = PlacementMode.Relative;
                MenuPopup.HorizontalOffset = 10;
                MenuPopup.VerticalOffset = 50;
                MenuPopup.IsOpen = true;

                // 🟢 Slide In Animation
                var menuPanel = MenuPopup.Child as AdminMenu;
                if (menuPanel != null)
                {
                    var animIn = new DoubleAnimation
                    {
                        From = -menuPanel.ActualWidth,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.3),
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                    };
                    menuPanel.MenuTranslate.BeginAnimation(TranslateTransform.XProperty, animIn);
                }

                isMenuOpen = true;

                // ✅ จับคลิกนอก popup
                Application.Current.MainWindow.PreviewMouseDown += MainWindow_PreviewMouseDown;
            }
            else
            {
                // 🔴 ถ้ากด Hamburger อีกครั้งให้ปิด
                ClosePopupWithAnimation();
            }
        }

        private void MainWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // ตรวจว่าคลิกอยู่นอก Popup หรือไม่
            if (MenuPopup.IsOpen && !IsClickInsidePopup(e))
            {
                ClosePopupWithAnimation();
            }
        }

        private bool IsClickInsidePopup(MouseButtonEventArgs e)
        {
            if (MenuPopup?.Child is FrameworkElement child)
            {
                var pos = e.GetPosition(child);
                return pos.X >= 0 && pos.X <= child.ActualWidth &&
                       pos.Y >= 0 && pos.Y <= child.ActualHeight;
            }
            return false;
        }

        private void ClosePopupWithAnimation()
        {
            var menuPanel = MenuPopup.Child as AdminMenu;
            if (menuPanel == null) return;

            var animOut = new DoubleAnimation
            {
                From = 0,
                To = -menuPanel.ActualWidth,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            animOut.Completed += (s, _) =>
            {
                MenuPopup.IsOpen = false;
                isMenuOpen = false;
                Application.Current.MainWindow.PreviewMouseDown -= MainWindow_PreviewMouseDown;
            };

            menuPanel.MenuTranslate.BeginAnimation(TranslateTransform.XProperty, animOut);
        }


        private void LoadConsulters()
        {
            WrapPanelContainer.Children.Clear();

            string connectionString = "server=localhost;user id=root;password=;database=student;charset=utf8;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // ✅ เพิ่มเงื่อนไขไม่ดึงแถวที่ Username = 'Admin'
                string query = @"
                                SELECT id, username, image_path, name, role
                                FROM student
                                WHERE username <> 'Admin' 
                                  AND role = 'Student';
                            ";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32("id");

                        // ✅ สร้าง UserControl จาก CardConsulter
                        var card = new CardForAdmin
                        {
                            DisplayName = reader["name"].ToString(),
                            Username = reader["Username"].ToString(),
                            ImagePath = reader["image_path"].ToString(),
                            Tag = id // เก็บ id สำหรับตอนเปิดรายละเอียด
                        };

                        // ✅ เพิ่มการ์ดลงใน WrapPanel
                        WrapPanelContainer.Children.Add(card);
                    }
                }
            }
        }
        private void Reban_Click(object sender, RoutedEventArgs e)
        {
            WrapPanelContainer.Children.Clear(); // ล้างการ์ดเดิม

            string connectionString = "server=localhost;user id=root;password=;database=student;charset=utf8;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // เลือกเฉพาะผู้ที่ถูกแบน is_banned = 1 และไม่ใช่ Admin
                string query = @"
            SELECT id, username, image_path, name, role
            FROM student
            WHERE username <> 'Admin'
              AND is_banned = 1;
        ";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32("id");

                        // สร้าง CardForBanned
                        var card = new CardForBanned
                        {
                            DisplayName = reader["name"].ToString(),
                            Username = reader["username"].ToString(),
                            ImagePath = reader["image_path"].ToString(),
                            Tag = id
                        };

                        // เพิ่มลง WrapPanel
                        WrapPanelContainer.Children.Add(card);
                    }
                }
            }
        }

    }
}
