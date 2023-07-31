namespace Backend.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public double Point { get; set; }
        public int CommentId { get; set; }
        public Comment? Comment { get; set; }
    }
}
