using UnityEngine;
using UnityEngine.InputSystem;

public class VRMenuToggle : MonoBehaviour
{
    //The two UI panels representing the menu
    public GameObject menuUI1;
    public GameObject menuUI2;
    //The camera is used to position the menu in front of it 
    public Transform cameraTransform; 

    public InputActionProperty toggleAction;

    private bool isOpen = false;

    void Start()
    {
        //Menu starts hidden
        menuUI1.SetActive(false);
        menuUI2.SetActive(false);
    }

    void Update()
    {
        // We listen for each frame if button menu was clicked
        if (toggleAction.action != null && toggleAction.action.WasPressedThisFrame())
        {
            ToggleMenu();
        }
    }

    void ToggleMenu()
    {
        isOpen = !isOpen;

        menuUI1.SetActive(isOpen);
        menuUI2.SetActive(isOpen);

        if (isOpen)
        {
            //Menus positions are placed 4m in front of the camera
            Vector3 basePos = cameraTransform.position + cameraTransform.forward * 4f;

            Vector3 pos1 = basePos;
            pos1.y = basePos.y;

            //Menus are rotated to face the camera 
            menuUI1.transform.SetPositionAndRotation(
                basePos,
                Quaternion.LookRotation(cameraTransform.forward)
            );

            //Title is placed 3 m higher
            menuUI2.transform.SetPositionAndRotation(
                new Vector3(basePos.x, basePos.y + 3f, basePos.z),
                Quaternion.LookRotation(cameraTransform.forward)
            );
        }
    }
}