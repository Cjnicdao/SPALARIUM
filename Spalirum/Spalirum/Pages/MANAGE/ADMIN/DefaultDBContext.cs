namespace Spalarium.Pages.Manage.Admin
{
    public class DefaultDBContext
    {
        public object Customer { get; internal set; }
        public object Schedule { get; internal set; }

        internal void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}