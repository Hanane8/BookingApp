using Booking.Database.Database;
using Booking.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Booking.Database.DataSeedHelper
{
    public class SeedHelper
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<SeedHelper>>();
            var context = serviceProvider.GetRequiredService<BokningContext>();
            var passwordHasher = new PasswordHasher<User>();

            try
            {
                // Seed Users
                if (!context.Users.Any())
                {
                    var user1 = new User
                    {
                        UserName = "hanane",
                        Email = "Hanane@gmail.com",
                        PasswordHash = passwordHasher.HashPassword(null, "Hanane123!")
                    };

                    var user2 = new User
                    {
                        UserName = "ali",
                        Email = "Ali@gmail.com",
                        PasswordHash = passwordHasher.HashPassword(null, "Ali123!")
                    };

                    context.Users.AddRange(user1, user2);
                    context.SaveChanges();

                    logger.LogInformation("Users seeded successfully.");
                }

                // Seed Concerts and Performances
                if (!context.Concerts.Any())
                {
                    var concert1 = new Concert
                    {
                        Title = "Symphony Night",
                        Description = "A mesmerizing evening with classical symphonies."
                    };

                    var concert2 = new Concert
                    {
                        Title = "Rock Legends",
                        Description = "A thrilling rock experience featuring legendary bands."
                    };

                    context.Concerts.AddRange(concert1, concert2);
                    context.SaveChanges();

                    logger.LogInformation("Concerts seeded successfully.");

                    // Add Performances for Concert 1
                    var performances1 = new[]
                    {
                        new Performance
                        {
                            ConcertId = concert1.Id,
                            DateTime = DateTime.Now.AddDays(10),
                            Location = "Main Hall"
                        },
                        new Performance
                        {
                            ConcertId = concert1.Id,
                            DateTime = DateTime.Now.AddDays(11),
                            Location = "Downtown Theater"
                        }
                    };

                    // Add Performances for Concert 2
                    var performances2 = new[]
                    {
                        new Performance
                        {
                            ConcertId = concert2.Id,
                            DateTime = DateTime.Now.AddDays(15),
                            Location = "Grand Arena"
                        },
                        new Performance
                        {
                            ConcertId = concert2.Id,
                            DateTime = DateTime.Now.AddDays(16),
                            Location = "Open Air Stage"
                        }
                    };

                    context.Performances.AddRange(performances1.Concat(performances2));
                    context.SaveChanges();

                    logger.LogInformation("Performances seeded successfully.");
                }

                // Seed Bookings
                if (!context.Boknings.Any())
                {
                    var performance = context.Performances.FirstOrDefault();

                    var user1 = context.Users.FirstOrDefault(u => u.Email == "Hanane@gmail.com");
                    var user2 = context.Users.FirstOrDefault(u => u.Email == "Ali@gmail.com");

                    if (performance != null && user1 != null && user2 != null)
                    {
                        var boknings = new[]
                        {
                            new Bokning
                            {
                                CustomerName = user1.UserName, 
                                CustomerEmail = user1.Email,  
                                PerformanceId = performance.Id,
                                UserId = user1.Id,           
                                BookingDate = DateTime.Now
                            },
                            new Bokning
                            {
                                CustomerName = user2.UserName, 
                                CustomerEmail = user2.Email,  
                                PerformanceId = performance.Id,
                                UserId = user2.Id,            
                                BookingDate = DateTime.Now.AddMinutes(-30)
                            }
                        };

                        context.Boknings.AddRange(boknings);
                        context.SaveChanges();

                        logger.LogInformation("Bookings seeded successfully.");
                    }
                    else
                    {
                        logger.LogWarning("No performances or users available to create bookings.");
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}