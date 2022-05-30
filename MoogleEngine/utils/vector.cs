using System;

public class vector{
    public string path="placeholder.txt";
    public string full_text="placeholder_text";
    public List<string> words=new List<string>();
    public Dictionary<string,int> wordcount= new Dictionary<string,int>();
    public List<double> vec=new List<double>();
    public double angle_with=100.0;
    


    public bool find(string s){
        return wordcount.ContainsKey(s);
    }
    public int frecuency(string s){
        if(wordcount.ContainsKey(s))return wordcount[s];
        else return 0;
    }
    public double dot_product(vector B){
        double dot=0;
        for(int i=0;i<vec.Count;i++){
            dot += B.vec[i] * vec[i];
        }
        return dot;
    }
    public double module(){
        double dot=0;
        for(int i=0;i<vec.Count;i++){
            dot += vec[i] * vec[i];
        }
        return Math.Sqrt(dot);
    }

    public double angle(vector B){
        double mod=(module()*B.module());
        if(mod<1e-9)return 100.0;
        return Math.Acos( dot_product(B)/mod );
    }
    public void normalize(){
        double mod=module();
        if(mod<1e-9)return;
        for(int i=0;i<vec.Count;i++){
            vec[i]=vec[i]/mod;
        }
    }
}
