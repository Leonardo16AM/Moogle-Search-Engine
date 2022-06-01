namespace MoogleEngine;

// Add synonims
// Fix Snippet, some error
// Fix kdtree build
// Make kdtree query
// Separate cases

public static class Moogle{
    public static SearchResult Query(string query,ref search_engine engine) {

        Console.WriteLine("===================Making a new search=======================");
     

        var watch=System.Diagnostics.Stopwatch.StartNew();

        // int number_of_results=5;
        // List<vector> result = engine.query(real_query,number_of_results);

        model text_model=new model();
        text_model.build_from_txts();
        
        string real_query=text_model.recomendation(query);
        text_model.build_matrix_for_query(real_query);
        
        text_model.print();
        Console.WriteLine($"Elapsed Time Miliseconds: { watch.ElapsedMilliseconds } ");

        SearchItem[] items = new SearchItem[0];
        // for(int i=0;i<result.Count;i++){
        //     items[i]=new SearchItem(result[i].path.Substring(11), result[i].full_text, (float)result[i].angle_with );
        // }
        return new SearchResult(items, real_query);
    }
}
