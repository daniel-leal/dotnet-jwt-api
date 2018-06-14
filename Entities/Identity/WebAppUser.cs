using Microsoft.AspNetCore.Identity;

namespace Entities.Identity
{
    public class WebAppUser : IdentityUser
    {
        [PersonalData]
        public string Name { get; set; }

        [PersonalData]
        public string AvatarUrl { get; set; }
    }
}
