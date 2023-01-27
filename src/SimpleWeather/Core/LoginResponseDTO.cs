namespace SimpleWeather.Core
{
    public class LoginResponseDTO: BaseCommandResponse
    {
        public string Token { get; set; }

        public string Username { get; set; }

    }
}
