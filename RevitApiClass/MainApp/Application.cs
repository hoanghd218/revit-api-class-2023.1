using MainApp.CreatingElement.CreateSheetsFromExcel;
using MainApp.Geometry;
using Nice3point.Revit.Toolkit.External;
using Serilog.Events;

namespace MainApp
{
   [UsedImplicitly]
   public class Application : ExternalApplication
   {
      public override void OnStartup()
      {
         CreateLogger();
         CreateRibbon();
      }

      public override void OnShutdown()
      {
         Log.CloseAndFlush();
      }

      private void CreateRibbon()
      {
         var panel = Application.CreatePanel("BimApp", "MainApp");

         var fowmworkButton = panel.AddPushButton<FormworkCmd>("Fowmwork");
         fowmworkButton.SetImage("/MainApp;component/Resources/Icons/RibbonIcon16.png");
         fowmworkButton.SetLargeImage("/MainApp;component/Resources/Icons/RibbonIcon32.png");


         var sheetButton = panel.AddPushButton<CreateSheetsFromExcelCmd>("Sheets");
         sheetButton.SetImage("/MainApp;component/Resources/Icons/RibbonIcon16.png");
         sheetButton.SetLargeImage("/MainApp;component/Resources/Icons/RibbonIcon32.png");
      }

      private static void CreateLogger()
      {
         const string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

         Log.Logger = new LoggerConfiguration()
             .WriteTo.Debug(LogEventLevel.Debug, outputTemplate)
             .MinimumLevel.Debug()
             .CreateLogger();

         AppDomain.CurrentDomain.UnhandledException += (_, args) =>
         {
            var e = (Exception)args.ExceptionObject;
            Log.Fatal(e, "Domain unhandled exception");
         };
      }
   }
}