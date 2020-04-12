using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Text))]
public class Teletype : MonoBehaviour
{
    /// <summary>
    /// Constant to indicate a float is not a real time
    /// </summary>
    private const float dontRead = float.NegativeInfinity;

    /// <summary>
    /// This component can operate in 3 different modes that control how it types out it's message
    /// </summary>
    /// <remarks>
    /// 0) FixedRate takes the same SecondsPerLetter for each charecter in the message
    /// 1) FixedDuration always takes Duration seconds to type a message, no matter how long that message is
    /// 2) HuntAndPeck simulates a user typing the message, varying SecondsPerLetter based on the charecters being typed</remarks>
    public enum Mode
    {
        FixedRate,
        FixedDuration,
        HuntAndPeck
    }

    /// <summary>
    /// Cache'd Text component we're typing out to
    /// </summary>
    private Text _text;

    /// <summary>
    /// Store for the message we're going to type out
    /// </summary>
    private string buffer = string.Empty;
    /// <summary>
    /// Where in the buffer we've got to
    /// </summary>
    private int readIndex = 0;
    /// <summary>
    /// When we're going to type out the next charcter
    /// </summary>
    private float nextReadTime = dontRead;

    /// <summary>
    /// Which mode the Teletype component is operating in
    /// </summary>
    public Mode mode = Mode.FixedRate;
    /// <summary>
    /// How long in Seconds the teletype will take to type messages sent in FixedDuration mode
    /// </summary>
    public float Duration = 3.0f;
    /// <summary>
    /// How long the teletype takes between letters in FixedRate mode
    /// </summary>
    public float SecondsPerLetter = 0.05f;

    /// <summary>
    /// Event that occurs when this component writes out a letter
    /// </summary>
    public UnityEvent OnLetterTyped;

    /// <summary>
    /// Event that occurs when it finishes typing it's message(s)
    /// </summary>
    public UnityEvent OnFinished;

    /// <summary>
    /// Cache the Text component, so we dont need to look it up every frame
    /// </summary>
    void Start ()
	{
	    _text = GetComponent<Text>();

        if(_text == null) Debug.LogWarningFormat("Teletype component on GameObject {0} requires a Text component", gameObject.name);
	}
	
	/// <summary>
    /// Writes to the Text component from our buffer over time
    /// </summary>
	void Update ()
	{
	    if (_text == null) return;  //must have somewhere to write to
        if (string.IsNullOrEmpty(buffer)) return; //must have something to write
	    if (nextReadTime == dontRead) return; //must allowed to write
	    if (readIndex >= buffer.Length) return; //must have something left to write
	    if (nextReadTime > Time.time) return; //must be due to write something

        //get a charecter from the buffer and append it to the Text component
	    char lastChar = buffer[readIndex];
        _text.text = _text.text + lastChar;
	    readIndex++;
        //Debug.Log(_text.text);

        //trigger the OnLetterTyped event
        if (OnLetterTyped != null) OnLetterTyped.Invoke();

        if (readIndex >= buffer.Length)
        {
            //if we've reached the end of our buffer trigger the OnFinished event
            if (OnFinished != null) OnFinished.Invoke();
        }

        //work out how long we should wait to write the next charecter
        if (mode == Mode.HuntAndPeck)
	    {
	        if (readIndex >= buffer.Length)
	        {
	            nextReadTime = dontRead;
	            return;
	        }

	        char nextChar = buffer[readIndex];
	        
            nextReadTime = Time.time + GetHuntingTime(lastChar, nextChar); ;
        }
	    else
	    {
            nextReadTime = Time.time + SecondsPerLetter;
        }
	}

    /// <summary>
    /// how close to the home keys on a QWERTY keyboard each key is, used for Hunt and Peck
    /// </summary>
    private Dictionary<Char,int> LetterMap = new Dictionary<char, int>()
    {
        {'f',0},
        {'j',0},
        {'r',1},
        {'t',1},
        {'g',1},
        {'v',1},
        {'c',1},
        {'d',1},
        {'u',1},
        {'i',1},
        {'k',1},
        {'m',1},
        {'n',1},
        {'h',1},
        {'y',2},
        {'b',2},
        {'x',2},
        {'s',2},
        {'e',2},
        {'o',2},
        {'l',2},
        {'w',3},
        {'a',3},
        {'z',3},
        {'p',3},
        {'q',4},
    };

    /// <summary>
    /// Guestimate how long it would take a human to find and press the next key
    /// </summary>
    /// <param name="lastChar"></param>
    /// <param name="nextChar"></param>
    /// <returns>a time in seconds</returns>
    private float GetHuntingTime(char lastChar, char nextChar)
    {
        float delay = SecondsPerLetter;

        if (lastChar == nextChar)
        {
            delay = SecondsPerLetter / 2; //repeat key presses are quick
        }
        else
        {
            if (Char.IsLetterOrDigit(nextChar))
            {
                int distance = 5;
                if (!LetterMap.TryGetValue(char.ToLowerInvariant(nextChar), out distance)) distance = 5;
                
                delay = SecondsPerLetter * (distance + 1);

                if (Char.IsUpper(nextChar)) delay += SecondsPerLetter*3;
            }
            else
            {
                if (nextChar == ' ') delay = SecondsPerLetter * 4; //actually more about thinking time than hunting
                if (nextChar == '\n') delay = SecondsPerLetter * 5; //actually more about thinking time than hunting

                if (Char.IsPunctuation(nextChar)) delay += SecondsPerLetter * 2;

                if (nextChar == '.') delay += SecondsPerLetter * 3; //actually more about thinking time than hunting
            }
        }

        return delay;
    }

    /// <summary>
    /// Start writing out text to the Text component
    /// </summary>
    /// <param name="message">Message to be written out to the Text component</param>
    /// <remarks>Consecutive calls append to the previously called message</remarks>
    public void Write(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        if(_text == null)
        {
            _text = GetComponent<Text>();
        }

        if (readIndex == buffer.Length)
        {
            buffer = message;
            readIndex = 0;
        }
        else
        {
            buffer = buffer + message;
        }

        if (mode == Mode.FixedDuration)
        {
            SecondsPerLetter = Duration/buffer.Length;
        }

        if (nextReadTime == dontRead)
        {
            nextReadTime = Time.time;
        }
    }

    /// <summary>
    /// Clear the Text component and start writing out out message
    /// </summary>
    /// <param name="message">Message to be written out to the Text component</param>
    /// <remarks>Trunacates any previously written messages</remarks>
    public void ClearAndWrite(string message)
    {
        Clear();
        Write(message);
    }

    /// <summary>
    /// Trunacates any previously written messages
    /// </summary>
    public void Reset()
    {
        buffer = string.Empty;
        readIndex = 0;
        nextReadTime = dontRead;
    }

    /// <summary>
    /// Trunacates any previously written messages and emtpy the Text component
    /// </summary>
    public void Clear()
    {
        Reset();

        if (_text == null)
        {
            _text = GetComponent<Text>();
        }

        _text.text = "";
    }

    /// <summary>
    /// Immediately write any remaining mesage(s) out, so the user doesn't have to wait.
    /// </summary>
    public void Finish()
    {
        if (string.IsNullOrEmpty(buffer)) return;
        if (nextReadTime == dontRead) return;
        if (readIndex >= buffer.Length) return;

        if (_text == null)
        {
            _text = GetComponent<Text>();
        }

        string remainder = buffer.Substring(readIndex);
        _text.text = _text.text + remainder;

        //if we've reached the end of our buffer trigger the OnFinished event
        if (OnFinished != null) OnFinished.Invoke();

        Reset();
    }
}
