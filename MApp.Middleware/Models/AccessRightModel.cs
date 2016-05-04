using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware.Models
{
    public class AccessRightModel
    {
        public int UserId { get; set; }
        public string Right { get; set; }
        public string Name { get; set; }
        public AccessRightModel()
        {

        }

        public AccessRightModel(int userId, string right)
        {
            UserId = userId;
            Right = right;
        }

        public AccessRightModel(int userId, string right, string name)
        {
            UserId = userId;
            Right = right;
            Name = name;
        }
    }
}
