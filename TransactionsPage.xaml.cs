using Npgsql;
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
using System.Windows.Threading;

namespace CRM
{
    /// <summary>
    /// Логика взаимодействия для TransactionsPage.xaml
    /// </summary>
    public partial class TransactionsPage : Page
    {
        private string connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=crm;";

        public TransactionsPage()
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

        private bool ValidateTransaction(Transaction transaction)
        {
            if (transaction.ClientId <= 0)
            {
                MessageBox.Show("ID клиента должен быть больше 0!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (transaction.Cost <= 0)
            {
                MessageBox.Show("Сумма транзакции должна быть больше 0!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (transaction.TranzactionTime > DateTime.Now)
            {
                MessageBox.Show("Время транзакции не может быть в будущем!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void AddTransactionToDatabase(Transaction transaction)
        {
            if (!ValidateTransaction(transaction)) return;

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var transactionScope = conn.BeginTransaction())
                    {
                        string query = @"
                        INSERT INTO tranzactions (client_id, tranzaction_time, cost)
                        VALUES (@clientId, @tranzactionTime, @cost)
                        RETURNING tranzaction_id";

                        using (var cmd = new NpgsqlCommand(query, conn, transactionScope))
                        {
                            cmd.Parameters.AddWithValue("@clientId", transaction.ClientId);
                            cmd.Parameters.AddWithValue("@tranzactionTime", transaction.TranzactionTime);
                            cmd.Parameters.AddWithValue("@cost", transaction.Cost);

                            transaction.TranzactionId = (int)cmd.ExecuteScalar();
                        }

                        transactionScope.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении транзакции: " + ex.Message);
            }
        }

        private void UpdateTransactionInDatabase(Transaction transaction)
        {
            if (!ValidateTransaction(transaction)) return;

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                    UPDATE tranzactions
                    SET client_id = @clientId,
                        tranzaction_time = @tranzactionTime,
                        cost = @cost
                    WHERE tranzaction_id = @tranzactionId";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@clientId", transaction.ClientId);
                        cmd.Parameters.AddWithValue("@tranzactionTime", transaction.TranzactionTime);
                        cmd.Parameters.AddWithValue("@cost", transaction.Cost);
                        cmd.Parameters.AddWithValue("@tranzactionId", transaction.TranzactionId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении транзакции: " + ex.Message);
            }
        }

        private void LoadData()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                    SELECT t.tranzaction_id, t.client_id, t.tranzaction_time, t.cost, 
                           c.first_name, c.second_name, c.surename
                    FROM tranzactions t
                    JOIN clients c ON t.client_id = c.client_id";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            var transactions = new List<Transaction>();

                            while (reader.Read())
                            {
                                var transaction = new Transaction
                                {
                                    TranzactionId = reader.GetInt32(0),
                                    ClientId = reader.GetInt32(1),
                                    TranzactionTime = reader.GetDateTime(2),
                                    Cost = reader.GetDecimal(3),
                                    ClientFullName = $"{reader.GetString(4)} {reader.GetString(5)} {reader.GetString(6)}" // ФИО клиента
                                };
                                transactions.Add(transaction);
                            }

                            TransactionsDataGrid.ItemsSource = transactions;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void TransactionsDataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new Transaction
            {
                ClientId = 0,
                TranzactionTime = DateTime.Now,
                Cost = 0
            };
        }

        private void TransactionsDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                TransactionsDataGrid.Dispatcher.InvokeAsync(() =>
                {
                    var editedTransaction = e.Row.Item as Transaction;

                    if (editedTransaction != null)
                    {
                        if (editedTransaction.TranzactionId == 0)
                        {
                            AddTransactionToDatabase(editedTransaction);
                        }
                        else 
                        {
                            UpdateTransactionInDatabase(editedTransaction);
                        }

                        LoadData();
                    }
                }, DispatcherPriority.Background);
            }
        }

        private void TransactionsDataGrid_AddingNewItem_1(object sender, AddingNewItemEventArgs e)
        {

        }
    }
}
