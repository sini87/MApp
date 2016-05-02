using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    public class PropertyOp
    {
        /// <summary>
        /// returns all possilb
        /// </summary>
        public static List<Property> Properties
        {
            get
            {
                ApplicationDBEntities ctx = DbConnection.Instance.DbContext;
                return ctx.Property.ToList();
            }
        }

        public static DbSet<Property> PropertySet
        {
            get
            {
                return DbConnection.Instance.DbContext.Property;
            }
        }

        public static List<Property> GetUserProperties(int userId)
        {
            return DbConnection.Instance.DbContext.User.Find(userId).Property.ToList();
        }

        public static List<Property> AddUserProperties(int userId, List<Property> properties)
        {
            ApplicationDBEntities ctx = DbConnection.Instance.DbContext;

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
                    ctx.SaveChanges();
                }
                else
                {
                    updateProp = ctx.Property.Find(prop.Id);
                }
                user.Property.Add(updateProp);
                ctx.SaveChanges();
            }
            return user.Property.ToList();
        }
    }
}
