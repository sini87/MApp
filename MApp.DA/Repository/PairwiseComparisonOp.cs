using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    public class PairwiseComparisonOp
    {
        public static Dictionary<double,string> Values = new Dictionary<double, string> {
            { 9, "Extremely more important" },
            { 8, "Far more important to extremely more important" },
            { 7, "Far more important" },
            { 6, "Much to far more important" },
            { 5, "Much more important" },
            { 4, "Slightly to much more important" },
            { 3, "Slightly more important" },
            { 2, "Equally or slightly more important" },
            { 1, "Equally important" },
            { 1.0 / 2.0 , "Equally or slightly less important" },
            { 1.0 / 3.0 , "Slightly less important" },
            { 1.0 / 4.0, "Slightly to way less important" },
            { 1.0 / 5.0, "Way less important" },
            { 1.0 / 6.0, "Way to far less important" },
            { 1.0 / 7.0, "Far less important" },
            { 1.0 / 8.0, "Far less important to extremely less important" },
            { 1.0 / 9.0, "Extremely less important" } };

        /// <summary>
        /// returns list of weight pairwise comparisons
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<PairwiseComparisonCC> GetWeightComparison(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            List<PairwiseComparisonCC> list;
            string sqlQuery = "SELECT * FROM PairwiseComparisonCC WHERE UserId = {0} AND CriterionLeft IN (SELECT Id From Criterion Where Issue = {1})";
            list = ctx.Database.SqlQuery<PairwiseComparisonCC>(sqlQuery, userId, issueId).ToList();
            ctx.Dispose();
            return list;
        }

        public static List<PairwiseComparisonAC> GetAlternativeComparison(int issueId, int userId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            List<PairwiseComparisonAC> list;
            string sqlQuery = "SELECT * FROM PairwiseComparisonAC WHERE UserId = {0} AND AlternativeLeft IN (SELECT Id From Alternative Where IssueId = {1})";
            list = ctx.Database.SqlQuery<PairwiseComparisonAC>(sqlQuery, userId, issueId).ToList();
            ctx.Dispose();
            return list;
        }

        /// <summary>
        /// returns true if consistency check OK and save performed
        /// if consistency check faild returns false
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<CriterionWeight> SaveWeightComparison(List<PairwiseComparisonCC> list)
        {
            List<CriterionWeight> criteriaWeights = new List<CriterionWeight>();
            if (list == null || list.Count == 0)
            {
                return criteriaWeights;
            }

            //first check if user has made comparisons
            //if true then delete old comparisons
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            int issueId = ctx.Criterion.Find(list[0].CriterionLeft).Issue;
            int userId = list[0].UserId;
            string sqlQuery = "SELECT COUNT(*) FROM PairwiseComparisonCC WHERE UserId = {0} AND CriterionLeft IN (SELECT Id From Criterion Where Issue = {1})";
            if (ctx.Database.SqlQuery<int>(sqlQuery,userId, issueId).FirstOrDefault() > 0)
            {
                sqlQuery = "DELETE FROM PairwiseComparisonCC WHERE UserId = {0} AND CriterionLeft IN (SELECT Id From Criterion Where Issue = {1})";
                ctx.Database.ExecuteSqlCommand(sqlQuery, userId, issueId);
                sqlQuery = "DELETE FROM CriterionWeight WHERE UserId = {0} AND CriterionId IN (SELECT Id From Criterion Where Issue = {1})";
                ctx.Database.ExecuteSqlCommand(sqlQuery, userId, issueId);
                ctx.SaveChanges();
            }

            sqlQuery = "SELECT Count(*) From Criterion Where Issue = {0}";
            int critCnt = ctx.Database.SqlQuery<int>(sqlQuery,issueId).FirstOrDefault();
            //sort comparisons
            list = list.OrderBy(x => x.CriterionLeft).ThenBy(x => x.CriterionRight).ToList();

            //create reciprocal matrix
            Matrix<double> critMatrix = Matrix<double>.Build.Dense(critCnt, critCnt, 1.0);
            int i = 0;
            int j = 1;
            foreach (var pcc in list)
            {
                if (j == critCnt)
                {
                    i++;
                    j = i + 1;
                }
                critMatrix[i, j] = pcc.Value;
                critMatrix[j, i] = 1.0 / pcc.Value;

                j++;
            }

            //if consistency check ok insert criterion weights
            if (AhpConsistency(critMatrix))
            {
                foreach (var pcc in list)
                {
                    ctx.PairwiseComparisonCC.Add(pcc);
                    ctx.Entry(pcc).State = System.Data.Entity.EntityState.Added;
                }
                ctx.SaveChanges();

                Vector<double> priorityWeightVector = Eigenvector(critMatrix);
                List<Criterion> cList = ctx.Criterion.Where(x => x.Issue == issueId).OrderBy(x => x.Id).ToList();
                CriterionWeight cw;
                i = 0;
                foreach (Criterion c in cList)
                {
                    cw = new CriterionWeight();
                    cw.UserId = userId;
                    cw.CriterionId = c.Id;
                    cw.Weight = priorityWeightVector[i];
                    ctx.CriterionWeight.Add(cw);
                    ctx.Entry(cw).State = System.Data.Entity.EntityState.Added;
                    criteriaWeights.Add(cw);
                    i++;
                }
                ctx.SaveChanges();

                ctx.Dispose();
            }
            return criteriaWeights;
        }

        public static string SaveAlternativeComparison(int issueId, int userId, List<PairwiseComparisonAC> compList)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            List<Criterion> cList = ctx.Criterion.Where(x => x.Issue == issueId).OrderBy(x => x.Id).ToList();
            List<PairwiseComparisonAC> compUnderCrit;
            Matrix<double> altCritCompMatrix;
            List<int> altIds = ctx.Alternative.Where(x => x.IssueId == issueId).Select(x => x.Id).ToList();
            Rating rat;
            int altCnt = altIds.Count();
            int i, j;
            string msg = "success";
            altIds.Sort();

            //first check if user has made comparisons
            //if true then delete old comparisons
            string sqlQuery = "SELECT COUNT(*) FROM PairwiseComparisonAC WHERE UserId = {0} AND AlternativeLeft IN (SELECT Id From Alternative Where IssueId = {1})";
            if (ctx.Database.SqlQuery<int>(sqlQuery, userId, issueId).FirstOrDefault() > 0)
            {
                sqlQuery = "DELETE FROM PairwiseComparisonAC WHERE UserId = {0} AND AlternativeLeft IN (SELECT Id From Alternative Where IssueId = {1})";
                ctx.Database.ExecuteSqlCommand(sqlQuery, userId, issueId);
                sqlQuery = "DELETE FROM Rating WHERE UserId = {0} AND AlternativeId IN (SELECT Id From Alternative Where IssueId = {1})";
                ctx.Database.ExecuteSqlCommand(sqlQuery, userId, issueId);
            }

            foreach (Criterion crit in cList)
            {
                //comparisons of alternatives under criterion
                compUnderCrit = compList.Where(x => x.CriterionId == crit.Id).OrderBy(x => x.AlternativeLeft).ThenBy(x => x.AlternativeRight).ToList();

                //make repripocal matrix
                altCritCompMatrix = Matrix<double>.Build.Dense(altCnt, altCnt, 1.0);
                i = 0;
                j = 1;
                foreach (var pca in compUnderCrit)
                {
                    if (j == altCnt)
                    {
                        i++;
                        j = i + 1;
                    }
                    altCritCompMatrix[i, j] = pca.Value;
                    altCritCompMatrix[j, i] = 1.0 / pca.Value;

                    j++;
                }

                //if consistency OK do inserts
                if (AhpConsistency(altCritCompMatrix)){
                    //insert into pairwise alternative comparison table
                    foreach (var pca in compUnderCrit)
                    {
                        ctx.PairwiseComparisonAC.Add(pca);
                        ctx.Entry(pca).State = System.Data.Entity.EntityState.Added;
                    }

                    //calculate priority vector and insert into Rating table
                    Vector<double> priorityAlternativeVectorUnderCrit = Eigenvector(altCritCompMatrix);
                    i = 0;
                    foreach (int altId in altIds)
                    {
                        rat = new Rating();
                        rat.CriterionId = crit.Id;
                        rat.AlternativeId = altId;
                        rat.UserId = userId;
                        rat.Value = priorityAlternativeVectorUnderCrit[i];
                        ctx.Rating.Add(rat);
                        i++;
                    }
                }
                else
                {
                    msg = "Evaluation inconsistent under criterion " + crit.Name + "!";
                    break;
                }
            }

            ctx.SaveChanges();
            ctx.Dispose();
            return msg;
        }

        public static void CalculateResult(int issueId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            string sqlQuery;
            //calculate assessment sum
            sqlQuery = "(SELECT SUM(SelfAssessmentValue) From AccessRight a WHERE " +
                "IssueId = {0} AND a.UserId in (Select distinct(UserId) From Rating r, " +
                "Criterion c Where r.CriterionId = c.Id AND c.Issue = {0}))";
            double selfAssessmentSum = ctx.Database.SqlQuery<double>(sqlQuery, issueId).FirstOrDefault();
            selfAssessmentSum = selfAssessmentSum / 1.0;

            sqlQuery = "SELECT Id FROM Alternative WHERE IssueId = {0} ORDER BY Id";
            List<int> altIds = ctx.Database.SqlQuery<int>(sqlQuery, issueId).ToList();
            sqlQuery = "SELECT Id FROM Criterion WHERE Issue = {0} ORDER BY Id";
            List<int> critIds = ctx.Database.SqlQuery<int>(sqlQuery, issueId).ToList();

            Matrix<double> resultMatrix = DenseMatrix.Build.Dense(altIds.Count, critIds.Count);

            int i = 0;
            int j = 0;
            foreach(int critId in critIds)
            {
                j = 0;
                foreach(int altId in altIds)
                {
                    sqlQuery = "(Select SUM(r.Value * ar.SelfAssessmentValue / {0}) " +
                        "From Rating r, Criterion c, Issue i, AccessRight ar " +
                        "WHERE r.CriterionId = c.Id AND " +
                        "c.Issue = i.Id AND " +
                        "i.id = {1} AND " +
                        "ar.IssueId = i.Id AND " +
                        "ar.UserId = r.UserId AND " +
                        "c.Id = {2} AND " +
                        "r.AlternativeId = {3})";
                    resultMatrix[j, i] = ctx.Database.SqlQuery<double>(sqlQuery, selfAssessmentSum, issueId, critId, altId).FirstOrDefault();

                    j++;
                }
                i++;
            }

            Vector<double> weightVector = DenseVector.Build.Dense(critIds.Count);
            i = 0;
            foreach(Criterion crit in ctx.Criterion.Where(x => x.Issue == issueId).OrderBy(x => x.Id).ToList())
            {
                weightVector[i] = crit.Weight ?? default(double);
                i++;
            }

            Vector<double> resultVector = resultMatrix.Multiply(weightVector);

            i = 0;
            Alternative alt;
            foreach(double value in resultVector)
            {
                alt = ctx.Alternative.Find(altIds[i]);
                alt.Rating = value;
                ctx.Entry(alt).State = System.Data.Entity.EntityState.Modified;
                i++;
            }

            ctx.SaveChanges();
            ctx.Dispose();
        }

        /// <summary>
        /// returns true if the matrix is consistent
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        private static bool AhpConsistency(Matrix<double> mat)
        {
            //raondom consistency index
            double[] ri = new double[15] { 0.0, 0.0, 0.58, 0.9, 1.12, 1.24, 1.32, 1.41, 1.45, 1.49, 1.51, 1.48, 1.56, 1.57, 1.59 };
            //Console.WriteLine(mat.Evd().EigenValues);
            //Console.WriteLine(mat.Evd().EigenVectors);
            var evals = mat.Evd().EigenValues;
            double res;
            double lambdaMax = 0.0;
            foreach (var x in mat.Evd().EigenValues)
            {
                if (x.IsReal())
                {
                    res = x.Real;
                    if (res > lambdaMax)
                    {
                        lambdaMax = res;
                    }
                }
            }
            double ci = (lambdaMax - Double.Parse(mat.ColumnCount.ToString())) / (Double.Parse(mat.ColumnCount.ToString()) - 1.0);
            double cr = ci / ri[mat.RowCount - 1];
            Console.WriteLine("lambda max:" + lambdaMax);
            Console.WriteLine("consistency index: " + ci);
            Console.WriteLine("consistency ratio: " + cr);
            if (cr <= 0.1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// calculates priority vector on 4 decimal exact
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        static Vector<double> Eigenvector(Matrix<double> mat)
        {
            Vector<double> ev, prevEv;
            bool dec4 = false;
            bool first = true;
            int loops = 0;
            prevEv = mat.RowSums();

            do
            {
                loops++;
                mat = mat.Multiply(mat);
                Vector<double> sum = mat.RowSums();
                ev = DenseVector.Build.Dense(sum.Count);
                for (int i = 0; i < ev.Count; i++)
                {
                    ev[i] = sum[i] / sum.Sum();
                }

                if (!first)
                {
                    bool[] checks = new bool[ev.Count];
                    for (int i = 0; i < ev.Count; i++)
                    {
                        if (ev[i].ToString().PadLeft(6) == prevEv[i].ToString().PadLeft(6))
                        {
                            checks[i] = true;
                        }
                        else
                        {
                            checks[i] = false;
                        }
                    }

                    dec4 = true;
                    foreach (bool b in checks)
                    {
                        if (!b)
                        {
                            dec4 = false;
                            break;
                        }
                    }
                }
                else
                {
                    first = false;
                }

                prevEv = ev;
            } while (!dec4);

            Console.WriteLine("Priority Vector");
            Console.WriteLine(ev);
            Console.WriteLine(loops.ToString());
            return ev;
        }
    }
}
