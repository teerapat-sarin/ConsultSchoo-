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

namespace projectover
{
    
    /// <summary>
    /// Interaction logic for ConsulterShowDetail.xaml
    /// </summary>
    public partial class ConsulterShowDetail : UserControl
    {
        private int currentConsulterId;
        string connectionString = "server=localhost;user=root;password=;database=student;";
        public ConsulterShowDetail()
        {
            InitializeComponent();
        }
        public void LoadConsulterData(int id)
        {
            currentConsulterId = id;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // ✅ ดึงข้อมูล + รูปจาก table student
                    string query = @"
                SELECT c.name, c.fullname, c.role, c.topic, s.image_path
                FROM consulter c
                JOIN student s ON c.username = s.username
                WHERE c.id = @id
            ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TBName.Text = reader["name"].ToString();
                            TBFullname.Text = reader["fullname"].ToString();
                            TBRole.Text = reader["role"].ToString();
                            TBTopic.Text = reader["topic"].ToString();

                            // ✅ โหลดรูปภาพจาก student.image_path
                            string imagePath = reader["image_path"].ToString();

                            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                            {
                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.EndInit();

                                var brush = new ImageBrush(bitmap)
                                {
                                    Stretch = System.Windows.Media.Stretch.UniformToFill
                                };

                                BorderImage.Background = brush;
                            }
                            else
                            {
                                BorderImage.Background = System.Windows.Media.Brushes.LightGray;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }


        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            // ✅ ดึง MainWindow ปัจจุบัน
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow == null || string.IsNullOrEmpty(mainWindow.CurrentUsername))
            {
                MessageBox.Show("ไม่พบข้อมูลผู้ใช้งานปัจจุบัน (CurrentUsername)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string senderId = mainWindow.CurrentUsername; // จาก Login
            string receiverId = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // ✅ 1. ดึง Username ของผู้ให้คำปรึกษา (จาก id)
                    string getUsernameQuery = "SELECT Username FROM consulter WHERE id = @id";
                    using (MySqlCommand getCmd = new MySqlCommand(getUsernameQuery, conn))
                    {
                        getCmd.Parameters.AddWithValue("@id", currentConsulterId); // คุณต้องมีตัวแปรเก็บ id ที่ใช้ใน LoadConsulterData()
                        object result = getCmd.ExecuteScalar();

                        if (result == null)
                        {
                            MessageBox.Show("ไม่พบข้อมูล Username ของที่ปรึกษาในฐานข้อมูล", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        receiverId = result.ToString();
                    }
                    // ✅ 2. ตรวจว่าผู้ใช้กำลังจะคุยกับตัวเองหรือไม่
                    if (receiverId.Equals(senderId, StringComparison.OrdinalIgnoreCase))
                    {
                        ShowLargeMessage("คุณไม่สามารถคุยกับตัวเองได้", "แจ้งเตือน");
                        return;
                    }

                    // ✅ 2. แทรกข้อมูลลงในตาราง messages
                    string insertMessageQuery = @"INSERT INTO messages (SenderId, ReceiverId, MessageText, Timestamp)
                                          VALUES (@sender, @receiver, @message, @time)";

                    using (MySqlCommand insertCmd = new MySqlCommand(insertMessageQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@sender", senderId);
                        insertCmd.Parameters.AddWithValue("@receiver", receiverId);
                        insertCmd.Parameters.AddWithValue("@message", "เริ่มต้นการสนทนาใหม่");
                        insertCmd.Parameters.AddWithValue("@time", DateTime.Now);

                        insertCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("สร้างข้อความเริ่มต้นสำเร็จ!", "สำเร็จ", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ShowLargeMessage(string message, string title)
        {
            Window parentWindow = Window.GetWindow(this);
            Window alert = new Window
            {
                Title = title,
                Width = 400,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Background = Brushes.White,
                WindowStyle = WindowStyle.ToolWindow
            };
            if (parentWindow != null)
                alert.Owner = parentWindow;

            var textBlock = new TextBlock
            {
                Text = message,
                FontSize = 20, // ✅ ขนาดฟอนต์ใหญ่ขึ้น
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(20)
            };

            var okButton = new Button
            {
                Content = "ตกลง",
                FontSize = 16,
                Padding = new Thickness(10, 5, 10, 5),
                Width = 100,
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            okButton.Click += (s, e) => alert.Close();

            var stack = new StackPanel();
            stack.Children.Add(textBlock);
            stack.Children.Add(okButton);

            alert.Content = stack;
            alert.ShowDialog();
        }
    }

}
