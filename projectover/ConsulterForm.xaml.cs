using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace projectover
{
    /// <summary>
    /// Interaction logic for ConsulterForm.xaml
    /// </summary>
    public partial class ConsulterForm : UserControl, INotifyPropertyChanged
    {
        private string _imagePath;

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
       
        public ConsulterForm()
        {
            InitializeComponent();
            this.DataContext = this; // ✅ ตั้ง DataContext ให้ตัวเอง
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Content = new Mainmenu();
            }

            // ✅ อ่านค่าจาก TextBox
            string name = TBName.Text.Trim();
            string fullname = TBFullname.Text.Trim();
            string role = CBRole.Text.Trim();
            string topic = TBTopic.Text.Trim();

            // ✅ ตรวจสอบว่าไม่เว้นว่าง
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(fullname) ||
                string.IsNullOrEmpty(role) || string.IsNullOrEmpty(topic))
            {
                MessageBox.Show("กรุณากรอกข้อมูลให้ครบทุกช่อง", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ✅ ตรวจสอบว่ามีเลือกรูปหรือไม่
            if (string.IsNullOrEmpty(ImagePath))
            {
                MessageBox.Show("กรุณาเลือกรูปภาพก่อนบันทึก", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ✅ ดึง Username จาก MainWindow
            string username = mainWindow?.CurrentUsername;
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("ไม่พบ Username ของผู้ใช้งาน", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string connectionString = "Server=127.0.0.1;Port=3306;Uid=root;Pwd=;Database=student;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // ✅ INSERT รวมทุกคอลัมน์ในครั้งเดียว รวม Username
                    string query = @"INSERT INTO consulter (Username, name, fullname, role, topic, image_path)
                             VALUES (@Username, @name, @fullname, @role, @topic, @image)
                             ON DUPLICATE KEY UPDATE 
                                 name=@name, fullname=@fullname, role=@role, topic=@topic, image_path=@image";
                    // ถ้า Username มีอยู่แล้ว จะอัปเดตข้อมูลแทน

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@fullname", fullname);
                        cmd.Parameters.AddWithValue("@role", role);
                        cmd.Parameters.AddWithValue("@topic", topic);
                        cmd.Parameters.AddWithValue("@image", ImagePath);

                        cmd.ExecuteNonQuery();
                    }
                    string updateStudent = "UPDATE student SET Name = @name, image_path = @image WHERE Username = @Username";
                    using (MySqlCommand cmd2 = new MySqlCommand(updateStudent, conn))
                    {
                        cmd2.Parameters.AddWithValue("@name", name);
                        cmd2.Parameters.AddWithValue("@image", ImagePath);
                        cmd2.Parameters.AddWithValue("@Username", username);
                        cmd2.ExecuteNonQuery();
                    }

                    MessageBox.Show("บันทึกข้อมูลสำเร็จ!", "สำเร็จ", MessageBoxButton.OK, MessageBoxImage.Information);

                    // ✅ ล้างค่า TextBox และรูป
                    TBName.Clear();
                    TBFullname.Clear();
                    CBRole.SelectedIndex = -1;
                    TBTopic.Clear();
                    ImagePath = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาดในการบันทึกข้อมูล: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "เลือกไฟล์รูปภาพ"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName; // ✅ แค่เก็บไว้รอบันทึกตอน Confirm_Click
            }
        }
        private void backbutton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Content = new ChooseStuorTea();
            }
        }
    }
}
