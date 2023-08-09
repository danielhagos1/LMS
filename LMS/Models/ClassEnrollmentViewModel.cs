using LMS.Data;

namespace LMS.Models
{
    public class ClassEnrollmentViewModel
    {
        public Class? Class { get; set; }
        public List<StudentEnrollmentViewModel> Enrollments { get; set; } = new
            List<StudentEnrollmentViewModel>();

    }

}
