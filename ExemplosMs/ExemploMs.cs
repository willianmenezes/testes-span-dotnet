using BenchmarkDotNet.Attributes;

namespace ExemplosMs;

[MemoryDiagnoser]
[ThreadingDiagnoser]
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
    public int Execute()
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
        return matchQuery.Count();
    }

    [Benchmark]
    public int ExecuteOtimizado()
    {
        var searchTerm = "data".AsSpan();
        var textAsSpan = _text.AsSpan();

        var nameCount = 0;

        for (var x = 0; x < textAsSpan.Length; x++)
        {
            if (textAsSpan[x] != searchTerm[0] && (textAsSpan[x] + 32) != searchTerm[0])
                continue;

            if (textAsSpan[x..].Length < searchTerm.Length)
                return 0;

            if (textAsSpan.Slice(x, 4).Equals(searchTerm, StringComparison.InvariantCultureIgnoreCase))
            {
                nameCount++;
            }
        }

        return nameCount;
    }

    // criar um exemplo onde eu preciso utilizar a string que manipulei com o span, para mostrar que nem sempre utilizar o span eh o caminho
    // se eu precisar usar a string. O readonlyb span eh ainda mais lento quando se quer utilizar a string que estamos manipulando.
}