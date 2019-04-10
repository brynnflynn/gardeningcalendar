using GoogleCalendarGardeningGenerator.Host.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GoogleCalendarGardeningGenerator.Host
{
    public class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}