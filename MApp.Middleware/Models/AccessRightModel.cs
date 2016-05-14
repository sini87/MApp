using MApp.DA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware.Models
{
    public class SelfAssessmentHEntry
    {
        public long ChangeDate { get; set; }
        public double SelfAssessmentValue { get; set; }
        public string SelfAssessmentDescr { get; set; }

        public SelfAssessmentHEntry()
        {

        }

        public SelfAssessmentHEntry(DateTime changeDate, double selfAssessmentValue, string selfAssessmentDescr)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0);
            var unixDateTime = (changeDate.Ticks - epoch.Ticks) / TimeSpan.TicksPerMillisecond - Convert.ToInt32(TimeZone.CurrentTimeZone.GetUtcOffset(changeDate).TotalMilliseconds);

            ChangeDate = unixDateTime;
            SelfAssessmentValue = selfAssessmentValue;
            SelfAssessmentDescr = selfAssessmentDescr;
        }
    }
    public class AccessRightModel : ListModel<AccessRight,AccessRightModel>, IListModel<AccessRight,AccessRightModel>
    {
        public int UserId { get; set; }
        public string Right { get; set; }
        public string Name { get; set; }
        public double? SelfAssessmentValue { get; set; }
        public string SelfAssessmentDescr { get; set; }
        public List<SelfAssessmentHEntry> SelfAssessmentHistory { get; set; }

        public AccessRightModel()
        {
            SelfAssessmentValue = 10;
            SelfAssessmentDescr = "";
            SelfAssessmentHistory = new List<SelfAssessmentHEntry>();
        }

        public AccessRightModel(int userId, string right)
        {
            UserId = userId;
            Right = right;
            SelfAssessmentValue = 10;
            SelfAssessmentDescr = "";
            SelfAssessmentHistory = new List<SelfAssessmentHEntry>();
        }

        public AccessRightModel(int userId, string right, string name)
        {
            UserId = userId;
            Right = right;
            Name = name;
            SelfAssessmentValue = 10;
            SelfAssessmentDescr = "";
            SelfAssessmentHistory = new List<SelfAssessmentHEntry>();
        }

        public AccessRight ToEntity(AccessRightModel model)
        {
            AccessRight ar = new AccessRight();
            ar.UserId = model.UserId;
            switch (model.Right)
            {
                case "Owner":
                    ar.Right = "O";
                    break;
                case "Contributor":
                    ar.Right = "C";
                    break;
                case "Viewer":
                    ar.Right = "V";
                    break;
            }

            ar.SelfAssessmentValue = SelfAssessmentValue;
            ar.SelfAssesmentDescr = SelfAssessmentDescr;

            return ar;
        }

        public AccessRightModel ToModel(AccessRight entity)
        {
            AccessRightModel arm = new AccessRightModel(entity.UserId,entity.Right);
            arm.SelfAssessmentDescr = entity.SelfAssesmentDescr;
            arm.Right = entity.Right;
            if (entity.SelfAssessmentValue == null)
                arm.SelfAssessmentValue = 0;
            else
                arm.SelfAssessmentValue = entity.SelfAssessmentValue;
            return arm;
        }
    }
}
