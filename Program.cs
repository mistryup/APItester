using RestSharp;  // Add this line
using System.Diagnostics;

namespace APILoadTest
{
    public class LoadTest
    {
        private RestClient _client;
        private RestRequest _request;

        public void Setup()
        {
            _client = new RestClient("https://httpbin.org/get");
            _request = new RestRequest("/");
        }

        public void LoadTesting()
        {
            int numberOfRequests = 5;
            var tasks = new Task[numberOfRequests];
            var timeTaken = new TimeSpan[numberOfRequests + 1];

            using (var writer = new StreamWriter("C:\\Temp\\results.csv"))
            {
                writer.WriteLine("RequestNumber : TimeTaken");

                for (int i = 0; i < numberOfRequests; i++)
                {
                    int iLambda = i; // Make a copy as i can go above 1 within Lambda somehow !?!
                    Console.WriteLine("**** OUT iLambda: " + iLambda);
                    tasks[iLambda] = Task.Factory.StartNew(() =>
                    {
                        var stopWatch = new Stopwatch();
                        Console.WriteLine("**** IN iLambda: " + iLambda);
                        stopWatch.Start();
                        _client.Execute<RestResponse>(_request);
                        stopWatch.Stop();

                        timeTaken[iLambda] = stopWatch.Elapsed;
                        writer.WriteLine($"{iLambda + 1} : {timeTaken[iLambda]}");

                        stopWatch.Reset();
                    });
                }

                Console.WriteLine("**** FINISHED tasks: " + tasks);
                Task.WaitAll(tasks);
            }

            var totalTimeTaken = TimeSpan.FromTicks(timeTaken.Sum(time => time.Ticks));
            Console.WriteLine($"Time taken for {numberOfRequests} requests: {totalTimeTaken}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var loadTest = new LoadTest();
            loadTest.Setup();
            loadTest.LoadTesting();
        }
    }
}