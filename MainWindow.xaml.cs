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

namespace CRM
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new ClientsPage());
        }

        private void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientsPage());
        }

        private void EventsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EventsPage());
        }

        private void TransactionsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TransactionsPage());
        }

        private void AnalyticButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AnalyticPage());
        }

        private void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EmailPage());
        }

        private void SurveyButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SurveyCreatePage());
        }
    }
}
