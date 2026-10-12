using UnityEngine;

public class LockCursor : MonoBehaviour
{
    private void Start()
    {
        // I-lock at itago ang mouse cursor sa game window
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Pindutin ang Escape para maibalik ang cursor kung kailangan
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}