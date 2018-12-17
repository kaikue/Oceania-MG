using System;

namespace Oceania_MG
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
			//options:
			//Game(): runs the game normally
			//GenerateTest(): runs a debug window showing terrain generation
			//BiomeTest(false): runs a debug window showing biome arrangement chart
			//BiomeTest(true): runs a debug window showing biomes for a random world
			using (var game = new BiomeTest(true))
			{
				game.Run();
			}
        }
    }
}
