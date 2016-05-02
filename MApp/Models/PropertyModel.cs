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

        public static PropertyModel ToModel(Property property)
        {
            PropertyModel model = new PropertyModel();
            model.Id = property.Id;
            model.Name = property.Name;
            return model;
        }

        public static Property ToEntity(PropertyModel propertyModel)
        {
            Property p = new Property();
            p.Id = propertyModel.Id;
            p.Name = propertyModel.Name;
            return p;
        }

        public static List<Property> ToEntityList(List<PropertyModel> propertyModelList)
        {
            List<Property> list = new List<Property>();
            foreach(PropertyModel p in propertyModelList)
            {
                list.Add(PropertyModel.ToEntity(p));
            }
            return list;
        }

        public static List<PropertyModel> ToModelList(List<Property> propertyList)
        {
            List<PropertyModel> list = new List<PropertyModel>();
            foreach(Property p in propertyList)
            {
                list.Add(PropertyModel.ToModel(p));
            }
            return list;
        }
    }
}