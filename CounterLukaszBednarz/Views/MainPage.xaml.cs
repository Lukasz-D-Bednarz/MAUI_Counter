using System.Collections.ObjectModel;
using CounterLukaszBednarz.Models;

namespace CounterLukaszBednarz.Views;

public partial class MainPage : ContentPage
{
    private bool isLoaded;

    public ObservableCollection<Counter> Counters { get; } = [];

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (isLoaded)
        {
            return;
        }

        LoadCounters();
        isLoaded = true;
    }

    private void LoadCounters()
    {
        Counters.Clear();

        var filePaths = Directory.GetFiles(FileSystem.AppDataDirectory, "counter-*.txt");
        foreach (var filePath in filePaths)
        {
            var lines = File.ReadAllLines(filePath);
            var counter = new Counter
            {
                Name = lines[0],
                Value = int.Parse(lines[1]),
                FileName = Path.GetFileName(filePath)
            };
            Counters.Add(counter);
        }
    }

    private void OnAddCounterClicked(object? sender, EventArgs e)
    {
        var name = CounterNameEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        var counter = new Counter { Name = name };
        Counters.Add(counter);
        CounterNameEntry.Text = string.Empty;
        SaveCounter(counter);
    }

    private void OnDecrementClicked(object? sender, EventArgs e)
    {
        ChangeValue(sender, -1);
    }

    private void OnIncrementClicked(object? sender, EventArgs e)
    {
        ChangeValue(sender, 1);
    }

    private static Counter? GetCounter(object? sender)
    {
        return (sender as Button)?.BindingContext as Counter;
    }

    private void ChangeValue(object? sender, int change)
    {
        if (GetCounter(sender) is not { } counter)
        {
            return;
        }

        counter.Value += change;
        SaveCounter(counter);
    }

    private static void SaveCounter(Counter counter)
    {
        if (string.IsNullOrEmpty(counter.FileName))
        {
            counter.FileName = $"counter-{Path.GetRandomFileName()}.txt";
        }

        var filePath = Path.Combine(FileSystem.AppDataDirectory, counter.FileName);
        File.WriteAllLines(filePath, [counter.Name, counter.Value.ToString()]);
    }
}
