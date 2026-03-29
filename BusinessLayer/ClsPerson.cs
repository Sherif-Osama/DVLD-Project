using DataAccessLayer;
using DTO;
using System;
using System.Data;

namespace BusinessLayer
{
    // Business logic class for managing person-related operations
    public class ClsPerson
    {
        // Enum to track whether this object is for adding or updating a person
        public enum EnGender { Male = 0, Female = 1 }
        public enum EnMode { AddNew = 1, Update = 2 };
        public EnMode Mode { get; private set; }
        public int PersonID { get; private set; }
        public string NationalNo { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string LastName { get; set; }
        public string FullName
        { get => $"{FirstName} {SecondName} {ThirdName} {LastName}"; }
        public DateTime DateOfBirth { get; set; }
        public EnGender Gender { set; get; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int NationalityCountryID { get; set; }
        public ClsCountry Country { get; private set; }
        public string ImagePath { get; set; }

        #region Constructors
        // Constructor for loading existing person from DB
        private ClsPerson(PersonDTO PersonInfoDTO)
        {
            this.PersonID = PersonInfoDTO.PersonID;
            this.NationalNo = PersonInfoDTO.NationalNo;
            this.FirstName = PersonInfoDTO.FirstName;
            this.SecondName = PersonInfoDTO.SecondName;
            this.ThirdName = PersonInfoDTO.ThirdName;
            this.LastName = PersonInfoDTO.LastName;
            this.DateOfBirth = PersonInfoDTO.DateOfBirth;
            this.Gender = (EnGender)PersonInfoDTO.Gender;
            this.Address = PersonInfoDTO.Address;
            this.Phone = PersonInfoDTO.Phone;
            this.Email = PersonInfoDTO.Email;
            this.NationalityCountryID = PersonInfoDTO.NationalityCountryID;
            Country = ClsCountry.Find(PersonInfoDTO.NationalityCountryID);
            this.ImagePath = PersonInfoDTO.ImagePath;
            Mode = EnMode.Update;
        }

        // Default constructor for creating a new person
        public ClsPerson()
        {
            PersonID = -1;
            NationalNo = string.Empty;
            FirstName = string.Empty;
            SecondName = string.Empty;
            ThirdName = string.Empty;
            LastName = string.Empty;
            DateOfBirth = DateTime.Now;
            Gender = EnGender.Male;
            Address = string.Empty;
            Phone = string.Empty;
            Email = string.Empty;
            NationalityCountryID = -1;
            ImagePath = string.Empty;
            Mode = EnMode.AddNew;
        }
        #endregion
        // Convert this object to DTO for DAL operations
        private PersonDTO MappingToDTO()
        {
            return new PersonDTO()
            {
                PersonID = this.PersonID,
                NationalNo = this.NationalNo,
                FirstName = this.FirstName,
                SecondName = this.SecondName,
                ThirdName = this.ThirdName,
                LastName = this.LastName,
                DateOfBirth = this.DateOfBirth,
                Gender = (int)this.Gender,
                Address = this.Address,
                Phone = this.Phone,
                Email = this.Email,
                NationalityCountryID = this.NationalityCountryID,
                ImagePath = this.ImagePath,
            };
        }

        // Get all people from DB
        static public DataTable GetAllPeople() => ClsPeopleData.GetAllPeople();

        #region Existence Check Methods
        // Check if a person exists by National Number
        static public bool PersonExists(string NationalNo) => ClsPeopleData.PersonExists(NationalNo);

        // Check if a person exists by ID
        static public bool PersonExists(int ID) => ClsPeopleData.PersonExists(ID);
        #endregion
        #region Find Methods
        // Find person by ID
        static public ClsPerson Find(int PersonID)
        {
            PersonDTO PersonInfoDTO = ClsPeopleData.Find(PersonID);
            if (PersonInfoDTO == null) return null; // Return null if not found

            return new ClsPerson(PersonInfoDTO);
        }

        // Find person by National Number
        static public ClsPerson Find(string NationalNo)
        {
            PersonDTO PersonInfoDTO = ClsPeopleData.Find(NationalNo);
            if (PersonInfoDTO == null) return null; // Return null if not found

            return new ClsPerson(PersonInfoDTO);
        }
        #endregion
        #region Add/Update/Delete Methods

        private bool BusinessRules()
        {
            if (this.Mode == EnMode.AddNew)
            {
                return !PersonExists(this.NationalNo); // Ensure national number is unique
            }
            else if (this.Mode == EnMode.Update)
            {
                PersonDTO Person = ClsPeopleData.Find(this.NationalNo);
                return Person == null || this.PersonID == Person?.PersonID;
            }
            return true;
        }

        // Add a new person to the database
        private bool AddNewPerson()
        {
            PersonID = ClsPeopleData.AddNewPerson(MappingToDTO());

            return PersonID != -1; // Return true if added successfully
        }

        // Update existing person in the database
        private bool UpdatePerson() => ClsPeopleData.UpdatePersonInfo(MappingToDTO());

        // Save the person (insert if new, update if existing)
        public bool Save()
        {
            if (BusinessRules())
            {
                switch (Mode)
                {
                    case EnMode.AddNew:
                        if (AddNewPerson()) { Mode = EnMode.Update; return true; }
                        return false;

                    case EnMode.Update:
                        return UpdatePerson();
                }
            }
            return false;
        }
        public static bool Delete(int PersonID) => ClsPeopleData.Delete(PersonID);
    }
    #endregion
}