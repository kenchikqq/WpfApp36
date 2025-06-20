using System;
using System.Windows;

namespace WpfApp36
{
   
    public partial class AddToDoWindow : Window
    {
        
        public string ToDoTitle => titleBox.Text;
        
        public DateTime? ToDoDate => datePicker.SelectedDate;
        
        public string ToDoDescription => descBox.Text;

        public AddToDoWindow()
        {
            InitializeComponent();
            datePicker.SelectedDate = DateTime.Now;
        }

        
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ToDoTitle))
            {
                MessageBox.Show("Введите название дела!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!ToDoDate.HasValue)
            {
                MessageBox.Show("Выберите дату выполнения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
            Close();
        }
    }
}