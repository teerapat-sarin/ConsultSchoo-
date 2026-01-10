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
using MySql.Data.MySqlClient;
using System.IO;

namespace projectover
{
    /// <summary>
    /// Interaction logic for FindYourSelf.xaml
    /// </summary>
    public partial class FindYourSelf : UserControl
    {
        public string CurrentStudentId { get; set; }
        public FindYourSelf(string studentId)
        {
            InitializeComponent();
            CurrentStudentId = studentId;
            LoadQuestion();
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

        private void LoadQuestion()
        {
            WrapPanelContainer.Children.Clear();

            string connectionString = "server=localhost;user id=root;password=;database=student;charset=utf8;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // ✅ เพิ่มเงื่อนไขไม่ดึงแถวที่ Username = 'Admin'
                string query = @"
                                SELECT id, Question , dimension
                                FROM question
                            ";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32("id");

                        // ✅ สร้าง UserControl จาก CardConsulter
                        var card = new CardQuesion
                        {
                            QuestionId = reader.GetInt32("id"),   // ✅ ตั้งค่า id
                            Question = reader.GetString("Question"),
                            Dimension = reader["dimension"].ToString()
                        };
                        // ✅ เพิ่มการ์ดลงใน WrapPanel
                        WrapPanelContainer.Children.Add(card);
                    }
                }
            }
        }
        private void CheckResult_Click(object sender, RoutedEventArgs e)
        {
            var totals = new Dictionary<string, int>()
    {
        {"E",0}, {"I",0}, {"S",0}, {"N",0},
        {"T",0}, {"F",0}, {"J",0}, {"P",0}
    };

            foreach (var child in WrapPanelContainer.Children)
            {
                if (child is CardQuesion card)
                {
                    if (!string.IsNullOrEmpty(card.TargetDimension))
                    {
                        totals[card.TargetDimension] += card.Score;
                    }
                }
            }

            // สรุป MBTI 4 ตัวอักษร
            string mbtiResult = GetMBTI(totals);

            // บันทึกลงฐานข้อมูล
            SaveMBTIToDatabase(CurrentStudentId, mbtiResult);

            // ✅ เปิดหน้า DetailMBTi และส่ง CurrentStudentId
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var detailPage = new DetailMBTi(CurrentStudentId);
                mainWindow.MainFrame.Content = detailPage;
            }
        }
        private string GetMBTI(Dictionary<string, int> totals)
        {
            // เปรียบเทียบแต่ละคู่
            string first = totals["E"] >= totals["I"] ? "E" : "I";
            string second = totals["S"] >= totals["N"] ? "S" : "N";
            string third = totals["T"] >= totals["F"] ? "T" : "F";
            string fourth = totals["J"] >= totals["P"] ? "J" : "P";

            return first + second + third + fourth;
        }
        private void SaveMBTIToDatabase(string studentId, string mbti)
        {
            string connectionString = "server=localhost;user id=root;password=;database=student;charset=utf8;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "UPDATE student SET MBTI = @mbti WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@mbti", mbti);
                    cmd.Parameters.AddWithValue("@id", studentId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("บันทึก MBTI สำเร็จ");
                    }
                    else
                    {
                        Console.WriteLine("ไม่พบรหัสนักเรียน");
                    }
                }
            }
        }
    }
}
