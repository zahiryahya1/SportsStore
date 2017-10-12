using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
    A view model is not part of the domain model. 
    It is just a convenient class for passing data between the controller and the view.
 */
namespace SportsStore.WebUI.Models
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        
        
        public int TotalPages
        {
            get { return (int)Math.Ceiling( (decimal)TotalItems/ItemsPerPage );  }
        }
    }
}