using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script : MonoBehaviour
{
    
    public GameObject prefab;                //modelo ponto
    public LineRenderer bezier_curve;        //modelo curva
    public Text curvaDisplay;                //UI "Curva atual"
    public Rigidbody2D selected;             //Ponto arrastado
    public InputField inputQuantAvaliacao;   //UI InputField

    public List<List<GameObject>> pontosDeAvaliacao = new List<List<GameObject>>();
    public List<List<LineRenderer>> curva = new List<List<LineRenderer>>();    //List que guarda as curvas
    public List<List<GameObject>> points = new List<List<GameObject>>();       //List que guarda os pontos  
    public List<List<LineRenderer>> lines = new List<List<LineRenderer>>();    //List que guarda linhas que ligam os pontos 
    private List<Color> colors = new List<Color> { Color.red, Color.blue, Color.magenta, Color.green, Color.yellow, Color.cyan };

    public int qtdAvsCurva = 12;
    public int quantCurvas = 0;
    private int curvaAtual = 0;
    public bool arrastar;
    
    // Start is called before the first frame update
    void Start()
    {
        //inicializando listas
        lines.Add(new List<LineRenderer>());
        points.Add(new List<GameObject>());
        curva.Add(new List<LineRenderer>());
        pontosDeAvaliacao.Add(new List<GameObject>());

        //interface
        inputQuantAvaliacao.text = (12).ToString();
        curvaDisplay.text = "Curva atual: " + (curvaAtual + 1).ToString();
        
        //inicializando variaveis
        qtdAvsCurva = int.Parse(inputQuantAvaliacao.text);
        arrastar = false;
        
        //curva teste
        //curves.Add(Instantiate(bezier_curve, new Vector3(0, 0, 0), Quaternion.identity));
        //Vector3[] test = { new Vector3(0f, 0f, 0f), new Vector3(2f, 2f, 2f)};
        //curves[0].SetPositions(test);
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
                    lines[curvaAtual][start].material = new Material(Shader.Find("Sprites/Default"));
                    lines[curvaAtual][start].startColor = colors[curvaAtual % colors.Count];
                    lines[curvaAtual][start].endColor = colors[curvaAtual % colors.Count];
                    CriarCurva();
                }

                    
            }
        }

        if (Input.GetMouseButtonDown(0) && arrastar)
        {
            trySelectObject();
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
        curva.Add(new List<LineRenderer>());
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
    private void trySelectObject()
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
        atualizarCurvaBezier();
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
    public void CriarCurva()
    {
        double particoes = 1.0 / qtdAvsCurva;
        int indexPontoAnterior = 0;
        Vector2 pontoAnterior = points[curvaAtual][0].transform.position;
        Vector2 pontoAtual = Vector2.zero;

        for(int i = 0; i < pontosDeAvaliacao[curvaAtual].Count; i++)
        {
            pontosDeAvaliacao[curvaAtual][i].GetComponent<SpriteRenderer>().enabled = false ;
        }

        for (double t = particoes; t <= 1.0; t += particoes, indexPontoAnterior++)
        {
            GameObject instantiated = Instantiate(prefab, pontoAnterior, Quaternion.identity) as GameObject;
            instantiated.GetComponent<SpriteRenderer>().color = Color.black;
            pontosDeAvaliacao[curvaAtual].Add(instantiated);
            LineRenderer newLine = instantiated.GetComponent<LineRenderer>();
            curva[curvaAtual].Add(newLine);
            for (int i = 0; i < points[curvaAtual].Count; i++)
            {
                float bern = (float)(comb(points[curvaAtual].Count - 1, i) * Math.Pow((1.0 - t), (points[curvaAtual].Count - 1 - i)) * Math.Pow(t, i));
                pontoAtual.x += (bern * points[curvaAtual][i].transform.position.x);
                pontoAtual.y += (bern * points[curvaAtual][i].transform.position.y);
            }
            Vector3[] vecs = { pontoAnterior, pontoAtual };
            //Debug.Log(vecs[0] + " " + vecs[1]);
            curva[curvaAtual][indexPontoAnterior].material = new Material(Shader.Find("Sprites/Diffuse"));
            curva[curvaAtual][indexPontoAnterior].SetPositions(vecs);
            curva[curvaAtual][indexPontoAnterior].SetWidth(0.1f, 0.1f);
            curva[curvaAtual][indexPontoAnterior].enabled = true;
            curva[curvaAtual][indexPontoAnterior].startColor = Color.black;
            curva[curvaAtual][indexPontoAnterior].endColor = Color.black;
            pontoAnterior = pontoAtual;
            pontoAtual = Vector2.zero;
        }

    }
    public double comb(int n, int i)
    {
        double res = 1.0;
        for (int j = n; j > i; j--)
        {
            res *= j;
        }
        double fatnMenosi = 1.0;
        for (int k = (n - i); k > 0; k--)
        {
            fatnMenosi *= k;
        }
        res = res / fatnMenosi;
        return res;
    }
    public void atualizarCurvaBezier()
    {
        double particoes = 1.0 / qtdAvsCurva;
        int indexPontoAnterior = 0;
        Vector2 pontoAnterior = points[curvaAtual][0].transform.position;
        Vector2 pontoAtual = Vector2.zero;

        for (int i = 0; i < pontosDeAvaliacao[curvaAtual].Count; i++)
        {
            pontosDeAvaliacao[curvaAtual][i].GetComponent<SpriteRenderer>().enabled = false;
        }

        for (double t = particoes; t <= 1.0; t += particoes, indexPontoAnterior++)
        {
           
            for (int i = 0; i < points[curvaAtual].Count; i++)
            {
                float bern = (float)(comb(points[curvaAtual].Count - 1, i) * Math.Pow((1.0 - t), (points[curvaAtual].Count - 1 - i)) * Math.Pow(t, i));
                pontoAtual.x += (bern * points[curvaAtual][i].transform.position.x);
                pontoAtual.y += (bern * points[curvaAtual][i].transform.position.y);
            }
            Vector3[] vecs = { pontoAnterior, pontoAtual };
            //Debug.Log(vecs[0] + " " + vecs[1]);
            curva[curvaAtual][indexPontoAnterior].material = new Material(Shader.Find("Sprites/Diffuse"));
            curva[curvaAtual][indexPontoAnterior].SetPositions(vecs);
            curva[curvaAtual][indexPontoAnterior].SetWidth(0.1f, 0.1f);
            curva[curvaAtual][indexPontoAnterior].enabled = true;
            curva[curvaAtual][indexPontoAnterior].startColor = Color.black;
            curva[curvaAtual][indexPontoAnterior].endColor = Color.black;
            pontoAnterior = pontoAtual;
            pontoAtual = Vector2.zero;
        }
    }

}
