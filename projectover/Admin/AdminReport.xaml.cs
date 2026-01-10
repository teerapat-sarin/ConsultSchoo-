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
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using MySql.Data.MySqlClient;

namespace projectover
{
    /// <summary>
    /// Interaction logic for AdminReport.xaml
    /// </summary>
    public partial class AdminReport : UserControl
    {
        private string connectionString = "server=localhost;database=student;uid=root;pwd=;";
        public AdminReport()
        {
            InitializeComponent();
            LoadStats();
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
        private void LoadStats()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // 1. จำนวนผู้ใช้ทั้งหมด
                var cmdTotalUsers = new MySqlCommand("SELECT COUNT(*) FROM student", conn);
                int totalUsers = Convert.ToInt32(cmdTotalUsers.ExecuteScalar());
                GreenNumberTextBlock.Text = totalUsers.ToString();

                // 2. จำนวนที่ทำ MBTI
                var cmdDoneMBTI = new MySqlCommand("SELECT COUNT(*) FROM student WHERE MBTI IS NOT NULL AND MBTI != ''", conn);
                int doneMBTI = Convert.ToInt32(cmdDoneMBTI.ExecuteScalar());
                RedNumberTextBlock.Text = doneMBTI.ToString();

                // 3. สัดส่วน MBTI
                var cmdMBTIStats = new MySqlCommand(
                    "SELECT MBTI, COUNT(*) AS CountMBTI FROM student WHERE MBTI IS NOT NULL GROUP BY MBTI", conn);

                Dictionary<string, int> mbtiCounts = new Dictionary<string, int>();
                using (var reader = cmdMBTIStats.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string mbti = reader["MBTI"].ToString();
                        int count = Convert.ToInt32(reader["CountMBTI"]);
                        mbtiCounts[mbti] = count;
                    }
                }

                // 16 Personalities
                string[] personalities = new string[]
                {
            "ISTJ","ISFJ","INFJ","INTJ",
            "ISTP","ISFP","INFP","INTP",
            "ESTP","ESFP","ENFP","ENTP",
            "ESTJ","ESFJ","ENFJ","ENTJ"
                };

                MBTIWrapPanel.Children.Clear();

                foreach (var p in personalities)
                {
                    double percent = 0;
                    if (mbtiCounts.ContainsKey(p))
                    {
                        percent = Math.Round((double)mbtiCounts[p] / doneMBTI * 100, 2);
                    }

                    TextBlock tb = new TextBlock
                    {
                        FontSize = 36,
                        FontWeight = FontWeights.Bold,
                        FontFamily = new FontFamily("Itim"),
                        Margin = new Thickness(10),
                        Width = 300,
                        Height = 90,
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap
                    };

                    // ส่วน MBTI เป็นสีดำ
                    tb.Inlines.Add(new Run(p + " ") { Foreground = Brushes.Black });

                    // ส่วน percent เป็นสีที่อ่านง่าย เช่น สีแดงเข้ม
                    tb.Inlines.Add(new Run($"{percent}%")
                    {
                        Foreground = Brushes.Red,
                        FontSize = 62 // เปลี่ยนขนาดตามที่ต้องการ
                    });

                    MBTIWrapPanel.Children.Add(tb);
                }
            }
        }
    }
}
