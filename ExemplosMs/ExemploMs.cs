using BenchmarkDotNet.Attributes;

namespace ExemplosMs;

[MemoryDiagnoser]
public class ExemploMs
{
    private string _text;

    [GlobalSetup]
    public void Setup()
    {
        _text = """
                Historically, the world of data and the world of objects 
                have not been well integrated. Programmers work in C# or Visual Basic 
                and also in SQL or XQuery. On the one side are concepts such as classes, 
                objects, fields, inheritance, and .NET APIs. On the other side 
                are tables, columns, rows, nodes, and separate languages for dealing with 
                them. Data types often require translation between the two worlds; there are 
                different standard functions. Because the object world has no notion of query, a 
                query can only be represented as a string without compile-time type checking or 
                IntelliSense support in the IDE. Transferring data from SQL tables or XML trees to 
                objects in memory is often tedious and error-prone. 
                """;
    }


    [Benchmark]
    public void Execute()
    {
        for (int i = 0; i < 100_000; i++)
        {
            var searchTerm = "data";

            //Convert the string into an array of words
            char[] separators = ['.', '?', '!', ' ', ';', ':', ','];
            var source = _text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            // Create the query.  Use the InvariantCultureIgnoreCase comparison to match "data" and "Data"
            var matchQuery = from word in source
                where word.Equals(searchTerm, StringComparison.InvariantCultureIgnoreCase)
                select word;

            // Count the matches, which executes the query.
            int wordCount = matchQuery.Count();
            Console.WriteLine($"""{wordCount} occurrences(s) of the search term "{searchTerm}" were found.""");
            /* Output:
               3 occurrences(s) of the search term "data" were found.
            */
        }
    }

    [Benchmark]
    public void ExecuteOtimizado()
    {
        for (var i = 0; i < 100_000; i++)
        {
            var searchTerm = "data".AsSpan();
            var textAsSpan = _text.AsSpan();

            using var enumerator = textAsSpan.GetEnumerator();
            var index = 0;
            var nameCount = 0;

            while (enumerator.MoveNext())
            {
                if (textAsSpan[index] == searchTerm[0] || (textAsSpan[index] + 32) == searchTerm[0])
                {
                    if (textAsSpan.Slice(index).Length < searchTerm.Length)
                        return;

                    if (textAsSpan.Slice(index, 4).Equals(searchTerm, StringComparison.InvariantCultureIgnoreCase))
                    {
                        nameCount++;
                    }
                }

                index++;
            }

            Console.WriteLine($"""{nameCount} occurrences(s) of the search term "{searchTerm}" were found.""");
        }
    }
}