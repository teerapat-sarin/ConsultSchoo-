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
using System.Windows.Controls.Primitives;
using MySql.Data.MySqlClient;
using System.IO;
using System.Windows.Threading;
using System.Runtime.ConstrainedExecution;


namespace projectover
{
    /// <summary>
    /// Interaction logic for Mainmenu.xaml
    /// </summary>


    public partial class Mainmenu : UserControl
    {
        private readonly string[] bannerPaths = new string[]
            {
                    @"C:\Users\acer\Desktop\Professor\SaDuang.jpg",
                    @"C:\Users\acer\Desktop\Professor\chayodoms.jpg",
                    @"C:\Users\acer\Downloads\KhunaKorn.jpg",
                    @"C:\Users\acer\Desktop\Professor\Pe_jumpa.jpg",
                   @"C:\Users\acer\Desktop\Professor\Peechayah.jpg",
                   @"D:\UseThisFileForProject\Bank.jpg",
                   @"D:\UseThisFileForProject\Best.jpg"

            };
        private int currentBannerIndex = 0;
        private DispatcherTimer bannerTimer;

        string connectionString = "Server=127.0.0.1;Port=3306;Uid=root;Pwd=;Database=student;";
        string studentId;
        public string CurrentStudentId { get; set; }

        private const double MenuSlideDurationSeconds = 0.3;
        private bool isMenuOpen = false;
        public Mainmenu()
        {
            InitializeComponent();
            StartBannerSlideshow();
        }
        private void StartBannerSlideshow()
        {
            bannerTimer = new DispatcherTimer();
            bannerTimer.Interval = TimeSpan.FromSeconds(3);
            bannerTimer.Tick += BannerTimer_Tick;
            bannerTimer.Start();
        }

        private void BannerTimer_Tick(object sender, EventArgs e)
        {
            int nextIndex = (currentBannerIndex + 1) % bannerPaths.Length;
            string nextImage = bannerPaths[nextIndex];

            double width = BannerImage.ActualWidth;

            // ✅ ตั้งภาพถัดไปไว้ทาง "ซ้าย" นอกจอ (แทนจากเดิมที่อยู่ขวา)
            BannerImageNext.Source = new BitmapImage(new Uri(nextImage, UriKind.Absolute));
            BannerTranslateNext.X = -width;

            // ✅ ปรับทิศทาง Animation: ปัจจุบันออกไปทางขวา, ถัดไปเข้ามาจากซ้าย
            var slideOut = new DoubleAnimation(0, width, TimeSpan.FromSeconds(1))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            var slideIn = new DoubleAnimation(-width, 0, TimeSpan.FromSeconds(1))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            slideIn.Completed += (s, a) =>
            {
                BannerImage.Source = BannerImageNext.Source;
                BannerTranslate.X = 0;
                BannerTranslateNext.X = 0;
                currentBannerIndex = nextIndex;
            };

            BannerTranslate.BeginAnimation(TranslateTransform.XProperty, slideOut);
            BannerTranslateNext.BeginAnimation(TranslateTransform.XProperty, slideIn);
        }
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
    }
}
