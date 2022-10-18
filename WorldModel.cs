using System;
using System.Threading;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GravityBalls
{
	public class Ball
	{
		private double WorldWidth;
		private double WorldHeight;
		private double CursorX;
		private double CursorY;
		
		public double BallX;
		public double BallY;
		public double BallRadius;
		private double BallSpeedX = 100.0;
		private double BallSpeedY = -50.0;
		
		private double Resistance = 0.05;
		private const double G = 5000.0;
		private double FearBall = 5000.0;
		public Brush BallColor;

		public Ball()
		{
			
		}
		
		public Ball(double worldWidth, double worldHeight, double ballRadius, double ballX, double ballY, double ballSpeedX, double ballSpeedY, Brush color)
		{
			WorldWidth = worldWidth;
			WorldHeight = worldHeight;

			BallRadius = ballRadius;
			BallX = ballX;
			BallY = ballY;
			BallSpeedX = ballSpeedX;
			BallSpeedY = ballSpeedY;

			BallColor = color;
		}
		
		public void SetWorldInfo(double worldWidth, double worldHeight)
		{
			WorldWidth = worldWidth;
			WorldHeight = worldHeight;
		}

		public void SetCursorInfo(double cursorX, double cursorY)
		{
			CursorX = cursorX;
			CursorY = cursorY;
		}
		
		public void SetPosition(double x, double y)
		{
			BallX = x;
			BallY = y;
		}

		public void Move(double dx, double dy)
		{
			BallX += dx;
			BallY += dy;
		}
		
		public void SetSpeed(double speedX, double speedY)
		{
			BallSpeedX = speedX;
			BallSpeedY = speedY;
		}

		public void AddForce(double forceX, double forceY, double dt)
		{
			BallSpeedX += forceX * dt;
			BallSpeedY += forceY * dt;
		}
		
		public void LockPositionOnWindow()
		{
			if (BallX > WorldWidth - BallRadius)
			{
				BallX = WorldWidth - BallRadius;
				BallSpeedX *= -1;
			}
			
			if (BallX < BallRadius)
			{
				BallX = BallRadius;
				BallSpeedX *= -1;
			}
			
			if (BallY > WorldHeight - BallRadius)
			{
				BallY = WorldHeight - BallRadius;
				BallSpeedY *= -1;
			}
			
			if (BallY < BallRadius)
			{
				BallY = BallRadius;
				BallSpeedY *= -1;
			}
		}

		public void MakeResistance(double dt)
		{
			AddForce(-Resistance * BallSpeedX, -Resistance * BallY, dt);
		}

		public void MakeGravity(double dt)
		{
			AddForce(0.0, G * dt, dt);
		}
		
		private void RunFromCursor(double dt)
		{
			var vectorX = BallX - CursorX;
			var vectorY = BallY - CursorY;

			var vectorLength = WorldModel.VectorLength(vectorX, vectorY);
			if (vectorLength == 0)
			{
				return;
			}

			vectorX /= vectorLength;
			vectorY /= vectorLength;
			
			AddForce(vectorX * FearBall / (vectorLength * vectorLength), vectorY * FearBall / (vectorLength * vectorLength), dt);
		}

		public void MakeCollision(Ball otherBall, double dt)
		{
			var range = Math.Sqrt(Math.Pow(BallX - otherBall.BallX, 2) + Math.Pow(BallY - otherBall.BallY, 2));
			if (BallRadius + otherBall.BallRadius > range)
			{
				var speed = Math.Sqrt(BallSpeedX * BallSpeedX + BallSpeedY * BallSpeedY);
				var normalX = (BallX - otherBall.BallX) / range;
				var normalY = (BallY - otherBall.BallY) / range;
				SetSpeed((speed + 1.0) * normalX, (speed + 1.0) * normalY);
			}
		}
		
		public void Update(double dt)
		{
			
			Move(BallSpeedX * dt, BallSpeedY * dt);
			LockPositionOnWindow();

			MakeResistance(dt);
			MakeGravity(dt);
			RunFromCursor(dt);
		}
	}
	
	public class WorldModel
	{
		
		public double WorldWidth;
		public double WorldHeight;
		private double CursorX;
		private double CursorY;
		
		public Ball[] BallArray = new Ball[300];

		public static double VectorLength(double x, double y)
		{
			return Math.Sqrt(x * x + y * y);
		}
		public void TakeCursorPosition(double cursorX, double cursorY)
		{
			CursorX = cursorX;
			CursorY = cursorY;
		}
		
		public void SimulateTimeframe(double dt)
		{
			for (var i = 0; i < BallArray.Length; i++)
			{
				BallArray[i].SetWorldInfo(WorldWidth, WorldHeight);
				BallArray[i].SetCursorInfo(CursorX, CursorY);
				BallArray[i].Update(dt);
			}
			
			for (var i = 0; i < BallArray.Length; i++)
			{
				for (var j = i + 1; j < BallArray.Length; j++)
				{
					BallArray[i].MakeCollision(BallArray[j], dt);
					BallArray[j].MakeCollision(BallArray[i], dt);
				}
			}
		}
	}
}