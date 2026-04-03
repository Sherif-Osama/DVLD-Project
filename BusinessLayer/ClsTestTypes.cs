using DataAccessLayer;
using DTO;
using System.Data;
using System.Threading.Tasks;

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
                        return EnTestType.VisionTest;
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
        public static Task<DataTable> GetAllTestTypeAsync() => ClsTestTypesData.GetAllTestTypeAsync();

        // Finds a test type by ID
        public static async Task<ClsTestTypes> FindAsync(int TestTypeID)
        {
            TestTypesDTO testTypesDTO = await ClsTestTypesData.FindAsync(TestTypeID);

            if (testTypesDTO == null) return null;

            return new ClsTestTypes(testTypesDTO);
        }

        // Finds a test type by Title
        public static async Task<ClsTestTypes> FindAsync(string Title)
        {
            TestTypesDTO testTypesDTO = await ClsTestTypesData.FindAsync(Title);

            if (testTypesDTO == null) return null;

            return new ClsTestTypes(testTypesDTO);
        }

        // Saves the current object (updates if exists)
        public async Task<bool> SaveAsync()
        {
            // Check if a test type with the same title already exists
            ClsTestTypes testTypes = await FindAsync(this.Title);
            if (testTypes == null || this.TestTypeID == testTypes.TestTypeID)
            {
                // Update the record in database
                return await ClsTestTypesData.UpdateAsync(MappingToDTO());
            }

            // Return false if a duplicate title exists
            return false;
        }
    }
}