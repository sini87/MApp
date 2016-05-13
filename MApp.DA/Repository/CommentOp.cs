﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    public class CommentOp
    {
        public static void AddComment(Comment comment)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            comment.DateTime = System.DateTime.Now;
            ctx.Comment.Add(comment);
            ctx.Entry(comment).State = EntityState.Added;
            ctx.SaveChanges();

            ctx.Dispose();
        }

        public static List<Comment> GetAlternativeComments(int issueId, int userId)
        {
            string sql;
            List<Comment> list;
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            using (var dbContextTransaction = ctx.Database.BeginTransaction())
            {
                sql = "Select * from Comment " + 
                    "Where Type in (" + 
                    "Select CONCAT('Alternative', Id) From Alternative Where IssueId = {0})";
                list = ctx.Database.SqlQuery<Comment>(sql,  issueId).ToList();
            }

            ctx.Dispose();

            return list;
        }
    }
}
