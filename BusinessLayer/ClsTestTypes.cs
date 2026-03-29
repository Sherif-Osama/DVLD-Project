using DataAccessLayer;
using DTO;
using System.Data;

namespace BusinessLayer
{
    public class ClsTestTypes
    {
        // Properties representing TestType data
        public enum EnTestType { VisionTest = 1, WrittenTest = 2, StreetTest = 3 };
        public int TestTypeID { get; set; }
        public EnTestType TestType
        {
            get
            {
                switch (TestTypeID)
                {
                    case 1:
                        return EnTestType.VisionTest;
                    case 2:
                        return EnTestType.WrittenTest;
                    case 3:
                        return EnTestType.StreetTest;
                    default:
                        return 0;
                }
            }
        }
        public string Title { set; get; }
        public string Description { set; get; }
        public float Fees { set; get; }
        
        // Default constructor initializing default values
        public ClsTestTypes()
        {
            TestTypeID = -1;
            Title = string.Empty;
            Description = string.Empty;
            Fees = 0;
        }

        // Converts the current object to a DTO object
        private TestTypesDTO MappingToDTO()
        {
            return new TestTypesDTO
            {
                TestTypeID = this.TestTypeID,
                Title = this.Title,
                Description = this.Description,
                Fees = this.Fees
            };
        }

        // Private constructor that creates an object from a DTO
        private ClsTestTypes(TestTypesDTO TestTypesDTO)
        {
            TestTypeID = TestTypesDTO.TestTypeID;
            Title = TestTypesDTO.Title;
            Description = TestTypesDTO.Description;
            Fees = TestTypesDTO.Fees;
        }

        // Gets all test types from the database
        public static DataTable GetAllTestType() => ClsTestTypesData.GetAllTestType();

        // Finds a test type by ID
        public static ClsTestTypes Find(int TestTypeID)
        {
            TestTypesDTO testTypesDTO = ClsTestTypesData.Find(TestTypeID);

            if (testTypesDTO == null) return null;

            return new ClsTestTypes(testTypesDTO);
        }

        // Finds a test type by Title
        public static ClsTestTypes Find(string Title)
        {
            TestTypesDTO testTypesDTO = ClsTestTypesData.Find(Title);

            if (testTypesDTO == null) return null;

            return new ClsTestTypes(testTypesDTO);
        }

        // Saves the current object (updates if exists)
        public bool Save()
        {
            // Check if a test type with the same title already exists
            ClsTestTypes testTypes = Find(this.Title);
            if (testTypes == null || this.TestTypeID == testTypes.TestTypeID)
            {
                // Update the record in database
                return ClsTestTypesData.Update(MappingToDTO());
            }

            // Return false if a duplicate title exists
            return false;
        }
    }
}