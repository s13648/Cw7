using System;

namespace Cw7.Dto
{
    public class Student
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public DateTime BirthDate { get; set; }
        
        public string StudyName { get; set; }
        
        public int Semester { get; set; }
        
        public string Password { get; set; }
        
        public string IndexNumber { get; set; }
        public string Salt { get; set; }
    }
}
