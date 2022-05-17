// Code by Leonardo Artiles Montero 2022



public static class string_utils{

    public static int edit_distance(string a,string b){
        // Calculate the number of changes needed to convert string a in string b with Dynamic Programming O(N^2)
        int n=a.Length;
        int m=b.Length;

        int[,] dp=new int[n+1,m+1]; // Dynamic programming table to make memorization

        for(int i=0;i<=n;i++){
            for(int j=0;j<=m;j++){
                if(i==0)dp[i,j]=j;
                if(j==0)dp[i,j]=i;
                if( i!=0 && j!=0 ){
                    if(a[i-1]==b[j-1]){
                        dp[i,j] = dp[i-1,j-1];
                    }else{
                        dp[i,j] = 1 + Math.Min( dp[i-1,j] , Math.Min(dp[i-1,j-1],dp[i,j-1]) );
                    }
                }
            }
        }
        
        return dp[n,m];
    }

    public static int longest_common_prefix(string a,string b){
        // Returns the longest common prefix between two strings O(n)
        for(int i=0;i<Math.Min(a.Length,b.Length);i++){
            if(a[i]!=b[i]){
               return 1+i;
            }
        }
        return 1+Math.Min(a.Length,b.Length);
    }
    
    public static double distance(string a,string b){
        // My own distance formula 
        int ed=edit_distance(a,b);
        int lcp=longest_common_prefix(a,b);
        return (double)ed/lcp;
    }


    public static bool is_letter(char mander){  
        return (mander>='a'&&mander<='z')||(mander>='A'&&mander<='Z')||
        mander=='á'||mander=='é'||mander=='í'||mander=='ó'||mander=='ú'||mander=='ñ'||
        mander=='Á'||mander=='Á'||mander=='Í'||mander=='Ó'||mander=='Ú'||mander=='Ñ';
    }


    public static List<string> normalize_text(string text){        
        // Converts a string to a list of lowercase strings
        char[] delimiters = {' ', ',', '.', ':',';', '\t', '\n' };
        string[] ntext=text.Split(delimiters);

        HashSet<string>set = new HashSet<string>(ntext);
        string[] ret = new string[set.Count];
        set.CopyTo(ret);

        List<string> lst=new List<string>();
            
        for(int i=0;i<ret.Length;i++){
            string s=ret[i];
            s=s.ToLower();
            string stri="";
            for(int j=0;j<s.Length;j++){
                if(is_letter(s[j])) stri+=s[j];
            }
            if(stri.Length>0) lst.Add(stri);
        }

        return lst;
    }   
    public static List<string> normalize_text_with_quotation(string text){        
        // Converts a string to a list of lowercase strings with quotation marks
       
        string ktext="";
        for(int i=0;i<text.Length;i++){
            if( !is_letter(text[i]) ){
                ktext+=" "+text[i]+" ";
            }else{
                ktext+=text[i];
            }
        }   

        text=ktext;
       
        char[] delimiters = {' ', '\t', '\n' };
        string[] ntext=text.Split(delimiters);

        List<string> lst=new List<string>();
            
        for(int i=0;i<ntext.Length;i++){
            string s=ntext[i];
            s=s.ToLower();
            lst.Add(s);
        }

        return lst;
    }   



    public static List<string> get_word_list(List<string> list){        
        // return list of different strings
        List<string>ret=new List<string>();
        Dictionary<string,int> dict= new Dictionary<string,int>();
        for(int i=0;i<list.Count;i++){
                if(dict.ContainsKey(list[i])){
                    dict[ list[i] ]++;
                    ret.Add(list[i]);
                }else{
                    dict.Add(list[i],1);
                }    
        }
        return ret;
    }   

    public static List<string> remove_duplicates(List<string> list){        
        // return list of different strings
        List<string>ret=new List<string>();
        Dictionary<string,int> dict= new Dictionary<string,int>();
        for(int i=0;i<list.Count;i++){
            if(!dict.ContainsKey(list[i])){
                dict[ list[i] ]++;
                ret.Add(list[i]);
            }
        }
        return ret;
    }   
    public static List<string> join_lists(List<string> a,List<string> b){        
        for(int i=0;i<b.Count;i++){
            a.Add(b[i]);
        }
        return a;
    }   



    public static List<string> to_list(string[] s){        
        List<string>ret=new List<string>();
        for(int i=0;i<s.Length;i++){
                ret.Add(s[i]);
        }
        return ret;
    }   
    public static void print_list(List<string>lis){        
        for(int i=0;i<lis.Count;i++){
                Console.Write(lis[i]+" ");
        }
        Console.WriteLine();
    }   
}
