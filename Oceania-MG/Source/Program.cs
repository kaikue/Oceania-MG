using System;

namespace Oceania_MG.Source
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
			//BiomeTest(): runs a debug window showing biome arrangement chart
			//StructureEditor(): runs the editor for creating premade structures
			using (var game = new StructureEditor())
			{
				game.Run();
			}
        }
    }
}
