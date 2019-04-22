using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace GhostHugoMigrator
{
    class Program
    {
        private const string jsonExportPath = @"C:\Users\mvincze\Google Drive\Blog\Vultr\Backups\2019-04-14\mark-vinczes-coding-blog.ghost.2019-04-14-trimmed.json";

        private const string hugoPostsFolder = @"C:\Workspaces\Github\markvincze-blog\content\posts";

        static void Main(string[] args)
        {
            var json = File.ReadAllText(jsonExportPath);
            var jobj = JObject.Parse(json);

            var posts = jobj["db"][0]["data"]["posts"] as JArray;

            Console.WriteLine("Number of posts: {0}", posts.Count);
        }
    }
}
