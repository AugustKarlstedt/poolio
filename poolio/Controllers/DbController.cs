using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace poolio.Controllers
{
    public static class DbController
    {
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
                user.Address = updatedAddress;

                return db.SaveChanges() > 0;
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
    }
}