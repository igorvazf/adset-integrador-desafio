using System.ComponentModel.DataAnnotations;

namespace AdSetIntegrador.Web.Models.Enums
{
    public enum PhotoFilter
    {
        [Display(Name = "Com Fotos")]
        WithPhotos = 1,

        [Display(Name = "Sem Fotos")]
        WithoutPhotos = 2
    }
}
