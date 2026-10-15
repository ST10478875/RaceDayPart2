using Microsoft.EntityFrameworkCore;
using RaceDay.API.models;

namespace RaceDay.API.Data
{//start of namespace
    public static class DbInitializer
    {//start of class
        public static void Initialize(RaceDayDbContext context)
        {//start of method

            // Ensure the database is created
            context.Database.EnsureCreated();

            // Look for any existing users
            if (context.Users.Any())
            {//start of if

                return;  
            }//end of if

            // Seed Test Users
            var organiser = new User
            {
                Username = "LwazzyCoach",
                Email = "organiser@raceday.co.za",
                PasswordHash = "hashed_password_123", 
                Role = "Organiser"
            };

            var participant = new User
            {
                Username = "SowetoRunner",
                Email = "runner@raceday.co.za",
                PasswordHash = "hashed_password_123",
                Role = "Participant"
            };

            context.Users.AddRange(organiser, participant);
            context.SaveChanges();

            // Seed Test Event
            var raceEvent = new Event
            {
                Name = "Soweto Spring Marathon",
                Description = "Annual 10km road running and walking event through Moroka and Dlamini",
                Date = DateTime.UtcNow.AddDays(30),
                Location = "Vilakazi Street, Soweto",
                Distance = 10.0m,
                EventType = "Run",
                OrganiserId = organiser.Id
            };

            context.Events.Add(raceEvent);
            context.SaveChanges();

            //Seed Test Categories
            var category = new Category
            {
                Name = "Senior Open (10km)",
                EventId = raceEvent.Id
            };

            context.Categories.Add(category);
            context.SaveChanges();

            // Seed Test Enrolment
            var enrolment = new Enrolment
            {
                UserId = participant.Id,
                EventId = raceEvent.Id,
                CategoryId = category.Id,
                EnrolmentDate = DateTime.UtcNow
            };

            context.Enrolments.Add(enrolment);
            context.SaveChanges();

        }//end of method
    }//end of class
}//end of namespace