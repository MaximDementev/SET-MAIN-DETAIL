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
        public Result OnShutdown(UIControlledApplication application)
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location,
                   iconsDirectoryPath = Path.GetDirectoryName(assemblyLocation) + @"\icons\";

            string tabName = "KRGP";
            string panelName = "Арматура";
            string ribbonName = "Каркасы";


            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch { }

            #region 1. Архитектура
            {
                RibbonPanel panel = application.GetRibbonPanels(tabName).Where(p => p.Name == panelName).FirstOrDefault();
                if (panel == null)
                {
                    panel = application.CreateRibbonPanel(tabName, panelName);
                }

                panel.AddItem(new PushButtonData(nameof(StartRebarSelection), ribbonName, assemblyLocation, typeof(StartRebarSelection).FullName)
                {
                    LargeImage = new BitmapImage(new Uri(iconsDirectoryPath + "StartRebarSelection.png"))
                });
            }
            #endregion


            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            throw new NotImplementedException();
        }
    }
}
