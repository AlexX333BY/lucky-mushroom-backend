namespace LuckyMushroom.Models
{
    public partial class RequestPhoto
    {
        public int PhotoId { get; set; }
        public string PhotoFilename { get; set; }
        public int RequestId { get; set; }

        public virtual RecognitionRequest Request { get; set; }
    }
}
