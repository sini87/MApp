using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    /// <summary>
    /// handles all db opearations to table InformationRead
    /// </summary>
    public class InformationReadOp
    {
        /// <summary>
        /// marks issue as read for user
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user id</param>
        /// <returns>true if successfull</returns>
        public static bool MarkIssue(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            DbCommand cmd;
            string sql;
            bool marked = false;
            ctx.Database.Connection.Open();

            sql = "select count(*) from InformationRead Where TName Like 'Issue' AND UserId = {0} AND [Read] = 0 AND FK LIKE {1}";

            if (ctx.Database.SqlQuery<int>(sql, userId, issueId).FirstOrDefault() > 0)
            {
                cmd = ctx.Database.Connection.CreateCommand();
                sql = "update appSchema.InformationRead SET [Read] = 1 WHERE TName LIKE 'Issue' and FK LIKE '" + issueId + "' AND UserId = " + userId;
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();

                //decision trustworthiness weighting
                if (ctx.Issue.Find(issueId).Status == "BRAINSTORMING2")
                {
                    //if user has read criteria info and now issue info then mark DT Criteria True
                    sql = "select count(*) from InformationRead Where TName Like 'Criterion' AND UserId = {0} AND [Read] = 0 AND FK IN (SELECT Id From Criterion Where Issue = {1})";
                    if (ctx.Database.SqlQuery<int>(sql, userId, issueId).FirstOrDefault() == 0)
                    {
                        cmd = ctx.Database.Connection.CreateCommand();
                        sql = "update appSchema.InformationRead SET [Read] = 1 WHERE TName LIKE 'DTCritWeight' and FK LIKE '" + issueId + "' AND UserId = " + userId;
                        cmd.CommandText = sql;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }

                //decision trustworthiness evaluation
                if (ctx.Issue.Find(issueId).Status == "EVALUATING")
                {
                    //if user has read criteria, alternatives info and now issue info then mark DT Evaluation True
                    sql = "select count(*) from InformationRead Where UserId = {0} AND [Read] = 0 AND " +
                        "(TName Like 'Criterion' AND FK IN (SELECT Id From Criterion Where Issue = {1}) OR " +
                        "TName Like 'Alternative' AND FK IN (Select Id From Alternative Where IssueId = {1}))";
                    if (ctx.Database.SqlQuery<int>(sql, userId, issueId).FirstOrDefault() == 0)
                    {
                        cmd = ctx.Database.Connection.CreateCommand();
                        sql = "update appSchema.InformationRead SET [Read] = 1 WHERE TName LIKE 'DTEvaluation' and FK LIKE '" + issueId + "' AND UserId = " + userId;
                        cmd.CommandText = sql;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }

                marked = true;
            }
            ctx.Database.Connection.Close();
            ctx.Dispose();
            return marked;
        }

        /// <summary>
        /// marks alternatives as rad for user
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user id</param>
        /// <returns>true if successful</returns>
        public static bool MarkAlternatives(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            DbCommand cmd;
            string sql;
            bool marked = false;
            ctx.Database.Connection.Open();

            sql = "select count(*) from appSchema.InformationRead " +
                "WHERE TName LIKE 'Alternative' AND [Read] = 0 AND CAST(FK AS INT) IN " +
                "(SELECT Id FROM Alternative WHERE IssueId = " + issueId + ") AND UserId=" + userId;

            if (ctx.Database.SqlQuery<int>(sql).FirstOrDefault() > 0)
            {
                cmd = ctx.Database.Connection.CreateCommand();
                sql = "update appSchema.InformationRead SET [Read] = 1 " +
                    "WHERE TName LIKE 'Alternative' AND CAST(FK AS INT) IN " +
                    "(SELECT Id FROM Alternative WHERE IssueId = " + issueId + ") AND UserId=" + userId;
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();
                marked = true;

                //decision trustworthiness evaluation
                if (ctx.Issue.Find(issueId).Status == "EVALUATING")
                {
                    //if user has read criteria, issue info and now alternatives info then mark DT Evaluation True
                    sql = "select count(*) from InformationRead Where UserId = {0} AND[Read] = 0 AND " +
                        "(TName Like 'Criterion' AND FK IN (SELECT Id From Criterion Where Issue = {1}) OR " +
                        "TName Like 'Issue' AND FK LIKE {1})";
                    if (ctx.Database.SqlQuery<int>(sql, userId, issueId).FirstOrDefault() == 0)
                    {
                        cmd = ctx.Database.Connection.CreateCommand();
                        sql = "update appSchema.InformationRead SET [Read] = 1 WHERE TName LIKE 'DTEvaluation' and FK LIKE '" + issueId + "' AND UserId = " + userId;
                        cmd.CommandText = sql;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            
            ctx.Database.Connection.Close();
            ctx.Dispose();

            return marked;
        }

        /// <summary>
        /// marks criteria as read
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user id</param>
        /// <returns>if successful true</returns>
        public static bool MarkCritera(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            DbCommand cmd;
            bool marked;
            string sql;
            ctx.Database.Connection.Open();

            sql = "SELECT count(*) FROM appSchema.InformationRead " +
                "WHERE [Read] = 0 AND TName LIKE 'Criterion' AND CAST(FK AS INT) IN " +
                "(SELECT Id FROM Criterion WHERE Issue = " + issueId + ") AND UserId=" + userId;

            if (ctx.Database.SqlQuery<int>(sql).FirstOrDefault() > 0)
            {
                cmd = ctx.Database.Connection.CreateCommand();
                sql = "update appSchema.InformationRead SET [Read] = 1 " +
                    "WHERE TName LIKE 'Criterion' AND CAST(FK AS INT) IN " +
                    "(SELECT Id FROM Criterion WHERE Issue = " + issueId + ") AND UserId=" + userId;
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();

                //decision trustworthiness
                if (ctx.Issue.Find(issueId).Status == "BRAINSTORMING2")
                {
                    //if user has read Issue and now Criteria then mark DTCriteria as Read
                    sql = "select count(*) from InformationRead Where TName Like 'Issue' AND UserId = {0} AND [Read] = 0 AND FK LIKE {1}";
                    if (ctx.Database.SqlQuery<int>(sql, userId, issueId).FirstOrDefault() == 0)
                    {
                        cmd = ctx.Database.Connection.CreateCommand();
                        sql = "update appSchema.InformationRead SET [Read] = 1 WHERE TName LIKE 'DTCritWeight' and FK LIKE '" + issueId + "' AND UserId = " + userId;
                        cmd.CommandText = sql;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }

                //decision trustworthiness evaluation
                if (ctx.Issue.Find(issueId).Status == "EVALUATING")
                {
                    //if user has read issue, alternatives info and now criteria info then mark DT Evaluation True
                    sql = "select count(*) from InformationRead Where UserId = {0} AND [Read] = 0 AND " +
                        "(TName Like 'Issue' AND FK LIKE {1} OR " +
                        "TName Like 'Alternative' AND FK IN (Select Id From Alternative Where IssueId = {1}))";
                    if (ctx.Database.SqlQuery<int>(sql, userId, issueId).FirstOrDefault() == 0)
                    {
                        cmd = ctx.Database.Connection.CreateCommand();
                        sql = "update appSchema.InformationRead SET [Read] = 1 WHERE TName LIKE 'DTEvaluation' and FK LIKE '" + issueId + "' AND UserId = " + userId;
                        cmd.CommandText = sql;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }

                marked = true;
            }else
            {
                marked = false;
            }

            
            ctx.Database.Connection.Close();
            ctx.Dispose();
            return marked;
        }

        /// <summary>
        /// marks issue comments as read
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user id</param>
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

        /// <summary>
        /// marks alternatives comments for user as read
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user id</param>
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

        /// <summary>
        /// gets a list of unread infos of issue for user
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user id</param>
        /// <returns>list of key value pairs where key string is the information name and value is the count of unread information</returns>
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

        /// <summary>
        /// returns a list of user who have not seen all core information
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public static List<string> GetGroupTrustworthiness(int issueId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            List<string> list = new List<string>();

            string query = "SELECT Distinct(UserId) from InformationRead WHERE [Read] = 0 AND " +
                "((TName Like 'Issue' AND FK Like CAST({0} AS varchar)) OR " +
                "(TName Like 'Alternative' AND FK IN (SELECT CAST(Id as varchar) FROM Alternative WHere IssueId = {0})) OR " +
                "(TName LIKE 'Criterion' AND FK IN (SELECT CAST(Id as varchar) FROM Criterion WHERE Issue = {0}))) And UserId in (SELECT UserId FROM AccessRight Where IssueId = {0} AND [Right] != 'V')";

            var queryResult = ctx.Database.SqlQuery<int>(query, issueId);
            User u;
            List<User> userLIst = ctx.User.ToList();
            foreach(int userId in queryResult)
            {
                u = userLIst.Find(x => x.Id == userId);
                list.Add(u.FirstName + " " + u.LastName);
            }

            ctx.Dispose();
            return list;
        }

        /// <summary>
        /// gets decision trustworthiness
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <returns>list of users names</returns>
        public static List<string> GetDecisionTrustwortiness(int issueId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            List<string> list = new List<string>();
            List<User> userList = ctx.User.ToList();

            string query = "SELECT distinct(un.UserId) FROM" +
                "(SELECT ir.UserId FROM InformationRead ir Where [Read] = 0 AND ir.TName Like 'DTEvaluation' AND FK LIKE {0} and ir.UserId IN " +
                "(SELECT UserId From Rating Where AlternativeId in (Select Id From Alternative Where IssueId = {0})) " +
                "UNION " +
                "SELECT ir.UserId FROM InformationRead ir Where [Read] = 0 AND ir.TName Like 'DTCritWeight' AND FK LIKE {0} and ir.UserId IN " +
                "	(SELECT UserId From CriterionWeight Where CriterionId in (Select Id From Criterion Where Issue = {0}))) un";

            var result = ctx.Database.SqlQuery<int>(query, issueId);

            User u;
            foreach(int userId in result)
            {
                u = userList.Find(x => x.Id == userId);
                list.Add( u.FirstName + ' ' + u.LastName);
            }

            ctx.Dispose();
            return list;
        }

    }
}
