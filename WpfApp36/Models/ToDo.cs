using System;

namespace WpfApp36.Models
{
    public class ToDo : ViewModels.BaseViewModel
    {
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private DateTime _dueDate;
        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private bool _doing;
        public bool Doing
        {
            get => _doing;
            set
            {
                _doing = value;
                OnPropertyChanged();
            }
        }
    }
}