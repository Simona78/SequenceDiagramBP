using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SequenceDiagram : MonoBehaviour {

    public Transform target;
    public GameObject classPrefab;
    public GameObject messagePrefab;

    private Transform classes;
    private Transform messages;

    private Dictionary<string, Transform> classesDict;

    private GameObject obj;

    private Boolean started = false;

    float[] topLeft = new float[3];
    float[] topRight = new float[3];
    float[] bottomLeft = new float[3];
    float[] bottomRight = new float[3];

    IEnumerator Test()
    {
        AddClass("test1");
        AddClass("test2");
        AddClass("test3");
        Canvas.ForceUpdateCanvases();
        AddMessage("test1", "test2", "bla()");
        AddMessage("test2", "test3", "bla()");
        AddMessage("test3", "test2", "bla()", true);
        yield return new WaitForSeconds(5);
        RemoveClass("test3");
        yield return new WaitForEndOfFrame();
        AddClass("test3");
        AddMessage("test2", "test3", "bla()", true);
    }

    private void Update()
    {
        this.refreshBounds();
    }

    private void Awake()
    {


        this.refreshInfo();



    }

    // set classes and messages prefab wrapper 
    void refreshInfo()
    {
        if (this.obj != null)
        {
            this.obj.GetComponent<CanvasGroup>().alpha = 1;

            classes = this.obj.transform.Find("ClassesHL");
            messages = this.obj.transform.Find("MessagesVL");
            Debug.Log(classes);
        }


        classesDict = new Dictionary<string, Transform>();
    }

    // Use this for initialization
    void Start () {

       
       
        //StartCoroutine(Test());

        Debug.Log(GameObject.Find("TCPService").GetComponent<TCPService>().getDiagCount());
            
        this.obj = GameObject.Find("#"+ (GameObject.Find("TCPService").GetComponent<TCPService>().getDiagCount()-1));



        this.refreshBounds();

        this.refreshInfo();
    }

    public void refreshBounds()
    {
        if (this.obj != null) {

            int spacing = 25;

            float x = this.obj.GetComponent<RectTransform>().sizeDelta.x; // get diagram width
            float y = this.obj.GetComponent<RectTransform>().sizeDelta.y; // get diagram height

            //draw lines by calculatin their positions 
            this.obj.GetComponent<LineRenderer>().SetPosition(0, new Vector3( 0,    y/2 + spacing, 0));
            this.obj.GetComponent<LineRenderer>().SetPosition(1, new Vector3(-x/2 - spacing,  y/2 + spacing, 0));
            this.obj.GetComponent<LineRenderer>().SetPosition(2, new Vector3(-x/2 - spacing, -y/2 - spacing, 0));
            this.obj.GetComponent<LineRenderer>().SetPosition(3, new Vector3( x/2 + spacing, -y/2 - spacing, 0));
            this.obj.GetComponent<LineRenderer>().SetPosition(4, new Vector3( x/2 + spacing,  y/2 + spacing, 0));
            this.obj.GetComponent<LineRenderer>().SetPosition(5, new Vector3( 0,    y/2 + spacing, 0));

        }

    }

    



    public Transform AddClass(string name)
    {
        if(!classesDict.ContainsKey(name) || classesDict[name] == null)
        {
            var cls = Instantiate(classPrefab, classes);
            int currLayer = (GameObject.Find("#" + (GameObject.Find("TCPService").GetComponent<TCPService>().getDiagCount() - 1))).layer;
            cls.layer = currLayer;
            TCPService.SetLayerRecursively(cls, currLayer);
            var tmp = cls.GetComponentInChildren<TextMeshProUGUI>();
            cls.name = tmp.text = name;
            classesDict[name] = cls.transform;
        }
        return classesDict[name];
    }

    public void AddMessage(string from, string to, string message, bool dashed=false)
    {
        var msg = Instantiate(messagePrefab, messages);
        int currLayer = (GameObject.Find("#" + (GameObject.Find("TCPService").GetComponent<TCPService>().getDiagCount() - 1))).layer;
        msg.layer = currLayer;
        TCPService.SetLayerRecursively(msg, currLayer);
        var label = msg.transform.Find("label");
        var msgScript = msg.GetComponent<Message>();
        var fromClass = AddClass(from);
        var toClass = AddClass(to);
        msgScript.fromClass = fromClass.GetComponent<RectTransform>();
        msgScript.toClass = toClass.GetComponent<RectTransform>();
        msgScript.dashed = dashed;
        label.GetComponent<TextMeshProUGUI>().text = message;
    }

    public void RemoveMessage(string from, string to)
    {
        //TODO
        //destroy
        //but need unique id and from to is not unique
        //probably will be possible from UI only
    }

    public void RemoveClass(string name)
    {
        var toRemove = classesDict[name];
        Destroy(toRemove.gameObject);
        classesDict.Remove(name);
    }

}
