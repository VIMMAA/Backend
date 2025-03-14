using Microsoft.EntityFrameworkCore;

using Domain.Entities;
using Domain.Entities.Application;
namespace Api.Context;
public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; }
     public DbSet<ApplicationModel> Applications { get; set; }
    public DbSet<AttachedFile> Files {get;set;}
     public DbSet<Lesson> Lessons { get; set; }
    public DbSet<InvitationLink> InvitationLinks { get; set; }


    public bool TestConnection()
    {
        try
        {
            return Database.CanConnect(); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка подключения к базе данных: {ex.Message}");
            return false;
        }
    }
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
}
