using UnityEngine;

/*
 * Used to easily override Input-System
 * Tobias Zimmerlin
 * Going to use InputCotroller from Stay_Alive
 * */
public class Controls
{

    public static bool GetButton(string button)
    {
        return Input.GetButton(button);
    }
    public static bool GetButtonDown(string button)
    {
        return Input.GetButtonDown(button);
    }
    public static bool GetButtonUp(string button)
    {
        return Input.GetButtonUp(button);
    }

    public static bool GetKey(KeyCode key)
    {
        return Input.GetKey(key);
    }
    public static bool GetKeyDown(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }
    public static bool GetKeyUp(KeyCode key)
    {
        return Input.GetKeyUp(key);
    }

    public static float GetAxis(string axis)
    {
        return Input.GetAxis(axis);
    }

}
