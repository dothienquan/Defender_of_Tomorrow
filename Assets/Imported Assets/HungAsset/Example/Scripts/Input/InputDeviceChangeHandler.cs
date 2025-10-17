using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputDeviceChangeHandler : MonoBehaviour {

    public GameObject keyboardImage;
    public GameObject xboxImage;
    public GameObject ps4Image;

    void Awake() {
        PlayerInput input = FindObjectOfType<PlayerInput>();
        updateButtonImage(input.currentControlScheme);
    }

    void OnEnable() {
        InputUser.onChange += onInputDeviceChange;
    }

    void OnDisable() {
        InputUser.onChange -= onInputDeviceChange;
    }

    void onInputDeviceChange(InputUser user, InputUserChange change, InputDevice device) {
        if (change == InputUserChange.ControlSchemeChanged) {
            updateButtonImage(user.controlScheme.Value.name);
        }
    }

    void updateButtonImage(string schemeName) {
        if (schemeName.Equals("Xbox")) {
            keyboardImage.SetActive(false);
            xboxImage.SetActive(true);
            ps4Image.SetActive(false);
        }
        else if (schemeName.Equals("PS4")) {
            keyboardImage.SetActive(false);
            xboxImage.SetActive(false);
            ps4Image.SetActive(true);
        }
        else {
            keyboardImage.SetActive(true);
            xboxImage.SetActive(false);
            ps4Image.SetActive(false);
        }
    }
}