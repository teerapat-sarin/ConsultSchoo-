using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;
using MySql.Data.MySqlClient;

namespace projectover
{
    /// <summary>
    /// Interaction logic for ReportPage.xaml
    /// </summary>
    public partial class ReportPage : UserControl
    {
        public ReportPage()
        {
            InitializeComponent();
            LoadAdmin();
        }
        private bool isMenuOpen = false; // สถานะเมนูเปิดอยู่ไหม

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isMenuOpen)
            {
                // 🟢 สร้าง MenuPanel ถ้ายังไม่มี
                if (MenuPopup.Child == null)
                {
                    var menu = new MenuPanel();
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
                var menuPanel = MenuPopup.Child as MenuPanel;
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
            var menuPanel = MenuPopup.Child as MenuPanel;
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
        private void LoadAdmin()
        {
            WrapPanelContainer.Children.Clear();

            string connectionString = "server=localhost;user id=root;password=;database=student;charset=utf8;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // ✅ เพิ่มเงื่อนไขไม่ดึงแถวที่ Username = 'Admin'
                string query = @"
                                SELECT id, name, fullname, role, topic, image_path 
                                FROM consulter 
                                WHERE username = 'Admin';
                            ";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32("id");

                        // ✅ สร้าง UserControl จาก CardConsulter
                        var card = new CardConsulter
                        {
                            DisplayName = reader["name"].ToString(),
                            FullName = reader["fullname"].ToString(),
                            Role = reader["role"].ToString(),
                            Topic = reader["topic"].ToString(),
                            ImagePath = reader["image_path"].ToString(),
                            Tag = id // เก็บ id สำหรับตอนเปิดรายละเอียด
                        };

                        // ✅ เพิ่ม event คลิกเพื่อเปิดรายละเอียด
                        card.MouseLeftButtonUp += Card_Click;

                        // ✅ เพิ่มการ์ดลงใน WrapPanel
                        WrapPanelContainer.Children.Add(card);
                    }
                }
            }
        }
        private void Card_Click(object sender, MouseButtonEventArgs e)
        {
            var card = sender as CardConsulter;
            if (card == null) return;
            int id = (int)card.Tag; // ดึง id ของผู้ให้คำปรึกษา

            // ✅ แสดง popup / หน้ารายละเอียด
            ConsulterShowDetail detail = new ConsulterShowDetail();
            detail.LoadConsulterData(id); // เรียกใช้เมธอดในอีก UserControl

            // แสดงแบบ Popup
            Window popupWindow = new Window
            {
                Title = "รายละเอียดผู้ให้คำปรึกษา",
                Content = detail,
                Width = 1000,
                Height = 800,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                Owner = Application.Current.MainWindow,
                WindowStyle = WindowStyle.None,  // ✅ ไม่มีขอบ
                AllowsTransparency = true,       // ✅ โปร่งใสได้
                Background = Brushes.Transparent,
                ShowInTaskbar = false,
                Topmost = true// ✅ โปร่งใส
            };
            // ✅ เพิ่ม event จับคลิกใน MainWindow
            MouseButtonEventHandler handler = null;
            handler = (s, ev) =>
            {
                // ตรวจว่า MouseClick อยู่นอก popupWindow
                var pos = ev.GetPosition(popupWindow);
                if (pos.X < 0 || pos.Y < 0 || pos.X > popupWindow.ActualWidth || pos.Y > popupWindow.ActualHeight)
                {
                    popupWindow.Close();
                    Application.Current.MainWindow.PreviewMouseDown -= handler; // ลบ event หลังปิด
                }
            };

            Application.Current.MainWindow.PreviewMouseDown += handler;

            popupWindow.Show();
        }

    }
}
