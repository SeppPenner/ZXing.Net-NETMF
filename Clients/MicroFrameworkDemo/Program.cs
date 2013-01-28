﻿using System;
using System.IO;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using ZXing;

namespace MicroFrameworkDemo
{
   public class Program : Microsoft.SPOT.Application
   {
      public static void Main()
      {
         Program myApplication = new Program();

         Window mainWindow = myApplication.CreateWindow();

         // Create the object that configures the GPIO pins to buttons.
         GPIOButtonInputProvider inputProvider = new GPIOButtonInputProvider(null);

         // Start the application
         myApplication.Run(mainWindow);
      }

      private Window mainWindow;
      private Text text;
      private Image image;
      private Bitmap Screen;

      public Window CreateWindow()
      {
         // Create a window object and set its size to the
         // size of the display.
         mainWindow = new Window();
         mainWindow.Height = SystemMetrics.ScreenHeight;
         mainWindow.Width = SystemMetrics.ScreenWidth;

         var panel = new StackPanel();

         text = new Text();
         text.Font = Resources.GetFont(Resources.FontResources.small);
         text.TextContent = Resources.GetString(Resources.StringResources.String1);
         text.HorizontalAlignment = HorizontalAlignment.Center;
         text.VerticalAlignment = VerticalAlignment.Center;
         panel.Children.Add(text);

         image = new Image();
         panel.Children.Add(image);

         // Add the text control to the window.
         mainWindow.Child = panel;

         // Connect the button handler to all of the buttons.
         mainWindow.AddHandler(Buttons.ButtonUpEvent, new RoutedEventHandler(OnButtonUp), false);

         // Set the window visibility to visible.
         mainWindow.Visibility = Visibility.Visible;

         // Attach the button focus to the window.
         Buttons.Focus(mainWindow);

         Screen = new Bitmap(SystemMetrics.ScreenWidth, SystemMetrics.ScreenHeight);

         return mainWindow;
      }

      private void OnButtonUp(object sender, RoutedEventArgs evt)
      {
         ButtonEventArgs bargs = (ButtonEventArgs) evt;

         if (bargs.Button.Equals(Button.VK_UP))
            Encoding();
         if (bargs.Button.Equals(Button.VK_DOWN))
            Decoding();
      }

      private void Encoding()
      {
         text.TextContent = "Generating barcode...";
         var ft = Resources.GetFont(Resources.FontResources.small);
         DateTime dt = DateTime.Now;
         var writer = new ZXing.QrCode.QRCodeWriter();

         var matrix = writer.encode("Time is " + dt, BarcodeFormat.QR_CODE, 50, 50);
         var bitmap = matrix.ToBitmap(BarcodeFormat.QR_CODE, null);

         Screen.DrawRectangle(Color.White, 0, 0, 0, Screen.Width, Screen.Height, 0, 0, Color.White, 0, 0,
                              Color.White, Screen.Width, Screen.Height, 255);
         Screen.DrawImage(2, 2, bitmap, 0, 0, 49, 49);
         Screen.DrawText((DateTime.Now - dt).ToString(), ft, Color.Black, 2, 70);
         Screen.Flush();

         Debug.Print((DateTime.Now - dt).ToString());
      }

      private void Decoding()
      {
         text.TextContent = "This will take a while...";
         var bmp = new Bitmap(Convert.FromBase64String(bitmapString), Bitmap.BitmapImageType.Bmp);
         image.Bitmap = bmp;

         var operation = mainWindow.Dispatcher.BeginInvoke((obj) =>
                                              {
                                                 var reader = new ZXing.BarcodeReader {TryHarder = false};
                                                 return reader.Decode((Bitmap)obj);
                                              }, bmp);
         operation.Wait();
         if (operation.Result != null)
         {
            text.TextContent = ((ZXing.Result)(operation.Result)).Text;
         }
         else
         {
            text.TextContent = "No barcode found.";
         }
      }

