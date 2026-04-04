using UnityEngine;

public class Wall : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(110, 50) : new(65, 25);
    }
}
