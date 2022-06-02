// Code by Leonardo Artiles Montero 2022

namespace MoogleEngine;


public class search_engine{
    public model model=new model();
   
   
   
    public search_engine(){
        
        var watch=System.Diagnostics.Stopwatch.StartNew();
        Console.WriteLine($" Reading files ...");
        model.build_from_txts();
        Console.WriteLine($"Build Time: { watch.ElapsedMilliseconds } ");
    } 




    private SearchItem quick_snippet(SearchItem v,string s, int snippet_length=200){
        char[] delimiters = {' ', ',', '.', ':',';', '\t', '\n'};
        string[] ntext=v.Snippet.Split(delimiters);

        string wr="";
        int pos=0;
        while( pos<snippet_length && pos<v.Snippet.Length){
            wr+=ntext[pos]+" ";
            pos++;
        }
        v.Snippet=wr;
        return v;
    }

    // private vector snippet(vector v,string s, int snippet_length=200){
    //     char[] delimiters = {' ', ',', '.', ':',';', '\t', '\n'};
    //     string[] ntext=v.full_text.Split(delimiters);

    //     string wr="";
    //     int beg=0;
    //     int pos=0;
    //     while( pos<snippet_length && pos<v.full_text.Length){
    //         wr+=ntext[pos]+" ";
    //         pos++;
    //     }

    //     model text_model=new model();
    //     text_model.build_from_lstr(s);

    //     vector best=text_model.naive_search(wr,1,true)[0];
    //     best.full_text=wr;
        
    //     string ans=wr;
    //     double best_ang=best.angle_with;
    //     if(double.IsNaN(best_ang))best_ang=100;
        

    //     int last=0;
    //     for(int i=0;i<v.full_text.Length && pos<ntext.Length;i++){
    //         wr=wr.Substring(ntext[beg].Length+1);
    //         wr+=" "+ntext[pos];
    //         beg++;
    //         pos++;


    //         if(  pos-last>190 && string_utils.is_mayus(wr[1]) ){
    //             vector newv=text_model.naive_search(wr,1,true)[0];
    //             if( newv.angle_with<best_ang && !double.IsNaN(newv.angle_with) ){
    //                 best_ang=newv.angle_with;
    //                 ans=wr;
    //                 last=pos;
    //             }
    //         }
    //     }
        
    //     best.path=v.path;
    //     best.angle_with=v.angle_with;
    //     best.full_text=ans;
    //     return best;
    // }


    private double op_not_in(List<string> tnorm,List<string> qnorm,string_map word_count){

        bool del=false;
        for(int i=0;i<qnorm.Count;i++){
            if(qnorm[i]=="!"){
                del=true;
            }else{
                if(del){
                    if(word_count.Contains(qnorm[i])){
                        return 0.0;
                    }
                    del=false;
                }
            }
        }

        return 1.0;
    }

    private double op_obl_in(List<string> tnorm,List<string> qnorm,string_map word_count){

        bool del=false;
        for(int i=0;i<qnorm.Count;i++){
            if(qnorm[i]=="^"){
                del=true;
            }else{
                if(del){
                    if(!word_count.Contains(qnorm[i])){
                        return 0.0;
                    }
                    del=false;
                }
            }
        }

        return 1.0;
    }

    private double op_more_imp(List<string> tnorm,List<string> qnorm,string_map word_count){

        bool del=false;
        int cnt=0;
        double ret=1.0;

        for(int i=0;i<qnorm.Count;i++){
            if(qnorm[i]=="*"){
                del=true;
                cnt++;    
            }else{
                if(del){
                    if( word_count.Contains(qnorm[i]) ){
                        ret*=cnt*(Math.Log(word_count.val(qnorm[i]))) ;
                    }
                    del=false;
                    cnt=0;
                }
            }
        }
        return ret;
    }

    private double op_near(List<string> tnorm,List<string> qnorm){

        double ret=1.0;

          for(int i=0;i<qnorm.Count;i++){
            if(qnorm[i]=="~"){
                int min=100000;
                string s1="",s2="";    
                for(int j=i-1;j>=0;j--){
                    if(string_utils.is_letter(qnorm[j][0]) ){s1=qnorm[j];break;}
                }
                for(int j=i+1;j<qnorm.Count;j++){
                    if(string_utils.is_letter(qnorm[j][0]) ){s2=qnorm[j];break;}
                }
                if(s1==s2)continue;
                int lst1=(int)-1e5;
                int lst2=(int)-1e5;
                for(int j=0;j<tnorm.Count;j++){
                    if(tnorm[j]==s1){
                        lst1=j;        
                        min=Math.Min(min,Math.Abs(j-lst2) );
                    }
                    if(tnorm[j]==s2){
                        lst2=j;
                        min=Math.Min(min,Math.Abs(j-lst1) );
                    }
                }
                ret*=1.5/(Math.Log(min)+1);
            }
        }
        return ret;
    }

    public double calculate_importance(SearchItem v,string q){
        List<string> tnorm=string_utils.normalize_text(v.Snippet);
        List<string> qnorm=string_utils.normalize_text_with_quotation(q);
        string_map word_count=new string_map();
        for(int i=0;i<tnorm.Count;i++){
            word_count.Add(tnorm[i],1);
        }
        
        double orig=v.Score;
        orig*=op_not_in(tnorm,qnorm,word_count);
        orig*=op_obl_in(tnorm,qnorm,word_count);
        orig*=op_more_imp(tnorm,qnorm,word_count);
        orig*=op_near(tnorm,qnorm);
        return orig;
    }

    
    List<SearchItem> operators(List<SearchItem> v,string s){
        for(int i=0;i<v.Count;i++){
            v[i].Score=(float)calculate_importance(v[i],s);
        }
        v.Sort(delegate(SearchItem a,SearchItem b){if(a.Score>b.Score)return -1;else return     1;});
        return v;
    } 


    public List<SearchItem>  query(string s,int cant=7,bool fast=false){
        Console.WriteLine($"Words {model.words.Count}  Texts:{model.txt_names.Count}");
        Console.WriteLine(s);
        if(fast==false){
            List<SearchItem> result= model.query(s,cant);
            result=operators(result,s);

            for(int i=0;i<result.Count;i++){
                result[i]=quick_snippet(result[i],s);
            }
            
            List<SearchItem> nres=new List<SearchItem>();
            for(int i=0;i<result.Count;i++){
                if(result[i].Score>0.00000001){
                    Console.WriteLine("Showing:  "+result[i].Title);
                    nres.Add(result[i]);
                }
            }
            return nres;
        }else{    
            List<SearchItem> result= model.query(s,cant);  
            return result;
        }
    }


}