      private const string bitmapString =
         "Qk0yJAAAAAAAADYEAAAoAAAAWQAAAFkAAAABAAgAAAAAAPwfAAAAAAAAAAAAAAABAAAAAAAABAIEAPz+/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAAAAAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEAAAABAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQAAAAEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAAAAAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEAAAABAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQAAAAEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAAAAAQEBAQEBAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAEBAQAAAAAAAAEBAQEBAQEBAQEBAQEBAQEBAQAAAAEBAQEBAQEBAQEBAQAAAAEBAQEBAQAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQEBAAAAAAAAAQEBAQEBAQEBAQEBAQEBAQEBAAAAAQEBAQEBAQEBAQEBAAAAAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAAAAAAAAAAAAAAAAAAAAAAAAAABAQEAAAAAAAABAQEBAQEBAQEBAQEBAQEBAQEAAAABAQEBAQEBAQEBAQEAAAABAQEBAQEAAAABAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQEBAQEBAQEBAQEBAQAAAAEBAQAAAAAAAAEBAQAAAAAAAAEBAQAAAAEBAQEBAQEBAQAAAAAAAAAAAAEBAQAAAAAAAAAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAQEBAQEBAQEBAQEBAAAAAQEBAAAAAAAAAQEBAAAAAAAAAQEBAAAAAQEBAQEBAQEBAAAAAAAAAAAAAQEBAAAAAAAAAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEBAQEBAQEBAQEBAQEAAAABAQEAAAAAAAABAQEAAAAAAAABAQEAAAABAQEBAQEBAQEAAAAAAAAAAAABAQEAAAAAAAAAAAABAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQAAAAAAAAAAAAEBAQAAAAEBAQEBAQEBAQAAAAAAAAAAAAEBAQEBAQEBAQAAAAAAAAEBAQAAAAAAAAEBAQAAAAAAAAAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAAAAAAAAAAAAAQEBAAAAAQEBAQEBAQEBAAAAAAAAAAAAAQEBAQEBAQEBAAAAAAAAAQEBAAAAAAAAAQEBAAAAAAAAAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEAAAAAAAAAAAABAQEAAAABAQEBAQEBAQEAAAAAAAAAAAABAQEBAQEBAQEAAAAAAAABAQEAAAAAAAABAQEAAAAAAAAAAAABAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQAAAAAAAAAAAAEBAQAAAAEBAQAAAAEBAQEBAQAAAAAAAAEBAQAAAAEBAQEBAQAAAAAAAAAAAAEBAQAAAAEBAQAAAAAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAAAAAAAAAAAAAQEBAAAAAQEBAAAAAQEBAQEBAAAAAAAAAQEBAAAAAQEBAQEBAAAAAAAAAAAAAQEBAAAAAQEBAAAAAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEAAAAAAAAAAAABAQEAAAABAQEAAAABAQEBAQEAAAAAAAABAQEAAAABAQEBAQEAAAAAAAAAAAABAQEAAAABAQEAAAAAAAABAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQAAAAAAAAAAAAEBAQAAAAEBAQAAAAAAAAAAAAAAAAAAAAEBAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEBAQAAAAEBAQEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAAAAAAAAAAAAAQEBAAAAAQEBAAAAAAAAAAAAAAAAAAAAAQEBAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQEBAAAAAQEBAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEAAAAAAAAAAAABAQEAAAABAQEAAAAAAAAAAAAAAAAAAAABAQEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAQEAAAABAQEBAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQEBAQEBAQEBAQEBAQAAAAEBAQEBAQEBAQAAAAAAAAEBAQAAAAAAAAAAAAAAAAEBAQEBAQEBAQAAAAEBAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAQEBAQEBAQEBAQEBAAAAAQEBAQEBAQEBAAAAAAAAAQEBAAAAAAAAAAAAAAAAAQEBAQEBAQEBAAAAAQEBAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEBAQEBAQEBAQEBAQEAAAABAQEBAQEBAQEAAAAAAAABAQEAAAAAAAAAAAAAAAABAQEBAQEBAQEAAAABAQEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAQEBAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAEBAQEBAQAAAAEBAQAAAAAAAAAAAAEBAQEBAQAAAAEBAQAAAAEBAQAAAAAAAAEBAQEBAQAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQEBAQEBAAAAAQEBAAAAAAAAAAAAAQEBAQEBAAAAAQEBAAAAAQEBAAAAAAAAAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAAAAAAAAAAAAAAAAAAAAAAAAAABAQEBAQEAAAABAQEAAAAAAAAAAAABAQEBAQEAAAABAQEAAAABAQEAAAAAAAABAQEBAQEAAAABAQEBAQEBAAAAAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQAAAAAAAAAAAAEBAQEBAQAAAAEBAQEBAQAAAAEBAQEBAQEBAQAAAAEBAQEBAQAAAAEBAQEBAQEBAQEAAAABAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAAAAAAAAAAAAAQEBAQEBAAAAAQEBAQEBAAAAAQEBAQEBAQEBAAAAAQEBAQEBAAAAAQEBAQEBAQEBAQAAAAEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEAAAAAAAAAAAABAQEBAQEAAAABAQEBAQEAAAABAQEBAQEBAQEAAAABAQEBAQEAAAABAQEBAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQAAAAEBAQAAAAEBAQAAAAEBAQAAAAEBAQAAAAAAAAAAAAAAAAEBAQAAAAAAAAAAAAAAAAAAAAAAAAEBAQAAAAAAAAEBAQEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAAAAAQEBAAAAAQEBAAAAAQEBAAAAAQEBAAAAAAAAAAAAAAAAAQEBAAAAAAAAAAAAAAAAAAAAAAAAAQEBAAAAAAAAAQEBAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEAAAABAQEAAAABAQEAAAABAQEAAAABAQEAAAAAAAAAAAAAAAABAQEAAAAAAAAAAAAAAAAAAAAAAAABAQEAAAAAAAABAQEBAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQAAAAEBAQAAAAAAAAEBAQAAAAEBAQAAAAEBAQAAAAAAAAEBAQAAAAEBAQEBAQEBAQEBAQEBAQEBAQEBAQAAAAEBAQAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAAAAAQEBAAAAAAAAAQEBAAAAAQEBAAAAAQEBAAAAAAAAAQEBAAAAAQEBAQEBAQEBAQEBAQEBAQEBAQEBAAAAAQEBAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEAAAABAQEAAAAAAAABAQEAAAABAQEAAAABAQEAAAAAAAABAQEAAAABAQEBAQEBAQEBAQEBAQEBAQEBAQEAAAABAQEAAAABAQEBAQEBAAAAAQEBAQEBAQAAAAAAAAEBAQEBAQEBAQEBAQAAAAEBAQAAAAEBAQEBAQEBAQEBAQAAAAEBAQAAAAEBAQAAAAEBAQAAAAAAAAAAAAAAAAAAAAAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAAAAAQEBAQEBAQEBAQEBAAAAAQEBAAAAAQEBAQEBAQEBAQEBAAAAAQEBAAAAAQEBAAAAAQEBAAAAAAAAAAAAAAAAAAAAAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAAAAAABAQEBAQEBAQEBAQEAAAABAQEAAAABAQEBAQEBAQEBAQEAAAABAQEAAAABAQEAAAABAQEAAAAAAAAAAAAAAAAAAAAAAAABAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQAAAAAAAAAAAAAAAAEBAQAAAAAAAAEBAQAAAAAAAAEBAQAAAAEBAQAAAAAAAAEBAQEBAQEBAQAAAAEBAQEBAQAAAAEBAQEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAAAAAAAAAAAAAAAAAQEBAAAAAAAAAQEBAAAAAAAAAQEBAAAAAQEBAAAAAAAAAQEBAQEBAQEBAAAAAQEBAQEBAAAAAQEBAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEAAAAAAAAAAAAAAAABAQEAAAAAAAABAQEAAAAAAAABAQEAAAABAQEAAAAAAAABAQEBAQEBAQEAAAABAQEBAQEAAAABAQEBAQEBAQEBAAAAAQEBAQEBAQEBAQAAAAEBAQAAAAEBAQEBAQAAAAAAAAAAAAAAAAAAAAEBAQAAAAEBAQEBAQAAAAEBAQEBAQAAAAEBAQEBAQEBAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEBAQEBAQEBAAAAAQEBAAAAAQEBAQEBAAAAAAAAAAAAAAAAAAAAAQEBAAAAAQEBAQEBAAAAAQEBAQEBAAAAAQEBAQEBAQEBAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQEBAQEBAQEAAAABAQEAAAABAQEBAQEAAAAAAAAAAAAAAAAAAAABAQEAAAABAQEBAQEAAAABAQEBAQEAAAABAQEBAQEBAQEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQAAAAAAAAEBAQAAAAEBAQAAAAEBAQEBAQEBAQEBAQEBAQEBAQAAAAEBAQEBAQEBAQAAAAEBAQAAAAAAAAAAAAAAAAAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAAAAAAAAAQEBAAAAAQEBAAAAAQEBAQEBAQEBAQEBAQEBAQEBAAAAAQEBAQEBAQEBAAAAAQEBAAAAAAAAAAAAAAAAAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEAAAAAAAABAQEAAAABAQEAAAABAQEBAQEBAQEBAQEBAQEBAQEAAAABAQEBAQEBAQEAAAABAQEAAAAAAAAAAAAAAAAAAAABAQEBAQEBAAAAAQEBAQEBAQAAAAAAAAEBAQEBAQAAAAAAAAAAAAAAAAEBAQAAAAEBAQAAAAEBAQEBAQEBAQAAAAEBAQAAAAEBAQEBAQAAAAAAAAEBAQEBAQAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAAAAAQEBAQEBAAAAAAAAAAAAAAAAAQEBAAAAAQEBAAAAAQEBAQEBAQEBAAAAAQEBAAAAAQEBAQEBAAAAAAAAAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAAAAAABAQEBAQEAAAAAAAAAAAAAAAABAQEAAAABAQEAAAABAQEBAQEBAQEAAAABAQEAAAABAQEBAQEAAAAAAAABAQEBAQEAAAABAQEBAQEBAAAAAQEBAQEBAQEBAQAAAAEBAQAAAAEBAQAAAAEBAQAAAAEBAQEBAQAAAAAAAAEBAQEBAQAAAAAAAAAAAAEBAQEBAQAAAAAAAAAAAAAAAAAAAAEBAQEBAQEBAQEAAAABAQEBAQEBAQEBAAAAAQEBAAAAAQEBAAAAAQEBAAAAAQEBAQEBAAAAAAAAAQEBAQEBAAAAAAAAAAAAAQEBAQEBAAAAAAAAAAAAAAAAAAAAAQEBAQEBAQEBAQAAAAEBAQEBAQEBAQEAAAABAQEAAAABAQEAAAABAQEAAAABAQEBAQEAAAAAAAABAQEBAQEAAAAAAAAAAAABAQEBAQEAAAAAAAAAAAAAAAAAAAABAQEBAQEBAQEBAAAAAQEBAQEBAQAAAAAAAAEBAQAAAAAAAAEBAQAAAAEBAQEBAQAAAAEBAQAAAAAAAAEBAQAAAAEBAQAAAAEBAQAAAAEBAQEBAQEBAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAAAAAQEBAAAAAAAAAQEBAAAAAQEBAQEBAAAAAQEBAAAAAAAAAQEBAAAAAQEBAAAAAQEBAAAAAQEBAQEBAQEBAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAAAAAABAQEAAAAAAAABAQEAAAABAQEBAQEAAAABAQEAAAAAAAABAQEAAAABAQEAAAABAQEAAAABAQEBAQEBAQEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQAAAAAAAAEBAQEBAQEBAQAAAAAAAAAAAAAAAAEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEAAAABAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAAAAAAAAAQEBAQEBAQEBAAAAAAAAAAAAAAAAAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQAAAAEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEAAAAAAAABAQEBAQEBAQEAAAAAAAAAAAAAAAABAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAAAAAQEBAQEBAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAEBAQAAAAEBAQAAAAEBAQAAAAEBAQAAAAEBAQAAAAEBAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQEBAAAAAQEBAAAAAQEBAAAAAQEBAAAAAQEBAAAAAQEBAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAAAAAAAAAAAAAAAAAAAAAAAAAABAQEAAAABAQEAAAABAQEAAAABAQEAAAABAQEAAAABAQEAAAAAAAAAAAAAAAAAAAAAAAAAAAABAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQEBAQEBAQEBAQEBAQAAAAEBAQEBAQAAAAEBAQAAAAEBAQEBAQAAAAEBAQAAAAEBAQAAAAEBAQEBAQEBAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAQEBAQEBAQEBAQEBAAAAAQEBAQEBAAAAAQEBAAAAAQEBAQEBAAAAAQEBAAAAAQEBAAAAAQEBAQEBAQEBAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEBAQEBAQEBAQEBAQEAAAABAQEBAQEAAAABAQEAAAABAQEBAQEAAAABAQEAAAABAQEAAAABAQEBAQEBAQEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQAAAAAAAAAAAAEBAQAAAAEBAQEBAQEBAQAAAAAAAAAAAAEBAQAAAAEBAQAAAAEBAQAAAAEBAQAAAAAAAAAAAAEBAQAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAAAAAAAAAAAAAQEBAAAAAQEBAQEBAQEBAAAAAAAAAAAAAQEBAAAAAQEBAAAAAQEBAAAAAQEBAAAAAAAAAAAAAQEBAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEAAAAAAAAAAAABAQEAAAABAQEBAQEBAQEAAAAAAAAAAAABAQEAAAABAQEAAAABAQEAAAABAQEAAAAAAAAAAAABAQEAAAABAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQAAAAAAAAAAAAEBAQAAAAEBAQEBAQEBAQAAAAAAAAAAAAAAAAEBAQEBAQAAAAEBAQAAAAEBAQAAAAAAAAAAAAEBAQAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAAAAAAAAAAAAAQEBAAAAAQEBAQEBAQEBAAAAAAAAAAAAAAAAAQEBAQEBAAAAAQEBAAAAAQEBAAAAAAAAAAAAAQEBAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEAAAAAAAAAAAABAQEAAAABAQEBAQEBAQEAAAAAAAAAAAAAAAABAQEBAQEAAAABAQEAAAABAQEAAAAAAAAAAAABAQEAAAABAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQAAAAAAAAAAAAEBAQAAAAEBAQEBAQAAAAEBAQAAAAEBAQAAAAAAAAEBAQAAAAEBAQAAAAEBAQAAAAAAAAAAAAEBAQAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAAAAAAAAAAAAAQEBAAAAAQEBAQEBAAAAAQEBAAAAAQEBAAAAAAAAAQEBAAAAAQEBAAAAAQEBAAAAAAAAAAAAAQEBAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEAAAAAAAAAAAABAQEAAAABAQEBAQEAAAABAQEAAAABAQEAAAAAAAABAQEAAAABAQEAAAABAQEAAAAAAAAAAAABAQEAAAABAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQEBAQEBAQEBAQEBAQAAAAEBAQEBAQEBAQEBAQAAAAAAAAEBAQEBAQEBAQAAAAEBAQAAAAEBAQEBAQEBAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAQEBAQEBAQEBAQEBAAAAAQEBAQEBAQEBAQEBAAAAAAAAAQEBAQEBAQEBAAAAAQEBAAAAAQEBAQEBAQEBAQEBAQEBAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAABAQEBAQEBAQEBAQEBAQEAAAABAQEBAQEBAQEBAQEAAAAAAAABAQEBAQEBAQEAAAABAQEAAAABAQEBAQEBAQEBAQEBAQEAAAABAQEBAQEBAAAAAQEBAQEBAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAEBAQAAAAAAAAAAAAAAAAEBAQEBAQAAAAEBAQEBAQEBAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAEBAQEBAQEAAAABAQEBAQEBAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQEBAAAAAAAAAAAAAAAAAQEBAQEBAAAAAQEBAQEBAQEBAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQEBAQEBAQAAAAEBAQEBAQEAAAAAAAAAAAAAAAAAAAAAAAAAAAABAQEAAAAAAAAAAAAAAAABAQEBAQEAAAABAQEBAQEBAQEAAAAAAAAAAAAAAAAAAAAAAAAAAAABAQEBAQEBAAAAAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEAAAABAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQAAAAEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAAAAAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEAAAABAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQAAAAEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAAAAAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEAAAA=";
   }
}
