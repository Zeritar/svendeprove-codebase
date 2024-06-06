namespace SystemOverseer_API.Auth
{
    public class TokenModel
    {
        public string Token { get; set; } = "";
        public DateTime Expiration { get; set; }
    }
}
