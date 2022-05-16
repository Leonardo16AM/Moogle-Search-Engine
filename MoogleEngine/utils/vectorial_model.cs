// Code by Leonardo Artiles Montero 2022




public class model{
    public List<string> original_texts =new List<string>(); // all texts 
    public List<List<string>> texts =new List<List<string>>(); // all texts 
    public List<string> words =new List<string>(); // all words 
    public List<string> files =new List<string>(); // txt names 
    public List<int> wordcount =new List<int>(); // in how many texts the word appears
    public Dictionary<string,int> wordindex= new Dictionary<string,int>(); //index of every word in dict
    List<vector>vectrs = new List<vector>(); //separated vectors
    trie suffix_trie=new trie();

    public vector TFIDF(vector vc){
        vector ret =vc;
        for(int i=0;i<words.Count;i++){
            int f=ret.frecuency(words[i]);
            int df=0;
            if(wordindex.ContainsKey(words[i]))
                df=wordcount[wordindex[words[i]]];
            else
                df=words.Count;
            ret.vec.Add(0);
            ret.vec[i]=f*Math.Log(words.Count/df );
        }
        return ret;
    }

    public vector create_vector(List<string> s){
        vector ret=new vector();

        for(int i=0;i<s.Count;i++){
            if( !ret.wordcount.ContainsKey(s[i]) ){
                ret.words.Add(s[i]);
                ret.wordcount.Add(s[i],1);
            }else{
                ret.wordcount[s[i]]++;
            }
        }

        ret=TFIDF(ret);
        return ret;
    }

    public string recomendation(string s){
        string ret="";
        var norm=string_utils.normalize_text_with_quotation(s);

        for(int i=0;i<norm.Count;i++){
            double min_dist=100;
            string real=norm[i];
            for(int j=0;j<words.Count;j++){
                if(string_utils.distance(norm[i],words[j])<min_dist){
                    real=words[j];
                    min_dist=string_utils.distance(norm[i],words[j]);
                }
            }
            if(norm[i].Length==1){
                if(string_utils.is_letter(norm[i][0])){
                    ret+=" +"+real;
                }else{
                    ret+=char.Parse(norm[i]);
                }
            }else{
                if(norm[i].Length!=0)ret+=" "+real;
            }
        }
        return ret;
    }

    public void build_from_txts(){

        files=txt_reader.ls("../Content");

        for(int i=0;i<files.Count;i++){
            string text=txt_reader.read(files[i]);  

            original_texts.Add(text);
            texts.Add(string_utils.normalize_text(text));

            vectrs.Add(new vector());
        }
        
        HashSet<string>word_set = new HashSet<string>();
        for(int i=0;i<texts.Count;i++)
            for(int j=0;j<texts[i].Count;j++){
                word_set.Add(texts[i][j]);    
                suffix_trie.insert(texts[i][j]);
            }                    
            
        string [] wrds=new string[word_set.Count];
        word_set.CopyTo(wrds);
        words=string_utils.to_list(wrds);
        for(int i=0;i<words.Count;i++){
            wordindex[words[i]]=i;
            wordcount.Add(0);
        }

        for(int i=0;i<texts.Count;i++){
            for(int j=0;j<texts[i].Count;j++){
                if(vectrs[i].wordcount.ContainsKey(texts[i][j])){
                    vectrs[i].wordcount[ texts[i][j] ]++;
                }else{
                    vectrs[i].wordcount.Add(texts[i][j],0);
                    wordcount[wordindex[texts[i][j]]]++;
                }   
            }
        }
        for(int i=0;i<texts.Count;i++){
            vectrs[i]=create_vector(texts[i]);  
            string text=txt_reader.read(files[i]);  
            vectrs[i].path=files[i];
            vectrs[i].full_text=text;
        }

        var kdt=new kdtree();
        kdt.build(ref kdt.root,vectrs,0);
        kdt.print(kdt.root);
    }

    public List<vector> naive_search(string s){
        
        List<string>norm_vector=string_utils.normalize_text(s);
    
        for(int i=0;i<norm_vector.Count;i++){
            double min_dist=100;
            string real=norm_vector[i];
            for(int j=0;j<words.Count;j++){
                if(string_utils.distance(norm_vector[i],words[j])<min_dist){
                    real=words[j];
                    min_dist=string_utils.distance(norm_vector[i],words[j]);
                }
            }
            norm_vector[i]=real;
        }

        List<string>famil=new List<string>();
        for(int i=0;i<norm_vector.Count;i++){
            famil=string_utils.join_lists(famil,suffix_trie.family_words(norm_vector[i]));
        }
        norm_vector=famil;


        vector v=create_vector(norm_vector);

        string_utils.print_list(norm_vector);
        for(int i=0;i<v.vec.Count;i++){
            if(v.vec[i]>0.0000001){
                Console.WriteLine($"{words[i]} : {v.vec[i]}");
            }
        }

        for(int i=0;i<vectrs.Count;i++){
            vectrs[i].angle_with=vectrs[i].angle(v);;
        }

        vectrs.Sort(delegate(vector a,vector b){if(a.angle_with<b.angle_with)return -1;else return 1;});
        
        var ret=new List<vector>();
        for(int i=0;i<3;i++){
            ret.Add(vectrs[i]);
        }

        return ret;
    }


    public void print(){
        Console.WriteLine("\n============= Printing model.original_texts ==============");
        for(int i=0;i<original_texts.Count;i++){
            Console.WriteLine(original_texts[i]);
            Console.WriteLine("_________________________________________");
        }

        Console.WriteLine("\n============= Printing model.texts ==============");
        for(int i=0;i<texts.Count;i++){
            for(int j=0;j<texts[i].Count;j++)
                Console.WriteLine(texts[i][j]);
            Console.WriteLine("_________________________________________");
        }

        Console.WriteLine("\n============= Printing model.files ==============");
        for(int i=0;i<files.Count;i++){
            Console.WriteLine(files[i]);
            Console.WriteLine("_________________________________________");
        }

        Console.WriteLine("\n============= Printing model.words ==============");
        for(int i=0;i<words.Count;i++){
            Console.WriteLine($" {wordindex[words[i]] } - {words[i]} : { wordcount[wordindex[words[i]]] }");
            Console.WriteLine("_________________________________________");
        }

        Console.WriteLine("\n============= Printing model.vectrs ==============");
        for(int i=0;i<vectrs.Count;i++){
            for(int j=0;j<vectrs[i].vec.Count;j++){
                Console.WriteLine($" {words[j] }({vectrs[i].vec[j]}) ");
            }
            if(i!=0){
                Console.WriteLine($"\n{vectrs[i].angle(vectrs[i-1]) }");
            }
            Console.WriteLine("\n_________________________________________");
        }
    }


}

