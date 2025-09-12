namespace PWC.Challenge.Infrastructure.Configurations
{
    public class RedisSettings
    {
        public string InstanceName { get; set; } = "PWChallenge:";
        public int AbsoluteExpirationInSeconds { get; set; } = 30;
        public int SlidingExpirationInSeconds { get; set; } = 15;
        public bool Enabled { get; set; } = true;
    }
}