namespace LuckyMushroom.Models
{
    public partial class RequestPhoto
    {
        public uint PhotoId { get; set; }
        public string PhotoFilename { get; set; }
        public uint? RequestId { get; set; }

        public virtual RecognitionRequest Request { get; set; }
    }
}
