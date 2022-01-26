using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace GhostHugoMigrator
{
    class Program
    {
        private const string jsonExportPath = @"C:\Users\mvincze\Google Drive\Blog\Vultr\Backups\2020-10-27\mark-vinczes-coding-blog.ghost.2020-10-27.json";

        private const string hugoPostsFolder = @"C:\Workspaces\Github\markvincze-blog\content\posts";

        static void Main(string[] args)
        {
            var json = File.ReadAllText(jsonExportPath);
            var jobj = JObject.Parse(json);

            var posts = jobj["db"][0]["data"]["posts"] as JArray;
            var tags = jobj["db"][0]["data"]["tags"] as JArray;
            var posts_tags = jobj["db"][0]["data"]["posts_tags"] as JArray;

            var excludeSlugs = new []
            {
                "ghost-0-7",
                "about-me",
                "welcome-to-ghost"
            };

            Console.WriteLine("Number of posts: {0}", posts.Count);

            foreach (var post in posts)
            {
                var slug = post["slug"].Value<string>();

                if (excludeSlugs.Contains(slug))
                {
                    continue;
                }

                var id = post["id"].Value<string>();
                var commentId = post["comment_id"].Value<string>();
                var title = post["title"].Value<string>();
                var date = DateTime.Parse(post["published_at"].Value<string>());
                var dateIso = date.ToString("o");
                var mobiledoc = post["mobiledoc"].Value<string>();
                var cards = JObject.Parse(mobiledoc)["cards"] as JArray;
                var metaDescription = post["meta_description"].Value<string>();

                if (cards.Count != 1)
                {
                    Console.WriteLine("WARNING: Post with more than 1 cards. Title: {0}", title);
                    continue;
                }

                var card = cards[0] as JArray;

                if(card.Count != 2)
                {
                    Console.WriteLine("WARNING: Card array contains {0} elements", card.Count);
                    continue;
                }

                var cardType = card[0].Value<string>();
                if(cardType != "card-markdown" && cardType != "markdown")
                {
                    Console.WriteLine("WARNING: The card type is {0}", card[0].Value<string>());
                    continue;
                }

                var markdown = card[1]["markdown"].Value<string>()
                    .Replace("](/content/images", "](/images");

                var postTags = tags.Where(
                    t => posts_tags.Any(pt => pt["post_id"].Value<string>() == id &&
                                              pt["tag_id"].Value<string>() == t["id"].Value<string>()));
                
                var tagsString = 
                    String.Join(
                        ", ",
                        postTags.Select(t => "\"" + t["name"].Value<string>() + "\""));

                File.WriteAllText(
                    $@"{hugoPostsFolder}\{slug}.md",
                    $@"+++
title = ""{title}""
slug = ""{slug}""
description = ""{metaDescription}""
date = ""{dateIso}""
tags = [{tagsString}]
ghostCommentId = ""ghost-{commentId}""
+++

{markdown}
"
                );
            }
        }
    }
}
