using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;
using Npgsql;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Npgsql.Internal;
using Paragraph = iTextSharp.text.Paragraph;

namespace CRM
{
    public partial class SurveysListPage : Page
    {
        private string connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=crm;";

        public SurveysListPage()
        {
            InitializeComponent();
            Loaded += SurveysListPage_Loaded;
        }

        private void SurveysListPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSurveys();
        }

        private void LoadSurveys(string filter = null)
        {
            try
            {
                var surveys = new List<Survey>();
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT s.survey_id as SurveyId, s.title as Title, s.created_at as CreatedAt, 
                       s.is_active as IsActive,
                       (SELECT COUNT(*) FROM questions q WHERE q.survey_id = s.survey_id) as QuestionsCount,
                       (SELECT COUNT(*) FROM responses r WHERE r.survey_id = s.survey_id) as ResponsesCount
                FROM surveys s";

                    if (!string.IsNullOrEmpty(filter))
                    {
                        query += $" WHERE {filter}";
                    }

                    query += " ORDER BY s.created_at DESC";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            surveys.Add(new Survey
                            {
                                SurveyId = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                CreatedAt = reader.GetDateTime(2),
                                IsActive = reader.GetBoolean(3)
                            });
                        }
                    }
                }

                SurveysDataGrid.ItemsSource = surveys;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки опросов: {ex.Message}");
            }
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            string filter = null;
            switch (StatusFilterComboBox.SelectedIndex)
            {
                case 1: filter = "s.is_active = true"; break;
                case 2: filter = "s.is_active = false"; break;
            }
            LoadSurveys(filter);
        }

        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            StatusFilterComboBox.SelectedIndex = 0;
            LoadSurveys();
        }

        private void ViewSurvey_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is int surveyId)
            {
                MessageBox.Show($"Просмотр опроса ID: {surveyId}");
                // NavigationService.Navigate(new SurveyViewPage(surveyId));
            }
        }


    private void ViewReport_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is int surveyId)
        {
            try
            {
                // Получаем данные опроса из БД
                var surveyData = GetSurveyData(surveyId);

                // Генерируем PDF
                string filePath = GeneratePdfReport(surveyId, surveyData);

                // Открываем PDF
                Process.Start(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчёта: {ex.Message}");
            }
        }
    }

    private DataTable GetSurveyData(int surveyId)
    {
            string query = @"
                SELECT 
                    s.title AS survey_title,
                    q.question_id,
                    q.question_text,
                    a.answer_text,
                    COUNT(a.answer_id) OVER (PARTITION BY q.question_id) AS total_answers
                FROM answers a
                JOIN questions q ON a.question_id = q.question_id
                JOIN surveys s ON q.survey_id = s.survey_id
                WHERE s.survey_id = @surveyId
                ORDER BY q.question_id";

            using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@surveyId", surveyId);
                var adapter = new NpgsqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
    }

    private string GeneratePdfReport(int surveyId, DataTable surveyData)
    {
            // Путь для сохранения
        string filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            $"SimpleSurveyReport_{surveyId}_{DateTime.Now:yyyyMMddHHmmss}.pdf");

        using (var fs = new FileStream(filePath, FileMode.Create))
        {
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);

            document.Open();

            // Шрифты с поддержкой русского
            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
            BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var titleFont = new Font(baseFont, 18, Font.BOLD);
            var normalFont = new Font(baseFont, 10);

            // Заголовок
            string surveyTitle = surveyData.Rows.Count > 0
                ? surveyData.Rows[0]["survey_title"].ToString()
                : $"Опрос #{surveyId}";

            document.Add(new Paragraph(surveyTitle, titleFont));
            document.Add(new Paragraph($"Дата генерации: {DateTime.Now:dd.MM.yyyy HH:mm}", normalFont));
            document.Add(Chunk.NEWLINE);

            // Простой список ответов
            document.Add(new Paragraph("Все полученные ответы:", normalFont));
            document.Add(Chunk.NEWLINE);

            int counter = 1;
            foreach (DataRow row in surveyData.Rows)
            {
                string questionText = row["question_text"].ToString();
                string answerText = row["answer_text"].ToString();

                document.Add(new Paragraph($"{counter}. Вопрос: {questionText}", normalFont));
                document.Add(new Paragraph($"   Ответ: {answerText}", normalFont));
                document.Add(Chunk.NEWLINE);

                counter++;
            }

            document.Close();
        }

        return filePath;
    }

    private void SendSurvey_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is int surveyId)
            {
                var row = ((Button)sender).DataContext as DataRowView;
                string title = row?["title"]?.ToString() ?? "Без названия";
                NavigationService.Navigate(new SurveySendPage(surveyId, title));
            }
        }

        private void CreateNewSurvey_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SurveyCreatePage());
        }

        private void RefreshList_Click(object sender, RoutedEventArgs e)
        {
            LoadSurveys();
        }
    }
}