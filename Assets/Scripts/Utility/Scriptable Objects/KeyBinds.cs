using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[CreateAssetMenu(fileName = "Command.asset", menuName = "Utility/Bindable Command")]
public class Command : ScriptableObject
{
    public static Dictionary<string, Command> commands = new Dictionary<string, Command>();

    [Serializable]
    public class PressEvent : UnityEvent { };

    [Serializable]
    public class ReleaseEvent : DynamicEvent<float> { };

    public PressEvent OnPress = new PressEvent();
    public ReleaseEvent OnRelease = new ReleaseEvent();

    private void Awake()
    {
        commands.Add(name, this);
    }
}

/// <summary>
/// Batch of keys to commands bindings that can be edited in the inspector
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "Keybind.asset", menuName = "Utility/Key Bindings")]
public class KeyBinds:ScriptableObject
{
    public static bool Logging = false;

    /// <summary>
    /// Map a specific key to specific command and keep track of when that key is held
    /// </summary>
    /// <remarks>Serializable, so you cna configure it in the inspector</remarks>
    [Serializable]
    public class KeyBind
    {
        public Command command;
        public KeyCode key;

        [NonSerialized]
        public bool keyIsHeld = false;
        [NonSerialized]
        public float pressTime;

        public string ToJSON()
        {
            return string.Format("\"{0}\":\"{1}\",\n\r", key, command.name);
        }

        public bool TryParse(string JSON )
        {
            if (string.IsNullOrEmpty(JSON))
                return false;

            if (!JSON.EndsWith(","))
                return false;

            JSON = JSON.Substring(0, JSON.Length - 1);
            JSON = JSON.Replace("\"", "");

            var parts = JSON.Split(":".ToCharArray(), 2);

            if (parts.Length == 1)
                return false;

            if (!Enum.IsDefined(typeof(KeyCode), parts[0]))
                return false;

            key = (KeyCode)Enum.Parse(typeof(KeyCode), parts[0]);

            if (!Command.commands.ContainsKey(parts[1]))
                return false;

            command = Command.commands[parts[1]];

            if (command == null)
                return false;            

            return true;
        }//end TryParse

        public void CheckForKeyPress()
        {
            bool IsHeld = Input.GetKey(key);

            if (IsHeld == keyIsHeld)
                return;

            if (keyIsHeld)
            {
                //releasing
                if(KeyBinds.Logging)
                    Debug.LogFormat("Released {0} key", key);

                command.OnRelease.DynamicInvoke(Time.time - pressTime);
            }
            else
            {
                //pressing
                if (KeyBinds.Logging)
                    Debug.LogFormat("Pressed {0} key", key);

                pressTime = Time.time;
                command.OnPress.Invoke();
            }

            keyIsHeld = IsHeld;
        }
    }//end KeyBind

    //a Dictionary<keycode, Command> would be better, but the Inspector doesn't like those as much as arrays
    public KeyBind[] bindings;

    /// <summary>
    /// Call per-frame to check if any bound keys are pressed
    /// </summary>
    public void CheckForCommands()
    {
        foreach(KeyBind bind in bindings)
        {
            bind.CheckForKeyPress();
        }
    }

    /// <summary>
    /// Is the key for the specified command pressed?
    /// </summary>
    /// <param name="command">command to be checked</param>
    /// <returns>true if the corrisponding key is pressed</returns>
    public bool GetCmd(Command command)
    {
        foreach (KeyBind bind in bindings)
        {
            if (bind.command == command)
                return Input.GetKey(bind.key);
        }

        if (KeyBinds.Logging)
            Debug.LogWarningFormat("Command {0} is not bound to any key", command);

        return false;
    }

    /// <summary>
    /// Name the key that this command is bound to
    /// </summary>
    /// <param name="command"></param>
    /// <returns>name of the key the command is bound to OR null if it is unbound</returns>
    /// <remarks>Intended for use in tutorials to tell the player which key to press</remarks>
    public string GetCmdKeyName(Command command)
    {
        foreach (KeyBind bind in bindings)
        {
            if (bind.command == command)
                return string.Format("{0}", bind.key);
        }

        return null;
    }

    /// <summary>
    /// Check for the same key being bound to multiple commands
    /// </summary>
    /// <param name="failures">A message describing which keys are multiply bound</param>
    /// <returns>true if no keys are bound to more than one command</returns>
    public bool IsValid(ref string failures)
    {
        bool result = true;
        var failmsg = new System.Text.StringBuilder();
        var keys = bindings.GroupBy(x => x.key);

        foreach (var keyset in keys)
        {
            if (keyset.Count() == 1)
                continue;

            result = false;

            foreach (var binding in keyset)
            {
                failmsg.AppendFormat("{0} is bound to {1}.  ", binding.key, binding.command);
            }
        }

        if(result == false)
        {
            failures = failmsg.ToString();
        }

        return result;
    }

    public bool Save(string filePath)
    {
        try
        {
            //File.WriteAllText(filePath, JsonUtility.ToJson(this));

            using (var sw = new StreamWriter(filePath))
            {
                sw.WriteLine("{\"bindings\":");
                sw.WriteLine("//first part of each line is a keycode");
                sw.WriteLine("//see https://docs.unity3d.com/ScriptReference/KeyCode.html for a complete list of key codes");
                sw.WriteLine("//second part of each line is a command");
                sw.Write("//the following commands are available: ");

                var commands = Enum.GetNames(typeof(Command));
                foreach (string command in commands)
                {
                    sw.Write(command); sw.Write(", ");
                }
                sw.Write("\n\r\n\r");

                foreach (KeyBind bind in bindings)
                {
                    sw.Write(bind.ToJSON());
                }
                sw.Write("]}");
            }
        } catch (Exception oops)
        {
            Debug.LogException(oops);

            return false;
        }

        return true;
    } //end Save

    public static KeyBinds Load(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        try
        {
            //var json = File.ReadAllText(filePath);

            //return JsonUtility.FromJson<KeyBinds>(json);
            var json = File.ReadAllLines(filePath);

            if (json == null || json.Length < 2)
                return null;

            if (json[0]!= "{\"bindings\":")
                return null;

            var buffer = new List<KeyBind>();

            for(int line=1;line < json.Length;line++)
            {
                var bind = new KeyBind();

                if (json[line].StartsWith("//") || json[line].Length == 0)
                    continue; //skip empty and comment lines

                if (json[line] == "]}")
                    continue; //ignore the closing brace

                if (bind.TryParse(json[line]))
                {
                    buffer.Add(bind);
                } else {
                    Debug.LogWarningFormat("invalid keybinding at {0}:{1} \"{2}\"", filePath, line, json[line]);
                }   
            }

            var result = ScriptableObject.CreateInstance<KeyBinds>();

            result.bindings = buffer.ToArray();

            return result;
        }
        catch (Exception oops)
        {
            Debug.LogException(oops);

            return null;
        }
    } //end Load
}//end class
