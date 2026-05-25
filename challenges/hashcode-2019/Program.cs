using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace hashcode_2019
{

    public class Photo {
        public Photo(int index, bool isVertical, string[] tags){
            Index = index;
            IsVertical = isVertical;
            Tags = tags;
        }
        public int Index { get;}
        public bool IsVertical {get;}

        public string[] Tags {get;}
    }

    class Parser {
        public static IEnumerable<Photo> Parse(TextReader reader){
            var numberOfPhotos = int.Parse(reader.ReadLine());

            return Enumerable.Range(0,numberOfPhotos)
                    .Select( i => new {index = i,  data = reader.ReadLine().Split(' ').ToArray()} )
                    .Select( o => new Photo(o.index, o.data[0] == "V", o.data.Skip(2).ToArray() ) );
        
        }    
    }
    class Program
    {
        static void Main(string[] args)
        {
            // parse
            var inputFile = Path.GetFullPath(@"a_example.txt");

            //var outputFile = Path.ChangeExtension(inputFile, ".out");

            var photos = Parser.Parse(File.OpenText(inputFile));

            var dico = new Dictionary<string,List<int>>();
            foreach (var photo in photos)
            {
                foreach (var tag in photo.Tags)
                {
                    if(!dico.ContainsKey(tag)){
                        dico.Add(tag, new List<int>());
                    }

                    dico[tag].Add(photo.Index);

                }
                
            }

            // REM clear solo photo ...

            foreach (var item in dico)
            {
                Console.WriteLine($"{item.Key}: {string.Join(';',item.Value)}");
            }

          

            

            

            //var stopwatch = Stopnwatch.StartNew();

            //var bestWayToCut = slicer.FindBestWayToCut(context.Pizza);
            //stopwatch.Stop();

            //File.WriteAllText(outputFile, bestWayToCut.ToOutputString());

            //var elapsed = stopwatch.Elapsed;
            //Console.WriteLine($"Slicing took : {elapsed.Hours}H {elapsed.Minutes}M {elapsed.Seconds}S {elapsed.Milliseconds}MS");
            //Console.WriteLine($"{bestWayToCut.PointEarned} / {context.Pizza.Size} ({bestWayToCut.PointEarned / context.Pizza.Size:P})");

            Console.ReadKey();
            
        }
    }
}
