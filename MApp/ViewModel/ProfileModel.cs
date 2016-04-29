using MApp.DA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MApp.Web.Models
{
    public class ProfileModel
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
        public List<PropertyModel> Properties {  get; set; }

        public string Name {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public User GetUserEntity()
        {
            User user = new User();
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.PasswordHash = Password;
            user.Answer = Answer;
            user.StakeholderDescription = StakeholderDescription;
            user.SecretQuestion = SecretQuestion;
            user.Email = Email;
            user.Id = Id;
            return user;
        }
    }
}