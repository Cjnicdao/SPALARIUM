using Spalarium.Infrastructure.Domain.Security;
using Spalarium.Infrastructure.Domain;
using Spalarium.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Spalarium.Infrastructure.Domain.Models;
using System;
using Spalarium.Infrastructure.Domain.Models.Enums;
using Spalarium.Pages.Manage.Account;

namespace Spalarium.Pages.Manage.Admin
{
    [Authorize(Roles = "admin")]
    public class DashboardModel : PageModel
    {
        private ILogger<Index> _logger;
        private DefaultDBContext _context;
        [BindProperty]
        public ViewModel View { get; set; }




        public DashboardModel(DefaultDBContext context, ILogger<Index> logger)
        {
            _logger = logger;
            _context = context;
            View = View ?? new ViewModel();
        }

        public IActionResult OnGet(object oldrecords, Guid? id = null, Guid? crid = null, int? pageIndex = 1, int? pageSize = 10, string? sortBy = "", SortOrder sortOrder = SortOrder.Ascending, string? keyword = "", Status? status = null, DateTime? date = null)
        {
            Guid? userId = User.Id();
            var user = _context?.Users?.Where(a => a.ID == id).FirstOrDefault();

            //patient query 
            var query = _context.Patients.AsQueryable();

            var skip = (int)((pageIndex - 1) * pageSize);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a =>
                            a.FirstName != null && a.FirstName.ToLower().Contains(keyword.ToLower())
                        || a.LastName != null && a.LastName.ToLower().Contains(keyword.ToLower())
                        || a.MiddleName != null && a.MiddleName.ToLower().Contains(keyword.ToLower())
                );
            }

            var totalRows = query.Count();

            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy.ToLower() == "firstname" && sortOrder == SortOrder.Ascending)
                {
                    query = query.OrderBy(a => a.FirstName);
                }
                else if (sortBy.ToLower() == "middlename" && sortOrder == SortOrder.Descending)
                {
                    query = query.OrderByDescending(a => a.MiddleName);
                }
                else if (sortBy.ToLower() == "lastname" && sortOrder == SortOrder.Ascending)
                {
                    query = query.OrderBy(a => a.LastName);
                }
                else if (sortBy.ToLower() == "lastname" && sortOrder == SortOrder.Descending)
                {
                    query = query.OrderByDescending(a => a.Address);
                }
                else if (sortBy.ToLower() == "address" && sortOrder == SortOrder.Ascending)
                {
                    query = _context.Patients.OrderBy(a => a.Address);
                }
                else if (sortBy.ToLower() == "address" && sortOrder == SortOrder.Descending)
                {
                    query = query.OrderByDescending(a => a.Address);
                }
            }

#pragma warning disable CS8629 // Nullable value type may be null.
            var consumer = query
                           .Skip(skip)
                           .Take((int)pageSize)
                           .ToList();
