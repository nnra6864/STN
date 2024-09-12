using UnityEngine;

namespace Core
{
    public class InputHandler : MonoBehaviour
    {
        //Used to block selection from happening if pointer has moved
        [SerializeField] private float _selectionMoveThreshold = 0.1f;
        private float _selectionMoveDelta;
        
        //Used to store references to institutions
        private Institutions.Institution _store, _workshop, _factory, _lab;
        
        private void Awake()
        {
            _store = FindFirstObjectByType<Institutions.Store>().GetComponent<Institutions.Institution>();
            _workshop = FindFirstObjectByType<Institutions.Workshop>().GetComponent<Institutions.Institution>();
            _factory = FindFirstObjectByType<Institutions.Factory>().GetComponent<Institutions.Institution>();
            _lab = FindFirstObjectByType<Institutions.Laboratory>().GetComponent<Institutions.Institution>();
        }

        private void Update()
        {
            //Return if Won or Lost
            if (Stats.HasSavedTheNord || Stats.NordScript.IsDestroyed) return;
            
            //Run the keyboard input functions
            InstitutionsInput();
            ToolInput();
            
            //Handle the mouse input
            MouseInput();
            
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                //If no UI is active, open the pause menu, otherwise close the UI
                if (!PauseMenu.PauseMenuObject.activeSelf && Stats.SelectedInfo == null)
                    Stats.SelectedInfo = PauseMenu.PauseMenuObject;
                else
                {
                    PauseMenu.EscPauseMenu();
                    Stats.SelectedTile = null;
                }
            }
            
            //Toggle Vegetation Goals
            if (Input.GetKeyDown(KeyCode.Tab)) Stats.VegetationGoals.ToggleUI();
        }

        private void InstitutionsInput()
        {
            if (Input.GetKeyDown(KeyCode.S)) _store.ToggleUI();
            if (Input.GetKeyDown(KeyCode.W)) _workshop.ToggleUI();
            if (Input.GetKeyDown(KeyCode.D)) _factory.ToggleUI();
            if (Input.GetKeyDown(KeyCode.A)) _lab.ToggleUI();
        }

        private void ToolInput()
        {
            //Return if the planting UI is open
            if (Stats.SelectedTile != null) return;
            
            if (Input.GetKeyDown(KeyCode.Alpha1)) Hotbar.SelectedTool = Hotbar.Tools.Hand;
            if (Input.GetKeyDown(KeyCode.Alpha2)) Hotbar.SelectedTool = Hotbar.Tools.Shears;
            if (Input.GetKeyDown(KeyCode.Alpha3)) Hotbar.SelectedTool = Hotbar.Tools.Axe;
            if (Input.GetKeyDown(KeyCode.Alpha4)) Hotbar.SelectedTool = Hotbar.Tools.Workers;
            if (Input.GetKeyDown(KeyCode.Alpha5)) Hotbar.SelectedTool = Hotbar.Tools.Fertilizer;
            if (Input.GetKeyDown(KeyCode.Alpha6)) Hotbar.SelectedTool = Hotbar.Tools.WaterPurifier;
        }

        private void MouseInput()
        {
            if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Return))
                Stats.SelectedInteract?.Click();
            if (Input.GetKeyDown(KeyCode.Mouse1)) Stats.SelectedInteract?.RightClick();
        }
    }
}