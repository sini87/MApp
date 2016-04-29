using System;
using System.Collections.Generic;
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
    }
}
