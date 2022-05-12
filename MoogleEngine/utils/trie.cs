// Code by Leonardo Artiles Moontero 2022


public class trie{
    private List<int>[] graph= new List<int>[500000];
    private string[] node_string= new string[500000];
    private int last=1;
    private List<int> parent= new List<int>();
    private List<bool> end= new List<bool>();

    public int insert(string s){
        int u=0;
        node_string[0]="";
        end.Add(false);
        parent.Add(-1);
        if(last==1)graph[0]=new List<int>();

        for(int i=0;i<s.Length;i++){
            
            int pos=last;
            for(int j=0;j<graph[u].Count;j++){
                int v=graph[u][j];
                if(node_string[v][i]==s[i]){
                    pos=v;break;
                }
            }

            if(pos==last){
                end.Add(false);
                parent.Add(u);
                graph[last]=new List<int>();
                graph[u].Add(last);
                node_string[last]=node_string[u]+s[i];
                last++;
            }
            parent[pos]=u;
            u=pos;
        }
        end[u]=true;
        return u;
    }

    private List<string> dfs(int node){
        List<string> ret= new List<string>();
        if(end[node]){
            ret.Add(node_string[node]);
        }        
        for(int i=0;i<graph[node].Count;i++){
            int v=graph[node][i];
            ret=string_utils.join_lists(ret,dfs(v));
        }
        return ret;
    }

    public List<string> family_words(string s,int deep=4){
        int node=insert(s);
        int orig_node=node;
        while( deep>0 && node!=0 ){
            node=parent[node];   
            deep--;
        }
        if(node==0)return dfs(orig_node);
        return dfs(node);
    }


}
