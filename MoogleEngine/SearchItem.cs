namespace MoogleEngine;

public class SearchItem{
    public SearchItem(string title, string snippet, float score){
        this.Title = title;
        this.Snippet = snippet;
        this.Score = score;
    }

    public string Title { get; set; }

    public string Snippet { get;  set; }

    public float Score { get; set; }
}
