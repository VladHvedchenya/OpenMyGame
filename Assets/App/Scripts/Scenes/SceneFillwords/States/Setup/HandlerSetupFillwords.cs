using System;
using System.Threading.Tasks;
using App.Scripts.Infrastructure.GameCore.States.SetupState;
using App.Scripts.Infrastructure.LevelSelection;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels.View.ViewGridLetters;
using App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel;

namespace App.Scripts.Scenes.SceneFillwords.States.Setup
{
    public class HandlerSetupFillwords : IHandlerSetupLevel
    {
        private readonly ContainerGrid _containerGrid;
        private readonly IProviderFillwordLevel _providerFillwordLevel;
        private readonly IServiceLevelSelection _serviceLevelSelection;
        private readonly ViewGridLetters _viewGridLetters;
        private int _lastIndex;

        public HandlerSetupFillwords(IProviderFillwordLevel providerFillwordLevel,
            IServiceLevelSelection serviceLevelSelection,
            ViewGridLetters viewGridLetters, ContainerGrid containerGrid)
        {
            _providerFillwordLevel = providerFillwordLevel;
            _serviceLevelSelection = serviceLevelSelection;
            _viewGridLetters = viewGridLetters;
            _containerGrid = containerGrid;
        }

        public Task Process()
        {
            GridFillWords currentModel;
            bool canLoadLevel = false;

            do
            {
                currentModel = _providerFillwordLevel.LoadModel(_serviceLevelSelection.CurrentLevelIndex);

                if (currentModel != null)
                {
                    _viewGridLetters.UpdateItems(currentModel);
                    _containerGrid.SetupGrid(currentModel, _serviceLevelSelection.CurrentLevelIndex);

                    _lastIndex = _serviceLevelSelection.CurrentLevelIndex;
                    canLoadLevel = true;
                }
                else
                {
                    int currentIndex = _serviceLevelSelection.CurrentLevelIndex;

                    if (currentIndex > _lastIndex)
                        _serviceLevelSelection.UpdateSelectedLevel(currentIndex + 1);
                    else if (currentIndex < _lastIndex)
                        _serviceLevelSelection.UpdateSelectedLevel(currentIndex - 1);
                }
            }
            while (currentModel == null);

            return canLoadLevel == false ? throw new Exception() : Task.CompletedTask;
        }
    }
}