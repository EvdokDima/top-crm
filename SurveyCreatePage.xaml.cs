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

namespace CRM
{
    /// <summary>
    /// Логика взаимодействия для SurveyCreatePage.xaml
    /// </summary>
    public partial class SurveyCreatePage : Page
    {
        private string connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=crm;";
        private List<Question> questions = new List<Question>();

        public SurveyCreatePage()
        {
            InitializeComponent();
        }

        private void AddQuestion_Click(object sender, RoutedEventArgs e)
        {
            var question = new Question
            {
                QuestionText = "Новый вопрос",
                QuestionType = "single", // По умолчанию "один вариант"
                IsRequired = true,
                QuestionOrder = questions.Count + 1,
                Options = new List<AnswerOption>()
            };

            questions.Add(question);
            RefreshQuestionsList();
        }

        private void RefreshQuestionsList()
        {
            QuestionsStackPanel.Children.Clear();

            for (int i = 0; i < questions.Count; i++)
            {
                var question = questions[i];
                var questionIndex = i; // Для замыкания в событиях

                // Основная панель вопроса
                var questionBorder = new Border
                {
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(0, 5, 0, 5),
                    Padding = new Thickness(10)
                };

                var questionPanel = new StackPanel();

                // Заголовок вопроса
                var headerPanel = new StackPanel { Orientation = Orientation.Horizontal };
                headerPanel.Children.Add(new TextBlock
                {
                    Text = $"Вопрос {question.QuestionOrder}:",
                    FontWeight = FontWeights.Bold,
                    Width = 100
                });

                var textBox = new TextBox
                {
                    Text = question.QuestionText,
                    Width = 300,
                    Margin = new Thickness(5)
                };
                textBox.TextChanged += (s, e) =>
                {
                    questions[questionIndex].QuestionText = textBox.Text;
                };
                headerPanel.Children.Add(textBox);

                // Комбобокс для типа вопроса
                var typeCombo = new ComboBox
                {
                    Width = 150,
                    Margin = new Thickness(5),
                    SelectedIndex = question.QuestionType == "multiple" ? 1 : 0
                };
                typeCombo.Items.Add("Один вариант");
                typeCombo.Items.Add("Несколько вариантов");
                typeCombo.SelectionChanged += (s, e) =>
                {
                    questions[questionIndex].QuestionType =
                        typeCombo.SelectedIndex == 1 ? "multiple" : "single";
                    RefreshQuestionsList();
                };
                headerPanel.Children.Add(typeCombo);

                questionPanel.Children.Add(headerPanel);

                // Варианты ответов (если не текстовый вопрос)
                if (question.QuestionType != "text")
                {
                    var optionsPanel = new StackPanel { Margin = new Thickness(20, 5, 0, 5) };

                    foreach (var option in question.Options)
                    {
                        var optionIndex = question.Options.IndexOf(option);
                        var optionPanel = new StackPanel { Orientation = Orientation.Horizontal };

                        var optionBox = new TextBox
                        {
                            Text = option.OptionText,
                            Width = 250,
                            Margin = new Thickness(5)
                        };
                        optionBox.TextChanged += (s, e) =>
                        {
                            questions[questionIndex].Options[optionIndex].OptionText = optionBox.Text;
                        };

                        var deleteBtn = new Button
                        {
                            Content = "Удалить",
                            Margin = new Thickness(5),
                            Tag = new { QIndex = questionIndex, OIndex = optionIndex }
                        };
                        deleteBtn.Click += DeleteOption_Click;

                        optionPanel.Children.Add(optionBox);
                        optionPanel.Children.Add(deleteBtn);
                        optionsPanel.Children.Add(optionPanel);
                    }

                    // Панель добавления нового варианта
                    var addOptionPanel = new StackPanel { Orientation = Orientation.Horizontal };
                    var newOptionBox = new TextBox
                    {
                        Width = 250,
                        Margin = new Thickness(5)
                    };
                    var addOptionBtn = new Button
                    {
                        Content = "Добавить вариант",
                        Margin = new Thickness(5),
                        Tag = questionIndex
                    };
                    addOptionBtn.Click += AddOption_Click;

                    addOptionPanel.Children.Add(newOptionBox);
                    addOptionPanel.Children.Add(addOptionBtn);
                    optionsPanel.Children.Add(addOptionPanel);

                    questionPanel.Children.Add(optionsPanel);
                }

                // Кнопка удаления вопроса
                var deleteQuestionBtn = new Button
                {
                    Content = "Удалить вопрос",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(5),
                    Tag = questionIndex
                };
                deleteQuestionBtn.Click += DeleteQuestion_Click;

                questionPanel.Children.Add(deleteQuestionBtn);
                questionBorder.Child = questionPanel;
                QuestionsStackPanel.Children.Add(questionBorder);
            }
        }

