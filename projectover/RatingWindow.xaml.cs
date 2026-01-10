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
using MySql.Data.MySqlClient;

namespace projectover
{
    /// <summary>
    /// Interaction logic for RatingWindow.xaml
    /// </summary>
    public partial class RatingWindow : Window
    {
        private int selectedRating = 0;
        private string consultantUsername;
        public RatingWindow(string username)
        {
            InitializeComponent();
            consultantUsername = username;
            CreateStars();
        }
        private void CreateStars()
        {
            for (int i = 1; i <= 5; i++)
            {
                var star = new TextBlock
                {
                    Text = "★",
                    FontSize = 40,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(5),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Tag = i
                };
                star.MouseEnter += Star_MouseEnter;
                star.MouseLeave += Star_MouseLeave;
                star.MouseLeftButtonUp += Star_Click;
                StarPanel.Children.Add(star);
            }
        }

        private void Star_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            int hover = (int)((TextBlock)sender).Tag;
            UpdateStarColor(hover);
        }

        private void Star_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            UpdateStarColor(selectedRating);
        }

        private void Star_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            selectedRating = (int)((TextBlock)sender).Tag;
            UpdateStarColor(selectedRating);
        }

        private void UpdateStarColor(int upTo)
        {
            for (int i = 0; i < StarPanel.Children.Count; i++)
            {
                var star = StarPanel.Children[i] as TextBlock;
                star.Foreground = (i < upTo) ? Brushes.Gold : Brushes.Gray;
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRating == 0)
            {
                MessageBox.Show("กรุณาเลือกคะแนนก่อน", "แจ้งเตือน", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string connStr = "Server=127.0.0.1;Port=3306;Database=student;Uid=root;Pwd=;";
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    // ดึงค่า rate เดิมและจำนวนครั้ง
                    string sqlSelect = "SELECT rate, rate_count FROM consulter WHERE username = @user LIMIT 1";
                    double oldRate = 0;
                    int rateCount = 0;

                    using (MySqlCommand cmd = new MySqlCommand(sqlSelect, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", consultantUsername);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                oldRate = reader["rate"] != DBNull.Value ? Convert.ToDouble(reader["rate"]) : 0;
                                rateCount = reader["rate_count"] != DBNull.Value ? Convert.ToInt32(reader["rate_count"]) : 0;
                            }
                        }
                    }
                    // คำนวณค่าเฉลี่ยใหม่
                    double newRate = ((oldRate * rateCount) + selectedRating) / (rateCount + 1);

                    // จำกัดไม่ให้เกิน 5 (กันไว้เฉย ๆ)
                    if (newRate > 5)
                        newRate = 5;

                    // เก็บไว้ในฐานข้อมูลเป็นทศนิยม 1 ตำแหน่ง (เช่น 3.4, 4.6)
                    newRate = Math.Round(newRate, 1);

                    // อัปเดตกลับ
                    string sqlUpdate = "UPDATE consulter SET rate = @rate, rate_count = @count WHERE username = @user";
                    using (MySqlCommand cmd = new MySqlCommand(sqlUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@rate", Math.Round(newRate, 2));
                        cmd.Parameters.AddWithValue("@count", rateCount + 1);
                        cmd.Parameters.AddWithValue("@user", consultantUsername);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show($"ขอบคุณที่ให้คะแนน {selectedRating} ดาว!", "สำเร็จ", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาดในการบันทึกคะแนน: " + ex.Message);
            }
        }
    }
}
