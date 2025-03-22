using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Npgsql;

namespace CRM
{
    public partial class ClientsPage : Page
    {
        private string connectionString = "Host=rc1a-pqvpr43vs961p74p.mdb.yandexcloud.net;Port=6432;Username=user;Password=postgres;Database=crm;";

        public ClientsPage()
        {
            InitializeComponent();

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                }
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
            }
        }

        private bool ValidateClient(Client client)
        {
            if (string.IsNullOrWhiteSpace(client.FirstName) ||
                string.IsNullOrWhiteSpace(client.SecondName) ||
                string.IsNullOrWhiteSpace(client.Surename))
            {
                MessageBox.Show("Фамилия, имя и отчество не могут быть пустыми!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!Regex.IsMatch(client.PhoneNumber, @"^\d{10,15}$"))
            {
                MessageBox.Show("Некорректный номер телефона. Введите от 10 до 15 цифр.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!Regex.IsMatch(client.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Некорректный email!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (client.Birthday > DateTime.Now)
            {
                MessageBox.Show("Дата рождения не может быть в будущем!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void AddClientToDatabase(Client client)
        {
            if (!ValidateClient(client)) return;

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string query = @"
                        INSERT INTO clients (first_name, second_name, surename, phone_number, email, birthday, registration, total)
                        VALUES (@firstName, @secondName, @surename, @phoneNumber, @email, @birthday, @registration, @total)
                        RETURNING client_id";

                        using (var cmd = new NpgsqlCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@firstName", client.FirstName);
                            cmd.Parameters.AddWithValue("@secondName", client.SecondName);
                            cmd.Parameters.AddWithValue("@surename", client.Surename);
                            cmd.Parameters.AddWithValue("@phoneNumber", client.PhoneNumber);
                            cmd.Parameters.AddWithValue("@email", client.Email);
                            cmd.Parameters.AddWithValue("@birthday", client.Birthday);
                            client.Registration = DateTime.Now.Date;
                            cmd.Parameters.AddWithValue("@registration", client.Registration);
                            cmd.Parameters.AddWithValue("@total", client.Total);

                            client.ClientId = (int)cmd.ExecuteScalar();
                        }

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении клиента: " + ex.Message);
            }
        }

        private void UpdateClientInDatabase(Client client)
        {
            if (!ValidateClient(client)) return;

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                            UPDATE clients
                            SET first_name = @firstName,
                                second_name = @secondName,
                                surename = @surename,
                                phone_number = @phoneNumber,
                                email = @email,
                                birthday = @birthday,
                                registration = @registration,
                                total = @total
                            WHERE client_id = @clientId";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@firstName", client.FirstName);
                        cmd.Parameters.AddWithValue("@secondName", client.SecondName);
                        cmd.Parameters.AddWithValue("@surename", client.Surename);
                        cmd.Parameters.AddWithValue("@phoneNumber", client.PhoneNumber);
                        cmd.Parameters.AddWithValue("@email", client.Email);
                        cmd.Parameters.AddWithValue("@birthday", client.Birthday);
                        cmd.Parameters.AddWithValue("@registration", client.Registration);
                        cmd.Parameters.AddWithValue("@total", client.Total);
                        cmd.Parameters.AddWithValue("@clientId", client.ClientId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении клиента: " + ex.Message);
            }
        }

        private void LoadData()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT client_id, first_name, second_name, surename, phone_number, email, birthday, registration, total FROM clients";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            var clients = new List<Client>();

                            while (reader.Read())
                            {
                                var client = new Client
                                {
                                    ClientId = reader.GetInt32(0),
                                    FirstName = reader.GetString(1),
                                    SecondName = reader.GetString(2),
                                    Surename = reader.GetString(3),
                                    PhoneNumber = reader.GetString(4),
                                    Email = reader.GetString(5),
                                    Birthday = reader.GetDateTime(6),
                                    Registration = reader.GetDateTime(7),
                                    Total = reader.GetFloat(8)
                                };
                                clients.Add(client);
                            }

                            ClientsDataGrid.ItemsSource = clients;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void ClientsDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                ClientsDataGrid.Dispatcher.InvokeAsync(() =>
                {
                    var editedClient = e.Row.Item as Client;

                    if (editedClient != null)
                    {
                        if (editedClient.ClientId == 0)
                        {
                            AddClientToDatabase(editedClient);
                        }
                        else
                        {
                            UpdateClientInDatabase(editedClient);
                        }

                        LoadData();
                    }
                }, DispatcherPriority.Background);
            }
        }
    }
}
