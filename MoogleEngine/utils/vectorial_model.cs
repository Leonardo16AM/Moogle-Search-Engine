// Code by Leonardo Artiles Montero 2022




public class model{
    public List<string> txt_names =new List<string>(); // txt names 
    
    public List<string> full_texts =new List<string>(); // all procesed texts 
    public List<List<string>> proc_texts =new List<List<string>>(); // all texts 

    public List<string> words =new List<string>(); // all words 
    public List<int> word_df =new List<int>(); // in how many texts the word appears
    public string_map word_index= new string_map(); //index of every word in dict
    
    List<List<int>> frec_on_text =new List<List<int>>(); // frecuency of word on text
    List<List<double>> tf_idf =new List<List<double>>(); // frecuency of word on text


    numci.matrix query_matrix= new numci.matrix();
    numci.vector query_vector= new numci.vector();
    trie suffix_trie=new trie();
    


    public List<double> TFIDF(List<string> wds,List<int> frec){
        List<double>ret=new List<double>();

        for(int i=0;i<wds.Count;i++){
            
            ret.Add(0.0);
            if(!word_index.Contains(wds[i])){   
                continue;
            }else{
                int index=word_index.val(wds[i]);
                int f=frec[i];
                int df=word_df[index];
                
                if(full_texts.Count!=1)
                    ret[i]=((double)f/((double)wds.Count))*Math.Log((double)full_texts.Count/df);
                else
                    ret[i]=((double)f/((double)wds.Count));
            }
        }
        for(int i=0;i<ret.Count;i++){
            if(double.IsNaN(ret[i]))ret[i]=0.0;
        }

        return ret;
    }

    // public vector create_vector(List<string> s){
    //     vector ret=new vector();
    //     for(int i=0;i<s.Count;i++){
    //         if( !ret.wordcount.ContainsKey(s[i]) ){
    //             ret.words.Add(s[i]);
    //             ret.wordcount.Add(s[i],1);
    //         }else{
    //             ret.wordcount[s[i]]++;
    //         }
    //     }

    //     ret=TFIDF(ret);
    //     ret.normalize();
    //     return ret;
    // }


    // public vector create_vector_str(string s){
    //     List<string>norm=string_utils.normalize_text(s);
    //     vector ret=create_vector(norm);
    //     ret.full_text=s;
    //     return ret;
    // }



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

        txt_names=txt_reader.ls("../Content");

        for(int i=0;i<txt_names.Count;i++){
            string text=txt_reader.read(txt_names[i]);  
            full_texts.Add(text);
            proc_texts.Add(string_utils.normalize_text(text));
        }
        

        for(int i=0;i<proc_texts.Count;i++){
            for(int j=0;j<proc_texts[i].Count;j++){
                words.Add(proc_texts[i][j]);
                suffix_trie.insert(proc_texts[i][j]);
            }
        }
        words=string_utils.remove_duplicates(words);

        for(int i=0;i<words.Count;i++){
            word_index.Add(words[i],i);
            word_df.Add(0);
        }

