﻿using FundMon.Repository;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;

namespace FundMon.Controls;

public sealed partial class LineChart : UserControl
{
    #region PrivateMembers
    private double _minVal;
    private double _maxVal;
    private double _verticalAxisMinVal;
    private double _verticalAxisMaxVal;
    private double _verticalScale;
    private int[] _verticalTicks;
    private double _verticalAxisTextWidth;
    private Line[] _verticalAxisTickMarks;
    private TextBlock[] _verticalLabelTextBoxes;
    private long _firstTicks;
    private double _horizontalScale;
    private double _horizontalAxisTextHeight;
    private string _verticalAxisTextFormat;
    private string _horizontalAxisTextFormat;
    private Line[] _horizontalAxisTickMarks;
    private double _halfDateLabelWidth;
    private TextBlock[] _horizontalLabelTextBoxes;
    private List<DateValue> _averageValues;
    private double _selectedXPosition;
    private DateValue _selectedDataValue;
    private bool _isDown = false;
    #endregion

    #region DependencyProperties
    public Brush ChartBackground
    {
        get => (Brush)GetValue(ChartBackgroundProperty);
        set
        {
            SetValue(ChartBackgroundProperty, value);
            GenerateChart();
        }
    }

    public static readonly DependencyProperty ChartBackgroundProperty =
        DependencyProperty.Register(
            nameof(ChartBackground),
            typeof(Brush),
            typeof(LineChart),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 3, 3, 3))));

    public Brush LegendBackground
    {
        get => (Brush)GetValue(LegendBackgroundProperty);
        set { SetValue(LegendBackgroundProperty, value); }
    }

    public static readonly DependencyProperty LegendBackgroundProperty =
        DependencyProperty.Register(
            nameof(LegendBackground),
            typeof(Brush),
            typeof(LineChart),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 16, 16, 16))));

    public List<DateValue> Values
    {
        get => (List<DateValue>)GetValue(ValuesProperty);
        set
        {
            SetValue(ValuesProperty, value);
            GenerateChart();
        }
    }

    public static readonly DependencyProperty ValuesProperty =
        DependencyProperty.Register(
            nameof(Values),
            typeof(List<DateValue>),
            typeof(LineChart),
            new PropertyMetadata(new List<DateValue>()));

    public Brush Stroke
    {
        get => (Brush)GetValue(StrokeProperty);
        set
        {
            SetValue(StrokeProperty, value);
            GenerateChart();
        }
    }

    public static readonly DependencyProperty StrokeProperty =
        DependencyProperty.Register(
            nameof(Stroke),
            typeof(Brush),
            typeof(LineChart),
            new PropertyMetadata(Application.Current.Resources["ButtonBorderThemeBrush"] as Brush));

    public Brush AverageStroke
    {
        get => (Brush)GetValue(AverageStrokeProperty);
        set
        {
            SetValue(AverageStrokeProperty, value);
            GenerateChart();
        }
    }

    public static readonly DependencyProperty AverageStrokeProperty =
        DependencyProperty.Register(
            nameof(AverageStroke),
            typeof(Brush),
            typeof(LineChart),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255))));

    public double StrokeThickness
    {
        get => (double)GetValue(StrokeThickProperty);
        set
        {
            SetValue(StrokeThickProperty, value);
            GenerateChart();
        }
    }

    public static readonly DependencyProperty StrokeThickProperty =
        DependencyProperty.Register(
            nameof(StrokeThickness),
            typeof(double),
            typeof(LineChart),
            new PropertyMetadata(1.0));

    public int ChartPadding
    {
        get => (int)GetValue(ChartPaddingProperty);
        set
        {
            SetValue(ChartPaddingProperty, value);
            GenerateChart();
        }
    }

    public static readonly DependencyProperty ChartPaddingProperty =
        DependencyProperty.Register(
            nameof(ChartPadding),
            typeof(int),
            typeof(LineChart), new PropertyMetadata(6));

    public Brush AxisStroke
    {
        get => (Brush)GetValue(AxisStrokeProperty);
        set
        {
            SetValue(AxisStrokeProperty, value);
            GenerateChart();
        }
    }

    public static readonly DependencyProperty AxisStrokeProperty =
        DependencyProperty.Register(
            nameof(AxisStroke),
            typeof(Brush),
            typeof(LineChart),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 200, 200, 200))));

    public int AxisTextSize
    {
        get => (int)GetValue(AxisTextSizeProperty);
        set
        {
            SetValue(AxisTextSizeProperty, value);
            GenerateChart();
        }
    }

    public static readonly DependencyProperty AxisTextSizeProperty =
        DependencyProperty.Register(nameof(AxisTextSize), typeof(int), typeof(LineChart), new PropertyMetadata(12));

    public int InnerMargin
    {
        get => (int)GetValue(InnerMarginProperty);
        set
        {
            SetValue(InnerMarginProperty, value);
            GenerateChart();
        }
    }

    public static readonly DependencyProperty InnerMarginProperty =
        DependencyProperty.Register(nameof(InnerMargin), typeof(int), typeof(LineChart), new PropertyMetadata(3));

    public bool IsAverageEnabled
    {
        get => (bool)GetValue(IsAverageEnabledProperty);
        set
        {
            SetValue(IsAverageEnabledProperty, value);
            GenerateChart();
        }
    }

    public static readonly DependencyProperty IsAverageEnabledProperty =
        DependencyProperty.Register(nameof(IsAverageEnabled), typeof(bool), typeof(LineChart), new PropertyMetadata(false));

    public int AverageCount
    {
        get => (int)GetValue(AverageCountProperty);
        set
        {
            SetValue(AverageCountProperty, value);
            GenerateChart();
        }
    }

    public static readonly DependencyProperty AverageCountProperty =
        DependencyProperty.Register(nameof(AverageCount), typeof(int), typeof(LineChart), new PropertyMetadata(5));

    public bool IsZoomEnabled
    {
        get =>  (bool)GetValue(IsZoomEnabledProperty);
        set { 
            SetValue(IsZoomEnabledProperty, value);
            CheckZoomState();
        }
    }

    public static readonly DependencyProperty IsZoomEnabledProperty =
        DependencyProperty.Register("IsZoomEnabled", typeof(bool), typeof(LineChart), new PropertyMetadata(false));

    #endregion

    #region Constructor
    public LineChart()
    {
        InitializeComponent();
        _verticalLabelTextBoxes = System.Array.Empty<TextBlock>();
        _verticalAxisTickMarks = System.Array.Empty<Line>();
        _horizontalAxisTickMarks = System.Array.Empty<Line>();
        _horizontalLabelTextBoxes = System.Array.Empty<TextBlock>();
    }
    #endregion

    #region PrivateMethods
    private void GenerateChart()
    {
        if (Values.Count == 0)
            return;

        GetMinMaxVal();
        CalculateVerticalBoundaries();
        CalculateVerticalAxisWidth();
        CalculateHorizontalAxisHeight();
        CalculateVerticalScale();
        CalculateVerticalTicks();
        CalculateHorizontalScale();
        DrawBackground();
        DrawAxes();
        DrawVerticalTicks();
        DrawVerticalLabels();
        DrawHorizontalTicks();
        DrawHorizontalLabels();
        DrawPolyline();
        if (IsAverageEnabled)
        {
            AverageLine.Visibility = Visibility.Visible;
            DrawAveragePolyline();
        }
        else
        {
            AverageLine.Visibility = Visibility.Collapsed;
        }
    }

    private void GetMinMaxVal()
    {
        _minVal = Values.Min(d => d.Value);
        _maxVal = Values.Max(d => d.Value);
    }
    private void CalculateVerticalBoundaries()
    {
        if (_maxVal <= 0)
            return;

        _verticalAxisMinVal = 0;

        double exponent = Math.Floor(Math.Log10(_maxVal));
        double fraction = _maxVal / Math.Pow(10, exponent);
        double niceFraction;

        if (fraction <= 1)
            niceFraction = 1;
        else if (fraction <= 2)
            niceFraction = 2;
        else if (fraction <= 2.5)
            niceFraction = 2.5;
        else if (fraction <= 5)
            niceFraction = 5;
        else if (fraction <= 7.5)
            niceFraction = 7.5;
        else
            niceFraction = 10;

        _verticalAxisMaxVal = niceFraction * Math.Pow(10, exponent);
    }

    private void CalculateVerticalAxisWidth()
    {
        double digits = Math.Floor(Math.Log10(_verticalAxisMaxVal / 10));
        _verticalAxisTextFormat = "{0:" + ((digits > 0) ? "G0" : $"F{-digits}") + "}";
        TextBlock tb = new();
        tb.Text = string.Format(_verticalAxisTextFormat, _verticalAxisMaxVal);
        tb.FontSize = AxisTextSize;
        tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        _verticalAxisTextWidth = tb.DesiredSize.Width;
    }

    private void CalculateHorizontalAxisHeight()
    {
        TextBlock tb = new();
        TimeSpan timeSpan = Values[Values.Count - 1].Date - Values[0].Date;
        _horizontalAxisTextFormat = timeSpan.TotalDays > 365 ? "{0:MM/yy}" : "{0:dd/MM/yy}";
        tb.Text = string.Format("{0:dd/MM/yy}", Values[0].Date);
        tb.FontSize = AxisTextSize;
        tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        _halfDateLabelWidth = tb.DesiredSize.Width / 2;
        _horizontalAxisTextHeight = tb.DesiredSize.Height;
    }

    private void CalculateVerticalScale()
    {

        if (_minVal == _maxVal)
        {
            _verticalScale = 1;
            return;
        }

        _verticalScale = (Box.ActualHeight - (2 * ChartPadding) - (2 * InnerMargin) - _horizontalAxisTextHeight) / (_verticalAxisMaxVal - _verticalAxisMinVal);
    }

    private void CalculateVerticalTicks()
    {
        int ticksCount = ((Box.ActualHeight - 2 * ChartPadding - (2 * InnerMargin) - _horizontalAxisTextHeight) / 50 > 10) ? 11 : 6;

        _verticalTicks = new int[ticksCount];
        for (int i = 0; i < ticksCount; i++)
        {
            _verticalTicks[i] = (int)(i * _verticalAxisMaxVal / (ticksCount - 1) * _verticalScale + ChartPadding);
        }
    }

    private void CalculateHorizontalScale()
    {
        if (Values.Count == 0)
        {
            _horizontalScale = 1.0;
            return;
        }

        _firstTicks = Values[0].Date.Ticks;
        _horizontalScale = (Box.ActualWidth - (2 * ChartPadding) - (2 * InnerMargin) - _verticalAxisTextWidth - _halfDateLabelWidth) / (Values[Values.Count - 1].Date.Ticks - _firstTicks);
    }

    private void DrawBackground()
    {
        PointCollection points = new();
        points.Add(new(ChartPadding, ChartPadding));
        points.Add(new(Box.ActualWidth - ChartPadding, ChartPadding));
        points.Add(new(Box.ActualWidth - ChartPadding, Box.ActualHeight - ChartPadding));
        points.Add(new(ChartPadding, Box.ActualHeight - ChartPadding));

        BackgroundRectangle.Points = points;
    }

    private void DrawPolyline()
    {
        PointCollection points = new();

        foreach (DateValue data in Values)
        {
            points.Add(new Point(
                (int)((data.Date.Ticks - _firstTicks) * _horizontalScale + VerticalAxis.X1),
                (int)((_verticalAxisMaxVal - data.Value) * _verticalScale + ChartPadding)));
        }

        ValueLine.Points = points;
    }

    private void DrawAveragePolyline()
    {
        PointCollection points = new();
        _averageValues = new();
        double mean = 1 / ((double)AverageCount);

        if (Values.Count < AverageCount + 1)
            return;

        double stack = Values[0].Value;
        for (int i = 1; i < AverageCount; i++)
        {
            stack += Values[i].Value;
        }
        _averageValues.Add(new(stack * mean, Values[AverageCount - 1].Date));

        for (int i = AverageCount; i < Values.Count; i++)
        {
            stack += Values[i].Value - Values[i - AverageCount].Value;
            _averageValues.Add(new(stack * mean, Values[i].Date));
        }

        foreach (DateValue data in _averageValues)
        {
            points.Add(new Point(
                (int)((data.Date.Ticks - _firstTicks) * _horizontalScale + VerticalAxis.X1),
                (int)((_verticalAxisMaxVal - data.Value) * _verticalScale + ChartPadding)));
        }

        AverageLine.Points = points;
    }

    private void DrawAxes()
    {
        VerticalAxis.X1 = ChartPadding + 2 * InnerMargin + _verticalAxisTextWidth;
        VerticalAxis.Y1 = ChartPadding;
        VerticalAxis.X2 = VerticalAxis.X1;
        VerticalAxis.Y2 = Box.ActualHeight - ChartPadding - 2 * InnerMargin - _horizontalAxisTextHeight;

        HorizontalAxis.X1 = VerticalAxis.X1;
        HorizontalAxis.Y1 = VerticalAxis.Y2;
        HorizontalAxis.X2 = Box.ActualWidth - ChartPadding - _halfDateLabelWidth;
        HorizontalAxis.Y2 = VerticalAxis.Y2;
    }

    private void DrawVerticalTicks()
    {
        foreach (Line line in _verticalAxisTickMarks)
        {
            _ = Box.Children.Remove(line);
        }

        _verticalAxisTickMarks = new Line[11];
        for (int i = 0; i < _verticalTicks.Length; i++)
        {
            Line line = new();
            line.X1 = VerticalAxis.X1 - InnerMargin;
            line.X2 = VerticalAxis.X1;
            line.Y1 = _verticalTicks[i];
            line.Y2 = _verticalTicks[i];
            line.Stroke = AxisStroke;
            _verticalAxisTickMarks[i] = line;
            Box.Children.Add(line);
        }
    }

    private void DrawVerticalLabels()
    {
        double tick = _verticalAxisMaxVal / (_verticalTicks.Length - 1);
        foreach (TextBlock tb in _verticalLabelTextBoxes)
        {
            Box.Children.Remove(tb);
        }
        _verticalLabelTextBoxes = new TextBlock[_verticalTicks.Length];
        for (int i = 0; i < _verticalTicks.Length; i++)
        {
            TextBlock tb = new();
            tb.Text = string.Format(_verticalAxisTextFormat, tick * (_verticalTicks.Length - i - 1));
            tb.FontSize = AxisTextSize;
            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Box.Children.Add(tb);
            _verticalLabelTextBoxes[i] = tb;
            Canvas.SetTop(tb, _verticalTicks[i] - _horizontalAxisTextHeight / 2);
            Canvas.SetLeft(tb, ChartPadding + _verticalAxisTextWidth - tb.ActualWidth);
        }
    }

    private void DrawHorizontalTicks()
    {
        double tick = (double)(HorizontalAxis.X2 - HorizontalAxis.X1) / 10;
        foreach (Line line in _horizontalAxisTickMarks)
        {
            _ = Box.Children.Remove(line);
        }
        _horizontalAxisTickMarks = new Line[11];
        for (int i = 0; i < 11; i++)
        {
            Line line = new();
            line.Y1 = HorizontalAxis.Y1;
            line.Y2 = HorizontalAxis.Y1 + InnerMargin;
            line.X1 = HorizontalAxis.X1 + i * tick;
            line.X2 = line.X1;
            line.Stroke = AxisStroke;
            _horizontalAxisTickMarks[i] = line;
            Box.Children.Add(line);
        }
    }

    private void DrawHorizontalLabels()
    {
        long tick = (Values[Values.Count - 1].Date.Ticks - Values[0].Date.Ticks) / 10;
        foreach (TextBlock tb in _horizontalLabelTextBoxes)
        {
            _ = Box.Children.Remove(tb);
        }
        _horizontalLabelTextBoxes = new TextBlock[11];
        for (int i = 0; i < 11; i++)
        {
            TextBlock tb = new();
            tb.Text = string.Format(_horizontalAxisTextFormat, new DateTime(tick * i + Values[0].Date.Ticks));
            tb.FontSize = AxisTextSize;
            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Box.Children.Add(tb);
            _horizontalLabelTextBoxes[i] = tb;
            Canvas.SetTop(tb, HorizontalAxis.Y1 + InnerMargin);
            Canvas.SetLeft(tb, _horizontalAxisTickMarks[i].X1 - tb.ActualWidth / 2);
        }

    }

    private void Box_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        Pointer ptr = e.Pointer;

        if (ptr.PointerDeviceType == PointerDeviceType.Mouse)
        {
            PointerPoint point = e.GetCurrentPoint(Box);

            if (point.Position.X > VerticalAxis.X1 && point.Position.Y < HorizontalAxis.Y1)
            {
                PointerLine.Visibility = Visibility.Visible;
                PointerLine.X1 = point.Position.X;
                PointerLine.X2 = point.Position.X;
                PointerLine.Y1 = VerticalAxis.Y1;
                PointerLine.Y2 = VerticalAxis.Y2 - 1;

                PointerLineLegend.Visibility = Visibility.Visible;
                double ticks = (point.Position.X - VerticalAxis.X1) / _horizontalScale + _firstTicks;
                DateValue fundData = FindClosest(ticks);
                LegendText.Text = string.Format("{0:dd/MM/yy}\n{1:F2} €", fundData.Date, fundData.Value);
                PointerLineLegend.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                double x = HorizontalAxis.X2 - point.Position.X < PointerLineLegend.ActualWidth ?
                    point.Position.X - PointerLineLegend.ActualWidth - 5
                    : point.Position.X + 5;
                double y = (_verticalAxisMaxVal - fundData.Value) * _verticalScale + ChartPadding;
                ValueRectangle.Visibility = Visibility.Visible;
                Canvas.SetTop(ValueRectangle, y - 6);
                Canvas.SetLeft(ValueRectangle, (fundData.Date.Ticks - _firstTicks) * _horizontalScale + VerticalAxis.X1 - 6);
                if (y + PointerLineLegend.ActualHeight > HorizontalAxis.Y1)
                    y -= PointerLineLegend.ActualHeight + 5;
                Canvas.SetTop(PointerLineLegend, y);
                Canvas.SetLeft(PointerLineLegend, x);
            }
            else
            {
                PointerLine.Visibility = Visibility.Collapsed;
                PointerLineLegend.Visibility = Visibility.Collapsed;
                ValueRectangle.Visibility = Visibility.Collapsed;
            }

            if (_isDown)
            {
                if (point.Position.X < _selectedXPosition)
                {
                    Canvas.SetLeft(SelectionRectangle, point.Position.X);
                    SelectionRectangle.Width = _selectedXPosition - point.Position.X;
                }
                else
                {
                    Canvas.SetLeft(SelectionRectangle, _selectedXPosition);
                    SelectionRectangle.Width = point.Position.X - _selectedXPosition;
                }
            }
        }

        e.Handled = true;
    }

    private DateValue FindClosest(double ticks)
    {
        int i = 0, j = Values.Count - 1, k;
        double daySpan = TimeSpan.FromDays(1).Ticks;

        while (j - i > 2 && Math.Abs(Values[i].Date.Ticks - ticks) > daySpan)
        {
            k = (i + j) / 2;
            if (Math.Abs(Values[k].Date.Ticks - ticks) < daySpan)
            {
                return Values[k];
            }
            if (Values[k].Date.Ticks > ticks)
                j = k;
            else
                i = k;
        }

        return (Math.Abs(Values[i].Date.Ticks - ticks) <= Math.Abs(Values[j].Date.Ticks - ticks)) ?
            Values[i] : Values[j];
    }

    private void Box_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        GenerateChart();
    }
    #endregion

    private void Box_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        e.Handled = true;

        if (!IsZoomEnabled)
            return;

        PointerPoint point = e.GetCurrentPoint(Box);
        double ticks = (point.Position.X - VerticalAxis.X1) / _horizontalScale + _firstTicks;
        _selectedDataValue = FindClosest(ticks);
        _selectedXPosition = point.Position.X;
        Canvas.SetTop(SelectionRectangle, VerticalAxis.Y2);
        SelectionRectangle.Visibility = Visibility.Visible;
        _isDown = true;
    }

    private void Box_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        e.Handled = true;
        
        _isDown = false;
        SelectionRectangle.Visibility = Visibility.Collapsed;
    }

    private void CheckZoomState()
    {
        if (IsZoomEnabled && _isDown)
        {
            _isDown = false;
            SelectionRectangle.Visibility = Visibility.Collapsed;
        }
    }
}
