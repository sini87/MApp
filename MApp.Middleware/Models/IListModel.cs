﻿using System.Collections.Generic;

namespace MApp.Middleware.Models
{
    public interface IListModel<Entity, ListModel>
    {
        Entity ToEntity(ListModel model);
        ListModel ToModel(Entity entity);
    }
}