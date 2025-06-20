using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp36
{
    
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

       
         name="propertyName">
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}