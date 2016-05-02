using MApp.DA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MApp.Middleware.Models
{
    public class TagModel : ListModel<Tag, TagModel>, IListModel<Tag, TagModel>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public TagModel()
        {

        }

        public TagModel(int id, String name)
        {
            Id = id;
            Name = name;
        }

        public Tag ToEntity(TagModel model)
        {
            Tag tag = new Tag();
            tag.Id = model.Id;
            tag.Name = model.Name;
            return tag;
        }

        public TagModel ToModel(Tag entity)
        {
            return new TagModel(entity.Id, entity.Name);
        }
    }
}