using System;

namespace DTO
{
    // DTO for transferring person data between layers (DAL ↔ BLL ↔ UI)
    public class PersonDTO
    {
        public int PersonID { get; set; }          // Unique person ID
        public string NationalNo { get; set; }     // National ID
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Gender { get; set; }            // 0 = Male, 1 = Female
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int NationalityCountryID { get; set; }
        public string ImagePath { get; set; }      // Path to profile image
    }
}
