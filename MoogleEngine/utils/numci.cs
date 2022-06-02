// Code by Leonardo Artiles Montero 2022
using System;




public static class numci{

    public class vector{
        public List<double> vec=new List<double>();
        public double angle_with=100.0;

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


    public class matrix{
        public List<vector> rows=new List<vector>();
        
        public void Add(vector v){
            rows.Add(v);
        }
        
        public void Add_list(List<double> v){
            vector newv=new vector();
            newv.vec=v;
            rows.Add(newv);
        }
    }

    

    public static vector matrix_mult_with_vec(matrix a,vector b){
        vector ret=new vector();
        for(int i=0;i<a.rows.Count;i++){
            ret.vec.Add(a.rows[i].dot_product(b));
        }
        return ret;   
    }


    public static vector gpu_matrix_mult_with_vec(matrix a,vector b){
        vector ret= new vector();
        for(int i=0;i<a.rows.Count;i++){
            ret.vec.Add(a.rows[i].dot_product(b));
        }
        return ret;   
    }

}



