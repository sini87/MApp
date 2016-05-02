using MApp.DA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MApp.Web.Models
{
    public class UserModel
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please write down an valid E-Mail Adress")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DisplayName("Sequret question")]
        public string SecretQuestion { get; set; }
        [DisplayName("Answer to Question")]
        public string Answer { get; set; }
        [DisplayName("Stakeholder description")]
        public string StakeholderDescription { get; set; }
        [ScaffoldColumn(false)]
        public List<Property> Properties { get; set; }

        /// <summary>
        /// converts DB entity User to UserModel
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UserModel DbEntityToUserModel (User user)
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

        /// <summary>
        /// converts DB entity User to ProfilModel
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static ProfileModel DbEntityToProfilModel(User user)
        {
            ProfileModel model = new ProfileModel();
            model.Id = user.Id;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Password = user.PasswordHash;
            model.Properties = new List<PropertyModel>();
            foreach (Property p in user.Property.ToList())
            {
                model.Properties.Add(PropertyModel.ToModel(p));
            }
            model.SecretQuestion = user.SecretQuestion;
            model.StakeholderDescription = user.StakeholderDescription;
            model.Email = user.Email;
            model.Answer = user.Answer;
            return model;
        }
    }
}