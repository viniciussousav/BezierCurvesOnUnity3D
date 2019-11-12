using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    public List<LineRenderer> curva = new List<LineRenderer>();    //List que guarda as curvas
    public List<List<GameObject>> points = new List<List<GameObject>>();       //List que guarda os pontos  
    public List<List<LineRenderer>> lines = new List<List<LineRenderer>>();    //List que guarda linhas que ligam os pontos 
    private List<Color> colors = new List<Color> { Color.red, Color.blue, Color.magenta, Color.green, Color.yellow, Color.cyan };

    public int qtdAvsCurva = 100;
    public int quantCurvas = 0;
    private int curvaAtual = 0;
    public bool arrastar;

    public bool visibPtsControle = true;
    public bool visibPlgsControle = true;
    public List<bool> visibCurvas = new List<bool>();
    public bool visibGeralCurvas = true;

    // Start is called before the first frame update
    void Start()
    {
        //inicializando listas
        visibCurvas.Add(new bool());
        lines.Add(new List<LineRenderer>());
        points.Add(new List<GameObject>());
        curva.Add(new LineRenderer());
        pontosDeAvaliacao.Add(new List<GameObject>());

        //interface
        inputQuantAvaliacao.text = (100).ToString();
        curvaDisplay.text = "Curva atual: " + (curvaAtual + 1).ToString();

        //inicializando variaveis
        visibCurvas[curvaAtual] = true;
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
            Vector2 mouseClick = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            if (mouseClick.y < 0.7f)
            {
                GameObject instantiated = Instantiate(prefab, point, Quaternion.identity) as GameObject;
                instantiated.GetComponent<SpriteRenderer>().color = Color.black;
                LineRenderer newLine = instantiated.GetComponent<LineRenderer>();
                lines[curvaAtual].Add(newLine);
                if (visibPtsControle)
                {
                    instantiated.GetComponent<Renderer>().enabled = true;
                }
                else
                {
                    instantiated.GetComponent<Renderer>().enabled = false;
                }
                points[curvaAtual].Add(instantiated);

                if (points[curvaAtual].Count >= 2)
                {
                    int start = points[curvaAtual].Count - 2;
                    int target = points[curvaAtual].Count - 1;
                    Vector3[] vecs = { points[curvaAtual][start].transform.position, points[curvaAtual][target].transform.position };

                    lines[curvaAtual][start].SetPositions(vecs);
                    lines[curvaAtual][start].material = new Material(Shader.Find("Sprites/Default"));
                    lines[curvaAtual][start].startColor = colors[curvaAtual % colors.Count];
                    lines[curvaAtual][start].endColor = colors[curvaAtual % colors.Count];
                    if(visibPlgsControle)
                    {
                        lines[curvaAtual][start].enabled = true;
                    }
                    else
                    {
                        lines[curvaAtual][start].enabled = false;
                    }
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
        curva.Add(new LineRenderer());
        points.Add(new List<GameObject>());
        lines.Add(new List<LineRenderer>());
        visibCurvas.Add(new bool());
        pontosDeAvaliacao.Add(new List<GameObject>());
        quantCurvas += 1;
        curvaAtual = quantCurvas;
        curvaDisplay.text = "Curva atual: " + (curvaAtual + 1).ToString();
        exibirLinhasAtuais();
        visibCurvas[curvaAtual] = visibGeralCurvas;
    }
    public void alterarCurvaAtual(int sentido)
    {
        if (sentido == 1 && curvaAtual <= quantCurvas - 1) //alterei (quantCurvas - 1)
        {
            curvaAtual += 1;
            curvaDisplay.text = "Curva atual: " + (curvaAtual + 1).ToString();

        }
        else if (sentido == -1 && curvaAtual != 0)
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
                if (i != curvaAtual)
                {
                    lines[i][j].enabled = false;
                    points[i][j].SetActive(false);
                }
                else
                {
                    if (visibPtsControle)
                    {
                        lines[i][j].enabled = true;
                    }
                    points[i][j].SetActive(true);
                    lines[i][j].startColor = colors[curvaAtual % colors.Count];
                    lines[i][j].endColor = colors[curvaAtual % colors.Count];
                    if (visibPtsControle)
                        points[i][j].GetComponent<Renderer>().enabled = true;
                    else
                        points[i][j].GetComponent<Renderer>().enabled = false;
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
        if (int.Parse(inputQuantAvaliacao.text) < 1)
        {
            EditorUtility.DisplayDialog("Valor inválido", "Não é possível escolher um valor abaixo de 1, favor verificar o número digitado!", "Entendi!");
            inputQuantAvaliacao.text = (100).ToString();
            qtdAvsCurva = 100;
        }
        else
        {
            qtdAvsCurva = int.Parse(inputQuantAvaliacao.text);
        }
        if (points[curvaAtual].Count > 1)
        {
            atualizarCurvaBezier();
        }
    }
    public void removerCurva()
    {
        if (curvaAtual != 0)
        {
            for (int i = 0; i < points[curvaAtual].Count; i++)
            {
                Destroy(points[curvaAtual][i]);
            }
            points.RemoveAt(curvaAtual);
            lines.RemoveAt(curvaAtual);
            Destroy(curva[curvaAtual]);
            quantCurvas -= 1;
            alterarCurvaAtual(-1);
        }

        else if (curvaAtual == 0)
        {
            Destroy(curva[curvaAtual]);
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
        Vector2 pontoAnterior = points[curvaAtual][0].transform.position;
        Vector2 pontoAtual = Vector2.zero;
        List<Vector3> vecs = new List<Vector3>();
        for (int i = 0; i < pontosDeAvaliacao[curvaAtual].Count; i++)
        {
            pontosDeAvaliacao[curvaAtual][i].GetComponent<SpriteRenderer>().enabled = false;
        }

        for (double t = particoes; t <= 1.0; t += particoes)
        {
            for (int i = 0; i < points[curvaAtual].Count; i++)
            {
                float bern = (float)(comb(points[curvaAtual].Count - 1, i) * Math.Pow((1.0 - t), (points[curvaAtual].Count - 1 - i)) * Math.Pow(t, i));
                pontoAtual.x += (bern * points[curvaAtual][i].transform.position.x);
                pontoAtual.y += (bern * points[curvaAtual][i].transform.position.y);
            }
            vecs.Add(pontoAnterior);
            vecs.Add(pontoAtual);
            //Debug.Log(vecs[0] + " " + vecs[1]);
            pontoAnterior = pontoAtual;
            pontoAtual = Vector2.zero;
        }
        vecs.Add(pontoAnterior);
        Vector2 pontoFinal = new Vector2(points[curvaAtual][points[curvaAtual].Count - 1].transform.position.x, points[curvaAtual][points[curvaAtual].Count - 1].transform.position.y);
        vecs.Add(pontoFinal);
        Destroy(curva[curvaAtual]);
        GameObject instantiated = Instantiate(prefab, pontoFinal, Quaternion.identity) as GameObject;
        instantiated.GetComponent<SpriteRenderer>().color = colors[curvaAtual % colors.Count];
        pontosDeAvaliacao[curvaAtual].Add(instantiated);
        LineRenderer newLine = instantiated.GetComponent<LineRenderer>();
        curva[curvaAtual] = newLine;
        curva[curvaAtual].material = new Material(Shader.Find("Sprites/Diffuse"));
        curva[curvaAtual].enabled = true;
        curva[curvaAtual].positionCount = vecs.Count;
        curva[curvaAtual].SetPositions(vecs.ToArray());
        if (visibCurvas[curvaAtual])
        {
            curva[curvaAtual].enabled = true;
        }
        else
        {
            curva[curvaAtual].enabled = false;
        }
    }
    public double comb(int n, int i)
    {
        double res = 1.0;
        if (i >= (n - i))
        {
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
        }
        if ((n - i) > i)
        {
            for (int j = n; j > (n - i); j--)
            {
                res *= j;
            }
            double fati = 1.0;
            for (int k = i; k > 0; k--)
            {
                fati *= k;
            }
            res = res / fati;
        }
        return res;
    }
    public void atualizarCurvaBezier()
    {
        double particoes = 1.0 / qtdAvsCurva;
        int indexPontoAnterior = 0;
        Vector2 pontoAnterior = points[curvaAtual][0].transform.position;
        Vector2 pontoAtual = Vector2.zero;
        List<Vector3> vecs = new List<Vector3>();
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
            vecs.Add(pontoAnterior);
            vecs.Add(pontoAtual);
            //Debug.Log(vecs[0] + " " + vecs[1]);
            pontoAnterior = pontoAtual;
            pontoAtual = Vector2.zero;
        }
        vecs.Add(pontoAnterior);
        vecs.Add(new Vector2(points[curvaAtual][points[curvaAtual].Count - 1].transform.position.x, points[curvaAtual][points[curvaAtual].Count - 1].transform.position.y));
        curva[curvaAtual].positionCount = vecs.Count;
        curva[curvaAtual].SetPositions(vecs.ToArray());
        if (visibCurvas[curvaAtual])
        {
            curva[curvaAtual].enabled = true;
        }
        else
        {
            curva[curvaAtual].enabled = false;
        }
    }

    public void PlgsControleIsVisible (bool visibilidade)
    {
        if(visibilidade)
        {
            for(int i = 0; i < lines[curvaAtual].Count; i++)
            {
                lines[curvaAtual][i].enabled = true;
            }
            visibPlgsControle = true;
        }
        else
        {
            for (int i = 0; i < lines[curvaAtual].Count; i++)
            {
                lines[curvaAtual][i].enabled = false;
            }
            visibPlgsControle = false;
        }
    }

    public void CurvasIsVisible (bool visibilidade)
    {
        if (visibilidade)
        {
            for(int i=0;i<curva.Count;i++)
            {
                if (curva[i] != null)
                    curva[i].enabled = true;
                visibCurvas[i] = true;
            }
            visibGeralCurvas = true;
        }
        else
        {
            for (int i = 0; i < curva.Count; i++)
            {
                if(curva[i] != null)
                    curva[i].enabled = false;
                visibCurvas[i] = false;
            }
            visibGeralCurvas = false;
        }
    }

    public void CurvaAtualIsVisible ()
    {
        if(curva[curvaAtual] != null)
            curva[curvaAtual].enabled = !curva[curvaAtual].enabled;
        visibCurvas[curvaAtual] = !visibCurvas[curvaAtual];
    }

    public void PtsControleIsVisible (bool visibilidade)
    {
        if (visibilidade)
        {
            for (int i = 0; i < points[curvaAtual].Count; i++)
            {
                points[curvaAtual][i].GetComponent<Renderer>().enabled = true;
            }
            visibPtsControle = true;
        }
        else
        {
            for (int i = 0; i < points[curvaAtual].Count; i++)
            {
                points[curvaAtual][i].GetComponent<Renderer>().enabled = false;
            }
            visibPtsControle = false;
        }
    }
}
