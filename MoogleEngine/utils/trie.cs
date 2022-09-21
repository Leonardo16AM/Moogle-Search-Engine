// Code by Leonardo Artiles Moontero 2022
namespace MoogleEngine;



public class trie{
    // Suffix Trie
    private List<int>[] graph= new List<int>[100000000];
    private string[] node_string= new string[100000000];
    private int last=1;
    private List<int> parent= new List<int>();
    private List<bool> end= new List<bool>();

    public int insert(string s){
        // Inserts a string s in the trie
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
        node_string[u]=s;
        return u;
    }

    private List<string> dfs(int node,int dist){
        // Find  all the words with the last "dist" letters different 
        List<string> ret= new List<string>();
        if(dist>1)return ret;
        if(end[node]){
            ret.Add(node_string[node]);
        }        
        for(int i=0;i<graph[node].Count;i++){
            int v=graph[node][i];
            ret=string_utils.join_lists(ret,dfs(v,dist+1));
        }
        return ret;
    }

    public List<string> family_words(string s,int deep=0){
        // Search family words
        int node=insert(s);
        int orig_node=node;
        while( deep>0 && node!=0 ){
            node=parent[node];   
            deep--;
        }
        if(node==0)return dfs(orig_node,0);
        return dfs(node,0);
    }


}