#pragma warning restore CS8629 // Nullable value type may be null.

            View.Pasyente = new Paged<Infrastructure.Domain.Models.Patient>()
            {
                Items = consumer,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalRows = totalRows,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Keyword = keyword


            };

            //patient query end

            //appts Query
            var query1 = _context.Appointments.Include(a => a.Customer).AsQueryable();
            var skip1 = (int)((pageIndex - 1) * pageSize);

            if (!string.IsNullOrEmpty(keyword))
            {
                query1 = query1.Where(a =>
                         a.Customer.FirstName != null && a.Customer.FirstName.ToLower().Contains(keyword.ToLower())
                      || a.Customer.MiddleName != null && a.Customer.MiddleName.ToLower().Contains(keyword.ToLower())
                      || a.Customer.LastName != null && a.Customer.LastName.ToLower().Contains(keyword.ToLower())

                );
            }
            var totalRows1 = query1.Count();

            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy.ToLower() == "firstname" && sortOrder == SortOrder.Ascending)
                {
                    query1 = query1.OrderBy(a => a.Customer.FirstName);
                }
                else if (sortBy.ToLower() == "middlename" && sortOrder == SortOrder.Ascending)
                {
                    query1 = query1.OrderBy(a => a.Customer.MiddleName);
                }
                else if (sortBy.ToLower() == "lastname" && sortOrder == SortOrder.Ascending)
                {
                    query1 = query1.OrderBy(a => a.Customer.LastName);
                }

            }
            if (status != null)
            {
                query1 = query1.Where(a => a.Status == status);
            }

            if (date != null)
            {
                query1 = query1.Where(a => a.EndTime > date && a.EndTime < date.Value.AddDays(1));
            }
            var appts = query1
                          .Skip(skip)
                          .Take((int)pageSize)
                          .ToList();


            View.Appts = new Paged<Infrastructure.Domain.Models.Appointment>()
            {
                Items = appts,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalRows = totalRows,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Keyword = keyword,


            };



            var profile = _context?.Users?.Where(a => a.ID == userId)
                .Select(a => new ViewModel()
                {

                    NewAddress = a.Address,
                    NewBirthDate = a.BirthDate,
                    NewEmail = a.Email,
                    NewFirstName = a.FirstName,
                    NewGender = a.Gender,
                    NewLastName = a.LastName,
                    NewMiddleName = a.MiddleName,
                }).FirstOrDefault();





            ViewData["address"] = profile.Address;
            ViewData["birthdate"] = profile.BirthDate.ToString("dd/MM/yyyy");
            ViewData["email"] = profile.Email;
            ViewData["firstname"] = profile.FirstName;
            ViewData["middlename"] = profile.MiddleName;
            ViewData["lastname"] = profile.LastName;
            ViewData["gender"] = profile.Gender;


            //View = profile;
            var appointments = _context?.Appointments?.Include(a => a.Patient).ToList();
            View.Appointments = appointments;
            var Customer = _context?.Patients?.ToList();
            View.Customer = Customer;
            var Services = _context?.ConsultationRecords?.ToList();
            View.Records = oldrecords;
            

            return Page();
        }
        public IActionResult OnPostChangeProfile()
        {
            if (string.IsNullOrEmpty(View.FirstName))
            {
                ModelState.AddModelError("", "Role name cannot be blank.");
                return Page();
            }
            if (string.IsNullOrEmpty(View.MiddleName))
            {
                ModelState.AddModelError("", "Role name cannot be blank.");
                return Page();
            }
            if (string.IsNullOrEmpty(View.LastName))
            {
                ModelState.AddModelError("", "Role name cannot be blank.");
                return Page();
            }

            if (string.IsNullOrEmpty(View.Address))
            {
                ModelState.AddModelError("", "Description cannot be blank.");
                return Page();
            }


            var existingCustomer = _context?.Patients?.FirstOrDefault(a =>
                    a.FirstName.ToLower() == View.FirstName.ToLower() &&
                    a.LastName.ToLower() == View.LastName.ToLower() &&
                    a.MiddleName.ToLower() == View.MiddleName.ToLower() &&
                    a.Address.ToLower() == View.Address.ToLower()


            );

            if (existingCustomer != null)
            {
                ModelState.AddModelError("", "Patient is already existing.");
                return Page();
            }

            var user = _context?.Users?.FirstOrDefault(a => a.ID == User.Id());


            if (user != null)
            {


                user.FirstName = View.NewFirstName;
                user.MiddleName = View.NewMiddleName;
                user.LastName = View.NewLastName;
                user.Address = View.NewAddress;




                _context?.Users?.Update(user);
                _context?.SaveChanges();

                return RedirectPermanent("~/Manage/Customer/Dashboard");
            }

            return Page();

        }

        public JsonResult? OnGetPraetors(Guid? id = null)
        {
            if (id != null && id != Guid.Empty)
            {
                return new JsonResult(new List<string>()
                {
                    "Ara Rodriguez, Jose Chan",
                    "Calvin, Jeril Nicdao",
                    "Kenneth Lance",
                    "Cj Nicdao",
                    "Jigs Panganiban",
                    id.ToString()!
                });
            }

            return null;
        }

        public IActionResult OnPostChangePass()
        {

            if (string.IsNullOrEmpty(View.CurrentPass))
            {
                ModelState.AddModelError("", "Field Required");
                return Page();
            }

            if (string.IsNullOrEmpty(View.NewPass))
            {
                ModelState.AddModelError("", "Field Required");
                return Page();
            }

            if (string.IsNullOrEmpty(View.RetypedPassword))
            {
                ModelState.AddModelError("", "Field Required");
                return Page();
            }

            if (View.NewPass != View.RetypedPassword)
            {
                ModelState.AddModelError("", "Passwords are not the same");
                return Page();
            }


            var passwordInfo = _context?.UserLogins?.FirstOrDefault(a => a.UserID == User.Id() && a.Key.ToLower() == "password");

            if (passwordInfo != null)
            {
                if (BCrypt.Net.BCrypt.Verify(View.CurrentPass, passwordInfo.Value))
                {
                    var userRole = _context?.UserRoles?.Include(a => a.Role)!.FirstOrDefault(a => a.UserID == User.Id());

                    passwordInfo.Value = BCrypt.Net.BCrypt.HashPassword(View.NewPass);
                    _context?.UserLogins?.Update(passwordInfo);
                    _context?.SaveChanges();

                    if (userRole!.Role!.Name.ToLower() == "admin")
                    {
                        return RedirectPermanent("/manage/admin/dashboard");
                    }
                    else
                    {
                        return RedirectPermanent("/dashboard/patient");
                    }
                }
            }
            return RedirectPermanent("/manage/admin/dashboard");
        }

        public IActionResult OnPostEdit()
        {
            if (string.IsNullOrEmpty(View.EditFirstName))
            {
                ModelState.AddModelError("", "First name cannot be blank.");
                return RedirectPermanent("/manage/admin/dashboard");
            }

            if (string.IsNullOrEmpty(View.EditMiddleName))
            {
                ModelState.AddModelError("", "Middle name cannot be blank.");
                return RedirectPermanent("/manage/admin/dashboard");
            }
            if (string.IsNullOrEmpty(View.EditLastName))
            {
                ModelState.AddModelError("", "Last name cannot be blank.");
                return RedirectPermanent("/manage/admin/dashboard");
            }


            if (string.IsNullOrEmpty(View.EditAddress))
            {
                ModelState.AddModelError("", "Address name cannot be blank.");
                return RedirectPermanent("/manage/admin/dashboard");
            }



            var customer = _context?.Customers?.FirstOrDefault(a => a.ID == Guid.Parse(View.EditPatientId));

            if (customer != null)
            {

                customer.FirstName = View.EditFirstName;
                customer.MiddleName = View.EditMiddleName;
                customer.LastName = View.EditLastName;
                customer.Address = View.EditAddress;

                _context?.Patients?.Update(customer);

                _context?.SaveChanges();

                return RedirectPermanent("~/manage/admin/dashboard");
            }







            return RedirectPermanent("/manage/admin/dashboard");
        }




        public IActionResult OnPostNewuser()
        {
            if (string.IsNullOrEmpty(View.NewFirstName))
            {
                ModelState.AddModelError("", "First name cannot be blank.");
                return Page();
            }

            if (string.IsNullOrEmpty(View.NewMiddleName))
            {
                ModelState.AddModelError("", "Middle name cannot be blank.");
                return Page();
            }
            if (string.IsNullOrEmpty(View.NewLastName))
            {
                ModelState.AddModelError("", "Last name cannot be blank.");
                return Page();
            }
            if (!Enum.IsDefined(typeof(Gender), View.NewGender))
            {
                ModelState.AddModelError("", "Sex name cannot be blank.");
                return Page();
            }
            if (DateTime.MinValue >= View.NewBirthDate)
            {
                ModelState.AddModelError("", "Birthdate name cannot be blank.");
                return Page();
            }

            if (string.IsNullOrEmpty(View.NewAddress))
            {
                ModelState.AddModelError("", "Address name cannot be blank.");
                return Page();
            }
            if (string.IsNullOrEmpty(View.NewEmail))
            {
                ModelState.AddModelError("", "Email cannot be blank.");
                return Page();
            }
            if (string.IsNullOrEmpty(View.NewPassword))
            {
                ModelState.AddModelError("", "Password cannot be blank.");
                return Page();
            }







            Guid userGuid = Guid.NewGuid();
            User newUser = new User()
            {
                ID = userGuid,
                FirstName = View.NewFirstName,
                MiddleName = View.NewMiddleName,
                LastName = View.NewLastName,
                Gender = View.NewGender,
                BirthDate = View.NewBirthDate,
                Address = View.NewAddress,
                Email = View.NewEmail,
                RoleID = Guid.Parse("54f00f70-72b8-4d34-a953-83321ff6b101")
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
                    Value = BCrypt.Net.BCrypt.EnhancedHashPassword(View.NewPassword)
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

            UserRole userRole = new UserRole()
            {
                Id = Guid.NewGuid(),
                UserID = userGuid,
                RoleID = Guid.Parse("54f00f70-72b8-4d34-a953-83321ff6b101")

            };


            _context?.Users?.Add(newUser);

            _context?.UserLogins?.AddRange(userLogins);
            _context?.UserRoles?.Add(userRole);
            _context?.SaveChanges();

            View.FirstName = View.FirstName;
            View.LastName = View.LastName;
            View.Email = View.Email;
            View.Address = View.Address;
            View.BirthDate = View.BirthDate;
            View.MiddleName = View.MiddleName;
            View.Gender = View.Gender;


            return Page();
        }
        public IActionResult OnPostEditApt()
        {
            if (string.IsNullOrEmpty(View.Services))
            {
                ModelState.AddModelError("", "First name cannot be blank.");
                return RedirectPermanent("/manage/admin/dashboard");
            }




            var symptom = _context?.Appointments?.FirstOrDefault(a => a.ID == Guid.Parse(View.SymptomID));

            if (symptom != null)
            {

                symptom.Symptom = View.Symptom1;
                symptom.StartTime = View.StartTime1;


                _context?.Appointments?.Update(symptom);

                _context?.SaveChanges();

                return RedirectPermanent("~/manage/admin/dashboard");
            }







            return RedirectPermanent("/manage/admin/dashboard");
        }

        public IActionResult OnPostEditstatus()
        {
            if (!Enum.IsDefined(typeof(Status), View.Statusedit))
            {
                ModelState.AddModelError("", " status cannot be blank.");
                return RedirectPermanent("/manage/admin/dashboard");
            }




            var symptom = _context?.Appointments?.FirstOrDefault(a => a.ID == Guid.Parse(View.SymptomID));

            if (symptom != null)
            {

                symptom.Status = View.Statusedit;



                _context?.Appointments?.Update(symptom);

                _context?.SaveChanges();

                return RedirectPermanent("~/manage/admin/dashboard");
            }







            return RedirectPermanent("/manage/admin/dashboard");
        }

        public IActionResult OnPostCustomer()
        {
            if (string.IsNullOrEmpty(View.AddFirstName))
            {
                ModelState.AddModelError("", "First name cannot be blank.");
                return RedirectPermanent("/manage/admin/dashboard");
            }

            if (string.IsNullOrEmpty(View.AddMiddleName))
            {
                ModelState.AddModelError("", "Middle name cannot be blank.");
                return RedirectPermanent("/manage/admin/dashboard");
            }
            if (string.IsNullOrEmpty(View.AddLastName))
            {
                ModelState.AddModelError("", "Last name cannot be blank.");
                return RedirectPermanent("/manage/admin/dashboard");
            }
            if (!Enum.IsDefined(typeof(Gender), View.AddGender))
            {
                ModelState.AddModelError("", "Sex name cannot be blank.");
                return RedirectPermanent("/manage/admin/dashboard");
            }
            if (DateTime.MinValue >= View.AddBirthDate)
            {
                ModelState.AddModelError("", "Birthdate name cannot be blank.");
                return RedirectPermanent("/manage/admin/dashboard");
            }

            if (string.IsNullOrEmpty(View.AddAddress))
            {
                ModelState.AddModelError("", "Address name cannot be blank.");
                return RedirectPermanent("/manage/admin/dashboard");
            }
            if (string.IsNullOrEmpty(View.AddEmail))
            {
                ModelState.AddModelError("", "Address name cannot be blank.");
                return RedirectPermanent("/manage/admin/dashboard");
            }
            if (string.IsNullOrEmpty(View.AddPass))
            {
                ModelState.AddModelError("", "Address name cannot be blank.");
                return RedirectPermanent("/manage/admin/dashboard");
            }
            Guid customerGuid = Guid.NewGuid();
            Guid userGuid = Guid.NewGuid();
            User user = new User()
            {
                ID = userGuid,
                customerID = customerGuid,
                FirstName = View.AddFirstName,
                MiddleName = View.AddMiddleName,
                LastName = View.AddLastName,
                Gender = View.AddGender,
                BirthDate = View.AddBirthDate,
                Address = View.AddAddress,
                Email = View.AddEmail,
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
                    Value = BCrypt.Net.BCrypt.EnhancedHashPassword(View.AddPass)
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
            Infrastructure.Domain.Models.Patient customer = new Infrastructure.Domain.Models.Patient()
            {

                ID = customerGuid,
                FirstName = View.AddFirstName,
                MiddleName = View.AddMiddleName,
                LastName = View.AddLastName,
                Gender = View.AddGender,
                BirthDate = View.AddBirthDate,
                Address = View.AddAddress

            };
            UserRole userRole = new UserRole()
            {
                Id = Guid.NewGuid(),
                UserID = userGuid,
                RoleID = Guid.Parse("2afa881f-e519-4e67-a841-e4a2630e8a2a")

            };


            _context?.Users?.Add(user);
            _context?.Patients?.Add(customer);
            _context?.UserLogins?.AddRange(userLogins);
            _context?.UserRoles?.Add(userRole);
            _context?.SaveChanges();

            return RedirectPermanent("/manage/admin/dashboard");
        }



        public class ViewModel : UserViewModel
        {
            internal object Records;

            public string? CurrentPass { get; set; }
            public string? NewPass { get; set; }
            public string? RetypedPassword { get; set; }
            public Guid? ID { get; set; }

            [ForeignKey("CUSTOMERID")]
            public Infrastructure.Domain.Models.Customer? Customer { get; set; }
            public Paged<Infrastructure.Domain.Models.Customer>? Pasyente { get; set; }
            public Paged<Infrastructure.Domain.Models.Appointment>? Appts { get; set; }


            public string? NewFirstName { get; set; }
            public string? NewLastName { get; set; }
            public string? NewMiddleName { get; set; }
            public string? NewEmail { get; set; }

            public string? NewAddress { get; set; }
            public string? NewPassword { get; set; }
            public DateTime NewBirthDate { get; set; }
            public Infrastructure.Domain.Models.Enums.Gender NewGender { get; set; }
            public List<Appointment>? Appointments { get; set; }
            public string? EditFirstName { get; internal set; }

            public List<Records? Old Records { get; set; }
            public List<Infrastructure.Domain.Models.Customer>? Customer{ get; set; }
            public List<FromServicesAttribute>? Services { get; set; }
           

           
            public DateTime? StartTime1 { get; set; }

 




            public string? EditFirstName { get; set; }
            public string? EditLastName { get; set; }
            public string? EditMiddleName { get; set; }
            public string? EditAddress { get; set; }
            public string? EditCustomerId { get; set; }
         

            //new patient
            public string? AddFirstName { get; set; }
            public string? AddLastName { get; set; }
            public string? AddMiddleName { get; set; }
            public string? AddAddress { get; set; }
            public string? AddEmail { get; set; }
            public string? AddPass { get; set; }
            public DateTime AddBirthDate { get; set; }
            public Infrastructure.Domain.Models.Enums.Gender AddGender { get; set; }


        }
    }
