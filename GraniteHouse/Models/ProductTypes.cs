using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Models
{
    //Make this in models and move to applicationdbcontext to make a database table
    public class ProductTypes
    {
        public int Id { get; set; }

        // You can have numerous validations here
        [Required]
        public string Name { get; set; }
    }
}
