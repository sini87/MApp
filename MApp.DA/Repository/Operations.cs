using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    abstract public class Operations
    {
        protected static ApplicationDBEntities Ctx
        {
            get
            {
                return DbConnection.Instance.DbContext;
            }
        }
    }
}
