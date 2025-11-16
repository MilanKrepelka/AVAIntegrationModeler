using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Data;

  public class RegistrationDetails
  {
      public RegistrationDetails()
      {

      }

      [Required(ErrorMessage = "Email ID is required.")]
      [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
      public string Email { get; set; } = string.Empty;

  [Required(ErrorMessage = "Password is required.")]
      [DataType(DataType.Password)]
      public string Password { get; set; } = string.Empty;

      [Required(ErrorMessage = "Confirm Password is required.")]
      [Compare("Password", ErrorMessage = "Passwords do not match.")]
      [DataType(DataType.Password)]
      public string ConfirmPassword { get; set; } = string.Empty;

      [Required(ErrorMessage = "First Name is required.")]
      public string FirstName { get; set; } = string.Empty;

      [Required(ErrorMessage = "Last Name is required.")]
      public string LastName { get; set; } = string.Empty;

      [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number.")]
      public string PhoneNumber { get; set; } = string.Empty;

      public string Job { get; set; } = string.Empty;

      [Required(ErrorMessage = "Country is required.")]
      public string Country { get; set; } = string.Empty;

      [Required(ErrorMessage = "State is required.")]
      public string State { get; set; } = string.Empty;

      [Required(ErrorMessage = "City is required.")]
      public string City { get; set; } = string.Empty;

      public string ZipCode { get; set; } = string.Empty; 

      [Required(ErrorMessage = "You must accept the Terms and Conditions.")]
      [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the Terms and Conditions")]
      public bool TermsAndConditions { get; set; }
      public DateTime? DateOfBirth { get; set; }
  }
