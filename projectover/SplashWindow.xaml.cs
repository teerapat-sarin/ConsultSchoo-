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
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Threading.Tasks;

namespace projectover
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
            // เรียกใช้ฟังก์ชันหลักเมื่อหน้าต่างโหลดเสร็จ
            this.Loaded += SplashWindow_Loaded;
        }

        private async void SplashWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 1. รอเป็นเวลา 3 วินาที (ปรับเปลี่ยนได้ตามต้องการ)
            await Task.Delay(3000);

            // 2. เริ่ม Animation Fade Out
            StartFadeOutAnimation();
        }

        private void StartFadeOutAnimation()
        {
            // สร้าง Animation ให้ Opacity เปลี่ยนจาก 1 เป็น 0
            DoubleAnimation fadeOut = new DoubleAnimation()
            {
                From = 1.0,  // Opacity เริ่มต้น (ทึบ)
                To = 0.0,    // Opacity สุดท้าย (โปร่งใส)
                Duration = new Duration(TimeSpan.FromSeconds(1.0)) // ใช้เวลา 1 วินาทีในการ Fade
            };

            // เมื่อ Animation จบ ให้ทำการเปลี่ยนหน้า
            fadeOut.Completed += FadeOut_Completed;

            // เริ่ม Animation ที่ตัว Window เอง (Property Opacity)
            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }

        private void FadeOut_Completed(object? sender, EventArgs e)
        {
            // 3. เปิดหน้า Login (สมมติว่าคุณเปลี่ยนชื่อ MainWindow เป็น LoginPage)
            MainWindow loginWindow = new MainWindow(); // หรือหน้า Login/Main ตัวจริงของคุณ
            loginWindow.Show();

            // 4. ปิดหน้า Splash Screen นี้
            this.Close();
        }
    }
}
