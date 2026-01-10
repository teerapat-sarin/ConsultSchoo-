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
using System.Xml.Linq;
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace projectover
{
    /// <summary>
    /// Interaction logic for ChangeImg.xaml
    /// </summary>
    public partial class ChangeImg : UserControl, INotifyPropertyChanged
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
        public ChangeImg()
        {
            InitializeComponent();
            this.DataContext = this;
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
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;

            if (string.IsNullOrEmpty(ImagePath))
            {
                MessageBox.Show("กรุณาเลือกรูปภาพก่อนบันทึก", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(TBName.Text))
            {
                MessageBox.Show("กรุณากรอกชื่อเล่น", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string connectionString = "Server=127.0.0.1;Port=3306;Uid=root;Pwd=;Database=student;";
            string studentId = mainWindow?.CurrentStudentId;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "UPDATE student SET `Name` = @name, `image_path` = @image WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", TBName.Text);
                        cmd.Parameters.AddWithValue("@image", ImagePath);
                        cmd.Parameters.AddWithValue("@id", studentId);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("บันทึกข้อมูลสำเร็จ!", "สำเร็จ", MessageBoxButton.OK, MessageBoxImage.Information);

                    ImagePath = null;
                    TBName.Text = "";

                    if (mainWindow != null)
                    {
                        mainWindow.MainFrame.Content = new Mainmenu();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาดในการบันทึกข้อมูล: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void backbutton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Content = new Mainmenu();
            }
        }
    }
}
