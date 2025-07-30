using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Yeet.Common {
  public static class Constants {
    public const string MOD_ID = "yeet";
    public const GlKeys DEFAULT_YEET_KEY = GlKeys.BracketLeft;
    public const string YEET_ONE_CODE = "yeetit";
    public const string YEET_ONE_DESC = "Hard throw item";
    public const string YEET_ALL_CODE = "yeetthem";
    public const string YEET_ALL_DESC = "Hard throw itemstack";
    public const string YEET_HALF_CODE = "yeetsome";
    public const string YEET_HALF_DESC = "Hard throw half stack";
    public const string YEET_CHANNEL_NAME = "yeet-mod";
    public const string ERROR_HUNGER = "You're too hungry and cannot muster the energy.";
    public const string ERROR_NOTHING_TO_YEET = "No item to yeet.";
    public const double PHI = GameMath.PI / 4.0; // angle (in radians) between the ground plane and the Y-axis, always 45 degrees for maximum distance
    public static readonly double SIN_PHI = GameMath.Sin(PHI);
    public static readonly double COS_PHI = GameMath.Cos(PHI);
  }
}
