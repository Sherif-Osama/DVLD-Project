using DataAccessLayer;
using DTO;
using System.Data;

namespace BusinessLayer
{
    public class ClsUser
    {
        public enum EnMode { AddNew = 1, Update = 2 }; // Enum to track object mode (new or update)
        public EnMode Mode { get; private set; }
        public int UserID { get; set; }
        public int PersonID { get; set; }
        public ClsPerson PersonInfo { get; private set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }

        #region Constants
        public ClsUser()
        {
            this.UserID = -1;
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.IsActive = false;
            Mode = EnMode.AddNew;
        }

        private ClsUser(UserDTO UserDTO)
        {
            UserID = UserDTO.UserID;
            PersonID = UserDTO.PersonID;
            PersonInfo = ClsPerson.Find(UserDTO.PersonID);
            UserName = UserDTO.UserName;
            Password = UserDTO.Password;
            IsActive = UserDTO.IsActive;
            Mode = EnMode.Update;
        }
        #endregion

        // Convert this object to DTO for DAL operations
        private UserDTO MappingToDTO()
        {
            return new UserDTO()
            {
                UserID = this.UserID,
                PersonID = this.PersonID,
                UserName = this.UserName,
                Password = this.Password,
                IsActive = this.IsActive,
            };
        }

        // Retrieve all users as a DataTable
        static public DataTable GetAllUsersData() => ClsUserData.GetAllUsersData();

        #region check User Methods
        // Check if a user exists with a given username and password
        static public bool UserExists(string UserName, string Password)
        {
            if (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password))
            {
                UserDTO User = ClsUserData.Find(UserName, Password);

                return (User != null && User.IsActive); // Also ensure the user is active
            }
            return false;
        }

        // Check if a username already exists
        static public bool UserExists(string UserName) => ClsUserData.UserExists(UserName);

        // Check if a person ID is linked to a user
        static public bool IsUser(int PersonID) => ClsUserData.IsUser(PersonID);
        #endregion

        #region Find Methods
        // Find user by PersonID
        public static ClsUser FindUserByPersonID(int PersonID)
        {
            UserDTO UserDTO = ClsUserData.FindUserByPersonID(PersonID);

            if (UserDTO == null) return null; // Return null if not found

            return new ClsUser(UserDTO);
        }

        // Find user by username and password
        public static ClsUser Find(string UserName, string Password)
        {
            Password = HashHelper.ComputeHashing(Password);

            UserDTO UserDTO = ClsUserData.Find(UserName, Password);

            if (UserDTO == null) return null; // Return null if not found

            return new ClsUser(UserDTO);
        }

        // Find user by username only
        public static ClsUser Find(string UserName)
        {
            UserDTO UserDTO = ClsUserData.Find(UserName);

            if (UserDTO == null) return null; // Return null if not found

            return new ClsUser(UserDTO);
        }

        // Find user by user ID
        public static ClsUser Find(int UserID)
        {
            UserDTO UserDTO = ClsUserData.Find(UserID);

            if (UserDTO == null) return null; // Return null if not found

            return new ClsUser(UserDTO);
        }
        #endregion

        #region Add/Update/Delete Methods
        // Delete user by ID
        static public bool Delete(int UserID) => ClsUserData.DeleteUser(UserID);

        // Add a new user
        private bool AddNewUser()
        {
            this.Password = HashHelper.ComputeHashing(this.Password);
            UserID = ClsUserData.AddNewUser(MappingToDTO());

            return (UserID != -1); // Return true if added successfully
        }

        // Update an existing user
        private bool UpdateUser() => ClsUserData.UpdateUser(MappingToDTO());


        private bool BusinessRules()
        {
            if (this.Mode == EnMode.AddNew)
            {
                return (!IsUser(this.PersonID) && !UserExists(this.UserName));
            }
            else if (this.Mode == EnMode.Update)
            {
                ClsUser OtherUserWithSameName = Find(this.UserName);

                return (OtherUserWithSameName == null || this.UserID == OtherUserWithSameName.UserID);
            }
            return false;
        }

        // Save user (AddNew or Update based on Mode)
        public bool Save()
        {
            if (BusinessRules())
            {
                switch (this.Mode)
                {
                    case EnMode.AddNew:
                        if (AddNewUser()) { Mode = EnMode.Update; return true; }
                        return false;
                    case EnMode.Update:
                        return UpdateUser();
                    default:
                        return false;
                }
            }
            return false;
        }

        public bool ChangePassword(string NewPassWord)
        {
            NewPassWord = HashHelper.ComputeHashing(NewPassWord);

            if (NewPassWord != this.Password)
            {
                if (ClsUserData.ChangePassword(this.UserID, NewPassWord))
                { this.Password = NewPassWord; return true; }
                else
                { return false; }
            }

            return false;
        }
        #endregion
    }
}