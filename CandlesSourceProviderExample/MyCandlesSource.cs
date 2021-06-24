using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using FancyCandles;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Diagnostics;
using System.Threading;

namespace CandlesSourceProviderExample
{
    internal class MyCandlesSource : ObservableCollection<ICandle>, ICandlesSourceFromProvider, IResourceWithUserCounter
    {
        private readonly HttpClient httpClient = new HttpClient();

        private readonly DispatcherTimer updateTimer;
        private static readonly TimeSpan updateTimerInterval = new TimeSpan(0,0,3);

        private readonly string exchangeName;
        private readonly string ticker;
        //---------------------------------------------------------------------------------------------------------------------------------------
        internal MyCandlesSource(string exchangeName, string ticker, TimeFrame timeFrame, Action<string> OnNoMoreUsersAction)
        {
            this.exchangeName = exchangeName;
            this.ticker = ticker;
            this.timeFrame = timeFrame;
            this.OnNoMoreUersAction = OnNoMoreUsersAction;

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            updateTimer.Tick += new EventHandler(OnUpdateTimerTickAsync);
            updateTimer.Start();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private readonly TimeFrame timeFrame;
        public TimeFrame TimeFrame
        {
            get { return timeFrame; }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public string SecID
        {
            get { return $"{exchangeName}_{ticker}"; }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        private async void OnUpdateTimerTickAsync(object sender, EventArgs e)
        {
            await LoadFromRestAPIAndUpdateCandlesAsync();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        private async Task LoadFromRestAPIAndUpdateCandlesAsync()
        {
            long lastCandle_unixTime;
            if (Count == 0)
            {
                lastCandle_unixTime = 0;
                updateTimer.Interval = updateTimerInterval;
            }
            else 
                lastCandle_unixTime = ((DateTimeOffset)this[Count - 1].t).ToUnixTimeSeconds();

            int timeFrameInSeconds = TimeFrame.ToSeconds();
            //await client.GetAsync("https://api.cryptowat.ch/markets/coinbase-pro/btcusd/ohlc?before=1566239760&after=1566232020&periods=60");
            string requestString = $"https://api.cryptowat.ch/markets/{exchangeName}/{ticker}/ohlc?after={lastCandle_unixTime}&periods={timeFrameInSeconds}";
            HttpResponseMessage response = await httpClient.GetAsync(requestString);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject jObj = JObject.Parse(responseBody);
            JToken jToken = jObj["result"][timeFrameInSeconds.ToString()];
            int N = jToken.Count();

            for (int i = 0; i < N; i++)
            {
                JToken jCandle = jToken[i];
                long unixTime = (long)jCandle[0];
                DateTime t = UnixTimeStampToDateTime(unixTime);

                ICandle cndl = new Candle(t, (double)jCandle[1], (double)jCandle[2], (double)jCandle[3], (double)jCandle[4], (double)jCandle[5]);
                if (Count == 0 || t > this[Count - 1].t)
                    Add(cndl);
                else if (t == this[Count - 1].t)
                    this[Count - 1] = cndl;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        private readonly Action<string> OnNoMoreUersAction;

        public int UserCount { get; private set; }

        public void IncreaseUserCount()
        {
            UserCount++;
        }

        public void DecreaseUserCount()
        {
            UserCount--;

            if (UserCount <= 0)
                OnNoMoreUersAction($"{SecID}_{(int)TimeFrame}");
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
