using MApp.DA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MApp.Web.Models
{
    public class PropertyModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static PropertyModel FromEntity(Property property)
        {
            PropertyModel model = new PropertyModel();
            model.Id = property.Id;
            model.Name = property.Name;
            return model;
        }
    }
}