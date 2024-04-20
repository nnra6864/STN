using System;
using UnityEngine;

namespace Core
{
    public class InputHandler : MonoBehaviour
    {
        private Institutions.Institution _store, _workshop, _factory, _lab;
        private void Awake()
        {
            _store = FindObjectOfType<Institutions.Store>().GetComponent<Institutions.Institution>();
            _workshop = FindObjectOfType<Institutions.Workshop>().GetComponent<Institutions.Institution>();
            _factory = FindObjectOfType<Institutions.Factory>().GetComponent<Institutions.Institution>();
            _lab = FindObjectOfType<Institutions.Laboratory>().GetComponent<Institutions.Institution>();
        }

        private void Update()
        {
            if (Stats.HasSavedTheNord || Stats.NordScript.IsDestroyed) return;
            InstitutionsInput();
            ToolInput();
            if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Return))
                Stats.SelectedInteract?.Click();
            if (Input.GetKeyDown(KeyCode.Mouse1)) Stats.SelectedInteract?.RightClick();
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (!PauseMenu.PauseMenuObject.activeSelf && Stats.SelectedInfo == null)
                    Stats.SelectedInfo = PauseMenu.PauseMenuObject;
                else
                {
                    PauseMenu.EscPauseMenu();
                    Stats.SelectedTile = null;
                }
            }
            if (Input.GetKeyDown(KeyCode.Tab)) Stats.VegetationGoals.ToggleUI();
        }

        void InstitutionsInput()
        {
            if (Input.GetKeyDown(KeyCode.S)) _store.ToggleUI();
            if (Input.GetKeyDown(KeyCode.W)) _workshop.ToggleUI();
            if (Input.GetKeyDown(KeyCode.D)) _factory.ToggleUI();
            if (Input.GetKeyDown(KeyCode.A)) _lab.ToggleUI();
        }
        
        void ToolInput()
        {
            if (Stats.SelectedTile != null) return;
            if (Input.GetKeyDown(KeyCode.Alpha1)) Hotbar.SelectedTool = Hotbar.Tools.Hand;
            if (Input.GetKeyDown(KeyCode.Alpha2)) Hotbar.SelectedTool = Hotbar.Tools.Shears;
            if (Input.GetKeyDown(KeyCode.Alpha3)) Hotbar.SelectedTool = Hotbar.Tools.Axe;
            if (Input.GetKeyDown(KeyCode.Alpha4)) Hotbar.SelectedTool = Hotbar.Tools.Workers;
            if (Input.GetKeyDown(KeyCode.Alpha5)) Hotbar.SelectedTool = Hotbar.Tools.Fertilizer;
            if (Input.GetKeyDown(KeyCode.Alpha6)) Hotbar.SelectedTool = Hotbar.Tools.WaterPurifier;
        }
    }
}