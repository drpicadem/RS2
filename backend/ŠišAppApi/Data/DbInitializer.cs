using Microsoft.EntityFrameworkCore;
using ŠišAppApi.Models;
using BCrypt.Net;

namespace ŠišAppApi.Data;

public static class DbInitializer
{
    public static async Task Initialize(ApplicationDbContext context)
    {
        // Provjeri ima li već podataka
        if (await context.Users.AnyAsync())
        {
            return; // Baza je već inicijalizirana
        }

        // Kreiraj kategorije usluga
        var kategorije = new List<ServiceCategory>
        {
            new ServiceCategory { Name = "Muško šišanje", Description = "Različiti stilovi muškog šišanja", DisplayOrder = 1 },
            new ServiceCategory { Name = "Žensko šišanje", Description = "Stilovi i tretmani za žensku kosu", DisplayOrder = 2 },
            new ServiceCategory { Name = "Brada", Description = "Oblikovanje i njega brade", DisplayOrder = 3 },
            new ServiceCategory { Name = "Boja kose", Description = "Farbanje i tretmani boje", DisplayOrder = 4 },
            new ServiceCategory { Name = "Tretmani", Description = "Različiti tretmani za kosu", DisplayOrder = 5 }
        };
        await context.ServiceCategories.AddRangeAsync(kategorije);
        await context.SaveChangesAsync();

        // Kreiraj salone
        var saloni = new List<Salon>
        {
            new Salon
            {
                Name = "Frizerski Salon Džanan",
                Description = "Moderan frizerski salon u centru grada",
                Address = "Ferhadija 12",
                City = "Sarajevo",
                PostalCode = "71000",
                Country = "Bosna i Hercegovina",
                Phone = "033/123-456",
                Email = "info@dzanan.ba",
                Website = "www.dzanan.ba",
                Rating = 4.8f,
                ReviewCount = 0,
                IsVerified = true,
                BusinessHours = "{\"Monday\":\"09:00-20:00\",\"Tuesday\":\"09:00-20:00\",\"Wednesday\":\"09:00-20:00\",\"Thursday\":\"09:00-20:00\",\"Friday\":\"09:00-20:00\",\"Saturday\":\"09:00-17:00\",\"Sunday\":\"Zatvoreno\"}",
                Amenities = "[\"WiFi\",\"Parking\",\"Klima\",\"Kafa\"]",
                IsActive = true
            },
            new Salon
            {
                Name = "Hair Studio Lejla",
                Description = "Premium frizerski salon za sve vaše potrebe",
                Address = "Maršala Tita 45",
                City = "Sarajevo",
                PostalCode = "71000",
                Country = "Bosna i Hercegovina",
                Phone = "033/789-012",
                Email = "info@lejla.ba",
                Website = "www.lejla.ba",
                Rating = 4.9f,
                ReviewCount = 0,
                IsVerified = true,
                BusinessHours = "{\"Monday\":\"10:00-21:00\",\"Tuesday\":\"10:00-21:00\",\"Wednesday\":\"10:00-21:00\",\"Thursday\":\"10:00-21:00\",\"Friday\":\"10:00-21:00\",\"Saturday\":\"10:00-18:00\",\"Sunday\":\"Zatvoreno\"}",
                Amenities = "[\"WiFi\",\"Parking\",\"Klima\",\"Kafa\",\"Magazine\"]",
                IsActive = true
            }
        };
        await context.Salons.AddRangeAsync(saloni);
        await context.SaveChangesAsync();

        // Kreiraj korisnike (admin, frizeri, klijenti)
        var users = new List<User>
        {
            new User
            {
                Username = "admin",
                Email = "admin@sisapp.ba",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin",
                IsEmailVerified = true,
                IsActive = true
            },
            new User
            {
                Username = "dzanan",
                Email = "dzanan@dzanan.ba",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Dzanan123!"),
                FirstName = "Džanan",
                LastName = "Hodžić",
                Role = "Barber",
                IsEmailVerified = true,
                IsActive = true
            },
            new User
            {
                Username = "lejla",
                Email = "lejla@lejla.ba",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Lejla123!"),
                FirstName = "Lejla",
                LastName = "Kovačević",
                Role = "Barber",
                IsEmailVerified = true,
                IsActive = true
            },
            new User
            {
                Username = "ahmed",
                Email = "ahmed@email.ba",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Ahmed123!"),
                FirstName = "Ahmed",
                LastName = "Hadžić",
                Role = "Customer",
                IsEmailVerified = true,
                IsActive = true
            }
        };
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        // Kreiraj frizere
        var frizeri = new List<Barber>
        {
            new Barber
            {
                UserId = users[1].Id,
                SalonId = saloni[0].Id,
                Bio = "Profesionalni frizer sa 10 godina iskustva",
                Rating = 4.8f,
                ReviewCount = 0,
                IsAvailable = true,
                IsVerified = true,
                Skills = "[\"Muško šišanje\",\"Brada\",\"Farbanje\"]",
                Languages = "[\"Bosanski\",\"Engleski\"]"
            },
            new Barber
            {
                UserId = users[2].Id,
                SalonId = saloni[1].Id,
                Bio = "Specijalist za žensko šišanje i tretmane",
                Rating = 4.9f,
                ReviewCount = 0,
                IsAvailable = true,
                IsVerified = true,
                Skills = "[\"Žensko šišanje\",\"Farbanje\",\"Tretmani\"]",
                Languages = "[\"Bosanski\",\"Engleski\",\"Njemački\"]"
            }
        };
        await context.Barbers.AddRangeAsync(frizeri);
        await context.SaveChangesAsync();

        // Kreiraj klijenta
        var klijent = new Customer
        {
            UserId = users[3].Id,
            Preferences = "{\"favoriteStyles\":[\"Kratko\",\"Moderno\"],\"allergies\":[]}",
            PaymentMethods = "[]"
        };
        await context.Customers.AddAsync(klijent);
        await context.SaveChangesAsync();

        // Kreiraj usluge
        var usluge = new List<Service>
        {
            new Service
            {
                SalonId = saloni[0].Id,
                Name = "Muško šišanje",
                Description = "Profesionalno muško šišanje",
                DurationMinutes = 30,
                Price = 15.00m,
                CategoryId = kategorije[0].Id,
                IsPopular = true,
                DisplayOrder = 1,
                IsActive = true
            },
            new Service
            {
                SalonId = saloni[0].Id,
                Name = "Šišanje brade",
                Description = "Oblikovanje i šišanje brade",
                DurationMinutes = 20,
                Price = 10.00m,
                CategoryId = kategorije[2].Id,
                IsPopular = true,
                DisplayOrder = 2,
                IsActive = true
            },
            new Service
            {
                SalonId = saloni[1].Id,
                Name = "Žensko šišanje",
                Description = "Stilsko žensko šišanje",
                DurationMinutes = 60,
                Price = 25.00m,
                CategoryId = kategorije[1].Id,
                IsPopular = true,
                DisplayOrder = 1,
                IsActive = true
            },
            new Service
            {
                SalonId = saloni[1].Id,
                Name = "Farbanje kose",
                Description = "Profesionalno farbanje kose",
                DurationMinutes = 120,
                Price = 50.00m,
                CategoryId = kategorije[3].Id,
                IsPopular = true,
                DisplayOrder = 2,
                IsActive = true
            }
        };
        await context.Services.AddRangeAsync(usluge);
        await context.SaveChangesAsync();

        // Kreiraj radno vrijeme za frizere
        var radnoVrijeme = new List<WorkingHours>
        {
            new WorkingHours
            {
                BarberId = frizeri[0].Id,
                DayOfWeek = 1, // Ponedjeljak
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(20, 0, 0),
                IsWorking = true,
                IsDefault = true
            },
            new WorkingHours
            {
                BarberId = frizeri[1].Id,
                DayOfWeek = 1, // Ponedjeljak
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(21, 0, 0),
                IsWorking = true,
                IsDefault = true
            }
        };
        await context.WorkingHours.AddRangeAsync(radnoVrijeme);
        await context.SaveChangesAsync();
    }
} 