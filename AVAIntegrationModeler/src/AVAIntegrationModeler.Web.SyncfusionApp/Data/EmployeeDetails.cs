using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Data;

  public class EmployeeDetails
  {
      public EmployeeDetails()
      {

      }

      [Editable(false)]
      [Display(Name = "Employee ID")]
      public int? EmployeeId { get; set; }

      [Required(ErrorMessage = "First Name is required")]
      [Display(Name = "First Name")]
      public string FirstName { get; set; } = string.Empty;

  [Required(ErrorMessage = "Last Name is required")]
      [Display(Name = "Last Name")]
      public string LastName { get; set; } = string.Empty;

      [Required(ErrorMessage = "Date Of Birth is required")]
      [Display(Name = "Date Of Birth")]
      public DateOnly? DateOfBirth { get; set; }

      [Required(ErrorMessage = "Department is required")]
      [Display(Name = "Department")]
      public string Department { get; set; } = string.Empty;

      [Required(ErrorMessage = "Please choose the country")]
      [Display(Name = "Country")]
      public string Country { get; set; } = string.Empty;
  }
