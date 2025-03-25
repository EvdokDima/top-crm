using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CRM
{
    public partial class EventsPage : Page
    {
        private List<Event> events = new List<Event>();
        private string connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=crm;";

        public EventsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            events.Clear();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT event_id, event_date, start_time, end_time, title, description FROM events";

                using (var cmd = new NpgsqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        events.Add(new Event
                        {
                            EventId = reader.GetInt32(0),
                            EventDate = reader.GetDateTime(1),
                            StartTime = reader.GetDateTime(2),
                            EndTime = reader.GetDateTime(3),
                            Title = reader.GetString(4),
                            Description = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
                        });
                    }
                }
            }

            EventsDataGrid.ItemsSource = events;
            EventsCalendar.DataContext = this;
        }

        private void EventsCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDate = EventsCalendar.SelectedDate;
            if (selectedDate.HasValue)
            {
                var filteredEvents = events.Where(ev => ev.EventDate.Date == selectedDate.Value.Date).ToList();
                EventsDataGrid.ItemsSource = filteredEvents;
            }
        }

        private void AddEventButton_Click(object sender, RoutedEventArgs e)
        {
            if (!EventsCalendar.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Введите название мероприятия!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var startTime = GetDateTimeFromPickers(StartDatePicker, StartHourComboBox, StartMinuteComboBox);
            var endTime = GetDateTimeFromPickers(EndDatePicker, EndHourComboBox, EndMinuteComboBox);

            if (startTime >= endTime)
            {
                MessageBox.Show("Время окончания должно быть позже времени начала!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newEvent = new Event
            {
                EventDate = EventsCalendar.SelectedDate.Value,
                StartTime = startTime,
                EndTime = endTime,
                Title = TitleTextBox.Text,
                Description = DescriptionTextBox.Text
            };

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO events (event_date, start_time, end_time, title, description) VALUES (@eventDate, @startTime, @endTime, @title, @description) RETURNING event_id;";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("eventDate", newEvent.EventDate);
                    cmd.Parameters.AddWithValue("startTime", newEvent.StartTime);
                    cmd.Parameters.AddWithValue("endTime", newEvent.EndTime);
                    cmd.Parameters.AddWithValue("title", newEvent.Title);
                    cmd.Parameters.AddWithValue("description", (object)newEvent.Description ?? DBNull.Value);

                    newEvent.EventId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            events.Add(newEvent);
            EventsDataGrid.ItemsSource = events.Where(ev => ev.EventDate.Date == newEvent.EventDate.Date).ToList();

            TitleTextBox.Clear();
            DescriptionTextBox.Clear();
            StartDatePicker.SelectedDate = null;
            StartHourComboBox.SelectedIndex = -1;
            StartMinuteComboBox.SelectedIndex = -1;
            EndDatePicker.SelectedDate = null;
            EndHourComboBox.SelectedIndex = -1;
            EndMinuteComboBox.SelectedIndex = -1;
        }

        private DateTime GetDateTimeFromPickers(DatePicker datePicker, ComboBox hourComboBox, ComboBox minuteComboBox)
        {
            var date = datePicker.SelectedDate ?? DateTime.Now.Date;
            var hour = int.Parse((hourComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "0");
            var minute = int.Parse((minuteComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "0");
            return new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
        }
    }
}
