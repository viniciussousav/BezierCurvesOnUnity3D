using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Script : MonoBehaviour
{
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO
    //SE ISSO AQUI ESTIVER SUMINDO VC DESFEZ TUDO

    public GameObject prefab;                //modelo ponto
    public LineRenderer bezier_curve;        //modelo curva
    public Text curvaDisplay;                //UI "Curva atual"
    public Rigidbody2D selected;             //Ponto arrastado
    public InputField inputQuantAvaliacao;   //UI InputField

    public List<LineRenderer> curva = new List<LineRenderer>();    //List que guarda as curvas
    public List<List<GameObject>> points = new List<List<GameObject>>();       //List que guarda os pontos  
    public List<LineRenderer> lines = new List<LineRenderer>();    //List que guarda linhas que ligam os pontos 
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
        /*
        
          Vector3[] aux = new Vector3[points[curvaAtual].Count];
                    for (int i = 0; i < points[curvaAtual].Count; i++)
                        aux[i] = points[curvaAtual][i].transform.position;
        */

        //inicializando listas
        visibCurvas.Add(new bool());
        lines.Add(new LineRenderer());
        points.Add(new List<GameObject>());
        curva.Add(new LineRenderer());

        //interface
        inputQuantAvaliacao.text = (100).ToString();
        curvaDisplay.text = "Curva atual: " + (curvaAtual + 1).ToString();

        //inicializando variaveis
        visibCurvas[curvaAtual] = true;
        qtdAvsCurva = int.Parse(inputQuantAvaliacao.text);
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
            Vector2 mouseClick = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            if (mouseClick.y < 0.7f)
            {
                GameObject instantiated = Instantiate(prefab, point, Quaternion.identity) as GameObject;
                points[curvaAtual].Add(instantiated);
                instantiated.GetComponent<Renderer>().enabled = visibPtsControle;
                
                if(points[curvaAtual].Count == 1)//mudei
                {
                    lines[curvaAtual] = Instantiate(bezier_curve, Vector3.zero, Quaternion.identity);
                    lines[curvaAtual].startColor = colors[curvaAtual % colors.Count];
                    lines[curvaAtual].endColor = colors[curvaAtual % colors.Count];
                    lines[curvaAtual].SetPosition(0, points[curvaAtual][0].transform.position);
                    lines[curvaAtual].enabled = visibPlgsControle;
                }
             
                if (points[curvaAtual].Count >= 2)
                {
                    
                    lines[curvaAtual].positionCount = points[curvaAtual].Count;
                    lines[curvaAtual].SetPosition(lines[curvaAtual].positionCount - 1, points[curvaAtual][points[curvaAtual].Count - 1].transform.position);
                    
                    if(curva[curvaAtual] == null)
                        curva[curvaAtual] = Instantiate(bezier_curve, Vector3.zero, Quaternion.identity);
                    else
                        CriarCurva();

                    Debug.Log(lines[curvaAtual].positionCount);
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
        if (selected != null)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                bool encPonto = false;
                for (int i = 0; i < points[curvaAtual].Count && !encPonto; i++)
                {
                    if(points[curvaAtual].Count == 1)
                    {
                        points[curvaAtual][i].SetActive(false);
                        points[curvaAtual].Clear();
                        selected = null;
                    }
                    else if (selected.transform.position == points[curvaAtual][i].transform.position)
                    {
                        encPonto = true;
                        points[curvaAtual][i].SetActive(false);
                        points[curvaAtual].RemoveAt(i);
                        if (points[curvaAtual].Count > 0)
                        {
                            atualizarLinhas();
                        } 
                        
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (selected)
        {
            atualizarLinhas();
            CriarCurva();
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
        lines.Add(new LineRenderer());
        visibCurvas.Add(new bool());
        quantCurvas += 1;
        curvaAtual = quantCurvas;
        curvaDisplay.text = "Curva atual: " + (curvaAtual + 1).ToString();
        visibCurvas[curvaAtual] = visibGeralCurvas;
        exibirLinhasAtuais();
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
            if(lines[i] != null)
            {
                if(i == curvaAtual)
                {
                    lines[i].enabled = visibPlgsControle;
                    for (int j = 0; j < points[i].Count; j++)
                    {
                        points[i][j].GetComponent<Renderer>().enabled = visibPtsControle;
                    }
                } else
                {
                    lines[i].enabled = false;
                    for(int j = 0; j < points[i].Count; j++)
                    {
                        points[i][j].GetComponent<Renderer>().enabled = false;
                    }
                }
            }
        }
    } //refatorar
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
        if (points[curvaAtual].Count > 0)
        {
            List<Vector3> list = new List<Vector3>();
            for (int i = 0; i < points[curvaAtual].Count; i++)
            {
                list.Add(points[curvaAtual][i].transform.position);
            }
            lines[curvaAtual].positionCount = points[curvaAtual].Count;
            lines[curvaAtual].SetPositions(list.ToArray());
        }
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
            CriarCurva();
        }
    }
    public void removerCurva()
    {
        for (int i = 0; i < points[curvaAtual].Count; i++)
        {
            Destroy(points[curvaAtual][i]);
        }
        Destroy(curva[curvaAtual]);
        Destroy(lines[curvaAtual]);
        points[curvaAtual].Clear();
    }
    public void CriarCurva()
    {
        double particoes = 1.0 / qtdAvsCurva;
        Vector2 pontoAnterior = points[curvaAtual][0].transform.position;
        Vector2 pontoAtual = Vector2.zero;
        List<Vector3> vecs = new List<Vector3>();
        vecs.Add(pontoAnterior);

        for (double t = particoes; t <= 1.0; t += particoes)
        {
            for (int i = 0; i < points[curvaAtual].Count; i++)
            {
                float bern = (float)(comb(points[curvaAtual].Count - 1, i) * Math.Pow((1.0 - t), (points[curvaAtual].Count - 1 - i)) * Math.Pow(t, i));
                pontoAtual.x += (bern * points[curvaAtual][i].transform.position.x);
                pontoAtual.y += (bern * points[curvaAtual][i].transform.position.y);
            }
            //vecs.Add(pontoAnterior);
            vecs.Add(pontoAtual);
            pontoAnterior = pontoAtual;
            pontoAtual = Vector2.zero;
        }
        vecs.Add(pontoAnterior); //desnecessário
        Vector2 pontoFinal = new Vector2(points[curvaAtual][points[curvaAtual].Count - 1].transform.position.x, points[curvaAtual][points[curvaAtual].Count - 1].transform.position.y);
        vecs.Add(pontoFinal); //desnecessário
        curva[curvaAtual].positionCount = vecs.Count;
        curva[curvaAtual].SetPositions(vecs.ToArray());
        curva[curvaAtual].enabled = visibCurvas[curvaAtual];
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
    
    public void PlgsControleIsVisible (bool visibilidade)
    {
        if(lines[curvaAtual] != null)
            lines[curvaAtual].enabled = visibilidade;
        visibPlgsControle = visibilidade;
        
        
    }

    public void CurvasIsVisible (bool visibilidade)
    {
        for(int i=0;i<curva.Count;i++)
        {
            if (curva[i] != null)
                curva[i].enabled = visibilidade;
            visibCurvas[i] = visibilidade;
        }
        visibGeralCurvas = visibilidade;
    }


    public void PtsControleIsVisible (bool visibilidade)
    {
        for (int i = 0; i < points[curvaAtual].Count; i++)
        {
            points[curvaAtual][i].GetComponent<Renderer>().enabled = visibilidade;
        }
        visibPtsControle = visibilidade;
    }
}
