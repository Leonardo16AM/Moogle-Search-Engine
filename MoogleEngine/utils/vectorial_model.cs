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
    kdtree kdt=new kdtree();

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
            df=Math.Max(df,0);
            if(df==0)
                ret.vec[i]=0;
            else
                if(texts.Count!=1)
                    ret.vec[i]=((double)f/((double)vc.words.Count))*Math.Log((double)texts.Count/df);
                else
                    ret.vec[i]=((double)f/((double)vc.words.Count));


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
        ret.normalize();
        return ret;
    }


    public vector create_vector_str(string s){
        List<string>norm=string_utils.normalize_text(s);
        vector ret=create_vector(norm);
        ret.full_text=s;
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
                    ret+="  "+real;
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
        words=string_utils.remove_duplicates(string_utils.to_list(wrds));
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
        // kdt.build(ref kdt.root,vectrs,0);
    }





    public void build_from_lstr(string s){

        List<string> nq=string_utils.normalize_text(s);
        words=string_utils.remove_duplicates(nq);
        
        string text=s;  
        original_texts.Add(text);
        texts.Add(string_utils.normalize_text(text));
        vectrs.Add(new vector());
        
        
        for(int i=0;i<words.Count;i++){
            wordindex.Add(words[i],i);
            wordcount.Add(0);
        }

        for(int i=0;i<texts.Count;i++){
            for(int j=0;j<texts[i].Count;j++){
                if(vectrs[i].wordcount.ContainsKey(texts[i][j])){
                    vectrs[i].wordcount[ texts[i][j] ]++;
                }else{
                    vectrs[i].wordcount.Add(texts[i][j],0);
                    if(wordindex.ContainsKey(texts[i][j]))
                        wordcount[wordindex[texts[i][j]]]++;
                }   
            }
        }


        for(int i=0;i<texts.Count;i++){
            List<string>ntxt= new List<string>();
            for(int j=0;j<texts[i].Count;j++){
                if(wordindex.ContainsKey(texts[i][j]) ) ntxt.Add(texts[i][j]);
            }
            vectrs[i]=create_vector(ntxt);  
            vectrs[i].full_text=s;
        }
    }

    public List<string>fast_prepare_string(string s){
        List<string>norm_vector=string_utils.normalize_text(s);
        return norm_vector;
    }


    public List<string>prepare_string(string s){
        List<string>norm_vector=string_utils.normalize_text(s);
        List<string>real_list=new List<string>();

        for(int i=0;i<norm_vector.Count;i++){
            double min_dist=100;
            string real=norm_vector[i];
            for(int j=0;j<words.Count;j++){
                double dist=string_utils.distance(norm_vector[i],words[j]);
                if(dist<min_dist){
                    real=words[j];
                    min_dist=dist;
                }
            }
            if(norm_vector[i].Length<4){
                if(min_dist<0.0000001 ){
                    real_list.Add(real);
                }
            }else{
                if(min_dist<= 1/norm_vector[i].Length ){
                    real_list.Add(real);
                }
            }
        }
        norm_vector=real_list;

        List<string>famil=new List<string>();
        for(int i=0;i<norm_vector.Count;i++){
            famil=string_utils.join_lists(famil,suffix_trie.family_words(norm_vector[i]));
        }
        norm_vector=famil;
        return norm_vector;
    }


    public List<vector> naive_search(string s,int cnt=5,bool fast=false){
        List<string>norm_vector=new List<string>();
        
        if(!fast)
            norm_vector=prepare_string(s);
        else
            norm_vector=fast_prepare_string(s);
        

        vector v=create_vector(norm_vector);

        for(int i=0;i<vectrs.Count;i++){
            vectrs[i].angle_with=vectrs[i].angle(v);
        }
        
        vectrs.Sort(delegate(vector a,vector b){if(a.angle_with<b.angle_with)return -1;else return 1;});
        
        var ret=new List<vector>();

        for(int i=0;i<Math.Min(vectrs.Count,cnt);i++){
            ret.Add(vectrs[i]);
        }
        return ret;
    }


    public void print(){
        Console.WriteLine("\nVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV");
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
        Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
    }


}

