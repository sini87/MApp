using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    public class UserOp
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns null if login fails, else return UserObject</returns>
        public static User Login(string email, string password)
        {
            ApplicationDBEntities ctx = DbConnection.Instance.DbContext;
            string encryptedPw = CustomEnrypt.Encrypt(password);
            List<User> uList = ctx.User.Where(u => u.Email == email && u.PasswordHash == encryptedPw).ToList();
            if (uList.Count > 0)
            {
                return uList[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="password"></param>
        /// <param name="secretQuestion"></param>
        /// <param name="answer"></param>
        /// <param name="stakeholderDescrip"></param>
        /// <returns>positive user-id if user is created</returns>
        public static bool Register(string email, string firstName, string lastName, string password, string secretQuestion, string answer, string stakeholderDescrip)
        {
            ApplicationDBEntities ctx = DbConnection.Instance.DbContext;
            try
            {
                var user = ctx.User.Create();
                user.Email = email;
                user.FirstName = firstName;
                user.LastName = lastName;
                user.PasswordHash = CustomEnrypt.Encrypt(password);
                user.SecretQuestion = secretQuestion;
                user.Answer = answer;
                user.StakeholderDescription = stakeholderDescrip;
                user = ctx.User.Add(user);
                ctx.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return false;
        }

        /// <summary>
        /// retrieves user by Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static User GetUser(int userId)
        {
            ApplicationDBEntities ctx = DbConnection.Instance.DbContext;
            User user = ctx.User.Find(userId);
            return user;
        }

        /// <summary>
        /// updates user informations
        /// </summary>
        /// <param name="u">User</param>
        public static void UpdateUser(User userUpdate)
        {
            ApplicationDBEntities ctx = DbConnection.Instance.DbContext;
            User user = ctx.User.Find(userUpdate.Id);
            user.Email = userUpdate.Email;
            user.FirstName = userUpdate.FirstName;
            user.LastName = userUpdate.LastName;
            user.SecretQuestion = userUpdate.SecretQuestion;
            user.Answer = userUpdate.Answer;
            if (userUpdate.StakeholderDescription != null && userUpdate.StakeholderDescription.Length == 0)
            {
                user.StakeholderDescription = null;
            }else
            {
                user.StakeholderDescription = userUpdate.StakeholderDescription;
            }
            ctx.SaveChanges();
        }
    }
}
