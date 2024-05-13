using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.DTO
{
    //property names violate naming conventions but I need it to match the response
    public class StravaUserResponseDTO
    {
        String token_type { get; set; }
        String expires_at { get; set; }
        String expires_in { get; set; }
        String refresh_token { get; set; }
        String access_token { get; set; }
        StravaUserDataDTO athlete { get; set; }
    }
    public class StravaUserDataDTO
    {
        String id { get; set; }
        String firstname { get; set; }
        String lastname { get; set; }
    }
}
