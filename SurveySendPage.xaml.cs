using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using Npgsql;

namespace CRM
{
    public partial class SurveySendPage : Page
    {
        private string connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=crm;";
        private int surveyId;
        private string surveyTitle;

        public class ClientForSurvey
        {
            public int ClientId { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public bool IsSelected { get; set; }
        }

        public SurveySendPage(int surveyId, string surveyTitle)
        {
            InitializeComponent();
            this.surveyId = surveyId;
            this.surveyTitle = surveyTitle;
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    var cmd = new NpgsqlCommand(
                        "SELECT client_id, first_name, second_name, surename, email " +
                        "FROM clients WHERE email IS NOT NULL AND email != ''", conn);

                    var adapter = new NpgsqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    var clients = new List<ClientForSurvey>();
                    foreach (DataRow row in dt.Rows)
                    {
                        clients.Add(new ClientForSurvey
                        {
                            ClientId = (int)row["client_id"],
                            FullName = $"{row["first_name"]} {row["second_name"]} {row["surename"]}",
                            Email = row["email"].ToString(),
                            IsSelected = false
                        });
                    }

                    ClientsDataGrid.ItemsSource = clients;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedClients = ((IEnumerable<ClientForSurvey>)ClientsDataGrid.ItemsSource)
                .Where(c => c.IsSelected).ToList();

            if (selectedClients.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одного клиента");
                return;
            }

            if (string.IsNullOrWhiteSpace(SubjectTextBox.Text))
            {
                MessageBox.Show("Введите тему письма");
                return;
            }

            if (string.IsNullOrWhiteSpace(BodyTextBox.Text))
            {
                MessageBox.Show("Введите текст письма");
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

                string surveyLink = $"http://localhost:3000/survey.html?id={surveyId}";
                int successCount = 0;

                foreach (var client in selectedClients)
                {
                    try
                    {
                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress("Cuigogog@yandex.ru"),
                            Subject = SubjectTextBox.Text,
                            Body = BodyTextBox.Text
                                .Replace("{name}", client.FullName)
                                .Replace("{link}", surveyLink),
                            IsBodyHtml = false
                        };
                        mailMessage.To.Add(client.Email);

                        smtpClient.Send(mailMessage);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка отправки для {client.Email}: {ex.Message}");
                    }
                }

                MessageBox.Show($"Письма успешно отправлены {successCount} из {selectedClients.Count} клиентов");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке писем: {ex.Message}");
            }
        }
    }
}