using System.ComponentModel.DataAnnotations;

namespace AdSetIntegrador.Web.Models.Enums
{
    public enum PriceRange
    {
        [Display(Name = "10mil a 50mil")]
        From10To50K = 1,

        [Display(Name = "50mil a 90mil")]
        From50To90K = 2,

        [Display(Name = "+90mil")]
        Above90K = 3
    }
}
