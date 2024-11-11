using CaseStudyAPI.Authentication;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaseStudyAPI.Models
{
    public class FileModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FileType { get; set; }

        [Required]
        public long FileSize { get; set; }

        [Required]
        public byte[] FileData { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }

        [Required]
        public DateTime ModifiedDate { get; set; }
        
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
