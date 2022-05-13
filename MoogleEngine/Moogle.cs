namespace MoogleEngine;



public static class Moogle{
    public static SearchResult Query(string query) {
        Console.WriteLine("===================Making a new search=======================");
        model model= new model();
        model.build_from_txts();
        // model.print();

        List<vector> result= model.naive_search(query);


        string real_query=model.recomendation(query);
        SearchItem[] items = new SearchItem[3] {
            new SearchItem(result[0].path.Substring(11), result[0].full_text, 0.9f),
            new SearchItem(result[1].path.Substring(11), result[1].full_text, 0.5f),
            new SearchItem(result[2].path.Substring(11), result[2].full_text, 0.1f),
        };

        return new SearchResult(items, real_query);
    }
}
