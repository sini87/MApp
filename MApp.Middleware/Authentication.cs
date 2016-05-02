using MApp.DA;
using MApp.DA.Repository;
using MApp.Middleware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware
{
    public class Authentication
    {
        public Authentication()
        {

        }

        public ProfileModel GetUserProfileModel(int userId)
        {
            ProfileModel model = UserModel.DbEntityToProfilModel(UserOp.GetUser(userId));

            model.AllProperties = PropertyModel.ToModelList(PropertyOp.Properties);
            model.Properties = PropertyModel.ToModelList(PropertyOp.GetUserProperties(userId));
            return model;
        }

        public ProfileModel UpdateProfile(ProfileModel profileModel)
        {
            UserOp.UpdateUser(profileModel.GetUserEntity());
            List<Property> pm = PropertyModel.ToEntityList(profileModel.Properties);
            pm = pm.GroupBy(test => test.Name)
                   .Select(grp => grp.First())
                   .ToList();
            profileModel.Properties = PropertyModel.ToModelList(PropertyOp.AddUserProperties(profileModel.Id, pm));
            return profileModel;
        }

        public UserModel Login(string email, string password)
        {
            User user = UserOp.Login(email, password);
            if (user != null)
            {
                UserModel model = new UserModel();
                model.Id = user.Id;
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Password = user.PasswordHash;
                model.Properties = user.Property.ToList();
                model.SecretQuestion = user.SecretQuestion;
                model.StakeholderDescription = user.StakeholderDescription;
                model.Email = user.Email;
                return model;
            }
            return null;
        }
    }
}
