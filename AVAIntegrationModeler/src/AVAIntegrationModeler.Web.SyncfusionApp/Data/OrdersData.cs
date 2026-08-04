using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Data;

  public class OrderDetails
  {
      public OrderDetails()
      {

      }

      // Group 1
      [Required(ErrorMessage = "First Name is required")]
      public string FirstName { get; set; } = string.Empty; 

      [Required(ErrorMessage = "Last Name is required")]
      public string LastName { get; set; } = string.Empty;

      [Required(ErrorMessage = "Email is required")]
      [EmailAddress]
      public string EmailID { get; set; } = string.Empty;     

      [Required(ErrorMessage = "Order Date is required")]
      public DateTime? OrderDate { get; set; }

      [Required(ErrorMessage = "You must agree to the Terms and Conditions")]
      public bool AcceptTerms { get; set; } = false;

  //Group-2 Shipping Address
  [Required(ErrorMessage = "Shipped Date is required")]
      public DateTime? ShippedDate { get; set; }

      [Required(ErrorMessage = "Country is required")]
      public string Country { get; set; } = string.Empty;

      [Required(ErrorMessage = "City is required")]
      public string City { get; set; } = string.Empty;  

      [Required(ErrorMessage = "Address Line is required")]
      public string AddressLine { get; set; } = string.Empty;

      public string AddressLine2 { get; set; } = string.Empty;
      [Required(ErrorMessage = "Product Name is required")]
      public string ProductName { get; set; } = string.Empty;

      public int Quantity { get; set; } = 1;
  }
