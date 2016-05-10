using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA
{
    public class DbConnection
    {
        private static DbConnection instance;
        private static ApplicationDBEntities dbContext;

        private DbConnection() { }

        public static DbConnection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DbConnection();
                    dbContext = new ApplicationDBEntities();
                }else
                {
                    dbContext = new ApplicationDBEntities();
                }
                return instance;
            }
        }

        public ApplicationDBEntities DbContext { get { return dbContext; } }

        public void DisposeAndReload()
        {
            dbContext.Dispose();
            dbContext = new ApplicationDBEntities();
        }
    }
}
