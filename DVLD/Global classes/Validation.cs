using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DVLD.Global_classes
{
    internal static class ClsValidation
    {
        // Validates all TextBox controls inside a given container (e.g., a Form or Panel)
        // Ensures that required fields are not empty
        public static bool ValidateEmptyTextBoxes(ErrorProvider Error, Control Controls)
        {
            // Clear any previous error messages
            Error.Clear();

            // Loop through all controls inside the given parent control
            foreach (Control control in Controls.Controls)
            {
                // Check if the control is a TextBox and is empty or whitespace
                // Skip validation if the TextBox Tag is marked as "Optional"
                if (control is TextBox text && string.IsNullOrEmpty(text.Text) && text.Tag?.ToString() != "Optional")
                {
                    // Display an error message beside the invalid TextBox
                    Error.SetError(text, "This field is required");

                    // Set focus on the invalid TextBox
                    text.Focus();

                    // Return false since validation failed
                    return false;
                }
            }

            // Return true if all required textboxes are valid
            return true;
        }

        public static bool ValidateEmail(string EmailAddress)
        {
            var pattern = @"^[a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";

            var RegeX = new Regex(pattern);

            return RegeX.IsMatch(EmailAddress);
        }
    }
}