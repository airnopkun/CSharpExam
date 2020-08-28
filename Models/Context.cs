using Microsoft.EntityFrameworkCore;
namespace Exam.Models
{ 
    // the MyContext class representing a session with our MySQL 
    // database allowing us to query for or save data
    public class Context : DbContext 
    { 
        public Context(DbContextOptions options) : base(options) { }
        // the "Users" table name will come from the DbSet variable name
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<RSVP> RSVPs { get; set; }
    }
}