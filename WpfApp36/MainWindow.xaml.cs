using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using System.IO;
using WpfApp36;
using System.Text;
using Microsoft.Win32;

namespace WpfApp36
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<ToDo> ToDoList { get; set; } = new ObservableCollection<ToDo>();

        public MainWindow()
        {
            InitializeComponent();
            todoList.ItemsSource = ToDoList;
            ToDoList.CollectionChanged += (s, e) => SaveToJson();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFromJson();
            UpdateProgress();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            SaveToJson();
        }

        private void SaveToJson()
        {
            try
            {
                string dir = Path.Combine(Directory.GetCurrentDirectory(), "Files");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string path = Path.Combine(dir, "test.json");
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(ToDoList, options);
                File.WriteAllText(path, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadFromJson()
        {
            try
            {
                string dir = Path.Combine(Directory.GetCurrentDirectory(), "Files");
                string path = Path.Combine(dir, "test.json");
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path, Encoding.UTF8);
                    var list = JsonSerializer.Deserialize<ObservableCollection<ToDo>>(json);
                    if (list != null)
                    {
                        ToDoList.Clear();
                        foreach (var item in list)
                            ToDoList.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var addWindow = new AddToDoWindow();
            addWindow.Owner = this;
            if (addWindow.ShowDialog() == true)
            {
                ToDoList.Add(new ToDo
                {
                    Title = addWindow.ToDoTitle,
                    DueDate = addWindow.ToDoDate ?? DateTime.Now,
                    Description = string.IsNullOrWhiteSpace(addWindow.ToDoDescription) ? "Нет описания" : addWindow.ToDoDescription,
                    Doing = false
                });
                UpdateProgress();
            }
        }

        private void SaveCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (ToDoList.Count == 0)
            {
                MessageBox.Show("В списке нет дел", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var dialog = new SaveFileDialog
            {
                Filter = "JSON файлы (*.json)|*.json",
                DefaultExt = "json",
                FileName = "todo.json",
                Title = "Сохранить список дел в JSON"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string json = JsonSerializer.Serialize(ToDoList, options);
                    File.WriteAllText(dialog.FileName, json, Encoding.UTF8);
                    MessageBox.Show($"Список дел успешно сохранён в {dialog.FileName}!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ToDo todo)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить дело?", "Удаление дела", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    ToDoList.Remove(todo);
                    UpdateProgress();
                }
            }
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.DataContext is ToDo todo)
            {
                todo.Doing = cb.IsChecked == true;
                UpdateProgress();
            }
        }

        private void UpdateProgress()
        {
            int total = ToDoList.Count;
            int completed = ToDoList.Count(x => x.Doing);

            // Кастомный прогресс-бар
            if (total > 0)
            {
                progressBarFill.Width = (completed * 200.0) / total;
            }
            else
            {
                progressBarFill.Width = 0;
            }

            progressTextBlock.Text = $"{completed}/{total}";
        }
    }
}