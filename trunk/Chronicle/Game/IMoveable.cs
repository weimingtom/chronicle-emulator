using Chronicle.Utility;

namespace Chronicle.Game
{
    public interface IMoveable
    {
        bool FacingLeft { get; }
        byte Stance { get; set; }
        ushort Foothold { get; set; }
        Coordinates Position { get; set; }
    }
}
