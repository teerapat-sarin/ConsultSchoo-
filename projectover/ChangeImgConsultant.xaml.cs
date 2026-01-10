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
    /// Interaction logic for ChangeImgConsultant.xaml
    /// </summary>
    public partial class ChangeImgConsultant : UserControl, INotifyPropertyChanged
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
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public ChangeImgConsultant()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.png;*.jpeg;*.bmp;*.gif";

            if (dlg.ShowDialog() == true)
            {
                ImagePath = dlg.FileName; // ✅ เมื่อ set property → UI จะอัปเดต
            }
        }


        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow == null) return;

            string studentId = mainWindow.CurrentStudentId;
            string connectionString = "Server=127.0.0.1;Port=3306;Uid=root;Pwd=;Database=student;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // ✅ 1) ดึง username จาก student โดยอิง studentId
                    string getUserSql = "SELECT username FROM student WHERE id = @id";
                    string username = null;

                    using (MySqlCommand getCmd = new MySqlCommand(getUserSql, conn))
                    {
                        getCmd.Parameters.AddWithValue("@id", studentId);
                        var obj = getCmd.ExecuteScalar();
                        if (obj == null)
                        {
                            MessageBox.Show("ไม่พบข้อมูลผู้ใช้", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        username = obj.ToString();
                    }

                    // ✅ 2) อัปเดตข้อมูลใน consulter
                    string updateConsulter =
                        "UPDATE consulter SET fullname=@fullname, role=@role, topic=@topic, name=@name WHERE username=@user";

                    using (MySqlCommand cmd1 = new MySqlCommand(updateConsulter, conn))
                    {
                        cmd1.Parameters.AddWithValue("@fullname", TBFullname.Text);
                        cmd1.Parameters.AddWithValue("@role", (CBRole.SelectedItem as ComboBoxItem)?.Content.ToString());
                        cmd1.Parameters.AddWithValue("@topic", TBTopic.Text);
                        cmd1.Parameters.AddWithValue("@name", TBName.Text);
                        cmd1.Parameters.AddWithValue("@user", username);
                        cmd1.ExecuteNonQuery();
                    }

                    // ✅ 3) อัปเดต name + image_path ในตาราง student
                    string updateStudent =
                        "UPDATE student SET name=@name, image_path=@image WHERE id=@id";

                    using (MySqlCommand cmd2 = new MySqlCommand(updateStudent, conn))
                    {
                        cmd2.Parameters.AddWithValue("@name", TBName.Text);
                        cmd2.Parameters.AddWithValue("@image", ImagePath); // ✅ ตรงนี้เก็บ path
                        cmd2.Parameters.AddWithValue("@id", studentId);
                        cmd2.ExecuteNonQuery();
                    }

                    MessageBox.Show("อัปเดตข้อมูลสำเร็จ!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    mainWindow.MainFrame.Content = new Mainmenu();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void backbutton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Content = new Mainmenu();
            }
        }
    }
}
