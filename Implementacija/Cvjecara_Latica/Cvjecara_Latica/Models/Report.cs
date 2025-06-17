using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cvjecara_Latica.Models
{
    public class Report
    {
        [Key]
        public int ReportID { get; set; }

        [Display(Name="Report Type")]
        public ReportType ReportType { get; set; }
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Report Content")]
        public string Content { get; set; }

        [ForeignKey("Person")]
        public string PersonID { get; set; }
        
        [ValidateNever]
        public Person Person { get; set; }
        public Report()
        {

        }

    }
}
