using CustomInspector;
using PixelEngine.Core.Initialization;
using PixelEngine.Systems.ServiceLocator;
using UnityEngine;

namespace PixelEngine.Core.GameManagement
{
    public class MainMenuManager : MonoBehaviour
    {
        public void NewGame()
        {
            ServiceLocator.GlobalContainer.Get(out GameManager gameManager);

            var context = new GameContext(EGameMode.NewGame);
            
            gameManager.StartGame(context);
        }

        public void LoadGame()
        {
            //TODO: get chosen save
        }

        public void Continue()
        {
            //TODO: get latest save
        }

        public void Options()
        {
        
        }

        public void Quit()
        {
        
        }
    }
}
