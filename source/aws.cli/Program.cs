namespace Aws
{
    static class Program
    {
        /// <summary>
        /// Starting point for the program logic
        /// </summary>
        /// <param name="args">The args.</param>
        static int Main(string[] args)
        {
            return new AwsConsole().Run(args);
        }
    }
}
