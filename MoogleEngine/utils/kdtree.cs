

public class kdtree{

    public class node{
        public node? lft,rgt;
        public vector? vec;
    };

    public node root=new node();

    public void build(ref node wr,List<vector>vectors,int dim){
        if( vectors.Count==0 ){
            return;
        }
        if( vectors.Count==1 ){
            wr.vec=vectors[0];
            return;
        }

        int wds=vectors[0].vec.Count;   

        vectors.Sort(delegate(vector a,vector b){if(a.vec[dim%wds]<b.vec[dim%wds])return -1;else return 1;});

        int med=vectors.Count/2;
        wr.vec=vectors[med];

        var lft=new List<vector>();
        var rgt=new List<vector>();        
        wr.lft=new node();
        wr.rgt=new node();

        for(int i=0;i<vectors.Count;i++){
            if( vectors[i].vec[dim%wds]<vectors[med].vec[dim%wds] ){
                lft.Add(vectors[i]);
            }else{
                rgt.Add(vectors[i]);
            }
        }
        build(ref wr.lft,lft,dim+1);
        build(ref wr.rgt,rgt,dim+1);
    }

    // public PriorityQueue<vector> query(ref node wr,vector q){
    //     PriorityQueue<vector>ret;
    //     if(node.vec.Count<=)
    //     return ret;
    // }


    public void  print(node wr){
        Console.WriteLine("node");
        if(wr.lft!=null)print(wr.lft);
        if(wr.rgt!=null)print(wr.rgt);
    }
}