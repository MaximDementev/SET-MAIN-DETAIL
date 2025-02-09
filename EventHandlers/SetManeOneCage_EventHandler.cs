using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SET_MAIN_DETAIL
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class SetManeOneCage_EventHandler : IExternalEventHandler
    {
        #region Private Fields
        private ExternalEvent _externalEvent;
        private DisplayRebarCages _parentForm;
        private RebarCages _rebarCages;
        private OneRebarCage _oneRebarCage;

        private static Document _doc;
        private UIDocument _uiDoc;
        #endregion

        #region Constructor
        public void Initialize()
        {
            _externalEvent = ExternalEvent.Create(this);
        }
        #endregion

        //------------------ Methods ----------------------------------

        public RebarCages Raise(DisplayRebarCages parentForm, RebarCages rebarCages)
        {
            _parentForm = parentForm;

            _rebarCages = rebarCages;
            _parentForm.Hide();

            _externalEvent.Raise();
            return _rebarCages;
        }

        public void Execute(UIApplication app)
        {

            _uiDoc = app.ActiveUIDocument;
            _doc = _uiDoc.Document;
            _oneRebarCage = new OneRebarCage(_rebarCages.CageName);


            try
            {
                _oneRebarCage.AddInstances(
                    RebarSelectionHandler.RebarSelection(_uiDoc, $"Выбор каркаса {_rebarCages.CageName} (Esc - отмена)")
                    );
                OneRebarSelection($"Выбор главной детали {_rebarCages.CageName} (Esc - отмена)");

                _rebarCages.SetMainOneRebarCage (_oneRebarCage);
                _rebarCages.DivideAllInstancesToCages();

                _rebarCages.SetFlagAsMainRebar(0);
                _rebarCages.SetMainRebarInstance();
                _rebarCages.ValidateRebarsCages();

                using (Transaction transaction = new Transaction(_doc))
                {
                    transaction.Start($"Установка главной детали {_rebarCages.CageName}");
                    _rebarCages.RebarInstancesList.ForEach(rebarInstance =>
                    {
                        try
                        {
                            rebarInstance.SetAllParamValue();
                        }

                    }

                    if (!isAddedToExistingCage)
                    {
                        DimensionBox dimensionBox = new DimensionBox(rebarInstance.GetLocationPoint(), dimensionBoxRadius);
                        OneRebarCage newOneRebarCage = new OneRebarCage(rebarInstance.GetRebarCageName());
                        newOneRebarCage.DimensionBox = dimensionBox;
                        newOneRebarCage.AddInstance(rebarInstance);
                        rebarCages.oneRebarCagesList.Add(newOneRebarCage);
                    }
                }

                string MainRebarInstanceName = _oneRebarCage.MainRebarInstance.GetRebarInstanceName();
                foreach (OneRebarCage cage in rebarCages.oneRebarCagesList)
                {
                    cage.SetMainRebarInstance(MainRebarInstanceName);
                }

                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start($"Установка главной детали");
                    _parentForm.TaskCount = rebarCages.RebarInstancesList.Count;
                    rebarCages.RebarInstancesList.ForEach(rebarInstance => 
                    {
                        Thread.Sleep(300);
                        rebarInstance.SetAllParamValue();
                        _parentForm.CountOfComplete++;

                    });

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", ex.Message);
            }


            _parentForm.Show();

        }

        public string GetName()
        {
            return "SetManeOneCage Handler";
        }

        //--------------- private methods ------------------------
        

        private void OneRebarSelection(string processName)
        {
            Selection selection = _uiDoc.Selection;

            Reference SelectedReference = selection
                .PickObject(ObjectType.Element, new SeparatorSelectionFilter_Rebar(), processName);

            FamilyInstance element = _doc.GetElement(SelectedReference.ElementId) as FamilyInstance;

            RebarInstance rebarInstance = new RebarInstance(element);
            if (_oneRebarCage != null)
                _oneRebarCage.AddMainRebar(rebarInstance);

        }
          
    }
}
