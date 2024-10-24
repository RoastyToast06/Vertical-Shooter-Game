global using static Program; // Makes static fields on Program available globally (for `RNG`).

using COIS2020.AidenGomes0801606.Assignment3;
using COIS2020.StarterCode.Assignment3;

static class Program
{
    /// <summary>
    /// The random number generator used for all RNG in the program.
    /// </summary>
    public static Random RNG = new(/* Seed here */);


    private static void Main()
    {
        // The wizard/goblin ToString methods use emojis. You need this line if you want to Console.WriteLine them.
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var renderer = new CastleGameRenderer()
        {
            CaptureConsoleOutput = true,    // Makes your `Console.WriteLine` calls appear in the game window
            FrameDelayMS = 100,             // Controls how fast the animation plays
        };

        renderer.Run(new CastleDefender(), startPaused: false);
    }
}
