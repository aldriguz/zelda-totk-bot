using Tweetinvi;

namespace ZeldaTotKBot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly string _CONSUMER_KEY;
        private readonly string _CONSUMER_SECRET;
        private readonly string _ACCESS_TOKEN;
        private readonly string _ACCESS_TOKEN_SECRET;
        private readonly DateTime _releaseDate;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _CONSUMER_KEY = Environment.GetEnvironmentVariable("TWITTER_ZELDA_BOT_CUSTOMER_KEY") ?? String.Empty;
            _CONSUMER_SECRET = Environment.GetEnvironmentVariable("TWITTER_ZELDA_BOT_CONSUMER_SECRET") ?? String.Empty;
            _ACCESS_TOKEN = Environment.GetEnvironmentVariable("TWITTER_ZELDA_BOT_ACCESS_TOKEN") ?? String.Empty;
            _ACCESS_TOKEN_SECRET = Environment.GetEnvironmentVariable("TWITTER_ZELDA_BOT_ACCESS_TOKEN_SECRET") ?? String.Empty;
            _releaseDate = configuration.GetValue<DateTime>("Bot:ReleaseDate");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if(string.IsNullOrEmpty(_CONSUMER_KEY) || string.IsNullOrEmpty(_CONSUMER_SECRET) || string.IsNullOrEmpty(_ACCESS_TOKEN) || string.IsNullOrEmpty(_ACCESS_TOKEN_SECRET))
                {
                    _logger.LogError("One of the twitter keys is missing, check your environment variables.");
                }
                else if(_releaseDate.Equals(DateTime.MinValue))
                {
                    _logger.LogError("Missing release date, current value is " + _releaseDate);
                }
                else
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    var daysRemaining = Convert.ToInt32(_releaseDate.Subtract(DateTime.Now).TotalDays);
                    TwitterClient client = new TwitterClient(_CONSUMER_KEY, _CONSUMER_SECRET, _ACCESS_TOKEN, _ACCESS_TOKEN_SECRET);
                    var tweet = await client.Tweets.PublishTweetAsync($"The Legend of Zelda Tears of the Kingdom is comming in {daysRemaining} days!");
                    _logger.LogInformation("Tweet published: " + tweet);
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}