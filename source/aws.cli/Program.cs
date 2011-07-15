namespace Amazon
{
    static class Program
    {
        /// <summary>
        /// Entry point for the program logic
        /// </summary>
        /// <param name="args">The args.</param>
        static void Main(string[] args)
        {
            new AwsConsole().Run(args);
        }
    }
}
