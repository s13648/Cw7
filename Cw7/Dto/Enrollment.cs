using System;

namespace Cw7.Dto
{
    public class Enrollment
    {
        public DateTime StartDate { get; set; }
        public string Semester { get; set; }
        public int IdStudy { get; set; }
        public int IdEnrollment { get; set; }
    }
}