        for(int i=0;i<proc_texts.Count;i++){
            frec_on_text.Add(new List<int>());
            tf_idf.Add(new List<double>());
            for(int j=0;j<words.Count;j++){
                frec_on_text[i].Add(0);
                tf_idf[i].Add(0.0);
            }
            for(int j=0;j<proc_texts[i].Count;j++){
                string s=proc_texts[i][j];
                int index=word_index.val(s);
                if(frec_on_text[i][index]==0 ){
                    word_df[index]++; 
                }
                frec_on_text[i][index]++;
            }
            
            tf_idf[i]=TFIDF(words,frec_on_text[i]);
        }

    }



    public void build_matrix_for_query(string q){
        List<string> normq=string_utils.normalize_text(q);
        string_map wd_frec=new string_map();
        List<string> qwds=string_utils.remove_duplicates(normq);
        
        for(int i=0;i<qwds.Count;i++){  
            wd_frec.Add(qwds[i],1);
        }
        List<int> frec=new List<int>();
        for(int i=0;i<qwds.Count;i++){  
            frec.Add(wd_frec.val(qwds[i]));
        }

        for(int i=0;i<full_texts.Count;i++){
            List<int> txt_f= new List<int>();
            for(int j=0;j<qwds.Count;j++){
                int index=word_index.val(qwds[j]);
                txt_f.Add(0);

                if(!word_index.Contains(qwds[j])){   
                    continue;
                }else{            
                    txt_f[j]+=frec_on_text[i][index];
                }
            }
            query_matrix.Add_list(TFIDF(qwds,txt_f) );
        }
        query_vector.vec=TFIDF(qwds,frec);
    }





    // public void build_from_lstr(string s){

    //     List<string> nq=string_utils.normalize_text(s);
    //     words=string_utils.remove_duplicates(nq);
        
    //     string text=s;  
    //     original_texts.Add(text);
    //     texts.Add(string_utils.normalize_text(text));
    //     vectrs.Add(new vector());
        
        
    //     for(int i=0;i<words.Count;i++){
    //         wordindex.Add(words[i],i);
    //         wordcount.Add(0);
    //     }

    //     for(int i=0;i<texts.Count;i++){
    //         for(int j=0;j<texts[i].Count;j++){
    //             if(vectrs[i].wordcount.ContainsKey(texts[i][j])){
    //                 vectrs[i].wordcount[ texts[i][j] ]++;
    //             }else{
    //                 vectrs[i].wordcount.Add(texts[i][j],0);
    //                 if(wordindex.ContainsKey(texts[i][j]))
    //                     wordcount[wordindex[texts[i][j]]]++;
    //             }   
    //         }
    //     }


    //     for(int i=0;i<texts.Count;i++){
    //         List<string>ntxt= new List<string>();
    //         for(int j=0;j<texts[i].Count;j++){
    //             if(wordindex.ContainsKey(texts[i][j]) ) ntxt.Add(texts[i][j]);
    //         }
    //         vectrs[i]=create_vector(ntxt);  
    //         vectrs[i].full_text=s;
    //     }
    // }



    // public void build_to_query(string s){
    //     matrix=new matrix();
    //     var nq=string_utils.normalize_text(s);
    //     var map=new string_map;
    //     for(){
        
    //     }

    //     words=string_utils.remove_duplicates(nq);
    //     string text=s;  
    //     original_texts.Add(text);
    //     texts.Add(string_utils.normalize_text(text));
        


        
    //     for(int i=0;i<texts.Count;i++){
    //         for(int j=0;j<texts[i].Count;j++){
    //             if(vectrs[i].wordcount.ContainsKey(texts[i][j])){
    //                 vectrs[i].wordcount[ texts[i][j] ]++;
    //             }else{
    //                 vectrs[i].wordcount.Add(texts[i][j],0);
    //                 if(wordindex.ContainsKey(texts[i][j]))
    //                     wordcount[wordindex[texts[i][j]]]++;
    //             }   
    //         }
    //     }


    //     for(int i=0;i<texts.Count;i++){
    //         List<string>ntxt= new List<string>();
    //         for(int j=0;j<texts[i].Count;j++){
    //             if(wordindex.ContainsKey(texts[i][j]) ) ntxt.Add(texts[i][j]);
    //         }
    //         vectrs[i]=create_vector(ntxt);  
    //         vectrs[i].full_text=s;
    //     }
    // }





    // public List<string>fast_prepare_string(string s){
    //     List<string>norm_vector=string_utils.normalize_text(s);
    //     return norm_vector;
    // }


    // public List<string>prepare_string(string s){
    //     List<string>norm_vector=string_utils.normalize_text(s);
    //     List<string>real_list=new List<string>();

    //     for(int i=0;i<norm_vector.Count;i++){
    //         double min_dist=100;
    //         string real=norm_vector[i];
    //         for(int j=0;j<words.Count;j++){
    //             double dist=string_utils.distance(norm_vector[i],words[j]);
    //             if(dist<min_dist){
    //                 real=words[j];
    //                 min_dist=dist;
    //             }
    //         }
    //         if(norm_vector[i].Length<4){
    //             if(min_dist<0.0000001 ){
    //                 real_list.Add(real);
    //             }
    //         }else{
    //             if(min_dist<= 1/(norm_vector[i].Length+3) ){
    //                 real_list.Add(real);
    //             }
    //         }
    //     }
    //     norm_vector=real_list;

    //     List<string>famil=new List<string>();
    //     for(int i=0;i<norm_vector.Count;i++){
    //         famil=string_utils.join_lists(famil,suffix_trie.family_words(norm_vector[i]));
    //     }
    //     norm_vector=famil;
    //     return norm_vector;
    // }


    // public List<vector> naive_search(string s,int cnt=5,bool fast=false){
    //     // List<string>norm_vector=new List<string>();
        
    //     // if(!fast)
    //     //     norm_vector=prepare_string(s);
    //     // else
    //     //     norm_vector=fast_prepare_string(s);
        

    //     // vector v=create_vector(norm_vector);

    //     // for(int i=0;i<vectrs.Count;i++){
    //     //     vectrs[i].angle_with=vectrs[i].angle(v);
    //     // }
        
    //     // vectrs.Sort(delegate(vector a,vector b){if(a.angle_with<b.angle_with)return -1;else return 1;});
        
    //     var ret=new List<vector>();

    //     // for(int i=0;i<Math.Min(vectrs.Count,cnt);i++){
    //     //     ret.Add(vectrs[i]);
    //     // }
    //     return ret;
    // }


    public void print(){
        Console.WriteLine("\nVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV");
        Console.WriteLine("\n============= Printing txt_names ==============");
        string_utils.print_list(txt_names);

        Console.WriteLine("\n============= Printing full_texts ==============");
        string_utils.print_list(full_texts);
        
        Console.WriteLine("\n============= Printing proc_texts ==============");
        for(int i=0;i<proc_texts.Count;i++){
            string_utils.print_list(proc_texts[i]);
            Console.WriteLine("________________________________");
        }

        Console.WriteLine("\n============= Printing words ==============");
        for(int i=0;i<words.Count;i++){
            int index=word_index.val(words[i]);
            Console.WriteLine($" {index } - {words[i]} : { word_df[index] }");
            Console.WriteLine("_________________________________________");
        }
        Console.WriteLine("\n============= Printing frec_words on text ==============");
        for(int j=0;j<full_texts.Count;j++){
            Console.WriteLine(full_texts[j]);
            for(int i=0;i<words.Count;i++){
                int index=word_index.val(words[i]);
                Console.WriteLine($" {words[i]} : { frec_on_text[j][index] }");
                Console.WriteLine("_________________________________________");
            }
        }

        Console.WriteLine("\n============= Printing tf_idf ==============");
        for(int j=0;j<full_texts.Count;j++){
            Console.WriteLine(full_texts[j]);
            for(int i=0;i<words.Count;i++){
                int index=word_index.val(words[i]);
                Console.WriteLine($" {words[i]} : { tf_idf[j][index] }");
                Console.WriteLine("_________________________________________");
            }
        }

        Console.WriteLine("\n============= Printing query_matrix ==============");
        for(int i=0;i<query_vector.vec.Count;i++){
            Console.WriteLine(query_vector.vec[i]);
        }
        for(int j=0;j<full_texts.Count;j++){
            Console.WriteLine(full_texts[j]);
            for(int i=0;i<query_matrix.rows[j].vec.Count;i++){
                Console.WriteLine(query_matrix.rows[j].vec[i]);
            }
        }


        Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
    }


}

