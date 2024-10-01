using Godot;
using System;

public partial class Star : Window
{
	private int xvel = 20;
	private int yvel = 20;
	public static float xmin;
	public static float xmax;
	public static float ymin;	
	public static float ymax;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Random rand= new Random();
		Position = new Vector2I(rand.Next(0, Pet.screenWidth - Size.X), rand.Next(0, Pet.screenHeight - Size.Y));
		Borderless = true;
		AlwaysOnTop = true;
		Transparent = true;	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += new Vector2I(xvel, yvel);

		if (Position.Y > Pet.screenHeight - Size.Y || Position.Y < 0) {
			yvel *= -1;
		}
		if (Position.X > Pet.screenWidth - Size.X|| Position.X < 0) {
			xvel *= -1;
		}

		xmin = Position.X;
		xmax = xmin + 200;

		ymin = Position.Y;
		ymax = Position.Y + 200;
	}
}
