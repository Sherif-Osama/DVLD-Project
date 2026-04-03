using BusinessLayer;
using DVLD.Global_classes;
using DVLD.Properties;
using GlobalClasses;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD
{
    // Form for adding or editing a person
    public partial class AddAndEditPerson : Form
    {
        public delegate void DataBackEvent(int PersonID); // Delegate to notify parent form
        public event DataBackEvent Databake; // Event triggered after save

        public delegate void RefreshFormData();
        public event RefreshFormData RefreshFormDataEvent;
        private ClsPerson Person; // Current person being edited/added
        private int PersonID;
        private enum EnMode { AddNew = 1, Update = 2 }; // Form mode
        EnMode Mode;

        #region Initialization Components

        public AddAndEditPerson()
        {
            InitializeComponent();
            Mode = EnMode.AddNew;
        }

        public AddAndEditPerson(int PersonID) : this()
        {
            this.PersonID = PersonID;
            Mode = EnMode.Update;
        }

        // Load all countries into ComboBox
        private async Task InitializeCountries()
        {
            CbCountries.Items.Clear();
            DataTable Countries = await ClsCountry.GetAllCountriesAsync();
            if (Countries?.Rows.Count > 0)
                CbCountries.Items.AddRange(Countries.AsEnumerable().Select(r => r["CountryName"].ToString()).ToArray());
        }

        // Set max date for 18+ age restriction
        private void InitializeDate()
        {
            dateTimePicker1.MaxDate = DateTime.Now.AddYears(-18);
            dateTimePicker1.MinDate = DateTime.Now.AddYears(-100);
        }
        #endregion

        #region Load UI Data
        // Load form data on load event
        private async void AddAndEditPerson_Load(object sender, EventArgs e)
        {
            await InitializeCountries();
            InitializeDate();
            if (Mode == EnMode.AddNew)
            {
                Showlabel1.Text = "Add New Person";
                CbCountries.SelectedIndex = CbCountries.Items.IndexOf("Egypt");
                linkRemoveImage.Visible = false;
                Person = new ClsPerson();
            }
            else
            {
                await SetDataInForm();
            }
        }

        // Fill form fields with existing person data
        private async Task SetDataInForm()
        {
            Person = await ClsPerson.FindAsync(PersonID);
            Showlabel1.Text = "Update Person";
            if (Person != null)
            {
                dateTimePicker1.Value = Person.DateOfBirth;
                txtFirst.Text = Person.FirstName;
                txtSecond.Text = Person.SecondName;
                txtThird.Text = Person.ThirdName;
                txtLast.Text = Person.LastName;
                LapNA.Text = Person.PersonID.ToString();
                txtNationalNo.Text = Person.NationalNo;
                txtPhone.Text = Person.Phone;
                txtEmail.Text = Person.Email;
                radioButtonMale.Checked = Person.Gender == ClsPerson.EnGender.Male;
                radioButtonFemale.Checked = Person.Gender == ClsPerson.EnGender.Female;
                int Index = CbCountries.Items.IndexOf(Person?.Country?.CountryName);
                CbCountries.SelectedIndex = Index >= 0 ? Index : 0;
                textAddress.Text = Person.Address;

                // Load image or default gender icon
                if (!string.IsNullOrEmpty(Person.ImagePath) && File.Exists(Person.ImagePath))
                    PicPerson.ImageLocation = Person.ImagePath;
                else
                {
                    PicPerson.Image = Person.Gender == 0 ? Properties.Resources.Male_512 : Properties.Resources.Female_512;
                    linkRemoveImage.Visible = false;
                }
            }
            else { Save.Enabled = false; }
        }
        #endregion
        #region Button Click Events
        // Close form
        private void butClose_Click(object sender, EventArgs e) => this.Close();

        // Update picture on male selection
        private void radioButtonMale_CheckedChanged(object sender, EventArgs e) => PicPerson.Image = Resources.Male_512;

        // Update picture on female selection
        private void radioButtonFemale_CheckedChanged(object sender, EventArgs e) => PicPerson.Image = Resources.Female_512;


        private async Task<bool> HandlePersonPicture()
        {
            if (Person.ImagePath != PicPerson.ImageLocation)
            {
                if (!string.IsNullOrEmpty(Person.ImagePath))
                {
                    try
                    { File.Delete(Person.ImagePath); }
                    catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
                }

                if (!string.IsNullOrEmpty(PicPerson.ImageLocation))
                {
                    string SourceImageFile = PicPerson.ImageLocation.ToString();

                    if (UtilitiesClass.CopyToProjectImagesFolder(ref SourceImageFile))
                    {
                        Person.ImagePath = SourceImageFile;
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Error Copying Image File", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            return true;
        }

        // Save person data
        private async void Save_Click(object sender, EventArgs e)
        {
            txtEmail.Tag = "Optional";
            if (ClsValidation.ValidateEmptyTextBoxes(errorProvider1, panel1) && this.ValidateChildren())
            {

                if (!await HandlePersonPicture())
                    return;

                Person.FirstName = txtFirst.Text.Trim();
                Person.SecondName = txtSecond.Text.Trim();
                Person.ThirdName = txtThird.Text.Trim();
                Person.LastName = txtLast.Text.Trim();
                Person.NationalNo = txtNationalNo.Text.Trim();
                Person.Phone = txtPhone.Text.Trim();
                Person.Email = txtEmail.Text.Trim();
                Person.Address = textAddress.Text.Trim();
                Person.Gender = radioButtonFemale.Checked ? ClsPerson.EnGender.Female : ClsPerson.EnGender.Male;
                Person.DateOfBirth = dateTimePicker1.Value;
                int NationalityCountryID = (await ClsCountry.FindAsync(CbCountries.Text))?.CountryID ?? -1;
                Person.NationalityCountryID = NationalityCountryID;

                if (await Person.SaveAsync())
                {
                    LapNA.Text = Person.PersonID.ToString();
                    MessageBox.Show("Saved successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Mode = EnMode.Update;
                    Databake?.Invoke(PersonID); // Notify parent
                    RefreshFormDataEvent?.Invoke(); // Refresh form data
                    this.Close();
                }
                else
                    MessageBox.Show("An error occurred while saving. Try again", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void txtEmail_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtEmail.Text.Trim()))
                return;

            if (!ClsValidation.ValidateEmail(txtEmail.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtEmail, "Invalid Email Address Format!");
            }
            else
            { errorProvider1.SetError(txtEmail, ""); }
        }

        // Validate National Number uniqueness
        private async void txtNationalNo_Validating(object sender, CancelEventArgs e)
        {
            if (Mode == EnMode.AddNew)
            {
                if (await ClsPerson.PersonExistsAsync(txtNationalNo.Text.Trim()))
                {
                    txtNationalNo.Focus();
                    errorProvider1.SetError(txtNationalNo, "This national number is used by another person!");
                    e.Cancel = true;
                    return;
                }
                else
                    errorProvider1.SetError(txtNationalNo, "");
                return;
            }

            if (await ClsPerson.PersonExistsAsync(txtNationalNo.Text.Trim()) && Person.NationalNo != txtNationalNo.Text)
            {
                txtNationalNo.Focus();
                errorProvider1.SetError(txtNationalNo, "This national number is used by another person!");
                e.Cancel = true;
                return;
            }
            else
                errorProvider1.SetError(txtNationalNo, "");
            return;
        }

        private void LinkSetImage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string SelectedFilePath = openFileDialog1.FileName;

                PicPerson.Load(SelectedFilePath);
                linkRemoveImage.Visible = true;
            }
        }

        private void linkRemoveImage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PicPerson.ImageLocation = string.Empty;

            if (radioButtonMale.Checked)
            { PicPerson.Image = Resources.Male_512; }
            else
            { PicPerson.Image = Resources.Female_512; }

            linkRemoveImage.Visible = false;
        }
    }
}