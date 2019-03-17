
using System;
using System.Data.Entity;
using System.Linq;

namespace Vidzy
{
    class Program
    {
        static void Main(string[] args)
        {
            // Note that here I have used a "using" block for each exercise to ensure
            // we start with a new fresh context and the data loaded for each exercise,
            // does not impact our approach to solve subsequent exercises.

            // Exercise 1: Add a new video (Terminator 1)
            // .
            // Here I have hardcoded the GenreId (2). In a real-world application,
            // the user often selects the genre from a drop-down list. There, you should
            // have the Id for each genre. If you're building a WPF application, this 
            // GenreId is already in the memory. If you're building an ASP.NET MVC application,
            // the GenreId is posted with the request and you can set it here. 
            AddVideo(new Video
            {
                Name = "Terminator 1",
                GenreId = 2,
                Classification = Classification.Silver,
                ReleaseDate = new DateTime(1984, 10, 26)
            });


            // Exercise 2: Add two tags "classics" and "drama" to the database.
            AddTags("classics", "drama");


            // Exercise 3: Add three tags "classics", "drama" and "comedy" to video with Id 1.
            AddTagsToVideo(1, "classics", "drama", "comedy");


            // Exercise 4: Remove the "comedy" tag from Video with Id 1. 
            RemoveTagsFromVideo(1, "comedy");


            // Exercise 5: Remove the video with Id 1.
            RemoveVideo(1);


            // Exercise 6: Remove the genre with Id 2.
            // .
            // Note use of named parameter here to improve the readability of the code. 
            // Without this, my code would look like: 
            //
            // RemoveGenre(2, true); // What does true mean here? 
            RemoveGenre(2, enforceDeletingVideos: true);
        }

        public static void AddVideo(Video video)
        {
            using (var context = new VidzyContext())
            {
                context.Videos.Add(video);
                context.SaveChanges();
            }
        }

        public static void AddTags(params string[] tagNames)
        {
            using (var context = new VidzyContext())
            {
                // We load the tags with the given names first, to prevent adding duplicates.
                var tags = context.Tags.Where(t => tagNames.Contains(t.Name)).ToList();

                foreach (var name in tagNames)
                {
                    if (!tags.Any(t => t.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
                        context.Tags.Add(new Tag { Name = name });
                }

                context.SaveChanges();
            }
        }

        // In terms of API design, this method expects tag names in the form of a string array.
        // We shouldn't use TagId because that would only work if the given tag exists in the database.
        // But often, in an application with a good user experience, the user should be able to pick a
        // tag from a drop-down list, or add one at the same time as adding or editing a video. So, 
        // we should use tag names to add a new tag to the database. Plus, tag names should be unique, 
        // so conceptually, they can be treated as primary keys, but we use an int (TagId) for optimization.
        public static void AddTagsToVideo(int videoId, params string[] tagNames)
        {
            using (var context = new VidzyContext())
            {
                // This technique with LINQ leads to 
                // 
                // SELECT FROM Tags WHERE Name IN ('classics', 'drama')
                var tags = context.Tags.Where(t => tagNames.Contains(t.Name)).ToList();

                // So, first we load tags with the given names from the database 
                // to ensure we won't duplicate them. Now, we loop through the list of
                // tag names, and if we don't have such a tag in the database, we add
                // them to the list.
                foreach (var tagName in tagNames)
                {
                    if (!tags.Any(t => t.Name.Equals(tagName, StringComparison.CurrentCultureIgnoreCase)))
                        tags.Add(new Tag { Name = tagName });
                }

                var video = context.Videos.Single(v => v.Id == videoId);

                tags.ForEach(t => video.AddTag(t));

                context.SaveChanges();
            }
        }

        public static void RemoveTagsFromVideo(int videoId, params string[] tagNames)
        {
            using (var context = new VidzyContext())
            {
                // We can use explicit loading to only load tags that we're going to delete.
                context.Tags.Where(t => tagNames.Contains(t.Name)).Load();

                var video = context.Videos.Single(v => v.Id == videoId);

                foreach (var tagName in tagNames)
                {
                    // I've encapsulated the concept of removing a tag inside the Video class. 
                    // This is the object-oriented way to implement this. The Video class
                    // should be responsible for adding/removing objects to its Tags collection. 
                    video.RemoveTag(tagName);
                }

                context.SaveChanges();
            }
        }

        public static void RemoveVideo(int videoId)
        {
            using (var context = new VidzyContext())
            {
                var video = context.Videos.SingleOrDefault(v => v.Id == videoId);
                if (video == null) return;

                context.Videos.Remove(video);
                context.SaveChanges();
            }
        }

        public static void RemoveGenre(int genreId, bool enforceDeletingVideos)
        {
            using (var context = new VidzyContext())
            {
                var genre = context.Genres.Include(g => g.Videos).SingleOrDefault(g => g.Id == genreId);
                if (genre == null) return;

                if (enforceDeletingVideos)
                    context.Videos.RemoveRange(genre.Videos);

                context.Genres.Remove(genre);
                context.SaveChanges();
            }
        }
    }
}
