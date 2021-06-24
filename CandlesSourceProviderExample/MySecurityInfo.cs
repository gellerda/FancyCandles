using FancyCandles;

namespace CandlesSourceProviderExample
{
    public class MySecurityInfo : FancyCandles.ISecurityInfo
    {
        private readonly string secID;
        public string SecID { get { return secID; } }

        public SecurityTypes SecurityType
        { get { return SecurityTypes.CryptocurrencyPair; } }

        private readonly string exchangeName;
        public string ExchangeName { get { return exchangeName; } }

        private readonly string ticker;
        public string Ticker { get { return ticker; } }

        private readonly string securityName;
        public string SecurityName { get { return securityName; } }

        public MySecurityInfo(string exchangeName, string ticker, string securityName)
        {
            this.ticker = ticker;
            this.exchangeName = exchangeName;
            this.securityName = securityName;
            this.secID = $"{exchangeName}_{ticker}";
        }
    }
}
