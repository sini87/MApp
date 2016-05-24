using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    public class InformationReadOp
    {
        public static void MarkIssue(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            DbCommand cmd;
            string sql;
            ctx.Database.Connection.Open();
            
            cmd = ctx.Database.Connection.CreateCommand();
            sql = "update appSchema.InformationRead SET [Read] = 1 WHERE TName LIKE 'Issue' and FK LIKE '" + issueId + "' AND UserId = " + userId;
            cmd.CommandText = sql;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.ExecuteNonQuery();            
            ctx.Database.Connection.Close();
            ctx.Dispose();
        }

        public static void MarkAlternatives(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            DbCommand cmd;
            string sql;
            ctx.Database.Connection.Open();

            cmd = ctx.Database.Connection.CreateCommand();
            sql = "update appSchema.InformationRead SET [Read] = 1 " + 
                "WHERE TName LIKE 'Alternative' AND CAST(FK AS INT) IN " +
                "(SELECT Id FROM Alternative WHERE IssueId = " + issueId + ") AND UserId=" + userId;
            cmd.CommandText = sql;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.ExecuteNonQuery();
            ctx.Database.Connection.Close();
            ctx.Dispose();
        }

        public static void MarkCritera(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            DbCommand cmd;
            string sql;
            ctx.Database.Connection.Open();

            cmd = ctx.Database.Connection.CreateCommand();
            sql = "update appSchema.InformationRead SET [Read] = 1 " +
                "WHERE TName LIKE 'Criterion' AND CAST(FK AS INT) IN " +
                "(SELECT Id FROM Criterion WHERE Issue = " + issueId + ") AND UserId=" + userId;
            cmd.CommandText = sql;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.ExecuteNonQuery();
            ctx.Database.Connection.Close();
            ctx.Dispose();
        }

        public static void MarkIssueComments(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            DbCommand cmd;
            string sql;
            ctx.Database.Connection.Open();

            cmd = ctx.Database.Connection.CreateCommand();
            sql = "update appSchema.InformationRead SET [Read] = 1 " +
                "WHERE TName LIKE 'CommentIssue" + issueId + "' AND UserId=" + userId;
            cmd.CommandText = sql;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.ExecuteNonQuery();
            ctx.Database.Connection.Close();
            ctx.Dispose();
        }

        public static void MarkAlternativeComments(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            ApplicationDBEntities ctx2 = new ApplicationDBEntities();
            DbCommand cmd;
            string sql;
            ctx.Database.Connection.Open();
            foreach (Alternative alt in ctx2.Alternative.Where(x => x.IssueId == issueId))
            {
                cmd = ctx.Database.Connection.CreateCommand();
                sql = "update appSchema.InformationRead SET [Read] = 1 " +
                    "WHERE TName LIKE 'CommentAlternative" + alt.Id + "' AND UserId=" + userId;
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();
            } 
            
            ctx.Database.Connection.Close();
            ctx.Dispose();
        }

        public static void MarkCriteriaComments(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            ApplicationDBEntities ctx2 = new ApplicationDBEntities();
            DbCommand cmd;
            string sql;
            ctx.Database.Connection.Open();
            foreach (Criterion crit in ctx2.Criterion.Where(x => x.Issue == issueId))
            {
                cmd = ctx.Database.Connection.CreateCommand();
                sql = "update appSchema.InformationRead SET [Read] = 1 " +
                    "WHERE TName LIKE 'CommentCriterion" + crit.Id + "' AND UserId=" + userId;
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();
            }

            ctx.Database.Connection.Close();
            ctx.Dispose();
        }

    }
}
