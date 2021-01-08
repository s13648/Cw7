using System;

namespace Cw7.Dto
{
    public class AccessToken
    {
        public string Token { get; set; }

        public Guid RefreshToken { get; set; }
    }
}
