using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using System;
using System.Threading;

namespace SET_MAIN_DETAIL
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class SetManeOneCage_EventHandler : IExternalEventHandler
    {
        #region Private Fields
        private ExternalEvent _externalEvent;
        private static Document _doc;
        private UIDocument _uiDoc;

        private DisplayRebarCages _parentForm;

        private RebarCages _rebarCages;
        private OneRebarCage _oneRebarCage;

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

                using (Transaction transaction1 = new Transaction(_doc))
                {
                    bool isAddedToExistingCage = false;

                    foreach (OneRebarCage cage in _rebarCages.OneRebarCagesList)
                    {
                        transaction1.Start($"Установка главной детали {_rebarCages.CageName}");
                        _rebarCages.RebarInstancesList.ForEach(rebarInstance =>
                        {
                            if (cage.CheckElementIsInsideDimensionBox(rebarInstance))
                            {
                                try
                                {
                                    cage.AddInstance(rebarInstance);
                                    isAddedToExistingCage = true;
                                    rebarInstance.SetAllParamValue();
                                }
                                catch { }
                            }

                            else
                            {
                                DimensionBox dimensionBox = new DimensionBox(rebarInstance.GetLocationPoint(), _oneRebarCage.DimensionBox.Radius);
                                OneRebarCage newOneRebarCage = new OneRebarCage(rebarInstance.GetRebarCageName());
                                newOneRebarCage.DimensionBox = dimensionBox;
                                newOneRebarCage.AddInstance(rebarInstance);
                                _rebarCages.OneRebarCagesList.Add(newOneRebarCage);
                            }
                        });

                    }
                    transaction1.Commit();
                }

                string MainRebarInstanceName = _oneRebarCage.MainRebarInstance.GetRebarInstanceName();
                foreach (OneRebarCage cage in _rebarCages.OneRebarCagesList)
                {
                    cage.SetMainRebarInstance(MainRebarInstanceName);
                }

                using (Transaction transaction = new Transaction(_doc))
                {
                    transaction.Start($"Установка главной детали");
                    _parentForm.TaskCount = _rebarCages.RebarInstancesList.Count;
                    _rebarCages.RebarInstancesList.ForEach(rebarInstance => 
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
