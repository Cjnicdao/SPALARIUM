using Spalarium.Infrastructure.Domain.Models.Enums;
using Spalarium.Infrastructure.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Spalarium.Infrastructure.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spalrium.ViewModel;
using System.Reflection;
using Spalarium.Pages.Manage.Admin;

namespace Spalarium.Pages.Manage.Account
{

    public class SignUp : PageModel
    {
        private ILogger<Index> _logger;
        private DefaultDBContext _context;

        [BindProperty]
        public ViewModel View { get; set; }

        public SignUp(DefaultDBContext context, ILogger<Index> logger)
        {
            _logger = logger;
            _context = context;
            View = View ?? new ViewModel();
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(View.FirstName))
            {
                ModelState.AddModelError("", "First name cannot be blank.");
                return Page();
            }

            if (string.IsNullOrEmpty(View.MiddleName))
            {
                ModelState.AddModelError("", "Middle name cannot be blank.");
                return Page();
            }
            if (string.IsNullOrEmpty(View.LastName))
            {
                ModelState.AddModelError("", "Last name cannot be blank.");
                return Page();
            }
            if (!Enum.IsDefined(typeof(Gender), View.Gender))
            {
                ModelState.AddModelError("", "Sex name cannot be blank.");
                return Page();
            }
            if (DateTime.MinValue >= View.BirthDate)
            {
                ModelState.AddModelError("", "Birthdate name cannot be blank.");
                return Page();
            }

            if (string.IsNullOrEmpty(View.Address))
            {
                ModelState.AddModelError("", "Address name cannot be blank.");
                return Page();
            }
            if (string.IsNullOrEmpty(View.Email))
            {
                ModelState.AddModelError("", "Address name cannot be blank.");
                return Page();
            }
            if (string.IsNullOrEmpty(View.Password))
            {
                ModelState.AddModelError("", "Address name cannot be blank.");
                return Page();
            }
            Guid CustomerGuid = Guid.NewGuid();
            Guid userGuid = Guid.NewGuid();
            User user = new User()
            {
                ID = userGuid,
                CustomerID = CustomerGuid,
                FirstName = View.FirstName,
                MiddleName = View.MiddleName,
                LastName = View.LastName,
                Gender = View.Gender,
                BirthDate = View.BirthDate,
                Address = View.Address,
                Email = View.Email,
                RoleID = Guid.Parse("2afa881f-e519-4e67-a841-e4a2630e8a2a")
            };
            List<UserLogin> userLogins = new List<UserLogin>();
            userLogins.AddRange(new List<UserLogin>()
            {
                new UserLogin()
                {
                    ID = Guid.NewGuid(),
                    UserID =userGuid,
                    Type = "General",
                    Key = "Password",
                    Value = BCrypt.Net.BCrypt.HashPassword(View.Password)
                },
                new UserLogin()
                {
                    ID = Guid.NewGuid(),
                    UserID =userGuid,
                    Type = "General",
                    Key = "IsActive",
                    Value = "true"
                },
                new UserLogin()
                {
                    ID = Guid.NewGuid(),
                    UserID =userGuid,
                    Type = "General",
                    Key = "LoginRetries",
                    Value = "0"
                }
            });
            Infrastructure.Domain.Models.Patient patient = new Infrastructure.Domain.Models.Patient()
            {

                ID = patientGuid,
                FirstName = View.FirstName,
                MiddleName = View.MiddleName,
                LastName = View.LastName,
                Gender = View.Gender,
                BirthDate = View.BirthDate,
                Address = View.Address

            };
            UserRole userRole = new UserRole()
            {
                Id = Guid.NewGuid(),
                UserID = userGuid,
                RoleID = Guid.Parse("2afa881f-e519-4e67-a841-e4a2630e8a2a")

            };


            _context?.Users?.Add(user);
            _context?.Patients?.Add(patient);
            _context?.UserLogins?.AddRange(userLogins);
            _context?.UserRoles?.Add(userRole);
            _context?.SaveChanges();

            return RedirectPermanent("~/index");
        }

        public class ViewModel : UserViewModel
        {
            public string? FirstName { get; internal set; }
            public string? MiddleName { get; internal set; }
            public string? LastName { get; internal set; }
            public object Gender { get; internal set; }
            public DateTime BirthDate { get; internal set; }
            public string? Address { get; internal set; }
            public string? Email { get; internal set; }
            public string? Password { get; internal set; }
        }
    }

    internal class UserLogin
    {
        public UserLogin()
        {
        }

        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public string Type { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
    }
}