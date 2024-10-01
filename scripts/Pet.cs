using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class Pet : AnimatedSprite2D
{
	private Window mainWindow;
	private float xvel = 10;
	private float yvel = 10;
	private Vector2I playerSize;
	public static int screenWidth = DisplayServer.ScreenGetSize().X;
	public static int screenHeight = DisplayServer.ScreenGetSize().Y;
	public static List<Particle> particles = new List<Particle>();
	private bool starSpawned = false;
	private bool starMode = false;
	public override void _Ready()
	{
		playerSize = new Vector2I((int)Math.Ceiling(180 * Scale.X), (int)Math.Ceiling(521 * Scale.Y));
		Position = playerSize/2;
		mainWindow = GetWindow();
		mainWindow.Size = playerSize;
		(Material as ShaderMaterial).SetShaderParameter("onoff", 0);
	}

    public async override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent) {
			if (mouseEvent.IsPressed() && mouseEvent.ButtonIndex == MouseButton.Left) {
				GetNode<AudioStreamPlayer>("EatingSound").Play();

				var starScene = GD.Load<PackedScene>("scenes/star.tscn"); 
				var star = starScene.Instantiate();
				GetNode<Node>("..").AddChild(star);
				starSpawned = true;


				for (int i = 0; i < 12; i++) {
					if (Frame != 1) {
						Frame = 1;
					} else if (Frame == 1) {
						Frame = 2;
					}

					if (i % 2 == 0) {
						for (int j = 0; j < 4; j++) {
							Particle particle = new Particle();
							AddChild(particle);
							particles.Add(particle);
						}
					}
					await Task.Delay(100);
					
				}
				Frame = 0;
			}
		}
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		mainWindow.Position += new Vector2I((int)xvel, (int)yvel);

		if (mainWindow.Position.Y > screenHeight - mainWindow.Size.Y || mainWindow.Position.Y < 0) {
			yvel *= -1;
		}
		if (mainWindow.Position.X > screenWidth - mainWindow.Size.X|| mainWindow.Position.X < 0) {
			xvel *= -1;
		}

		var xmin = mainWindow.Position.X;
		var xmax = mainWindow.Position.X + 180 * Scale.X;
		var ymin = mainWindow.Position.Y;
		var ymax = mainWindow.Position.Y + 521 * Scale.Y;

		if (starSpawned){
			if (xmax > Star.xmin && Star.ymax > ymin && Star.xmax > xmin && ymax > Star.ymin) {
				GetNode<AudioStreamPlayer>("StarSound").Play();
				(Material as ShaderMaterial).SetShaderParameter("onoff", 1.0);
				mainWindow.Size = new Vector2I((int)Math.Round(521 * Scale.Y), (int)Math.Round(521 * Scale.Y));
				Position = mainWindow.Size/2;
				mainWindow.Position -= new Vector2I((int)(521 * Scale.Y - 180 * Scale.X), 0);
				xvel *= 2;
				yvel *= 2;
				GetNode("../StarWindow").QueueFree();
				starMode =  true;
				starSpawned = false;
			}
		}

		if (starMode){
			Rotation += (float)Math.Sin(Time.GetTicksMsec() * 0.001) * 0.1f;
		}

		if (starMode && GetNode<AudioStreamPlayer>("StarSound").Playing is false) {
			(Material as ShaderMaterial).SetShaderParameter("onoff", 0);
			xvel /= 2;
			yvel /= 2;
			Rotation = 0;
			mainWindow.Size = new Vector2I((int)Math.Round(180 * Scale.X), (int)Math.Round(521 * Scale.Y));
			Position = mainWindow.Size/2;
			mainWindow.Position += new Vector2I((int)(521 * Scale.Y - 180 * Scale.X)/2, 0);
			starMode = false;
			
		}

		foreach (Particle particle in particles) {
			particle.Update();
		}
	}
}		

public partial class Particle : Sprite2D
{
	private int gravity = 1;
	private int yvel = 8;
	private int xvel = 0;
    public override void _Ready()
    {
		Random rand = new Random();
		RegionEnabled = true;
		Texture = (Texture2D)GD.Load("res://assets/cucumber.png");
		RegionRect = new Rect2(rand.Next(800), rand.Next(120, 400), rand.Next(10, 20), rand.Next(10, 30));
		Position = new Vector2(rand.Next(-20, 20), -170);
		xvel = rand.Next(-3, 3);
    }

    public void Update()
    {
        Position -= new Vector2(xvel, yvel);
		yvel -= gravity;

		if (GlobalPosition.Y == GetWindow().Size.Y){
			Pet.particles.Remove(this);
			QueueFree();
		}
    }
}