using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lsystems : MonoBehaviour
{
    [SerializeField] public int iterations;
    [SerializeField] public float angle;
    [SerializeField] public float length;

    private string axiom = "X";
    private Dictionary<char, string> rules = new Dictionary<char, string>();
    private string currentString;

    // Start is called before the first frame update
    void Start()
    {
        rules.Add('X', "[-FX][+FX][FX]");
        rules.Add('F', "FF");

        GenerateLSystem();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Redraw L-System with updated parameters when Space is pressed
            GenerateLSystem();
        }
    }

    void HandleInput()
    {
        // Increase angle with Up arrow
        if (Input.GetKey(KeyCode.UpArrow))
        {
            angle += 1f;
        }
        // Decrease angle with Down arrow
        if (Input.GetKey(KeyCode.DownArrow))
        {
            angle -= 1f;
        }

        // Increase branch length with Right arrow
        if (Input.GetKey(KeyCode.RightArrow))
        {
            length += 0.1f;
        }
        // Decrease branch length with Left arrow
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            length -= 0.1f;
        }

        // Clamp angle and length to avoid invalid values
        angle = Mathf.Clamp(angle, 0f, 180f);
        length = Mathf.Clamp(length, 0.1f, 50f);
    }

    void GenerateLSystem()
    {
        currentString = axiom;
        for (int i = 0; i < iterations; i++)
        {
            currentString = GenerateNextString(currentString);
        }
        DrawLSystem(currentString);
    }

    string GenerateNextString(string input)
    {
        string result = "";
        foreach (char c in input)
        {
            result += rules.ContainsKey(c) ? rules[c] : c.ToString();
        }
        return result;
    }

    void DrawLSystem(string input)
    {
        Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
        Vector3 position = Vector3.zero;
        Vector3 direction = Vector3.up;

        foreach (char c in input)
        {
            if (c == 'F')
            {
                Vector3 newPosition = position + direction * length;
                Debug.DrawLine(position, newPosition, Color.green, 1000f, false);
                position = newPosition;
            }
            else if (c == '+')
            {
                direction = Quaternion.Euler(0, 0, angle) * direction;
            }
            else if (c == '-')
            {
                direction = Quaternion.Euler(0, 0, -angle) * direction;
            }
            else if (c == '[')
            {
                transformStack.Push(new TransformInfo
                {
                    position = position,
                    direction = direction
                });
            }
            else if (c == ']')
            {
                TransformInfo ti = transformStack.Pop();
                position = ti.position;
                direction = ti.direction;
            }
        }
    }

    private struct TransformInfo
    {
        public Vector3 position;
        public Vector3 direction;
    }
}
