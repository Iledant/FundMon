using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FundMon.ViewModel;

public abstract class Bindable : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
