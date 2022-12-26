using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

public class DemoScene : MonoBehaviour
{
    public TMP_Text resultText;

    CancellationTokenSource cts = new CancellationTokenSource();


    public void ExecuteSyncOperation()
    {
        resultText.text = "";
        var watch = System.Diagnostics.Stopwatch.StartNew();
        RunDownloadSync();

        watch.Stop();
        var elapsedTime = watch.ElapsedMilliseconds;
        resultText.text = resultText.text +  " Total Time : " + elapsedTime;

    }

    public async void ExecuteAsyncOperation()
    {
        cts = new CancellationTokenSource();
        resultText.text = "";
        var watch = System.Diagnostics.Stopwatch.StartNew();
        await RunDownloadAsync();

        watch.Stop();
        var elapsedTime = watch.ElapsedMilliseconds;
        resultText.text = resultText.text + " Total Time : " + elapsedTime;
    }

    public async void ExecuteAsyncParallelOperation()
    {
        resultText.text = "";
        var watch = System.Diagnostics.Stopwatch.StartNew();
        await RunDownloadParallelAsync();

        watch.Stop();
        var elapsedTime = watch.ElapsedMilliseconds;
        resultText.text = resultText.text + " Total Time : " + elapsedTime;
    }

    public void CancelOperation()
    {
        cts.Cancel();
    }

    private void RunDownloadSync()
    {
        List<string> websites = PrepData();

        foreach(string site in websites)
        {
            WebsiteDataModel results = DownloadWebsite(site);
            ReportWebsiteInfo(results);
        }
    }


    private async Task RunDownloadAsync()
    {
        List<string> websites = PrepData();

        foreach (string site in websites)
        {
            WebsiteDataModel results = await Task.Run(()=> DownloadWebsite(site));
            ReportWebsiteInfo(results);
            if (cts.IsCancellationRequested) {
                resultText.text = resultText.text +  " Cancelled operation";
                return;
            }
           
        }
    }


    private async Task RunDownloadParallelAsync()
    {
        List<string> websites = PrepData();
        List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

        foreach (string site in websites)
        {
            tasks.Add(Task.Run(() => DownloadWebsite(site)));
        }

        var results = await Task.WhenAll(tasks);

        foreach(var item in results)
        {
            ReportWebsiteInfo(item);
        }
    }


    public List<string> PrepData()
    {
        List<string> output = new List<string>();


        output.Add("https://yahoo.com/");
        output.Add("https://google.com/");
        output.Add("https://youtube.com/");
        output.Add("https://facebook.com/");
     //   output.Add("https://stackoverflow.com/");

        return output;
    }

    private WebsiteDataModel DownloadWebsite(string websiteurl)
    {
        WebsiteDataModel output = new WebsiteDataModel();
        WebClient client = new WebClient();

        output.websiteUrl = websiteurl;
        output.websiteData = client.DownloadString(websiteurl);

        return output;

    }

    private void ReportWebsiteInfo(WebsiteDataModel data)
    {
        resultText.text = resultText.text +  data.websiteUrl + " Downloaded " + data.websiteData.Length + "  characters long \n";
    }

}

public class WebsiteDataModel
{
    public string websiteUrl { get; set; } = "";
    public string websiteData { get; set; } = "";
}
