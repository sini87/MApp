using MApp.DA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware.Models
{
    public class AccessRightModel : ListModel<AccessRight,AccessRightModel>, IListModel<AccessRight,AccessRightModel>
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

        public AccessRight ToEntity(AccessRightModel model)
        {
            AccessRight ar = new AccessRight();
            ar.UserId = model.UserId;
            switch (model.Right)
            {
                case "Owner":
                    ar.Right = "O";
                    break;
                case "Contributor":
                    ar.Right = "C";
                    break;
                case "Viewer":
                    ar.Right = "V";
                    break;
            }

            return ar;
        }

        public AccessRightModel ToModel(AccessRight entity)
        {
            AccessRightModel arm = new AccessRightModel(entity.UserId,entity.Right,entity.User.FirstName + " " + entity.User.LastName);
            return arm;
        }
    }
}
