using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LMS.Data
{
    public class ClassMetaData
    {
        [DisplayName("Lecturer")]
        public int? LecturerId { get; set; }

        [DisplayName("Lecturer")]
        public int? CourseId { get; set; }
    }
    [ModelMetadataType(typeof(ClassMetaData))]
    public partial class Class { }

}