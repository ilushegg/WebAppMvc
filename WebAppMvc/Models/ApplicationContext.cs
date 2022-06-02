
using Microsoft.EntityFrameworkCore;



namespace WebAppMvc.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Person> Persons { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
    }
}
/*
using Microsoft.EntityFrameworkCore;
using WebAppMvc.Models;
public class ApplicationContext : DbContext
{
    public DbSet<Person> People { get; set; }
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
}
*/