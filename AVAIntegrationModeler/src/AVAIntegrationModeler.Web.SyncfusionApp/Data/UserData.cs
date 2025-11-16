using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Data;

  public class UserDetails
  {
  public UserDetails()
      {

      }

      [Required]
      public string FirstName { get; set; } = string.Empty;

      [Required]
      public string LastName { get; set; } = string.Empty;

      [Required]
      [EmailAddress]
      public string EmailId { get; set; } = string.Empty;

      [Required]
      public string City { get; set; } = string.Empty;

      [Required]
      public string State { get; set; } = string.Empty; 

      [Required]
      public string ZipCode { get; set; } = string.Empty;
  }
