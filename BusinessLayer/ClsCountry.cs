using DataAccessLayer;
using DTO;
using System.Data;

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
        public static DataTable GetAllCountries() => ClsCountryData.GetAllCountries();

        public static ClsCountry Find(int CountryID)
        {
            CountryDTO Country = ClsCountryData.Find(CountryID);

            if (Country == null) { return null; }

            return new ClsCountry(Country);
        }

        public static ClsCountry Find(string CountryName)
        {
            CountryDTO Country = ClsCountryData.Find(CountryName);

            if (Country == null) { return null; }

            return new ClsCountry(Country);
        }
    }
}