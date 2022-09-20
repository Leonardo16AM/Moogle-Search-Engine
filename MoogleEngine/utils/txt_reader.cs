// Code by Leonardo Artiles Montero 2022

using System;
using System.IO;
using System.Text;


public static class txt_reader{
    public static string read(string path){
        // Reads txts 
        string content = File.ReadAllText(path, Encoding.UTF8);
        return content;
    }
    
    public static string read2(string a,string b){
        // Reads txts 
        string path=a+b;
        string content = File.ReadAllText(path, Encoding.UTF8);
        return content;
    }

    public static List<string> ls(string gpath){
        // List of files in the directory
        string[] path = Directory.GetFiles(gpath, "*.*", SearchOption.AllDirectories);
        List<string>ret=new List<string>();
        for(int i=0;i<path.Length;i++){
            ret.Add(path[i]);
        }
        return ret;
    }
}


