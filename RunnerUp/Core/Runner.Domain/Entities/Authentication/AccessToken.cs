
namespace Runner.Domain.Entities.Authentication
{
    public class AccessToken
    {
        public EntityId AccessTokenId { get; set; }
        public EntityId UserId { get; set; }
        public required string Token { get; set; }
        public DateTime ExpireDateimeUTC { get; set; }
        public AccessTokenType Type { get; set; }
        public AccessTokenState State { get; set; }
    }
}
