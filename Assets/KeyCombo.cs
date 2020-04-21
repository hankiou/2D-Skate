using UnityEngine;
public class KeyCombo
{
    public string[] buttons;
    private int currentIndex = 0; //moves along the array as buttons are pressed
 
    public float allowedTimeBetweenButtons = 0.3f; //tweak as needed
    private float timeLastButtonPressed;
 
    public KeyCombo(string[] b)
    {
        buttons = b;
    }
 
    //usage: call this once a frame. when the combo has been completed, it will return true
    public bool Check()
    {
        if(Time.time > timeLastButtonPressed + allowedTimeBetweenButtons) currentIndex = 0;
        {
            if(currentIndex < buttons.Length)
            {
                if(buttons[currentIndex] == KeyPress())
                {
                    timeLastButtonPressed = Time.time;
                    currentIndex++;
                }
 
                if(currentIndex >= buttons.Length)
                {
                    currentIndex = 0;
                    return true;
                }
                else return false;
            }
        }
 
        return false;
    }

    public string KeyPress(){ // Basically defines keys

        // L-Joystick
        if(Input.GetAxisRaw("Vertical") == -1) return "down";
        if(Input.GetAxisRaw("Vertical") > 0.8f) return "up";
        if(Input.GetAxisRaw("Horizontal") == -1) return "left";
        if(Input.GetAxisRaw("Horizontal") == 1) return "right";
        if(Input.GetAxisRaw("Horizontal") > 0.1f && Input.GetAxisRaw("Horizontal") < 0.8f && Input.GetAxisRaw("Vertical") > 0.1f && Input.GetAxisRaw("Vertical") < 0.8f) return "upright";

        // Letter Buttons
        if(Input.GetButton("KeyA")) return "keyA";
        if(Input.GetButton("KeyB")) return "keyB";
        if(Input.GetButton("KeyX")) return "keyX";
        if(Input.GetButton("KeyY")) return "keyY";
        if(Input.GetButton("JoyLeftClick")) return "JoyLeftClick";

        return "null";
    }
} 