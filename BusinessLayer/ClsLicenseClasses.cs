using DataAccessLayer;
using DTO;
using System.Data;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class ClsLicenseClasses
    {



        public int LicenseClassID { get; private set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public byte MinimumAllowedAge { get; set; }
        public byte DefaultValidityLength { get; set; }
        public float ClassFees { get; set; }

        #region Constructors
        public ClsLicenseClasses()
        {
            this.LicenseClassID = -1;
            this.ClassName = string.Empty;
            this.ClassDescription = string.Empty;
            this.MinimumAllowedAge = 0;
            this.DefaultValidityLength = 0;
            this.ClassFees = 0;
        }

        private ClsLicenseClasses(LicenseClassesDTO LicenseClassesDTO)
        {
            this.LicenseClassID = LicenseClassesDTO.LicenseClassID;
            this.ClassName = LicenseClassesDTO.ClassName;
            this.ClassDescription = LicenseClassesDTO.ClassDescription;
            this.MinimumAllowedAge = LicenseClassesDTO.MinimumAllowedAge;
            this.DefaultValidityLength = LicenseClassesDTO.DefaultValidityLength;
            this.ClassFees = LicenseClassesDTO.ClassFees;
        }
        #endregion

        public static Task<DataTable> GetAllLicenseClassesAsync() => ClsLicenseClassesData.GetAllLicenseClassesAsync();

        #region Find Methods
        public static async Task<ClsLicenseClasses> FindAsync(int LicenseClassID)
        {
            LicenseClassesDTO License = await ClsLicenseClassesData.FindAsync(LicenseClassID);

            if (License == null) return null;

            return new ClsLicenseClasses(License);
        }

        public static async Task<ClsLicenseClasses> FindAsync(string LicenseClassesName)
        {
            LicenseClassesDTO License = await ClsLicenseClassesData.FindAsync(LicenseClassesName);

            if (License == null) return null;

            return new ClsLicenseClasses(License);
        }
        #endregion
    }
}