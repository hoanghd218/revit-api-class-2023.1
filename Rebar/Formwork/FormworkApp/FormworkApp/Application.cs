using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;
using System.Reflection;
using System.Windows.Media.Imaging;
using FormworkApp.ColumnRebar;
using FormworkApp.Commands;

namespace FormworkApp
{
   [UsedImplicitly]
   public class Application : ExternalApplication
   {
      public override void OnStartup()
      {
         CreateRibbon();
      }

      private void CreateRibbon()
      {
         var panel = Application.CreatePanel("Panel name", "FormworkApp");

         var showButton = panel.AddPushButton<ColumnRebarCmd>("Column Rebar");
         showButton.SetImage("/FormworkApp;component/Resources/Icons/RibbonIcon16.png");
         showButton.SetLargeImage("/FormworkApp;component/Resources/Icons/RibbonIcon32.png");


         panel.AddSeparator();

         var pullDown = panel.AddPullDownButton("pulldown", "Rebar");

         pullDown.SetImage("/FormworkApp;component/Resources/Icons/RibbonIcon16.png");
         pullDown.SetLargeImage("/FormworkApp;component/Resources/Icons/RibbonIcon32.png");

         var stirrupButton = pullDown.AddPushButton<TestCreateStirrupCmd>("Stirrup");
         var directShapeButton = pullDown.AddPushButton<CreateDirectShapeCmd>("Direct Shape");

         stirrupButton.SetImage("/FormworkApp;component/Resources/Icons/RibbonIcon16.png");
         stirrupButton.SetLargeImage("/FormworkApp;component/Resources/Icons/RibbonIcon32.png");

         directShapeButton.SetImage("/FormworkApp;component/Resources/Icons/RibbonIcon16.png");
         directShapeButton.SetLargeImage("/FormworkApp;component/Resources/Icons/RibbonIcon32.png");


         #region First way to do manually dont use library

         //Application.CreateRibbonTab("Bim App");
         //var panel = Application.CreateRibbonPanel("Bim App", "Rebar Tools");

         //var columnRebarButton =
         //   new PushButtonData("ColumnRebar", "Column Rebar", path, "FormworkApp.ColumnRebar.ColumnRebarCmd");


         //var pushButton = panel.AddItem(columnRebarButton) as PushButton;
         //pushButton.SetImage("/FormworkApp;component/Resources/Icons/RibbonIcon16.png");
         //pushButton.SetLargeImage("/FormworkApp;component/Resources/Icons/RibbonIcon32.png");

         #endregion
      }
   }
}