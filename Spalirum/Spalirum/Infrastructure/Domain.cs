namespace Infrastructure
{
    internal class Domain
    {
        internal class Models
        {
            public class Enums
            {
                public class Gender
                {
                }
            }

            internal class Patient
            {
                public Guid ID { get; internal set; }
                public string FirstName { get; internal set; }
                public string MiddleName { get; internal set; }
                public string LastName { get; internal set; }
                public object Gender { get; internal set; }
                public object gender { get; internal set; }
                public DateTime BirthDate { get; internal set; }
                public string Address { get; internal set; }
            }

            internal class Customer
            {
                public static implicit operator Customer(Patient v)
                {
                    throw new NotImplementedException();
                }
            }

            internal class Schedule
            {
            }
        }
    }
}