using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GraphDBAPITest
{

    [Test]
    public async void QueryTest()
    {
        var api = new GraphDBAPI();

        var jsonText = await api.Query("PREFIX imdb: <http://academy.ontotext.com/imdb/>\r\n" +
                                    "PREFIX schema: <http://schema.org/>\r\n\r\n" +
                                    "SELECT ?movie ?director\r\n" +
                                    "WHERE {\r\n  " +
                                    "?movie a schema:Movie .\r\n  " +
                                    "?movie schema:director ?director .\r\n}");

        Debug.Log(jsonText);



        File.WriteAllText(Application.dataPath + "/KGVRUnity/Data/query.json", jsonText);

        var data = JsonConvert.DeserializeObject<JObject>(jsonText);

        // (string)obj["name"]
        Debug.Log(data);
    }


    //[Test]
    //public async void GetNodes()
    //{
    //    var api = new GraphDBAPI();
    //    var json = await api.Query("PREFIX imdb: <http://academy.ontotext.com/imdb/>\r\nPREFIX schema: <http://schema.org/>\r\n\r\nSELECT ?movie ?director\r\nWHERE {\r\n  ?movie a schema:Movie .\r\n  ?movie schema:director ?director .\r\n} LIMIT 50");


    //    var data = JsonConvert.DeserializeObject<JObject>(json);

    //    var nodes = new List<Node>();
    //    var edges = new List<Edge>();


    //    string directorEdgeUri = "http://schema.org";
    //    string directorEdgeLabel = "director";

    //    foreach (JToken binding in data["results"]["bindings"])
    //    {
    //        var movieUriFull = binding["movie"]["value"].Value<string>();
    //        var directorUriFull = binding["director"]["value"].Value<string>();

    //        int movieLastSlashId = movieUriFull.LastIndexOf('/');
    //        string movieUri = movieUriFull.Substring(0, movieLastSlashId);
    //        string movieLabel = movieUriFull.Substring(movieLastSlashId + 1);

    //        int directorLastSlashId = directorUriFull.LastIndexOf('/');
    //        string directorUri = directorUriFull.Substring(0, directorLastSlashId);
    //        string directorLabel = directorUriFull.Substring(directorLastSlashId + 1);

    //        var movie = new Node("Movie", movieLabel, movieUri);
    //        var director = new Node("Director", directorLabel, directorUri);
    //        nodes.Add(movie);
    //        nodes.Add(director);


    //        edges.Add(new Edge(directorEdgeLabel, directorEdgeUri, movie, director));
    //    }



    //    Debug.Log(nodes + " " + edges);
    //}

}
