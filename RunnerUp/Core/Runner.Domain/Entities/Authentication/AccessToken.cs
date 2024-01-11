
namespace Runner.Domain.Entities.Authentication
{
    public class AccessToken
    {
        public Guid AccessTokenId { get; set; }
        public Guid UserId { get; set; }
        public required string Token { get; set; }
        public DateTime ExpireDateimeUTC { get; set; }
        public AccessTokenType Type { get; set; }
        public AccessTokenState State { get; set; }
    }
}
