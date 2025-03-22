using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using Npgsql;

namespace CRM
{
    public partial class EmailPage : Page
    {
        private List<MailClients> clients = new List<MailClients>();

        public EmailPage()
        {
            InitializeComponent();
            LoadClients(); // Загружаем клиентов при инициализации страницы
        }

        private void LoadClients()
        {
            try
            {
                string connectionString = "Host=rc1a-pqvpr43vs961p74p.mdb.yandexcloud.net;Port=6432;Username=user;Password=postgres;Database=crm;";

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT client_id, first_name, second_name, email FROM clients";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var client = new MailClients
                                {
                                    ClientId = reader.GetInt32(0),
                                    FirstName = reader.GetString(1),
                                    SecondName = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    IsSelected = false // По умолчанию не выбран
                                };
                                clients.Add(client);
                            }
                        }
                    }
                }

                ClientsDataGrid.ItemsSource = clients;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке клиентов: " + ex.Message);
            }
        }

        private void SendEmailsButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedClients = clients.Where(c => c.IsSelected).ToList();

            if (selectedClients.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одного клиента!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(SubjectTextBox.Text))
            {
                MessageBox.Show("Введите тему письма!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(BodyTextBox.Text))
            {
                MessageBox.Show("Введите текст письма!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Настройки SMTP (пример для Gmail)
                var smtpClient = new SmtpClient("smtp.yandex.ru")
                {
                    Port = 25,
                    Credentials = new NetworkCredential("Cuigogog@yandex.ru", "hngwgxcvrtguvaxx"),
                    EnableSsl = true,
                };

                foreach (var client in selectedClients)
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("Cuigogog@yandex.ru"),
                        Subject = SubjectTextBox.Text,
                        Body = BodyTextBox.Text,
                        IsBodyHtml = false,
                    };
                    mailMessage.To.Add(client.Email);

                    smtpClient.Send(mailMessage);
                }

                MessageBox.Show("Письма успешно отправлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при отправке писем: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}