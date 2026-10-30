using System;
using System.Numerics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;
...

namespace AimBodyApp;

public partial class MainPage : ContentPage
{
    bool aimEnabled = false;
    Vector2 crosshair = new(200, 400);
    Vector2 targetPos = new(200, 400);
    Vector2 targetVel = new(3f, 1.5f);
    float bodyW = 40, bodyH = 90;
    Vector2 BodyPoint => targetPos + new Vector2(0, bodyH * 0.15f);
    IDispatcherTimer timer;

    public MainPage()
    {
        InitializeComponent();
        CanvasView.Drawable = new AimDrawable(this);
        
        timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(16);
        timer.Tick += (s, e) => {
            targetPos += targetVel;
            if (targetPos.X < 50 || targetPos.X > CanvasView.Width - 50) targetVel.X *= -1;
            if (targetPos.Y < 100 || targetPos.Y > CanvasView.Height - 100) targetVel.Y *= -1;
            
            if (aimEnabled) {
                Vector2 toBody = BodyPoint - crosshair;
                if (toBody.Length() < 300f) {
                    crosshair += (BodyPoint - crosshair) / 5f;
                }
            }
            CanvasView.Invalidate();
        };
        timer.Start();
    }

    void OnTapped(object sender, TappedEventArgs e) => aimEnabled = !aimEnabled;

    class AimDrawable : IDrawable
    {
        MainPage p;
        public AimDrawable(MainPage page) { p = page; }
        public void Draw(ICanvas canvas, RectF rect)
        {
            canvas.FillColor = Color.FromArgb("#16181C");
            canvas.FillRectangle(rect);
            
            canvas.FillColor = Colors.Blue;
            canvas.FillRectangle(p.targetPos.X - p.bodyW/2, p.targetPos.Y - p.bodyH/2, p.bodyW, p.bodyH);

            canvas.FillColor = Colors.Lime;
            canvas.FillCircle(p.BodyPoint.X, p.BodyPoint.Y, 6);

            canvas.StrokeColor = p.aimEnabled ? Colors.Lime : Colors.Orange;
            canvas.StrokeSize = 3;
            canvas.DrawLine(p.crosshair.X - 15, p.crosshair.Y, p.crosshair.X + 15, p.crosshair.Y);
            canvas.DrawLine(p.crosshair.X, p.crosshair.Y - 15, p.crosshair.X, p.crosshair.Y + 15);
            
            canvas.FontColor = Colors.White;
            canvas.DrawString(p.aimEnabled ? "AIM: ON" : "AIM: OFF (Tap screen)", 20, 40, 300, 30, HorizontalAlignment.Left, VerticalAlignment.Center);
        }
    }
}
