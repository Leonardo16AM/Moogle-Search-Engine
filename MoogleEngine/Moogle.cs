namespace MoogleEngine;



public static class Moogle{
    public static SearchResult Query(string query,ref search_engine engine) {

        Console.WriteLine("===================Making a new search=======================");
     

        var watch=System.Diagnostics.Stopwatch.StartNew();

        int number_of_results=5;
        string real_query=engine.model.recomendation(query);
        List<vector> result = engine.query(query,number_of_results);

        Console.WriteLine($"Elapsed Time Miliseconds: { watch.ElapsedMilliseconds } ");

        SearchItem[] items = new SearchItem[result.Count];
        for(int i=0;i<result.Count;i++){
            items[i]=new SearchItem(result[i].path.Substring(11), result[i].full_text, (float)result[i].angle_with );
        }
        return new SearchResult(items, real_query);
    }
}
