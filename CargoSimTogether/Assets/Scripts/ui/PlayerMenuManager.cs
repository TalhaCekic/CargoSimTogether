using Mirror;
using UnityEngine;
using Player.Manager;
using UnityEngine.SceneManagement;

namespace Player.PlayerMenu
{
    public class PlayerMenuManager : NetworkBehaviour
    {
        [Header("Pause")] public bool isPause;
        public GameObject pauseMenu;
        public bool chande;
        private InputManager _inputManager;
        private PlayerInteract _playerInteract;
        //public bool mouseActivity;
    
        void Start()
        {
            chande = false;
            isPause = false;
            //mouseActivity = false;
            pauseMenu.SetActive(false);
            _inputManager = GetComponent<InputManager>();
            _playerInteract = GetComponent<PlayerInteract>();
            
            if(!isLocalPlayer)return;
            OnPauseSelect();
        }
    
        private void Update()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)return;
            if (isLocalPlayer)
            {
                CursorState();
            }
        }
    
       public void OnPauseSelect()
        {
            if (isPause)
            {
                _playerInteract.mouseActivity = true;
                pauseMenu.SetActive(true);
            }
            else
            {
                _playerInteract.mouseActivity = false;
                pauseMenu.SetActive(false);
            }
        }
        //CursorSettings
        private void CursorState()
        {
            if (_playerInteract.mouseActivity)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
