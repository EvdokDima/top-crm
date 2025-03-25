using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using Npgsql;

namespace CRM
{
    public partial class AnalyticPage : Page
    {
        public SeriesCollection SeriesCollection { get; set; }

        public AnalyticPage()
        {
            InitializeComponent();

            // Загружаем данные для диаграммы
            LoadData();

            // Устанавливаем DataContext для привязки данных
            DataContext = this;
        }

        private void LoadData()
        {
            // Получаем данные из базы данных
            var clientTotals = GetClientTotalsFromDatabase();

            // Создаем коллекцию для диаграммы
            SeriesCollection = new SeriesCollection();

            foreach (var clientTotal in clientTotals)
            {
                SeriesCollection.Add(new PieSeries
                {
                    Title = $"Клиент {clientTotal.ClientId}",
                    Values = new ChartValues<double> { clientTotal.TotalSum },
                    DataLabels = true
                });
            }
        }

        private List<ClientTotal> GetClientTotalsFromDatabase()
        {
            var clientTotals = new List<ClientTotal>();

           string connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=crm;";

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT client_id, total
                        FROM clients
                        GROUP BY client_id";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var clientTotal = new ClientTotal
                                {
                                    ClientId = reader.GetInt32(0),
                                    TotalSum = reader.GetDouble(1)
                                };
                                clientTotals.Add(clientTotal);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }

            return clientTotals;
        }
    }
}