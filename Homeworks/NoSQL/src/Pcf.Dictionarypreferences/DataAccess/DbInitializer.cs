using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public interface IDbInitializer
    {
        public void Initialize();
    }

    public class DbInitializer : IDbInitializer
    {
        PreferenceDbContext _context;

        public DbInitializer(PreferenceDbContext context)
        {
            _context = context;
        }
        public void Initialize()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _context.AddRange(FakeDataFactory.Preferences);
            _context.SaveChanges();
        }
    }
}
