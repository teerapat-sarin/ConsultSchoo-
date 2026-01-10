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
    /// Interaction logic for CardConsulter.xaml
    /// </summary>
    public partial class CardConsulter : UserControl
    {
        public CardConsulter()
        {
            InitializeComponent();
        }
        public string FullNameAndRate => $"{Role} | {Rate}";

        // DisplayName
        public static readonly DependencyProperty DisplayNameProperty =
            DependencyProperty.Register("DisplayName", typeof(string), typeof(CardConsulter));

        public string DisplayName
        {
            get => (string)GetValue(DisplayNameProperty);
            set => SetValue(DisplayNameProperty, value);
        }

        // FullName
        public static readonly DependencyProperty FullNameProperty =
            DependencyProperty.Register("FullName", typeof(string), typeof(CardConsulter));

        public string FullName
        {
            get => (string)GetValue(FullNameProperty);
            set => SetValue(FullNameProperty, value);
        }

        // Role
        public static readonly DependencyProperty RoleProperty =
            DependencyProperty.Register("Role", typeof(string), typeof(CardConsulter));

        public string Role
        {
            get => (string)GetValue(RoleProperty);
            set => SetValue(RoleProperty, value);
        }

        // Topic
        public static readonly DependencyProperty TopicProperty =
            DependencyProperty.Register("Topic", typeof(string), typeof(CardConsulter));

        public string Topic
        {
            get => (string)GetValue(TopicProperty);
            set => SetValue(TopicProperty, value);
        }

        // rate
        public static readonly DependencyProperty RateProperty =
            DependencyProperty.Register("Rate", typeof(string), typeof(CardConsulter));

        public string Rate
        {
            get => (string)GetValue(RateProperty);
            set => SetValue(RateProperty, value);
        }

        // ImagePath
        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(string), typeof(CardConsulter),
                new PropertyMetadata("", OnImagePathChanged));

        public string ImagePath
        {
            get => (string)GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        private static void OnImagePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CardConsulter;
            string path = e.NewValue as string;
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    control.CardImage.Source = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
                }
                catch { }
            }
        }
    }
}
