using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace IdentityAppCourse2022.ViewModels
{
    public class UploadImageViewModel
    {
        [Required]
        [Display(Name = "Image")]
        public IFormFile SpeakerPicture { get; set; }
    }
}
