using Json2DB.Models;
namespace Json2DB
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new ConversionManager();
            var result = manager.Json2Db();

            if (result.Status == ResponseStatus.Success)
            {
                if (result.NumRowsUpdated == 0)
                {
                    System.Console.WriteLine("I think it is strange. 0 rows updated????");
                }
                else
                {
                    System.Console.WriteLine(string.Format("Everything was OK. Updated {0} rows", result.NumRowsUpdated));
                }
            }
            else
            {
                System.Console.WriteLine(string.Format("Something was wrong :'( \n {0}", result.TraceError));
            }

            System.Console.WriteLine("Press a key to finish");
            var ky = System.Console.ReadKey();
        }
    } //end of class
} //end of namespace
