using DataAccessLayer;
using DTO;
using System.Data;
using System.Threading.Tasks;

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
        static public Task<DataTable> GetAllUsersDataAsync() => ClsUserData.GetAllUsersDataAsync();

        #region check User Methods
        // Check if a user exists with a given username and password
        static public async Task<bool> UserExistsAsync(string UserName, string Password)
        {
            if (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password))
            {
                UserDTO User = await ClsUserData.FindAsync(UserName, Password);
                return (User != null && User.IsActive); // Also ensure the user is active
            }

            return false;
        }

        // Check if a username already exists
        static public Task<bool> UserExistsAsync(string UserName) => ClsUserData.UserExistsAsync(UserName);

        // Check if a person ID is linked to a user
        static public Task<bool> IsUserAsync(int PersonID) => ClsUserData.IsUserAsync(PersonID);
        #endregion

        #region Find Methods

        private async Task LoadRelatedDataAsync()
        {
            this.PersonInfo = await ClsPerson.FindAsync(this.PersonID);
        }

        // Find user by PersonID
        public static async Task<ClsUser> FindUserByPersonIDAsync(int PersonID)
        {
            UserDTO UserDTO = await ClsUserData.FindUserByPersonIDAsync(PersonID);

            if (UserDTO == null) return null; // Return null if not found

            ClsUser UserObj = new ClsUser(UserDTO);

            await UserObj.LoadRelatedDataAsync();

            return UserObj;
        }

        // Find user by username and password
        public static async Task<ClsUser> FindAsync(string UserName, string Password)
        {
            Password = HashHelper.ComputeHashing(Password);

            UserDTO UserDTO = await ClsUserData.FindAsync(UserName, Password);
            if (UserDTO == null) return null; // Return null if not found

            ClsUser UserObj = new ClsUser(UserDTO);

            await UserObj.LoadRelatedDataAsync();

            return UserObj;
        }

        // Find user by username only
        public static async Task<ClsUser> FindAsync(string UserName)
        {
            UserDTO UserDTO = await ClsUserData.FindAsync(UserName);

            if (UserDTO == null) return null; // Return null if not found

            ClsUser UserObj = new ClsUser(UserDTO);

            await UserObj.LoadRelatedDataAsync();

            return UserObj;
        }

        // Find user by user ID
        public static async Task<ClsUser> FindAsync(int UserID)
        {
            UserDTO UserDTO = await ClsUserData.FindAsync(UserID);

            if (UserDTO == null) return null; // Return null if not found

            ClsUser UserObj = new ClsUser(UserDTO);

            await UserObj.LoadRelatedDataAsync();

            return UserObj;
        }
        #endregion

        #region Add/Update/Delete Methods
        // Delete user by ID
        static public Task<bool> DeleteAsync(int UserID) => ClsUserData.DeleteUserAsync(UserID);

        // Add a new user
        private async Task<bool> AddNewUserAsync()
        {
            this.Password = HashHelper.ComputeHashing(this.Password);
            UserID = await ClsUserData.AddNewUserAsync(MappingToDTO());

            return (UserID != -1); // Return true if added successfully
        }

        // Update an existing user
        private Task<bool> UpdateUserAsync() => ClsUserData.UpdateUserAsync(MappingToDTO());


        private async Task<bool> BusinessRules()
        {
            if (this.Mode == EnMode.AddNew)
            {
                return (!await IsUserAsync(this.PersonID) && !await UserExistsAsync(this.UserName));
            }
            else if (this.Mode == EnMode.Update)
            {
                ClsUser OtherUserWithSameName = await FindAsync(this.UserName);
                return (OtherUserWithSameName == null || this.UserID == OtherUserWithSameName.UserID);
            }
            return false;
        }

        // Save user (AddNew or Update based on Mode)
        public async Task<bool> SaveAsync()
        {
            if (await BusinessRules())
            {
                switch (this.Mode)
                {
                    case EnMode.AddNew:
                        if (await AddNewUserAsync()) { Mode = EnMode.Update; return true; }
                        return false;
                    case EnMode.Update:
                        return await UpdateUserAsync();
                    default:
                        return false;
                }
            }
            return false;
        }

        public async Task<bool> ChangePasswordAsync(string NewPassWord)
        {
            NewPassWord = HashHelper.ComputeHashing(NewPassWord);

            if (NewPassWord != this.Password)
            {
                if (await ClsUserData.ChangePasswordAsync(this.UserID, NewPassWord))
                { this.Password = NewPassWord; return true; }
                else
                { return false; }
            }

            return false;
        }
        #endregion
    }
}