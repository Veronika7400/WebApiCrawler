
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApiCrawler.Data;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
}
