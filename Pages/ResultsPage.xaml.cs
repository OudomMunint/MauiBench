using MauiBench.Data;

namespace MauiBench.Pages;

public partial class ResultsPage : ContentPage
{
	public ResultsPage()
	{
		InitializeComponent();
        //GenerateDummyData();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        ItemDatabase database = await ItemDatabase.Instance;
        MainCollectionView.ItemsSource = await database.GetItemsAysnc();
    }

    private void GenerateDummyData()
    {
        var random = new Random();
        var dummyData = new List<Models.BenchmarkModel>();

        for (int i = 0; i < 10; i++)
        {
            var timestamp = DateTime.Now.AddMinutes(-random.Next(0, 600)); // Last 10 hours
            var testNameValues = Enum.GetValues(typeof(Models.BenchmarkModel.TestName));
            var result = $"Result {random.Next(1, 1000)}";

            var benchmarkTypeValues = Enum.GetValues(typeof(Models.BenchmarkModel.Type));
            var benchmarkType = (Models.BenchmarkModel.Type)benchmarkTypeValues.GetValue(random.Next(benchmarkTypeValues.Length))!;

            dummyData.Add(new Models.BenchmarkModel
            {
                Timestamp = timestamp,
                TestNameValue = (Models.BenchmarkModel.TestName)testNameValues.GetValue(random.Next(testNameValues.Length))!,
                BenchmarkType = benchmarkType,
                Result = result
            });
        }

        MainCollectionView.ItemsSource = dummyData;
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        ItemDatabase database = await ItemDatabase.Instance;
        var result = await DisplayAlert("Delete All", "Are you sure you want to delete all benchmark results?", "Yes", "No");
        if (result)
        {
            await database.DeleteAllItemsAsync();
            var items = await database.GetItemsAysnc();
            MainCollectionView.ItemsSource = items;
        }
    }
}