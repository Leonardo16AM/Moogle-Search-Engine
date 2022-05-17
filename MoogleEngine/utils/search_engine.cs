


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

        // for(int i=0;i<v.full_text.Length && pos<ntext.Length;i++){
        //     wr=wr.Substring(ntext[beg].Length);
        //     wr+=ntext[pos];
        //     beg++;
        //     pos++;

        //     int idpos=model.wordindex[ntext[beg-1]];
        //     int idbeg=model.wordindex[ntext[beg-1]];




        //     // double wra=vquery.angle(newv);
        //     // if(best.angle_with<wra){
        //     //     best=newv;
        //     //     best.angle_with=wra;
        //     // }
        // }
        best.path=v.path;
        best.angle_with=v.angle_with;
        return best;
    }


    private double op_not_in(){


    }

    private double op_not_in(){


    }

    private double op_more_imp(){


    }
    private double op_near(){


    }

    public double calculate_importance(vector v){
        double orig=(1-(v.angle_with%1))*100;
        return orig;
    }

    
    List<vector>operators(List<vector>v){
        for(int i=0;i<v.Count;i++){
            v[i].angle_with=calculate_importance(v[i]);
        }
        v.Sort(delegate(vector a,vector b){if(a.angle_with<b.angle_with)return 1;else return -1;});
        return v;
    } 



    public List<vector>  query(string s,int cant=5,bool fast=false){
        
        if(fast==false){
            List<vector> result= model.naive_search(s);
            for(int i=0;i<result.Count;i++){
                result[i]=snippet(result[i],s);
            }


            result=operators(result);
            return result;
        }else{    
            List<vector> result= model.naive_search(s);    
            return result;
        }
    }


}