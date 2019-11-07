﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Script : MonoBehaviour
{
    
    public GameObject prefab;
    public int quantCurvas = 0;
    public List<List<GameObject>> points = new List<List<GameObject>>();
    private List<Color> colors = new List<Color> { Color.red, Color.blue, Color.green, Color.black, Color.yellow, Color.cyan };
    private int curvaAtual = 0;
    public Text curvaDisplay;
    // Start is called before the first frame update
    void Start()
    {
        curvaDisplay.text = "Curva atual: " + (curvaAtual + 1).ToString();
        points.Add(new List<GameObject>());
     
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePoint = Input.mousePosition;
            Vector3 point = Camera.main.ScreenToWorldPoint(mousePoint);
            point.z = 0;

            if(mousePoint.y < 275f)
            {
                GameObject instantiated = Instantiate(prefab, point, Quaternion.identity) as GameObject;
                instantiated.GetComponent<SpriteRenderer>().color = colors[curvaAtual% colors.Count];

                points[curvaAtual].Add(instantiated);
                Debug.Log(points.Count);
            }


        }
    }

    public void AdicionarCurva()
    {
        points.Add(new List<GameObject>());
        quantCurvas += 1;
        curvaAtual = quantCurvas;
        curvaDisplay.text = "Curva atual: " + (curvaAtual + 1).ToString();
    }


    public void alterarCurvaAtual(int sentido)
    {
        if(sentido == 1 && curvaAtual <= quantCurvas-1)
        {
            curvaAtual += 1;
            curvaDisplay.text = "Curva atual: " + (curvaAtual + 1).ToString();

        }
        else if(sentido == -1 && curvaAtual != 0)
        {
            curvaAtual -= 1;
            curvaDisplay.text = "Curva atual: " + (curvaAtual + 1).ToString();

        }
    }
}