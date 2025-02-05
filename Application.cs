using Autodesk.Revit.UI;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace SET_MAIN_DETAIL
{
    public class Application : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location,
                   iconsDirectoryPath = Path.GetDirectoryName(assemblyLocation) + @"\icons\";

            string tabName = "KRGP";
            string panelName = "Арматура";
            string ribbonName = "Главная деталь\nкаркаса";

            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch { }

            #region 1. Конструктив
            {
                RibbonPanel panel = application.GetRibbonPanels(tabName).Where(p => p.Name == panelName).FirstOrDefault();
                if (panel == null)
                {
                    panel = application.CreateRibbonPanel(tabName, panelName);
                }

                panel.AddItem(new PushButtonData(nameof(StartRebarSelection), ribbonName, assemblyLocation, typeof(StartRebarSelection).FullName)
                {
                    LargeImage = new BitmapImage(new Uri(iconsDirectoryPath + "SET_MAIN_DETAIL.png"))
                });
            }
            #endregion


            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
