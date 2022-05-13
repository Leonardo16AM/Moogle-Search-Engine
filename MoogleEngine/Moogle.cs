namespace MoogleEngine;



public static class Moogle{
    public static SearchResult Query(string query) {

        model model= new model();
        model.build_from_txts();
        model.print();


        string real_query=model.recomendation(query);

        SearchItem[] items = new SearchItem[3] {
            new SearchItem("Hello World", model.naive_search(query), 0.9f),
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.5f),
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.1f),
        };

        return new SearchResult(items, real_query);
    }
}
