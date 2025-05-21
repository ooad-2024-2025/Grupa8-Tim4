using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cvjecara_Latica.Models
{
    public class Report
    {
        [Key]
        public int ReportID { get; set; }
        public ReportType ReportType { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("Person")]
        public string EmployeeID { get; set; }
        public Person Person { get; set; }
        public Report()
        {

        }

    }
}
