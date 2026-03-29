namespace DTO
{
    // DTO for transferring user data between layers (DAL ↔ BLL ↔ UI)
    public class UserDTO
    {
        public int UserID { get; set; }       // Unique user ID
        public int PersonID { get; set; }     // Links to a person record
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }    // true = active, false = inactive
    }
}
