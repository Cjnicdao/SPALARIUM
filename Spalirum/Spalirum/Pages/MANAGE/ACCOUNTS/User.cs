namespace Spalarium.Pages.Manage.Account
{
    internal class User
    {
        public Guid ID { get; internal set; }
        public Guid CustomerID { get; internal set; }
        public string FirstName { get; internal set; }
        public string MiddleName { get; internal set; }
        public string LastName { get; internal set; }
        public object Gender { get; internal set; }
        public DateTime BirthDate { get; internal set; }
        public string Address { get; internal set; }
        public string Email { get; internal set; }
        public Guid RoleID { get; internal set; }
    }
}