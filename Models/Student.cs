using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Final8Net.Models
{
    public class Student
    {
        [Key]
        public int student_id { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        //public DateTime date_of_birth { get; set; }

        //public string gender { get; set; }

        public string email { get; set; }

        //public string phone_number { get; set; }

        //public string nationality { get; set; }

        //// public DateTime? registration_date { get; set; }

        //public string faculty { get; set; }
        public string password { get; set; }

    }
}
