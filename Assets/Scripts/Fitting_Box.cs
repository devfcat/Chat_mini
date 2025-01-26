using System;
using TMPro;
using UnityEngine;

public class Fitting_Box : MonoBehaviour
{
    public int character;
    public int line;
    public float init_width = 35f;
    public float init_height = 144f;
    public RectTransform parents;
    public RectTransform textBox;
    public TextMeshProUGUI message;

    public void setText(string msg)
    {
        message.text = msg;

        character = msg.Length;

        if (character < 25)
        {
            textBox.sizeDelta = new Vector2(init_width*character+100f, init_height+100f);
        }
        else
        {
            double d = character % 25;
            if (d > 0)
            {
                line = (int) (character / 25) + 1;
            }
            else
            {
                line = (int) (character / 25);
            }
            textBox.sizeDelta = new Vector2(init_width*25, line*init_height+100f);

        }
        parents.sizeDelta = new Vector2(900, textBox.sizeDelta.y);
        parents.anchoredPosition = new Vector2(0, 0);
    }
}
