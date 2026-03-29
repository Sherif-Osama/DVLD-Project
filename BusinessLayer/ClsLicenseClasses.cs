using DataAccessLayer;
using DTO;
using System.Data;

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

        public static DataTable GetAllLicenseClassName() => ClsLicenseClassesData.GetAllLicenseClassName();

        #region Find Methods
        public static ClsLicenseClasses Find(int LicenseClassID)
        {
            LicenseClassesDTO License = ClsLicenseClassesData.Find(LicenseClassID);

            if (License == null) return null;

            return new ClsLicenseClasses(License);
        }

        public static ClsLicenseClasses Find(string LicenseClassesName)
        {
            LicenseClassesDTO License = ClsLicenseClassesData.Find(LicenseClassesName);

            if (License == null) return null;

            return new ClsLicenseClasses(License);
        }
        #endregion
    }
}