﻿using MApp.DA;
using MApp.DA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MApp.Middleware.Models
{
    public class IssueModel : ListModel<Issue, IssueModel>, IListModel<Issue,IssueModel>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Setting { get; set; }
        public bool AnonymousPosting { get; set; }
        public Nullable<int> Parent { get; set; }
        public Nullable<int> DependsOn { get; set; }
        public string GroupThink { get; set; }
        public Nullable<double> ReviewRating { get; set; }
        public string ParentTitle { get; set; }
        public string DependsOnTitle { get; set; }
        public List<IssueModel> Children { get; set; }
        public List<TagModel> Tags { get; set; }

        public IssueModel()
        {

        }

        
        public IssueModel ToModel(Issue entity)
        {
            IssueModel model = new IssueModel();
            model.Id = entity.Id;
            model.Title = entity.Title;
            model.Status = entity.Status;
            model.Description = entity.Description;
            model.Setting = entity.Setting;
            model.Parent = entity.Parent;
            model.DependsOn = entity.DependsOn;
            model.GroupThink = entity.GroupThink;
            model.ReviewRating = entity.ReviewRating;
            if (entity.AnonymousPosting == "false")
            {
                model.AnonymousPosting = false;
            }else
            {
                model.AnonymousPosting = true;
            }
            
            if (model.Parent != null)
            {
                model.ParentTitle = IssueOp.IssueTitle(entity.Parent.Value);
            }
            if (model.DependsOn != null)
            {
                model.ParentTitle = IssueOp.IssueTitle(entity.DependsOn.Value);
            }

            return model;
        }

        public Issue ToEntity(IssueModel model)
        {
            Issue entity = new Issue();
            entity.Id = model.Id;
            entity.Title = model.Title;
            entity.Status = model.Status;
            entity.Description = model.Description;
            entity.Setting = model.Setting;
            entity.Parent = model.Parent;
            entity.DependsOn = model.DependsOn;
            entity.GroupThink = model.GroupThink;
            entity.ReviewRating = model.ReviewRating;
            if (model.AnonymousPosting)
            {
                entity.AnonymousPosting = "true";
            }else
            {
                entity.AnonymousPosting = "false";
            }
            return entity;
        }

        public Issue ToEntity()
        {
            return ToEntity(this);
        }

    }
}