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

namespace projectover
{
    /// <summary>
    /// Interaction logic for CardQuesion.xaml
    /// </summary>
    public partial class CardQuesion : UserControl
    {
        public int QuestionId { get; set; }  // id ของ question

        // DependencyProperty สำหรับ binding TextBlock หรือ TextBox ใน XAML
        public static readonly DependencyProperty QuestionProperty =
            DependencyProperty.Register("Question", typeof(string), typeof(CardQuesion));

        public string Question
        {
            get => (string)GetValue(QuestionProperty);
            set => SetValue(QuestionProperty, value);
        }
        public string Dimension { get; set; } // เช่น "E" หรือ "I"

        public int Score { get; private set; } // เก็บคะแนนที่เลือก
        public CardQuesion()
        {
            InitializeComponent();
        }
        private void Score_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag != null)
            {
                var parts = rb.Tag.ToString().Split(',');
                string color = parts[0];      // "Green" หรือ "Red"
                int selectedScore = int.Parse(parts[1]);

                Score = selectedScore;

                if (color == "Green") // เขียว = Dimension เดิม
                    TargetDimension = Dimension;
                else // แดง = Dimension ตรงข้าม
                    TargetDimension = GetOppositeDimension(Dimension);

                Console.WriteLine($"Q{QuestionId}: {Dimension} -> {TargetDimension} = {Score}");
            }
        }

        // แปลง Dimension ตรงข้าม
        private string GetOppositeDimension(string dim)
        {
            return dim switch
            {
                "E" => "I",
                "I" => "E",
                "S" => "N",
                "N" => "S",
                "T" => "F",
                "F" => "T",
                "J" => "P",
                "P" => "J",
                _ => dim
            };
        }

        // ตัวแปรบอกว่า Score จะไปเก็บที่ Dimension ไหน
        public string TargetDimension { get; private set; }
    }
}
