namespace Api.Contracts;

public class GetMoviesResponse
{
    public class Movie
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
    }
    
    public List<Movie> Movies { get; set; }
}