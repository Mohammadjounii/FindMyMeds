using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FindMyMeds.Core.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(200)]
        public string? FullName { get; set; }

    }
}
