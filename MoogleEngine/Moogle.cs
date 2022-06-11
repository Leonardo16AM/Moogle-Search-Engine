namespace MoogleEngine;

// Add synonims
// Separate cases

// bug on operators

public static class Moogle{
    public static SearchResult Query(string query,ref search_engine engine) {

        Console.WriteLine("===================Making a new search=======================");
        var watch=System.Diagnostics.Stopwatch.StartNew();
        string real_query=engine.model.recomendation(query);
        List<SearchItem> ans=engine.query(real_query);   
        Console.WriteLine($"Elapsed Time Miliseconds: { watch.ElapsedMilliseconds } ");
        
        SearchItem[] items = new SearchItem[ans.Count];
        for(int i=0;i<ans.Count;i++){
            items[i]=ans[i];
            items[i].Title=items[i].Title.Substring(11);
        }
        return new SearchResult(items, real_query);
    }
}


