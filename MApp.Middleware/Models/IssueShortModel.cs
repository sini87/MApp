using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware.Models
{
    public class IssueShort
    {
        public IssueShort()
        {

        }

        public IssueShort(int id, string title)
        {
            Id = id;
            Title = title;
        }
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
