using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GravityBalls
{
	public class BallsForm : Form
	{
		private Timer timer;
		private WorldModel world;

		private WorldModel CreateWorldModel()
		{
			var w = new WorldModel
			{
				WorldHeight = ClientSize.Height,
				WorldWidth = ClientSize.Width,
			};
			var r = new Random();
			for (var i = 0; i < w.BallArray.Length; i++)
			{
				Brush color = Brushes.Black;
				switch (r.Next(5))
				{
					case 0:
						color = Brushes.Aqua;;
						break;
					case 1:
						color = Brushes.Chartreuse;
						break;
					case 2:
						color = Brushes.Brown;
						break;
					case 3:
						color = Brushes.Bisque;
						break;
					case 4:
						color = Brushes.Gold;
						break;
				}
				w.BallArray[i] = new Ball(w.WorldWidth, w.WorldHeight, r.Next(5, 15), r.Next((int)w.WorldWidth), r.Next((int)w.WorldHeight), r.Next(-100, 100), r.Next(-100, 100), color);
			}

			return w;
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			world.WorldHeight = ClientSize.Height;
			world.WorldWidth = ClientSize.Width;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			DoubleBuffered = true;
			BackColor = Color.Black;
			world = CreateWorldModel();
			timer = new Timer { Interval = 30 };
			timer.Tick += TimerOnTick;
			timer.Start();
			world.WorldHeight = ClientSize.Height;
			world.WorldWidth = ClientSize.Width;
		}

		private void TimerOnTick(object sender, EventArgs eventArgs)
		{
			world.SimulateTimeframe(timer.Interval / 1000d);
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			var g = e.Graphics;
			g.SmoothingMode = SmoothingMode.HighQuality;
			for (int i = 0; i < world.BallArray.Length; i++)
			{
				g.FillEllipse(world.BallArray[i].BallColor,
				(float)(world.BallArray[i].BallX - world.BallArray[i].BallRadius),
				(float)(world.BallArray[i].BallY - world.BallArray[i].BallRadius),
				2 * (float)world.BallArray[i].BallRadius,
				2 * (float)world.BallArray[i].BallRadius);
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			Text = "Лучшая версия этого задания!)";
			world.TakeCursorPosition(e.X, e.Y);
		}
	}
}