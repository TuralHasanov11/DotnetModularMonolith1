using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModularMonolith.Users.Core.UserAggregate;
using SharedKernel;

namespace ModularMonolith.Web.Areas.Identity.Pages.Accounts
{
    public class IndexModel : PageModel
    {
        // setup user manager
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public UserListViewModel Users { get; private set; }

        public class UserListViewModel(List<UserViewModel> users, int count)
            : List<UserViewModel>
        {
            public int TotalCount { get; } = count;

            public Guid? NextCursor => Users.LastOrDefault()?.Id;

            private List<UserViewModel> Users { get; } = users;
        }

        public class UserViewModel
        {
            public Guid Id { get; set; }

            public string? UserName { get; set; }

            public string? Email { get; set; }

            public string? PhoneNumber { get; set; }
        }


        public async Task OnGet(Guid? cursor = null, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = _userManager.Users.WhereIf(cursor != null, u => u.Id <= cursor)
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                });

            Users = new UserListViewModel(
                await query
                    .OrderByDescending(u => u.Id)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken),
                await query.CountAsync(cancellationToken));
        }
    }
}
