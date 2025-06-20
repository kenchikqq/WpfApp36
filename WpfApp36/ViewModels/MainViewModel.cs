using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using WpfApp36.Models;
using WpfApp36.Infrastructure;

namespace WpfApp36.ViewModels
{
   
    public class MainViewModel : BaseViewModel
    {
        private ObservableCollection<ToDo> _toDoList = new ObservableCollection<ToDo>();
        public ObservableCollection<ToDo> ToDoList
        {
            get => _toDoList;
            set
            {
                _toDoList = value;
                OnPropertyChanged();
            }
        }

        private ToDo _selectedToDo;
        public ToDo SelectedToDo
        {
            get => _selectedToDo;
            set
            {
                _selectedToDo = value;
                OnPropertyChanged();
            }
        }

        private double _progressBarWidth;
        public double ProgressBarWidth
        {
            get => _progressBarWidth;
            set
            {
                _progressBarWidth = value;
                OnPropertyChanged();
            }
        }

        private string _progressTextBlock;
        public string ProgressTextBlock
        {
            get => _progressTextBlock;
            set
            {
                _progressTextBlock = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddToDoCommand { get; }
        public ICommand SaveToDoCommand { get; }
        public ICommand DeleteToDoCommand { get; }
        
        public MainViewModel()
        {
            LoadFromJson();
            ToDoList.CollectionChanged += ToDoList_CollectionChanged;

            AddToDoCommand = new RelayCommand(ExecuteAddToDoCommand);
            SaveToDoCommand = new RelayCommand(ExecuteSaveToDoCommand, CanExecuteSaveToDoCommand);
            DeleteToDoCommand = new RelayCommand(ExecuteDeleteToDoCommand, CanExecuteDeleteToDoCommand);

            UpdateProgress();
        }

        private void ToDoList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ToDo item in e.OldItems)
                {
                    item.PropertyChanged -= ToDoItem_PropertyChanged;
                }
            }
            UpdateProgress();
            SaveToJson();
        }

        private void ToDoItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ToDo.Doing))
            {
                UpdateProgress();
                SaveToJson();
            }
        }

        private void ExecuteAddToDoCommand(object obj)
        {
            var addWindow = new AddToDoWindow();
            addWindow.Owner = Application.Current.MainWindow;
            if (addWindow.ShowDialog() == true)
            {
                var newToDo = new ToDo
                {
                    Title = addWindow.ToDoTitle,
                    DueDate = addWindow.ToDoDate ?? DateTime.Now,
                    Description = string.IsNullOrWhiteSpace(addWindow.ToDoDescription) ? "Нет описания" : addWindow.ToDoDescription,
                    Doing = false
                };
                newToDo.PropertyChanged += ToDoItem_PropertyChanged;
                ToDoList.Add(newToDo);
            }
        }

        private void ExecuteSaveToDoCommand(object obj)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JSON файлы (*.json)|*.json",
                DefaultExt = "json",
                FileName = "todolist.json",
                Title = "Сохранить список дел"
            };

            if (dialog.ShowDialog() == true)
            {
                WriteToJsonFile(dialog.FileName, ToDoList);
                MessageBox.Show($"Список дел успешно сохранён в {dialog.FileName}!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool CanExecuteSaveToDoCommand(object obj)
        {
            return ToDoList.Any();
        }

        private void ExecuteDeleteToDoCommand(object parameter)
        {
            if (parameter is ToDo todo)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить дело '{todo.Title}'?", "Удаление дела", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    ToDoList.Remove(todo);
                }
            }
        }

        private bool CanExecuteDeleteToDoCommand(object parameter)
        {
            return parameter is ToDo;
        }

        private void UpdateProgress()
        {
            int total = ToDoList.Count;
            int completed = ToDoList.Count(x => x.Doing);

            if (total > 0)
            {
                ProgressBarWidth = (completed * 200.0) / total;
            }
            else
            {
                ProgressBarWidth = 0;
            }
            ProgressTextBlock = $"{completed}/{total}";
        }

        private void SaveToJson()
        {
            WriteToJsonFile("Files/todolist.json", ToDoList);
        }

        private void LoadFromJson()
        {
            var list = ReadFromJsonFile<ObservableCollection<ToDo>>("Files/todolist.json");
            if (list != null)
            {
                ToDoList = list;
                foreach (var item in ToDoList)
                {
                    item.PropertyChanged += ToDoItem_PropertyChanged;
                }
            }
        }

        private void WriteToJsonFile<T>(string path, T data)
        {
            try
            {
                string dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
                string json = JsonSerializer.Serialize(data, options);
                File.WriteAllText(path, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private T ReadFromJsonFile<T>(string path) where T : class
        {
            try
            {
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path, Encoding.UTF8);
                    return JsonSerializer.Deserialize<T>(json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }
    }
} 