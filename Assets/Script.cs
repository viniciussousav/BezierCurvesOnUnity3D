using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script : MonoBehaviour
{
    
    public GameObject prefab;
    public Text curvaDisplay;
    public Rigidbody2D selected;
    public InputField inputQuantAvaliacao;
    
    public List<List<GameObject>> points = new List<List<GameObject>>();
    public List<List<LineRenderer>> lines = new List<List<LineRenderer>>();
    private List<Color> colors = new List<Color> { Color.red, Color.blue, Color.green, Color.black, Color.yellow, Color.cyan };

    public int qtdAvsCurva = 1;
    public int quantCurvas = 0;
    private int curvaAtual = 0;
    public bool arrastar;
    
    // Start is called before the first frame update
    void Start()
    {
        inputQuantAvaliacao.text = (1).ToString();
        qtdAvsCurva = int.Parse(inputQuantAvaliacao.text);
        curvaDisplay.text = "Curva atual: " + (curvaAtual + 1).ToString();
        points.Add(new List<GameObject>());
        lines.Add(new List<LineRenderer>());
        arrastar = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !arrastar)
        {
            Vector2 mousePoint = Input.mousePosition;
            Vector3 point = Camera.main.ScreenToWorldPoint(mousePoint);
            point.z = 0;

            if(mousePoint.y < 275f)
            {
                GameObject instantiated = Instantiate(prefab, point, Quaternion.identity) as GameObject;
                instantiated.GetComponent<SpriteRenderer>().color = Color.black;
                points[curvaAtual].Add(instantiated);
                LineRenderer newLine = instantiated.GetComponent<LineRenderer>();
                lines[curvaAtual].Add(newLine);
                
                if(points[curvaAtual].Count >= 2)
                {
                    int start = points[curvaAtual].Count - 2;
                    int target = points[curvaAtual].Count - 1;

                    Vector3[] vecs = { points[curvaAtual][start].transform.position, points[curvaAtual][target].transform.position };

                    lines[curvaAtual][start].SetPositions(vecs);

                    //teste
                    lines[curvaAtual][start].material = new Material(Shader.Find("Sprites/Default"));
                    lines[curvaAtual][start].startColor = colors[curvaAtual % colors.Count];
                    lines[curvaAtual][start].endColor = colors[curvaAtual % colors.Count];
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && arrastar)
        {
            TrySelectObject();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            selected = null;
        }
    }

    private void FixedUpdate()
    {
        if (selected)
        {
            atualizarLinhas();
        }

        if (Input.GetMouseButton(0) && selected != null)
        {
            selected.MovePosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    public void AdicionarCurva()
    {
        points.Add(new List<GameObject>());
        lines.Add(new List<LineRenderer>());
        quantCurvas += 1;
        curvaAtual = quantCurvas;
        curvaDisplay.text = "Curva atual: " + (curvaAtual + 1).ToString();
        exibirLinhasAtuais();
    }

    public void alterarCurvaAtual(int sentido)
    {
        if(sentido == 1 && curvaAtual <= quantCurvas-1) //alterei (quantCurvas - 1)
        {
            curvaAtual += 1;
            curvaDisplay.text = "Curva atual: " + (curvaAtual + 1).ToString();

        }
        else if(sentido == -1 && curvaAtual != 0)
        {
            curvaAtual -= 1;
            curvaDisplay.text = "Curva atual: " + (curvaAtual + 1).ToString();
        }

        exibirLinhasAtuais();
    }

    public void exibirLinhasAtuais()
    {
        for (int i = 0; i <= quantCurvas; i++)
        {
            for (int j = 0; j < lines[i].Count; j++)
            {
                if(i != curvaAtual)
                {
                    lines[i][j].enabled = false;
                    points[i][j].SetActive(false);
                } else {
                    lines[i][j].enabled = true;
                    points[i][j].SetActive(true);
                    lines[i][j].startColor = colors[curvaAtual % colors.Count];
                    lines[i][j].endColor = colors[curvaAtual % colors.Count];
                }
            }

            
        }
    }

    public void arrastarControle()
    {
        arrastar = !arrastar;
    }

    private void TrySelectObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Point"))
            {
                selected = hit.collider.GetComponent<Rigidbody2D>();
            }
        }
    }

    public void atualizarLinhas()
    {
        if (points[curvaAtual].Count >= 2)
        {
            for (int i = 0; i < lines[curvaAtual].Count - 1; i++)
            {
                Vector3[] array = { points[curvaAtual][i].transform.position, points[curvaAtual][i + 1].transform.position };
                lines[curvaAtual][i].SetPositions(array);
            }
        }
    }

    public void atualizarQuantidadeAvaliação()
    {
        qtdAvsCurva = int.Parse(inputQuantAvaliacao.text);
    }

    public void removerCurva()
    {
        if(curvaAtual != 0)
        {
            for (int i = 0; i < points[curvaAtual].Count; i++)
            {
                Destroy(points[curvaAtual][i]);
            }
            points.RemoveAt(curvaAtual);
            lines.RemoveAt(curvaAtual);
            quantCurvas -= 1;
            alterarCurvaAtual(-1);
        }

        else if(curvaAtual == 0)
        {
            for (int i = 0; i < points[curvaAtual].Count; i++)
            {
                Destroy(points[curvaAtual][i]);
            }
            points[0].Clear();
            lines[0].Clear();
        }

    
    }


    public void criarCurva()
    {

    }

    

}
