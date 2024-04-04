namespace Api.Contracts;

public class GetMovieResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
}