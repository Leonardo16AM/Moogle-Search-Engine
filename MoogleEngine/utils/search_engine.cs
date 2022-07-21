// Code by Leonardo Artiles Montero 2022

namespace MoogleEngine;


public class search_engine{
    public model model=new model();
   
   
   
    public search_engine(){
        
        var watch=System.Diagnostics.Stopwatch.StartNew();
        Console.WriteLine($" Reading files ...");
        if(!model.built){    
            model.build_from_txts();
            model.built=true;
        }
        Console.WriteLine($"Build Time: { watch.ElapsedMilliseconds } ");
    } 




    private SearchItem quick_snippet(SearchItem v,string s, int snippet_length=200){
        char[] delimiters = {' ', ',', '.', ':',';', '\t', '\n'};
        string[] ntext=v.Snippet.Split(delimiters);

        string wr="";
        int pos=0;
        while( pos<snippet_length && pos<ntext.Length){
            wr+=ntext[pos]+" ";
            pos++;
        }
        v.Snippet=wr;
        return v;
    }

    private SearchItem snippet(SearchItem v,string s, int snippet_length=200){
        char[] delimiters = {' ', ',', '.', ':',';', '\t', '\n'};
        string[] ntext=v.Snippet.Split(delimiters);

        string wr="";
        int pos=0;
        while( pos<snippet_length/2 && pos<ntext.Length){
            wr+=ntext[pos]+" ";
            pos++;
        }
        string snippet=wr+" ( ... ) ";



        string[] qtext=s.Split(delimiters);

        int cnt=0;
        Queue<string>qu=new Queue<string>();

        while(pos<ntext.Count() && qu.Count()<snippet_length/2){
            qu.Enqueue(ntext[pos]);
            for(int i=0;i<qtext.Count();i++){
                if(string_utils.is_same( qtext[i],ntext[pos]) ){
                    cnt++;
                }
            }
            pos++;
        }
        int ans=pos;
        int  max=cnt;

        for(int i=pos;i<ntext.Count();i++){
            qu.Enqueue(ntext[pos]);
            bool fg=false;
            for(int j=0;j<qtext.Count();j++){
                if(string_utils.is_same( qtext[j],ntext[i]) ){
                    cnt++;
                }
                if(string_utils.is_same( qtext[j],qu.Peek()) ){
                    fg=true;
                }
            }
            qu.Dequeue();
            if(fg){
                cnt--;
            }

            if(cnt>max){
                ans=i;
                max=cnt;
            }
        }

        for(int i=Math.Max(0,ans-snippet_length/2);i<=Math.Min(ans,ntext.Count()-1);i++){
            snippet=snippet+ntext[i]+" ";    
        }


        v.Snippet=snippet;
        return v;
    }


    private double op_not_in(List<string> tnorm,List<string> qnorm,string_map word_count){
        bool del=false;
        for(int i=0;i<qnorm.Count;i++){
            if(qnorm[i]=="!"){
                del=true;
            }else{
                if(del){
                    if(word_count.Contains(qnorm[i])){
                        return 0;
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


    public SearchItem highlight(SearchItem v,string q){
        List<string>words=string_utils.normalize_text(q);
        string new_snip="";
        string snip=v.Snippet;

        List<string>sep_text=string_utils.text_to_list(snip);

        for(int j=0;j<words.Count;j++){
            for(int i=0;i<sep_text.Count;i++){
                if(sep_text[i].Length>=2){
                    if(string_utils.is_same( sep_text[i],words[j]) ){
                        sep_text[i]="<mark>"+sep_text[i]+"</mark>";
                    }

                }
            }
        }
        for(int i=0;i<sep_text.Count;i++){
            new_snip+=sep_text[i]+" ";
        }

        v.Snippet=new_snip;
        return v;
    }


    public List<SearchItem>  query(string s,int cant=7,bool fast=false){
        Console.WriteLine($"Words {model.words.Count}  Texts:{model.txt_names.Count}");
        Console.WriteLine(s);
        if(fast==false){
            List<SearchItem> result= model.query(s,cant);
            result=operators(result,s);

            for(int i=0;i<result.Count;i++){
                result[i]=snippet(result[i],s);
            }
            
            List<SearchItem> nres=new List<SearchItem>();
            for(int i=0;i<result.Count;i++){
                if(result[i].Score>0.00000001){
                    Console.WriteLine("Showing:  "+result[i].Title);
                    nres.Add(result[i]);
                }
            }
            
            for(int i=0;i<result.Count;i++){
                result[i]=highlight(result[i],s);
            }

            return nres;
        }else{    
            List<SearchItem> result= model.query(s,cant); 
            return result;
        }
    }


}