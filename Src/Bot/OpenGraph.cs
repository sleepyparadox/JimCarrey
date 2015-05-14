using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JimCarrey
{
    public class OpenGraph
    {
        public const string BaseUrl = "https://graph.facebook.com/v2.3/";
        public const string Get = "GET";
        public const string Post = "POST";
     
        public static FB.Edge<TItem> RequestEdge<TItem>(string method, string url)
        {
            return Request<FB.Edge<TItem>>(method, url);
        }

        public static TResponse Request<TResponse>(string method, string url) where TResponse : FB.Response, new()
        {
            var request = WebRequest.Create(url);
            request.Method = method;

            try
            {
                var reader = new StreamReader(request.GetResponse().GetResponseStream());
                var json = reader.ReadToEnd();
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TResponse>(json);
                return obj;
            }
            catch(Exception e)
            {
                if(e is WebException)
                {
                    var webException = e as WebException;
                    var reader = new StreamReader(webException.Response.GetResponseStream());
                    var json = reader.ReadToEnd();
                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TResponse>(json);
                    if(obj.error != null)
                        obj.error.WebExceptionStatus = webException.Status;
                    return obj;
                }
                else
                {
                    throw e;
                }
            }
        }

        public static FB.Data<string> Request(string method, string url)
        {
            var request = WebRequest.Create(url);
            request.Method = method;

            try
            {
                var reader = new StreamReader(request.GetResponse().GetResponseStream());
                var data = reader.ReadToEnd();
                return new FB.Data<string>()
                {
                    data = data,
                };
            }
            catch (Exception e)
            {
                if (e is WebException)
                {
                    var webException = e as WebException;
                    var reader = new StreamReader(webException.Response.GetResponseStream());
                    var json = reader.ReadToEnd();
                    var error = Newtonsoft.Json.JsonConvert.DeserializeObject<FB.Error>(json);
                    return new FB.Data<string>()
                    {
                        error = error,
                    };
                }
                else
                {
                    throw e;
                }
            }
        }
    }
}
