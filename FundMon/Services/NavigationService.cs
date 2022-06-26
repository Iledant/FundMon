using Microsoft.UI.Xaml.Controls;
using System;

namespace FundMon.Services;

public interface INavigationService
{
    public void GoBack();
    public void GoForward();
    public bool Navigate(Type source, object parameter = null);

    public void SetNavigationFrame(Frame frame);
}

public class NavigationService : INavigationService
{
    private Frame frame = null;

    public void GoBack() => frame.GoBack();

    public void GoForward() => frame.GoForward();

    public bool Navigate(Type source, object parameter = null) => frame.Navigate(source, parameter);

    public void SetNavigationFrame(Frame frame)
    {
        if (this.frame is null)
            this.frame = frame;
    }
}