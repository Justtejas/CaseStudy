using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CaseStudyAPI.Models
{
    public class Resume
    {
        [JsonIgnore]
        public string ResumeId { get; set; }
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
        public string JobSeekerId { get; set; }

        [JsonIgnore]
        public virtual JobSeeker? JobSeeker { get; set; }
    }
}
