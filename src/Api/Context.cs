using Microsoft.EntityFrameworkCore;

using Domain.Entities;

namespace Api.Context;
public class ApplicationContext : DbContext
{
    public DbSet <User> Users { get; set; }
    // public DbSet<DoctorModel> Doctors { get; set; }

    // public DbSet<InspectionModel> Inspections {get;set;}
    // public DbSet<BlackToken> BlackTokens { get; set; }

    // public DbSet<Icd10RecordModel> Records {get; set;}


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
