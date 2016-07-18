/*
 * Created by SharpDevelop.
 * User: Taras.Omarov
 * Date: 05.11.2015
 * Time: 13:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Drawing;
using System.Text;

namespace Raspoznanie_cifr
{
	/// <summary>
	/// Description of Class2.
	/// </summary>
	public class SudokuRecognitor
	{
		public Bitmap picture;
		public Bitmap picture1;
		public int maxX,maxY;
		public int [,]board;
		public Rectangle sudokuCord;
		
		enum Directions { Up, Right, Down ,Left};
		
		public SudokuRecognitor(Bitmap initpic)
		{
			maxX=9;
			maxY=9;
			board = new int[maxX,maxY];

			if (initpic.Width > 500)
			{
				double alfa = (double)initpic.Width/initpic.Height;
				picture = new Bitmap(initpic, (int)(500.0*alfa),(int)(500.0/alfa));
				picture1 = new Bitmap(picture);
			}
			else
			{
				picture = (Bitmap)initpic.Clone();
				picture1 = (Bitmap)initpic.Clone();
			}
			
			//picture1 = rotateImage(picture, (float)(180*(Math.Atan2(2,0)/3.14)));
			
			sudokuCord = new Rectangle(0,0,0,0);
		}
		
		private Bitmap rotateImage(Bitmap input, float angle)
		{
			Bitmap result = new Bitmap(input.Width, input.Height);
			Graphics g = Graphics.FromImage(result);
			g.TranslateTransform((float)input.Width/2, (float)input.Height / 2);
			g.RotateTransform(angle);
			g.TranslateTransform(-(float)input.Width/2,-(float)input.Height / 2);
			g.DrawImage(input, new Point(0, 0));
			return result;
		}
		
		private Point Seekline (Point startpoint, Directions seekDirection, Rectangle seekedge, double etalonBright, double accuracy)
		{
			int x = startpoint.X;
			int y = startpoint.Y;
			int maxValue;
			
			switch (seekDirection)
			{
				case Directions.Up:
					maxValue = seekedge.Y;
					while (y > maxValue) //ищем продолжение линии вверх до упора
					{
						y--;
						if (Math.Abs(picture.GetPixel(x,y).GetBrightness() - etalonBright) < accuracy)
						{} //если следующий такой же по цвету продолжаем
						else if (Math.Abs(picture.GetPixel(x-1,y).GetBrightness() - etalonBright) < accuracy)
						{x--;} //если нет то проверяем соседа с лева
						else if (Math.Abs(picture.GetPixel(x+1,y).GetBrightness() - etalonBright) < accuracy)
						{x++;} //и права
						else
						{y++;break; } //выходим с последним значением
					}					
					break;
				case Directions.Right:
					maxValue = seekedge.X + seekedge.Width;
					while (x < maxValue) //ищем продолжение линии вверх до упора
					{
						x++;
						if (Math.Abs(picture.GetPixel(x,y).GetBrightness() - etalonBright) < accuracy)
						{} //если следующий такой же по цвету продолжаем
						else if (Math.Abs(picture.GetPixel(x,y-1).GetBrightness() - etalonBright) < accuracy)
						{y--;} //если нет то проверяем соседа с лева
						else if (Math.Abs(picture.GetPixel(x,y+1).GetBrightness() - etalonBright) < accuracy)
						{y++;} //и права
						else
						{x--; break; } //выходим с последним значением
					}
					break;
				case Directions.Down:
					maxValue = seekedge.Y + seekedge.Height;
					while (y < maxValue) //ищем продолжение линии вверх до упора
					{
						y++;
						if (Math.Abs(picture.GetPixel(x,y).GetBrightness() - etalonBright) < accuracy)
						{} //если следующий такой же по цвету продолжаем
						else if (Math.Abs(picture.GetPixel(x-1,y).GetBrightness() - etalonBright) < accuracy)
						{x--;} //если нет то проверяем соседа с лева
						else if (Math.Abs(picture.GetPixel(x+1,y).GetBrightness() - etalonBright) < accuracy)
						{x++;} //и права
						else
						{y--;break; } //выходим с последним значением
					}					
					break;
				case Directions.Left:
					maxValue = seekedge.X;
					while (x > maxValue) //ищем продолжение линии вверх до упора
					{
						x--;
						if (Math.Abs(picture.GetPixel(x,y).GetBrightness() - etalonBright) < accuracy)
						{} //если следующий такой же по цвету продолжаем
						else if (Math.Abs(picture.GetPixel(x,y-1).GetBrightness() - etalonBright) < accuracy)
						{y--;} //если нет то проверяем соседа с лева
						else if (Math.Abs(picture.GetPixel(x,y+1).GetBrightness() - etalonBright) < accuracy)
						{y++;} //и права
						else
						{x++; break; } //выходим с последним значением
					}
					break;					
			}
			
			return new Point(x,y);
		}
		
		public void  recognit ()
		{
			double kvantX = (double)sudokuCord.Width / maxX;
			double kvantY = (double)sudokuCord.Height / maxY;
			Graphics g1 = Graphics.FromImage(picture1);
			for (int j=0; j<maxY;
			     j++)
			{
				for (int i=0; i<maxX; i++)
				{
					Rectangle cutrect = Cutrect (sudokuCord.X+(int)(kvantX*((float)i+0.2)),
					                             sudokuCord.Y+(int)(kvantY*((float)j+0.2)));
					g1.DrawRectangle(Pens.Red,cutrect);
					
					NumberRecognitor number =
						new NumberRecognitor(picture.Clone(cutrect,picture.PixelFormat));
					board[i,j] = number.recognize();
				}
			}
			
		}
		
		private Rectangle Cutrect (int x, int y)
		{
			Rectangle rect = new Rectangle(0,0,0,0);
			double etalonBright = picture.GetPixel(x,y).GetBrightness();
			Point seekPoint = new Point(x,y);			
			
			seekPoint = Seekline(seekPoint, Directions.Left, sudokuCord, etalonBright, 0.1);
			rect.X = seekPoint.X;
			
			seekPoint = Seekline(seekPoint, Directions.Up, sudokuCord, etalonBright, 0.1);
			rect.Y = seekPoint.Y;
			
			seekPoint = Seekline(seekPoint, Directions.Right, sudokuCord, etalonBright, 0.1);
			rect.Width = seekPoint.X - rect.X;
			
			seekPoint = Seekline(seekPoint, Directions.Down, sudokuCord, etalonBright, 0.1);
			rect.Height = seekPoint.Y - rect.Y;			
			
			return rect;
		}
		
		
		public Rectangle seekSudoku ()
		{
			int x=picture.Width / 2;
			int y=picture.Height / 2;
			/*int dX=picture.Width / maxX;
			int dY=picture.Height / maxY;*/
			
			Point centr = new Point(x,y);
			Point seekPoint = new Point(x,y);
			Point edgePoint = new Point(x,y);
			
			Rectangle vertline = new Rectangle(0,0,0,0);
			Rectangle horizline = new Rectangle(0,0,0,0);
			Rectangle seekEdge = new Rectangle((picture.Width / 4), (picture.Height / 4), 
			                                   (picture.Width / 2), (picture.Height / 2));
			Rectangle picEdge = new Rectangle(0,0,picture.Width-1,picture.Height-1);

			Graphics g1 = Graphics.FromImage(picture1);
			Color col = Color.Red;
			bool naidenVert = false;
			bool naidenHor = false;
			for (int i = 0; i<2; i++)
			{
				naidenVert = false;								
				while (!naidenVert) //поиск вертикальной линии
				{					
					x = ++seekPoint.X;//сдвиг на 1 точку
					
					Double etalonBright = picture.GetPixel(x,y).GetBrightness();
					
					//находим ширину вертикальной линии
					while (Math.Abs(picture.GetPixel(x,y).GetBrightness() - etalonBright) < 0.1)
					{x++;}
					x--;
					
					seekPoint.X = x;				
					picture1.SetPixel(x,y,Color.Red);				
					
					if (x > (seekEdge.X + seekEdge.Width)) //если поиск затянулся то выходим
					{return sudokuCord;}
																					
					edgePoint = Seekline(seekPoint, Directions.Up, picEdge, etalonBright, 0.1);
					picture1.SetPixel(edgePoint.X,edgePoint.Y,Color.Red);
					
					vertline.X=edgePoint.X; //запоминаем найденый конец
					vertline.Y=edgePoint.Y;
													
					edgePoint = Seekline(seekPoint, Directions.Down, picEdge, etalonBright, 0.1);				
					picture1.SetPixel(edgePoint.X,edgePoint.Y,Color.Red);
					
					//формируем длину и высоту вертикальной линии
					vertline.Height = edgePoint.Y - vertline.Y;				
					vertline.Width = edgePoint.X - vertline.X;			
					
					if (vertline.Height>(picture.Height/5))
					{
						naidenVert=true;
						break;
					}
					
					y = seekPoint.Y; //возвращаемся на линию поиска
					x = seekPoint.X;
				}			
			picture1 = rotateImage(picture, (float)(180*(Math.Atan2(vertline.Width, vertline.Height)/3.14)));
			picture = rotateImage(picture, (float)(180*(Math.Atan2(vertline.Width, vertline.Height)/3.14)));
			x = centr.X; //возвращаемся на центр
			y = centr.Y;
			seekPoint = centr;
			}
			
			while (!naidenHor) //поиск вертикальной линии
			{				
				y = ++seekPoint.Y;//сдвиг на 1 точку
				
				Double etalonBright = picture.GetPixel(x,y).GetBrightness();
				
				//находим ширину вертикальной линии
				while (Math.Abs(picture.GetPixel(x,y).GetBrightness() - etalonBright) < 0.1)
				{y++;}
				y--;
				seekPoint.Y=y;				
				picture1.SetPixel(x,y,Color.Red);
				
				if (y > (seekEdge.Y + seekEdge.Height)) //если поиск затянулся то выходим
				{return sudokuCord;}
								
				edgePoint = Seekline(seekPoint, Directions.Left, picEdge, etalonBright, 0.1);
				picture1.SetPixel(edgePoint.X,edgePoint.Y,Color.Red);
				
				horizline.X=edgePoint.X; //запоминаем найденый конец
				horizline.Y=edgePoint.Y;						
								
				edgePoint = Seekline(seekPoint, Directions.Right, picEdge, etalonBright, 0.1);
				picture1.SetPixel(edgePoint.X,edgePoint.Y,Color.Red);
				
				//формируем длину и высоту вертикальной линии
				horizline.Width = edgePoint.X - horizline.X;				
				horizline.Height = edgePoint.Y - horizline.Y;				
				
				if (horizline.Width>(picture.Width/5))
				{
					naidenHor=true;
					break;
				}
				x = seekPoint.X;
				y = seekPoint.Y;
			}
			
			if (naidenHor&&naidenVert)
			{
				sudokuCord.X = horizline.X;
				sudokuCord.Width = horizline.Width;
				
				sudokuCord.Y = vertline.Y;
				sudokuCord.Height = vertline.Height;
				drawsetka(sudokuCord);
			}
			
			return sudokuCord;
		}
		
		public void drawsetka(Rectangle rect)
		{
			double shagX = (double)rect.Width / maxX;
			double shagY = (double)rect.Height / maxY;
			double x = rect.X;
			double y = rect.Y;
			int maxCordY = rect.Y + rect.Height;
			int maxCordX = rect.X + rect.Width;
			
			Graphics g1 = Graphics.FromImage(picture1);
			g1.DrawRectangle(Pens.Green, rect);
			for (int i=0; i < maxX; i++)
			{
				g1.DrawLine(Pens.Green, (int)x, rect.Y, (int)x, maxCordY);
				g1.DrawLine(Pens.Green, rect.X, (int)y, maxCordX, (int)y);
				x +=shagX;
			}
		}
	}
}
