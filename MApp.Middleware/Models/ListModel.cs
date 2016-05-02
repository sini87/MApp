using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MApp.Middleware.Models
{
    abstract public class ListModel<Entity,Model> where Model : IListModel<Entity,Model>
    {

        public List<Entity> ToEntityList(List<Model> modelList)
        {
            List<Entity> list = new List<Entity>();
            foreach(Model m in modelList)
            {
                list.Add(m.ToEntity(m));
            }
            return list;
        }

        public List<Model> ToModelList(List<Entity> entityList, Model model)
        {
            List<Model> list = new List<Model>();
            foreach (Entity entity in entityList){
                list.Add(model.ToModel(entity));
            }
            return list;
        }
    }
}