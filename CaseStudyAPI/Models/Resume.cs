namespace CaseStudyAPI.Models
{
    public class Resume
    {
        public string ResumeId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public byte[] FileData { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string JobSeekerId { get; set; }

        public virtual JobSeeker? JobSeeker { get; set; }
    }
}
