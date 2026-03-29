using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace DVLD.Global_classes
{
    internal static class ClsValidation
    {
        // Validates all TextBox controls inside a given container (e.g., a Form or Panel)
        // Ensures that required fields are not empty
        public static bool ValidateTextBoxes(ErrorProvider Error, Control Controls)
        {
            // Clear any previous error messages
            Error.Clear();

            // Loop through all controls inside the given parent control
            foreach (Control control in Controls.Controls)
            {
                // Check if the control is a TextBox and is empty or whitespace
                // Skip validation if the TextBox Tag is marked as "Optional"
                if (control is TextBox text && string.IsNullOrWhiteSpace(text.Text) && text.Tag?.ToString() != "Optional")
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
    }
}
