using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware.Models
{
    public class UserShortModel
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        private string name = null;
        public string Name
        {
            get
            {
                if (name == null)
                {
                    return FirstName + " " + LastName;
                }else
                {
                    return name;
                }
                
            }
        }

        public UserShortModel()
        {

        }

        public UserShortModel(int id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }

        public UserShortModel(int id, string name)
        {
            Id = id;
            this.name = name;
        }
    }
}
