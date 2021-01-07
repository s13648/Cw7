using System;
using System.ComponentModel.DataAnnotations;

namespace Cw7.Dto
{
    public class EnrollStudent
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public string Studies { get; set; }

        [Required]
        public string IndexNumber { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
