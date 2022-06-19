using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;

namespace FundMon.Controls;

public class DoneEventArgs : EventArgs
{
    public bool Escaped { get; set; }
    public DoneEventArgs(bool escaped)
    {
        Escaped = escaped;
    }
}

[INotifyPropertyChanged]
public sealed partial class PortfolioEditModal : UserControl
{
    public delegate void DoneEventHandler(object sender, DoneEventArgs e);

    public event DoneEventHandler Done;

    public Visibility VisualState
    {
        get => (Visibility)GetValue(VisualStateProperty);
        set => SetValue(VisualStateProperty, value);
    }

    public static readonly DependencyProperty VisualStateProperty =
        DependencyProperty.Register(nameof(VisualState), typeof(Visibility), typeof(PortfolioEditModal), new PropertyMetadata(Visibility.Collapsed));

    public string PortfolioName
    {
        get => (string)GetValue(PortfolioNameProperty);
        set
        {
            SetValue(PortfolioNameProperty, value);
            NameTextBox.Text = value;
        }
    }

    public static readonly DependencyProperty PortfolioNameProperty =
        DependencyProperty.Register(nameof(PortfolioName), typeof(string), typeof(PortfolioEditModal), new PropertyMetadata(""));

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set
        {
            SetValue(DescriptionProperty, value);
            DescriptionTextBox.Text = value;
        }
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(PortfolioEditModal), new PropertyMetadata(""));

    public string DoneButtonName
    {
        get => (string)GetValue(DoneButtonNameProperty);
        set { SetValue(DoneButtonNameProperty, value); }
    }

    public static readonly DependencyProperty DoneButtonNameProperty =
        DependencyProperty.Register(nameof(DoneButtonName), typeof(string), typeof(PortfolioEditModal), new PropertyMetadata("Créer"));

    public PortfolioEditModal()
    {
        InitializeComponent();
    }

    private void NameTextBox_TextChanged(object _1, TextChangedEventArgs _2)
    {
        DoneButton.IsEnabled = NameTextBox.Text != "";
    }

    private void RectangleTapped(object _1, TappedRoutedEventArgs _2)
    {
        Escape();
    }

    private void EscapeKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        Escape();
    }
    private void EscapeButton_Click(object sender, RoutedEventArgs e)
    {
        Escape();
    }

    private void Escape()
    {
        VisualState = Visibility.Collapsed;
        Done?.Invoke(this, new DoneEventArgs(true));
    }

    private void DoneButton_Click(object _1, RoutedEventArgs _2)
    {
        if (NameTextBox.Text != "")
        {
            VisualState = Visibility.Collapsed;
            PortfolioName = NameTextBox.Text;
            Description = DescriptionTextBox.Text;
            Done?.Invoke(this, new DoneEventArgs(false));
        }
    }

}
