using DataAccessLayer;
using DTO;
using System.Data;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class ClsCountry
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }

        public ClsCountry()
        {
            CountryID = -1;
            CountryName = string.Empty;
        }

        private ClsCountry(CountryDTO CountryDTO)
        {
            CountryID = CountryDTO.CountryID;
            CountryName = CountryDTO.CountryName;
        }

        // Get all countries from DB
        public static Task<DataTable> GetAllCountriesAsync() => ClsCountryData.GetAllCountriesAsync();

        public static async Task<ClsCountry> FindAsync(int CountryID)
        {
            CountryDTO Country = await ClsCountryData.FindAsync(CountryID);
            if (Country == null) { return null; }

            return new ClsCountry(Country);
        }

        public static async Task<ClsCountry> FindAsync(string CountryName)
        {
            CountryDTO Country = await ClsCountryData.FindAsync(CountryName);

            if (Country == null) { return null; }

            return new ClsCountry(Country);
        }
    }
}