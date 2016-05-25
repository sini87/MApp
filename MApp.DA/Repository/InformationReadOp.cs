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

        /// <summary>
        /// returns the infomations which a user have not seen yet
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<InformationRead> GetInfosToReadForUser(int issueId, int userId)
        {
            return null;
        }

        /// <summary>
        /// returns the ammounnt of all infomations for an user by issue
        /// </summary>
        /// <returns></returns>
        public static int GetInfosCount(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            string sql = "SELECT count(*) FROM InformationRead " + WhereClauseInfoCount(issueId, userId, ctx);
            int cnt = ctx.Database.SqlQuery<int>(sql).FirstOrDefault();
            ctx.Dispose();
            return cnt;
        }

        /// <summary>
        /// returns the count of unread infomation for user by issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int GetReadInfosCount(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            string sql = "SELECT count(*) FROM InformationRead " + WhereClauseInfoCount(issueId, userId, ctx) + " AND [Read] = 1";
            int cnt = ctx.Database.SqlQuery<int>(sql).FirstOrDefault();
            ctx.Dispose();
            return cnt;
        }

        private static string WhereClauseInfoCount(int issueId, int userId, ApplicationDBEntities ctx)
        {
            string where;
            List<int> altIds;
            List<int> critIds;
            altIds = ctx.Alternative.Where(x => x.IssueId == issueId).Select(x => x.Id).ToList();
            critIds = ctx.Criterion.Where(x => x.Issue == issueId).Select(x => x.Id).ToList();

            where = "WHERE UserId = " + userId +
                " AND (TName LIKE 'Issue' AND FK LIKE '" + issueId + "' OR ";

            if(altIds.Count > 0)
            {
                where = where + "TName LIKE 'Alternative' AND FK IN (";
                for (int i = 0; i < altIds.Count; i++)
                {
                    where = where + "'" + altIds[i].ToString() + "'";
                    if (i + 1 < altIds.Count)
                    {
                        where = where + ",";
                    }
                }
                where = where + ") OR ";
                for (int i = 0; i < altIds.Count; i++)
                {
                    where = where + "TName LIKE 'CommentAlternative" + altIds[i] + "' OR ";
                }
            }
            
            if(critIds.Count > 0)
            {
                where = where + "TName LIKE 'Criterion' AND FK IN (";
                for (int i = 0; i < critIds.Count; i++)
                {
                    where = where + "'" + critIds[i].ToString() + "'";
                    if (i + 1 < critIds.Count)
                    {
                        where = where + ",";
                    }
                }
                where = where + ") OR ";

                for (int i = 0; i < critIds.Count; i++)
                {
                    where = where + "TName LIKE 'CommentCriterion" + critIds[i] + "' OR ";
                }
            }

            where = where + " TName LIKE 'CommentIssue" + issueId + "')";
            return where;
        }

        public static List<KeyValuePair<string, int>> GetUnreadInfos(int issueId, int userId)
        {
            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
            string sql;
            List<int> altIds;
            List<int> critIds;
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            altIds = ctx.Alternative.Where(x => x.IssueId == issueId).Select(x => x.Id).ToList();
            critIds = ctx.Criterion.Where(x => x.Issue == issueId).Select(x => x.Id).ToList();

            sql = "SELECT 'Issue Information' as 'TName', count(*) as 'Sum' FROM InformationRead WHERE UserId = " + userId + " AND TName LIKE 'Issue' AND FK LIKE '" + issueId + "' AND [Read] = 0 UNION ";
            sql = sql + "SELECT 'Issue Comments' as 'TName', count(*) as 'Sum' FROM InformationRead WHERE UserId = " + userId + " AND TName LIKE 'CommentIssue" + issueId + "' AND [Read] = 0 ";
            if (altIds.Count > 0)
            {
                sql = sql + "UNION SELECT 'Alternative Information' as 'TName', count(*) as 'Sum' FROM InformationRead WHERE UserId = " + userId + " AND TName LIKE 'Alternative' AND [Read] = 0 AND FK IN (";
                for (int i = 0; i < altIds.Count; i++)
                {
                    sql = sql + "'" + altIds[i] + "'";
                    if (i + 1 < altIds.Count)
                    {
                        sql = sql + ",";
                    }
                }
                sql = sql + ") UNION ";

                sql = sql + "SELECT 'Alternative Comments' as 'TName', count(*) as 'Sum' FROM InformationRead WHERE UserId = " + userId + " AND [Read] = 0 AND TName IN ( ";
                for (int i = 0; i < altIds.Count; i++)
                {
                    sql = sql + "'CommentAlternative" + altIds[i] + "'";
                    if (i + 1 < altIds.Count)
                    {
                        sql = sql + ",";
                    }
                }
                sql = sql + ") ";
            }
            if(critIds.Count > 0)
            {
                sql = sql + "UNION SELECT 'Criteria Information' as 'TName', count(*) as 'Sum' FROM InformationRead WHERE UserId = " + userId + " AND TName LIKE 'Criterion' AND [Read] = 0 AND FK IN (";
                for (int i = 0; i < critIds.Count; i++)
                {
                    sql = sql + "'" + critIds[i] + "'";
                    if (i + 1 < critIds.Count)
                    {
                        sql = sql + ",";
                    }
                }
                sql = sql + ") UNION ";

                sql = sql + "SELECT 'Criteria Comments' as 'TName', count(*) as 'Sum' FROM InformationRead WHERE UserId = " + userId + " AND [Read] = 0 AND TName IN ( ";
                for (int i = 0; i < critIds.Count; i++)
                {
                    sql = sql + "'CommentCriterion" + critIds[i].ToString() + "'";
                    if (i + 1 < critIds.Count)
                    {
                        sql = sql + ",";
                    }
                }
                sql = sql + ")";
            }
            

            DbCommand cmd = ctx.Database.Connection.CreateCommand();
            ctx.Database.Connection.Open();
            cmd.CommandText = sql;
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    KeyValuePair<string, int> kvp = new KeyValuePair<string, int>(reader.GetString(0), reader.GetInt32(1));
                    list.Add(kvp);
                }
                reader.Close();
            }

            ctx.Database.Connection.Close();
            ctx.Dispose();
            return list;
        }

    }
}
