using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    /// <summary>
    /// makes all operations to table property
    /// </summary>
    public class PropertyOp
    {
        /// <summary>
        /// returns all possilb
        /// </summary>
        public static List<Property> Properties
        {
            get
            {
                ApplicationDBEntities ctx = new ApplicationDBEntities();
                List<Property> list = ctx.Property.AsNoTracking().ToList();
                ctx.Dispose();

                return list;
            }
        }

        /// <summary>
        /// returns list of properties for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<Property> GetUserProperties(int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            User u = ctx.User.AsNoTracking().Where(x => x.Id == userId).FirstOrDefault();
            List<Property> list = ctx.Property.AsNoTracking().Where(x => x.User == u).ToList();
            ctx.Dispose();

            return list;
       } 

        /// <summary>
        /// adds properties to user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static List<Property> AddUserProperties(int userId, List<Property> properties)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            using (var dbContextTransaction = ctx.Database.BeginTransaction())
            {
                ctx.Database.ExecuteSqlCommand("delete from [appSchema].[UserProperty] WHERE UserId =" + userId);


                dbContextTransaction.Commit();
            }          

            ctx.SaveChanges();
            User user = UserOp.GetUser(userId);
            var updateProp = ctx.Property.First();
            foreach (Property prop in properties)
            {
                if (prop.Id == -1)
                {
                    updateProp = ctx.Property.Create();
                    updateProp.Name = prop.Name;
                    updateProp = ctx.Property.Add(updateProp);
                    ctx.Entry(updateProp).State = EntityState.Added;
                    ctx.SaveChanges();
                }
                else
                {
                    updateProp = ctx.Property.Find(prop.Id);
                }
                user.Property.Add(updateProp);
                ctx.Entry(user).State = EntityState.Modified;
                ctx.SaveChanges();
            }

            ctx.Dispose();

            return GetUserProperties(userId);
        }
    }
}
