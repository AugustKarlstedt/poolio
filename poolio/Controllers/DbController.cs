using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace poolio.Controllers
{
    public static class DbController
    {
        // TODO: Move this into DB and make configurable per-driver
        private const double MaxDistance = 5.0;

        public static string GetFailureMessage(string name)
        {
            using (var db = new Model.PoolioEntities())
            {
                return db.FailureMessages.Where(afm => afm.Name == name).FirstOrDefault().Text;
            }
        }

        public static bool UpdateUserAddress(string username, string updatedAddress)
        {
            using (var db = new Model.PoolioEntities())
            {
                var user = db.Users.Where(u => u.Username == username).FirstOrDefault();
                var latLong = GmapController.GetCoordinates(updatedAddress);

                user.Address = updatedAddress;
                user.Latitude = (float)latLong.Item1;
                user.Longitude = (float)latLong.Item2;

                return db.SaveChanges() > 0 && true;
            }
        }

        public static bool DoesUserExist(string username)
        {
            using (var db = new Model.PoolioEntities())
            {
                return db.Users.Where(u => u.Username == username).Any();
            }
        }

        public static void CreateUser(string username)
        {
            using (var db = new Model.PoolioEntities())
            {
                db.Users.Add(new Model.User()
                {
                    Username = username
                });

                db.SaveChanges();
            }
        }

        public static void BecomeDriver(string username)
        {
            using (var db = new Model.PoolioEntities())
            {
                var userId = db.Users.Where(u => u.Username == username).FirstOrDefault().Id;

                db.Drivers.Add(new Model.Driver()
                {
                    UserId = userId
                });

                db.SaveChanges();
            }
        }

        public static bool IsDriver(string username)
        {
            using (var db = new Model.PoolioEntities())
            {
                return db.Drivers.Where(d => d.User.Username == username).Any();
            }
        }

        public static List<string> GetNearbyDriverUsernames(string username)
        {
            using (var db = new Model.PoolioEntities())
            {
                var rider = db.Users.Where(u => u.Username == username).FirstOrDefault();
                var riderCoords = new Coordinates((double)rider.Latitude, (double)rider.Longitude);

                var nearbyDrivers = new List<string>();

                foreach (var d in db.Drivers.Where(d => d.UserId != rider.Id))
                {
                    if (riderCoords.DistanceTo(new Coordinates((double)d.User.Latitude, (double)d.User.Longitude), UnitOfLength.Miles) <= MaxDistance)
                    {
                        nearbyDrivers.Add(d.User.Username);
                    }
                }

                return nearbyDrivers;
            }
        }

        public static bool AddressPopulated(string username)
        {
            using (var db = new Model.PoolioEntities())
            {
                return db.Users.Where(u => u.Username == username && !string.IsNullOrEmpty(u.Address) && u.Latitude.HasValue && u.Longitude.HasValue).Any();
            }
        }



    }
}