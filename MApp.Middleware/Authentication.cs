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
    /// <summary>
    /// middleware class for authentication controller
    /// </summary>
    public class Authentication
    {
        public Authentication()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns>user profile model</returns>
        public ProfileModel GetUserProfileModel(int userId)
        {
            ProfileModel model = UserModel.DbEntityToProfilModel(UserOp.GetUser(userId));

            model.AllProperties = PropertyModel.ToModelList(PropertyOp.Properties);
            model.Properties = PropertyModel.ToModelList(PropertyOp.GetUserProperties(userId));
            return model;
        }

        /// <summary>
        /// updates user profile
        /// </summary>
        /// <param name="profileModel">updated user prifile</param>
        /// <returns>updated profile model</returns>
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

        /// <summary>
        /// performs login
        /// </summary>
        /// <param name="email">user e-mail adress</param>
        /// <param name="password">user password</param>
        /// <returns>is login is successful then usermodel is returned else null</returns>
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
                model.SecretQuestion = user.SecretQuestion;
                model.StakeholderDescription = user.StakeholderDescription;
                model.Email = user.Email;
                return model;
            }
            return null;
        }
    }
}
