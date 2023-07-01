using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LMS.Data
{
    public class StudentMetaData
    {
        [DisplayName("First Name")]
        public string FirstName { get; set; } = null!;
        [DisplayName("Last Name")]
        public string LastName { get; set; } = null!;
        
        [DisplayName("Date Of Birth")]
        public DateTime? DateOfBirth { get; set; }
    }
    [ModelMetadataType(typeof(StudentMetaData))]
    public partial class Student { }

}
