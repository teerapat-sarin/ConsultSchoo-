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

namespace projectover
{
    /// <summary>
    /// Interaction logic for BanDialog.xaml
    /// </summary>
    public partial class BanDialog : Window
    {

        public int BanDays { get; private set; } = 0;
        public string ReasonText { get; private set; } = "";
        public BanDialog()
        {
            InitializeComponent();
        }
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(DaysTextBox.Text, out int days) && days > 0)
            {
                BanDays = days;
                ReasonText = ReasonTextBox.Text?.Trim() ?? "";
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("กรุณากรอกจำนวนวันให้ถูกต้อง", "ข้อผิดพลาด");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
