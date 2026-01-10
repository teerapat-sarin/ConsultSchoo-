using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// Interaction logic for ChatPage.xaml
    /// </summary>
    public partial class ChatPage : UserControl
    {

        public ObservableCollection<ChatRoomItem> ChatRooms { get; set; } = new();
        public ObservableCollection<ChatMessage> Messages { get; set; } = new();

        // ผู้ใช้ปัจจุบัน (จำลอง)
        private string CurrentUser
        {
            get
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    return mainWindow.CurrentUsername ?? "UnknownUser"; // fallback ถ้า null
                }
                return "UnknownUser";
            }
        }

        // ห้องที่เลือกอยู่ตอนนี้
        private ChatRoomItem currentRoom;

        public string CurrentUserAvatar { get; set; } = "";
        public string CurrentUserName { get; set; } = "";

        private bool isMenuOpen = false;
        public ChatPage()
        {
            InitializeComponent();
            DataContext = this;

            // โหลดรายชื่อห้องแชท
            LoadChatRooms();

            // ผูก ItemsSource
            ChatList.ItemsSource = Messages;
            ChatRoomList.ItemsSource = ChatRooms;

            HeaderGrid.DataContext = this;
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, SendCommandExecuted));
        }
        private void SendCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Send_Click(sender, e);
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
        private void LoadChatRooms()
        {
            try
            {
                string connectionString = "Server=127.0.0.1;Port=3306;Database=student;Uid=root;Pwd=;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"
                SELECT DISTINCT 
                    CASE WHEN SenderId = @currentUser THEN ReceiverId ELSE SenderId END AS ChatPartner
                FROM messages
                WHERE SenderId = @currentUser OR ReceiverId = @currentUser";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@currentUser", CurrentUser);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string partnerUsername = reader.GetString("ChatPartner");

                                // ดึงชื่อจริงจาก student.name
                                string displayName = GetDisplayName(partnerUsername);

                                // ดึงรูปจาก student.image_path
                                string avatarPath = GetUserAvatar(partnerUsername);

                                ChatRooms.Add(new ChatRoomItem
                                {
                                    Username = partnerUsername,  // ← ใช้ใน DB
                                    Name = displayName,          // ← ใช้โชว์
                                    Avatar = avatarPath,
                                    LastMessage = "คลิกเพื่อดูแชทกับ " + displayName
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading chat rooms: " + ex.Message);
            }
        }

        private string GetUserAvatar(string username)
        {
            string connectionString = "Server=127.0.0.1;Port=3306;Database=student;Uid=root;Pwd=;";
            string avatarPath = "C:\\Users\\acer\\Downloads\\default.jpg"; // 🔸 รูปเริ่มต้น ถ้าไม่มี

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // ✅ ดึง path จากตาราง student (คุณอาจเปลี่ยนชื่อ table ได้ถ้าไม่ตรง)
                    string sql = "SELECT image_path FROM student WHERE Username = @username LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        object result = cmd.ExecuteScalar();

                        if (result != null && File.Exists(result.ToString()))
                        {
                            avatarPath = result.ToString();
                        }
                    }
                }
            }
            catch
            {
                // เงียบไว้ ถ้ามีปัญหาก็ใช้ default image
            }

            return avatarPath;
        }

        private string GetDisplayName(string username)
        {
            string connectionString = "Server=127.0.0.1;Port=3306;Database=student;Uid=root;Pwd=;";
            string name = username; // fallback ถ้าไม่มีชื่อในฐานข้อมูล

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT name FROM student WHERE Username = @username LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                            name = result.ToString();
                    }
                }
            }
            catch
            {
                // ถ้ามี error ก็ให้ใช้ username เดิมแทน
            }

            return name;
        }

        // ===============================
        // 📌 เมื่อเลือกห้องแชท
        // ===============================
        private void ChatRoomList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChatRoomList.SelectedItem is ChatRoomItem selected)
            {
                currentRoom = selected;

                // อัปเดต header
                CurrentUserName = selected.Name;
                CurrentUserAvatar = selected.Avatar;
                HeaderGrid.DataContext = null;
                HeaderGrid.DataContext = this;

                // โหลดข้อความของห้องนี้
                LoadMessagesForRoom(selected.Username);
            }
        }

        // ===============================
        // 📌 โหลดข้อความในห้อง
        // ===============================
        private void LoadMessagesForRoom(string receiver)
        {
            Messages.Clear();
            string connectionString = "Server=127.0.0.1;Port=3306;Database=student;Uid=root;Pwd=;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
            SELECT SenderId, MessageText 
            FROM messages
            WHERE (SenderId = @user AND ReceiverId = @receiver)
               OR (SenderId = @receiver AND ReceiverId = @user)
            ORDER BY Timestamp ASC";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@user", CurrentUser);
                    cmd.Parameters.AddWithValue("@receiver", receiver);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string sender = reader.GetString("SenderId");
                            string message = reader.GetString("MessageText");
                            bool isMine = sender == CurrentUser;

                            // ✅ ดึงชื่อจริงของ sender จากตาราง student
                            string senderDisplayName = GetDisplayName(sender);

                            // ✅ ถ้าไม่ใช่ฝั่งเรา ให้ดึง avatar ของอีกฝั่ง
                            string avatarPath = null;
                            if (!isMine)
                            {
                                avatarPath = GetUserAvatar(sender);
                            }
                            else
                            {
                                avatarPath = CurrentUserAvatar; // รูปของเราเอง
                            }

                            Messages.Add(new ChatMessage
                            {
                                Text = message,
                                Align = isMine ? "Right" : "Left",
                                Background = isMine ? "#c8f7c5" : "#eeeeee",
                                Avatar = avatarPath,
                                SenderName = senderDisplayName // ✅ ใช้ชื่อจริงแทน username
                            });
                        }
                    }
                }
            }
        }


        // ===============================
        // 📌 ปุ่มส่งข้อความ
        // ===============================
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string msg = InputBox.Text.Trim();
            if (string.IsNullOrEmpty(msg) || currentRoom == null)
            {
                MessageBox.Show("กรุณาเลือกห้องก่อนส่งข้อความ");
                return;
            }

            Messages.Add(new ChatMessage
            {
                Text = msg,
                SenderName = CurrentUser,
                Align = "Right",
                Background = "#c8f7c5",
                Avatar = CurrentUserAvatar
            });

            SaveMessageToDatabase(msg, currentRoom.Username);
            InputBox.Clear();
        }

        // ===============================
        // 📌 บันทึกข้อความลงฐานข้อมูล
        // ===============================
        private void SaveMessageToDatabase(string text, string receiver)
        {
            string connectionString = "Server=127.0.0.1;Port=3306;Database=student;Uid=root;Pwd=;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO messages (SenderId, ReceiverId, MessageText) VALUES (@sender, @receiver, @message)";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@sender", CurrentUser);
                    cmd.Parameters.AddWithValue("@receiver", receiver);
                    cmd.Parameters.AddWithValue("@message", text);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void Score_Click(object sender, RoutedEventArgs e)
        {
            if (currentRoom == null)
            {
                MessageBox.Show("กรุณาเลือกผู้ให้คำปรึกษาก่อนให้คะแนน");
                return;
            }

            var ratingWindow = new RatingWindow(currentRoom.Username);
            ratingWindow.ShowDialog();
        }

        // ===============================
        // 📌 Converter
        // ===============================
        public class AlignToColumnConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                string align = value as string;
                return align == "Right" ? 1 : 0;
            }
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public class SenderAvatarVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                string align = value as string;
                return align == "Right" ? Visibility.Collapsed : Visibility.Visible;
            }
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }

    // ===============================
    // 📌 Model
    // ===============================
    public class ChatMessage
    {
        public string Text { get; set; }
        public string Align { get; set; }
        public string Background { get; set; }
        public string Avatar { get; set; }
        public string SenderName { get; set; }

        public ChatMessage() { } // ต้องมี constructor เปล่า
    }

    public class ChatRoomItem
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string LastMessage { get; set; }
        public ObservableCollection<ChatMessage> Messages { get; set; } = new();
    }
}