        private void AddOption_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int questionIndex = (int)btn.Tag;

            questions[questionIndex].Options.Add(new AnswerOption
            {
                OptionText = "Новый вариант",
                OptionOrder = questions[questionIndex].Options.Count + 1
            });

            RefreshQuestionsList();
        }

        private void DeleteOption_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var indices = (dynamic)btn.Tag;
            int qIndex = indices.QIndex;
            int oIndex = indices.OIndex;

            questions[qIndex].Options.RemoveAt(oIndex);
            RefreshQuestionsList();
        }

        private void DeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int questionIndex = (int)btn.Tag;

            questions.RemoveAt(questionIndex);
            // Обновляем порядковые номера
            for (int i = 0; i < questions.Count; i++)
            {
                questions[i].QuestionOrder = i + 1;
            }
            RefreshQuestionsList();
        }

        private void SaveSurvey_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Введите название опроса");
                return;
            }

            if (questions.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один вопрос");
                return;
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Сохраняем опрос
                        var surveyId = SaveSurveyToDb(conn, transaction);

                        // Сохраняем вопросы
                        SaveQuestionsToDb(conn, transaction, surveyId);

                        transaction.Commit();
                        MessageBox.Show("Опрос успешно сохранен");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка сохранения: {ex.Message}");
                    }
                }
            }
        }

        private int SaveSurveyToDb(NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            var cmd = new NpgsqlCommand(
                "INSERT INTO surveys (title, description, is_active) VALUES (@title, @desc, @active) RETURNING survey_id",
                conn, transaction);

            cmd.Parameters.AddWithValue("@title", TitleTextBox.Text);
            cmd.Parameters.AddWithValue("@desc",
                string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? DBNull.Value : (object)DescriptionTextBox.Text);
            cmd.Parameters.AddWithValue("@active", IsActiveCheckBox.IsChecked ?? true);

            return (int)cmd.ExecuteScalar();
        }

        private void SaveQuestionsToDb(NpgsqlConnection conn, NpgsqlTransaction transaction, int surveyId)
        {
            foreach (var question in questions)
            {
                var cmd = new NpgsqlCommand(
                    "INSERT INTO questions (survey_id, question_text, question_type, is_required, question_order) " +
                    "VALUES (@surveyId, @text, @type, @required, @order) RETURNING question_id",
                    conn, transaction);

                cmd.Parameters.AddWithValue("@surveyId", surveyId);
                cmd.Parameters.AddWithValue("@text", question.QuestionText);
                cmd.Parameters.AddWithValue("@type", question.QuestionType);
                cmd.Parameters.AddWithValue("@required", question.IsRequired);
                cmd.Parameters.AddWithValue("@order", question.QuestionOrder);

                var questionId = (int)cmd.ExecuteScalar();

                if (question.QuestionType != "text")
                {
                    SaveOptionsToDb(conn, transaction, questionId, question.Options);
                }
            }
        }

        private void SaveOptionsToDb(NpgsqlConnection conn, NpgsqlTransaction transaction, int questionId, List<AnswerOption> options)
        {
            foreach (var option in options)
            {
                var cmd = new NpgsqlCommand(
                    "INSERT INTO answer_options (question_id, option_text, option_order) " +
                    "VALUES (@qId, @text, @order)",
                    conn, transaction);

                cmd.Parameters.AddWithValue("@qId", questionId);
                cmd.Parameters.AddWithValue("@text", option.OptionText);
                cmd.Parameters.AddWithValue("@order", option.OptionOrder);

                cmd.ExecuteNonQuery();
            }
        }

        private void ListButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SurveysListPage());
        }
    }
}
