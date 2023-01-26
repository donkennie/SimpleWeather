namespace SimpleWeather.Core
{
    public class BaseCommandResponse
    {

        public bool IsSuccessful { get; set; }

        public int StatusCode { get; set; }

        public string Message { get; set; }

    }
}
