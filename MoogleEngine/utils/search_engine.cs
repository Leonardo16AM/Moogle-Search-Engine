


public class search_engine{
    public model model=new model();

    public search_engine(){
        model.build_from_txts();
    } 



    private vector snippet(vector v,string s, int snippet_length=512){
        

        char[] delimiters = {' ', ',', '.', ':',';', '\t', '\n'};
        string[] ntext=v.full_text.Split(delimiters);

        string wr="";
        int beg=0;
        int pos=0;
        while( pos<255 && pos<v.full_text.Length){
            wr+=ntext[pos]+" ";pos++;
        }

        vector best=model.create_vector_str(wr);
        vector orig=model.create_vector_str(wr);
        vector vquery=model.create_vector_str(s);
        

        double best_ang=best.angle(vquery);
        double dot=best.dot_product(vquery);
        double mod_q=vquery.module();
        double mod_o=orig.module();

        for(int i=0;i<v.full_text.Length && pos<ntext.Length;i++){
            wr=wr.Substring(ntext[beg].Length);
            wr+=ntext[pos];
            beg++;
            pos++;


        }
        best.path=v.path;
        best.angle_with=v.angle_with;
        return best;
    }


    private double op_not_in(vector v,string q){
        string s=v.full_text;
        
        List<string>tnorm=string_utils.normalize_text(s);
        List<string>qnorm=string_utils.normalize_text_with_quotation(q);

        bool del=false;
        for(int i=0;i<qnorm.Count;i++){
            if(qnorm[i]=="!"){
                del=true;
            }else{
                if(del){
                    if(v.wordcount.ContainsKey(qnorm[i])){
                        return 0.0;
                    }
                    del=false;
                }
            }
        }

        return 1.0;
    }

    private double op_obl_in(vector v,string q){
        string s=v.full_text;
        
        List<string>tnorm=string_utils.normalize_text(s);
        List<string>qnorm=string_utils.normalize_text_with_quotation(q);

        bool del=false;
        for(int i=0;i<qnorm.Count;i++){
            if(qnorm[i]=="^"){
                del=true;
            }else{
                if(del){
                    if(!v.wordcount.ContainsKey(qnorm[i])){
                        return 0.0;
                    }
                    del=false;
                }
            }
        }

        return 1.0;
    }

    private double op_more_imp(vector v,string q){
        string s=v.full_text;
        
        List<string>tnorm=string_utils.normalize_text(s);
        List<string>qnorm=string_utils.normalize_text_with_quotation(q);

        bool del=false;
        int cnt=0;
        double ret=1.0;

        string_utils.print_list(qnorm);
        for(int i=0;i<qnorm.Count;i++){
            if(qnorm[i]=="*"){
                del=true;
                cnt++;    
            }else{
                if(del){
                    if( v.wordcount.ContainsKey(qnorm[i]) ){
                        ret*=cnt*(Math.Log(v.wordcount[qnorm[i]])) ;
                    }
                    del=false;
                    cnt=0;
                }
            }
        }
        return ret;
    }

    private double op_near(vector v,string q){
        string s=v.full_text;
        
        List<string>tnorm=string_utils.dif_normalize_text(s);
        List<string>qnorm=string_utils.normalize_text_with_quotation(q);

        double ret=1.0;

        string_utils.print_list(qnorm);
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
                        
                Console.WriteLine(j);
                        min=Math.Min(min,Math.Abs(j-lst2) );
                    }
                    if(tnorm[j]==s2){
                        lst2=j;
                Console.WriteLine(j);
                        min=Math.Min(min,Math.Abs(j-lst1) );
                    }
                }
                Console.WriteLine(s1);
                Console.WriteLine(s2);
                Console.WriteLine(min);
                ret*=1.5/(Math.Log(min)+1);
            }
        }
        Console.WriteLine(ret);
        return ret;
    }

    public double calculate_importance(vector v,string q){
        double orig=(1-(v.angle_with%1))*100;
        orig*=op_not_in(v,q);
        orig*=op_obl_in(v,q);
        orig*=op_more_imp(v,q);
        orig*=op_near(v,q);
        return orig;
    }

    
    List<vector>operators(List<vector>v,string s){
        for(int i=0;i<v.Count;i++){
            v[i].angle_with=calculate_importance(v[i],s);
        }
        v.Sort(delegate(vector a,vector b){if(a.angle_with<b.angle_with)return 1;else return -1;});
        return v;
    } 


    public List<vector>  query(string s,int cant=5,bool fast=false){
        
        if(fast==false){
            List<vector> result= model.naive_search(s);
            for(int i=0;i<result.Count;i++){
                result[i]=snippet(result[i],s);
                Console.WriteLine(result[i].path);
            }


            result=operators(result,s);
            return result;
        }else{    
            List<vector> result= model.naive_search(s);    
            return result;
        }
    }


}