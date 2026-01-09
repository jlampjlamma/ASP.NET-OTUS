using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Employee : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; }

        public List<Role> Roles { get; set; }

        public int AppliedPromocodesCount { get; set; }

        public Employee() { }
        private Employee(string firstName, string lastName, string email, List<Role> roles) : base()
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Roles = roles;
            AppliedPromocodesCount = 0;
        }

        public static Employee Create(string firstName, string lastName, string email, List<Role> roles)
        {
            return new Employee(firstName, lastName, email, roles);
        }

        public void Update(string firstName, string lastName, string email, List<Role> roles, int appliedPromocodesCount)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            AppliedPromocodesCount = appliedPromocodesCount;
            Roles = roles;
        }

        public void AddRoles(Role role) => Roles.Add(role);
    }
}