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
            if (string.IsNullOrWhiteSpace(ToDoTitle) || !ToDoDate.HasValue)
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }
            DialogResult = true;
            Close();
        }
    }
}