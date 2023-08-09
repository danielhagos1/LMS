using LMS.Data;

namespace LMS.Models
{
    public class ClassEnrollmentViewModel
    {
        public Class? Class { get; set; }
        public List<StudentEnrollmentViewModel> Students { get; set; } = new
            List<StudentEnrollmentViewModel>();

    }

}
