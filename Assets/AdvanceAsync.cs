using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class AdvanceAsync : MonoBehaviour
{

    public TMP_Text resultText;

    CancellationTokenSource cts = new CancellationTokenSource();
    List<float> items = new List<float>();

    public void Start()
    {
        Debug.Log("Start");
        items.Add(2001.0f);
        items.Add(1002.0f);
        items.Add(3003.0f);
        items.Add(10004.0f);
        items.Add(10005.0f);

    }

    public void ExecuteSyncOperation()
    {
        resultText.text = "";
        var watch = System.Diagnostics.Stopwatch.StartNew();
        LongRunningOperationNonSync();

        watch.Stop();
        var elapsedTime = watch.ElapsedMilliseconds;
        resultText.text = resultText.text + " Total Time : " + elapsedTime;

    }

    public async void ExecuteAsyncOperation()
    {
        cts = new CancellationTokenSource();
        resultText.text = "";
        var watch = System.Diagnostics.Stopwatch.StartNew();
        await AsyncOperation();

        watch.Stop();
        var elapsedTime = watch.ElapsedMilliseconds;
        resultText.text = resultText.text + " Total Time : " + elapsedTime;
    }

    public async void ExecuteParallelAsyncOperation()
    {
        cts = new CancellationTokenSource();
        resultText.text = "";
        var watch = System.Diagnostics.Stopwatch.StartNew();
        await RunParallelAsync();

        watch.Stop();
        var elapsedTime = watch.ElapsedMilliseconds;
        resultText.text = resultText.text + " Total Time : " + elapsedTime;
    }


    private async Task RunParallelAsync()
    {
        List<Task<float>> tasks = new List<Task<float>>();

        foreach (var item in items)
        {
            tasks.Add(Task.Run(() => MyLongRunningJob(item)));
        }
        var result = await Task.WhenAny(tasks);
        Debug.Log("U>> Completed single task .. " + result.Result);
        var results = await Task.WhenAll(tasks);

        foreach (var item in results)
        {
            Debug.Log("U>> Completed task .. " + item);
        }
    }

    

    private void LongRunningOperationNonSync()
    {
        for (int i = 0; i < items.Count; i++)
        {
            MyLongRunningJob(items[i]);
        }
    }


    private async Task AsyncOperation()
    {

        for (int i = 0; i < 10; i++)
        {
            // Do something that takes times like a Thread.Sleep in .NET Core 2.
            float results = await Task.Run(() => MyLongRunningJob(i));
          
        }
    }

    private float MyLongRunningJob(float token)
    {
       // Debug.Log("U>> Runn " + token);
        // Do something that takes times like a Thread.Sleep in .NET Core 2.
        Thread.Sleep((int)token);
        return token;
    }



}
