
namespace MoogleEngine;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
public static class from_api{
    public static List<string> get_synonims(string s){
        List<string> ret=new List<string>();

        var url = $"http://sesat.fdi.ucm.es:8080/servicios/rest/sinonimos/json/{s}";
        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";
        request.ContentType = "application/json";
        request.Accept = "application/json";
        try{
            using (WebResponse response = request.GetResponse()){
                using (Stream strReader = response.GetResponseStream()){
                    if (strReader == null) return ret;
                    using (StreamReader objReader = new StreamReader(strReader)){
                        string responseBody = objReader.ReadToEnd();
                                        
                        char[] delimiters = {':', '{', '}', ' ', '\t', '\n','[',']',',','"' };
                        string[] words=responseBody.Split(delimiters);
                        bool error_in_connection=false;
                        for(int i=0;i<words.Length;i++){
                            if( words[i]!="" && !words[i].Equals("sinonimo") && !words[i].Equals("sinonimos") ){
                                ret.Add(words[i]);
                            }
                            if(words[i].Length>=13){
                                error_in_connection=true;
                            }
                        }
                        if(error_in_connection){
                            ret.Clear();
                            return ret;
                        }

                    }
                }
            }
        }
        catch (WebException ex){
            Console.WriteLine("Un error salvaje ha aparecido al usar el API!");
        }
        return ret;
    }

}
